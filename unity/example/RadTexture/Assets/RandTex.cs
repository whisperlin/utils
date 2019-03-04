using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandTex : MonoBehaviour {
    public Material mat;
    public RenderTexture rt;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        Graphics.Blit(rt, rt, mat);

    }
}
