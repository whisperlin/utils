using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AutoCreateUtil  {

	[MenuItem("Tool/AutoCreate")]
	static void AutoCreate()
	{
		if (null == Selection.activeGameObject)
			return;
		for (int i = 0; i < 10000; i++) {
			GameObject g = (GameObject)GameObject.Instantiate (Selection.activeGameObject);
			g.transform.position = new Vector3 (Random.Range(-5.0f,5.0f), Random.Range(-1.0f,1.0f),Random.Range(-5.0f,5.0f));
		}
	}
}
