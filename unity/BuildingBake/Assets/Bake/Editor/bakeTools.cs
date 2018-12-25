using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BakeTools :EditorWindow {

	public static int selectedSize = 512;
	static bool addBorder = true;
	void OnGUI()
	{
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		string[] names = new string[] {"256", "512", "1024","2048"};
		int[] sizes = {256, 512, 1024,2048};
		selectedSize = EditorGUILayout.IntPopup("烘焙贴图大小: ", selectedSize, names, sizes);
		addBorder = EditorGUILayout.Toggle("光照贴图描边", addBorder);
		if (GUILayout.Button ("烘焙")) {
			if (Selection.activeObject is GameObject) {
				GameObject g = (GameObject)Selection.activeObject;

				MeshFilter [] mfs = g.GetComponentsInChildren<MeshFilter> ();
				if( mfs.Length > 0 ) 
				{
					MeshFilter bakcOject = mfs [0];
					for (int i = 1; i < mfs.Length; i++) {
						var b0 = bakcOject.sharedMesh.bounds.size;
						var b1 = mfs [i].sharedMesh.bounds.size;
						float t0 = b0.x * b0.x + b0.y * b0.y + b0.z * b0.z;
						float t1 = b1.x * b1.x + b1.y * b1.y + b1.z * b1.z;
						if (t1 > t0) {
							bakcOject = mfs [i];
						}
					}
					BakeObj (bakcOject,g.transform);
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
				MeshFilter [] mfs = g.GetComponentsInChildren<MeshFilter> ();
				if( mfs.Length > 0 ) 
				{
					MeshFilter bakcOject = mfs [0];
					for (int i = 1; i < mfs.Length; i++) {
						var b0 = bakcOject.sharedMesh.bounds.size;
						var b1 = mfs [i].sharedMesh.bounds.size;
						float t0 = b0.x * b0.x + b0.y * b0.y + b0.z * b0.z;
						float t1 = b1.x * b1.x + b1.y * b1.y + b1.z * b1.z;
						if (t1 > t0) {
							bakcOject = mfs [i];
						}
					}
					BakeObj4Dir (bakcOject,g.transform);
				}
				else
				{
					EditorUtility.DisplayDialog ("", "所选物体没有MeshFilter", "ok");	
				}
			}
		}
		if (GUILayout.Button ("树烘焙")) {
			if (Selection.activeObject is GameObject) {
				GameObject g = (GameObject)Selection.activeObject;
				MeshFilter  fs = g.GetComponent<MeshFilter> ();
				if( fs != null ) 
				{
					BakeObj4DirTree (fs);

				}
				else
				{
					EditorUtility.DisplayDialog ("", "所选物体没有MeshFilter", "ok");	
				}
			}
		}

        if (GUILayout.Button("烘焙地形"))
        {
            if (Selection.activeObject is GameObject)
            {
                GameObject g = (GameObject)Selection.activeObject;
                if (g.GetComponent<MeshFilter>() != null)
                {
                    BakeTerrain(g);
                }
                else
                {
                    EditorUtility.DisplayDialog("", "所选物体没有MeshFilter", "ok");
                }
            }
        }
    }

	[MenuItem("TA/烘焙")]
	static void BakeSelectMod () {
		BakeTools window = (BakeTools)EditorWindow.GetWindow(typeof(BakeTools));
		window.Show();
	}

    [MenuItem("TA/BAKEMOD_ON")]
    static void BAKEMOD_ON()
    {
        Shader.EnableKeyword("BAKEMOD_ON");
    }
    [MenuItem("TA/BAKEMOD_Off")]
    static void BAKEMOD_Off()
    {
        Shader.DisableKeyword("BAKEMOD_ON");
    }

    public static GameObject BakeTerrain(GameObject g)
    {
        MeshFilter f0 = g.GetComponent<MeshFilter>();
        MeshRenderer r0 = g.GetComponent<MeshRenderer>();
        GameObject cam = new GameObject();
        Vector3 oldPos = g.transform.position;
        Quaternion oldRot = g.transform.rotation;
        g.transform.position = new Vector3(0, -1000, 0);
        g.transform.rotation = Quaternion.identity;
        Camera c = cam.AddComponent<Camera>();

        c.transform.forward = new Vector3(0, -1, 0);
        c.transform.position = g.transform.position + new Vector3(2500, 500,2500);
        c.orthographic = true;
        c.orthographicSize = 2000;
        c.farClipPlane = 1000;
        c.cullingMask = (1 << 13);
        RenderTexture tex = new RenderTexture(selectedSize, selectedSize, 24);
        c.targetTexture = tex;

        int savelayer = g.layer;
        g.layer = 13;

        Shader.EnableKeyword("BAKEMOD_ON");
        Shader.EnableKeyword("NO_USE_NORMAL");
        c.Render();

        c.enabled = false;
        Shader.DisableKeyword("BAKEMOD_ON");
        Shader.DisableKeyword("NO_USE_NORMAL");

        g.layer = savelayer;
        g.transform.position = oldPos;
        g.transform.rotation = oldRot;
        GameObject g2 = new GameObject();
        g2.transform.position = oldPos + Vector3.left * 20;
        MeshFilter f = g2.AddComponent<MeshFilter>();
        f.sharedMesh = f0.sharedMesh;
        Debug.Log(r0.sharedMaterial.mainTexture);
        Texture t0 = r0.sharedMaterial.GetTexture("_Control");
        string path = AssetDatabase.GetAssetPath(t0);
        Debug.Log("path = " + path);
        path = path.Substring(0, path.Length - 4) + "bake" + selectedSize.ToString() + ".png";
        SaveRenderToPng(tex, path);

        MeshRenderer mr = g2.AddComponent<MeshRenderer>();
        mr.sharedMaterial = new Material(Shader.Find("Unlit/TextureUV2"));

		string assetPath =  AssetDatabase.GetAssetPath (r0.sharedMaterial);
		assetPath = assetPath.Substring (0, assetPath.Length - 4);
		AssetDatabase.CreateAsset (mr.sharedMaterial, assetPath + "bake" + selectedSize.ToString () + ".mat");

        GameObject.DestroyImmediate(cam);
        GameObject.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);
        Texture2D t2 = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        mr.sharedMaterial.mainTexture = t2;
        return g2;
    }
	public static GameObject FixObjMeshRotation(GameObject g)
	{
		g.transform.position = Vector3.zero;
		g.transform.rotation = Quaternion.identity;
		MeshFilter [] mfs = g.GetComponentsInChildren<MeshFilter> ();
		if (mfs.Length > 0) {
			MeshFilter bakcOject = mfs [0];
			for (int i = 1; i < mfs.Length; i++) {
				var b0 = bakcOject.sharedMesh.bounds.size;
				var b1 = mfs [i].sharedMesh.bounds.size;
				float t0 = b0.x * b0.x + b0.y * b0.y + b0.z * b0.z;
				float t1 = b1.x * b1.x + b1.y * b1.y + b1.z * b1.z;
				if (t1 > t0) {
					bakcOject = mfs [i];
				}
			}
			if (bakcOject.transform.localRotation != Quaternion.identity) {
				Mesh cMesh = bakcOject.sharedMesh;

				string path0 = AssetDatabase.GetAssetPath ( bakcOject.sharedMesh);
				if (string.IsNullOrEmpty(path0))
				{
					// 如果直接获得路径失败，尝试获得当前选中对象的关联父Prefab
					var curSelectedParentPrefab = PrefabUtility.GetPrefabParent(bakcOject.sharedMesh);
					path0= AssetDatabase.GetAssetPath(curSelectedParentPrefab);
				}
				path0 = System.IO.Path.GetDirectoryName(path0) + "/"+ cMesh.name+"_basemesh.asset";

				cMesh = new Mesh ();
				CombineInstance[] combine = new CombineInstance[1];
				combine[0].mesh = bakcOject.sharedMesh;
				combine[0].transform =  bakcOject.transform.localToWorldMatrix;
				combine[0].transform = bakcOject.transform.localToWorldMatrix ;
				cMesh.CombineMeshes(combine);

				AssetDatabase.CreateAsset (cMesh,path0);

				bakcOject.sharedMesh = cMesh;
				bakcOject.transform.localRotation = Quaternion.identity;
			}
		}
		return g;
	}
	public static GameObject BakeObj4Dir(GameObject g )
	{
		 
		MeshFilter [] mfs = g.GetComponentsInChildren<MeshFilter> ();
		if( mfs.Length > 0 ) 
		{
			MeshFilter bakcOject = mfs [0];
			for (int i = 1; i < mfs.Length; i++) {
				var b0 = bakcOject.sharedMesh.bounds.size;
				var b1 = mfs [i].sharedMesh.bounds.size;
				float t0 = b0.x * b0.x + b0.y * b0.y + b0.z * b0.z;
				float t1 = b1.x * b1.x + b1.y * b1.y + b1.z * b1.z;
				if (t1 > t0) {
					bakcOject = mfs [i];
				}
			}
			return BakeObj4Dir (bakcOject,g.transform);
		}
		return null;
	}
	static GameObject BakeObj4Dir(MeshFilter f0 ,Transform root   )
	{
		MeshRenderer r0 = f0.gameObject .GetComponent<MeshRenderer> ();
		float cull = r0.sharedMaterial.GetFloat("_Cull");
		r0.sharedMaterial.SetFloat("_Cull",0);
		Vector3 oldPos = root.position;
		Vector3 oldForward = root.forward;
		root.position = Vector3.zero;
		root.forward = new  Vector3 (0, 0, 1);
		Mesh cMesh = f0.sharedMesh;
		{
			string path0 = AssetDatabase.GetAssetPath ( f0.sharedMesh);
			if (string.IsNullOrEmpty(path0))
			{
				// 如果直接获得路径失败，尝试获得当前选中对象的关联父Prefab
				var curSelectedParentPrefab = PrefabUtility.GetPrefabParent(f0.sharedMesh);
				path0= AssetDatabase.GetAssetPath(curSelectedParentPrefab);

			}
			path0 = System.IO.Path.GetDirectoryName(path0) + "/"+ cMesh.name+"_mesh.asset";

			cMesh = new Mesh ();
			CombineInstance[] combine = new CombineInstance[1];
			combine[0].mesh = f0.sharedMesh;
			combine[0].transform =  f0.transform.localToWorldMatrix;
			combine[0].transform = f0.transform.localToWorldMatrix ;
			cMesh.CombineMeshes(combine);
 
			AssetDatabase.CreateAsset (cMesh,path0);

		}
		root.position = oldPos;
		root.forward = oldForward;

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

		Shader.DisableKeyword ("BAKEMOD_ON");

		g2.transform.forward = new Vector3 (0, 0, 1);
		g2.transform.position = oldPos + Vector3.left * 20;

		Texture t0 = r0.sharedMaterial.GetTexture ("_Albedo");
		Texture _SpecIBL = r0.sharedMaterial.GetTexture ("_SpecIBL");
		Texture _Metallic = r0.sharedMaterial.GetTexture ("_Metallic");

		string path = AssetDatabase.GetAssetPath(t0);
		path = path.Substring(0,path.Length -4 )+"bake4_dir"+selectedSize.ToString()+".png";

		SaveRenderToPng (tex,path);
		mr.sharedMaterial = new Material (Shader.Find("Unlit/DiffuseLightMap4Dir"));
		GameObject.DestroyImmediate (cam);
		GameObject.DestroyImmediate (tex);

		AssetDatabase.ImportAsset (path);
		Texture2D t2 = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		mr.sharedMaterial.SetTexture ( "_MainTex",t0); 
		mr.sharedMaterial.SetTexture ( "_LightTex",t2); 
		mr.sharedMaterial.SetTexture ( "_SpecIBL",_SpecIBL); 
		mr.sharedMaterial.SetTexture ( "_Metallic",_Metallic); 
		mr.sharedMaterial.SetFloat("_SpecIBLPower", r0.sharedMaterial.GetFloat("_SpecIBLPower"));
		//g2.AddComponent<RotationBakeObj> ();

		g2.layer = f0.gameObject.layer;
		string assetPath =  AssetDatabase.GetAssetPath (r0.sharedMaterial);
		assetPath = assetPath.Substring (0, assetPath.Length - 4);
		AssetDatabase.CreateAsset (mr.sharedMaterial, assetPath + "bake" + selectedSize.ToString () + ".mat");

		var t = CopyTreeAndReplay(root,f0,root.parent,g2);
		//t.transform.position += t.transform.right * 5;

		r0.sharedMaterial.SetFloat("_Cull",cull);

		return t.gameObject;
		//拷贝整颗树.
	}


	static void BakeObj4DirTree(MeshFilter f0  )
	{
		

		Transform root = f0.transform;
		MeshRenderer r0 = f0.gameObject .GetComponent<MeshRenderer> ();
		float cull = r0.sharedMaterial.GetFloat("_Cull");
		r0.sharedMaterial.SetFloat("_Cull",0);

		Vector3 oldPos = root.position;
		Vector3 oldForward = root.forward;
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
		root.forward = oldForward;

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

		Shader.DisableKeyword ("BAKEMOD_ON");

		g2.transform.forward = new Vector3 (0, 0, 1);
		g2.transform.position = oldPos + Vector3.left * 20;

		Texture t0 = r0.sharedMaterial.GetTexture ("_Albedo");
 
		string path = AssetDatabase.GetAssetPath(t0);
		path = path.Substring(0,path.Length -4 )+"bake4_dir"+selectedSize.ToString()+".png";

		SaveRenderToPng (tex,path);
		mr.sharedMaterial = new Material (Shader.Find("Unlit/DiffuseLightMap4DirTree"));
		GameObject.DestroyImmediate (cam);
		GameObject.DestroyImmediate (tex);

		AssetDatabase.ImportAsset (path);
		Texture2D t2 = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		mr.sharedMaterial.SetTexture ( "_MainTex",t0); 
		mr.sharedMaterial.SetTexture ( "_LightTex",t2); 
		mr.sharedMaterial.SetFloat ("_Alphatest", r0.sharedMaterial.GetFloat ("_Alphatest"));

		mr.sharedMaterial.SetFloat ("leafWindPower", r0.sharedMaterial.GetFloat ("leafWindPower"));
		mr.sharedMaterial.SetFloat ("leafWindAtt", r0.sharedMaterial.GetFloat ("leafWindAtt"));
		mr.sharedMaterial.SetFloat ("trunkWindPower", r0.sharedMaterial.GetFloat ("trunkWindPower"));
		mr.sharedMaterial.SetFloat ("trunkWindAtt", r0.sharedMaterial.GetFloat ("trunkWindAtt"));
		mr.sharedMaterial.SetVector ("leafWindDir", r0.sharedMaterial.GetVector ("leafWindDir"));

	 
		mr.sharedMaterial.SetFloat ("LodInv", r0.sharedMaterial.GetFloat ("LodInv"));
		mr.sharedMaterial.SetFloat ("LodMax", r0.sharedMaterial.GetFloat ("LodMax"));

	
		g2.AddComponent<RotationBakeObj> ();

		g2.layer = f0.gameObject.layer;
		string assetPath =  AssetDatabase.GetAssetPath (r0.sharedMaterial);
		assetPath = assetPath.Substring (0, assetPath.Length - 4);
		AssetDatabase.CreateAsset (mr.sharedMaterial, assetPath + "bake" + selectedSize.ToString () + ".mat");

		g2.transform.position += root.transform.right * 5;
		//拷贝整颗树.

		r0.sharedMaterial.SetFloat("_Cull",cull);
	}
	static Transform CopyTreeAndReplay(Transform root, MeshFilter f0 ,Transform parent, GameObject g )
	{
		Transform cur = g.transform;
		if (root.GetComponent<MeshFilter> () == f0) {
			g.transform.parent = parent;
			g.transform.localPosition = root.localPosition;
			g.transform.forward = new Vector3 (0, 0, 1);
			g.transform.localScale = root.localScale;
			g.name = root.name;
		}
		else
		{
			GameObject g3 = new GameObject(root.name);
			g3.transform.parent = parent;
			g3.transform.position = root.position;
			g3.transform.localRotation = root.localRotation;
			g3.transform.localScale = root.localScale;
			cur = g3.transform;
			MeshFilter tmf = root.GetComponent<MeshFilter> ();
			if (tmf) {
				MeshFilter mf = g3.AddComponent<MeshFilter> ();
				mf.sharedMesh = tmf.sharedMesh;
				MeshRenderer mr = g3.AddComponent<MeshRenderer> ();
				mr.sharedMaterials = root.GetComponent<MeshRenderer> ().sharedMaterials ;
			}
		}
		int c = root.childCount;
		for (int i = 0; i < c; i++) {
			Transform t = root.GetChild (i);
			CopyTreeAndReplay (t, f0, cur, g);
		}
		return cur;
	}
	static void BakeObj(MeshFilter f0 ,Transform root)
	{
		MeshRenderer r0 = f0.gameObject .GetComponent<MeshRenderer> ();
		Vector3 oldPos = root.position;
		Vector3 oldForward = root.forward;
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
		root.forward = oldForward;


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


		float cull = r0.sharedMaterial.GetFloat("_Cull");
		r0.sharedMaterial.SetFloat("_Cull",0);
		c.Render ();
		r0.sharedMaterial.SetFloat("_Cull",cull);
 
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

		mr.sharedMaterial.SetTexture ( "_MainTex",t0); 
		mr.sharedMaterial.SetTexture ( "_LightTex",t2); 
		mr.sharedMaterial.SetTexture ( "_SpecIBL", r0.sharedMaterial.GetTexture("_SpecIBL")); 
		mr.sharedMaterial.SetTexture ( "_Metallic", r0.sharedMaterial.GetTexture("_Metallic")); 
		mr.sharedMaterial.SetFloat("_SpecIBLPower", r0.sharedMaterial.GetFloat("_SpecIBLPower"));
		g2.layer = f0.gameObject.layer;

		var t = CopyTreeAndReplay(root,f0,root.parent,g2);
		t.transform.position += t.transform.right * 5;
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
 

}
