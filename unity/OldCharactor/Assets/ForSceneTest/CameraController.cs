using UnityEngine;

public class CameraController : MonoBehaviour {
	[Range(0, 90)]
	public float rotX = 45;
	[Range(0, 360)]
	public float rotY = 0;
	[Range(0.1f, 100)]
	public float distance = 20;
	public Vector3 cameraPosition;
	public Vector3 cameraRotation;

	bool mRayCast = true;
	Mesh mCylinder;

	public void EnableRayCast(bool value) {
		mRayCast = value;
	}

	public void SetCameraPosition(Vector3 pos) {
		cameraPosition = pos;
		var dir = Quaternion.Euler(rotX, rotY, 0) * Vector3.forward;
		dir.Normalize();
		transform.position = cameraPosition + dir * distance;
	}

	public void SetCameraRotation(Vector3 euler) {
		rotX = euler.x;
		rotY = euler.y;
	}

	void OnDrawGizmos() {
		Transform target = transform;

		if(mRayCast) {
			RaycastHit hit;
			if(Physics.Raycast(target.position + new Vector3(0, 100, 0), Vector3.down, out hit, 200, -1)) {
				target.position = hit.point;
			}
		}

		cameraRotation = new Vector3(rotX, rotY, 0);

		var rot = Quaternion.Euler(cameraRotation);
		var dir = rot * Vector3.forward;
		dir.Normalize();
		cameraPosition = target.position - dir * distance;

		if(mCylinder == null) {
			var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			mCylinder = cylinder.GetComponent<MeshFilter>().sharedMesh;
			GameObject.DestroyImmediate(cylinder);
		}

		Gizmos.color = new Color(0, 1, 0, 0.5f);
		Gizmos.DrawMesh(mCylinder, (cameraPosition + target.position) * 0.5f, rot * Quaternion.Euler(90, 0, 0), new Vector3(0.5f, (cameraPosition - target.position).magnitude * 0.5f, 0.5f));

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(target.position, 1);

		Gizmos.color = Color.green;
		Gizmos.DrawSphere(cameraPosition, 1);

		var cam = Camera.main;
		if(cam) {
			cam.transform.position = cameraPosition;
			cam.transform.rotation = rot;
		}
	}
}
