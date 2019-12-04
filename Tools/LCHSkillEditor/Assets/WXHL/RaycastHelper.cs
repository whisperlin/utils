using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHelper  {

	public static MeshCollider helperObject;


    public static bool Raycast(MeshFilter mf,Ray ray,out RaycastHit hitInfor,float maxDistance)
    {

        if (null == helperObject)
        {
            GameObject g = new GameObject("RaycastHelperHelperObject");
            g.hideFlags = HideFlags.DontSave;
#if UNITY_EDITOR

#else
            GameObject.DontDestroyOnLoad(g);
#endif
           
            helperObject = g.AddComponent<MeshCollider>();
            helperObject.convex = false;
        }
        helperObject.sharedMesh = mf.sharedMesh;
        helperObject.transform.position = mf.transform.position;
        helperObject.transform.rotation = mf.transform.rotation;
        helperObject.transform.localScale = mf.transform.localScale;

        return helperObject.Raycast(ray, out hitInfor, maxDistance);
    }
}
