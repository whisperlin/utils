using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideObject : MonoBehaviour {

    public GameObject g;
	// Use this for initialization
	public void Click () {
        if(null != g)
            g.SetActive(!g.activeSelf);
	}

    public void ScreenSize(UnityEngine.UI.Toggle t)
    {
        ScreenManager.SetScreenSize(t.isOn);
    }
	
	 
}
