
using UnityEngine;
using UnityEditor;

namespace Kino
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Bloom))]
    public class BloomEditor : Editor
    {
        BloomGraphDrawer _graph;

        SerializedProperty _threshold;
        SerializedProperty _softKnee;
        SerializedProperty _radius;
        SerializedProperty _intensity;
        SerializedProperty _highQuality;
        SerializedProperty _antiFlicker;

        static GUIContent _textThreshold = new GUIContent("Threshold (gamma)");

        void OnEnable()
        {
            _graph = new BloomGraphDrawer();
            _threshold = serializedObject.FindProperty("_threshold");
            _softKnee = serializedObject.FindProperty("_softKnee");
            _radius = serializedObject.FindProperty("_radius");
            _intensity = serializedObject.FindProperty("_intensity");
            _highQuality = serializedObject.FindProperty("_highQuality");
            _antiFlicker = serializedObject.FindProperty("_antiFlicker");
        }

		public string[] options = new string[] {"关闭", "颜色拾取", "模糊结果"};
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

			if (!serializedObject.isEditingMultipleObjects) {
				EditorGUILayout.Space ();
				Bloom b = (Bloom)target;
				_graph.Prepare (b);
				_graph.DrawGraph ();
				EditorGUILayout.Space ();

				base.OnInspectorGUI ();
		 
				int d = EditorGUILayout.Popup ("开发模式：",b.debugMode, options);
				if (d != b.debugMode) {
					
					b.debugMode = d;
					EditorUtility.SetDirty( target );
				}
			} 
			else 
			{
				base.OnInspectorGUI ();
			}

           


            serializedObject.ApplyModifiedProperties();
        }
    }
}
