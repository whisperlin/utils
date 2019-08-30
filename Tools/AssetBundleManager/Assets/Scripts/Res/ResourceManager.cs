using System;
using Common;
using UnityEngine;

public class ResourceManager:Singleton<ResourceManager>
{
	private AssetLoader m_assetLoader = new AssetLoader();
	private ResourcesLoader m_resourceLoader = new ResourcesLoader();

	public ResourceManager ()
	{
		m_assetLoader.Initialize ("", null);
	}

	/// <summary>
	/// 同步加载动更资源
	/// </summary>
	/// <returns>资源内容</returns>
	/// <param name="fileName">文件名称</param>
	/// <typeparam name="T">资源类型</typeparam>
	public T LoadAsset<T> (string fileName) where T: UnityEngine.Object
	{
		T rtn = m_assetLoader.LoadAsset<T>(fileName);
		if (null != rtn) {
			return rtn;
		}

		return m_resourceLoader.LoadAsset<T>(fileName);
	}

	/// <summary>
	/// 异步加载动更资源
	/// </summary>
	/// <returns>资源内容</returns>
	/// <param name="fileName">文件名称</param>
	/// <typeparam name="T">资源类型</typeparam>
	public T LoadAssetAsync<T> (string fileName) where T: UnityEngine.Object
	{
		T rtn = m_assetLoader.LoadAssetAsync<T> (fileName);
		if (null != rtn) {
			return rtn;
		}

		return m_resourceLoader.LoadAssetAsync<T> (fileName);
	}

	/// <summary>
	/// 读取文本文件内容
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="fileName">File name.</param>
	public string LoadText (string fileName)
	{
		string rtn = m_assetLoader.LoadText (fileName);
		if (null != rtn) {
			return rtn;
		}

		return m_resourceLoader.LoadText (fileName);
	}
}

