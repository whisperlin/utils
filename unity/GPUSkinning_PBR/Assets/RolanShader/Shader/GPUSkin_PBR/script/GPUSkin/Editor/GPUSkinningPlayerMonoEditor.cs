﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GPUSkinningPlayerMono))]
public class GPUSkinningPlayerMonoEditor : Editor
{
    private float time = 0;

    private string[] clipsName = null;

	
	static GameObject GetPrefabInstanceParent(GameObject go)
	{
		 if(go == null){
			 return null;
		 }
		 PrefabType pType = EditorUtility.GetPrefabType(go);
		 if(pType != PrefabType.PrefabInstance){
			 return null;
		 }
		 if(go.transform.parent == null){
			 return go;
		 }
		 pType = EditorUtility.GetPrefabType(go.transform.parent.gameObject);
		 if(pType != PrefabType.PrefabInstance){
			 return go;
		 }
		 return GetPrefabInstanceParent(go.transform.parent.gameObject);
	}
	
    public override void OnInspectorGUI()
    {
        GPUSkinningPlayerMono player = target as GPUSkinningPlayerMono;
        if (player == null)
        {
            return;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("anim"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            player.DeletePlayer();
            player.Init();
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mesh"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            player.DeletePlayer();
            player.Init();
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mtrl"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            player.DeletePlayer();
            player.Init();
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("textureRawData"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            player.DeletePlayer();
            player.Init();
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rootMotionEnabled"), new GUIContent("Apply Root Motion"));
        if(EditorGUI.EndChangeCheck())
        {
            if(Application.isPlaying)
            {
                player.Player.RootMotionEnabled = serializedObject.FindProperty("rootMotionEnabled").boolValue;
            }
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lodEnabled"), new GUIContent("LOD Enabled"));
        if (EditorGUI.EndChangeCheck())
        {
            if (Application.isPlaying)
            {
                player.Player.LODEnabled = serializedObject.FindProperty("lodEnabled").boolValue;
            }
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cullingMode"), new GUIContent("Culling Mode"));
        if (EditorGUI.EndChangeCheck())
        {
            if (Application.isPlaying)
            {
                player.Player.CullingMode = 
                    serializedObject.FindProperty("cullingMode").enumValueIndex == 0 ? GPUSKinningCullingMode.AlwaysAnimate :
                    serializedObject.FindProperty("cullingMode").enumValueIndex == 1 ? GPUSKinningCullingMode.CullUpdateTransforms : GPUSKinningCullingMode.CullCompletely;
            }
        }

        GPUSkinningAnimation anim = serializedObject.FindProperty("anim").objectReferenceValue as GPUSkinningAnimation;
        SerializedProperty defaultPlayingClipIndex = serializedObject.FindProperty("defaultPlayingClipIndex");
        if (clipsName == null && anim != null)
        {
            List<string> list = new List<string>();
            for(int i = 0; i < anim.clips.Length; ++i)
            {
                list.Add(anim.clips[i].name);
            }
            clipsName = list.ToArray();

            defaultPlayingClipIndex.intValue = Mathf.Clamp(defaultPlayingClipIndex.intValue, 0, anim.clips.Length);
        }
        if (clipsName != null)
        {
            EditorGUI.BeginChangeCheck();
            defaultPlayingClipIndex.intValue = EditorGUILayout.Popup("Default Playing", defaultPlayingClipIndex.intValue, clipsName);
            if (EditorGUI.EndChangeCheck())
            {
                player.Player.Play(clipsName[defaultPlayingClipIndex.intValue]);
            }
        }

        serializedObject.ApplyModifiedProperties();

		if(GUILayout.Button("提交"))
		{
			//if(source == null) return;
			//
			MeshRenderer r = player.GetComponent<MeshRenderer> ();
			if (!player.mat.shader.isSupported) {
				player.mat.shader = r.sharedMaterial.shader;
			}
			player.mat.CopyPropertiesFromMaterial (r.sharedMaterial);
			 
			GameObject prefabGo = GetPrefabInstanceParent(player.gameObject);
			UnityEngine.Object prefabAsset = null;
			if(prefabGo != null){
			 	prefabAsset = PrefabUtility.GetPrefabParent(prefabGo);
			 	if(prefabAsset != null){
			    	PrefabUtility.ReplacePrefab(prefabGo, prefabAsset, ReplacePrefabOptions.ConnectToPrefab);
					string path = AssetDatabase.GetAssetPath (player.mat);
					Debug.Log (path);
					//System.IO.File.Delete (path);
					//AssetDatabase.CreateAsset(player.mat,path);
					EditorUtility.SetDirty(prefabGo); 
					EditorUtility.SetDirty(player.mat); 
					AssetDatabase.SaveAssets();
					EditorUtility.DisplayDialog ("", "替换", "确定");
			 	}
			}
			AssetDatabase.SaveAssets();
		}
    }

    private void Awake()
    {
        time = Time.realtimeSinceStartup;
        EditorApplication.update += UpdateHandler;

        GPUSkinningPlayerMono player = target as GPUSkinningPlayerMono;
        if (player != null)
        {
            player.Init();
        }
    }

    private void OnDestroy()
    {
        EditorApplication.update -= UpdateHandler;
    }

    private void UpdateHandler()
    {
        float deltaTime = Time.realtimeSinceStartup - time;
        time = Time.realtimeSinceStartup;

        GPUSkinningPlayerMono player = target as GPUSkinningPlayerMono;
        if (player != null)
        {
            player.Update_Editor(deltaTime);
        }

        foreach(var sceneView in SceneView.sceneViews)
        {
            if (sceneView is SceneView)
            {
                (sceneView as SceneView).Repaint();
            }
        }
    }

    private void BeginBox()
    {
        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
        EditorGUILayout.Space();
    }

    private void EndBox()
    {
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }
}
