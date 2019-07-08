#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;



public class MapInfoExport : EditorWindow {
    static float nWidthCheck = 664.0f;
    static float nHeightCheck = 582.0f;
    enum Layer {
        Scene = 8,
        Point = 9,
        Area = 10,
        Gate = 11,
        NPC = 12,
        Monster = 13,
    }

    [MenuItem("Easy/保存小地图相关信息(放大3倍)")]
    static void ExportMapInfo3() {
        ExportMapInfo(3, new Color(0, 0, 0, 0));
    }    

    [MenuItem("Easy/保存小地图相关信息")]
    static void ExportMapInfo0() {
        ExportMapInfo(1, Color.black);
    }
	static void ExportMapInfo(float nScale, Color bgColor) {

        var obj = GameObject.Find("MapInfo");
		if(obj == null) {
            obj = new GameObject("MapInfo");
		}
        MapInfo mapInfo = obj.GetComponent<MapInfo>();
        if (mapInfo == null) {
            mapInfo = obj.AddComponent<MapInfo>();

        }

        float nOrgHeight = 1.0f;
        float nH = 1.0f;
        float nW = 1.0f;
        Vector3 pos = CreateMapInfo(mapInfo, out nOrgHeight, out nW, out nH);
        string sceneName = SceneManager.GetActiveScene().name;

        var path = EditorUtility.SaveFilePanel("选择保存小地图路径", "", sceneName, "jpg");
        if (string.IsNullOrEmpty(path)) {
            return;
        }

        Camera m_camera = GetCamera(nW * nScale, nH * nScale, pos, nOrgHeight, bgColor, mapInfo);

        var rt_old = RenderTexture.active;
        var cam_old = Camera.current;        
        m_camera.Render();
        RenderTexture.active = m_camera.targetTexture;
        TextureFormat txtFormat = TextureFormat.RGB24;
        if (bgColor.a == 0) {
            txtFormat = TextureFormat.RGBA32;
        }
        var tex = new Texture2D(m_camera.targetTexture.width, m_camera.targetTexture.height, txtFormat, false);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
        //tex = ScaleTexture(tex);
        var bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        RenderTexture.active = rt_old;
        Camera.SetupCurrent(cam_old);
        GameObject.DestroyImmediate(m_camera.gameObject);
        var scenePath = EditorApplication.currentScene;
        EditorApplication.SaveScene(scenePath);
        Debug.Log("场景地图信息保存完成!");
	}

    static Texture2D ScaleTexture(Texture2D tex) {
        float nMaxValue = 1024;
        int targetWidth = 0, targetHeight = 0;
        if (tex.width > nMaxValue && tex.width >= tex.height) {
            targetWidth = (int)nMaxValue;
            targetHeight = (int)(tex.height * nMaxValue / tex.width);
        } else if (tex.height > nMaxValue) {
            targetWidth = (int)(tex.width * nMaxValue) / tex.height;
            targetHeight = (int)nMaxValue;
        } else {
            return tex;
        }

        Texture2D result = new Texture2D(targetWidth, targetHeight, tex.format, false);

        for (int i = 0; i < result.height; ++i) {
            for (int j = 0; j < result.width; ++j) {
                Color newColor = tex.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }

    static Texture2D AddTextureBord(Texture2D tex) {
        float nMaxValue = 1024;
        int targetWidth = 0, targetHeight = 0;
        if (tex.width > nMaxValue && tex.width >= tex.height) {
            targetWidth = (int)nMaxValue;
            targetHeight = (int)(tex.height * nMaxValue / tex.width);
        } else if (tex.height > nMaxValue) {
            targetWidth = (int)(tex.width * nMaxValue) / tex.height;
            targetHeight = (int)nMaxValue;
        } else {
            return tex;
        }

        Texture2D result = new Texture2D(targetWidth, targetHeight, tex.format, false);

        for (int i = 0; i < result.height; ++i) {
            for (int j = 0; j < result.width; ++j) {
                Color newColor = tex.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }

    static Camera GetCamera(float w, float h, Vector3 pos, float nOrgHeight, Color bgColor, MapInfo mapInfo) {
        Camera m_camera = null;
        if (m_camera == null) {
            var find = GameObject.Find("MinMapCamera");
            if (find != null) {
                m_camera = find.GetComponent<Camera>();
            }
        }

        if (m_camera != null) {
            if (m_camera.targetTexture.width != Mathf.FloorToInt(w) || m_camera.targetTexture.height != Mathf.FloorToInt(h)) {
                GameObject.Destroy(m_camera.gameObject);
                m_camera = null;
            }
        }

        if (m_camera == null) {
            var texture = new RenderTexture(Mathf.FloorToInt(w), Mathf.FloorToInt(h), 24, RenderTextureFormat.ARGB32);
            texture.antiAliasing = 8;            
            var camera = new GameObject("MinMapCamera").AddComponent<Camera>();
            camera.targetTexture = texture;
            m_camera = camera;
        }


        m_camera.clearFlags = CameraClearFlags.Color;
        m_camera.backgroundColor = bgColor;
        m_camera.orthographic = true;
        m_camera.orthographicSize = nOrgHeight / 2;
        m_camera.nearClipPlane = -200;
        m_camera.farClipPlane = 200;
        m_camera.cullingMask = -1;

        m_camera.transform.position = pos;
        var mainCamera = Camera.main;
        var eulerAgle = mainCamera.transform.rotation.eulerAngles;
        m_camera.transform.rotation = Quaternion.Euler(90, eulerAgle.y, eulerAgle.z);
        mapInfo.nRotateY = eulerAgle.y;
        return m_camera;
    }


    static Vector3 CreateMapInfo(MapInfo mapInfo, out float nOrgHeight, out float nW, out float nH) {
        /*GameObject obj = mapInfo.gameObject;
        BoxCollider box = obj.GetComponent<BoxCollider>();
        if (box == null) {
            box = obj.AddComponent<BoxCollider>();

        }
        */
        UnityEngine.Object [] objs = GameObject.FindObjectsOfTypeAll(typeof(MeshFilter));
        float minX = 10000;
        float minZ = 10000;
        float maxX = -10000;
        float maxZ = -10000;
        float cur_minX = 0;
        float cur_maxX = 0;
        float cur_minZ = 0;
        float cur_maxZ = 0;
        nOrgHeight = 1;
        nW = 1;
        nH = 1;

        foreach (UnityEngine.Object meshObj1 in objs) {            
            MeshFilter filter = (MeshFilter)meshObj1;
            if (!filter.gameObject.activeInHierarchy) continue;
            if (filter.sharedMesh == null) continue;
            if (filter.gameObject.layer != (int)Layer.Scene) continue;
            var size = filter.sharedMesh.bounds.size; 
            if (filter != null && size.x > 0 && size.z > 0) {
                var center = filter.sharedMesh.bounds.center;
                Transform tr = filter.transform; 
                float angleX = tr.rotation.eulerAngles.x;
                float angleY = tr.rotation.eulerAngles.y;
                float angleZ = tr.rotation.eulerAngles.z;

                if (angleX != 0 || angleY != 0 || angleZ != 0) {

                    Vector3 L_V1 = center + new Vector3(-size.x / 2, size.y / 2, -size.z / 2);
                    Vector3 L_V2 = center + new Vector3(-size.x / 2, size.y / 2, size.z / 2);
                    Vector3 L_V3 = center + new Vector3(size.x / 2, size.y / 2, -size.z / 2);
                    Vector3 L_V4 = center + new Vector3(size.x / 2, size.y / 2, size.z / 2);
                    Vector3 L_V5 = center + new Vector3(-size.x / 2, -size.y / 2, -size.z / 2);
                    Vector3 L_V6 = center + new Vector3(-size.x / 2, -size.y / 2, size.z / 2);
                    Vector3 L_V7 = center + new Vector3(size.x / 2, -size.y / 2, -size.z / 2);
                    Vector3 L_V8 = center + new Vector3(size.x / 2, -size.y / 2, size.z / 2);                    

                    Vector3 W_V1 = tr.TransformPoint(L_V1);
                    Vector3 W_V2 = tr.TransformPoint(L_V2);
                    Vector3 W_V3 = tr.TransformPoint(L_V3);
                    Vector3 W_V4 = tr.TransformPoint(L_V4);
                    Vector3 W_V5 = tr.TransformPoint(L_V5);
                    Vector3 W_V6 = tr.TransformPoint(L_V6);
                    Vector3 W_V7 = tr.TransformPoint(L_V7);
                    Vector3 W_V8 = tr.TransformPoint(L_V8);

                    cur_minX = Math.Min(W_V1.x, W_V2.x);
                    cur_minX = Math.Min(cur_minX, W_V3.x);
                    cur_minX = Math.Min(cur_minX, W_V4.x);
                    cur_minX = Math.Min(cur_minX, W_V5.x);
                    cur_minX = Math.Min(cur_minX, W_V6.x);
                    cur_minX = Math.Min(cur_minX, W_V7.x);
                    cur_minX = Math.Min(cur_minX, W_V8.x);

                    cur_maxX = Math.Max(W_V1.x, W_V2.x);
                    cur_maxX = Math.Max(cur_maxX, W_V3.x);
                    cur_maxX = Math.Max(cur_maxX, W_V4.x);
                    cur_maxX = Math.Max(cur_maxX, W_V5.x);
                    cur_maxX = Math.Max(cur_maxX, W_V6.x);
                    cur_maxX = Math.Max(cur_maxX, W_V7.x);
                    cur_maxX = Math.Max(cur_maxX, W_V8.x);

                    cur_minZ = Math.Min(W_V1.z, W_V2.z);
                    cur_minZ = Math.Min(cur_minZ, W_V3.z);
                    cur_minZ = Math.Min(cur_minZ, W_V4.z);
                    cur_minZ = Math.Min(cur_minZ, W_V5.z);
                    cur_minZ = Math.Min(cur_minZ, W_V6.z);
                    cur_minZ = Math.Min(cur_minZ, W_V7.z);
                    cur_minZ = Math.Min(cur_minZ, W_V8.z);

                    cur_maxZ = Math.Max(W_V1.z, W_V2.z);
                    cur_maxZ = Math.Max(cur_maxZ, W_V3.z);
                    cur_maxZ = Math.Max(cur_maxZ, W_V4.z);
                    cur_maxZ = Math.Max(cur_maxZ, W_V5.z);
                    cur_maxZ = Math.Max(cur_maxZ, W_V6.z);
                    cur_maxZ = Math.Max(cur_maxZ, W_V7.z);
                    cur_maxZ = Math.Max(cur_maxZ, W_V8.z);
                } else {

                    cur_minX = (center.x - size.x / 2);// *tr.localScale.x;
                    cur_maxX = (center.x + size.x / 2);// * tr.localScale.x;

                    cur_minZ = (center.z - size.z / 2);// * tr.localScale.z;
                    cur_maxZ = (center.z + size.z / 2);// * tr.localScale.z;

                    Vector3 L_V1Min = new Vector3(cur_minX, center.y, cur_minZ);
                    Vector3 L_V1Max = new Vector3(cur_maxX, center.y, cur_maxZ);

                    Vector3 W_V1Min = tr.TransformPoint(L_V1Min);
                    Vector3 W_V2Max = tr.TransformPoint(L_V1Max);

                    cur_minX = W_V1Min.x;
                    cur_maxX = W_V2Max.x;

                    cur_minZ = W_V1Min.z;
                    cur_maxZ = W_V2Max.z;
                }

                minX = Math.Min(minX, cur_minX);
                minZ = Math.Min(minZ, cur_minZ);
                maxX = Math.Max(maxX, cur_maxX);
                maxZ = Math.Max(maxZ, cur_maxZ);
                /*
                BoxCollider box1 = filter.GetComponent<BoxCollider>();
                if (box1 == null) {
                    box1 = filter.gameObject.AddComponent<BoxCollider>();
                }
                box1.size = new Vector3(size.x, size.y, size.z);
                box1.center = center;
                 */
            }
        }
        /*
        box.size = new Vector3(maxX - minX, 50, maxZ - minZ);
        box.center = new Vector3((maxX + minX) / 2, 0, (maxZ + minZ) / 2);
        box.transform.position = Vector3.zero;
                 **/

        Vector3 vc = new Vector3((maxX + minX) / 2, 0, (maxZ + minZ) / 2);
        float nMapW = maxX - minX;
        float nMapH = maxZ - minZ;

        //mapInfo.srcMapWidth = nMapW;
        //mapInfo.srcMapHeight = nMapH ;
        //mapInfo.nOffX = minX;
        //mapInfo.nOffZ = minZ;

        //开始算换成等比的大小
        //Debug.Log("MaxX=" + maxX + " MaxZ=" + maxZ + "  minX=" + minX + "  minZ=" + minZ);
        float nWidth = nWidthCheck;
        float nHeight = nHeightCheck;
        nW = nWidth;
        nH = nHeight;
        mapInfo.nOffX = vc.x;
        mapInfo.nOffZ = vc.z;
        float nWHRate = nWidth/nHeight;

        float nOrgWidth = nMapW;
        if (nMapW / nMapH > nWHRate) {
            nOrgWidth = nMapW;
            nOrgHeight = nMapW * nHeight / nWidth;
        } else {
            nOrgHeight = nMapH;
            nOrgWidth = nMapH * nWidth / nHeight;
        }
        mapInfo.srcMapWidth = nOrgWidth;
        mapInfo.srcMapHeight = nOrgHeight;

        return vc;
    }

    void GetMapInfoScale() {
        float nOrgHeight = 1;
        float nWidth = 664.0f;
        float nHeight = 582.0f;
        float nMapW = 1;
        float nMapH = 1;
        float nWHRate = nWidth / nHeight;

        if (nMapW / nMapH > nWHRate) {
            nOrgHeight = nMapW * nHeight / nWidth;
        } else {
            nOrgHeight = nMapH;
        }
    }
    /*
    [MenuItem("Assets/修改小地图路径为jpg")]
    public static void ChangeSceneUrl() {
        string[] filePaths = Directory.GetFiles("Assets/Scene_Processed/", "*.unity", SearchOption.TopDirectoryOnly);
        foreach (var filePath in filePaths) {
            var fileName = Path.GetFileName(filePath);
            //var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
            Scene scene = SceneManager.GetSceneByPath(filePath);
            List<GameObject> rootGameObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootGameObjects);
            var scenePath = EditorApplication.currentScene;
            EditorApplication.SaveScene(scenePath);
            break;
        }

    }*/

    [MenuItem("Assets/打印场景MeshRenderer丢失材质和Shader的对象")]
    public static void PrintMissShader() {
        string[] filePaths = Directory.GetFiles("Assets/Scene_Processed/", "*.unity", SearchOption.TopDirectoryOnly);
        EditorUtility.DisplayProgressBar("PrintMissShader", "", 0);
        uint count = 0;
        foreach (var filePath in filePaths) {
            EditorUtility.DisplayProgressBar("PrintMissShader", "", count / filePaths.Length);
            var fileName = Path.GetFileName(filePath);
            //var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
            //SceneManager.LoadScene(fileName, LoadSceneMode.Single);
            //Application.LoadLevel(fileName);
            //UnityEditor.EditorApplication.LoadLevelInPlayMode(filePath);
            //
            bool bresult = EditorApplication.OpenScene(filePath);
            Scene scene = SceneManager.GetSceneByPath(filePath);
            if (scene == null) {
                Debug.LogError(string.Format("scene={0} is null", filePaths));
                continue;
            }
            List<GameObject> rootGameObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootGameObjects);
            //var scenePath = EditorApplication.currentScene;
            //EditorApplication.SaveScene(scenePath);
            var iter = rootGameObjects.GetEnumerator();
            while (iter.MoveNext()) {
                var mesh = iter.Current.GetComponentsInChildren<MeshRenderer>();
                if (mesh != null) {
                    if (iter.Current.name == "Testaaa") {
                        int countx = 0;
                    }
                    for (int i = 0; i < mesh.Length; i++) {
                        var mats = mesh[i].sharedMaterials;
                        for (int j = 0; j < mats.Length; j++) {
                            if (mats[j] == null) {
                                Debug.Log(string.Format("scene={0}中的{1}中MeshRenderer={2}的mats index={3} is null", filePath, iter.Current.name, mesh[i].name, j.ToString()));
                            } else {
                                if (mats[j].shader.name == "Hidden/InternalErrorShader") {
                                    //Nature/Terrain/Diffuse 
                                    Debug.Log(string.Format("scene={0}中的{1}中MeshRenderer={2}的mats index={3} is InternalErrorShader", filePath, iter.Current.name, mesh[i].name, j.ToString()));
                                }
                            }
                        }
                    }
                }
            }

            count++;
        }
        EditorUtility.ClearProgressBar();
        Debug.Log(string.Format("执行完成"));

    }
   
}

#endif