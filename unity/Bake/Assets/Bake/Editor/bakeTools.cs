using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BakeTools :EditorWindow {


	void NewOAAnUV2()
	{
		
	}
	static int selectedSize = 512;

	bool isRootAtParent = true;
	static bool addBorder = true;
	void OnGUI()
	{
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		string[] names = new string[] {"128","256", "512","1024"};
		int[] sizes = {128,256, 512, 1024};
		selectedSize = EditorGUILayout.IntPopup("烘焙贴图大小: ", selectedSize, names, sizes);
		if (GUILayout.Button ("BAKEMOD_ON")) {
			Shader.EnableKeyword ("BAKEMOD_ON");
		}
		if (GUILayout.Button ("BAKEMOD_OFF")) {
			Shader.DisableKeyword ("BAKEMOD_ON");
		}
		isRootAtParent = EditorGUILayout.Toggle("父节点为根节点", isRootAtParent);
		addBorder = EditorGUILayout.Toggle("光照贴图描边", addBorder);
		if (GUILayout.Button ("烘焙")) {
			if (Selection.activeObject is GameObject) {
				GameObject g = (GameObject)Selection.activeObject;
				if ( g.GetComponent<MeshFilter>() != null) {
					BakeObj (g,isRootAtParent);
				}
				else
				{
					EditorUtility.DisplayDialog ("", "所选物体没有MeshFilter", "ok");	
				}
			}
		}
			
		if (GUILayout.Button ("四方向烘焙")) {
			if (Selection.activeObject is GameObject) {
				GameObject g = (GameObject)Selection.activeObject;
				if ( g.GetComponent<MeshFilter>() != null) {
					BakeObj4Dir (g,isRootAtParent);
				}
				else
				{
					EditorUtility.DisplayDialog ("", "所选物体没有MeshFilter", "ok");	
				}
			}
		}
	}

	[MenuItem("TA/烘焙")]
	static void BakeSelectMod () {
		BakeTools window = (BakeTools)EditorWindow.GetWindow(typeof(BakeTools));
		window.Show();
	}

	static int   GetUV(int index, Vector3 v,Vector2 u,    Vector3[] vs,Vector2[] uv)
	{
		for (int i = 0; i < vs.Length; i++) {
		
			if (vs [i] == v && uv [i] == u)
				return i;
		}
		Debug.LogError ( index.ToString()+" not found ");
		return 0;
	}
	static void MakeUV2AndMoveToUV4(MeshFilter f0 )
	{
		var mesh =  f0.sharedMesh;
		Vector3[] vs = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector2[] uv2 = mesh.uv2;
		Unwrapping.GenerateSecondaryUVSet (mesh);
		Vector3[] n_vertices = mesh.vertices;
		Vector2[] n_uvs = mesh.uv;
		Vector2[] n_uv3 = mesh.uv2;
		Vector2[] n_uv2 = new Vector2[n_uv3.Length];
		for (int i = 0; i < n_uv3.Length; i++) {
			n_uv2 [i] = uv2[GetUV (i,n_vertices[i],n_uvs[i], vs,uv)];
		}
		mesh.uv2 = n_uv2;
		mesh.uv3 = n_uv3;
		mesh.RecalculateNormals ();

	}
	 
	static void BakeObj4Dir(GameObject g,bool isRootAtParent)
	{
		MeshFilter f0 = g.GetComponent<MeshFilter> ();
		MeshRenderer r0 = g.GetComponent<MeshRenderer> ();

		Transform root = g.transform;
		if (isRootAtParent)
			root = g.transform.parent;
		Vector3 oldPos = root.position;
		root.position = Vector3.zero;
		root.forward = new  Vector3 (0, 0, 1);
		Mesh cMesh = f0.sharedMesh;
 
		{
			cMesh = new Mesh ();
			CombineInstance[] combine = new CombineInstance[1];
			combine[0].mesh = f0.sharedMesh;
			combine[0].transform =  f0.transform.localToWorldMatrix;

			combine[0].transform = f0.transform.localToWorldMatrix ;

			cMesh.CombineMeshes(combine);
		}
		root.position = oldPos;


		GameObject g2 = new GameObject ();
		MeshFilter f = g2.AddComponent<MeshFilter>();
		f.sharedMesh = cMesh;
		MeshRenderer mr = g2.AddComponent < MeshRenderer> ();
		mr.sharedMaterial = r0.sharedMaterial;

	 

		var bounds = cMesh.bounds ;
		GameObject cam = new GameObject ();
		Camera c = cam.AddComponent<Camera> ();
		c.backgroundColor = Color.gray; 
		c.clearFlags = CameraClearFlags.SolidColor;
		c.orthographic = true;
		c.orthographicSize = Mathf.Max (bounds.size.x*4, bounds.size.y*4);
		c.farClipPlane = bounds.size.z * 4;
		c.aspect = 1;
		c.transform.forward = new Vector3 (0, 0, -1);
		c.transform.position = Vector3.zero +  new Vector3 (0, 0, bounds.size.z*2);
	 
		RenderTexture tex = new RenderTexture (selectedSize, selectedSize, 24);
		//RenderTexture tex2 = new RenderTexture (selectedSize, selectedSize, 24);
		c.targetTexture = tex;
		c.enabled = false;
		c.cullingMask = 1 << 15;

		g2.layer = 15;
		int halfWidth = selectedSize / 2;

		c.transform.parent = g2.transform;
		Shader.EnableKeyword ("BAKEMOD_ON");

		g2.transform.position = new Vector3 (0, 10000, 0);

		//偏移1像素
		float offset = 1.0f / selectedSize/2;
		if (addBorder) {
			c.rect = new Rect (0+offset, 0, 0.5f+offset, 0.5f);
			g2.transform.forward = new Vector3 (0, 0, 1);
			c.Render ();
			c.rect = new Rect (0.5f+offset, 0, 0.5f+offset, 0.5f);
			g2.transform.forward = new Vector3 (1, 0, 0);
			c.Render ();
			c.rect = new Rect (0f+offset, 0.5f, 0.5f, 0.5f);
			g2.transform.forward = new Vector3 (0, 0, -1);
			c.Render ();
			c.rect = new Rect (0.5f+offset, 0.5f, 0.5f+offset, 0.5f);
			g2.transform.forward = new Vector3 (-1, 0, 0);
			c.Render ();
		
			c.clearFlags = CameraClearFlags.Depth;
 
			c.rect = new Rect (0, 0+offset, 0.5f, 0.5f+offset);
			g2.transform.forward = new Vector3 (0, 0, 1);
			c.Render ();
			c.rect = new Rect (0.5f, 0+offset, 0.5f, 0.5f+offset);
			g2.transform.forward = new Vector3 (1, 0, 0);
			c.Render ();
			c.rect = new Rect (0f, 0.5f+offset, 0.5f, 0.5f+offset);
			g2.transform.forward = new Vector3 (0, 0, -1);
			c.Render ();
			c.rect = new Rect (0.5f, 0.5f+offset, 0.5f, 0.5f+offset);
			g2.transform.forward = new Vector3 (-1, 0, 0);
			c.Render ();
			offset = offset*2;

			c.rect = new Rect (0+offset, 0, 0.5f+offset, 0.5f);
			g2.transform.forward = new Vector3 (0, 0, 1);
			c.Render ();
			c.rect = new Rect (0.5f+offset, 0, 0.5f+offset, 0.5f);
			g2.transform.forward = new Vector3 (1, 0, 0);
			c.Render ();
			c.rect = new Rect (0f+offset, 0.5f, 0.5f, 0.5f);
			g2.transform.forward = new Vector3 (0, 0, -1);
			c.Render ();
			c.rect = new Rect (0.5f+offset, 0.5f, 0.5f+offset, 0.5f);
			g2.transform.forward = new Vector3 (-1, 0, 0);
			c.Render ();

			c.clearFlags = CameraClearFlags.Depth;

			c.rect = new Rect (0, 0+offset, 0.5f, 0.5f+offset);
			g2.transform.forward = new Vector3 (0, 0, 1);
			c.Render ();
			c.rect = new Rect (0.5f, 0+offset, 0.5f, 0.5f+offset);
			g2.transform.forward = new Vector3 (1, 0, 0);
			c.Render ();
			c.rect = new Rect (0f, 0.5f+offset, 0.5f, 0.5f+offset);
			g2.transform.forward = new Vector3 (0, 0, -1);
			c.Render ();
			c.rect = new Rect (0.5f, 0.5f+offset, 0.5f, 0.5f+offset);
			g2.transform.forward = new Vector3 (-1, 0, 0);
			c.Render ();

			 
		}

		c.rect = new Rect (0, 0, 0.5f, 0.5f);
		g2.transform.forward = new Vector3 (0, 0, 1);
		c.Render ();
		c.rect = new Rect (0.5f, 0, 0.5f, 0.5f);
		g2.transform.forward = new Vector3 (1, 0, 0);
		c.Render ();
		c.rect = new Rect (0f, 0.5f, 0.5f, 0.5f);
		g2.transform.forward = new Vector3 (0, 0, -1);
		c.Render ();
		c.rect = new Rect (0.5f, 0.5f, 0.5f, 0.5f);
		g2.transform.forward = new Vector3 (-1, 0, 0);
		c.Render ();
		c.rect = new Rect (0f, 0f, 1f, 1f);

		Shader.DisableKeyword ("BAKEMOD_ON");
		c.clearFlags = CameraClearFlags.SolidColor;
 
		g2.transform.forward = new Vector3 (0, 0, 1);
		g2.transform.position = oldPos + Vector3.left * 20;

 

		Texture t0 = r0.sharedMaterial.GetTexture ("_Albedo");
		Texture _SpecIBL = r0.sharedMaterial.GetTexture ("_SpecIBL");
		Texture _Metallic = r0.sharedMaterial.GetTexture ("_Metallic");

		string path = AssetDatabase.GetAssetPath(t0);
		path = path.Substring(0,path.Length -4 )+"bake4_dir"+selectedSize.ToString()+".png";
		 
 		
		//SaveRenderToPngAlpha (tex, tex2, path);
		SaveRenderToPng (tex,path);

		mr.sharedMaterial = new Material (Shader.Find("Unlit/DiffuseLightMap4Dir"));

		GameObject.DestroyImmediate (cam);
		GameObject.DestroyImmediate (tex);
		//GameObject.DestroyImmediate (tex2);

		AssetDatabase.ImportAsset (path);
		Texture2D t2 = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		mr.sharedMaterial.SetTexture ( "_MainTex",t0); 
		mr.sharedMaterial.SetTexture ( "_LightTex",t2); 
		mr.sharedMaterial.SetTexture ( "_SpecIBL",_SpecIBL); 
		mr.sharedMaterial.SetTexture ( "_Metallic",_Metallic); 



		g2.layer = g.layer;
	}
	static void BakeObj(GameObject g,bool isRootAtParent)
	{
		MeshFilter f0 = g.GetComponent<MeshFilter> ();
		MeshRenderer r0 = g.GetComponent<MeshRenderer> ();

		Transform root = g.transform;
		if (isRootAtParent)
			root = g.transform.parent;
		Vector3 oldPos = root.position;
		root.position = Vector3.zero;
		root.forward = new  Vector3 (0, 0, 1);
		Mesh cMesh = f0.sharedMesh;
		{
			cMesh = new Mesh ();
			CombineInstance[] combine = new CombineInstance[1];
			combine[0].mesh = f0.sharedMesh;
			combine[0].transform =  f0.transform.localToWorldMatrix;

			combine[0].transform = f0.transform.localToWorldMatrix ;

			cMesh.CombineMeshes(combine);
		}
		root.position = oldPos;


		GameObject g2 = new GameObject ();
		MeshFilter f = g2.AddComponent<MeshFilter>();
		f.sharedMesh = cMesh;
		MeshRenderer mr = g2.AddComponent < MeshRenderer> ();
		mr.sharedMaterial = r0.sharedMaterial;



		var bounds = cMesh.bounds ;
		GameObject cam = new GameObject ();
		Camera c = cam.AddComponent<Camera> ();
		c.backgroundColor = Color.gray; 
		c.clearFlags = CameraClearFlags.SolidColor;
		c.orthographic = true;
		c.orthographicSize = Mathf.Max (bounds.size.x*4, bounds.size.y*4);
		c.farClipPlane = bounds.size.z * 4;
		c.aspect = 1;
		c.transform.forward = new Vector3 (0, 0, -1);
		c.transform.position = Vector3.zero +  new Vector3 (0, 0, bounds.size.z*2);
		RenderTexture tex = new RenderTexture (selectedSize, selectedSize, 24);
		c.targetTexture = tex;
		c.enabled = false;
		c.cullingMask = 1 << 15;

		g2.layer = 15;

		int halfWidth = selectedSize / 2;

		c.transform.parent = g2.transform;
		Shader.EnableKeyword ("BAKEMOD_ON");

		g2.transform.position = new Vector3 (0, 10000, 0);



		//c.rect = new Rect (0.5f, 0, 0.5f, 0.5f);
		g2.transform.forward = new Vector3 (1, 0, 0);
		c.Render ();


		Shader.DisableKeyword ("BAKEMOD_ON");
	 
		g2.transform.forward = new Vector3 (0, 0, 1);
		g2.transform.position = oldPos + Vector3.left * 20;


		Texture t0 = r0.sharedMaterial.GetTexture ("_Albedo");
		string path = AssetDatabase.GetAssetPath(t0);
		path = path.Substring(0,path.Length -4 )+"bake"+selectedSize.ToString()+".png";
		SaveRenderToPng (tex,path);

		mr.sharedMaterial = new Material (Shader.Find("Unlit/DiffuseLightMap"));


		GameObject.DestroyImmediate (cam);
		GameObject.DestroyImmediate (tex);

		AssetDatabase.ImportAsset (path);
		Texture2D t2 = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		Texture _Metallic = r0.sharedMaterial.GetTexture ("_Metallic");

		mr.sharedMaterial.SetTexture ( "_MainTex",t0); 
		mr.sharedMaterial.SetTexture ( "_LightTex",t2); 
		mr.sharedMaterial.SetTexture ( "_Metallic",_Metallic); 
		g2.layer = g.layer;


			 
	}

	static void CreateNewPerfab(GameObject obj, string localPath)
	{
		Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
		PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
	}
	static public void SaveRToRChangeUnity(Texture2D target, Texture2D alpha)
	{
		string path = AssetDatabase.GetAssetPath(target);

		 
		RenderTexture rt0 = new RenderTexture (target.width, target.height, 0);
		Graphics.Blit (target, rt0);

		Texture2D target2= new Texture2D (target.width, target.height,  TextureFormat.ARGB32,false);
		RenderTexture.active = rt0;
		target2.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
		target2.Apply ();


		RenderTexture rt = new RenderTexture (target.width, target.height, 0);
		Graphics.Blit (alpha, rt);
		Texture2D a = new Texture2D (target.width, target.height,  TextureFormat.ARGB32,true);
		RenderTexture.active = rt;
		a.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
		a.Apply ();



		var ary0 = target2.GetPixels ();
		var ary1 = a.GetPixels ();
		for (int i = 0; i < ary0.Length; i++) {
			//unity 保存图片会去掉alpha为0的像素的rgb位颜色、
			ary0 [i] = new Color (ary0 [i].r, ary0 [i].g, ary0 [i].b, ary1 [i].r * 0.5f + 0.5f);
		}
		target2.SetPixels (ary0);
		target2.Apply ();

	

		//CreateNewPerfab (target2,path);
	 
		byte[] b = target2.EncodeToPNG();
		System.IO.File.WriteAllBytes (path,b);
		GameObject.DestroyImmediate (a);
		RenderTexture.active = null;
	 
	}

	static public void SaveRenderToPng(RenderTexture renderT, string sysPath)
	{
		int width = renderT.width;
		int height = renderT.height;
		Texture2D tex2d = new Texture2D(width, height, TextureFormat.ARGB32, false);
		RenderTexture.active = renderT;
		tex2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex2d.Apply();
		byte[] b = tex2d.EncodeToPNG();
		System.IO.File.WriteAllBytes (sysPath,b);
		GameObject.DestroyImmediate (tex2d);
	}
	static public void SaveRenderToPngAlpha(RenderTexture renderT, RenderTexture renderAlpha,string sysPath)
	{
		int width = renderT.width;
		int height = renderT.height;
		Texture2D tex2d = new Texture2D(width, height, TextureFormat.ARGB32, false);
		Texture2D tex2dAlpha = new Texture2D(width, height, TextureFormat.ARGB32, false);
		RenderTexture.active = renderT;
		tex2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex2d.Apply();
		RenderTexture.active = renderAlpha;
		tex2dAlpha.ReadPixels(new Rect(0, 0, renderAlpha.width, renderAlpha.height), 0, 0);
		tex2dAlpha.Apply();
		int hW = width/2;
		int hH = height / 2;
		float delta0 = 1.0f / 255;
		var ary0 = tex2d.GetPixels ();
		var ary1 = tex2dAlpha.GetPixels ();
		for (int i = 0; i < ary0.Length; i++) {
			//unity 保存图片会去掉alpha为0的像素的rgb位颜色、
			ary0 [i] = new Color (ary0 [i].r, ary0 [i].g, ary0 [i].b, ary1 [i].r * 0.5f + 0.5f);
		}
		tex2d.SetPixels (ary0);
		tex2d.Apply ();
		byte[] b = tex2d.EncodeToPNG();
		System.IO.File.WriteAllBytes (sysPath,b);
		GameObject.DestroyImmediate (tex2d);
		RenderTexture.active = null;
	}
 

}
