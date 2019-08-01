using UnityEngine;
using System.Collections;

public class SetDepthCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
