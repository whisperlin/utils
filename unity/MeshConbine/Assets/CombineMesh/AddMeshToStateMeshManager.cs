using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMeshToStateMeshManager : MonoBehaviour {

	public CombineStaticMeshManager mgr;

	Plane  [] CachedPlanes = new  Plane[6] ;
	// Use this for initialization
	void Start () {
		mgr = new CombineStaticMeshManager ();
		 
		MeshFilter[] meshFilter = GetComponentsInChildren<MeshFilter>();

		for (int i = 0; i < meshFilter.Length; i++)
		{
			mgr.AddMesh (meshFilter [i]);
		}
		mgr.UpdateAllMesh (null);
	}
	
	// Update is called once per frame
	void Update () {
		GeometryUtilityUser.CalculateFrustumPlanes (Camera.main,ref CachedPlanes);
		mgr.UpdateAllMesh (CachedPlanes);

	}
}
