using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EasyExtend.Scene;


//https://docs.unity3d.com/ScriptReference/Editor.html
[CustomEditor(typeof(SceneLightingParam))]
[CanEditMultipleObjects]
public class SceneLightingParamEditor : Editor {
    void OnEnable()
    {
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        foreach (var target in targets)
        {
            (target as SceneLightingParam).SetupParams();
        }
    }
}
