using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KMFilterAttributesProcessor))]
public class KMFilterAttributesProcessorGUI : KrablMesh.ProcessorEditor {
	SerializedProperty materialsProp;
	SerializedProperty vertexColorsProp;
	SerializedProperty uv1OptionProp;
	SerializedProperty uv2OptionProp;
	
	void OnEnable() {
		if (target == null || serializedObject == null) return;
		
		materialsProp = serializedObject.FindProperty("materials");
		vertexColorsProp = serializedObject.FindProperty("vertexColors");
		uv1OptionProp = serializedObject.FindProperty("uv1Option");
		uv2OptionProp = serializedObject.FindProperty("uv2Option");
	}

	override public void OnInspectorGUI() {
		if (target == null || serializedObject == null) return;
		serializedObject.Update();

		EditorGUILayout.PropertyField(materialsProp, new GUIContent("Materials/Submeshes"));
		EditorGUILayout.PropertyField(vertexColorsProp, new GUIContent("Vertex Colors"));
		EditorGUILayout.PropertyField(uv1OptionProp, new GUIContent("UV"));
		EditorGUILayout.PropertyField(uv2OptionProp, new GUIContent("UV2"));
		// removing SKIN is dangerous, so it's omitted here
		// crashes happen if the skin of a mesh disappears that is visible in a skinnedmeshrenderer
		base.modifiedDuringLastUpdate = serializedObject.ApplyModifiedProperties();
	}
	
	override public string ProcessorToolTip() {
		return "The Filter Attributes processor allows to merge the materials/submeshes of a mesh and can remove mesh attributes such as UV coordinates or Vertex Colors. ";
	}

}
