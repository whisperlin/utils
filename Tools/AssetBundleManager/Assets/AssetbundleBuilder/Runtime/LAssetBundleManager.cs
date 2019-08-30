using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LAssetBundleManager : MonoBehaviour {

    internal class HandleData
    {
        public bool isFinish = false;
        public object asset = null;
        public string error;
        internal string path;
    }
    public class Handle
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
    public Object Obj;
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

    IEnumerator LoadConfig( )
    {
        List<WWW> wwwTemp = new List<WWW>();
        string url = "";
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
            baseUrl = "file:"+Application.streamingAssetsPath + "/StandaloneWindows/";
        }
        url = baseUrl+"objects_path.json";

        WWW www = new WWW(url);
        yield return www;
        data = JsonConvert.DeserializeObject<AssetsData>(www.text);
        www.Dispose();
        while (true)
        {
            yield return null;
            if (loadingHandleList.Count > 0)
            {
                HandleData _data = loadingHandleList.Dequeue();
                AssetIndormation infor;
                if (data.objs.TryGetValue(_data.path, out infor))
                {
                   // wwwTemp.Clear();
                    for (int i = 0; i < infor.dependencies.Count; i++)
                    {
                        var dep = infor.dependencies[i];

                        AssetBundle bd0;
                        if (bundles.TryGetValue(dep, out bd0))
                        {
                            if (null != bd0)
                                continue;
                        }
                        WWW www1 = new WWW(baseUrl+dep);
                        yield return www1;
                        bundles[dep] = www1.assetBundle;
                         
                    }
                    AssetBundle bd;
                    if (!bundles.TryGetValue(infor.package,out bd)|| null ==bd)
                    {
                        WWW www1 = new WWW(baseUrl+infor.package);
                        yield return www1;
                        bundles[infor.package] = bd = www1.assetBundle;
                        
                    }


                    

                    _data.asset = bd.LoadAsset(_data.path);
                    _data.isFinish = true;


                    for (int i = 0; i < infor.dependencies.Count; i++)
                    {
                        var dep = infor.dependencies[i];
                        AssetBundle bd0;
                        if (!bundles.TryGetValue(dep, out bd0) && null == bd0)
                        {
                            continue;
                        }
                        bd0.Unload(false);
                    }
                    bd.Unload(false);

                }
                else
                {
                    _data.isFinish = true;
                    _data.error = "找不到路径为"+_data.path+"的assetbundle";
                }
            }
        }
    }
    
}
