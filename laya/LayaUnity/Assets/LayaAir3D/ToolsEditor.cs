using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.AI;

public class ToolsEditor
{

    [MenuItem("Tools/Export NavMesh")]
    public static void ExportNavMesh()
    {
        Debug.Log("Export NavMesh");

        //Unity2017 API
        UnityEngine.AI.NavMeshTriangulation navMeshTriangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();
        string sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;

        //新建文件  
        string savePath = Application.dataPath + "/" + sceneName + "_NavMesh.obj";
        StreamWriter sw = new StreamWriter(savePath);

        //顶点  
        for (int i = 0; i < navMeshTriangulation.vertices.Length; i++)
        {
            sw.WriteLine("v  " + navMeshTriangulation.vertices[i].x + " " + navMeshTriangulation.vertices[i].y + " " + navMeshTriangulation.vertices[i].z);
        }

        sw.WriteLine("g navmesh");//组名称

        //索引  
        for (int i = 0; i < navMeshTriangulation.indices.Length;)
        {
            //obj文件中顶点索引是从1开始
            sw.WriteLine("f " + (navMeshTriangulation.indices[i] + 1) + " " + (navMeshTriangulation.indices[i + 1] + 1) + " " + (navMeshTriangulation.indices[i + 2] + 1));
            i = i + 3;
        }

        sw.Flush();
        sw.Close();

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        Debug.Log(string.Format("Verts:{0}  Tris:{1}", navMeshTriangulation.vertices.Length, navMeshTriangulation.indices.Length / 3));
        Debug.Log(savePath);
        Debug.Log("ExportNavMesh Success");
    }
}