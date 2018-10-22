using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SimpleLog : MonoBehaviour {

	static List<string> logs = new List<string> ();
	public static bool isModify = false;
	public static void Log(string str)
	{
		Debug.Log (str);
		if (logs.Count > 5) {
			logs.RemoveAt(4);
		}
		isModify = true;
		logs.Insert(0,str);
	}
	public static string getLogString()
	{
		isModify = false;
		string res = "";
		for(int i = 0 ; i < logs.Count ; i++)
		{
			res += logs[i]+"\n";
		}
		return res;
	}
}
