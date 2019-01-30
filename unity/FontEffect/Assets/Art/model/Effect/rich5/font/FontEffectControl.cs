using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontEffectControl : MonoBehaviour {

	public float animTime = 1.0f;
	float time = 0.0f;
	MeshRenderer _render;
	MaterialPropertyBlock mb;
	// Use this for initialization
	void Start () {
		_render = GetComponent<MeshRenderer> ();
		mb = new MaterialPropertyBlock ();
		_render.SetPropertyBlock (mb);
		
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		mb.SetFloat ("_EffectPower",Mathf.Min(time,1));
		_render.SetPropertyBlock (mb);
		
	}
	void OnDisable(){
		time = 0.0f;
	}

}
