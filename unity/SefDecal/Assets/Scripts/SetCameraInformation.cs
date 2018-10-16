using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraInformation : MonoBehaviour {

	public Material material;
	// Use this for initialization
	void Start () {
		MeshRenderer filder = GetComponent<MeshRenderer>();
		material = filder.material;
	}
	
	// Update is called once per frame
	void Update () {
		var p = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);// Unity flips its 'Y' vector depending on if its in VR, Editor view or game view etc... (facepalm)
		p[2, 3] = p[3, 2] = 0.0f;
		p[3, 3] = 1.0f;
		var clipToWorld = Matrix4x4.Inverse(p * Camera.main.worldToCameraMatrix) * Matrix4x4.TRS(new Vector3(0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);
		material.SetMatrix("clipToWorld", clipToWorld);

		material.SetMatrix("worldToLocal", transform.worldToLocalMatrix);

		Matrix4x4 mvp = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix;
		//Matrix4x4 m = Camera.main.worldToCameraMatrix * Camera.main.projectionMatrix;
		material.SetMatrix("MVP", mvp);
		material.SetMatrix("IMVP", Matrix4x4.Inverse(mvp));
		//transform.worldToLocalMatrix;
	}
}
