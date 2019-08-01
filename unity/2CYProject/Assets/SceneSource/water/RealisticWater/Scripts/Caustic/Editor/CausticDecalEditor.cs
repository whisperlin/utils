using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(CausticDecal))]
public class CausticDecalEditor : Editor {	
	
  private GameObject[] affectedObjects;
  private Terrain terrainObject;
  private TerrainData terrainData;
  private Vector3 terrainPos;
  private float terrainOffset = 0.001f;

  private void InitTerrain()
  {
    terrainObject = Selection.activeObject as Terrain;
    if (!terrainObject) {
      terrainObject = Terrain.activeTerrain;
    }
    if (!GameObject.Find("TempTerrainMesh") && terrainObject!=null) {
      if (terrainObject) {
        terrainData = terrainObject.terrainData;
        terrainPos = terrainObject.transform.position;
      }
      CreateTerrain();
    }
  }

  void CreateTerrain()
  {
    int w = terrainData.heightmapWidth;
    int h = terrainData.heightmapHeight;
    Vector3 meshScale = terrainData.size;
    int tRes = (int)Mathf.Pow(2, 4);
    meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
    Vector2 uvScale = new Vector2(1.0f / (w - 1), 1.0f / (h - 1));
    float[,] tData = terrainData.GetHeights(0, 0, w, h);

    w = (w - 1) / tRes + 1;
    h = (h - 1) / tRes + 1;
    Vector3[] tVertices = new Vector3[w * h];
    Vector2[] tUV = new Vector2[w * h];

    int[] tPolys;

    tPolys = new int[(w - 1) * (h - 1) * 6];

    // Build vertices and UVs
    for (int y = 0; y < h; y++)
    {
      for (int x = 0; x < w; x++)
      {
        tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(-y, tData[x * tRes, y * tRes], x)) + terrainPos;
        tUV[y * w + x] = Vector2.Scale(new Vector2(x * tRes, y * tRes), uvScale);
      }
    }

    int index = 0;
    // Build triangle indices: 3 indices into vertex array for each triangle
      for (int y = 0; y < h - 1; y++)
      {
        for (int x = 0; x < w - 1; x++)
        {
          // For each grid cell output two triangles
          tPolys[index++] = (y * w) + x;
          tPolys[index++] = ((y + 1) * w) + x;
          tPolys[index++] = (y * w) + x + 1;

          tPolys[index++] = ((y + 1) * w) + x;
          tPolys[index++] = ((y + 1) * w) + x + 1;
          tPolys[index++] = (y * w) + x + 1;
        }
      }

    var terrainGO = new GameObject("TempTerrainMesh");
    var mesh = new Mesh();
    terrainGO.AddComponent<MeshFilter>().mesh = mesh;
    terrainGO.AddComponent<MeshRenderer>();
    mesh.vertices = tVertices;
    mesh.uv = tUV;
    mesh.triangles = tPolys;

    Vector3[] normals = mesh.normals;
    for (int i = 0; i < normals.Length; i++)
      normals[i] = -normals[i];
    mesh.normals = normals;

    for (int m = 0; m < mesh.subMeshCount; m++)
    {
      int[] triangles = mesh.GetTriangles(m);
      for (int i = 0; i < triangles.Length; i += 3)
      {
        int temp = triangles[i + 0];
        triangles[i + 0] = triangles[i + 1];
        triangles[i + 1] = temp;
      }
      mesh.SetTriangles(triangles, m);
    }
    terrainGO.transform.parent = terrainObject.transform;
    terrainGO.transform.localScale = new Vector3(-1, 1, 1);
    var oldTerrPos = terrainObject.transform.position;
    terrainGO.transform.localPosition = new Vector3(oldTerrPos.x, terrainOffset, - oldTerrPos.z); 
    mesh.RecalculateBounds();
    mesh.RecalculateNormals();
  }

  public override void OnInspectorGUI() {

    var decal = (CausticDecal)target;
    terrainOffset = EditorGUILayout.FloatField("Push Distance", terrainOffset);
    terrainOffset = Mathf.Clamp(terrainOffset, 0.001f, 1);
    decal.affectedLayers = LayerMaskField("Affected Layers", decal.affectedLayers);
    EditorGUILayout.Separator();
    if (GUILayout.Button("Update Decal")) {
      InitTerrain();
      BuildDecal(decal);
      var terr = GameObject.Find("TempTerrainMesh");
      if(terr!=null) DestroyImmediate(terr);
    }
    EditorGUILayout.Separator();
	}

	private static LayerMask LayerMaskField(string label, LayerMask mask) {
		List<string> layers = new List<string>();
		for(int i=0; i<32; i++) {
			string name = LayerMask.LayerToName(i);
			if(name != "") layers.Add( name );
		}
		return EditorGUILayout.MaskField( label, mask, layers.ToArray() );
	}

	private static bool IsLayerContains(LayerMask mask, int layer) {
		return (mask.value & 1<<layer) != 0;
	}
	
	
	private void BuildDecal(CausticDecal decal) {
		MeshFilter filter = decal.GetComponent<MeshFilter>();
		if(filter == null) filter = decal.gameObject.AddComponent<MeshFilter>();
		if(decal.GetComponent<Renderer>() == null) decal.gameObject.AddComponent<MeshRenderer>();
		affectedObjects = GetAffectedObjects(decal.GetBounds(), decal.affectedLayers);
		foreach(GameObject go in affectedObjects) {
			CausticDecalBuilder.BuildDecalForObject( decal, go );
		}
		CausticDecalBuilder.Push( decal.pushDistance );

		Mesh mesh = CausticDecalBuilder.CreateMesh();
		if(mesh != null) {
			mesh.name = "CausticMesh";
			filter.mesh = mesh;
		}
	}

	private static GameObject[] GetAffectedObjects(Bounds bounds, LayerMask affectedLayers) {
		MeshRenderer[] renderers = (MeshRenderer[]) GameObject.FindObjectsOfType<MeshRenderer>();
		List<GameObject> objects = new List<GameObject>();
		foreach(Renderer r in renderers) {
			if( !r.enabled ) continue;
			if( !IsLayerContains(affectedLayers, r.gameObject.layer) ) continue;
			if( r.GetComponent<CausticDecal>() != null ) continue;
			
			if( bounds.Intersects(r.bounds) ) {
				objects.Add(r.gameObject);
			}
		}
		return objects.ToArray();
	}


	
}