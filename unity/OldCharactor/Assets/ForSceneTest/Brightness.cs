using UnityEngine;
using System.Collections;

public class Brightness : MonoBehaviour {
	public Color brightness = new Color(0.5f, 0.5f, 0.5f, 1);

	void Start () {
		Shader.EnableKeyword("BRIGHTNESS_ON");
	}

	void OnDestroy() {
		Shader.DisableKeyword("BRIGHTNESS_ON");
	}

	void Update () {
		Shader.SetGlobalColor("_Brightness", brightness);
	}
}