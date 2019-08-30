using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Common;

class AssetBuildBundleInfo
{
	public string fileName;
	public string assetName;
	public string bundleName;
	public List<string> dependencies;

	public void AddDependence(string dep)
	{
		if (dependencies == null) {
			dependencies = new List<string> ();
		}
		dependencies.Add (dep);
	}
}

public class ABBuild {
    // 资源目录

    private static string[] RES_DIRS = new string[] { "Assets/Test/" , "Assets/TestScene/" };
 
    private static string BASE_PATH = "Assets/";
	// 打包输出目录
	private static string RES_OUTPUT_PATH = "Assets/StreamingAssets";
	// AssetBundle打包后缀
	private static string ASSET_BUNDLE_SUFFIX = ".unity3d";
	// xml文件生成器
	private static XMLDocment doc;
	// bundleName <-> List<AssetBuildBundleInfo>
	private static Dictionary<string, List<AssetBuildBundleInfo>> bundleMap = new Dictionary<string, List<AssetBuildBundleInfo>>();
	// 文件名 <-> AssetBuildBundleInfo
	private static Dictionary<string, AssetBuildBundleInfo> fileMap = new Dictionary<string, AssetBuildBundleInfo>();

    [MenuItem("TA/OldAssets/打包", false, 351)]
    public static void Pack()
    {
        // 清理输出目录
        CreateOrClearOutPath();

        // 清理之前设置过的bundleName
        ClearAssetBundleName();

        // 设置bunderName
        bundleMap.Clear();
        List<string> resList = new List<string>();
        for (int i = 0; i < RES_DIRS.Length; i++)
        {
            resList.AddRange(GetAllResDirs(RES_DIRS[i]));
        }
		foreach (string dir in resList) {
			setAssetBundleName (dir);
		}

		// 打包
		BuildPipeline.BuildAssetBundles(RES_OUTPUT_PATH, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64);
		AssetDatabase.Refresh ();

		// 构建依赖关系
		AssetBundle assetBundle = AssetBundle.LoadFromFile (FileUtils.getPath ("StreamingAssets"));
		AssetBundleManifest mainfest = assetBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		string[] bundleNames = mainfest.GetAllAssetBundles ();
		foreach (string bundleName in bundleNames) {
			string[] deps = mainfest.GetAllDependencies (bundleName);
			foreach (string dep in deps) {
				List<AssetBuildBundleInfo> infoList = null;
				bundleMap.TryGetValue (bundleName, out infoList);
				if (null != infoList) {
					foreach (AssetBuildBundleInfo info in infoList) {
						info.AddDependence (dep);
					}
				}
			}
		}

        assetBundle.Unload(true);
        assetBundle = null;
        
        // 生成XML
        doc = new XMLDocment();
		doc.startObject ("files");
		foreach (KeyValuePair<string, AssetBuildBundleInfo> pair in fileMap) {
			AssetBuildBundleInfo info = pair.Value;

			doc.startObject ("file");
			doc.createElement ("bundleName", info.bundleName);
			doc.createElement ("fileName", info.fileName);
			doc.createElement ("assetName", info.assetName);

			if (null != info.dependencies) {
				doc.startObject ("deps");
				foreach (string dep in info.dependencies) {
					doc.createElement ("dep", dep);
				}
				doc.endObject ("deps");
			}
			doc.endObject ("file");
		}
		doc.endObject ("files");

		FileStream fs = new FileStream (Path.Combine (RES_OUTPUT_PATH, "StreamingAssets.xml"), FileMode.Create);
		byte[] data = System.Text.Encoding.UTF8.GetBytes (doc.ToString ());
		fs.Write (data, 0, data.Length);
		fs.Flush ();
		fs.Close ();

        
        // 打包后的清理
        ClearOutPath();
	}

	/// <summary>
	/// 设置AssetBundleName
	/// </summary>
	/// <param name="fullpath">Fullpath.</param>
	public static void setAssetBundleName(string fullPath) 
	{
		string[] files = System.IO.Directory.GetFiles (fullPath);
		if (files == null || files.Length == 0) {
			return;
		}

		Debug.Log ("Set AssetBundleName Start......");
		string dirBundleName = fullPath.Substring (BASE_PATH.Length);
		dirBundleName = dirBundleName.Replace ("/", "@") + ASSET_BUNDLE_SUFFIX;
		foreach (string file in files) {
			if (file.EndsWith (".meta")) {
				continue;
			}
			AssetImporter importer = AssetImporter.GetAtPath (file);
			if (importer != null) {
				string ext = System.IO.Path.GetExtension (file);
				string bundleName = dirBundleName;
				if (null != ext && (ext.Equals (".prefab")||ext.Equals(".unity"))) {
					// prefab单个文件打包
					bundleName = file.Substring (BASE_PATH.Length);
					bundleName = bundleName.Replace ("/", "@");
					if (null != ext) {
						bundleName = bundleName.Replace (ext, ext+ASSET_BUNDLE_SUFFIX);
					} else {
						bundleName += ASSET_BUNDLE_SUFFIX;
					}

				}
				bundleName = bundleName.ToLower ();
				Debug.LogFormat ("Set AssetName Succ, File:{0}, AssetName:{1}", file, bundleName);
				importer.assetBundleName = bundleName;
				EditorUtility.UnloadUnusedAssetsImmediate();

				// 存储bundleInfo
				AssetBuildBundleInfo info = new AssetBuildBundleInfo();
				info.assetName = file;
				info.fileName = file;
				info.bundleName = bundleName;
				/*if (null != ext) {
					info.fileName = file.Substring (0, file.IndexOf (ext));
				}*/
				fileMap.Add (file, info);

				List<AssetBuildBundleInfo> infoList = null;
				bundleMap.TryGetValue(info.bundleName, out infoList);
				if (null == infoList) {
					infoList = new List<AssetBuildBundleInfo> ();
					bundleMap.Add (info.bundleName, infoList);
				}
				infoList.Add (info);
			} else {
				Debug.LogFormat ("Set AssetName Fail, File:{0}, Msg:Importer is null", file);
			}
		}
		Debug.Log ("Set AssetBundleName End......");


	}

	/// <summary>
	/// 获取所有资源目录
	/// </summary>
	/// <returns>The res all dir path.</returns>
	public static List<string> GetAllResDirs(string fullPath)
	{
		List<string> dirList = new List<string> ();

		// 获取所有子文件
		GetAllSubResDirs (fullPath, dirList);

		return dirList;
	}

	/// <summary>
	/// 递归获取所有子目录文件夹
	/// </summary>
	/// <param name="fullPath">当前路径</param>
	/// <param name="dirList">文件夹列表</param>
	public static void GetAllSubResDirs(string fullPath, List<string> dirList)
	{
		if ((dirList == null) || (string.IsNullOrEmpty(fullPath)))
			return;

		string[] dirs = System.IO.Directory.GetDirectories (fullPath);
		if (dirs != null && dirs.Length > 0) {
			for (int i = 0; i < dirs.Length; ++i) {
				GetAllSubResDirs (dirs [i], dirList);
			}
		} else {
			dirList.Add (fullPath);
		}
	}

	/// <summary>
	/// 创建和清理输出目录
	/// </summary>
	public static void CreateOrClearOutPath()
	{
		if (!System.IO.Directory.Exists (RES_OUTPUT_PATH)) {
			// 不存在创建
			System.IO.Directory.CreateDirectory (RES_OUTPUT_PATH);
		} else {
			// 存在就清理
			System.IO.Directory.Delete(RES_OUTPUT_PATH, true);
			System.IO.Directory.CreateDirectory (RES_OUTPUT_PATH);
		}
	}

	/// <summary>
	/// 清理打包目录
	/// </summary>
	public static void ClearOutPath()
	{
//		string[] files = System.IO.Directory.GetFiles (RES_OUTPUT_PATH, "*.manifest", System.IO.SearchOption.AllDirectories);
//		foreach (string file in files) {
//			if (file.EndsWith ("StreamingAssets.manifest")) {
//				continue;
//			}
//			System.IO.File.Delete (file);
//		}
	}

	/// <summary>
	/// 清理之前设置的bundleName
	/// </summary>
	public static void ClearAssetBundleName()
	{
		string[] bundleNames = AssetDatabase.GetAllAssetBundleNames ();
		for (int i = 0; i < bundleNames.Length; i++) {
			AssetDatabase.RemoveAssetBundleName (bundleNames [i], true);
		}
	}
}
