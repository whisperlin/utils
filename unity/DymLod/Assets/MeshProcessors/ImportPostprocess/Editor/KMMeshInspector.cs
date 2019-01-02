using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(UnityEngine.Mesh))]
public class KMMeshInspector : Editor { 
	int previewMode = 0;
	
	KMProcessorProgram storedProg = null; // The program that is stored in the asset
	KMProcessorProgram prog = null; // A copy of the stored prog in memory to work on and then apply/revert
	KMProcessorProgramGUI progGUI = null; // the editor of the prog in memory
	
	UnityEngine.Mesh mesh = null; // the mesh the editor is working on
	//GameObject asset = null; // the mesh asset("prefab") the editor is working on
	string meshPath = null; // the path to the mesh inside the asset
	string assetPath = null; // the path to the asset in the Assets folder
	string assetGUID = null; // the GUID to the asset (more reliable than the path and used to index programs)
	
	bool searchedForProgram = false; // to avoid multiple searches for the prog, store the status
	bool hasChanges = false; // has the ram copy of the program been altered?
	
	private Editor builtInEditor = null;
	
	bool _assureBuiltInEditorCreated() {
		if (builtInEditor == null) {
			System.Type type = typeof(UnityEditor.Editor); // Can by any public class in the same assembly
			type = type.Assembly.GetType("UnityEditor.ModelInspector");
			builtInEditor = Editor.CreateEditor(targets, type);
		}
		return (builtInEditor != null);
	}
		
	void OnEnable() {
		// Debug.Log("Inspector OnEnable workingObj " + workingObj);
		storedProg = null;
		prog = null;
		hasChanges = false;
		progGUI = null;
		searchedForProgram = false;
		meshPath = null;
		mesh = null;
		// Don't fetch the prog here, as this will break the intial library buildup as it creates the settings before they are imported .. when creating previews
	}
	
	void OnDisable() {
		//Debug.Log("Inspector OnDisable workingObj: " + workingObj + " isPLaying " + Application.isPlaying);
		
		if (hasChanges && EditorUtility.DisplayDialog("Unapplied import settings", 
			"Unapplied import settings for '" + assetPath + "'", 
			"Apply", 
			"Revert")) {
			_applyProgramChanges();
		} 
		if (prog != null) {
			// First destroy the processors
			KrablMesh.Processor[] procs = prog.processors;
			for (int i = 0; i < procs.Length; ++i) {
				DestroyImmediate(procs[i]);
			}
			DestroyImmediate(prog);
			prog = null;
		}
		if (progGUI != null) {
			DestroyImmediate(progGUI);
			progGUI = null;
		}
	}
		
	void _initializeProgram() {
		storedProg = null; prog = null; progGUI = null;
		
		// Look for a program stored in the asset.
		storedProg = KMImportSettings.ProgramForMesh(assetGUID, meshPath);
		if (storedProg != null) {
			// Create a copy of the program to work with.
			prog = ScriptableObject.CreateInstance<KMProcessorProgram>();			
			EditorUtility.CopySerialized(storedProg, prog);
			int numProcessors = prog.processors.Length;
			for (int i = 0; i < numProcessors; ++i) {
				KrablMesh.Processor proc = prog.processors[i];
				KrablMesh.Processor clone = ScriptableObject.CreateInstance(proc.GetType()) as KrablMesh.Processor;
				EditorUtility.CopySerialized(proc, clone);
				clone.hideFlags = HideFlags.DontSave;
				prog.processors[i] = clone;
			}
			
			prog.hideFlags = HideFlags.DontSave;
			
			progGUI = (KMProcessorProgramGUI)Editor.CreateEditor(prog);
			
			if (progGUI != null) progGUI.usedByMeshImporter = true;
		}
	}
		
	override public void OnInspectorGUI() {
		DrawDefaultInspector();
		GUI.enabled = true;
		
		if (meshPath == null) {
			mesh = (UnityEngine.Mesh)target;
			assetPath = AssetDatabase.GetAssetPath(target);
			assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
			GameObject asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
			meshPath = KrablMesh.UnityEditorUtils.MeshPathInAsset(mesh, asset);
		}
					
		if (prog == null && searchedForProgram == false) {
			_initializeProgram();
			searchedForProgram = true;
		}
		
		// Mesh information on top.
		// Mesh path
		string path = "";
		if (assetPath != null && assetPath != "") path += assetPath.Replace("Assets/", "") + " - ";		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Mesh Path: ", GUILayout.Width(80.0f)); GUILayout.Label(path + meshPath);
		GUILayout.EndHorizontal();
		
		// Mesh description
		string desc;		
		if (prog != null) {
			desc = prog.inMeshDescription;
			if (desc == null || desc == "") desc = "n/a";
		} else {
			desc = KMProcessorProgram.MeshDescription(mesh);
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("Description: ", GUILayout.Width(80.0f)); GUILayout.Label(desc);
		GUILayout.EndHorizontal();
		
			
		GUI.enabled = true;

		if (prog != null && progGUI != null) {
			// The program object draws all the processors etc.
			progGUI.DrawImporterGUI(this);
		}
		// prog and progGUI might have disappeared from delete, so check again
		if (prog != null && progGUI != null) {
			// Draw the part below the processors with Apply and Revert
			if (progGUI.modifiedDuringLastUpdate) {
				hasChanges = true;
			}
					
			GUI.enabled = hasChanges;
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Revert", GUILayout.ExpandWidth(false))) {
				_revertProgramChanges();
				return;
			}

			if (GUILayout.Button("Apply", GUILayout.ExpandWidth(false))) {
				GUI.FocusControl(""); // make sure text editor field get committed
				_applyProgramChanges();
			}
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;
		} else {
			// Draw the Add mesh import program section.
			EditorGUILayout.Separator();	
		
			bool canUseProgram = (assetPath != null && assetPath != "");
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5
			if (mesh.blendShapeCount > 0) canUseProgram = false;
#endif

			if (canUseProgram) {
				GUIStyle largeButtonStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("LargeButton");
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Add Mesh Import Program", largeButtonStyle)) {
					AddProgram();
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			} else {
				bool specialError = false;

#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5
				if (mesh.blendShapeCount > 0) {
					EditorGUILayout.HelpBox(
						"Meshes with blendshapes can not yet be processed by Krabl Mesh Processors.\n\n" +
						"Support will be added as soon as Unity exposes blendshape access in the API.", 
						MessageType.Info);
					specialError = true;
				}
#endif
				if (specialError == false) {
					EditorGUILayout.HelpBox(
						"Only meshes inside the project folder can have mesh import programs.\n\n"
						+ "Built-In meshes and scene meshes are never imported by Unity.", 
						MessageType.Info);
				}
			}
		}
	}
	
	void AddProgram() {
		// Create an empty program in memory 
		prog = KMImportSettings.CreateProgram(assetGUID, meshPath, mesh);
		// Now create the GUI
		progGUI = (KMProcessorProgramGUI)Editor.CreateEditor(prog);
		if (progGUI != null) progGUI.usedByMeshImporter = true;
		hasChanges = true;
	}
	
	// Called from progGUI
	public void DeleteProgram() {
		if (prog == null) return;
		bool del = true;
		if (prog.processors.Length > 0) {
			del = EditorUtility.DisplayDialog("Delete Import Program", "Do you want to permanently delete this mesh import program?", "Delete", "Cancel");
		}
		if (del) {
			if (storedProg != null) {
				EditorUtility.DisplayProgressBar(KMImportProcessor.ProgressTitle(prog), "Unity3D reimports model file", 0.0f);
				KMImportSettings.RemoveProgram(storedProg);
			
				// Trigger model reimport
				AssetDatabase.ImportAsset(assetPath);
				EditorUtility.ClearProgressBar();
			}
			prog = null; progGUI = null;
			storedProg = null;
		}
	}
	
	void _applyProgramChanges() {
		if (prog == null) return;
		
		prog.importTriggeredByInspector = true;
	
		// 1st erase the store program
		if (storedProg) {
			KMImportSettings.RemoveProgram(storedProg);
		}
		// now store the prog in memory to the asset
		KMImportSettings.AddProgram(prog);
		
		// Trigger mesh reimport -> import processor will execute program
		EditorUtility.DisplayProgressBar(KMImportProcessor.ProgressTitle(prog), "Unity3D reimports model file", 0.0f);
				
		KMImportSettings.IncrementMasterImportRevision();
				
		// float dt = Time.realtimeSinceStartup;
		AssetDatabase.ImportAsset(assetPath);
		// dt = ;
		//Debug.LogError("Reimport took " + (Time.realtimeSinceStartup - dt)*1000.0f + " ms.");
		
		hasChanges = false;
		EditorUtility.ClearProgressBar();
		
		// Reload prog
		_initializeProgram();
	}
	
	void _revertProgramChanges() {
		if (prog != null) {
			KrablMesh.Processor[] procs = prog.processors;
			for (int i = 0; i < procs.Length; ++i) {
				DestroyImmediate(procs[i]);
			}
			DestroyImmediate(prog);
			prog = null;
		}
		// Get a new clone of the original prog
		_initializeProgram();
		hasChanges = false;
	}
	
	override public bool HasPreviewGUI() {
		return true;
	}
	
	override public void OnPreviewSettings() {
		if (_assureBuiltInEditorCreated()) {
			builtInEditor.OnPreviewSettings();
		}
		
		mesh = (UnityEngine.Mesh)target;
		if (mesh == null) return;
		
		string[] names = {"Solid", /*"Wire",*/ "UV", "UV2"};
		int len = 1; // 2
		if (mesh.uv.Length > 0) {
			len++;
			if (mesh.uv2.Length > 0) len++;
		}
		
		GUIStyle style = KrablMesh.UnityEditorUtils.PreviewToolbarButtonStyle();
		GUILayout.Button("", style, new GUILayoutOption[]{GUILayout.Width(1)}); // Fake to get a vertical line that is otherwise swallowed
		previewMode = GUILayout.SelectionGrid(previewMode, names, len, style, GUILayout.ExpandWidth(false));
	}
	
	override public void OnPreviewGUI (Rect r, GUIStyle background) {
		OnInteractivePreviewGUI(r, background);
	}
	
	override public void OnInteractivePreviewGUI (Rect r, GUIStyle background) {
		mesh = (UnityEngine.Mesh)target;		
		if (mesh == null) return;
		
		if (previewMode == 0) {
			if (_assureBuiltInEditorCreated()) {
				builtInEditor.OnInteractivePreviewGUI(r, background);
			}
			//_drawWireMesh(mesh, r);
		} else {						
			if (previewMode == 1) _drawUV(mesh, mesh.uv, r);
			else if (previewMode == 2) _drawUV(mesh, mesh.uv2, r);
			//else if (previewMode == 1) _drawWireMesh(mesh, r);
		}
	}
	
	override public GUIContent GetPreviewTitle() {
		return new GUIContent("Mesh Preview");
	}
	
	override public string GetInfoString() {
		if (_assureBuiltInEditorCreated()) {
			return builtInEditor.GetInfoString();
		}
		return "";		
	}
	
	override public Texture2D RenderStaticPreview (string assetPath, Object[] subAssets, int width, int height) {		
		_assureBuiltInEditorCreated();
		return builtInEditor.RenderStaticPreview(assetPath, subAssets, width, height);
	}
	
	void _drawUV(UnityEngine.Mesh mesh, Vector2[] uv, Rect r) {
		if (uv == null || uv.Length == 0) return;
		
		// For some reason if the material is not recreated on every draw, I am getting Leaks :(
		// This is obviously not very good for performance.
		Material mat = new Material("Shader \"UV Lines\" {" +
	    		"Properties { _Color (\"Main Color\", Color) = (1, 1, 1, 1) }" +
	      		"SubShader { Pass {" +
	        	"   BindChannels { Bind \"Color\",color }" +
	        	"   Blend SrcAlpha OneMinusSrcAlpha" +
	        	"   ZWrite Off Cull Off Fog { Mode Off }" +
	        	"} } }");
		mat.hideFlags = HideFlags.HideAndDontSave;			
		
		// Figure out min/max uv coordinates if they are larger than 0,0|1,1
		Vector2 minUV = new Vector2(0, 0);
		Vector2 maxUV = new Vector2(1, 1);
		for (int i = 0; i < uv.Length; ++i) {
			if 		(uv[i].x < minUV.x) minUV.x = uv[i].x;
			else if (uv[i].x > maxUV.x) maxUV.x = uv[i].x;
			if 		(uv[i].y < minUV.y) minUV.y = uv[i].y;
			else if (uv[i].y > maxUV.y) maxUV.y = uv[i].y;
		}
		
		GL.PushMatrix();
        mat.SetPass(0);
		GL.LoadPixelMatrix();
		const float kInfoBorder = 36.0f;
		float scale = Mathf.Min(r.width, r.height - kInfoBorder);
		GL.MultMatrix(Matrix4x4.TRS(new Vector3(0.5f*r.width, r.y + 0.5f*(r.height - kInfoBorder), 0.0f), Quaternion.identity, new Vector3(0.5f*scale, 0.5f*scale, 0.5f*scale)));
	
		GL.Begin(GL.LINES);
        GL.Color(Color.white);

		Vector2 scle = new Vector2(1.0f/(maxUV.x - minUV.x), 1.0f/(maxUV.y - minUV.y));

		float w = 2.0f*scle.x;
		float h = 2.0f*scle.y;
		float x = -1.0f - minUV.x*w;
		float y = -1.0f - minUV.y*w;
		
		int a, b, c, d;
		
		for (int m = 0; m < mesh.subMeshCount; ++m) {
			int[] indices = mesh.GetIndices(m);
			switch (mesh.GetTopology(m)) {
			case UnityEngine.MeshTopology.Triangles:			
				for (int i = 0; i < indices.Length;) {
					a = indices[i++];
					b = indices[i++];
					c = indices[i++];
					
					GL.Vertex3(x + uv[a].x*w, y + uv[a].y*h, 0);
					GL.Vertex3(x + uv[b].x*w, y + uv[b].y*h, 0);
					GL.Vertex3(x + uv[b].x*w, y + uv[b].y*h, 0);
					GL.Vertex3(x + uv[c].x*w, y + uv[c].y*h, 0);
					GL.Vertex3(x + uv[c].x*w, y + uv[c].y*h, 0);
					GL.Vertex3(x + uv[a].x*w, y + uv[a].y*h, 0);
				}
				break;
			case UnityEngine.MeshTopology.Quads:
				for (int i = 0; i < indices.Length;) {
					a = indices[i++];
					b = indices[i++];
					c = indices[i++];
					d = indices[i++];
					
					GL.Vertex3(x + uv[a].x*w, y + uv[a].y*h, 0);
					GL.Vertex3(x + uv[b].x*w, y + uv[b].y*h, 0);
					GL.Vertex3(x + uv[b].x*w, y + uv[b].y*h, 0);
					GL.Vertex3(x + uv[c].x*w, y + uv[c].y*h, 0);
					GL.Vertex3(x + uv[c].x*w, y + uv[c].y*h, 0);
					GL.Vertex3(x + uv[d].x*w, y + uv[d].y*h, 0);
					GL.Vertex3(x + uv[d].x*w, y + uv[d].y*h, 0);
					GL.Vertex3(x + uv[a].x*w, y + uv[a].y*h, 0);
				}
				break;
			}
		}
		GL.End();
		GL.PopMatrix();
		
		// Destroy material and shader now to avoid leaks.
		DestroyImmediate(mat.shader);
		DestroyImmediate(mat);
	}
	
#if false
	// (incomplete) Code to draw the mesh by ourselves.
	
	void _drawWireMesh(UnityEngine.Mesh mesh, Rect r) {
	    if (Event.current.type == EventType.MouseDrag) {
        	previewRotation.y += 0.5f*Event.current.delta.x;
        	previewRotation.x -= 0.5f*Event.current.delta.y;
			Repaint();
        }	
		
		/*Material mat2 = new Material(
			"Shader \"Simple\" {" +
        		"SubShader { Pass {" +
				"	Color (1, 0, 0, 1)" +
				//"	Material { Diffuse (1,1,1,1) Ambient(1,1,1,1) }" +
        		//"   Blend SrcAlpha OneMinusSrcAlpha" +
        		//"   ZWrite Off Cull Off Fog { Mode Off }" +
				//"	Lighting On" +
        		"} } }"+
		"");*/
		GL.PushMatrix();
		
		bool oldWireframe = GL.wireframe;
		
		GL.wireframe = true;
		//GL.LoadIdentity();
		Rect r2 = new Rect(0, 0, r.width, r.height);
		GL.SetRevertBackfacing(false);
		GL.Viewport(r2);
		GL.LoadProjectionMatrix(Matrix4x4.Perspective(60, r2.width/r2.height, 0.3f, 1000.0f));
   		//GL.MultMatrix(Matrix4x4.Scale(new Vector3(1500.0f, 1500.0f, 100.0f)));
		
		GL.modelview = Matrix4x4.TRS(new Vector3(0, 0, -2), Quaternion.Euler(previewRotation), new Vector3(-1,1,1));
		
		mat.SetPass(0);
		Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
		
		//GL.MultMatrix(Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(previewRotation), new Vector3(1,1,1)));

		/*GL.Begin(GL.LINES);
        GL.Color(Color.red);

		int a,b,c,d;
		Vector3[] vert = mesh.vertices;
		
		for (int m = 0; m < mesh.subMeshCount; ++m) {
			int[] indices = mesh.GetIndices(m);
			switch (mesh.GetTopology(m)) {
			case UnityEngine.MeshTopology.Triangles:			
				for (int i = 0; i < indices.Length;) {
					a = indices[i++];
					b = indices[i++];
					c = indices[i++];
					
					GL.Vertex(vert[a]);
					GL.Vertex(vert[b]);
					GL.Vertex(vert[b]);
					GL.Vertex(vert[c]);
					GL.Vertex(vert[c]);
					GL.Vertex(vert[a]);
				}
				break;
			case UnityEngine.MeshTopology.Quads:
				for (int i = 0; i < indices.Length;) {
					a = indices[i++];
					b = indices[i++];
					c = indices[i++];
					d = indices[i++];
					
					GL.Vertex(vert[a]);
					GL.Vertex(vert[b]);
					GL.Vertex(vert[b]);
					GL.Vertex(vert[c]);
					GL.Vertex(vert[c]);
					GL.Vertex(vert[d]);
					GL.Vertex(vert[d]);
					GL.Vertex(vert[a]);
				}
				break;
			}
		}
		GL.End();*/
		
		
		GL.Viewport(new Rect(0, 0, Screen.width, Screen.height));
		GL.wireframe = oldWireframe;
		GL.PopMatrix();
	}
#endif	
}
