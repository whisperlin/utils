using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor {
	void OnSceneGUI() {
		CameraController com = (CameraController) target;

		if(Event.current.type == EventType.MouseDown) {
			com.EnableRayCast(false);
		} else if(Event.current.type == EventType.MouseUp) {
			com.EnableRayCast(true);
		}

		var rot = Quaternion.Euler(com.cameraRotation);

		{
			Handles.color = Color.blue;
			var pos = com.cameraPosition;
			var posNew = Handles.Slider(pos, rot * Vector3.forward);
			if(posNew != pos) {
				var trs = Matrix4x4.TRS(posNew, rot, Vector3.one);
				var com_pos_in_cam = trs.inverse.MultiplyPoint3x4(com.transform.position);

				com.distance =  Mathf.Clamp(com_pos_in_cam.z, 0.1f, 100);
				var dir = rot * Vector3.forward;
				dir.Normalize();
				com.cameraPosition = com.transform.position - dir * com.distance;
			}
		}

		{
			Handles.color = Color.red;
			var pos = com.transform.position;
			var offset = com.cameraPosition - pos;
			var size = offset.magnitude / 2;
			var rotNew = Handles.Disc(rot, pos, rot * Vector3.right, size, false, 0);
			if(rotNew != rot) {
				var rotX = rotNew.eulerAngles.x;
				if(rotX > 180) {
					rotX -= 360;
				}
				com.rotX = Mathf.Clamp(rotX, 0, 90);
			}
		}

		{
			Handles.color = Color.green;
			var pos = com.transform.position;
			pos.y = com.cameraPosition.y;
			var offset = com.cameraPosition - pos;
			var size = offset.magnitude;
			var rotNew = Handles.Disc(rot, pos, Vector3.up, size, false, 0);
			if(rotNew != rot) {
				var rotY = rotNew.eulerAngles.y;
				com.rotY = rotY;
			}
		}

		/*
		{
			Handles.color = Color.green;
			var pos = com.cameraPosition;
			var dir = com.cameraPosition - com.transform.position;
			var posNew = Handles.Slider(pos, Vector3.down);
			if(posNew != pos) {
				if(posNew.y < com.transform.position.y) {
					posNew.y = com.transform.position.y;
				}

				var rotX = Mathf.Asin((posNew.y - com.transform.position.y) / (posNew - com.transform.position).magnitude) * Mathf.Rad2Deg;
				if(rotX < 0 || rotX > 90) {
					Debug.LogError(rotX);
				}
				com.rotX = rotX;
			}
		}
		*/
	}
}
