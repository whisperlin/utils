using UnityEngine;
using System.Collections;
public class TestMain : MonoBehaviour
{ 
	 
	[ContextMenu("ContextMenu1")] 
	public void ContextMenuFunc1() 
	{ 
		Debug.Log("ContextMenu1");
	} 
	public int a = 0; 
	public string b = ""; 
	[ContextMenuItem("add testName", "ContextMenuFunc2")] 
	public string testName = ""; 
	private void ContextMenuFunc2() 
	{ 
		testName = "testName"; 
	}
}