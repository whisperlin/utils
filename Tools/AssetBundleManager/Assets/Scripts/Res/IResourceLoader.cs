using System;
using UnityEngine;

/// <summary>
/// 资源加载器定义
/// </summary>
public abstract class IResourceLoader
{
	/// <summary>
	/// 同步加载动更资源
	/// </summary>
	/// <returns>资源内容</returns>
	/// <param name="fileName">文件名称</param>
	/// <typeparam name="T">资源类型</typeparam>
	public abstract T LoadAsset<T> (string fileName) where T: UnityEngine.Object; 

	/// <summary>
	/// 异步加载动更资源
	/// </summary>
	/// <returns>资源内容</returns>
	/// <param name="fileName">文件名称</param>
	/// <typeparam name="T">资源类型</typeparam>
	public abstract T LoadAssetAsync<T> (string fileName) where T: UnityEngine.Object;

	/// <summary>
	/// 读取文本文件内容
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="fileName">File name.</param>
	public abstract string LoadText (string fileName);


}
