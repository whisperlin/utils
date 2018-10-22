using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;  

public class Export : Editor {

	public static string sourcePath = Application.dataPath + "/UI";  
	const string AssetBundlesOutputPath = "Assets/StreamingAssets";  

	[MenuItem("Tools/AssetBundle/Build")]  
	public static void BuildAssetBundle()  
	{  
		ClearAssetBundlesName ();  

		Pack (sourcePath);  

		AddRelays (sourcePath);  

		string outputPath = Path.Combine (AssetBundlesOutputPath,Platform.GetPlatformFolder(BuildTarget.StandaloneWindows));  
		if (!Directory.Exists (outputPath))  
		{  
			Directory.CreateDirectory(outputPath);  
		}  

 

		BuildPipeline.BuildAssetBundles (outputPath,0,BuildTarget.StandaloneWindows);  
		 
		//BuildTarget target
		AssetDatabase.Refresh ();  

		Debug.Log ("打包完成");  

	}  


	[MenuItem("Tools/AssetBundle/Build ad")]  
	public static void BuildAssetBundlead()  
	{  
		ClearAssetBundlesName ();  

		Pack (sourcePath);  

		AddRelays (sourcePath);  

		string outputPath = Path.Combine (AssetBundlesOutputPath,Platform.GetPlatformFolder(BuildTarget.Android));  
		if (!Directory.Exists (outputPath))  
		{  
			Directory.CreateDirectory(outputPath);  
		}  

 

		BuildPipeline.BuildAssetBundles (outputPath,0,BuildTarget.Android);  

		//BuildTarget target
		AssetDatabase.Refresh ();  

		Debug.Log ("打包完成");  

	} 

	/// <summary>  
	/// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包  
	/// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下  
	/// </summary>  
	static void ClearAssetBundlesName()  
	{  
		int length = AssetDatabase.GetAllAssetBundleNames ().Length;  
		Debug.Log (length);  
		string[] oldAssetBundleNames = new string[length];  
		for (int i = 0; i < length; i++)   
		{  
			oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];  
		}  

		for (int j = 0; j < oldAssetBundleNames.Length; j++)   
		{  
			AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j],true);  
		}  
		length = AssetDatabase.GetAllAssetBundleNames ().Length;  
		Debug.Log (length);  
	}  

	static void Pack(string source)  
	{  
		DirectoryInfo folder = new DirectoryInfo (source);  
		FileSystemInfo[] files = folder.GetFileSystemInfos ();  
		int length = files.Length;  
		for (int i = 0; i < length; i++) {  
			if(files[i] is DirectoryInfo)  
			{  
				Pack(files[i].FullName);  
			}  
			else  
			{  
				if(!files[i].Name.EndsWith(".meta"))  
				{  
					file (files[i].FullName);  
				}  
			}  
		}  
	}  


	static void AddRelays(string source)  
	{  
		DirectoryInfo folder = new DirectoryInfo (source);  
		FileSystemInfo[] files = folder.GetFileSystemInfos ();  
		int length = files.Length;  
		for (int i = 0; i < length; i++) {  
			if(files[i] is DirectoryInfo)  
			{  
				AddRelays(files[i].FullName);  
			}  
			else  
			{  
				if(!files[i].Name.EndsWith(".meta"))  
				{  
					getRelays (files[i].FullName);  
				}  
			}  
		}  
	}
	static void getRelays(string source)  
	{  
		string _source = Replace (source);  
		string _assetPath = "Assets" + _source.Substring (Application.dataPath.Length);  

		GameObject g = AssetDatabase.LoadAssetAtPath(_assetPath,typeof(GameObject)) as GameObject;

		if (null != g) {
			Image [] img = g.GetComponentsInChildren<Image> ();
			if (img.Length > 0) {

				ImageData data = g.GetComponent<ImageData> ();
				if (null == data) {
					data = g.AddComponent<ImageData> ();
				}

				data.images.Clear ();
				data.paths.Clear ();
				data.names.Clear ();
				for (int i = 0; i < img.Length; i++) {
					string path = AssetDatabase.GetAssetPath (img [i].sprite.GetInstanceID ());
					AssetImporter assetImporter = AssetImporter.GetAtPath (path); 
					data.images.Add (img [i]);
					Debug.Log (img [i].name);
					Debug.Log (path);
					Debug.Log ("Add To Paths");
					data.paths.Add (assetImporter.assetBundleName);
					data.names.Add (img [i].sprite.name);
					//data.images
					Debug.LogWarning (assetImporter.assetBundleName);

				}

				//
			}
		}
	}

	static void file(string source)  
	{  
		string _source = Replace (source);  
		string _assetPath = "Assets" + _source.Substring (Application.dataPath.Length);  
		string _assetPath2 = _source.Substring (Application.dataPath.Length + 1);  
		//Debug.Log (_assetPath);  

		//在代码中给资源设置AssetBundleName  
		AssetImporter assetImporter = AssetImporter.GetAtPath (_assetPath); 
		string assetName = _assetPath2.Substring (_assetPath2.IndexOf("/") + 1);  
		assetName = assetName.Replace(Path.GetExtension(assetName),".unity3d");  
		//Debug.Log (assetName);  
		assetImporter.assetBundleName = assetName;  
	}  

	static string Replace(string s)  
	{  
		return s.Replace("\\","/");  
	}  
}  

public class Platform   
{  
	public static string GetPlatformFolder(BuildTarget target)  
	{  
		switch (target)  
		{  
		case BuildTarget.Android:  
			return "Android";  
		case BuildTarget.iOS:  
			return "IOS";  
		case BuildTarget.WebPlayer:  
			return "WebPlayer";  
		case BuildTarget.StandaloneWindows:  
		case BuildTarget.StandaloneWindows64:  
			return "Windows";  
		case BuildTarget.StandaloneOSXIntel:  
		case BuildTarget.StandaloneOSXIntel64:  
		case BuildTarget.StandaloneOSXUniversal:  
			return "OSX";  
		default:  
			return null;  
		}  
	}  
} 