using UnityEngine;
using System.Collections;
using UnityEditor;
public class ExampleClass : MonoBehaviour {
	
	[MenuItem("TA/Save Cube Map")]
	static void save() {
		Cubemap texture = new Cubemap(256, TextureFormat.RGBA32, false);
		AssetDatabase.CreateAsset (texture,"Assets/cubemap.asset");
	}
}