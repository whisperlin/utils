using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KMSubdivideQProcessor))]
public class KMSubdivideQProcessorGUI : KrablMesh.ProcessorEditor {
	SerializedProperty trisToQuadsProp;
	SerializedProperty trisToQuadsMaxEdgeAngleProp;
	SerializedProperty iterationsProp;
	SerializedProperty iterationsOverridePlatformProp;
	SerializedProperty iterationsOverrideProp;
	SerializedProperty normalModeProp;
	
	void OnEnable() {
		if (target == null || serializedObject == null) return;
		
		trisToQuadsProp = serializedObject.FindProperty("trisToQuads");
		trisToQuadsMaxEdgeAngleProp = serializedObject.FindProperty("trisToQuadsMaxEdgeAngle");
		iterationsProp = serializedObject.FindProperty("iterations");
		iterationsOverridePlatformProp = serializedObject.FindProperty("iterationsOverridePlatform");
		iterationsOverrideProp = serializedObject.FindProperty("iterationsOverride");
		normalModeProp = serializedObject.FindProperty("normalMode");
	}

	override public void OnInspectorGUI() {
		if (target == null || serializedObject == null) return;
		serializedObject.Update();
		
		EditorGUILayout.PropertyField(trisToQuadsProp, 
			new GUIContent("Tris to Quads", "This will merge adjacent triangles to quads. It starts with the lowest edge angle and will not dissolve special edges (uv seams etc.)"));
		if (trisToQuadsProp.boolValue) {
			trisToQuadsMaxEdgeAngleProp.floatValue = EditorGUILayout.Slider(new GUIContent("   Max Edge Angle", "Edges with a face angle below this value can be dissolved, creating a quad from two triangles."),
				trisToQuadsMaxEdgeAngleProp.floatValue, 0.0f, 180.0f);
		}		
		
		/*EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Normal Mode");
		bool val = false;
		GUILayout.Toggle(val, "Interpolate", EditorStyles.radioButton, GUILayout.ExpandWidth(false));
		GUILayout.Toggle(val, "Recalculate", EditorStyles.radioButton, GUILayout.ExpandWidth(false));
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();*/
		
		EditorGUILayout.PropertyField(normalModeProp, new GUIContent("Normal Mode", 
			"Interpolate - linearly interpolate normals of the input mesh during the subdivision.\n" +
			"Recalculate - calculate new normals based on the mesh shape once the subdivision is complete."));	
				
		KrablMesh.UnityEditorUtils.OnPropGUIDelegate onPropGUIDel = delegate(SerializedProperty prop) {
			KrablMesh.UnityEditorUtils.GUILayoutPlusMinusIntProperty(prop, new GUIContent("Iterations", "For every iteration every face is split into four quads. High iteration counts can lead to lots of faces and slow processing."));
			prop.intValue = Mathf.Clamp(prop.intValue, 0, 9);
			
			if (prop.intValue > 3) {
				EditorGUILayout.HelpBox("High iteration counts quickly lead to meshes with lots of vertices and faces.\n" + "" +
					"Every iterations multiplies the face count by ~4 and the vertex count by ~3.\n" +
					"Later processors can take a long time to process large meshes.\n" +
					"Unity can only handle (result) meshes with less than 65536 vertices.", MessageType.Warning);
			}
		};
		
		if (usedByMeshImporter) {
			KrablMesh.UnityEditorUtils.GUILayoutPlatformDependantProperty(
				iterationsProp,
				iterationsOverrideProp,
				iterationsOverridePlatformProp,
				onPropGUIDel
			);
		} else {
			onPropGUIDel(iterationsProp);
		}

		base.modifiedDuringLastUpdate = serializedObject.ApplyModifiedProperties();
	}
	
	override public string ProcessorToolTip() {
		return "The Subdivide Quads processor splits every face into four quads and applies a smoothing algorithm to produce a smoother surface." +
			"Edges marked as creases are preserved as creases. The subdivision algorithm works best for quads meshes." +
			"Therefore there is a first stage that tries to merge triangles to quads before the subdivision iterations begin.";
	}

}
