using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yw;

public class CombinedMesh : MonoBehaviour {


	public class MeshData
	{
		public MeshFilter f;
		public MeshRenderer r;
		public Transform t;
	}
	bool isInited = false;
	public Mesh mesh;
	MeshFilter _MeshFilter;
	MeshRenderer _MeshRender;
	public Bounds bounds;
	CTypeArray<MeshData>  combinObj = new CTypeArray<MeshData>();
	CTypeArray<CombineInstance>  combine = new CTypeArray<CombineInstance>();
	static Mesh empty;
	bool isMeshActive = false;
	void Init()
	{
		if (isInited)
			return;
		isInited = true;
		mesh = new Mesh ();
		_MeshFilter = gameObject.AddComponent<MeshFilter> ();
		_MeshFilter.mesh = mesh;
		_MeshRender = gameObject.AddComponent<MeshRenderer> ();


	}
	public void SetMat(Material mat)
	{
		Init ();
		_MeshRender.sharedMaterial = mat;
	}
	public void AddMesh(MeshFilter  f,MeshRenderer r,bool updateMesh = true)
	{
		Init ();
		MeshData d = new MeshData ();
		d.f = f;
		d.t = f.transform;
		d.r = r;
		combinObj.Add (d);
	}


	public void UpdateMesh(Plane  [] cut)
	{
		bool inView = GeometryUtility.TestPlanesAABB (cut, bounds);
		 
		if (null == empty)
			empty = new Mesh ();
		if (inView && isMeshActive)
			return;
		if (!inView && !isMeshActive)
			return;


		mesh.Clear ();

		if (isMeshActive) {
			_MeshRender.enabled = false;
			isMeshActive = false;
			return;
		}
		_MeshRender.enabled = true;
		for (int i = combinObj.Count-1; i>=0; i--)
		{
			if (combinObj[i].f == null)//释放已经删除
			{
				combinObj.Delete (i);
			}
		}

		int objCount = 0;
 
		for (int i = 0;  i < combinObj.Count  ; i++)
		{

			combinObj [i].r.enabled = false;
			objCount++;

			//这里以后扩展对会动的东西也能管理.
			/*if ( true ) {
				combinObj [i].r.enabled = false;
				objCount++;
			}
			else
			{
				combinObj [i].r.enabled = true;
			}*/
		}
		combine.Clear ();
		for (int i =0 ; i < combinObj.Count ; i++)
		{
			if (combinObj [i].r.enabled == false) {
				combine.AddOne ();
				combine.ary[i].mesh = combinObj [i].f.sharedMesh;
				combine.ary[i].transform =  gameObject.transform.worldToLocalMatrix *  combinObj [i].f.transform.localToWorldMatrix;
			}
		}
		for (int i =combinObj.Count ; i < combine.ary.Length ; i++)
		{
			combine.ary[i].mesh = empty;
			combine.ary[i].transform = Matrix4x4.identity;
		}

		mesh.CombineMeshes (combine.ary);
		_MeshFilter.mesh = mesh;
		bounds = _MeshRender.bounds;
		isMeshActive = true;
		/*
		BoxCollider c = _MeshFilter.gameObject.GetComponent<BoxCollider> ();
		if (null == c)
			c = _MeshFilter.gameObject.AddComponent<BoxCollider> ();
		c.center = mesh.bounds.center;
		c.size = mesh.bounds.size;
		*/
 

	}

	 
	 
	 

}
