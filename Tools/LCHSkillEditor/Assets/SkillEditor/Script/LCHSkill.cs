using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public interface SkillResourceLoader
{
    GameObject LoadModul(int type,string name, string path);
    void ReleaseModel(int type,string name, string path,GameObject g);
}
public class ObjectContain
{
    public string mod = null;
    public string mod_name = null;
    public int objId;
    public int type;
    public int bindObjId;
    public int bind;
    public string bindName;
    public Vector3 pos = Vector3.zero;
    public Quaternion rot = Quaternion.identity;
    public Vector3 scale = Vector3.one;
    public GameObject gameobject;
    public Animation animaction;
    public Collider collider;
    public List<LCHChannelData> channels = new List<LCHChannelData>();//这里搞不了多态，因为序列化json不支持多态。
    public List<LCHEventChannelData> events = new List<LCHEventChannelData>();
    public Renderer[] roleRenders;
    public ParticleSystem[] systems;
    public Animation[] anims;
    public LCharacterColliderData hitData;
    internal GameObject baseGameObject;
    public bool isFlyTool = false;
    public bool stopUpatePos = false;

    public void SetInformation(LCHObjectData od)
    {
        objId = od.id;
        type = od.type;
        bind = od.propertys.GetValueInt("bind",0);
        if(bind==2)
            bindName = od.propertys.GetValue<string>("bind_name", "");
        bindObjId = od.propertys.GetValueInt("objid",-1);
        mod = od.propertys.GetValue<string>("mod", "");
        mod_name = od.propertys.GetValue<string>("mod_name", "");
    }

    internal void ResetTransformData()
    {
        pos = Vector3.zero;
        rot = Quaternion.identity;
        scale = Vector3.one;
    }
}
public class LCHSkillSubData
{
    public List<ObjectContain> objs = new List<ObjectContain>();
}
public class LCHSkill  {

    SkillResourceLoader loader;
    public  LCHSkill(   SkillResourceLoader loader, LCHRoleData r, LCHSkillData s)
    {
        this.loader = loader;
        roleData = r;
        _skillData = s;
    }
    private LCHRoleData roleData;
    private LCHSkillData _skillData;
    public ObjectContain role = new ObjectContain();
    public LCHSkillSubData[] subDatas = new LCHSkillSubData[1] { new LCHSkillSubData() };


    public LCHRoleData RoleData
    {
        get
        {
            return roleData;
        }
    }
    public LCHSkillData skillData
    {
        get
        {
            return _skillData;
        }
 
    }
#if UNITY_EDITOR
    //编辑器才会做这步
    public void ReleaseRoleModule()
    {
        if(null !=role)
            GameObject.DestroyImmediate(role.gameobject);
    }
    public static Transform FindInChildrenIncludingInactive(Transform t, string name)
    {

        for (int i = 0; i < t.childCount; i++)
        {
            Transform t2 = t.GetChild(i);
            //Debug.Log(t2.gameObject.name + "= " + name + " " + (name == t2.gameObject.name));
            if (t2.gameObject.name == name)
                return t2;
            Transform found = FindInChildrenIncludingInactive(t.GetChild(i), name);
            if (found != null)
                return found;
        }

        return null;  //couldn't find crap
    }
    public void SetObjectParent(int index)
    {
        int c0 = skillData.subSkills[index].objs.Length;
        for (int i = 0; i < c0; i++)
        {
            var o = skillData.subSkills[index].objs[i];
            if (o.type == 1 || o.type == 2)
            {
                int bind = o.propertys.GetValueInt("bind", 0);
                if (bind == 2)
                {
                    string bind_name = o.propertys.GetValue<string>("bind_name", "");
                    var ct = GetObjectContainById(o.id,index);
                    if (ct.gameobject)
                    {
                        if (role.gameobject && ct.gameobject)
                        {
                            var t = FindInChildrenIncludingInactive(role.gameobject.transform, bind_name);
                            if(ct.gameobject.transform.parent != t )
                            {
                                ct.gameobject.transform.parent = t;
                                t.transform.localPosition = Vector3.zero;
                            }
                           
                        }
                    }
                }
                else if (bind == 1)
                {
                    var ct = GetObjectContainById(o.id,index);
                    if (role.gameobject && ct.gameobject)
                    {
                        ct.gameobject.transform.parent = role.gameobject.transform;
                    }
                }
                else if (bind == 0)
                {
                    var ct = GetObjectContainById(o.id,index);
                    if (null != ct.gameobject)
                        ct.gameobject.transform.parent = null;
                }
            }
     
                
            
        }
    }
#endif
    public void Release()
    {
        for (int index = 0; index < skillData.subSkills.Length; index++)
        {
            int c0 = skillData.subSkills[index].objs.Length;
#if UNITY_EDITOR
            int c1 = subDatas[index].objs.Count;
            if (c0 == c1)
            {
#endif
                for (int i = 0; i < c0; i++)
                {
                    var o = skillData.subSkills[index].objs[i];
                    if (null != subDatas[index].objs[i])
                        loader.ReleaseModel(o.type, o.propertys.GetValue<string>("mod_name", ""), o.propertys.GetValue<string>("mod", ""), subDatas[index].objs[i].gameobject);
                }
#if UNITY_EDITOR
            }
            else //编辑器才会出现.
            {
                for (int i = 0; i < c1; i++)
                {
                    var o = subDatas[index].objs[i];
                    if (null != subDatas[index].objs[i] && null != subDatas[index].objs[i].gameobject)
                    {
                        var ct = subDatas[index].objs[i];
                        loader.ReleaseModel(ct.type, ct.mod_name, ct.mod, o.gameobject);
                    }

                }
            }
#endif
            subDatas[index].objs.Clear();
        }
       
    }
    Dictionary<int,ObjectContain> objContains = new Dictionary<int,ObjectContain>();
    public ObjectContain GetObjectContainById(int objId ,int index)
    {
        if (objId == -1)
        {
            return role;
        }
        ObjectContain c;
        if (objContains.TryGetValue(  objId ,out c))
        {
            return c;
        }
        
        for (int i = 0, c0 = subDatas[index].objs.Count; i < c0; i++)
        {
            var o0 = subDatas[index]. objs[i];
            if (objId == o0.objId)
            {
                objContains[objId] = o0;
                return o0;
            }
        }
        return null;
    }
    public void ComputeAnim(float curTime,int index)
    {
#if UNITY_EDITOR
        SetObjectParent( index);
#endif
        //初始化.
        role.ResetTransformData();
        for (int i = 0, c0 = subDatas[index].objs.Count; i < c0; i++)
        {
            var o0 = subDatas[index].objs[i];
            o0.ResetTransformData();
        }
        //计算位置变换.
        for (int i = 0, c0 = _skillData.subSkills[index].channels.Count; i < c0; i++)
        {
            var channel = _skillData.subSkills[index].channels[i];
            var contain = GetObjectContainById( channel.objId,index);
            LCHChannelType t = (LCHChannelType)channel.type;
#if UNITY_EDITOR
            if (channel.times.Count == 0 || channel.times[0] != 0)
            {
                channel.AddKey(0, 0);
                if (t == LCHChannelType.PosX || t == LCHChannelType.PosY || t == LCHChannelType.PosZ)
                {
                    channel.values[0] = 0f;
                }
                else if (t == LCHChannelType.ScaleX || t == LCHChannelType.ScaleY || t == LCHChannelType.ScaleZ)
                {
                    channel.values[0] = 1f;
                }
                else if (t == LCHChannelType.RotY)
                {
                    channel.values[0] = 0f;
                }
            }
#endif

            switch (t)
            {
                case LCHChannelType.PosX:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.pos += new Vector3(v, 0f, 0f);
                        break;
                    }
                case LCHChannelType.PosY:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.pos += new Vector3(0f, v, 0f);
                        break;
                    }
                case LCHChannelType.PosZ:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.pos += new Vector3(0f, 0f, v);
                        break;
                    }

                case LCHChannelType.RotY:
                    {

                        contain.rot = channel.GetSlerpYValue(curTime);
                        break;
                    }

                case LCHChannelType.ScaleX:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.scale = new Vector3(v, contain.scale.y, contain.scale.z);
                        break;
                    }
                case LCHChannelType.ScaleY:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.scale = new Vector3(contain.scale.x, v, contain.scale.z);
                        break;
                    }
                case LCHChannelType.ScaleZ:
                    {
                        float v = channel.GetLerpValue(curTime);
                        contain.scale = new Vector3(contain.scale.x, contain.scale.y, v);
                        break;
                    }
            }
        }

        //赋值。
        {
            if (null != role.gameobject)
            {
                role.gameobject.transform.localPosition = role.pos;
                role.gameobject.transform.localRotation = role.rot;
                role.gameobject.transform.localScale = role.scale;
            }

        }
        
        for (int i = 0, c0 = subDatas[index].objs.Count; i < c0; i++)
        {
            var o0 = subDatas[index].objs[i];
            if (null == o0.gameobject)
                continue;
            o0.gameobject.transform.localPosition = o0.pos;
            o0.gameobject.transform.localRotation = o0.rot;
            o0.gameobject.transform.localScale = o0.scale;
        }

        //计算物体事件，比如播放动作，声音等。
        for (int i = 0, c0 = _skillData.subSkills[index].events.Count; i < c0; i++)
        {
            var _e = _skillData.subSkills[index].events[i];
            var contain = GetObjectContainById(_e.objId,index);
            LCHChannelType t = (LCHChannelType)_e.type;
            ObjDictionary value;
            float _time;
            if (t == LCHChannelType.Object)
            {
                if (_e.TryGetKeyFrame(curTime, out value, out _time))
                {
                    if (null != contain.gameobject)
                    {
                        bool enable = value.GetValue<bool>("enable", true);
                        contain.gameobject.SetActive(enable);
                        if (enable)
                        {
                            string anim_name = value.GetValue<string>("anim", "");
                            if (anim_name.Length > 0)
                            {
                                if (null == contain.animaction)
                                {
                                    contain.animaction = contain.gameobject.GetComponent<Animation>();
                                }
                                if (null != contain.animaction)
                                {
                                    contain.animaction[anim_name].time = _time;
                                    //animation[anim_name].speed = 0.0;
                                    contain.animaction.Play(anim_name);
                                    contain.animaction.Sample();
                                    contain.animaction.Stop();
                                }
                            }
                            if(_e.objId != -1)
                                ParticleSystemHelper.Simulate(contain.gameobject, _time);
                        }
                    }

                }
                else
                {
                    if (null != contain.gameobject)
                    {
                        if(contain.objId!=-1)

                            contain.gameobject.SetActive(false);
                    }
                }
            }
            if (t == LCHChannelType.Event)
            {
                if (_e.TryGetKeyFrame(curTime, out value, out _time))
                {
                    if (null != contain.gameobject)
                    {

                        bool enable = value.GetValue<bool>("enable", true);
                        contain.gameobject.SetActive(enable);

                    }
                }
                else
                {
                    if (null != contain.gameobject)
                    {
                        contain.gameobject.SetActive(false);
                    }
                }
            }

        }

    }

    GameObject FindColliderInChild(GameObject g,string name)
    {
        if (g != null)
        {
            Collider[] cs = g.transform.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < cs.Length; i++)
            {
                if (cs[i].gameObject.name == name)
                    return cs[i].gameObject;
            }
        }
        return null;
    }
#if UNITY_EDITOR

    public void CheckModulUpdate(int index)
    {
        CheckInstance();
        CheckModulUpdate( role,-1,-1, roleData.mod_name, roleData.mod,null);
        //释放已经删除的。
        for (int i = 0, c = subDatas[index]. objs.Count; i < c; i++)
        {
            var ct = subDatas[index].objs[i];
            if (null != ct)
            {
                for (int j = 0, c2 = _skillData.subSkills[index].objs.Length; j < c2; j++)
                {
                    if (_skillData.subSkills[index].objs[j].id == ct.objId)
                        goto ct;
                }
                loader.ReleaseModel(ct.objId, ct.mod_name, ct.mod, ct.gameobject);
            ct:;
            }
        }
        //编辑器添加删除了物体。objContains是用来提速的。
        if (_skillData.subSkills[index].objs.Length != subDatas[index]. objs.Count)
        {
            objContains.Clear();
        }
        ArrayHelper.ResizeArray<ObjectContain>(ref subDatas[index].objs, _skillData.subSkills[index] .objs.Length);
        for (int i = 0, c = _skillData.subSkills[index].objs.Length; i < c; i++)
        {
            var o = _skillData.subSkills[index].objs[i];
            string name = o.propertys.GetValue<string>("bind_name", "");
            var o1 = subDatas[index].objs[i];
            if (o.type == 3)
            {
                o1.objId = o.id ;
                o1.type = o.type;
               
                o1.gameobject = FindColliderInChild(role.gameobject, name);
            }
            else
            {
                CheckModulUpdate(o1, o.id, o.type, o.propertys.GetValue<string>("mod_name", ""), o.propertys.GetValue<string>("mod", ""), o.name);
            }
            
             
        }
    }
#if UNITY_EDITOR
    string[]  nullAnims = new string[1] {"无" };
    public void GetAllObjectList(int objId , ref string [] items , ref int [] ids ,ref string [] objects ,ref int []objIds,int index)
    {
        List<string> obj_items = new List<string>();
        List<int> obj_ids = new List<int>();

        List<string> _items = new List<string>();
        List<int> _ids = new List<int>();
        _items.Add("无");
        _ids.Add(-2);
        obj_items.Add("无");
        obj_ids.Add(-2);
        for (int i = 0; i < _skillData.subSkills[index] .objs.Length; i++)
        {
            var od = _skillData.subSkills[index].objs[i];
            if (od.type == 4)
            {
                _items.Add("(" + od.id + ")" + od.name);
                _ids.Add(od.id);
            }
            else if (od.type == 1)
            {
                obj_items.Add("(" + od.id + ")" + od.name);
                obj_ids.Add(od.id);
            }
        }
        items = _items.ToArray();
        ids = _ids.ToArray();
        objects = obj_items.ToArray();
        objIds = obj_ids.ToArray();
    }
    public string[]  GetAnimList(int objId,int index)
    {
        if (objId == -1)
        {
            if (null != role)
            {
                Animation ani = role.gameobject.GetComponent<Animation>();
                List<string> anims = new List<string>();
                anims.Add("无");
                if (null != ani)
                {
                    foreach (AnimationState state in ani)
                    {
                        anims.Add(state.name);
                    }
                }
                return anims.ToArray();
            }
            else
            {
                for (int j = 0, c2 = subDatas[index]. objs.Count; j < c2; j++)
                {
                    if (subDatas[index].objs[j].objId == objId)
                    {
                        if (null != subDatas[index].objs[j].gameobject)
                        {
                            Animation ani = subDatas[index]. objs[j].gameobject.GetComponent<Animation>();
                            List<string> anims = new List<string>();
                            anims.Add("无");
                            if (null != ani)
                            {
                                foreach (AnimationState state in ani)
                                {
                                    anims.Add(state.name);
                                }
                            }
                            return anims.ToArray();
                        }
                        
                    }
                }
            }
        }
        return nullAnims; ;
    }
#endif
    public void CheckModulUpdate( ObjectContain o,int id,int type, string mod_name,string mod,string name)
    {
        bool reload = false;
        if (o == null)
        {
            o = new ObjectContain();
            reload = true;
        }
        else if (o.gameobject == null)
        {
            reload = true;
        }
        else
        {
            if (o.mod != mod)
            {
                loader.ReleaseModel(-1, mod_name, mod, o.gameobject);
                reload = true;
            }
        }
        if (reload)
        {
            o.objId = id;
            o.type = type;
            o.mod = mod;
            o.mod_name = mod_name;
            o.gameobject = loader.LoadModul(type, mod_name, mod);
            if (null != o.gameobject)
            {
                if (null != name)
                    o.gameobject.name = name;
            }
        }
    }
#endif

    public void LoadModul()//非编辑器加载一次.
    {
        Release();
        
        role.gameobject = loader.LoadModul(-1, roleData.mod_name, roleData.mod);

        for (int index = 0; index < skillData.subSkills.Length; index++)
        {
            for (int i = 0, c = skillData.subSkills[index].objs.Length; i < c; i++)
            {
                if (subDatas[index]. objs[i] == null)
                    subDatas[index]. objs[i] = new ObjectContain();
                var o2 = subDatas[index].objs[i];
                var o = skillData.subSkills[index].objs[i];


                string name = o.propertys.GetValue<string>("bind_name", "");
                var o1 = subDatas[index]. objs[i];
                if (o.type == 3)
                {
                    o1.objId = o.id;
                    o1.type = o.type;

                    o1.gameobject = FindColliderInChild(role.gameobject, name);
                }
                else
                {
                    o2.gameobject = loader.LoadModul(o.type, o.propertys.GetValue<string>("mod_name", ""), o.propertys.GetValue<string>("mod", ""));
                    o2.objId = o.id;
                    o2.type = o.type;
                    o2.mod = o.propertys.GetValue<string>("mod", "");
                    o2.mod_name = o.propertys.GetValue<string>("mod_name", "");
                }
            }
        }
        
    }

    public void CheckInstance()
    {
        if (subDatas.Length < skillData.subSkills.Length)
        {
            LCHSkillSubData[] newAry = new LCHSkillSubData[skillData.subSkills.Length];
            for (int n = 0; n < skillData.subSkills.Length; n++)
            {
                if (n < subDatas.Length)
                {
                    newAry[n] = subDatas[n];
                }
                else
                {
                    newAry[n] = new LCHSkillSubData();
                }
            }
            subDatas = newAry;
        }
        
    }
}
