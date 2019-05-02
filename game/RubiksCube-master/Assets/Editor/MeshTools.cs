using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshTools {


    [MenuItem("TA/Create Mesh")]
    public static void buildFurMesh( )
    {
 
		Mesh 
        mesh = new Mesh();
        mesh.vertices = new Vector3[] { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1) };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
        mesh.RecalculateNormals();
        Mesh final = mesh;
	
        string path = EditorUtility.SaveFilePanelInProject("提示", "TextureName", "asset",
                    "请输入保存文件名");
        if (path.Length != 0)
        {
            AssetDatabase.CreateAsset(final, path);
            //CreateTextureArray2D(editorArray.ToArray(), path, bMiniMap);
        }
        else
        {
            GameObject.DestroyImmediate(final, true);
        }

    }
	 
}
