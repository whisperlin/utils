using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/**
 
 编辑器使用

  
 */

public class EditorAuidoPlayer :   AudioInterface
{
    public void PlaySound(string name, string path, Vector3 pos, object userData)
    {
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        GameObject g = new GameObject();
   
    }

     
}
