using CinemaDirector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SendMessageGameObject))]
class SendMessageGameObjectInspector : Editor
{

    private SerializedObject sendMessageGameObject;
    private SerializedProperty sendMessageMethodName;
    private SerializedProperty sendMessageParameterType;
    private SerializedProperty sendMessageOptions;

    private SerializedProperty intValue;
    //private SerializedProperty longValue;
    private SerializedProperty floatValue;
    private SerializedProperty doubleValue;
    private SerializedProperty boolValue;
    private SerializedProperty stringValue;

    public void OnEnable()
    {
        sendMessageGameObject = new SerializedObject(this.target);
        this.sendMessageMethodName = sendMessageGameObject.FindProperty("MethodName");
        this.sendMessageParameterType = sendMessageGameObject.FindProperty("ParameterType");
        this.sendMessageOptions = sendMessageGameObject.FindProperty("SendMessageOptions");

        this.intValue = sendMessageGameObject.FindProperty("intValue");
        //this.longValue = sendMessageGameObject.FindProperty("longValue");
        this.floatValue = sendMessageGameObject.FindProperty("floatValue");
        this.doubleValue = sendMessageGameObject.FindProperty("doubleValue");
        this.boolValue = sendMessageGameObject.FindProperty("boolValue");
        this.stringValue = sendMessageGameObject.FindProperty("stringValue");
    }

    public override void OnInspectorGUI()
    {
        sendMessageGameObject.Update();

        EditorGUILayout.PropertyField(sendMessageMethodName);
        EditorGUILayout.PropertyField(sendMessageParameterType);
        
        //  Show appropriate input field
        switch ((SendMessageGameObject.SendMessageValueType)sendMessageParameterType.enumValueIndex)
        {
            case SendMessageGameObject.SendMessageValueType.Int:
                EditorGUILayout.PropertyField(intValue);
                break;
            //case SendMessageGameObject.SendMessageValueType.Long:
            //    EditorGUILayout.PropertyField(longValue);
            //    break;
            case SendMessageGameObject.SendMessageValueType.Float:
                EditorGUILayout.PropertyField(floatValue);
                break;
            case SendMessageGameObject.SendMessageValueType.Double:
                EditorGUILayout.PropertyField(doubleValue);
                break;
            case SendMessageGameObject.SendMessageValueType.Bool:
                EditorGUILayout.PropertyField(boolValue);
                break;
            case SendMessageGameObject.SendMessageValueType.String:
                EditorGUILayout.PropertyField(stringValue);
                break;
        }        
            
        EditorGUILayout.PropertyField(sendMessageOptions);

        sendMessageGameObject.ApplyModifiedProperties();
    }
}



