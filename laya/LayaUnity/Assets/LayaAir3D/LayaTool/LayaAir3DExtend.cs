using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LayaAir3D  {

	Transform GetObjectInChild(Transform t,string name)
	{
		if (t.name.CompareTo (name) == 0)
			return t;
		for (int i = 0; i < t.childCount; i++) {
			Transform c = GetObjectInChild (t.GetChild(i),name);
			if (c)
				return c;
		}
		return null;
	}
	void ExportObj()
	{
		FirstlevelMenu = 1;
		LayaExport.DataManager.Type = FirstlevelMenu;
		if (Ios)
		{
			exportResource(false, 1,"/IOS");    
		}
		if (Android)
		{
			exportResource(false, 2,"/Android");

		}
		if (Conventional)
		{
			exportResource(false, 0,"/Conventional");

		}
	}
	void ExportRole( Animator animator)
	{
		string txt = "导出角色" + animator.name;
		UnityEditor.EditorUtility.DisplayProgressBar("导出中", txt, (float)(0)); 
		int count = 0;
		float CurCount = 0;
		//Debug.LogError (animator.name +" " + animator.gameObject.GetInstanceID());
		List<GameObject> guns = new List<GameObject> ();
		List<GameObject> bodys = new List<GameObject> ();
		Transform t = animator.transform;
		for (int i = 0; i < t.childCount; i++) {
			Transform c = t.GetChild(i);
			if (   c.GetComponent<SkinnedMeshRenderer> () != null || c.GetComponent<MeshRenderer> () != null  ) {
				if (c.name.StartsWith ("M_")) {
					guns.Add (c.gameObject);
				} else {
					bodys.Add (c.gameObject);
				}

			}
		}
		count = guns.Count + 1;
		Transform head = GetObjectInChild (animator.transform,"Head_P");
		Transform body = GetObjectInChild (animator.transform,"Body_P");
			Transform leg = GetObjectInChild (animator.transform,"Leg_P");
		//激活三个碰撞体。
		if (head)
			head.gameObject.SetActive (true);
		if (body)
			body.gameObject.SetActive (true);
		if (leg)
			leg.gameObject.SetActive (true);

		LayaExport.DataManager.overlapSceneName = animator.name;
		//导出角色.
		//隐藏枪支.
		foreach(GameObject g in guns)
		{
			g.SetActive (false);
		}
		//激活身体.
		foreach(GameObject g in bodys)
		{
			g.SetActive (true);
		}
		ExportObj ();
		CurCount++;
		UnityEditor.EditorUtility.DisplayProgressBar("导出中", txt, (float)(CurCount/count)); 
		//隐藏三个碰撞体
		if (head)
			head.gameObject.SetActive (false);
		if (body)
			body.gameObject.SetActive (false);
		if (leg)
			leg.gameObject.SetActive (false);
		//隐藏身体.
		foreach(GameObject g in bodys)
		{
			g.SetActive (false);
		}
		//导出枪支

		foreach(GameObject g in guns)
		{
			g.SetActive (true);
			LayaExport.DataManager.overlapSceneName = g.name;
			ExportObj ();
			CurCount++;
			UnityEditor.EditorUtility.DisplayProgressBar("导出中", txt, (float)(CurCount/count)); 
		}

		LayaExport.DataManager.overlapSceneName = null;

		foreach(GameObject g in bodys)
		{
			g.SetActive (true);
		}

	}
	void OnExtendGUI()
	{
		if (GUILayout.Button ("LayaAir Export Role", GUILayout.Height (26))) {
			Animator[] animators = GameObject.FindObjectsOfType<Animator> ();
			for (int i = 0; i < animators.Length; i++) {
				animators [i].gameObject.SetActive (false);
			}
			for (int i = 0; i < animators.Length; i++) {
				animators [i].gameObject.SetActive (true);
				if (animators [i].transform.parent == null) {
					ExportRole ( animators[i]);
				}
				animators [i].gameObject.SetActive (false);
			}
			for (int i = 0; i < animators.Length; i++) {
				animators [i].gameObject.SetActive (true);
			}
			UnityEditor.EditorUtility.DisplayDialog ("", "导出完成", "ok");
		}
		UnityEditor.EditorUtility.ClearProgressBar();

	}
}
