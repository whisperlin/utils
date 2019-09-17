using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoadObj : MonoBehaviour {

    public string path = "Assets/Props/Prefabs/Character/Role/gong_shou_108.prefab";
    // Use this for initialization
    void Start () {
        StartCoroutine(LoadObj());
	}

    private IEnumerator LoadObj()
    {
        var handle = LAssetBundleManager.Instance().loadAsset(path);
        while (!handle.isFinish)
        {
            yield return 1;
        }
        if (null == handle.asset)
        {
            Debug.LogError(handle.Error);
        }
        else
        {
            GameObject g =  GameObject.Instantiate<GameObject>((GameObject)handle.asset);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
