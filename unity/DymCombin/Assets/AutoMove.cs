using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : MonoBehaviour {

	float f = 0;
	Vector3 BeginPosition;
	// Use this for initialization
	void Start () {
		BeginPosition = transform.position;

	}
	
	// Update is called once per frame
	void Update () {
		f += Time.deltaTime;
		transform.position =BeginPosition + new Vector3(   Mathf.Sin (f)   , 0 , 0 );
		
	}
}
