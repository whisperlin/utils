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
		int layerTextureSize = layerTextures[0].width;
		string formatString = layerTextures[0].format.ToString();
		Texture2DArray textureArray = new Texture2DArray(layerTextureSize, layerTextureSize,
			layerCount, layerTextures[0].format, minMap);

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
	void OnConbine()
	{
		GUILayout.Label ("图片合成");
		bMiniMap = GUILayout.Toggle (bMiniMap, "min map");
		if (editorArray.Count == 0)
			editorArray.Add (null);
		for (int i = 0; i < editorArray.Count; i++) {
			editorArray [i] = (Texture2D)EditorGUILayout.ObjectField (editorArray [i], typeof(Texture2D));
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
	GameObject obj;
	GameObject empty;
	Material mat;
	Material defaultMat;
	Renderer r;
	int  showIndex = 0;
	PrimitiveType op = PrimitiveType.Sphere;
	void OnDestroy()
	{
		if (null != obj) {
			GameObject.DestroyImmediate (mat);
			GameObject.DestroyImmediate (obj);
			GameObject.DestroyImmediate (empty);

		}
	}
	void OnPerview()
	{
 
		if (null == mat) {
			empty =  GameObject.CreatePrimitive (PrimitiveType.Sphere);

			mat = new Material (Shader.Find ("Perview/Texture3dArray"));
			obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			obj.hideFlags = HideFlags.HideAndDontSave;
			r = obj.GetComponent<Renderer> ();
			gameObjectEditor = Editor.CreateEditor (obj);
			defaultMat = r.sharedMaterial;
			r.sharedMaterial = mat;
		}


		tex = (Texture2DArray)EditorGUILayout.ObjectField("预览贴图",tex,typeof(Texture2DArray)); 
		showIndex = EditorGUILayout.IntSlider (showIndex, 0, 20);

		PrimitiveType op1 = (PrimitiveType)EditorGUILayout.EnumPopup("预览类型", op);
		if (op1 != op) {
			GameObject.DestroyImmediate (obj);
			obj = GameObject.CreatePrimitive (op1);
			obj.hideFlags = HideFlags.HideAndDontSave;
			r = obj.GetComponent<Renderer> ();
			gameObjectEditor = Editor.CreateEditor (obj);
			r.sharedMaterial = mat;
			op = op1;
		}
		//obj = (GameObject)EditorGUILayout.ObjectField("预览贴图",obj,typeof(GameObject)); 

		if (tex) {
			mat.SetTexture ("_TextureArray", tex);
			mat.SetInt ("_Index", showIndex);
			r.sharedMaterial = mat;
		}
		GUIStyle bgColor = new GUIStyle();

		if (tex) {
			gameObjectEditor.OnInteractivePreviewGUI (GUILayoutUtility.GetRect (256, 256), bgColor);
		} else {
			 
			gameObjectEditor.OnInteractivePreviewGUI (GUILayoutUtility.GetRect (256, 256), bgColor);

		}
	}

	void OnGUI()
	{
		toolBar = GUILayout.Toolbar(toolBar, new GUIContent[]
			{
				new GUIContent("合成"),
				new GUIContent("预览"),
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
