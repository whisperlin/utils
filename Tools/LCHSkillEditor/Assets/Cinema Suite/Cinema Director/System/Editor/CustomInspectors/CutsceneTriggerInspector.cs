using UnityEditor;
using UnityEngine;
using CinemaDirector;

/// <summary>
/// A custom inspector for cutscene triggers.
/// </summary>
[CustomEditor(typeof(CutsceneTrigger), true)]
public class CutsceneTriggerInspector : Editor
{
    private SerializedObject trigger;

    private SerializedProperty startMethod;
    private SerializedProperty cutscene;
    private SerializedProperty triggerObject;
    private SerializedProperty skipButton;
    private SerializedProperty triggerButton;


    #region 
    private const string startMethodContent = "Start Method";
    #endregion

    /// <summary>
    /// On inspector enable, load the serialized properties
    /// </summary>
    private void OnEnable()
    {
        trigger = new SerializedObject(this.target);

        startMethod = trigger.FindProperty("StartMethod");
        cutscene = trigger.FindProperty("Cutscene");
        triggerObject = trigger.FindProperty("TriggerObject");
        skipButton = trigger.FindProperty("SkipButtonName");
        triggerButton = trigger.FindProperty("TriggerButtonName");
    }

    /// <summary>
    /// Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        trigger.Update();

        EditorGUILayout.PropertyField(cutscene);
        StartMethod newStartMethod = (StartMethod) EditorGUILayout.EnumPopup(startMethodContent, (StartMethod)startMethod.enumValueIndex);
        
        if (newStartMethod != (StartMethod)startMethod.enumValueIndex)
        {
            startMethod.enumValueIndex = (int)newStartMethod;

            CutsceneTrigger cutsceneTrigger = (this.target as CutsceneTrigger);
            if (newStartMethod == StartMethod.OnTrigger || newStartMethod == StartMethod.OnTriggerStayAndButtonDown)
            {
                if (cutsceneTrigger != null && cutsceneTrigger.gameObject.GetComponent<BoxCollider>() == null)
                {
                    cutsceneTrigger.gameObject.AddComponent<BoxCollider>();
                    cutsceneTrigger.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                }
                else if (cutsceneTrigger != null && cutsceneTrigger.gameObject.GetComponent<BoxCollider>() != null)
                {
                    cutsceneTrigger.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                }
            }
            else
            {
                // Can't cleanly destroy collider yet.
                //CutsceneTrigger cutsceneTrigger = (this.target as CutsceneTrigger);
                //if (cutsceneTrigger != null && cutsceneTrigger.gameObject.GetComponent<Collider>() != null)
                //{
                //    Collider c = cutsceneTrigger.gameObject.GetComponent<Collider>();
                //    DestroyImmediate(c, true);
                //}
            }
        }

        if (newStartMethod == StartMethod.OnTrigger)
        {
            EditorGUILayout.PropertyField(triggerObject);
        }
        else if (newStartMethod == StartMethod.OnTriggerStayAndButtonDown)
        {
            EditorGUILayout.PropertyField(triggerObject);
            EditorGUILayout.PropertyField(triggerButton);
        }

        EditorGUILayout.PropertyField(skipButton);

        trigger.ApplyModifiedProperties();
    }
}
