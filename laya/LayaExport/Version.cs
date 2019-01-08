using System;
using UnityEditor;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class Version : EditorWindow
{
	// Token: 0x06000083 RID: 131 RVA: 0x00005EEC File Offset: 0x000040EC
	[MenuItem("LayaAir3D/Help/Version", false)]
	private static void initTutorial()
	{
		global::Version.version = (global::Version)EditorWindow.GetWindow(typeof(global::Version));
		WWW www = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/layabox.png");
		GUIContent titleContent = new GUIContent("LayaAir3D", www.texture);
		global::Version.version.titleContent = titleContent;
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00005F48 File Offset: 0x00004148
	private void OnGUI()
	{
		GUI.Label(new Rect(base.position.width / 2f - 70f, base.position.height / 2f - 20f, 200f, 30f), "LayaBox Version Beta05 .");
	}

	// Token: 0x0400005B RID: 91
	private static global::Version version;
}
