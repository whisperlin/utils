using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KrablMesh {	
	public class UnityPlatform
	{
		public UnityPlatform(string n, BuildTargetGroup btg, GUIContent gc) {
			buildTargetGroup = btg;
			name = n;
			smallIconContent = gc;
		}
		public BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown;
		public string name;
		public GUIContent smallIconContent = null;
	}

	public class UnityEditorUtils {
	
		public static UnityPlatform[] ValidUnityPlatformGroups()
		{	
			List<UnityPlatform> result = new List<UnityPlatform>();
			
			Texture2D tex;
			tex = EditorGUIUtility.FindTexture("BuildSettings.Web.Small");
			//result.Add(new UnityPlatform("Web",				BuildTargetGroup.WebPlayer,	 	tex ? new GUIContent(tex) : new GUIContent("Web")));

			tex = EditorGUIUtility.FindTexture("BuildSettings.Standalone.Small");
			result.Add(new UnityPlatform("Standalone",		BuildTargetGroup.Standalone,	tex ? new GUIContent(tex) : new GUIContent("PC")));
			
			tex = EditorGUIUtility.FindTexture("BuildSettings.iPhone.Small");
			result.Add(new UnityPlatform("iPhone",			BuildTargetGroup.iOS,		tex ? new GUIContent(tex) : new GUIContent("iOS")));

			tex = EditorGUIUtility.FindTexture("BuildSettings.Android.Small");
			result.Add(new UnityPlatform("Android", 		BuildTargetGroup.Android,		tex ? new GUIContent(tex) : new GUIContent("And")));
				
			tex = EditorGUIUtility.FindTexture("BuildSettings.BlackBerry.Small");
			if (tex) {
				//result.Add(new UnityPlatform("BlackBerry", 	BuildTargetGroup.BB10,			tex ? new GUIContent(tex) : new GUIContent("BB10")));
			}
			
			if (Application.platform == RuntimePlatform.WindowsEditor) {
				tex = EditorGUIUtility.FindTexture("BuildSettings.Metro.Small");
				if (tex) {
					result.Add(new UnityPlatform("Windows Store Apps", BuildTargetGroup.Metro,		tex ? new GUIContent(tex) : new GUIContent("Win8")));
				}
			
				tex = EditorGUIUtility.FindTexture("BuildSettings.WP8.Small");
				if (tex) {
					result.Add(new UnityPlatform("WP8", BuildTargetGroup.WP8,		tex ? new GUIContent(tex) : new GUIContent("WP8")));
				}
			}
			
			// Consoles ... speculative code I have no licenses. The question is how to determine someone has a license.
			
			// Google Native Client. Has been removed from texture imported.. so let's comment it out.
			// tex = EditorGUIUtility.FindTexture("BuildSettings.NaCl.Small");
			// result.Add(new UnityPlatform(BuildTargetGroup.NaCl,			"Google Native Client",	tex ? new GUIContent(tex) : new GUIContent("NaCl")));
		
			// Search for new platforms... which have icons and are part of the license
			BuildTargetGroup[] platformValues = (BuildTargetGroup[])System.Enum.GetValues(typeof(BuildTargetGroup));
			string licenseInfo = UnityEditorInternal.InternalEditorUtility.GetLicenseInfo().ToLower();
	
			int num = platformValues.Length;
			for (int i = 0; i < num; ++i) {
				// Check if we have this already
				bool alreadyIncluded = false;
				for (int j = 0; j < result.Count; ++j) {
					if (result[j].buildTargetGroup == platformValues[i]) {
						alreadyIncluded = true;
						break;
					}
				}
				
				// Check whether the name appears in the license info (attempt to check if user has the special licenses)
				// Only add if an icon can be found
				string name = platformValues[i].ToString();
				if (!alreadyIncluded 
					//&& platformValues[i] != BuildTargetGroup.FlashPlayer 
					//&& platformValues[i] != BuildTargetGroup.NaCl
					&& licenseInfo.Contains(name.ToLower())) {
					tex = EditorGUIUtility.FindTexture("BuildSettings." + name + ".Small");
					if (tex != null) {
						result.Add(new UnityPlatform(name, platformValues[i], new GUIContent(tex)));
					}
				}
			} 
			return result.ToArray();
		}
		
		public static GUIStyle PreviewToolbarButtonStyle() {
			GUIStyle toolbar = EditorStyles.toolbarButton;
			GUIStyle result = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("PreButton");
			if (result == null) return toolbar;
			
			Color texCol = Color.white;
			result.font = toolbar.font;
			result.normal.textColor = texCol;
			result.active.textColor = texCol;
			result.focused.textColor = texCol;
			result.hover.textColor = texCol;
			result.onNormal.textColor = texCol;
			result.onActive.textColor = texCol;
			result.onFocused.textColor = texCol;
			result.onHover.textColor = texCol;
			
			result.margin = new RectOffset(0, 1, 0, 0);
			
			return result;
		}

		public static void GUILayoutAddHoriLine() {
			GUIStyle style = new GUIStyle("Box");
			style.fixedHeight = 1;
			style.stretchWidth = true;
			style.margin = new RectOffset(0, 0, 0, 3);
			style.padding = new RectOffset(0, 0, 0, 0);
			style.border = new RectOffset(1, 1, 1, 1);
			GUILayout.Box(new GUIContent(""), style);
		}
		
		public static void GUILayoutPlusMinusIntProperty(SerializedProperty intProp, GUIContent content) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(content, GUILayout.Width(140.0f));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) intProp.intValue--;
			intProp.intValue = EditorGUILayout.IntField(intProp.intValue, GUILayout.Width(30.0f));
			if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) intProp.intValue++;
			EditorGUILayout.EndHorizontal();
		}
		
		public delegate void OnPropGUIDelegate(SerializedProperty prop);
		
		public static int GUILayoutPlatformDependantProperty(
			SerializedProperty mainProp,
			SerializedProperty platformArrayProp,
			SerializedProperty platformIDProp,
			OnPropGUIDelegate onPropGUIDel
			)
		{
			UnityPlatform[] _platforms = ValidUnityPlatformGroups();
			
			int overridePlatformIndex = -1;
			const string overPlatformKey = "KrablOverridePropertyTargetGroupIndex";
			if (EditorPrefs.HasKey(overPlatformKey)) {
				int val = EditorPrefs.GetInt(overPlatformKey);
				if (val >= 0 && val < _platforms.Length) {
					overridePlatformIndex = val;
				}
			}
			
			
			// Platform override box
			GUIStyle style = new GUIStyle("box");
			style.padding = new RectOffset(0, 1, 0, 0);		
			GUILayout.BeginVertical(style);
		
			// Platform chooser toolbar
		
			GUILayout.BeginHorizontal();
			int oldPlatform = overridePlatformIndex;
			bool flag = (overridePlatformIndex == -1);
			flag = GUILayout.Toggle(flag, "Default", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
			if (flag) overridePlatformIndex = -1;				
			for (int i = 0; i < _platforms.Length; ++i) {
				flag = (overridePlatformIndex == i);
				flag = GUILayout.Toggle(flag, _platforms[i].smallIconContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
				if (flag) overridePlatformIndex = i;
			}		
			GUILayout.EndHorizontal();
			// If platform changed, fix keyboard focus on the input field
			if (oldPlatform != overridePlatformIndex) {
				GUI.FocusControl("");
				EditorPrefs.SetInt(overPlatformKey, overridePlatformIndex);
			}
		
			// Platform values logic!! complex
			
			string oPlatformID = "";
			int storageIndex = -1;
				
			bool guienabled = GUI.enabled;
			if (overridePlatformIndex != -1 && overridePlatformIndex < _platforms.Length) {
				oPlatformID = _platforms[overridePlatformIndex].buildTargetGroup.ToString();
				// Search for the platform
				bool oFlag = false;
				for (int i = 0; i < platformIDProp.arraySize; ++i) {
					if (platformIDProp.GetArrayElementAtIndex(i).stringValue == oPlatformID) {
						storageIndex = i;
						oFlag = true;
						break;
					}
				}
				bool nFlag = GUILayout.Toggle(oFlag, "Override for " + _platforms[overridePlatformIndex].name); 
				
				if (nFlag && !oFlag) { // adding new platform override
					storageIndex = platformIDProp.arraySize;
					platformIDProp.arraySize++;
					platformIDProp.GetArrayElementAtIndex(storageIndex).stringValue = oPlatformID;
					platformArrayProp.arraySize++;
					platformArrayProp.GetArrayElementAtIndex(storageIndex).intValue = mainProp.intValue;
				} else if (!nFlag && oFlag) { // deleting platform override
					int num = platformIDProp.arraySize;
					platformIDProp.MoveArrayElement(storageIndex, num - 1);
					platformIDProp.arraySize--;
					platformArrayProp.MoveArrayElement(storageIndex, num - 1);
					platformArrayProp.arraySize--;
				}
				
				if (nFlag == false) {
					GUI.enabled = false;
					onPropGUIDel(mainProp);
				} else {
					onPropGUIDel(platformArrayProp.GetArrayElementAtIndex(storageIndex));
				}
			} else {
				onPropGUIDel(mainProp);
			}
			GUI.enabled = guienabled;
			GUILayout.EndVertical();
			
			return overridePlatformIndex;
		}
		
		public static void AssureAssetFolderExists(string folderPath) {
			if (Directory.Exists(Path.Combine(Application.dataPath, folderPath)) == false) {
				List<string> dirs = new List<string>();
				while (folderPath.Equals("") == false) {
					dirs.Add(folderPath);
					folderPath = Path.GetDirectoryName(folderPath);
				}	 
				dirs.Add("");
			
				for (int i = dirs.Count - 1; i > 0; --i) {
					if (Directory.Exists(Application.dataPath + "/" + dirs[i - 1]) == false) {	
						AssetDatabase.CreateFolder(Path.Combine("Assets", dirs[i]), Path.GetFileName(dirs[i - 1]));
					}
				}
			}
		}
		
		public static string MeshPathInAsset(Mesh mesh, GameObject asset) {
			if (asset == null || mesh == null) return null;
						
			Stack<Transform> searchStack = new Stack<Transform>();
			Transform root = asset.transform;
			Transform target = null;
			searchStack.Push(root);
			while (searchStack.Count > 0) {
				Transform t = searchStack.Pop();
				int numChildren = t.childCount;
				for (int i = 0; i < numChildren; ++i) searchStack.Push(t.GetChild(i));
				
				MeshFilter mf = t.GetComponent<MeshFilter>();
				if (mf && mf.sharedMesh == mesh) {
					target = t;
					break;
				} else {
					SkinnedMeshRenderer smr = t.GetComponent<SkinnedMeshRenderer>();
					if (smr && smr.sharedMesh == mesh) {
						target = t;
						break;
					}
				}
			}		
			string result = mesh.name;
			while (target != null && target != root) {
				result = target.name + "/" + result;
				target = target.parent;
			}			
			return result;
		}
	}
}
