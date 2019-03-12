using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FurTool : EditorWindow {


    public enum OPTIONS
    {
        Mesh = 0,
        Panel = 1,
        MeshFilter = 2,
        SkinnedMeshRenderer = 3
    }
    OPTIONS op;
    MeshFilter _meshFilder  ;
    SkinnedMeshRenderer _smr    ;
    Mesh _mesh;
    int LayarCount = 10;
    void OnGUI()
    {
        op = (OPTIONS)EditorGUILayout.EnumPopup("类型:", op);
        
        if (op == OPTIONS.Mesh)
        {
            _mesh = EditorGUILayout.ObjectField(_mesh, typeof(Mesh), true) as Mesh;
        }
        if (op == OPTIONS.Panel)
        {
            _mesh = null;
        }
        if (op == OPTIONS.MeshFilter)
        {
            _meshFilder = EditorGUILayout.ObjectField(_meshFilder, typeof(MeshFilter), true) as MeshFilter;
        }
        if (op == OPTIONS.SkinnedMeshRenderer)
        {
            _smr = EditorGUILayout.ObjectField(_smr, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
        }
        LayarCount = EditorGUILayout.IntField("层数:", LayarCount);
        if (GUILayout.Button("确定"))
        {
            buildFurMesh(_mesh, LayarCount);
            GameObject.DestroyImmediate(this,true);
        }   
        
    }

    [MenuItem("TA/Fur Mesh")]
    public static void ShowWindow()
    {
        FurTool window = (FurTool)EditorWindow.GetWindow(typeof(FurTool));
        window.Show();
    }
    public static void buildFurMesh(Mesh mesh,int count)
    {
 
        if (null == mesh)
        {
            mesh = new Mesh();
            mesh.vertices = new Vector3[] { new Vector3(-10, 0, -10), new Vector3(10, 0, -10), new Vector3(-10, 0, 10), new Vector3(10, 0, 10) };
            mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
            mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
            mesh.RecalculateNormals();

        }
        Vector3[] vs =  mesh.vertices;
        int vsCount = vs.Length;
        int [] ts = mesh.triangles;
        Vector2[] uv = mesh.uv;
        Vector2[] uv2 = mesh.uv2;
        Vector3[] normals = mesh.normals;
     
 
        CombineInstance[] combine = new CombineInstance[count];
        for (int i = 0; i < count; i++)
        {
            Mesh m = new Mesh();
           
            m.vertices = vs;
            m.uv = uv;
            if(null != uv2 && uv2.Length>0)
                m.uv2 = uv2;
            if (null != normals && normals.Length > 0)
                m.normals = normals;
            Color[] col = new Color[vsCount];
            Color _c = new Color(1, 1, 1- ((float)i) / count, ((float)i)/ count);
            for (int j = 0; j < vsCount; j++)
            {
                col[j] = _c;
            }
            m.triangles = ts;
            m.colors = col;
            m.RecalculateNormals();
            combine[i].mesh = m;
            combine[i].transform =  Matrix4x4.identity;
        }
        Mesh final = new Mesh();
        final.CombineMeshes(combine);
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
