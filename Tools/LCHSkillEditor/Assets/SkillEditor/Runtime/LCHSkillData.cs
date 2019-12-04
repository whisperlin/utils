using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCHObjectData
{
    public int id = 0;
    public string name = "";

    public int type = 1;//1物体，2触发器 ,3,原生触发器 4，音效
    public ObjDictionary propertys = new ObjDictionary();

 
}
//jsong 序列号无法支持多态,
public  class LCHChannelData 
{
    public int type = 0;
    public int objId = -1;
    public List<int> times = new List<int>();
    public List<float> values = new List<float>();



    public Quaternion GetSlerpYValue(float time)
    {
        time = time * 100f;
        int ct0 = times.Count;
        for (int j = 0; j < ct0; j++)
        {
            int t = times[j];
            if (time == t)
            {
                return Quaternion.Euler(0f, values[j], 0f);
            }
            else if (t > time)
            {
                if (j == 0)
                    return Quaternion.Euler(0f, 0f, 0f);
                else
                {
                    float t0 = time - times[j - 1];
                    float t1 = t - times[j - 1];
                    return Quaternion.SlerpUnclamped(Quaternion.Euler(0f, values[j - 1], 0f), Quaternion.Euler(0f, values[j], 0f), t0 / t1);
                }
            }
        }
        if (ct0 > 0)
            return Quaternion.Euler(0f, values[ct0 - 1], 0f);
        return Quaternion.identity;
    }
    public static bool debug = false;
    public float GetLerpValue(float time)
    {
        time = time * 100f;
        int ct0 = times.Count;
        for (int j = 0 ; j < ct0; j++)
        {
            int t = times[j];
            if (time == t)
            {
                return values[j];
            }
            else if (t > time)
            {
                if (j == 0)
                    return  values[0];
                else
                {
                    float t0 = time - times[j - 1];
                    float t1 = t - times[j - 1];
                    float v = Mathf.Lerp(values[j - 1], values[j], t0 / t1); 
                    return v;
                }
            }
        }
        if (ct0 > 0)
        {
            var v = values[ct0 - 1];
            return v;
        }
           
        return 0;
    }
    public bool isLerpChannel()
    {
        return type < 100;
    }
    public int GetKeyframeIndex(int frame)
    {
        for (int i = 0, c = times.Count; i < c; i++)
        {
            if (times[i] == frame)
                return i;
        }
        return -1;
    }
    public void AddKey(int index, int frame,float value = 0f)
    {
        times.Insert(index, frame);
        values.Insert(index, value);
    }
    public void DeleteFrame(int index)
    {
        times.RemoveAt(index);
        values.RemoveAt(index);
    }
    public string GetTypeName()
    {
        LCHChannelType t = (LCHChannelType)type;

        switch (t)
        {
            case LCHChannelType.PosX:
                return " 位移X";
            case LCHChannelType.PosY:
                return " 位移Y";
            case LCHChannelType.PosZ:
                return " 位移Z";
    
            case LCHChannelType.RotY:
                return " 旋转Y";

            case LCHChannelType.ScaleX:
                return " 缩放X";
            case LCHChannelType.ScaleY:
                return " 缩放Y";
            case LCHChannelType.ScaleZ:
                return " 缩放Z";
            case LCHChannelType.Object:
                return " 显示/隐藏";
            case LCHChannelType.Event:
                return " 事件";
        }
        return " 不存在";
    }

    public bool AddKeyFrame(int frame, float value)
    {
        int l = times.Count;
        for (int i = 0; i < l; i++)
        {
            var f = times[i];
            if (f > frame)
            {
                times.Insert(i, frame);
                values.Insert(i, value);
                return true;
            }
            else if (f == frame)
            {
                return false;
            }
        }
        times.Add(frame);
        values.Add(value);
        return true;
    }
}

public class LCHHitEventData
{
    public int hp = 0;
    public int mp = 0;
    public int type = 0; //0"无",1"击飞",2"眩晕",3"击退",4"浮空" 
    public float ctrlType = 1f ;
    public float hitback = 0f;
    public float upspeed = 0f;
    public float backspeed = 0f;
}
public class LCHEventChannelData
{
    public int type = 0;
    public int objId = -1;
    public List<int> times = new List<int>();
    public List<ObjDictionary> values = new List<ObjDictionary>();
    public bool isLerpChannel()
    {
        return false;
    }
    public ObjDictionary GetKeyFrame(int frame)
    {
        for (int i = 0, c = times.Count; i < c; i++)
        {
            if (times[i] == frame)
            {
                return values[i];
            }
        }
        return null;
    }
    public void DeleteKeyFrame(ObjDictionary ObjDictionary)
    {
        for (int i = 0, c = times.Count; i < c; i++)
        {
            if (values[i] == ObjDictionary)
            {
                times.RemoveAt(i);
                values.RemoveAt(i);
                return;
            }
        }
    }
    public bool AddKeyFrame(int frame, ObjDictionary value)
    {
        int l = times.Count;
        for (int i = 0; i < l; i++)
        {
            var f = times[i];
            if (f > frame)
            {
                times.Insert(i, frame);
                values.Insert(i, value);
                return true;
            }
            else if (f == frame)
            {
                return false;
            }
        }
        times.Add( frame);
        values.Add(value);
        return true;
    }
    public void AddKey(int index, int frame, ObjDictionary defaultVal = null )
    {
        times.Insert(index, frame);
        if (null == defaultVal)
        {
            values.Insert(index, new ObjDictionary());
        }
        else
        {
            values.Insert(index, defaultVal);
        }
        
    }
    public void DeleteFrameByIndex(int index)
    {
        times.RemoveAt(index);
        values.RemoveAt(index);
    }
    public string GetTypeName()
    {
        LCHChannelType t = (LCHChannelType)type;
        switch (t)
        {
            case LCHChannelType.PosX:
                return " 位移X";
            case LCHChannelType.PosY:
                return " 位移Y";
            case LCHChannelType.PosZ:
                return " 位移Z";

            case LCHChannelType.RotY:
                return " 旋转Y";

            case LCHChannelType.ScaleX:
                return " 缩放X";
            case LCHChannelType.ScaleY:
                return " 缩放Y";
            case LCHChannelType.ScaleZ:
                return " 缩放Z";
            case LCHChannelType.Object:
                return " 显示/隐藏";
            case LCHChannelType.Event:
                return " 事件";
        }
        return " 不存在";
    }
    //返回值1，找到,0未设置过,1,已经设置过。
    public int TryGetKeyFrameRunTime(float _curFrame,float lastTime, out ObjDictionary value, out float time,out float keyFrameTime)
    {
        float curFrame = _curFrame * 100f;
        lastTime = lastTime * 100f;
        int ct0 = times.Count;
        int returnType = 0;
        for (int i = 0, c = ct0; i < c; i++)
        {
            int t = times[i];
            if (t >= lastTime && t <= curFrame)
            {
                keyFrameTime = t * 0.01f;
                time = _curFrame-keyFrameTime;
                value = values[i];
               
                return 1;
            }
            else if (curFrame > t)
            {

                returnType = 2;
            }

        }
        time = 0f;
        keyFrameTime = 0f;
        value = null;
        return returnType;
    }
    public bool TryGetKeyFrame(float curFrame, out ObjDictionary value, out float time)
    {
        curFrame = curFrame * 100f;
        int ct0 = times.Count;
        for (int i = 0, c = ct0; i < c; i++)
        {
            int t = times[i];
            if (curFrame == t)
            {
                time = 0f;
                value = values[i];
                return true;
            }
            else if (t > curFrame)
            {
                if (i == 0)
                {
                    time = 0f;
                    value = null;
                    return false;
                }
                else
                {
                    time = (curFrame - times[i-1]) * 0.01f;
                    value = values[i-1];
                    return true;
                }
            }
        }
        if (ct0 > 0 && times[ct0 - 1]< curFrame)
        {
            value = values[ct0-1];
            time = (curFrame - times[ct0-1]) * 0.01f;
            return true;
        }
 ;
        time = 0f;
        value = null;
        return false;
    }

    
}


public class LCHSkillData
{
#if UNITY_EDITOR
    static int innerId = 0;
    int instanceId = 0;
    public LCHSkillData()
    {
        innerId++;
        instanceId = innerId;
    }
#endif

    public string id = "";
    public string roleId = "";
    public float maxLength = 3f;
    public LCHObjectData[] objs = new LCHObjectData[0];
    public List<LCHChannelData> channels = new List<LCHChannelData>();//这里搞不了多态，因为序列化json不支持多态。
    public List<LCHEventChannelData> events = new List<LCHEventChannelData>();
    public float skillRange = 1f;//技能攻击范围。

    public float skillWidth = 1f;//技能宽度。
    public string CdName = "";
    public int skillIndex = -1;
    public SkillParams.TYPE type = SkillParams.TYPE.CLICK;

    public void GetidsAndName(ref string[] items0, ref int[] ids)
    {
        int objLen = objs.Length;
        if (items0.Length != objLen + 1)
        {
            items0 = new string[objLen + 1];
            ids = new int[objLen + 1];
        }
        items0[0] = "(-1)角色";
        ids[0] = -1;
        for (int i = 0; i < objs.Length; i++)
        {
            var _o = objs[i];
            if (_o.type == 1)
            {
                items0[i + 1] = "(" + _o.id + ")" + _o.name + " 对象";
            }
            else
            {
                items0[i + 1] = "(" + _o.id + ")" + _o.name + " 触发器";
            }
            ids[i + 1] = _o.id;
        }
    }
    public string GetObjectName(int _id)
    {
        if (_id == -1)
            return "角色";
        for (int i = 0; i < objs.Length; i++)
        {
            if (_id == objs[i].id)
                return objs[i].name;
        }
        return "不存在！！！";
    }
    public void RemoveObject(int _id)
    {
        for (int i = 0; i < channels.Count;i++ )
        {
            var c = channels[i];
            if (c.objId == _id)
            {
                channels.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < events.Count; i++)
        {
            var c = events[i];
            if (c.objId == _id)
            {
                events.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < objs.Length; i++)
        {
            if (_id == objs[i].id)
            {
                var _obj = objs[i];
                objs = ArrayHelper.DeleteItemAt<LCHObjectData>(objs, i);
 
                break;
            }
        }

    }

    public int GetObjectType(int _id)
    {
        if (_id == -1)
            return 0;
        for (int i = 0; i < objs.Length; i++)
        {
            if (_id == objs[i].id)
                return (int)objs[i].type;
        }
        return 0;
    }

    public LCHObjectData GetObject(int _id)
    {
        if (_id == -1)
            return null;
        for (int i = 0; i < objs.Length; i++)
        {
            if (_id == objs[i].id)
                return objs[i];
        }
        return null;
    }
}
