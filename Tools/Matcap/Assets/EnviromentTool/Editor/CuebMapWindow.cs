using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CuebMapWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    // Add menu named "My Window" to the Window menu
    [MenuItem("TA/环境球/CubeMap 转 捕捉Panoramic")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CuebMapWindow window = (CuebMapWindow)EditorWindow.GetWindow(typeof(CuebMapWindow));
        window.Show();
    }

    Cubemap cubemap;
    void OnGUI()
    {

        cubemap = (Cubemap)EditorGUILayout.ObjectField("天空球", cubemap, typeof(Cubemap), true);
        if (GUILayout.Button("转换"))
        {
            string path = EditorUtility.SaveFilePanelInProject("提示", "Panoramic", "png",
                     "请输入保存文件名");
            if (path.Length != 0)
            {
                Material conversionMaterial = new Material(Shader.Find("Hidden/CubemapToEquirectangular"));
            RenderTexture renderTexture = RenderTexture.GetTemporary(4096, 2048, 24 );
            Graphics.Blit(cubemap, renderTexture, conversionMaterial);


            var old = RenderTexture.active;
            RenderTexture.active = renderTexture;
            Texture2D equirectangularTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            equirectangularTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
            equirectangularTexture.Apply();
            RenderTexture.active = old;
            RenderTexture.ReleaseTemporary(renderTexture);
            System.IO.File.WriteAllBytes(path, equirectangularTexture.EncodeToPNG());
            AssetDatabase.ImportAsset(path);
            
            GameObject.DestroyImmediate(equirectangularTexture);
            }

        }
 
    }
}