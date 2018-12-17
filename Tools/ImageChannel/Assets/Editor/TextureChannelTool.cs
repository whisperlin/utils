using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextureChannelTool : EditorWindow {


	[MenuItem("Tools/图层通道合并")]
	static void Init()
	{
		EditorWindow.GetWindow<TextureChannelTool>("图层通道合并工具").Show ();
	}
	Texture2D[] editorArray = new Texture2D[4];


	void CombineMesh(string savePath)
	{
		Texture2D black = new Texture2D (1, 1, TextureFormat.RGBA32, false);
		black.SetPixel (0, 0, Color.black);
		black.Apply ();
		int width = 0;
		int height = 0;
		for (int i = 0; i < editorArray.Length; i++) {
			if (null == editorArray [i]) {
				editorArray [i] = black;
				continue;
			}
			width = Mathf.Max (width, editorArray [i].width);
			height = Mathf.Max (height, editorArray [i].height);
		}
		if (width == 0)
			return;
		RenderTexture [] temp = new RenderTexture[ editorArray.Length];
		Texture2D [] temp2 = new Texture2D[ editorArray.Length];
 
		for (int i = 0; i < editorArray.Length; i++) {
			temp[i]  = RenderTexture.GetTemporary (width, height);

			if (null != editorArray [i]) {
				Graphics.Blit (editorArray [i], temp [i]);
			}
		 

			temp2 [i] = new Texture2D (width, height,TextureFormat.RGBA32,false);
		 
			RenderTexture.active = 	temp[i] ;
			temp2 [i] .ReadPixels(new Rect(0, 0, width, height), 0, 0);
			temp2 [i].Apply ();
			RenderTexture.ReleaseTemporary (temp[i] );
		}

		Texture2D final = new Texture2D (width, height, TextureFormat.RGBA32,false);
		//不知道为什么转RGBA32通道R和A是反的.
		for(int i = 0 ; i < width ;i++)
		{
			for (int j = 0; j < height; j++) {
				final.SetPixel (i, j, new Color ( temp2[2].GetPixel(i,j).r ,  temp2[1].GetPixel(i,j).r ,  temp2[0].GetPixel(i,j).r ,temp2[3].GetPixel(i,j).r ));
			}
		}
		final.Apply ();
		for (int i = 0; i < editorArray.Length; i++) {
			GameObject.DestroyImmediate (temp2 [i]);
		}
		byte[] date = TgaUtil.Texture2DEx.EncodeToTGA (final, true);
		//byte [] date =  final.EncodeToPNG ();
		System.IO.File.WriteAllBytes (savePath, date);
		GameObject.DestroyImmediate (final);
		GameObject.DestroyImmediate (black);

	}
	void OnGUI()
	{
		GUILayout.Label ("R通道");
		editorArray [0] = (Texture2D)EditorGUILayout.ObjectField (editorArray [0], typeof(Texture2D));
		GUILayout.Label ("G通道");
		editorArray [1] = (Texture2D)EditorGUILayout.ObjectField (editorArray [1], typeof(Texture2D));
		GUILayout.Label ("B通道");
		editorArray [2] = (Texture2D)EditorGUILayout.ObjectField (editorArray [2], typeof(Texture2D));
		GUILayout.Label ("A通道");
		editorArray [3] = (Texture2D)EditorGUILayout.ObjectField (editorArray [3], typeof(Texture2D));
		 
		if (GUILayout.Button ("合成")) {
			bool found = false;
			for (int i = 0; i < 4; i++) {
				if (editorArray[i] != null)
					found = true;
			}
			if (!found) {
				EditorUtility.DisplayDialog ("提示", "没有图片呗选择", "ok");
				return;
			}
			string path = EditorUtility.SaveFilePanelInProject("Save Texture",    "TextureName", "tga",
				"请输入保存文件名");
			if (path.Length != 0) 
			{
				CombineMesh (path);
			}
		}
	}
}
