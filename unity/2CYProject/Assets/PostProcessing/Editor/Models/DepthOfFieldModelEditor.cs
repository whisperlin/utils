using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using Settings = DepthOfFieldModel.Settings;

    [PostProcessingModelEditor(typeof(DepthOfFieldModel))]
    public class DepthOfFieldModelEditor : PostProcessingModelEditor
    {
        SerializedProperty m_FocusDistance;
        SerializedProperty m_Aperture;
        SerializedProperty m_FocalLength;
 
 

        public override void OnEnable()
        {
            m_FocusDistance = FindSetting((Settings x) => x.focusDistance);
            m_Aperture = FindSetting((Settings x) => x.aperture);
            m_FocalLength = FindSetting((Settings x) => x.focalLength);
 
 
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_FocusDistance);
            EditorGUILayout.PropertyField(m_Aperture, EditorGUIHelper.GetContent("Aperture (f-stop)"));

            

        }
    }
}
