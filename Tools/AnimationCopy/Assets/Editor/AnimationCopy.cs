using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimationCopy  {

	static void SaveAnimationClip(AnimationClip c,string path)
	{
		string tempPath = "Assets/temp.anim";
		AssetDatabase.CreateAsset (c,tempPath);
		// AnimationClipのコピー（固定化したuuid）
		System.IO.File.Copy (tempPath, path, true);
		//System.IO.File.Delete (tempPath);
		AssetDatabase.DeleteAsset (tempPath);

		 
	}
	[MenuItem("lin/Copy")]
	static void AssetCopy ()
	{
		if (null == Selection.activeObject)
			return;
		// AnimationClipを持つFBXのパス
		string fbxPath = AssetDatabase.GetAssetPath (Selection.activeObject);
		Debug.Log (fbxPath);
		// AnimationClipの取得
		Object[]  animations = AssetDatabase.LoadAllAssetsAtPath (fbxPath );
		List<AnimationClip> cs = new List<AnimationClip> ();
		foreach(object c in animations)
		{
			if (c is AnimationClip) {
				cs.Add ( c  as AnimationClip);
			}
		}
		int idx = fbxPath.LastIndexOf ("/");
		string rootPath = fbxPath.Substring (0, idx+1);
		foreach(AnimationClip _c in cs)
		{
			if (_c.name.Contains ("__preview__") == false) {
				AnimationClip UpClip = Object.Instantiate (_c);
				Dictionary<EditorCurveBinding,AnimationCurve> bcMap = new Dictionary<EditorCurveBinding, AnimationCurve> ();
				List<EditorCurveBinding> newBinds = new List<EditorCurveBinding> ();
				EditorCurveBinding[]  binds = AnimationUtility.GetCurveBindings (UpClip);
				foreach (EditorCurveBinding b in binds) {
					//if (b.propertyName.Contains ("m_LocalPosition") || b.propertyName.Contains ("m_LocalScale") ) {
					//	continue;
					//}
					 
					newBinds.Add (b);
					AnimationCurve __clip = AnimationUtility.GetEditorCurve (UpClip,b);
					bcMap [b] = __clip;
 
				}

				UpClip.ClearCurves ();
				foreach (EditorCurveBinding b in newBinds) {
					AnimationUtility.SetEditorCurve (UpClip, b, bcMap [b]);
				}
				AssetDatabase.CreateAsset (UpClip, rootPath+_c.name+".anim");
			}
		}
	}


	[MenuItem("lin/RemoveScaleAndPosition")]
	static void RemoveScaleAndPosition ()
	{
		if (null == Selection.activeObject)
			return;
		// AnimationClipを持つFBXのパス
		string fbxPath = AssetDatabase.GetAssetPath (Selection.activeObject);
		Debug.Log (fbxPath);
		// AnimationClipの取得
		Object[]  animations = AssetDatabase.LoadAllAssetsAtPath (fbxPath );
		List<AnimationClip> cs = new List<AnimationClip> ();
		foreach(object c in animations)
		{
			if (c is AnimationClip) {
				cs.Add ( c  as AnimationClip);
			}
		}
		int idx = fbxPath.LastIndexOf ("/");
		string rootPath = fbxPath.Substring (0, idx+1);
		foreach(AnimationClip _c in cs)
		{
			if (_c.name.Contains ("__preview__") == false) {
				AnimationClip UpClip = Object.Instantiate (_c);
				Dictionary<EditorCurveBinding,AnimationCurve> bcMap = new Dictionary<EditorCurveBinding, AnimationCurve> ();
				List<EditorCurveBinding> newBinds = new List<EditorCurveBinding> ();
				EditorCurveBinding[]  binds = AnimationUtility.GetCurveBindings (UpClip);
				foreach (EditorCurveBinding b in binds) {
					if (b.propertyName.Contains ("m_LocalPosition") || b.propertyName.Contains ("m_LocalScale") ) {
						continue;
					}
					 
					newBinds.Add (b);
					AnimationCurve __clip = AnimationUtility.GetEditorCurve (UpClip,b);
					bcMap [b] = __clip;
 
				}

				UpClip.ClearCurves ();
				foreach (EditorCurveBinding b in newBinds) {
					AnimationUtility.SetEditorCurve (UpClip, b, bcMap [b]);
				}
				AssetDatabase.CreateAsset (UpClip, rootPath+_c.name+".anim");
			}
		}
	}



	[MenuItem("lin/Break3Anim")]
	static void Break3Anim ()
	{
		if (null == Selection.activeObject)
			return;
		// AnimationClipを持つFBXのパス
		string fbxPath = AssetDatabase.GetAssetPath (Selection.activeObject);
		Debug.Log (fbxPath);
		// AnimationClipの取得
		Object[]  animations = AssetDatabase.LoadAllAssetsAtPath (fbxPath );
		List<AnimationClip> cs = new List<AnimationClip> ();
		foreach(object c in animations)
		{
			if (c is AnimationClip) {
				cs.Add ( c  as AnimationClip);
			}
		}
		int idx = fbxPath.LastIndexOf ("/");
		string rootPath = fbxPath.Substring (0, idx+1);
		foreach(AnimationClip _c in cs)
		{
			if (_c.name.Contains ("__preview__") == false) {
				AnimationClip UpClip = Object.Instantiate (_c);
				AnimationClip DownClip = Object.Instantiate (_c);
				AnimationClip LeftClip = Object.Instantiate (_c);
				Dictionary<EditorCurveBinding,AnimationCurve> bcMap = new Dictionary<EditorCurveBinding, AnimationCurve> ();
				List<EditorCurveBinding> LBinds = new List<EditorCurveBinding> ();
				List<EditorCurveBinding>	UBinds = new List<EditorCurveBinding> ();
				List<EditorCurveBinding> DBinds = new List<EditorCurveBinding> ();
				EditorCurveBinding[]  binds = AnimationUtility.GetCurveBindings (UpClip);
				foreach (EditorCurveBinding b in binds) {


					if (b.path.Contains ("/Bip001 L Clavicle")) {
						LBinds.Add (b);
					} else if (b.path.Contains ("/upper")) {
						UBinds.Add (b);
					} else {
						DBinds.Add (b);
					}
 
					AnimationCurve __clip = AnimationUtility.GetEditorCurve (UpClip,b);
					bcMap [b] = __clip;

				}
				UpClip.ClearCurves ();
				DownClip.ClearCurves ();
				LeftClip.ClearCurves ();
				foreach (EditorCurveBinding b in UBinds) {
					AnimationUtility.SetEditorCurve (UpClip, b, bcMap [b]);
				}
				AssetDatabase.CreateAsset (UpClip, rootPath+_c.name+"_U.anim");


				foreach (EditorCurveBinding b in DBinds) {
					AnimationUtility.SetEditorCurve (DownClip, b, bcMap [b]);
				}
				AssetDatabase.CreateAsset (DownClip, rootPath+_c.name+"_D.anim");


				foreach (EditorCurveBinding b in LBinds) {
					AnimationUtility.SetEditorCurve (LeftClip, b, bcMap [b]);
				}
				AssetDatabase.CreateAsset (LeftClip, rootPath+_c.name+"_L.anim");
			}
		}
	}
}
