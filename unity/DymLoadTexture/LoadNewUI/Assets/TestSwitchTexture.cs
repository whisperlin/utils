using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSwitchTexture : MonoBehaviour {

	UISpriteCtrl GetUISpriteCtrl(GameObject g)
	{
		UISpriteCtrl c =  g.GetComponent<UISpriteCtrl> ();
		if (null == c)
			return GetUISpriteCtrl (g.transform.parent.gameObject);
		return c;
	}
	// Use this for initialization
	void Start () {
		UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image> ();
		if (null != img) {
			UISpriteCtrl c = GetUISpriteCtrl (gameObject);
			c.SwitchTexture(img,"table_1");
		}

	}
	
	 
}
