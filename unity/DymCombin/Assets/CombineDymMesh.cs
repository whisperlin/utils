using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class CombineDymMesh : MonoBehaviour {

	public float maxDistance = 5;
 
	//这个值无法运行时修改
	public int maxCombineCount = 40;
	public class MeshData
	{
		public MeshFilter f;
		public MeshRenderer r;
		public Transform t;
	}
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
		//	combinObj = new MeshData[combinObjArrayLen];
		//combinObj.Extend();
	 
		MeshData d = new MeshData ();
		d.f = f;
		d.t = f.transform;
		d.r = f.GetComponent<MeshRenderer> ();
		combinObj.Add (d);
		//combinObj[combinObjCount++] = d;

	}
	// Use this for initialization
	void Start () {
		GameObject g = new GameObject ("CombineMesh");
		gameObject.transform.position = Vector3.zero;
		_Transform = transform;

		_MeshRender = g.AddComponent<MeshRenderer> ();
		_MeshFilter = g.AddComponent<MeshFilter> ();
		_MeshRender.material = mat;
 
		_MeshFilter.mesh  = new Mesh ();
		empty = new Mesh ();

		combine =  new CombineInstance[maxCombineCount];
		maxMat = Matrix4x4.Translate (new Vector3 (max_value,max_value,max_value));
	}
	// Update is called once per frame
	void Update () {
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
				combine[ptr++].transform = combinObj [i].f.transform.localToWorldMatrix;
			}
		}
		while (ptr < maxCombineCount) {
			combine [ptr].mesh = empty;
			combine[ptr++].transform  =maxMat;
		}
		_MeshFilter.mesh.CombineMeshes(combine);
		_MeshFilter.mesh.bounds = maxBounds;
 
		 

		
	}
}
