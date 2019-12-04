using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontMesh : MonoBehaviour {

    Transform cam;
    public MeshRenderer mr;
    public MeshFilter mf;
    Material mat;
    public float destoryTime = 2f;
    public float curTime = 0f;
    public MaterialPropertyBlock block;
    [Range(0f,1f)]
    public float alpha = 1f;

    [System.NonSerialized]
    public SimplePool<FontMesh> pools;
    Mesh mesh;

    private void OnDestroy()
    {
        GameObject.DestroyImmediate(mesh);
    }
    // Use this for initialization
    void Start () {
        cam = Camera.main.transform;
        mat = mr.sharedMaterial;
        //mr.GetPropertyBlock(block);
        if(null != mesh)
            mf.mesh = mesh; 
        
        curTime = 0f;
    }

   
     

    // Update is called once per frame
    void Update () {
        curTime += Time.deltaTime;
        mr.material = FontMaterialManager.Instance.GetMaterial(mat, alpha);
        //Vector3 forward = cam.forward;
        //forward.y = 0;
        //block.SetFloat("_Alpha",alpha);
        //mr.SetPropertyBlock(block);
        transform.forward = -cam.forward;
        if (curTime > destoryTime)
        {
            if (pools == null)
            {
                GameObject.Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
                pools.Release(this);
            }
        }
        
    }

    public  void SetValue(int value, Dictionary<int, FontItem> fonts)
    {
        if(null ==mesh)
            mesh = new Mesh();
        buildFont(mesh, value, fonts);
    }
    static List<int> temp = new List<int>();
    static void buildFont(Mesh m, int value, Dictionary<int, FontItem> fonts)
    {
        float _width = 0f;
        float fontHeight = 65f;
        temp.Clear();
        int v;
        while (value > 0)
        {
            v = 48 + value % 10;
            value = value / 10;
            temp.Add(v);
            Debug.Log("v = " + v);
            var f = fonts[v];
            _width += f.width;
            fontHeight = f.height;
        }
        m.Clear();
        _width = -_width * 0.5f;
        int c = temp.Count;
        Vector3[] vertices = new Vector3[c * 4];
        Vector2[] uv = new Vector2[c * 4];
        int[] triangles = new int[c * 6];
        float dw = 0.5f;
        for (int i = 0; i < c; i++)
        {
            v = temp[i];
            var f = fonts[v];
            dw = f.width * 0.5f;
            vertices[i * 4 + 0] = new Vector3(-dw + _width, 0f, 0f);
            vertices[i * 4 + 1] = new Vector3(dw + _width, 0f, 0f);
            vertices[i * 4 + 2] = new Vector3(-dw + _width, fontHeight, 0f);
            vertices[i * 4 + 3] = new Vector3(dw + _width, fontHeight, 0f);




            uv[i * 4 + 0] = new Vector2(1, 0);
            uv[i * 4 + 1] = new Vector2(0, 0);
            uv[i * 4 + 2] = new Vector2(1, 1);
            uv[i * 4 + 3] = new Vector2(0, 1);

            uv[i * 4 + 0] = new Vector2(f.uv1.x, 1f - f.uv1.y);
            uv[i * 4 + 1] = new Vector2(f.uv0.x, 1f - f.uv1.y);
            uv[i * 4 + 2] = new Vector2(f.uv1.x, 1f - f.uv0.y);
            uv[i * 4 + 3] = new Vector2(f.uv0.x, 1f - f.uv0.y);
             

            triangles[i * 6 + 0] = i * 4 + 0;
            triangles[i * 6 + 1] = i * 4 + 2;
            triangles[i * 6 + 2] = i * 4 + 1;
            triangles[i * 6 + 3] = i * 4 + 1;
            triangles[i * 6 + 4] = i * 4 + 2;
            triangles[i * 6 + 5] = i * 4 + 3;
            _width += f.width;
        }


        m.vertices = vertices;
        m.uv = uv;
        m.triangles = triangles;
        m.RecalculateNormals();
    }
}
