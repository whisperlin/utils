using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class DirectMesh  {

    static void CircularRing(Mesh m, float radius0, float radius1)
    {
        List<Vector3> vectors = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indexs = new List<int>();

        float deltaA = Mathf.PI / 180f;
        float _x0 = 0f;
        float _z0 = 0f;
        float _x1 = 0f;
        float _z1 = 0f;

        Vector3 _p0;
        Vector3 _p1;
        Vector3 p0;
        Vector3 p1;

        for (int i = 0; i < 361; i++)
        {
            float a = deltaA * i;
            float x = Mathf.Sin(a);
            float z = Mathf.Cos(a);

            float x0 = x * radius0;
            float z0 = z * radius0;

            float x1 = x * radius1;
            float z1 = z * radius1;

            if (i > 0)
            {
                _p0 = new Vector3(_x0, 0f, _z0);
                _p1 = new Vector3(_x1, 0f, _z1);

                p0 = new Vector3(x0, 0f, z0);
                p1 = new Vector3(x1, 0f, z1);

                vectors.Add(_p0);
                vectors.Add(_p1);
                vectors.Add( p0);
                vectors.Add( p1);
                uvs.Add(new Vector2(0.5f, 1f));
                uvs.Add(new Vector2(0.5f,0f));
                uvs.Add(new Vector2(0.5f, 1f));
                uvs.Add(new Vector2(0.5f, 0f));
               
                int index = i - 1;
                indexs.Add(1 + index*4);
                indexs.Add(0 + index * 4);
                indexs.Add(2 + index * 4);
                indexs.Add(1 + index * 4);
                indexs.Add(2 + index * 4);
                indexs.Add(3 + index * 4);

            }

            _x0 = x0;
            _z0 = z0;
            _x1 = x1;
            _z1 = z1;
        }
        Debug.LogError("max = "+ indexs[indexs.Count-1]);
        m.vertices = vectors.ToArray();
        m.uv = uvs.ToArray();
        m.uv2 = uvs.ToArray();
        m.triangles = indexs.ToArray();
        m.RecalculateNormals();
    }

    static void BuildDirCtrl(Mesh m ,float width, float length)
    {

        if (width >= length)
        {
            float dw = width * 0.5f;
            m.vertices = new Vector3[] {
                    new Vector3(-dw, 0, 0), new Vector3(0, 0, 0), new Vector3(-dw, 0, length), new Vector3(0, 0, length) ,
                    new Vector3(0, 0, 0), new Vector3(dw, 0, 0), new Vector3(0, 0, length), new Vector3(dw, 0, length) };
            m.uv = new Vector2[] {
                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1),
                    new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1)};
            m.uv2 = m.uv;
            m.triangles = new int[] {
                    0, 2, 1, 1, 2, 3,
                    4, 6, 5, 5, 6, 7
                };
            m.RecalculateNormals();
        }
        else
        {
            float undraw = 0.1f;
            float dw = width * 0.5f;
            float l1 = length-width * 1- (undraw) ;
            float l2 = 0f;

            m.vertices = new Vector3[] {
                    new Vector3(-dw, 0, l1), new Vector3(0, 0, l1), new Vector3(-dw, 0, length), new Vector3(0, 0, length) ,
                    new Vector3(0, 0, l1), new Vector3(dw, 0, l1), new Vector3(0, 0, length), new Vector3(dw, 0, length),

                     new Vector3(-dw, 0, l2), new Vector3(0, 0, l2), new Vector3(-dw, 0, l1), new Vector3(0, 0, l1) ,
                    new Vector3(0, 0, l2), new Vector3(dw, 0, l2), new Vector3(0, 0, l1), new Vector3(dw, 0, l1)


            };
            m.uv = new Vector2[] {
                    new Vector2(0, undraw), new Vector2(1, undraw), new Vector2(0, 1), new Vector2(1, 1),
                    new Vector2(1, undraw), new Vector2(0, undraw), new Vector2(1, 1), new Vector2(0, 1),

                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, undraw), new Vector2(1, undraw),
                    new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, undraw), new Vector2(0, undraw)

            };
            m.uv2 = m.uv;
            m.triangles = new int[] {
                    0, 2, 1, 1, 2, 3,
                    4, 6, 5, 5, 6, 7,
                    8, 10, 9, 9, 10, 11,
                    12, 14, 13, 13, 14, 15
                };
            m.RecalculateNormals();
        }
        
    }
    [MenuItem("Test/Mesh")]
    static void Start1()
    {
        if (Selection.objects.Length == 1)
        {
            var obj = Selection.objects[0];
            Debug.LogError(obj);
            Mesh m = (Mesh)obj;
            if (null != m)
            {
                CircularRing(m, 1f, 0.8f);
                //BuildDirCtrl(m, 1f, 3f);
                //AssetDatabase.SaveAssets();
            }
        }
        
        
    }
	static void Start () {
        Mesh m;
        MeshRenderer planeRender;
        m = new Mesh();
        m.vertices = new Vector3[] { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1) };
        m.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
        m.uv2 = m.uv;
        m.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
        m.RecalculateNormals();
        AssetDatabase.CreateAsset(m, "Assets/myasset.asset"); ;

        GameObject g = new GameObject();
        g.name = "BakeObj";
        g.AddComponent<MeshFilter>().sharedMesh = m;
        planeRender = g.AddComponent<MeshRenderer>();
    }
	
	 
}
