using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthBufferDecalFrameCtrl : MonoBehaviour {

	public int UCount =1;
	public int VCount = 1;
	public float DeltaTime = 0.1f;
	public Material mat = null;
	float curTime = 0.0f;
	int curIndex = 0;	
	// Use this for initialization
	void Start () {
		MeshRenderer project = GetComponent<MeshRenderer>();
		mat = new Material(project.material);
		project.material = mat;
	}
	
	// Update is called once per frame
	void Update () {
		curTime += Time.deltaTime;
		if (curTime > DeltaTime)
		{
			curTime = -DeltaTime;
			curIndex++;
			if (curIndex >= UCount * VCount)
			{
				curIndex -= UCount * VCount;
			}
		}
		float FrameDeltaU = 1.0f / UCount;
		float FrameDeltaV = 1.0f / VCount;
		float OffsetU = FrameDeltaU * (curIndex % UCount);
		float OffsetV = FrameDeltaV * (curIndex / UCount);

		mat.SetFloat("_FrameSizeU", FrameDeltaU);
		mat.SetFloat("_FrameSizeV", FrameDeltaV);

		mat.SetFloat("_FrameU", OffsetU);
		mat.SetFloat("_FrameV", OffsetV);

	}
	private void OnDestroy()
	{
		if(null != mat)
			GameObject.DestroyImmediate(mat);
	}
}
