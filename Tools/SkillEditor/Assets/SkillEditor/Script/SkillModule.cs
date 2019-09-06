using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
public enum SkillEvent
{
    CreateRole,
    DeleteRole,

    OnRoleListModify,
    CreateSkill,
    SkillAddObject,
    OnSkillObjectModify,
    SkillLerpFloatChannel,
    AddChanneled,
    AddKeyFrame,
    RemoveKeyFrame,
    UpdateKeyFrame,
    SkillAddBoxCollider,
    RemoveChannel,
    RemoveObject,
    SkillEventChannel,
    SkillAddBaseCollider,
    SkillAddSound,
}
public enum LCHChannelType
{
    PosX = 0,
    PosY = 1,
    PosZ = 2,
    ScaleX = 3,
    ScaleY = 4,
    ScaleZ = 5,
    RotY = 7,

    Object = 100,
    Event = 101,
    RoleState = 102,
}

public enum SkillDataType
{
    ROLE,
    SKILL
}

public interface SkillDataLoader
{
    string LoadFile(string id, SkillDataType type);

    bool Exists(string id, SkillDataType type);
    void SaveFile(string id,string text, SkillDataType type);
    List<string> GetIds(SkillDataType type);

    string loadPropertys();
 
 
    void DeleveFile(string id, SkillDataType type);
}
public class SkillData {


    public  static LCHChannelType[] singleChannels = new LCHChannelType[] { LCHChannelType.PosX, LCHChannelType.PosY, LCHChannelType.PosZ };

    public bool EditorModel = false;
    static int objCount = 1;
    int id;
    public SkillData()
    {
        this.id = objCount;
        objCount++;
    }
#if UNITY_EDITOR
    public SkillDataLoader loader;
#else
    SkillDataLoader loader;
#endif
    public void Init(SkillDataLoader loader)
    {
        this.loader = loader;
#if UNITY_EDITOR
        EventManager.AddEvent((int)SkillEvent.CreateRole, CreateRole);
        EventManager.AddEvent((int)SkillEvent.CreateSkill, CreateSkill);
        EventManager.AddEvent((int)SkillEvent.SkillAddObject, SkillAddObject);
        EventManager.AddEvent((int)SkillEvent.SkillLerpFloatChannel, SkillLerpFloatChannel);

        EventManager.AddEvent((int)SkillEvent.AddKeyFrame, AddKeyFrame);
        EventManager.AddEvent((int)SkillEvent.RemoveKeyFrame, RemoveKeyFrame);
        EventManager.AddEvent((int)SkillEvent.SkillAddBoxCollider, SkillAddBoxCollider);
        EventManager.AddEvent((int)SkillEvent.RemoveChannel, RemoveChannel);
        EventManager.AddEvent((int)SkillEvent.RemoveObject, RemoveObject);
        EventManager.AddEvent((int)SkillEvent.SkillEventChannel, SkillEventChannel);
        EventManager.AddEvent((int)SkillEvent.SkillAddBaseCollider, SkillAddBaseCollider);
        EventManager.AddEvent((int)SkillEvent.SkillAddSound, SkillAddSound);
        

#endif
        Dictionary<string, object> _dic = JSonHelper.DeserializeDictionary(loader.loadPropertys());
        propertyTemp = (Dictionary<string, object>)_dic["propertys"];
        eventTemp = (Dictionary<string, object>)_dic["event"];
        objectTemp = (Dictionary<string, object>)_dic["object"];
        roleStatetTemp = (Dictionary<string, object>)_dic["role_state"];
        var l = new List<string>();
        l.AddRange(propertyTemp.Keys);
        propertyKeys = l.ToArray();
        l.Clear();
        l.AddRange(eventTemp.Keys);
        eventKeys = l.ToArray();
        l.Clear();
        l.AddRange(objectTemp.Keys);
        objectKeys = l.ToArray();

        l.Clear();
        l.AddRange(roleStatetTemp.Keys);
        roleStateKeys = l.ToArray();

    }

    public bool isInited()
    {
        return null != loader;
    }
    //这种只能在编辑器用。
    Dictionary<string, object> propertyTemp;
    Dictionary<string, object> eventTemp;
    Dictionary<string, object> objectTemp;
    Dictionary<string, object> roleStatetTemp;
    //用这个来避免gc。
    string[] propertyKeys  ;
    string[] eventKeys;
    string[] objectKeys;
    string[] roleStateKeys;

    public Dictionary<string, object> GetEventTemp()
    {
        return eventTemp;
    }
    public Dictionary<string, object> GetObjecctTemp()
    {
        return objectTemp;
    }
    
    public Dictionary<string, object> GetPropertyTemp()
    {
        return propertyTemp;
    }

    public Dictionary<string, object> GetRoleStateTemp()
    {
        return roleStatetTemp;
    }
    
    public string[] GetPropertyNames()
    {
        return propertyKeys;
    }
    public string[] GetEventNames()
    {
        return eventKeys;
    }

    public string[] GetObjectNames()
    {
        return objectKeys;
    }

    public string[] GetRoleStateNames()
    {
        return roleStateKeys;
    }
    
    public void Release()
    {
#if UNITY_EDITOR
        EventManager.RemoveEvent((int)SkillEvent.CreateRole, CreateRole);
        EventManager.RemoveEvent((int)SkillEvent.CreateSkill, CreateSkill);
        EventManager.RemoveEvent((int)SkillEvent.SkillAddObject, SkillAddObject);
        EventManager.RemoveEvent((int)SkillEvent.SkillLerpFloatChannel, SkillLerpFloatChannel);

        EventManager.RemoveEvent((int)SkillEvent.AddKeyFrame, AddKeyFrame);
        EventManager.RemoveEvent((int)SkillEvent.RemoveKeyFrame, RemoveKeyFrame);
        EventManager.RemoveEvent((int)SkillEvent.SkillAddBoxCollider, SkillAddBoxCollider);
        EventManager.RemoveEvent((int)SkillEvent.RemoveChannel, RemoveChannel);
        EventManager.RemoveEvent((int)SkillEvent.RemoveObject, RemoveObject);
        EventManager.RemoveEvent((int)SkillEvent.SkillEventChannel, SkillEventChannel);
        EventManager.RemoveEvent((int)SkillEvent.SkillAddBaseCollider, SkillAddBaseCollider);
        EventManager.RemoveEvent((int)SkillEvent.SkillAddSound, SkillAddSound);
#endif
        loader = null;
    }

    

    private void RemoveObject(object[] args)
    {
        string skillId = (string)args[0];
        int objId = (int)args[1];
        var skill = GetSkill(skillId);

        var o0 = skill.GetObject(objId);
        skill.RemoveObject(objId);
        if ( null != o0 && o0.type == 1)
        {
            for (int i = skill.objs.Length-1; i >=0; i-- )
            {
                var o1 = skill.objs[i];
                if (o1.type == 3)
                {
                    int bind = o1.propertys.GetValueInt("bind", 0);
                    if (bind == 2)
                    {
                        int objid = o1.propertys.GetValueInt("objid", 0);
                        if (objid == o0.id)
                        {
                            skill.RemoveObject(o1.id);
                        }
                    }
                }

            }
        }
        SaveSkill(skillId);


    }

    private void RemoveEventChannel(string skillId, int curSelectChannel)
    {
     
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 || curSelectChannel >= skill.events.Count)
            return;

        skill.events.RemoveAt(curSelectChannel);
        SaveSkill(skillId);

    }
    private void RemoveChannel(object[] args)
    {
        string skillId = (string)args[0];
        int curSelectChannel = (int)args[1];
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0  )
            return;
        if (curSelectChannel >= skill.channels.Count)
        {
            RemoveEventChannel(skillId, curSelectChannel - skill.channels.Count);
            return;
        }
            
        
        skill.channels.RemoveAt(curSelectChannel);
        SaveSkill(skillId);

    }
    private void RemoveKeyFrame(object[] args)
    {
        string skillId = (string)args[0];
        int curSelectChannel = (int)args[1];
        int curSelectFrame = (int)args[2];
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 )
            return;
        if (curSelectChannel >= skill.channels.Count)
        {
            RemoveEventKeyFrame(skillId, curSelectChannel - skill.channels.Count, curSelectFrame);
            return;
        }
        var channel = skill.channels[curSelectChannel];
        int count = channel.times.Count;
        for (int i = 0; i < count; i++)
        {
            var f = channel.times[i];
            if (f == curSelectFrame)
            {
                channel.DeleteFrame(i);
                return;
            }
        }
        SaveSkill(skillId);
    }

    private void RemoveEventKeyFrame(string skillId, int curSelectChannel, int curSelectFrame)
    {
     
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 || curSelectChannel >= skill.events.Count)
            return;
        var events = skill.events[curSelectChannel];
        int count = events.times.Count;
        for (int i = 0; i < count; i++)
        {
            var f = events.times[i];
            if (f == curSelectFrame)
            {
                events.DeleteFrameByIndex(i);
                return;
            }
        }
        SaveSkill(skillId);
    }

    private void AddKeyFrame(object[] args)
    {
        string skillId = (string)args[0];
        int curSelectChannel = (int)args[1];
        int curSelectFrame = (int)args[2];
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 )
            return;
        if (curSelectChannel >= skill.channels.Count)
        {
            AddEventKeyFrame(skillId, curSelectChannel - skill.channels.Count, curSelectFrame);
            return;
        }
        var channel = skill.channels[curSelectChannel];
        int count = channel.times.Count;
        for (int i = 0 ; i < count; i++)
        {
            var f = channel.times[i];
            if (f > curSelectFrame)
            {
                channel.AddKey(i, curSelectFrame);
                return;
            }
            else if (f == curSelectFrame)
            {
                return;
            }
        }
        channel.AddKey(count, curSelectFrame);
        SaveSkill(skillId);
       
    }

    private void AddEventKeyFrame(string skillId, int curSelectChannel, int curSelectFrame)
    {
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 || curSelectChannel >= skill.events.Count)
            return;
        var events = skill.events[curSelectChannel];
        int count = events.times.Count;
        for (int i = 0; i < count; i++)
        {
            var f = events.times[i];
            if (f > curSelectFrame)
            {
                events.AddKey(i, curSelectFrame);
                return;
            }
            else if (f == curSelectFrame)
            {
                return;
            }
        }
        events.AddKey(count, curSelectFrame);
        SaveSkill(skillId);

    }



    public void CreateRole(params object[] arg)
    {
        string id = (string)arg[0];
        int type = (int)arg[1];
        LCHRoleData r = new LCHRoleData();
        r.id= id;
        string json = JsonConvert.SerializeObject(r);
#if UNITY_EDITOR
        if (EditorModel && null == loader)
            loader = new EditorFileLoader();
        if (loader.Exists(id, SkillDataType.ROLE))
        {
            EditorUtility.DisplayDialog("提示", "同id角色已经存", "确定");
            return;
             
        }
#endif
        loader.SaveFile(id, json, SkillDataType.ROLE);
        EventManager.CallEvent((int)SkillEvent.OnRoleListModify, id, type);
    }

    bool HasChannel(LCHSkillData skill ,int objId, int type,params object[] arg)
    {
        int len = skill.channels.Count;
        for (int i = 0; i < len; i++)
        {
            var c = skill.channels[i];
            if (c.type == type&&c.objId == objId)
            {

                return false;
            }
        }
        return true;
    }

    bool HasEventChannel(LCHSkillData skill, int objId, int type, params object[] arg)
    {
        int len = skill.events.Count;
        for (int i = 0; i < len; i++)
        {
            var c = skill.events[i];
            if (c.type == type && c.objId == objId)
            {
                return false;
            }
        }
        return true;
    }
    private void SkillEventChannel(object[] args)
    {
        string skillId = (string)args[0];//pos_x
        int objId = (int)args[1];
        int type = (int)args[2];
        LCHSkillData skill = GetSkill(skillId);
        if (!HasEventChannel(skill, objId, type))
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("提示", "相同轨迹已经存在", "确定");
#endif
            return;
        }
        LCHEventChannelData channel = new LCHEventChannelData();
        channel.type = (int)type;
        channel.objId = objId;
        skill.events.Add(channel);
        SaveSkill(skill.id);
        EventManager.CallEvent((int)SkillEvent.AddChanneled);
    }
    private void SkillLerpFloatChannel(object[] args)
    {
        
        string skillId = (string)args[0];//pos_x
        int objId = (int)args[1];
        int type = (int)args[2];
        LCHSkillData skill = GetSkill(skillId);
        if (!HasChannel(skill,objId,type))
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("提示", "相同轨迹已经存在", "确定");
#endif
            return;
        }

        LCHChannelData channel = new LCHChannelData();
        channel.type = (int)type;
        channel.objId = objId;
        skill.channels.Add(channel);
        SaveSkill(skill.id);
        EventManager.CallEvent((int)SkillEvent.AddChanneled);
        //Dictionary<SkillData>

    }
    private void SkillAddSound(object[] args)
    {
        string skillId = (string)args[0];
 
        LCHSkillData skill = GetSkill(skillId);
        var v = skill.objs;
        int objCount = v.Length;
        int id = 0;
        List<object> _list = new List<object>();
        for (int i = 0; i < objCount; i++)
        {
            int _id = v[i].id;
            if (id <= _id)
            {
                id = _id + 1;
            }
        }
        LCHObjectData eo = new LCHObjectData();
        eo.type = 4;
        eo.id = id;
        eo.name = "声音-未命名";
        eo.propertys["mod"] = "";
        eo.propertys["mod_name"] = "";
 

        LCHSkillData s = new LCHSkillData();
        skill.objs = ArrayHelper.AddItem<LCHObjectData>(skill.objs, eo);
        SaveSkill(skillId);
        EventManager.CallEvent((int)SkillEvent.OnSkillObjectModify);
    }
    private void SkillAddBaseCollider(object[] args)
    {

        string skillId = (string)args[0];
        int objId = (int)args[1];
        string bindName = (string)args[2];
        LCHSkillData skill = GetSkill(skillId);
        var v = skill.objs;
        int objCount = v.Length;
        int id = 0;
        List<object> _list = new List<object>();
        for (int i = 0; i < objCount; i++)
        {
            int _id = v[i].id;
            if (id <= _id)
            {
                id = _id + 1;
            }
        }
        LCHObjectData eo = new LCHObjectData();
        eo.type = 3;
        eo.id = id;
        eo.name = "触发器-未命名";
        if (bindName.Length > 0)
        {
            eo.propertys["bind"] = 2;
        }
        else
        {
            eo.propertys["bind"] = 1;
        }
        
        eo.propertys["objid"] = objId;
        eo.propertys["bind_name"] = bindName;
        LCHSkillData s = new LCHSkillData();
        skill.objs = ArrayHelper.AddItem<LCHObjectData>(skill.objs, eo);
        SaveSkill(skillId);
        EventManager.CallEvent((int)SkillEvent.OnSkillObjectModify);
    }
    private void SkillAddBoxCollider(object[] args)
    {
        string skillId = (string)args[0];
        LCHSkillData skill = GetSkill(skillId);
        var v = skill.objs;
        int objCount = v.Length;
        int id = 0;
        List<object> _list = new List<object>();
        for (int i = 0; i < objCount; i++)
        {
            int _id = v[i].id;
            if (id <= _id)
            {
                id = _id + 1;
            }
        }
        LCHObjectData eo = new LCHObjectData();
        eo.type = 2;
        eo.id = id;
        eo.name = "触发器-未命名";
        LCHSkillData s = new LCHSkillData();
        skill.objs = ArrayHelper.AddItem<LCHObjectData>(skill.objs, eo);
        SaveSkill(skillId);
        EventManager.CallEvent((int)SkillEvent.OnSkillObjectModify);
    }
    private void SkillAddObject(object[] args)
    {
        string skillId = (string)args[0];
        LCHSkillData skill = GetSkill(skillId);
        var v = skill.objs;

        int objCount = v.Length;
        int id = 0;
        List<object> _list = new List<object>();
        for (int i = 0; i < objCount; i++)
        {
            int _id = v[i].id;
            if (id <= _id)
            {
                id = _id + 1;
            }
        }
        LCHObjectData eo = new LCHObjectData();
        eo.type = 1;
        eo.id = id;
        eo.name = "对象-未命名"; 
        LCHSkillData s = new LCHSkillData();
        skill.objs =  ArrayHelper.AddItem<LCHObjectData>(skill.objs, eo);
        SaveSkill(skillId);
        //skill["objs"] = JSonHelper.AddItem(sks, ct);
        EventManager.CallEvent((int)SkillEvent.OnSkillObjectModify);
    }

    

    public void CreateSkill(params object[] arg)
    {
        string id = arg[0].ToString();
        string roidId = arg[1].ToString();

#if UNITY_EDITOR
        if (this.loader.Exists(id, SkillDataType.SKILL))
        {
            EditorUtility.DisplayDialog("提示", "同id技能已经存在", "确定");
            return;
        }
#endif
        var roid = GetRole(roidId);
        roid.skills = ArrayHelper.AddItem(roid.skills, id);

        LCHSkillData skill = new LCHSkillData();
        skill.id = id;
        skill.roleId = roidId;
        string json = JsonConvert.SerializeObject(skill);
        loader.SaveFile(id, json, SkillDataType.SKILL);
        SaveRole(roidId);

    }

    private void DeleteSkill(object[] args)
    {
        string skillId = (string)args[0];
        var skill = GetSkill(skillId);
        var role = GetRole(skill.roleId);
        role.DeleteSkill(skillId);
        loader.DeleveFile(skillId, SkillDataType.SKILL);
        skills.Remove(skillId);
        SaveRole(role.id);
    }

    Dictionary<string, LCHSkillData> skills = new Dictionary<string, LCHSkillData>();
    public LCHSkillData GetSkill(string skillId)
    {
        if (skills.ContainsKey(skillId))
            return skills[skillId];
        var js = loader.LoadFile(skillId, SkillDataType.SKILL);
        if (js.Length == 0)
            return null;
        var v = JSonHelper.DeserializeSkill(js);
        skills[skillId] = v;
        return v;
    }
    public void SaveSkill(string skillId)
    {
        try
        {
            var s = skills[skillId];
            if (null == s)
                return;
            string json = JsonConvert.SerializeObject(s);
            loader.SaveFile(skillId, json, SkillDataType.SKILL);
        }
        catch (System.Exception e)
        {
            
            Debug.LogError(e.ToString() + e.StackTrace);
        }
       
    }

    Dictionary<string, LCHRoleData> roles = new Dictionary<string, LCHRoleData>();
 
    public LCHRoleData GetRole(string id)
    {
        if (roles.ContainsKey(id))
            return roles[id];
        var js = loader.LoadFile(id, SkillDataType.ROLE);
        //var v = JSonHelper.DeserializeDictionary(js);
        LCHRoleData v = JSonHelper.DeserializeRole(js);
        if (v == null)
            return null;
        roles[id] = v;
        v.skills = JSonHelper.ToStringList(v.skills);
        ////roid["skills"];
        return v;
    }
    public void DeleteRole(string id)
    {
        loader.DeleveFile(id, SkillDataType.ROLE);
        roles.Remove(id);
        int type = (int)SkillDataType.ROLE;
        EventManager.CallEvent((int)SkillEvent.OnRoleListModify, id, type );
    }
    public void DeleteSkill(string  curSkillId)
    {
        LCHSkillData sk = GetSkill( curSkillId);
        LCHRoleData role = GetRole(sk.roleId);
        List<string> ids = new List<string>();
        for (int i = 0; i < role.skills.Length; i++)
        {
            var s = role.skills[i];
            if (s ==  curSkillId)
                continue;
            ids.Add(s);
        }
        role.skills = ids.ToArray();
        loader.DeleveFile( curSkillId, SkillDataType.SKILL);
        skills.Remove( curSkillId);
        SaveRole(sk.roleId);



    }
    public void SaveRole(string id )
    {
        var r = roles[id];
        if (r == null)
            return;
        string json = JsonConvert.SerializeObject(r);
        loader.SaveFile(id, json, SkillDataType.ROLE);
    }
    
    public void ReleaseRole(string id)
    {
        roles.Remove(id);
    }

    public List<string> GetAllRoles()
    {
        return this.loader.GetIds(SkillDataType.ROLE);
    }
}
