using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(KMProcessorProgram))]
public class KMProcessorProgramGUI : Editor {
	public bool modifiedDuringLastUpdate = false;
	
	public bool usedByMeshImporter = false;
	KMProcessorProgram prog = null;
	SerializedProperty processorsProp = null;
	SerializedProperty unityOptimizeMeshProp = null;
	SerializedProperty inputToleranceProp = null;
	SerializedProperty bypassProp = null;
//	SerializedProperty targetPathProp = null;
	
	Dictionary<KrablMesh.Processor, KrablMesh.ProcessorEditor> processorEditors = new Dictionary<KrablMesh.Processor, KrablMesh.ProcessorEditor>();		
	
	GUIStyle largeButtonStyle = null;
	GUIStyle titleStyle = null;
	GUIStyle titleBoxStyle = null;
	GUIStyle processorNameStyle = null;
	string processorToAddID = null;

	public void OnEnable()
	{
		if (target != null) {
			prog = (KMProcessorProgram)target;
			processorsProp = serializedObject.FindProperty("processors");
			unityOptimizeMeshProp = serializedObject.FindProperty("unityOptimizeMesh");
			inputToleranceProp = serializedObject.FindProperty("inputTolerance");
			bypassProp = serializedObject.FindProperty("bypass");
	//		targetPathProp = serializedObject.FindProperty("targetPath");
		}
		processorEditors.Clear();
		largeButtonStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("LargeButton");
	}
	
	public void OnDisable()
	{
		processorEditors.Clear();
		prog = null;
	}
		
	public void DrawImporterGUI(KMMeshInspector inspector) {
		serializedObject.Update();
		
		modifiedDuringLastUpdate = false;
		if (prog == null) return;

		// Adding processors at the end. Triggered from the menu callback
		// During the callback this does no longer work in Unity 4.3
		if (processorToAddID != null) {
			AddProcessor(processorToAddID);
			processorToAddID = null;
		}
		//KrablMesh.Processor[] processors = prog.processors;
		
		if (titleBoxStyle == null) {
			titleBoxStyle = new GUIStyle("Box");
			titleBoxStyle.margin = new RectOffset(0, 0, 0, 0);
			titleBoxStyle.padding = new RectOffset(0, 0, 4, 4);

		}
		GUILayout.BeginVertical(titleBoxStyle);
		
		GUILayout.BeginHorizontal();
		if (titleStyle == null) {
			titleStyle = new GUIStyle(EditorStyles.largeLabel);
			titleStyle.fontStyle = FontStyle.Bold;
		}
		string titleString = "Mesh Import Program";
		if (KMImportSettings.DoDebug()) titleString += " (r" + prog.importRevision + ")";
		GUILayout.Label(titleString, titleStyle);
				
		GUILayout.FlexibleSpace();
		
		if (GUILayout.Button(new GUIContent("Delete", "Delete the mesh import program for this mesh"))) {
			// Call the inspector as only it can delete the program (and this editor).
			inspector.DeleteProgram();
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.Separator();
		
		EditorGUILayout.PropertyField(inputToleranceProp, new GUIContent("Input Tolerance", "The maximum difference considered equal for vertices and normals when they are fed into the processor chain. " +
			"Usually 0 should work fine, but apparently some modelling software produces only nearly equal vertices/normals. " + "" +
			"If weird topology is produced, try setting the value to something like 1e-6"));
		
		
	/*	EditorGUILayout.LabelField("Output Mesh Path");
		EditorGUILayout.BeginHorizontal();
		if (!prog.targetPath.Equals("replace")) {
			EditorGUILayout.PropertyField(targetPathProp, GUIContent.none);
			if (GUILayout.Button("Replace", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
				targetPathProp.stringValue = "replace";
			}
		} else {
			EditorGUILayout.LabelField("Replace imported Mesh");
			if (GUILayout.Button("Override", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
				targetPathProp.stringValue = KrablMeshUnityPreferences.defaultExtraMeshOutputPath + "/" + prog.inContainerName + "/" + prog.inMeshPath;
			}
		}
		EditorGUILayout.EndHorizontal();
*/
		
		EditorGUILayout.PropertyField(unityOptimizeMeshProp, new GUIContent("Optimize Mesh", "Improve the mesh element order after processing to render faster on the GPU"));
		
		EditorGUILayout.PropertyField(bypassProp, new GUIContent("Bypass Program", "Disable all processors and output the originally imported mesh"));

		GUILayout.EndVertical();
		
		if (bypassProp.boolValue == true) {
			GUI.enabled = false;
		}
		// Draw processor GUIs below for importer
		if (processorsProp != null) {
			int toDelete = -1, moveUp = -1, moveDown = -1;
            KrablMesh.ProcessorEditor edit;
			for (int i = 0; i < processorsProp.arraySize; ++i) {
				KrablMesh.Processor p = processorsProp.GetArrayElementAtIndex(i).objectReferenceValue as KrablMesh.Processor;
                edit = null;
				if (p != null && processorEditors.TryGetValue(p, out edit) == false) {
					edit = (p == null) ? null : Editor.CreateEditor(p) as KrablMesh.ProcessorEditor;
					edit.usedByMeshImporter = usedByMeshImporter;
					processorEditors.Add(p, edit);
				}
				//fold = EditorGUILayout.InspectorTitlebar(fold, p);
				
				EditorGUILayout.BeginHorizontal();
				if (p == null) {
					EditorGUILayout.LabelField("-- NULL PROCESSOR --");
				} else {					
					if (processorNameStyle == null) {
						processorNameStyle = new GUIStyle(EditorStyles.toggle);
						processorNameStyle.fontStyle = FontStyle.Bold;
					}
					
					edit.serializedObject.Update();
					SerializedProperty enabledProp = edit.serializedObject.FindProperty("enabled");
					if (enabledProp != null) {
						enabledProp.boolValue = GUILayout.Toggle(enabledProp.boolValue, new GUIContent(p.Name(), edit.ProcessorToolTip()), processorNameStyle, GUILayout.Width(146.0f));
					}
					if (edit.serializedObject.ApplyModifiedProperties()) modifiedDuringLastUpdate = true;
				}
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Up", EditorStyles.miniButtonLeft)) moveUp = i;
				if (GUILayout.Button("Down", EditorStyles.miniButtonRight)) moveDown = i;
				GUILayout.Space(5.0f);
				if (GUILayout.Button("DEL", EditorStyles.miniButton)) toDelete = i;
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Separator();
					
				if (p != null && edit != null) {
					edit.OnInspectorGUI();
					if (edit.modifiedDuringLastUpdate) {
						modifiedDuringLastUpdate = true;
					}
				}
				EditorGUILayout.Separator();
				
				KrablMesh.UnityEditorUtils.GUILayoutAddHoriLine();
			}
		
			if (toDelete >= 0) {
				int last = processorsProp.arraySize - 1;
				processorsProp.MoveArrayElement(toDelete, last);
				processorsProp.DeleteArrayElementAtIndex(last);
				processorsProp.arraySize--;
			} else if (moveUp > 0) {
				processorsProp.MoveArrayElement(moveUp, moveUp - 1); 
			} else if (moveDown >= 0 && moveDown < processorsProp.arraySize - 1) {
				processorsProp.MoveArrayElement(moveDown, moveDown + 1);
			}
		}
		EditorGUILayout.Separator();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		AddProcessorPopupButton("Add Processor here...", largeButtonStyle, _addProcessorCallback);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		EditorGUILayout.Separator();
		
		KrablMesh.UnityEditorUtils.GUILayoutAddHoriLine();
		if (serializedObject.ApplyModifiedProperties()) {
			modifiedDuringLastUpdate = true;
		}

	}

	void AddProcessor(string mid) {
		KrablMesh.Processor p = null;
		if (mid == "simp") {
			p = ScriptableObject.CreateInstance<KMSimplifyProcessor>();
		} else if (mid == "subq") {
			p = ScriptableObject.CreateInstance<KMSubdivideQProcessor>();
		} else if (mid == "crde") {
			p = ScriptableObject.CreateInstance<KMCreaseDetectProcessor>();
		} else if (mid == "fatt") {
			p = ScriptableObject.CreateInstance<KMFilterAttributesProcessor>();
		} 
		if (p != null) {
			p.name = p.Name();
			if (usedByMeshImporter) { // currently always true
				if (prog.descriptiveName() != null) {
					p.name += "[" + prog.descriptiveName() + "]";
				}
				// Append to array property
				int pos = processorsProp.arraySize;
				processorsProp.arraySize++;
				processorsProp.GetArrayElementAtIndex(pos).objectReferenceValue = p;
			}
		} 
	}

	public static void AddProcessorPopupButton(string buttonLabel, GUIStyle buttonStyle, GenericMenu.MenuFunction2 func)
	{
		if (GUILayout.Button(buttonLabel, buttonStyle, GUILayout.Width(200.0f))) {
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Simplify"), false, func, "simp");
			menu.AddItem(new GUIContent("Subdivide (Quads)"), false, func, "subq");
			menu.AddItem(new GUIContent("Detect Creases"), false, func, "crde");
			menu.AddItem(new GUIContent("Filter Attributes"), false, func, "fatt");

			menu.ShowAsContext();			
		} 
	}
	
	override public void OnInspectorGUI() {
		if (target == null || serializedObject == null) return;
		
		if (!KMImportSettings.DoDebug()) {
			EditorGUILayout.HelpBox("Mesh processors programs can only be used in imported programs displayed in the mesh inspector.", MessageType.Info);
		} else {
			DrawDefaultInspector();
		}
		return;
		
#if false 
		serializedObject.Update();
		
		// This will become the realtime section eventually maybe.
		
		modifiedDuringLastUpdate = false;
		//float dummy = 0.0f;
		// Input Section:
	//	GUIStyle headerStyle = new GUIStyle("Box");
	//	headerStyle.margin = new RectOffset(0, 0 ,0, 0);
		//EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);
		//EditorGUILayout.Separator();
			
	//	EditorGUILayout.BeginVertical(headerStyle);
	//	if (GUI.Button(r, GUIContent.none, GUIStyle.none)) {
	//		Debug.Log("BG clicked.");
	//	}
		//EditorGUILayout.BeginHorizontal();
		//EditorGUILayout.PrefixLabel("Original Mesh");
		EditorGUILayout.LabelField("Original Mesh", prog.inMeshDescription);
		//EditorGUILayout.EndHorizontal();
		
	/*	EditorGUILayout.LabelField("Output Mesh Path");
		EditorGUILayout.BeginHorizontal();
		if (!prog.targetPath.Equals("replace")) {
			EditorGUILayout.PropertyField(targetPathProp, GUIContent.none);
			if (GUILayout.Button("Replace", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
				targetPathProp.stringValue = "replace";
			}
		} else {
			EditorGUILayout.LabelField("Replace imported Mesh");
			if (GUILayout.Button("Override", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
				targetPathProp.stringValue = KrablMeshUnityPreferences.defaultExtraMeshOutputPath + "/" + prog.inContainerName + "/" + prog.inMeshPath;
			}
		}
		EditorGUILayout.EndHorizontal();
*/
		// EditorGUILayout.Separator();

	//	EditorGUILayout.EndVertical();
				
		KrablMesh.UnityEditorUtils.GUILayoutAddHoriLine();	
		
		EditorGUILayout.PropertyField(unityOptimizeMeshProp, new GUIContent("Optimize Mesh", "Improve the mesh element order to render faster on the GPU"));
		
		if (GUILayout.Button("Calculate")) {
			// TODO: move this to ProcessorProgramm class
			UnityEngine.Mesh mesh = null;
			MeshFilter mf = prog.GetComponent<MeshFilter>();
			SkinnedMeshRenderer smr = null;
			if (mf) { 
				mesh = mf.sharedMesh;
			} else {
				smr = prog.GetComponent<SkinnedMeshRenderer>();
				if (smr) {
					mesh = smr.sharedMesh;
				}
			}
			if (mesh == null) {
				Debug.LogError("Can't process without Mesh");
			} else {
				mesh = prog.Process(mesh);
				mesh.name = "TEMP";
				if (mf != null) mf.sharedMesh = mesh;
				else if (smr != null) smr.sharedMesh = mesh;
			}
		}
				
		if (serializedObject.ApplyModifiedProperties()) {
			modifiedDuringLastUpdate = true;
		}
#endif
	}
		
	// This is now a minimal menu callback as there have been problem with Unity4.3
	// and doing complicated stuff in the callback.
	void _addProcessorCallback(object choice) {
		processorToAddID = choice as string;
	}
}
