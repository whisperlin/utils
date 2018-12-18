using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class TestMenuItem : MonoBehaviour {

	 

	//& = Alt  #=shit  % = ctrl
	[MenuItem("MenuItem/普通的顶部菜单  _%#c")]
	private static void MenuItemFunc3()
	{ 
		Debug.Log("MenuItemFunc3");
	}


	[MenuItem("GameObject/在GameObject上",false,0)]
	static bool menu1 () {
		Object selectedObject = Selection.activeObject;
		/*if(selectedObject != null && 
			selectedObject.GetType() == typeof(GameObject))
		{
			return true;
		}
		return false;*/
		return false;
	}


 
	 
 

	
	[MenuItem("Assets/在Project目录里右键1")]
	private static void Assets_right_btn1()
	{ 
		Debug.Log("在Project目录里右键1");
	}

	[MenuItem("CONTEXT/Rigidbody/在Rigidbody上右键")]
	private static void CONTEXT_Rigidbody_right_btn()
	{ 
		Debug.Log("在Rigidbody上右键");
	}
	[MenuItem("GameObject/UI/在GameObject目录里右键")]
	private static void GameObject_right_btn()
	{ 
		Debug.Log("在GameObject目录里右键");
	}
}
