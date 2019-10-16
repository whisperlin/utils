using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;

public class DirectMesh  {


    static void CreateRound(Mesh m, float radius0 ,int age0 ,int age = 360)
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

        for (int i = age0,i1=-1; i <= age; i++,i1++)
        {
            float a = deltaA * i;
            float x = Mathf.Sin(a);
            float z = Mathf.Cos(a);

            float x0 = x * radius0;
            float z0 = z * radius0;

            float x1 = 0;
            float z1 = 0;

            if (i > age0)
            {
                _p0 = new Vector3(_x0, 0f, _z0);
                _p1 = new Vector3(_x1, 0f, _z1);

                p0 = new Vector3(x0, 0f, z0);
                p1 = new Vector3(x1, 0f, z1);

                vectors.Add(_p0);
                vectors.Add(_p1);
                vectors.Add(p0);
                //vectors.Add(p1);
                uvs.Add(new Vector2(_x0+0.5f, _z0));
                uvs.Add(new Vector2(_x1+0.5f, _z1));
                uvs.Add(new Vector2(x0+0.5f, z0));
                //uvs.Add(new Vector2(0.5f, 0f));

                int index = i - 1;
                indexs.Add(1 + i1 * 3);
                indexs.Add(0 + i1 * 3);
                indexs.Add(2 + i1 * 3);
                //indexs.Add(1 + index * 3);
                //indexs.Add(2 + index * 3);
                //indexs.Add(3 + index * 3);

            }

            _x0 = x0;
            _z0 = z0;
            _x1 = x1;
            _z1 = z1;
        }
        
        m.vertices = vectors.ToArray();
        m.uv = uvs.ToArray();
        m.uv2 = uvs.ToArray();
        m.triangles = indexs.ToArray();
        m.RecalculateNormals();
    }

    static void CircularRing(Mesh m, float radius0, float radius1,int a0,int a1,int head = 3 )
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
        int index = -1;
        for (int i0 = a0,i = 0; i0 <= a1  ; i0++, i++)
        {
            float a = deltaA * i0;
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
                vectors.Add(p0);
                vectors.Add(p1);
                float u = ((float)i0) /a0;
                if (u > 0)
                {
                    u = 1.0f - u;
                }
                uvs.Add(new Vector2(u, 0.9f));
                uvs.Add(new Vector2(u, 0.5f));
                uvs.Add(new Vector2(u, 0.9f));
                uvs.Add(new Vector2(u, 0.5f));

                index = i - 1;
                indexs.Add(1 + index * 4);
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

        {
            float a = deltaA * -head;
            float x = Mathf.Sin(a);
 
            float x0 = x * radius0;
            float z0 = radius0;
            float x1 = x0;
            float z1 = 0;

            _x0 = x0;
            _z0 = z0;
            _x1 = x1;
            _z1 = z1;

            x0 = 0;
            z0 = radius0;
            x1 = 0f;
            z1 = 0f;

            _p0 = new Vector3(_x0, 0f, _z0);
            _p1 = new Vector3(_x1, 0f, _z1);

            p0 = new Vector3(x0, 0f, z0);
            p1 = new Vector3(x1, 0f, z1);

            vectors.Add(_p0);
            vectors.Add(_p1);
            vectors.Add(p0);
            vectors.Add(p1);

            uvs.Add(new Vector2(0.04f, 0.1f));
            uvs.Add(new Vector2(0.85f, 0.1f));
            uvs.Add(new Vector2(0.04f, 0.0f));
            uvs.Add(new Vector2(0.85f, 0.0f));

            index ++;
            indexs.Add(1 + index * 4);
            indexs.Add(0 + index * 4);
            indexs.Add(2 + index * 4);
            indexs.Add(1 + index * 4);
            indexs.Add(2 + index * 4);
            indexs.Add(3 + index * 4);



             

            _x0 = x0;
            _z0 = z0;
            _x1 = x1;
            _z1 = z1;


            x0 = -x * radius0;
            z0 = radius0;
            x1 = x0;
            z1 = 0;

            _p0 = new Vector3(_x0, 0f, _z0);
            _p1 = new Vector3(_x1, 0f, _z1);

            p0 = new Vector3(x0, 0f, z0);
            p1 = new Vector3(x1, 0f, z1);

            vectors.Add(_p0);
            vectors.Add(_p1);
            vectors.Add(p0);
            vectors.Add(p1);

            uvs.Add(new Vector2(0.04f, 0.0f));
            uvs.Add(new Vector2(0.85f, 0.0f));
            uvs.Add(new Vector2(0.04f, 0.1f));
            uvs.Add(new Vector2(0.85f, 0.1f));

            index++;
            indexs.Add(1 + index * 4);
            indexs.Add(0 + index * 4);
            indexs.Add(2 + index * 4);
            indexs.Add(1 + index * 4);
            indexs.Add(2 + index * 4);
            indexs.Add(3 + index * 4);

        }

        m.Clear();
        m.vertices = vectors.ToArray();
        m.uv = uvs.ToArray();
        m.uv2 = uvs.ToArray();
        m.triangles = indexs.ToArray();
        m.RecalculateNormals();
    }
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

        for (int i = 0; i <= 360; i++)
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


    static void BuildRect(Mesh m  )
    {
        m.Clear();
        float dw =   0.5f;
        m.vertices = new Vector3[] {
                    new Vector3(-dw, 0, -dw), new Vector3(dw, 0, -dw), new Vector3(-dw, 0, dw), new Vector3(dw, 0, dw)
        };

        m.uv = new Vector2[] {
                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
        };
                     
        m.triangles = new int[] {
                    0, 2, 1, 1, 2, 3 
                };
        m.RecalculateNormals();

    }
    
     
    [MenuItem("Test/Mesh")]
    static void Start1()
    {
        /*if (Selection.objects.Length == 1)
        {
            var obj = Selection.objects[0];
            
            Mesh m = (Mesh)obj;
            if (null != m)
            {
                BuildRect(m);
                //CreateRound(m, 1f,-45, 45);
                //CircularRing(m, 1f, 0.8f);
                //CircularRing(m, 1f, 0.5f,-45,45);
                //BuildDirCtrl(m, 1f, 3f);
                //AssetDatabase.SaveAssets();
            }
        }*/
        

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
