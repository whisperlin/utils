using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

namespace yw
{
	public class CombineDymMeshManager :MonoBehaviour 
    {
        private static CombineDymMeshManager instance = null;

        public float maxDistance = 100;

        //这个值无法运行时修改
        int maxCombineCount = 4000;
        public class MeshData
        {
            public MeshFilter f;
            public MeshRenderer r;
            public Transform t;
        }

        GameObject _GameObj;
        Transform _Transform;
        MeshFilter _MeshFilter;
        MeshRenderer _MeshRender;
        public Material mat;
        static readonly int max_value = 1000000;

        CombineInstance[] combine ;
        //MeshData [] combinObj = null;
        CTypeArray<MeshData>  combinObj = new CTypeArray<MeshData>();
        //int combinObjArrayLen = 200;
        //int  combinObjCount = 0;

        Bounds maxBounds = new Bounds(Vector3.zero, new Vector3(max_value, max_value,max_value));
        Mesh empty;
        Matrix4x4 maxMat;

        public void AddMesh(MeshFilter  f)
        {
            //if(null == combinObj)
            //  combinObj = new MeshData[combinObjArrayLen];
            //combinObj.Extend();

            MeshData d = new MeshData ();
            d.f = f;
            d.t = f.transform;
            d.r = f.GetComponent<MeshRenderer> ();
            combinObj.Add (d);
            //combinObj[combinObjCount++] = d;

        }

        public void Init () 
        {
            _GameObj = new GameObject ("CombineMesh");
            _GameObj.transform.position = Vector3.zero;
            _Transform = _GameObj.transform;

            _MeshRender = _GameObj.AddComponent<MeshRenderer> ();
            _MeshFilter = _GameObj.AddComponent<MeshFilter> ();
            _MeshRender.material = mat;

            _MeshFilter.mesh  = new Mesh ();
            empty = new Mesh ();

            combine =  new CombineInstance[maxCombineCount];
            maxMat = Matrix4x4.Translate (new Vector3 (max_value,max_value,max_value));
        }

        public static CombineDymMeshManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CombineDymMeshManager();
                }

                return instance;
            }
        }

        public void OnTick(int deltaTime)
        {
            for (int i = combinObj.Count-1; i>=0; i--)
            {
                if (combinObj[i].f == null)//释放已经删除
                {
                    combinObj.Delete (i);
                }
            }
            int objCount = 0;
            int ptr = 0;
            for (int i = 0;  i < combinObj.Count  ; i++)
            {
                float dis =Vector3.Distance (combinObj [i].t.position , _Transform.position);
                if ( objCount < maxCombineCount  &&  dis < maxDistance) {
                    combinObj [i].r.enabled = false;
                    objCount++;
                }
                else
                {
                    combinObj [i].r.enabled = true;
                }
            }
            for (int i =0 ; i < combinObj.Count ; i++)
            {
                if (combinObj [i].r.enabled == false) {
                    combine[ptr].mesh = combinObj [i].f.sharedMesh;
					combine[ptr++].transform =  transform.worldToLocalMatrix * combinObj [i].f.transform.localToWorldMatrix;
                }
            }

 
			while (combinObj.Count < maxCombineCount) {
				combine [ptr].mesh = empty;
				combine[ptr++].transform  = maxMat;
			}

            while (ptr < maxCombineCount) {
                combine [ptr].mesh = empty;
                combine[ptr++].transform  = maxMat;
            }
            _MeshFilter.mesh.CombineMeshes(combine);
            _MeshFilter.mesh.bounds = maxBounds;
        }

        public void AddShodaw(Transform shodaw)
        {
            MeshFilter fs = shodaw.GetComponent<MeshFilter>();
            if (fs == null)
                return;

            if (_MeshRender.sharedMaterial == null)
            {
                MeshRenderer mr = shodaw.GetComponent<MeshRenderer>();
                _MeshRender.sharedMaterial = mr.sharedMaterial;
            }
                
            
            AddMesh (fs);
        }

        public void Start()
        {
            Init();
          
        }

        public static void SetInstanceNull()
        {
            instance = null;
        }

        public void Reset()
        {
             
        }
    }
}
