using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoControler : MonoBehaviour {
	string errstr = "";
	// Use this for initialization
	GUIStyle guiType=new GUIStyle();

	void Start()
	{
		guiType.normal.background = null; 
		guiType.normal.textColor=new Color(1,0,0); 
		guiType.fontSize = 40; 
	}
	// Update is called once per frame
	void OnGUI (){
		int posY = 0;
		int c = transform.childCount;

		for (int i = 0; i < c; i++) {
			posY = 40+i*120;
			if (GUI.Button (new Rect (20,posY,400,100), "Level "+i.ToString() )) {
				
				transform.GetChild(i).gameObject.SetActive(!transform.GetChild(i).gameObject.activeSelf);
			}
		}
		posY += 120;

 
		if (SimpleLog.isModify)
			errstr = SimpleLog.getLogString ();
		GUI.Label (new Rect (20, posY, 800, 200), errstr,guiType);
		
	}
}
