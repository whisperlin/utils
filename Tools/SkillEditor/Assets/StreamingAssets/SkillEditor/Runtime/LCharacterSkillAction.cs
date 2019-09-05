using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCharacterSkillAction : LChatacterAction
{
    LCHSkillData skillData;

    public bool HasLoaded
    {
        get
        {
            return hasLoaded;
        }
    }
    private bool hasLoaded = false;
    LCharacterSkillAction(LCHSkillData data)
    {
        
        this.skillData = data;
    }

    public ObjectContain role = new ObjectContain();
    public Dictionary<int,ObjectContain> objs = new Dictionary<int, ObjectContain>();
    public Dictionary<int, AudioClip> clips = new Dictionary<int, AudioClip>();

    

    public IEnumerator LoadSkill(LChatacterRecourceInterface loader)
    {
        hasLoaded = false;
        for (int i = 0, l = skillData.objs.Length; i < l; i++)
        {
            var o = skillData.objs[i];
            ObjectContain oc = new ObjectContain();
            oc.SetInformation(o);
            objs[o.id] = oc;
            if (o.id == -1)//不加载主模型。
            {
                continue;   
            }
            else if (o.id == 1)//对象(特效，模型...)。
            {
                var handle = loader.loadResource(oc.mod_name,oc.mod);
                while(!handle.isFinish)
                {
                    yield return 1;
                }
                if (null != handle.asset)
                {
                    oc.gameobject = (GameObject)handle.asset;
                    if (null != oc.gameobject)
                        oc.gameobject.name = o.name;
                }
                
            }
            else if(o.id == 2)//(碰撞体)。
            {
                GameObject g = new GameObject();
                g.AddComponent<BoxCollider>();
                g.hideFlags = HideFlags.DontSave;
                oc.gameobject = g;
                
                g.name = o.name; 
            }
            else if(o.id == -1)//模型本身碰撞体。暂不连接。
            {
                continue;
            }
            else if(o.id == -1)//不加载主模型。
            {
                loader.loadResource(oc.mod_name, oc.mod);
                //FindColliderInChild(role.gameobject, name);
            }
        }
        hasLoaded = true;

    }
    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        
    }

    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        
    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        return false;
    }

    public override bool isQualified(LChatacterInterface character, LChatacterInformationInterface information)
    {
        return false;
    }

    
}
