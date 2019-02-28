using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Texture2dArrayTool : EditorWindow {

	int toolBar = 0;
	[MenuItem("Tools/Texture2DArray工具")]
	static void Init()
	{
		EditorWindow.GetWindow<Texture2dArrayTool>("Texture2DArray工具").Show ();
	}
	List<Texture2D> editorArray = new List<Texture2D>();

	public static bool CheckTextureSize(Texture2D [] arys , out float width,out float height ,out TextureFormat format ,out string errorMessage)
	{
		width = 0;
		height = 0;
		format = TextureFormat.ARGB32;
		bool found = false;
		int index = 0;
		for (int i = 0; i < arys.Length; i++) {
			if (null == arys [i])
				continue;
			if (found) {
				if (arys [i].width != width || arys [i].height != height) {
					errorMessage = "第"+index.ToString()+"张图";
					errorMessage  += "与第"+i.ToString()+"张图";
					errorMessage  += "宽高不一致";
					return false;
				}
					
				if (format != arys [i].format) {
					errorMessage = "第"+index.ToString()+"张图";
					errorMessage += "格式" + format.ToString ();
					errorMessage  += "与第"+i.ToString()+"张图";
					errorMessage += "格式" +  arys [i].format.ToString ();
					errorMessage  += "格式不一致";
					return false;
				}
			}
			else
			{
				width = arys [i].width;
				height = arys [i].height;
				format = arys [i].format;
				index = i;
				found = true;
			}
		}
		errorMessage = "";
		return true;
	}
	static Texture2DArray CreateTextureArray2D(Texture2D []layerTextures, string TexturePath,bool minMap )
	{
		int layerCount = layerTextures.Length;
		int layerTextureSize = 1024;
		TextureFormat format = TextureFormat.RGBA32;
		for (int i = 0; i <layerCount; i++)
		{
			if (null == layerTextures [i])
				continue;
			layerTextureSize = layerTextures[i].width;
			format = layerTextures[i].format ;
		}
		Texture2DArray textureArray = new Texture2DArray(layerTextureSize, layerTextureSize,
			layerCount, format, minMap);

		for (int i = 0; i < layerTextures.Length; i++)
		{
			if (null == layerTextures [i])
				continue;
			Graphics.CopyTexture(layerTextures[i], 0, textureArray, i);
		}

		textureArray.filterMode = FilterMode.Bilinear;
		textureArray.wrapMode = TextureWrapMode.Repeat;
		//textureArray.Apply();

		AssetDatabase.CreateAsset(textureArray, TexturePath);
		return textureArray;
	}
	bool bMiniMap = false;
	Texture2D tt;
	void OnConbine()
	{
		GUILayout.Label ("图片合成");
		bMiniMap = GUILayout.Toggle (bMiniMap, "min map");
		if (editorArray.Count == 0)
			editorArray.Add (null);
		 
		for (int i = 0; i < editorArray.Count; i++) {
			//tt = (Texture2D)EditorGUILayout.ObjectField("读入贴图贴图",tt,typeof(Texture2D)); 
			editorArray [i]  = (Texture2D)EditorGUILayout.ObjectField(i.ToString(),editorArray [i],typeof(Texture2D)); 
			//editorArray [i] = (Texture2D)EditorGUILayout.ObjectField (editorArray [i], typeof(Texture2D));
		}

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("添加")) {
			editorArray.Add (null);
		}
		if (GUILayout.Button ("删除")) {
			if (editorArray.Count > 1)
				editorArray.RemoveAt (editorArray.Count-1);
		}
		if (GUILayout.Button ("合成")) {
			float width;
			float height;
			TextureFormat format;
			string errorMessage;
			if (CheckTextureSize (editorArray.ToArray (), out width, out height, out   format, out  errorMessage)) {
				string path = EditorUtility.SaveFilePanelInProject("Save Texture2DArray",    "TextureName", "asset",
					"请输入保存文件名");
				if (path.Length != 0) 
				{
					CreateTextureArray2D(editorArray.ToArray(), path,bMiniMap );
				}
			} 
			else
			{
				EditorUtility.DisplayDialog ("", errorMessage, "ok");
			}
		}
		GUILayout.EndHorizontal ();
	}
	Texture2DArray tex;
	Editor gameObjectEditor;
	Shader perviewShader;
 
 
 
	Material defaultMat;
	Renderer r;
	int  showIndex = 0;
	PrimitiveType op = PrimitiveType.Sphere;
	void OnDestroy()
	{
		 
	}
 
	void ReadTextureFromArray()
	{
		if (null == tex)
			return;
		editorArray.Clear ();

		bMiniMap =  tex.mipMapBias > 0;
		for (int i = 0; i < tex.depth; i++) {
			var cols = tex.GetPixels (i);
			if (null == cols || cols.Length< 1) {
				editorArray.Add (null);
			}
			else
			{
				Texture2D temp = new Texture2D (tex.width, tex.height, TextureFormat.RGBA32, bMiniMap);
				temp.SetPixels(cols);
				temp.Apply ();
				editorArray.Add( temp);
			}
		}
	}
	void OnPerview()
	{
 
	 

		tex = (Texture2DArray)EditorGUILayout.ObjectField("读入贴图贴图",tex,typeof(Texture2DArray)); 
		 
		if (GUILayout.Button ("读入")) {
			ReadTextureFromArray ();
		}

		 
	}

	void OnGUI()
	{
		toolBar = GUILayout.Toolbar(toolBar, new GUIContent[]
			{
				new GUIContent("合成"),
				new GUIContent("读入"),
			});
		switch (toolBar) {
		case 0:
			{
				OnConbine ();
			}
			break;
		case 1:
			{
				OnPerview ();
			}
			break;
 
		}
		
	}
	 
	 
}
