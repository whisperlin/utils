using UnityEngine;
using System.Collections;

public class KMDemoBoneRot : MonoBehaviour {
	Vector3 initialAngles;
	public float angle = 15.0f;
	public float tY = 0.31f;
	public float tZ = 0.47f;
	
	void Start() {
		initialAngles = transform.localEulerAngles;
	}
	// Update is called once per frame
	void Update () {
		float t = Time.time;
		transform.localEulerAngles = initialAngles + new Vector3(0.0f, angle*Mathf.Sin(tY*t), angle*Mathf.Cos(tZ*t));
	}
}
