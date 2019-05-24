using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

 

public class TransparentBackgroundScreenshotRecorder  
{

    

    static void WriteScreenImageToTexture(Texture2D tex, RenderTexture rt)
    {
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = old;
    }

    static void RenderCamToTexture(Camera cam, Texture2D tex, RenderTexture rt)
    {
        cam.enabled = true;
        cam.targetTexture = rt;
        cam.Render();
        cam.targetTexture = null;
        WriteScreenImageToTexture(tex,rt);
        cam.enabled = false;
    }
    [MenuItem("TA/小工具/截屏")]
    static void test()
    {

        Camera mainCam = Camera.main;
        GameObject whiteCamGameObject = (GameObject)new GameObject();
        whiteCamGameObject.name = "White Background Camera";
        Camera whiteCam = whiteCamGameObject.AddComponent<Camera>();
        whiteCam.CopyFrom(mainCam);
        whiteCam.backgroundColor = Color.white;
        
        whiteCamGameObject.transform.SetParent(mainCam.transform, true);
        whiteCamGameObject.transform.rotation = mainCam.transform.rotation;

        GameObject blackCamGameObject = (GameObject)new GameObject();
        blackCamGameObject.name = "Black Background Camera";
        Camera blackCam = blackCamGameObject.AddComponent<Camera>();
        blackCam.CopyFrom(mainCam);
        blackCam.backgroundColor = Color.black;
        
        blackCamGameObject.transform.SetParent(mainCam.transform, true);
        blackCamGameObject.transform.rotation = mainCam.transform.rotation;

        

        int screenWidth = Screen.width*2 ;
        int screenHeight =  Screen.height*2 ;
        Texture2D textureBlack = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
        Texture2D textureWhite = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
        Texture2D textureTransparentBackground = new Texture2D(screenWidth, screenHeight, TextureFormat.ARGB32, false);

        RenderTexture rt =  RenderTexture.GetTemporary(screenWidth, screenHeight, 24);
        rt.antiAliasing = 8;
        blackCam.clearFlags = CameraClearFlags.Color;
        whiteCam.clearFlags = CameraClearFlags.Color;

        RenderCamToTexture(blackCam, textureBlack, rt);
        RenderCamToTexture(whiteCam, textureWhite, rt);


        Color color;
        for (int y = 0; y < textureTransparentBackground.height; ++y)
        {
            // each row
            for (int x = 0; x < textureTransparentBackground.width; ++x)
            {
                // each column
                float alpha = textureWhite.GetPixel(x, y).r - textureBlack.GetPixel(x, y).r;
                alpha = 1.0f - alpha;
                if (alpha == 0)
                {
                    color = Color.clear;
                }
                else
                {
                    color = textureBlack.GetPixel(x, y) / alpha;
                }
                color.a = alpha;
                textureTransparentBackground.SetPixel(x, y, color);
            }
        }

        
        //var pngShot = textureTransparentBackground.EncodeToPNG();
 
        string path = EditorUtility.SaveFilePanelInProject("提示", "TextureName", "png",
               "请输入保存文件名");
        if (path.Length != 0)
        {
            byte[] bytes = textureTransparentBackground.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
        }
        GameObject.DestroyImmediate(whiteCamGameObject);
        GameObject.DestroyImmediate(blackCamGameObject);
        GameObject.DestroyImmediate(textureBlack);
        GameObject.DestroyImmediate(textureWhite);
        GameObject.DestroyImmediate(textureTransparentBackground);
        RenderTexture.ReleaseTemporary(rt);
        

    }

     
}