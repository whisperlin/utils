#define RESOURCE_LOAD_IN_EDITOR    
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class LAssetBundleManager : MonoBehaviour {

    internal class HandleData
    {
        public bool isFinish = false;
        public object asset = null;
        public string error;
        internal string path;
    }
    public class Handle : CharactorLoadHandle
    {
        internal Handle(HandleData data)
        {
            this.data = data;
        }
        HandleData data;
        public object asset
        {
            get
            {
                return data.asset;
            }
 
        }

        public bool isFinish
        {
            get
            {
                return data.isFinish;
            }

             
        }
        public string Error
        {
            get
            {
                return data.error;
            }


        }
    }
    public static LAssetBundleManager Instance()
    {
        if (null == _instance)
        {
            GameObject g = new GameObject();
            g.name = "AssetBundleManager";
            _instance = g.AddComponent<LAssetBundleManager>();
            GameObject.DontDestroyOnLoad(g);
        }
        return _instance;
    }
    static LAssetBundleManager _instance;
    private void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadConfig());
    }
    //public Object Obj;
    public AssetsData data = null;
    private static string RES_OUTPUT_PATH = "Assets/StreamingAssets/";

    Dictionary<string, object> objexts = new Dictionary<string, object>();
    Dictionary<string, HandleData> loadingHandles = new Dictionary<string, HandleData>();
    Queue<HandleData> loadingHandleList = new Queue<HandleData>();
    public Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
    public Handle loadAsset(string path)
    {
 
        HandleData data;
        object obj;
        if (objexts.TryGetValue(path, out obj))
        {
            data = new HandleData();
            data.path = path;
            data.isFinish = true;
            data.asset = obj;
        }
        else if (!loadingHandles.TryGetValue(path, out data))
        {
            data = new HandleData();
            data.path = path;
            loadingHandles[path] = data;
            loadingHandleList.Enqueue(data);
             //Enqueue():在队列的末端添加元素
            //Dequeue():在队列的头部读取和删除一个元素，注意，这里读取元素的同时也删除了这个元素。如果队列中不再有任何元素。就抛出异常


        }
        return new Handle(data); ;
    }
    public string GetRootUrl()
    {
 
        string baseUrl = "";
        //同步加载路径
        if (Application.platform == RuntimePlatform.Android)
        {
            baseUrl = Application.streamingAssetsPath ;

        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            baseUrl = Application.streamingAssetsPath  ;

        }
        else
        {
            baseUrl = "file:" + Application.streamingAssetsPath ;
        }
        return baseUrl+"/";
    }
    public string GetPlatformRootUrl()
    {
        string baseUrl = "";
        //同步加载路径
        if (Application.platform == RuntimePlatform.Android)
        {
            baseUrl = Application.streamingAssetsPath + "/Android/";

        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            baseUrl = Application.streamingAssetsPath + "/iOS/";

        }
        else
        {
            baseUrl = "file:" + Application.streamingAssetsPath + "/StandaloneWindows/";
        }
        return baseUrl;
    }
    IEnumerator LoadConfig( )
    {
        List<WWW> wwwTemp = new List<WWW>();
 
        HashSet<string> deplendsSet = new HashSet<string>();
        string url = "";
        string baseUrl = GetPlatformRootUrl();
        url = baseUrl+"objects_path.json";

        //Debug.LogError("load url" + url);
        WWW www = new WWW(url);
        yield return www;
        //Debug.LogError("www.error = "+ www.error);
        
        data = JsonConvert.DeserializeObject<AssetsData>(www.text);
        www.Dispose();
        float last = Time.realtimeSinceStartup;
        //Debug.LogError("load url" + url + " finish");
        while (true)
        {
            
            if (loadingHandleList.Count > 0)
            {
                HandleData _data = loadingHandleList.Dequeue();
                AssetIndormation infor;
                if (data.objs.TryGetValue(_data.path, out infor))
                {

#if RESOURCE_LOAD_IN_EDITOR && UNITY_EDITOR

                    //Resources.LoadAssetAtPath()


                    _data.asset = AssetDatabase.LoadAssetAtPath<GameObject>(_data.path);
                    loadingHandles.Remove(_data.path);
                    _data.isFinish = true;
                    continue;
#else
                    deplendsSet.Clear();

                    GetAllDeplend(infor.dependencies, ref deplendsSet);


                    var e = deplendsSet.GetEnumerator();

                    while (e.MoveNext())
                    {
                        string dep = e.Current;
                        AssetBundle bd0;
                        if (bundles.TryGetValue(dep, out bd0))
                        {
                            if (null != bd0)
                                continue;
                        }
                        WWW www1 = new WWW(baseUrl + dep);
                        yield return www1;
                        //Debug.Log("load url " + baseUrl + dep + " finish "   +(Time.realtimeSinceStartup - last));
                        last = Time.realtimeSinceStartup;
                        bundles[dep] = www1.assetBundle;

                    }
                    e.Dispose();


                    AssetBundle bd;
                    if (!bundles.TryGetValue(infor.package, out bd) || null == bd)
                    {
                        WWW www1 = new WWW(baseUrl + infor.package);
                        yield return www1;
                        //Debug.Log("load url " + baseUrl + infor.package + " finish " + (Time.realtimeSinceStartup- last));
                        last = Time.realtimeSinceStartup;
                        bundles[infor.package] = bd = www1.assetBundle;

                    }
                   
                    _data.asset = bd.LoadAsset(_data.path);
                    _data.isFinish = true;
                    //Debug.Log("load obj " + _data.path + " finish " + (Time.realtimeSinceStartup - last));
                    last = Time.realtimeSinceStartup;

                    foreach (string dep in deplendsSet)
                    {
                        AssetBundle bd0;
                        if (!bundles.TryGetValue(dep, out bd0) && null == bd0)
                        {
                            continue;
                        }
                        bd0.Unload(false);
                    }
                    deplendsSet.Clear();
                    bd.Unload(false);
                    loadingHandles.Remove(_data.path);

#endif


                }
                else
                {

#if RESOURCE_LOAD_IN_EDITOR && UNITY_EDITOR

                    if (_data.path.EndsWith(".prefab") || _data.path.EndsWith(".wav"))
                    {
                        _data.asset = AssetDatabase.LoadAssetAtPath<GameObject>(_data.path);
                        loadingHandles.Remove(_data.path);
                        _data.isFinish = true;
                        continue;
                    }
#endif
                    WWW textWWW = new WWW(_data.path);
                    yield return textWWW;
                    _data.asset = textWWW.text;

                    textWWW.Dispose();
                    _data.isFinish = true;
                    _data.error = "非打包文件" + _data.path + "当做文本读取";
                    loadingHandles.Remove(_data.path);
                }
            }
            else
            {
                yield return null;
                //Debug.Log("finished");
            }
        }
    }

    private void GetAllDeplend(List<string> dependencies,  ref HashSet<string> deplendsSet)
    {
        for (int i = 0, c = dependencies.Count; i < c; i++)
        {
            var d = dependencies[i];
            if (deplendsSet.Contains(d))
                continue;
            deplendsSet.Add(d);
            AssetIndormation infor;
            if (data.objs.TryGetValue(d, out infor))
            {
                GetAllDeplend(infor.dependencies,  ref deplendsSet);
            }
        }
    }
}
