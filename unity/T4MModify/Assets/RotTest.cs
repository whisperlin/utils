using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class RotTest : MonoBehaviour {
    public Transform target;
    public float a = 10f;
    public float distance;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (null != target)
        {
            transform.position = target.position;
            transform.forward = Vector3.forward;
            transform.localRotation = Quaternion.Euler(0,a, 0);

            transform.position = transform.position - transform.forward * distance;
        }
		
	}
}
