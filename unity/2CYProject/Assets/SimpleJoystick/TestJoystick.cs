using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJoystick : MonoBehaviour {

	public SimpleJoystick joyKey;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (null == joyKey)
			return;

		if (joyKey.state == SimpleJoystick.STATE.Up) {
			Debug.LogError ("SimpleJoystick.STATE.Up" +joyKey.pos );
		}
		if (joyKey.state == SimpleJoystick.STATE.Down) {
			Debug.LogError (joyKey.state.ToString() +joyKey.pos );
		}
		if (joyKey.state == SimpleJoystick.STATE.DRAG) {
			//Debug.LogError (joyKey.state.ToString () + joyKey.pos);
		}
		else {
			Debug.LogError (joyKey.state.ToString () + joyKey.pos);
		}
		
	}
}
