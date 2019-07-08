using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;

public class AnimationTool {
	[MenuItem("Easy/Animation/Set Default Idle Clip")]
	public static void SetDefaultClip() {
		var files = Directory.GetFiles("Assets/Props/Prefabs/Character", "*.prefab", SearchOption.AllDirectories);
		foreach(var i in files) {
			var asset = AssetDatabase.LoadMainAssetAtPath(i) as GameObject;
			if(asset) {
				var anim = asset.GetComponent<Animation>();
				if(anim) {
					var clip = anim.GetClip("Idle");
					if(clip == null) {
						clip = anim.GetClip("FightIdle");
					}
					if(clip) {
						anim.clip = clip;
					}
				}
			}
		}

		Debug.Log("Set animations default clip to Idle done.");
	}
}
