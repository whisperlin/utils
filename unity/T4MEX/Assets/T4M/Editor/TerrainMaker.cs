using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public partial class TerrainMaker : EditorWindow
{
	[MenuItem("Tools/Terrain maker")]
	static public void ShowWindow()
	{
		CustomEditorUtil.GetWindow<TerrainMaker>();
	}

	float GetHighMean(float []highData)
	{
		float totalHigh = 0;
		for(int i=0;i<highData.Length;i++)
		{
			totalHigh += (float)highData[i];
		}

		return totalHigh / highData.Length;
	}

	float GetHighVariance(float[] highData, float mean, out float []highPresentData)
	{
		highPresentData = new float[highData.Length];
		float maxDiv = float.MinValue;
		for(int i=0;i<highData.Length;i++)
		{
			float high = (float)highData[i];
			float highDiv = (high - mean);
			highPresentData[i] = highDiv;

			float absHighDiv = Mathf.Abs(highDiv);
			if (absHighDiv > maxDiv)
				maxDiv = absHighDiv;
		}

		return maxDiv;
	}

	public static string TerrainModelPath = "/Art/model/Terrain/";
	public static string TerrainPrefabPath = "/Art/Prefab/Terrain/";

	string MapName = "sim_city01_1";

	// 地形设置的最大高度
	int MaxHeight = 400;
	// 地形大小
	int MapSize = 4000;
	// 地形网格
	GameObject _terrainMesh;

	// 切分出的单个网格顶点数（一行）
	int SubMeshVertexCount = 50;

	// 切分出的单个网格大小（行）
	float SubMeshPlaneSize = 100;

	// 切分网格最大高度
	float[] subMeshMaxHeightLst;

	void OnGUI()
	{
		GUILayout.Label("高度图命名为terrain.raw，高度压平图命名为terrainZeroArea.tga"  +
			"放在model/Terrain/地图名 文件夹中，工具要打开地图场景使用" ); 

		GUILayout.Label("地图名", GUILayout.Width(70));
		MapName = GUILayout.TextField(MapName);

		GUILayout.BeginHorizontal();
		GUILayout.Label("地形高度", GUILayout.Width(70));
		MaxHeight = EditorGUILayout.IntField(MaxHeight);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("地形长宽", GUILayout.Width(70));
		MapSize = EditorGUILayout.IntField(MapSize);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("地形网格", GUILayout.Width(70));
		_terrainMesh = EditorGUILayout.ObjectField(_terrainMesh, typeof(GameObject), true) as GameObject;
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("切分网格行顶点数", GUILayout.Width(170));
		SubMeshVertexCount = EditorGUILayout.IntField(SubMeshVertexCount);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("切分网格行大小", GUILayout.Width(170));
		SubMeshPlaneSize = EditorGUILayout.FloatField(SubMeshPlaneSize);
		GUILayout.EndHorizontal();

		bFixHeight = EditorGUILayout.Toggle("高度图压平", bFixHeight);        

		if (GUILayout.Button("change Texture"))
		{
			if(!SetZeroAreaData())
			{
				return;
			}
			Texture2D tex = null;
			float mean, maxdiv;
			CreateHeightMap(out tex, out mean, out maxdiv);

			Texture2D normalTex = SetMeshNormalValue();

			Texture2D tanTex = CreateTangentMap();

			CreatePreafbAndMaterial(tex, mean, maxdiv, tanTex, normalTex);
		}

		OnLayerGUI();
	}

	// 每个单元的字节长度
	static int TerrainUnitLen = 2;
	// 无符号类型的最大值
	static int NumTypeMaxValue = ushort.MaxValue;

	// 创建高度图
	private void CreateHeightMap(out Texture2D tex, out float mean, out float maxdiv)
	{
		FileStream fileStream = File.Open(Application.dataPath + TerrainModelPath + MapName+
			"/terrain.raw", FileMode.Open);

		long unitCount = fileStream.Length / TerrainUnitLen;

		int iSize = (int)Mathf.Sqrt(unitCount);
		byte[] pixels = new byte[fileStream.Length];
		float[] HighData = new float[unitCount];

		fileStream.Read(pixels, 0, (int)fileStream.Length);

		int subMeshCount = (int)Mathf.Ceil(MapSize * 1f / SubMeshPlaneSize);
		subMeshMaxHeightLst = new float[subMeshCount * subMeshCount];

		float maxValue = float.MinValue;
		float minValue = float.MaxValue;
		for (int i = 0; i < fileStream.Length; i += TerrainUnitLen)
		{
			float unitValue;
			if (TerrainUnitLen == 1)
			{
				unitValue = pixels[i];
			}
			else
			{
				unitValue = System.BitConverter.ToUInt16(pixels, i);
			}

			unitValue = (unitValue * 1.0f / NumTypeMaxValue * MaxHeight);

			if (unitValue < minValue)
			{
				minValue = unitValue;
			}
			if (unitValue > maxValue)
			{
				maxValue = unitValue;
			}

			int index = i / TerrainUnitLen;
			int x = index % iSize;
			int y = index / iSize;

			// 地形压平
			unitValue = FixZeroAreaValue(x * 1.0f / iSize, y * 1.0f / iSize, unitValue);

			HighData[index] = unitValue;

			x = (int)( x * 1f / iSize * subMeshCount);
			y = (int)(y * 1f / iSize * subMeshCount);
			int subIndex = y * subMeshCount + x;
			if(unitValue > subMeshMaxHeightLst[subIndex])
			{
				subMeshMaxHeightLst[subIndex] = unitValue;
			}
		}

		Debug.Log("minHeight = " + minValue + " maxHeight" + maxValue);

		mean = GetHighMean(HighData);
		float[] standard_div_high = null;
		maxdiv = GetHighVariance(HighData, mean, out standard_div_high);

		tex = new Texture2D(iSize, iSize, TextureFormat.RGBA32, false, true);
		for (int i = 0, c = standard_div_high.Length; i < c; i++)
		{
			int x = i % iSize;
			int y = i / iSize;

			float diffValue = standard_div_high[i] / maxdiv * 0.5f + 0.5f;

			// 方差值存在RGBA通道
			Vector4 vecValue = EncodeFloatRGBA(diffValue);
			Color col = vecValue;

			//取出高度公式(colValue - 0.5) * 2 * heghtMaxDiv + heightMean
			tex.SetPixel(x, y, col);
		}

		fileStream.Close();
	}

	// T4M生成的网格顶点是从左下角到右上角按列排列
	private Texture2D SetMeshNormalValue()
	{
		MeshFilter mf = _terrainMesh.GetComponentInChildren<MeshFilter>();
		Vector3[] normalArr = mf.sharedMesh.normals;

		int normalWidth = (int)Mathf.Sqrt(normalArr.Length);

		Texture2D tex = new Texture2D(normalWidth, normalWidth, TextureFormat.RGBA32, false, true);

		float ratio = normalWidth * 1.0f / tex.width;

		for (int x = 0; x < tex.width; x ++)
		{
			for(int y = 0; y < tex.height; y++)
			{
				Color col = tex.GetPixel(x, y);

				int indexX = (int)(x* ratio);
				int indexY = (int)(y * ratio);

				int nlIndex = indexY +  indexX* normalWidth ;

				Vector3 nl3 = normalArr[nlIndex] * 0.5f + new Vector3(0.5f, 0.5f, 0.5f);
				col = new Color(nl3.x, nl3.y, nl3.z, 1);

				tex.SetPixel(x, y, col);
			}
		}
		return tex;
	}

	// T4M生成的网格顶点是从左下角到右上角按列排列
	private Texture2D CreateTangentMap()
	{
		MeshFilter mf = _terrainMesh.GetComponentInChildren<MeshFilter>();
		Vector4[] tangentArr = mf.sharedMesh.tangents;
		int tangentWidth = (int)Mathf.Sqrt(tangentArr.Length);
		Texture2D tex = new Texture2D(tangentWidth, tangentWidth, TextureFormat.RGBA32, false, true);
		for(int i = 0; i < tangentArr.Length; i++)
		{
			Vector4 tanValue = tangentArr[i] * 0.5f + new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
			tex.SetPixel(i / tangentWidth, i % tangentWidth, tanValue);
		}

		return tex;
	}


	Vector2 EncodeFloatRG(float v)
	{
		Vector2 kEncodeMul = new Vector2(1.0f, 255.0f);
		float kEncodeBit = 1.0f / 255.0f;
		Vector2 enc = kEncodeMul * v;
		enc = new Vector2(enc.x % 1.0f, enc.y % 1.0f);
		enc.x -= enc.y * kEncodeBit;
		return enc;
	}

	Vector4 EncodeFloatRGBA(float v)
	{
		Vector4 kEncodeMul = new Vector4(1.0f, 255.0f, 65025.0f, 16581375.0f);
		float kEncodeBit = 1.0f / 255.0f;
		Vector4 enc = kEncodeMul * v;
		enc = new Vector4(enc.x % 1.0f, enc.y % 1.0f, enc.z % 1.0f, enc.w % 1.0f);
		enc -= new Vector4(enc.y, enc.z, enc.w, enc.w) * kEncodeBit;
		return enc;
	}

	Vector2 EncodeViewNormalStereo(Vector3 n)
	{
		float kScale = 1.7777f;
		Vector2 enc;
		enc = new Vector2(n.x, n.y) / (n.z + 1.0f);
		enc /= kScale;
		enc = enc * 0.5f + new Vector2(0.5f, 0.5f);
		return enc;
	}

	private void CreatePreafbAndMaterial(Texture2D texHeight, float mean, float maxDiv, 
		Texture2D texTan, Texture2D texLocalNormal)
	{
		string heightMapPath = TerrainModelPath + MapName  + "/terrainHeight.png";
		string tangentMapPath = TerrainModelPath + MapName + "/terrainTangent.png";
		string localNormalMapPath = TerrainModelPath + MapName + "/terrainLocalNormal.bmp";

		Texture2D texHeightMap = SaveTexture2D(texHeight, heightMapPath, false, false);
		Texture2D texTangentMap = SaveTexture2D(texTan, tangentMapPath, true, false);
		Texture2D texLocalNormalMap = SaveTexture2D(texLocalNormal, localNormalMapPath, true, true);

		string normalMapPath ="Assets" + TerrainModelPath + MapName + "/terrainNormal.bmp";
		var normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalMapPath);

		string materialPath = GetMaterialPath(1);

		Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
		if( mat == null)
		{
			mat = new Material(Shader.Find("DAFUHAO_Editor/Terrain-HeightMap-Base"));
			AssetDatabase.CreateAsset(mat, materialPath);
		}
		mat.SetTexture("_TerrainHeightMap", texHeightMap);
		mat.SetTexture("_TerrainNormalMap", normalTex);
		mat.SetTexture("_TerrainTangentMap", texTangentMap);
		mat.SetTexture("_TerrainLocalNormalMap", texLocalNormalMap);
		mat.SetFloat("_TerrainHeightMean", mean);
		mat.SetFloat("_TerrainHeightMaxDiv", maxDiv);
		mat.SetFloat("_TerrainMapSize", MapSize);
		EditorUtility.SetDirty(mat);

		int MaterialCount = 8;
		Material[] matArray = new Material[MaterialCount];
		matArray[0] = mat;

		for (int i = 1; i < MaterialCount; i++)
		{
			matArray[i] = new Material(mat);

			string copyPath = GetMaterialPath(i + 1);
			AssetDatabase.DeleteAsset(copyPath);
			AssetDatabase.CreateAsset(matArray[i], GetMaterialPath(i + 1));            
		}

		GameObject terrainDataObj = CreateTerrainMeshObjs(matArray);

		string prefabPath = "Assets" + TerrainPrefabPath + MapName + "/TerrainData.prefab";
		GameObject tempPrefab = PrefabUtility.CreatePrefab(prefabPath, terrainDataObj, 
			ReplacePrefabOptions.ConnectToPrefab);

		EditorUtility.SetDirty(tempPrefab);
		//GameObject.DestroyImmediate(terrainDataObj);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}



	private Texture2D SaveTexture2D(Texture2D tex, string path, bool bCompress, bool bNormalMap)
	{
		string texPath = Application.dataPath + path;
		if (File.Exists(texPath))
		{
			AssetDatabase.DeleteAsset("Assets" + path);
			//File.Delete(texPath);
		}
		FileStream fs = File.Create(texPath);
		fs.Close();

		AssetDatabase.Refresh();

		byte[] jpgBytes = tex.EncodeToPNG();
		File.WriteAllBytes(texPath, jpgBytes);

		var impSetting = UnityEditor.TextureImporter.GetAtPath("Assets" + path) as TextureImporter;
		if (impSetting != null)
		{
			if (bNormalMap)
			{
				impSetting.textureType = TextureImporterType.NormalMap;
			}
			else
			{
				impSetting.textureType = TextureImporterType.Default;
			}
			impSetting.mipmapEnabled = false;
			impSetting.alphaIsTransparency = false;
			impSetting.maxTextureSize = 1024;
			impSetting.filterMode = FilterMode.Point;

			if (bCompress)
			{
				impSetting.textureCompression = TextureImporterCompression.CompressedHQ;
			}
			else
			{
				impSetting.textureCompression = TextureImporterCompression.Uncompressed;
			}

			impSetting.SaveAndReimport();
		}

		return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + path);
	}

 
	static Mesh CreatePlaneMesh(int vertexCountX, float lenX, int vertexCountZ, float lenZ, float yValue = 0f)
	{
		Mesh mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

		int vectorPoints = vertexCountX * vertexCountZ;
		Vector3[] meshVertice = new Vector3[vectorPoints];
		Vector2[] meshUV = new Vector2[vectorPoints];
		Vector3[] meshNormals = new Vector3[vectorPoints];

		float zStep = lenZ / (vertexCountZ - 1);
		float xStep = lenX / (vertexCountX - 1);

		float tz = 1.0f / (vertexCountZ - 1);
		float tx = 1.0f / (vertexCountX - 1);

		for (int z = 0; z < vertexCountZ; z++) 
		{
			for (int x = 0; x < vertexCountX; x++)
			{
				float posZ = lenX / 2.0f - zStep * z ;
				float posX = xStep * x - lenX / 2.0f;

				int index = z * vertexCountX + x;
				meshVertice[index] = new Vector3() { x = posX, y = yValue, z = posZ };
				// uv设置
				//(0,1) (1,1)
				//(0,0) (1,0)
				meshUV[index] = new Vector2() { x = tx * x, y =1 - tz * z };
			}
		}

		mesh.SetVertices(new List<Vector3>(meshVertice));
		mesh.SetUVs(0, new List<Vector2>(meshUV));

		int triCount = (vertexCountX - 1) * (vertexCountZ - 1) * 6;
		int[] triangles = new int[triCount];

		for (int z = 0; z < vertexCountZ - 1; z++) 
		{
			for (int x = 0; x < vertexCountX - 1; x++)
			{
				int currIndex = z * vertexCountX + x;
				int nextLineIndex = (z + 1) * vertexCountX + x;

				int trgIndex = (z * (vertexCountX - 1) + x ) * 6;

				triangles[trgIndex] = currIndex;
				triangles[trgIndex + 1] = nextLineIndex + 1;
				triangles[trgIndex + 2] = nextLineIndex;

				triangles[trgIndex + 3] = currIndex;
				triangles[trgIndex + 4] = currIndex + 1;
				triangles[trgIndex + 5] = nextLineIndex + 1;
			}
		}

		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		return mesh;
	}
	private GameObject CreateTerrainMeshObjs(Material[] matArray)
	{
		GameObject terrainDataObj = new GameObject("TerrainData");
		TerrainCustomData terrData = terrainDataObj.GetComponent<TerrainCustomData> ();
		if (null == terrData)
			terrData = terrainDataObj.AddComponent<TerrainCustomData> ();
		//TerrainCustomData terrData = Utility.GetOrAddComponent<TerrainCustomData>(terrainDataObj);
		terrData.LocalToWorldMatrix = Matrix4x4.identity;
		terrData.MaterialArray = matArray;

		Mesh mesh = CreatePlaneMesh(SubMeshVertexCount, SubMeshPlaneSize, 
			SubMeshVertexCount, SubMeshPlaneSize);
		string meshPath = "Assets" + TerrainModelPath + MapName + "/terrainMesh.asset";
		AssetDatabase.CreateAsset(mesh, meshPath);

		int meshCount = (int)Mathf.Ceil(MapSize * 1.0f / SubMeshPlaneSize);

		for (int z = 0; z < meshCount; z++) 
		{
			for (int x = 0; x < meshCount; x++)
			{
				GameObject terrObj = new GameObject(string.Format("Mesh_{0}_{1}", x, z));

				terrObj.transform.SetParent(terrainDataObj.transform);
				terrObj.transform.localScale = Vector3.one;
				terrObj.transform.localRotation = Quaternion.identity;
				Vector3 localPos = new Vector3((x + 0.5f) * SubMeshPlaneSize, 0f,
					(z + 0.5f) * SubMeshPlaneSize);

				terrObj.transform.localPosition = localPos;

				MeshFilter mf = terrObj.AddComponent<MeshFilter>();
				mf.sharedMesh = mesh;

				MeshRenderer md = terrObj.AddComponent<MeshRenderer>();
				md.sharedMaterial = matArray[0];

				// 设置GpuInc属性
				var terrProp = terrObj.AddComponent<TerrainMeshPropertyCom>();
				terrProp.LocalPosition = new Vector4(localPos.x, localPos.y, localPos.z, 0);

				// 获取该地形最高高度
				int index = (int)(z * meshCount + x);
				if(index >= subMeshMaxHeightLst.Length)
				{
					Debug.LogError("index = " + index +" x= " + x  + " z = " + z + " len = " + subMeshMaxHeightLst.Length);
				}
				float height = subMeshMaxHeightLst[index];

				height = Mathf.Max(height, 1f);

				// 设置Bounds
				Vector3 center = mesh.bounds.center;
				center.y = height / 2.0f;
				Vector3 size = mesh.bounds.size;
				size.y = height;
				terrProp.mSourceBounds = new Bounds(center, size);
			}
		}

		return terrainDataObj;
	}

	// 压平区域的高度
	private float zeroAreaHeight;
	// 是不是要压平中间区域
	private bool bFixHeight = true;

	Color[] zeroAreaCols;
	int zeroAreaSize;

	// 设置压平区域的数据
	private bool SetZeroAreaData()
	{
		if(!bFixHeight)
		{
			return true;
		}

		T4MObjSC t4mObj = GameObject.FindObjectOfType<T4MObjSC>();
		if (t4mObj == null)
		{
			Debug.LogError("can not find T4MObjSC");
			return false;
		}

		Vector3 mapPos = t4mObj.transform.position;
		zeroAreaHeight = -mapPos.y;

		string texPath = "Assets" + TerrainModelPath + MapName + "/terrainZeroArea.tga";

		Texture2D terrainZeroAreaTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
		if(terrainZeroAreaTex == null)
		{
			Debug.LogError("terrainZeroArea.tga == null where "+texPath+"not found");
			return false;
		}
		zeroAreaCols = terrainZeroAreaTex.GetPixels();
		zeroAreaSize = (int)Mathf.Sqrt(zeroAreaCols.Length);
		return true;
	}

	// 是不是在要压平的区域
	private float FixZeroAreaValue(float precentX, float precentY, float unitValue)
	{
		if(!bFixHeight)
		{
			return unitValue;
		}
		int xPos = (int)(precentX* zeroAreaSize);
		int yPos = (int)(precentY * zeroAreaSize);

		int index = (int)(yPos * zeroAreaSize + xPos);
		Color pixel = zeroAreaCols[index];

		return Mathf.Lerp(unitValue, zeroAreaHeight,  pixel.a);
	}

	private string GetMaterialPath(int index)
	{
		return "Assets" + TerrainModelPath + MapName + "/terrainMat" + index + ".mat"; 
	}

}
