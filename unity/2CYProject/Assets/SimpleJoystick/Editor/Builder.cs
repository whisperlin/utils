using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Builder : Editor
{
    [MenuItem("TA/当前场景安卓打包")]
    static public void Build()
    {
        
        string outputPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + "Out";
        Debug.Log(outputPath);
        if (false == System.IO.Directory.Exists(outputPath))
        {
            //创建pic文件夹
            System.IO.Directory.CreateDirectory(outputPath);
        }
 
        PlayerSettings.bundleIdentifier = "com.bengnana.artdemo";
        outputPath = outputPath.Replace('\\', '/');
        BuildPipeline.BuildPlayer(GetCureScenes(), outputPath+"/demo.apk", BuildTarget.Android, BuildOptions.None);
    }

    static string[] GetCureScenes()
    {
 
        
        return new string[] { SceneManager.GetActiveScene().path };
    }
    static string[] GetBuildScenes()
    {
        List<string> pathList = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                pathList.Add(scene.path);
            }
        }
        return pathList.ToArray();
    }

}