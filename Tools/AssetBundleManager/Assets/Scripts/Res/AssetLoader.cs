#define USE_UNITY5_X_BUILD
// #define USE_LOWERCHAR
#define USE_HAS_EXT
#define USE_DEP_BINARY
#define USE_DEP_BINARY_AB
#define USE_ABFILE_ASYNC

// 是否使用LoadFromFile读取压缩AB
#define USE_LOADFROMFILECOMPRESS

//#define USE_WWWCACHE

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Common;
using System.Xml;

class AssetBundleInfo
{
	
	public string bundleName;
	public string assetName;
	public List<string> dependencies;

	public void AddDependence(string dep)
	{
		if (dependencies == null) {
			dependencies = new List<string> ();
		}
		dependencies.Add (dep);
	}


}

/// <summary>
/// Asset loader 动更资源加载器
/// </summary>
public class AssetLoader : IResourceLoader
{
	private Dictionary<string, AssetBundleInfo> fileMap = new Dictionary<string, AssetBundleInfo>();

	/// <summary>
	///  初始化加载器
	/// </summary>
	/// <param name="fileList">文件列表</param>
	/// <param name="initOK">初始化完成回调</param>
	public void Initialize(string fileList, Action initOK)
	{
		// 
		string fileName = "StreamingAssets.xml";
		string path = FileUtils.getPath(fileName);
		if (!System.IO.File.Exists (path)) {
			return;
		}
		XmlDocument doc = new XmlDocument ();
		doc.Load (path);
		XmlNodeList nodeList = doc.SelectSingleNode ("files").ChildNodes;
		foreach (XmlElement xe in nodeList) {
			AssetBundleInfo info = new AssetBundleInfo ();
			string _fileName = xe.SelectSingleNode("fileName").InnerText;
			info.assetName = xe.SelectSingleNode ("assetName").InnerText;
			info.bundleName = xe.SelectSingleNode ("bundleName").InnerText;
			XmlNode deps = xe.SelectSingleNode ("deps");
			if (null != deps) {
				XmlNodeList depList = deps.ChildNodes;
				foreach (XmlElement _xe in depList) {
					info.AddDependence (_xe.InnerText);
				}
			}
			fileMap.Add (_fileName.Substring("Assets/Resources/".Length), info);
		}
	}

	/// <summary>
	/// 同步加载动更资源
	/// </summary>
	/// <returns>资源内容</returns>
	/// <param name="fileName">文件名称</param>
	/// <typeparam name="T">资源类型</typeparam>
    public override T LoadAsset<T> (string fileName)
	{
		AssetBundleInfo info = null;
		fileMap.TryGetValue (fileName, out info);
		if (null == info) {
			return null;
		}
		string path = FileUtils.getPath(info.bundleName);
		if (!System.IO.File.Exists (path)) {
			return null;
		}
		AssetBundle asset = AssetBundle.LoadFromFile (path);
		if (null == asset) {
			return null;
		}

		if (null != info.dependencies) {
			foreach (string dep in info.dependencies) {
				AssetBundle _asset = AssetBundle.LoadFromFile (FileUtils.getPath (dep));
			}
		}

		return asset.LoadAsset<T> (info.assetName);
	}

	/// <summary>
	/// 异步加载动更资源
	/// </summary>
	/// <returns>资源内容</returns>
	/// <param name="fileName">文件名称</param>
	/// <typeparam name="T">资源类型</typeparam>
	public override T LoadAssetAsync<T> (string fileName)
	{
		string path = FileUtils.getPath(fileName);
		AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync (path);
		if (request.isDone) {
			return request.assetBundle.LoadAsset<T> (fileName);
		}

		return null;
	}

	/// <summary>
	/// 读取文本文件内容
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="fileName">File name.</param>
	public override string LoadText (string fileName)
	{
		throw new NotImplementedException ();
	}
}
