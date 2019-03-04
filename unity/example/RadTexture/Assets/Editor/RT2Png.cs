using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RT2Png {



    static Texture2D SaveRT(RenderTexture rt, string filename)
    {

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rt.width, (int)rt.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();


        //ps: camera2.targetTexture = null;  
        RenderTexture.active = null; // JC: added to avoid errors  
     

        byte[] bytes = screenShot.EncodeToPNG();

        System.IO.File.WriteAllBytes(filename, bytes);
        AssetDatabase.ImportAsset(filename);

        return screenShot;
    }
    [MenuItem("TA/RenderTexture 2 png")]
    static void SaveRT2Png()
    {
        RenderTexture rt = Selection.activeObject as RenderTexture;
        if (rt == null)
        {
            EditorUtility.DisplayDialog("", "请选择一张RenderTexture", "ok"); 
        }
        else
        {
            string path = AssetDatabase.GetAssetPath(rt);
            int index = path.LastIndexOf(".");
            string pngPath = path.Substring(0, index)+".png";
            SaveRT(rt,pngPath);
            //EditorUtility.DisplayDialog("", AssetDatabase.GetAssetPath(rt), "ok");
        }
    }
}
