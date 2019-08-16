using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SetParams : MonoBehaviour {

    public float wposZ = 0;
    public MeshRenderer mr;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        mr.material.SetFloat("_BeginPosZInWorld", wposZ);
	}
}
