using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KMSimplifyProcessor))]
public class KMSimplifyProcessorGUI : KrablMesh.ProcessorEditor {
	SerializedProperty targetTriangleCountProp;
	SerializedProperty ttcOverridePlatformProp;
	SerializedProperty ttcOverrideTargetTriangleCountProp;
	//	SerializedProperty maximumErrorProp;
	
	SerializedProperty allowVertexRepositionProp;
	SerializedProperty preventNonManifoldEdgesProp;
	
	SerializedProperty bordersProp;
	SerializedProperty creasesProp;
	SerializedProperty uvSeamsProp;
	SerializedProperty uv2SeamsProp;
	SerializedProperty materialSeamsProp;
	
	//	SerializedProperty maxEdgesPerVertexProp;
	//	SerializedProperty minTriangleShapeProp;
	
	SerializedProperty boneWeightProtectionProp;
	SerializedProperty vertexColorProtectionProp;
	
	SerializedProperty advancedSettingsVisibleProp;
	
	void OnEnable() {
		if (target == null || serializedObject == null) return;
		
		allowVertexRepositionProp = serializedObject.FindProperty("allowVertexReposition");
		preventNonManifoldEdgesProp = serializedObject.FindProperty("preventNonManifoldEdges");
		
		targetTriangleCountProp = serializedObject.FindProperty("targetTriangleCount");
		ttcOverridePlatformProp = serializedObject.FindProperty("ttcOverridePlatform");
		ttcOverrideTargetTriangleCountProp = serializedObject.FindProperty("ttcOverrideTargetTriangleCount");
		
		//maximumErrorProp = serializedObject.FindProperty("maximumError");
		
		bordersProp = serializedObject.FindProperty("borders");
		creasesProp = serializedObject.FindProperty("creases");
		uvSeamsProp = serializedObject.FindProperty("uvSeams");
		uv2SeamsProp = serializedObject.FindProperty("uv2Seams");
		materialSeamsProp = serializedObject.FindProperty("materialSeams");
		
		//		minTriangleShapeProp = serializedObject.FindProperty("minTriangleShape");
		
		boneWeightProtectionProp = serializedObject.FindProperty("boneWeightProtection");
		vertexColorProtectionProp = serializedObject.FindProperty("vertexColorProtection");
		
		advancedSettingsVisibleProp = serializedObject.FindProperty("advancedSettingsVisible");
	}
	
	void _constraintSlider(SerializedProperty prop, GUIContent label) {
		float val = prop.floatValue;
		if (val > 1.0f) {
			val = (Mathf.Log10(val)/1.5f) + 1.0f;
			val = Mathf.Clamp(val, 1.0f, 3.0f);
		}
		
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.MinWidth(110.0f), GUILayout.MaxWidth(140.0f)});
		float newval = GUILayout.HorizontalSlider(val, 0.0f, 3.0f, new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.MinWidth(80.0f)});
		if (newval != val) GUI.FocusControl(""); // disable text field...
		//newval *= newval;
		if (newval <= 1.0f) {
			prop.floatValue = newval;
		} else if (newval < 3.0f) {
			prop.floatValue = Mathf.Pow(10.0f, (newval - 1.0f)*1.5f);
		} else {
			prop.floatValue = 10000.0f;
		}
		prop.floatValue = EditorGUILayout.FloatField(prop.floatValue, GUILayout.Width(50.0f));
		
		GUILayout.EndHorizontal();
	}
	
	override public void OnInspectorGUI() {		
		if (target == null || serializedObject == null) return;
		
		serializedObject.Update();
		
		EditorGUILayout.PropertyField(allowVertexRepositionProp, new GUIContent(
			"Allow Vertex Repositioning",
			"Calculating new vertex positions leads to a more accurate mesh shape " +
			"but can have a negative impact on mesh attributes (UVs, skin, colors, ...)." +
			"Disabling Repositioning will speed up calculations."));
		
		EditorGUILayout.PropertyField(preventNonManifoldEdgesProp, new GUIContent(
			"Protect Details",
			"Enable to prevent small independent mesh features to be totally removed.\n" +
			"Protecting details is not recommended for low target polygon counts."));
		
		advancedSettingsVisibleProp.boolValue = EditorGUILayout.Foldout(advancedSettingsVisibleProp.boolValue, new GUIContent("Simplification Weights (Advanced)", 
		                                                                                                                      "Extend the weights section by clicking on the small triangle to the left." +
		                                                                                                                      "This is a group of parameters that affect how the mesh is simplified. " +
		                                                                                                                      "They determine which edges get collapsed first and allow to preserve special features of a mesh."));
		if (advancedSettingsVisibleProp.boolValue) {
			GUIStyle insetStyle = new GUIStyle();
			insetStyle.margin = new RectOffset(10, 0, 0, 0);
			insetStyle.contentOffset = new Vector2(10.0f, 0.0f);
			EditorGUILayout.BeginVertical(insetStyle);
			_constraintSlider(bordersProp, new GUIContent("Mesh Borders", "The amount of protection for edges at mesh borders (edges only connected to one face)"));
			_constraintSlider(creasesProp, new GUIContent("Creases", "The Amount of protection for edges that have been marked as creases.\n"+
			                                              "At the beginning of processing all edges with normal breaks are automatically marked as creases. Different edges can be marked as creases with the 'Detect Creases' processor."));
			_constraintSlider(uvSeamsProp, new GUIContent("UV Seams", "The amount of protection for edges that are part of two UV islands."));
			_constraintSlider(uv2SeamsProp, new GUIContent("UV2 Seams", "The amount of protection for edges that are part of two UV2 islands."));
			_constraintSlider(materialSeamsProp, new GUIContent("Material Seams", "The amount of protection for edges connecting faces with different materials."));
			
			EditorGUILayout.Separator();
			_constraintSlider(boneWeightProtectionProp, new GUIContent("Bone Weights", "The amount of protection for edges which have different bone weights at their two vertices.\n" + "" +
			                                                           "This will prevent flexible areas of skinned meshes from being simplified."));
			_constraintSlider(vertexColorProtectionProp, new GUIContent("Vertex Colors", "The amount of protection for edges which have different vertex colors at their two vertices.\n" + "" +
			                                                            "This will prevent areas with changing vertex colors from being simplified."));
			
			EditorGUILayout.EndVertical();
		}
		
		KrablMesh.UnityEditorUtils.OnPropGUIDelegate onPropGUIDel = delegate(SerializedProperty prop) {
			EditorGUILayout.PropertyField(prop, new GUIContent("Target Triangle Count", "The number of triangles the result mesh should have\n" +
			                                                   "It's possible simplification will end up with up to three faces less because single faces can not be independently removed"));
			if (prop.intValue < 0) prop.intValue = 0;
		};
		
		if (usedByMeshImporter) {
			KrablMesh.UnityEditorUtils.GUILayoutPlatformDependantProperty(
				targetTriangleCountProp,
				ttcOverrideTargetTriangleCountProp,
				ttcOverridePlatformProp,
				onPropGUIDel
				);
		} else {
			onPropGUIDel(targetTriangleCountProp);
		}
		
		//	EditorGUILayout.PropertyField(maximumErrorProp, new GUIContent("Maximum Error", ""));
		//	maximumErrorProp.floatValue = Mathf.Clamp(maximumErrorProp.floatValue, 0.0f, 1000.0f);
		
		base.modifiedDuringLastUpdate = serializedObject.ApplyModifiedProperties();
	}
	
	override public string ProcessorToolTip() {
		return "The Simplify Processor reduces the number of triangles in a mesh by collapsing edges. It always triangulates a mesh.";
	}
	
}
