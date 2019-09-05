using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LChatacterRecourceLoader : MonoBehaviour ,LChatacterRecourceInterface
{

    public static LChatacterRecourceLoader _instace;
    public static LChatacterRecourceLoader Instance()
    {
        if (null == _instace)
        {
            GameObject g = new GameObject();
            _instace = g.AddComponent<LChatacterRecourceLoader>();
            g.name = "LChatacterRecourceLoader";
            //g.hideFlags = HideFlags.DontSave;
            //GameObject.DontDestroyOnLoad(g);
        }
        return _instace;
    }
   
   

    

    Dictionary<string, LCHSkillData> skillDatas = new Dictionary<string, LCHSkillData>();
    LCHSkillData GetSkill(string skillName)
    {
        LCHSkillData data;
        if (skillDatas.TryGetValue(skillName, out data))
        {
            return data;
        }
        return null;
    }
 

    public void LoadSound(string name, string path)
    {
        throw new System.NotImplementedException();
    }

    public void PlaySound(string name, string path, Vector3 pos)
    {
        throw new System.NotImplementedException();
    }

 

    public void ReleaseResource(string path, string name)
    {
        throw new System.NotImplementedException();
    }

  

    void LChatacterRecourceInterface.LoadSound(string name, string path)
    {
        throw new NotImplementedException();
    }

    void LChatacterRecourceInterface.PlaySound(string name, string path, Vector3 pos)
    {
        throw new NotImplementedException();
    }

    

    void LChatacterRecourceInterface.ReleaseResource(string path, string name)
    {
        throw new NotImplementedException();
    }

    void LChatacterRecourceInterface.AddCoroutine(IEnumerator fun)
    {
        throw new NotImplementedException();
    }

    CharactorLoadHandle LChatacterRecourceInterface.loadResource(string name, string path, AddCoroutineFun fun)
    {
        var handle = LAssetBundleManager.Instance().loadAsset(path);

        return handle;
    }

    private IEnumerator LoadSkillData(CharactorLoadResultData res, string skillId)
    {
        //G:\utils\Tools\SkillEditor\Assets\StreamingAssets\SkillEditor\Data
        string path = LAssetBundleManager.Instance().GetRootUrl() + "SkillEditor/Data/" + skillId + ".skill";
        var handle = LAssetBundleManager.Instance().loadAsset(path);
        while (!handle.isFinish)
        {
            yield return null;
        }
        string txt = (string)handle.asset;
        var skillData = JSonHelper.DeserializeSkill(txt);
        skillDatas[skillId] = skillData;
        res.asset = skillData;
        res.isFinish = true;
    }

    CharactorLoadHandle LChatacterRecourceInterface.LoadSkillDataFile(string skillId, AddCoroutineFun fun)
    {
        LCHSkillData skillData;
        CharactorLoadResultData _res = new CharactorLoadResultData();
        CharactorLoadResult h = new CharactorLoadResult(_res);
        if (skillDatas.TryGetValue(skillId, out skillData))
        {

            _res.asset = skillData;

        }
        else
        {
            fun(LoadSkillData(_res, skillId));
        }
        return h;
    }
}
