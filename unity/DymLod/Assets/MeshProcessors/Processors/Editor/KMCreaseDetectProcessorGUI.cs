using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KMCreaseDetectProcessor))]
public class KMCreaseDetectProcessorGUI : KrablMesh.ProcessorEditor {
	SerializedProperty creasesFromNormalsProp;
	SerializedProperty creasesFromEdgeAnglesProp;
	SerializedProperty minEdgeAngleProp;
	SerializedProperty creasesFromMaterialSeamsProp;
//	SerializedProperty creaseStrengthProp;
	
	void OnEnable() {
		if (target == null || serializedObject == null) return; // WHY DOES THIS HAPPEN?
		
		creasesFromNormalsProp = serializedObject.FindProperty("creasesFromNormals");
		creasesFromEdgeAnglesProp = serializedObject.FindProperty("creasesFromEdgeAngles");
		minEdgeAngleProp = serializedObject.FindProperty("minEdgeAngle");
		creasesFromMaterialSeamsProp = serializedObject.FindProperty("creasesFromMaterialSeams");
	//	creaseStrengthProp = serializedObject.FindProperty("creaseStrength");
	}

	override public void OnInspectorGUI() {
		if (target == null || serializedObject == null) return;
		serializedObject.Update();

		EditorGUILayout.PropertyField(creasesFromNormalsProp, new GUIContent("Mesh Normals", "If the normals on all sides of an edge do not match on the input mesh, mark the edge as a crease."));
		EditorGUILayout.PropertyField(creasesFromEdgeAnglesProp, new GUIContent("Edge Angles", "If the faces connected to an edge form a large angle, mark it as a crease."));
		if (creasesFromEdgeAnglesProp.boolValue == true) {
			minEdgeAngleProp.floatValue = EditorGUILayout.Slider(new GUIContent("   Min Edge Angle", "Edges with an angle larger than this are marked as creases."), minEdgeAngleProp.floatValue, 0.0f, 180.0f);
		}
		EditorGUILayout.PropertyField(creasesFromMaterialSeamsProp, new GUIContent("Material Seams", "Mark all edges at the border of multiple materials (submeshes) as creases."));
//		EditorGUILayout.Slider(creaseStrengthProp, 0.0f, 1.0f);
		base.modifiedDuringLastUpdate = serializedObject.ApplyModifiedProperties();
	}
	
	override public string ProcessorToolTip() {
		return "The Crease Detect processor marks edges as creases based on different criteria. " +
			"It does not change mesh geometry. " +
			"If necessary, the mesh normals are updated. " ;
	}

}
