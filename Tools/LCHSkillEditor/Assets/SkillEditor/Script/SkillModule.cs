﻿using Newtonsoft.Json;
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
    AddChanneled,
    OnRoleListModify,
}
/*public enum SkillEvent
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
}*/
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
#if UNITY_EDITOR
 
    public void BindEvent()
    {
     
        /*EventManager.AddEvent((int)SkillEvent.CreateRole, CreateRole);
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
        EventManager.AddEvent((int)SkillEvent.SkillAddSound, SkillAddSound);*/
    }
#endif
    public void Init(SkillDataLoader loader)
    {
        this.loader = loader;

 
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
#if UNITY_EDITOR
    
    public void RemoveBind()
    {
        
        /*EventManager.RemoveEvent((int)SkillEvent.CreateRole, CreateRole);
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
        EventManager.RemoveEvent((int)SkillEvent.SkillAddSound, SkillAddSound);*/
    }
#endif
    public void Release()
    {
 
        loader = null;
        
    }

    

    public void RemoveObject(string skillId, int objId ,int index)
    {
    
        var skill = GetSkill(skillId);

        var o0 = skill.subSkills[index].GetObject(objId);
        skill.subSkills[index].RemoveObject(objId);
        if ( null != o0 && o0.type == 1)
        {
            for (int i = skill.subSkills[index].objs.Length-1; i >=0; i-- )
            {
                var o1 = skill.subSkills[index].objs[i];
                if (o1.type == 3)
                {
                    int bind = o1.propertys.GetValueInt("bind", 0);
                    if (bind == 2)
                    {
                        int objid = o1.propertys.GetValueInt("objid", 0);
                        if (objid == o0.id)
                        {
                            skill.subSkills[index].RemoveObject(o1.id);
                        }
                    }
                }

            }
        }
        SaveSkill(skillId);


    }

   
    private void RemoveEventChannel(string skillId, int curSelectChannel,int index)
    {
     
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 || curSelectChannel >= skill.subSkills[index].events.Count)
            return;

        skill.subSkills[index].events.RemoveAt(curSelectChannel);
        SaveSkill(skillId);

    }
    public void RemoveChannel(string skillId, int curSelectChannel,int index)
    {
 
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0  )
            return;
        if (curSelectChannel >= skill.subSkills[index].channels.Count)
        {
            RemoveEventChannel(skillId, curSelectChannel - skill.subSkills[index].channels.Count,index);
            return;
        }
            
        
        skill.subSkills[index].channels.RemoveAt(curSelectChannel);
        SaveSkill(skillId);

    }
    public void RemoveKeyFrame(string skillId, int curSelectChannel, int curSelectFrame,int index)
    {
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 )
            return;
        if (curSelectChannel >= skill.subSkills[index].channels.Count)
        {
            RemoveEventKeyFrame(skillId, curSelectChannel - skill.subSkills[index].channels.Count, curSelectFrame,index);
            return;
        }
        var channel = skill.subSkills[index].channels[curSelectChannel];
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

    public void CopyKeyFrame(string skillId, int curSelectChannel, int curSelectFrame,int index)
    {
        
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0)
            return;
        if (curSelectChannel >= skill.subSkills[index].channels.Count)
        {
            
            EventCopyKeyFrame(skillId, curSelectChannel - skill.subSkills[index].channels.Count, curSelectFrame,index);
            return;
        }
        var channel = skill.subSkills[index].channels[curSelectChannel];
        int count = channel.times.Count;
        for (int i = 0; i < count; i++)
        {
            var f = channel.times[i];
            if (f == curSelectFrame)
            {
                //找到一个可拷贝的位置
                int time = f + 10;
                int oi = i;
                i++;
                for (; i < count; i++)
                {
                     
                    if (channel.times[i] == time)
                    {
                        time = channel.times[i] + 10;
                    }
                    else
                    {
                        AddKeyFrame(index, skillId, curSelectChannel, time, channel.values[oi]);
                        return;
                    }
                }
                AddKeyFrame(index, skillId, curSelectChannel, time, channel.values[oi]);
                return;
            }
        }
 
    }
    

    private void EventCopyKeyFrame(string skillId, int curSelectChannel, int curSelectFrame,int index)
    {

        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 || curSelectChannel >= skill.subSkills[index].events.Count)
            return;
        var events = skill.subSkills[index].events[curSelectChannel];
        int count = events.times.Count;
        for (int i = 0; i < count; i++)
        {
            var f = events.times[i];
            if (f == curSelectFrame)
            {
                //找到一个可拷贝的位置
                int time = f + 10;
                int oi = i;
                i++;
                for (; i < count; i++)
                {

                    if (events.times[i] == time)
                    {
                        time = events.times[i] + 10;
                    }
                    else
                    {
           
                        AddEventKeyFrame(index,skillId, curSelectChannel, time, events.values[oi].Copy());
                        
                        return;
                    }
                }
                AddEventKeyFrame(index,skillId, curSelectChannel  , time, events.values[oi].Copy());
                //AddKeyFrame(skillId, curSelectChannel, time, channel.values[oi]);
                return;
         
            }
        }
        SaveSkill(skillId);
    }
    private void RemoveEventKeyFrame(string skillId, int curSelectChannel, int curSelectFrame,int index)
    {
     
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 || curSelectChannel >= skill.subSkills[index].events.Count)
            return;
        var events = skill.subSkills[index].events[curSelectChannel];
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

    public void AddKeyFrame(int index,string skillId, int curSelectChannel, int curSelectFrame ,float defaultVal = 0f)
    {
 
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 )
            return;
        if (curSelectChannel >= skill.subSkills[index].channels.Count)
        {
            AddEventKeyFrame(index, skillId, curSelectChannel - skill.subSkills[index].channels.Count, curSelectFrame);
            return;
        }
        var channel = skill.subSkills[index].channels[curSelectChannel];
        int count = channel.times.Count;
        for (int i = 0 ; i < count; i++)
        {
            var f = channel.times[i];
            if (f > curSelectFrame)
            {
                channel.AddKey(i, curSelectFrame, defaultVal);
                SaveSkill(skillId);
                return;
            }
            else if (f == curSelectFrame)
            {
                return;
            }
        }
        channel.AddKey(count, curSelectFrame, defaultVal);
        SaveSkill(skillId);
       
    }

    private void AddEventKeyFrame(int index,string skillId, int curSelectChannel, int curSelectFrame, ObjDictionary defaultVal = null)
    {
        var skill = GetSkill(skillId);
        if (curSelectChannel < 0 || curSelectChannel >= skill.subSkills[index].events.Count)
            return;
        var events = skill.subSkills[index].events[curSelectChannel];
        int count = events.times.Count;
        for (int i = 0; i < count; i++)
        {
            var f = events.times[i];
            if (f > curSelectFrame)
            {
                events.AddKey(i, curSelectFrame, defaultVal);
                SaveSkill(skillId);
                return;
            }
            else if (f == curSelectFrame)
            {

                return;
            }
        }
        events.AddKey(count, curSelectFrame, defaultVal);
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
 
    }

    bool HasChannel(LCHSkillData skill ,int objId, int type ,int index)
    {
        int len = skill.subSkills[index] .channels.Count;
        for (int i = 0; i < len; i++)
        {
            var c = skill.subSkills[index].channels[i];
            if (c.type == type&&c.objId == objId)
            {

                return false;
            }
        }
        return true;
    }

    bool HasEventChannel(LCHSkillData skill, int objId, LCHChannelType type,int index)
    {
        int len = skill.subSkills[index].events.Count;
        for (int i = 0; i < len; i++)
        {
            var c = skill.subSkills[index].events[i];
            if (c.type == (int)type && c.objId == objId)
            {
                return false;
            }
        }
        return true;
    }
    public void SkillEventChannel(string skillId, int objId, LCHChannelType type,int index)
    {
   
        LCHSkillData skill = GetSkill(skillId);
        if (!HasEventChannel(skill, objId, type, index))
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("提示", "相同轨迹已经存在", "确定");
#endif
            return;
        }
        LCHEventChannelData channel = new LCHEventChannelData();
        channel.type = (int)type;
        channel.objId = objId;
        skill.subSkills[index].events.Add(channel);
        SaveSkill(skill.id); 
    }
    public void SkillLerpFloatChannel(int index,string skillId, int objId, LCHChannelType type,bool messsage = true)
    {
        LCHSkillData skill = GetSkill(skillId);
        if (!HasChannel(skill,objId,(int)type,index) && messsage)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("提示", "相同轨迹已经存在", "确定");
#endif
            return;
        }

        LCHChannelData channel = new LCHChannelData();
        channel.type = (int)type;
        channel.objId = objId;
        if (type == LCHChannelType.ScaleX || type == LCHChannelType.ScaleY || type == LCHChannelType.ScaleZ)
        {
            channel.AddKey(0, 0,1f);
        }
        else
        {
            channel.AddKey(0, 0,0f);
        }
        skill.subSkills[index].channels.Add(channel);
        

        SaveSkill(skill.id); 

    }
    public void SkillAddSound(int index, string skillId)
    {
 
 
        LCHSkillData skill = GetSkill(skillId);
        var v = skill.subSkills[index].objs;
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
        skill.subSkills[index].objs = ArrayHelper.AddItem<LCHObjectData>(skill.subSkills[index].objs, eo);
        SaveSkill(skillId);
 
    }
    public void SkillAddBaseCollider(string skillId,int index, int objId, string bindName)
    {
 
        LCHSkillData skill = GetSkill(skillId);
        var v = skill.subSkills[index].objs;
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
        skill.subSkills[index].objs = ArrayHelper.AddItem<LCHObjectData>(skill.subSkills[index].objs, eo);
        SaveSkill(skillId);
 
    }
    public void SkillAddBoxCollider(string skillId,int index)
    {
      
        LCHSkillData skill = GetSkill(skillId);
        var v = skill.subSkills[index].objs;
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
        skill.subSkills[index].objs = ArrayHelper.AddItem<LCHObjectData>(skill.subSkills[index].objs, eo);
        SaveSkill(skillId);
 
    }
    public int SkillAddObject(string skillId,int index)
    {
         
        LCHSkillData skill = GetSkill(skillId);
        var v = skill.subSkills[index].objs;

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
        skill.subSkills[index].objs =  ArrayHelper.AddItem<LCHObjectData>(skill.subSkills[index].objs, eo);
        SaveSkill(skillId);
        return id;
    }

    

    public bool CreateSkill(params object[] arg)
    {
        string id = arg[0].ToString();
        string roidId = arg[1].ToString();

#if UNITY_EDITOR
        if (this.loader.Exists(id, SkillDataType.SKILL))
        {
            EditorUtility.DisplayDialog("提示", "同id技能已经存在", "确定");
            return false;
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
        return true;

    }

    public void DeleteSkill(params object[] args)
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
#if UNITY_EDITOR
        if (EditorModel && null == loader)
            loader = new EditorFileLoader();
#endif
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

            var s = skills[skillId];
            if (null == s)
                return;
            string json = JsonConvert.SerializeObject(s);
            loader.SaveFile(skillId, json, SkillDataType.SKILL);
 
        }
       
    }

    Dictionary<string, LCHRoleData> roles = new Dictionary<string, LCHRoleData>();
    public bool HasRole(string curRoleId)
    {
        if (!loader.Exists(curRoleId, SkillDataType.ROLE))
        {
            roles.Remove(curRoleId);
            return false;
        }
        return true;
    }

    public LCHRoleData GetRole(string id)
    {
        if (roles.ContainsKey(id))
            return roles[id];
        var js = loader.LoadFile(id, SkillDataType.ROLE);
        //var v = JSonHelper.DeserializeDictionary(js);
        LCHRoleData v = JSonHelper.DeserializeRole(js);
        Array.Sort<string>(v.skills);
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
