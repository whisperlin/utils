using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TransparentBackgroundScreenVideoRecorder : EditorWindow {

    public  string saveFolderPath = "";
    bool isStart = false;

    bool pausedOnGetPicture = true;
    float RecordDeltaTime = 2f;

    [MenuItem("TA/小工具/屏幕录制")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TransparentBackgroundScreenVideoRecorder window = (TransparentBackgroundScreenVideoRecorder)EditorWindow.GetWindow(typeof(TransparentBackgroundScreenVideoRecorder));
        window.Show();
    }


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
        WriteScreenImageToTexture(tex, rt);
        cam.enabled = false;
    }
    void removeCmp() 
    {
        if(null != whiteCamGameObject)
            GameObject.DestroyImmediate(whiteCamGameObject);
        if(null != blackCamGameObject)
        GameObject.DestroyImmediate(blackCamGameObject);
    }
    private void OnDestroy()
    {
        removeCmp();
        ReleaaseTexture2d();
    }
    Camera blackCam= null;
    Camera whiteCam = null;
    GameObject whiteCamGameObject = null;
    GameObject blackCamGameObject = null;
    int screenWidth ;
    int screenHeight ;
    void initCmp()
    {
        if (null != blackCam)
            return;
        Camera mainCam = Camera.main;
        whiteCamGameObject = (GameObject)new GameObject();
        whiteCamGameObject.name = "White Background Camera";
        whiteCam = whiteCamGameObject.AddComponent<Camera>();
        whiteCam.CopyFrom(mainCam);
        whiteCam.backgroundColor = Color.white;
        whiteCam.enabled = false;
        whiteCam.hideFlags = HideFlags.HideAndDontSave;
        whiteCamGameObject.transform.SetParent(mainCam.transform, true);
        whiteCamGameObject.transform.rotation = mainCam.transform.rotation;

        blackCamGameObject = (GameObject)new GameObject();
        blackCamGameObject.name = "Black Background Camera";
        blackCam = blackCamGameObject.AddComponent<Camera>();
        blackCam.CopyFrom(mainCam);
        blackCam.backgroundColor = Color.black;

        blackCam.enabled = false;
        blackCam.hideFlags = HideFlags.HideAndDontSave;
        blackCamGameObject.transform.SetParent(mainCam.transform, true);
        blackCamGameObject.transform.rotation = mainCam.transform.rotation;

        whiteCamGameObject.hideFlags = HideFlags.HideAndDontSave;
        blackCamGameObject.hideFlags = HideFlags.HideAndDontSave;
    }

    Texture2D textureBlack = null;
    Texture2D textureWhite = null;
    Texture2D textureTransparentBackground = null;
    void initTexture2d()
    {
        if (textureBlack != null && textureBlack.width == screenWidth && textureBlack.height == screenHeight)
            return;
        ReleaaseTexture2d();
        textureBlack = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
        textureWhite = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
        textureTransparentBackground = new Texture2D(screenWidth, screenHeight, TextureFormat.ARGB32, false);
    }
    void ReleaaseTexture2d()
    {
        if(null != textureBlack)
            GameObject.DestroyImmediate(textureBlack);
        if (null != textureWhite)
            GameObject.DestroyImmediate(textureWhite);
        if (null != textureTransparentBackground)
            GameObject.DestroyImmediate(textureTransparentBackground);
    }
   
    void GetScreen(string path)
    {

        screenWidth = Screen.width ;
        screenHeight = Screen.height ;

        initCmp();
        initTexture2d();

        RenderTexture rt = RenderTexture.GetTemporary(screenWidth, screenHeight, 24);
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
        byte[] bytes = textureTransparentBackground.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        RenderTexture.ReleaseTemporary(rt);

    }
    
    int count = 0;

   
 

    float lastFrameTime = 0.0f;
    bool oneFrame = false;
    bool render = false;

    public float m_lastUpdateTime = 0f;
    public float totalTime = 0f;
    void Update()
    {

        float curTime = Time.realtimeSinceStartup;
        m_lastUpdateTime = Mathf.Min(m_lastUpdateTime, curTime);//very important!!!
        float deltaTime = curTime - m_lastUpdateTime;
        if (isStart)
        {
            totalTime += deltaTime;
            if (totalTime < RecordDeltaTime)
            {
                return;
            }
            totalTime -= RecordDeltaTime;
            if (pausedOnGetPicture)
            {
                EditorApplication.isPaused = true;
            }

            GetScreen(saveFolderPath + "/ScreenShot_" + count + ".png");

            count++;
            if (pausedOnGetPicture)
            {
                EditorApplication.isPaused = false;
                //EditorApplication.isPlaying = true;
            }
            
            if (oneFrame)
                isStart = false;
            lastFrameTime = Time.realtimeSinceStartup;
        }
        m_lastUpdateTime = curTime;
    }
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if(saveFolderPath.Length==0)
            GUILayout.Label("请选择文件保存的路径");
        else
            GUILayout.Label(saveFolderPath);

        if (isStart)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("选择"))
        {
            
            string p = EditorUtility.SaveFolderPanel("选择路径", saveFolderPath, "");
            if (p.Length > 0)
                saveFolderPath = p;
        }
        if (isStart)
        {
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
        RecordDeltaTime = EditorGUILayout.Slider("录制间隔",RecordDeltaTime ,1f,3f);
        pausedOnGetPicture = GUILayout.Toggle( pausedOnGetPicture,"获取贴图事自动暂停");           
        if (isStart)
        {
            if (GUILayout.Button("停止"))
            {
                isStart = false;
                totalTime = 0f;
            }
            
        }
        else
        {
            if (GUILayout.Button("开始"))
            {
                if (saveFolderPath.Length == 0)
                {
                    EditorUtility.DisplayDialog("提示", "请选择保存文件的路径", "确定");
                }
                else
                {
                    EditorApplication.isPlaying = true;
                    isStart = true;
                }
                
            }

            if (GUILayout.Button("录一帧"))
            {
                isStart = true;
                oneFrame = true;
            }
        }
        GUILayout.Label("Count = " + count);
    }
}
