using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSonHelper  {

    static object ForObjectArray<T>(object[] ary)
    {
        for (int i = 0; i < ary.Length; i++)
        {
            if (! ( ary[i] is T ))
            {
                return ary;
            }
        }
        T[] arys = new T[ary.Length];
        for (int i = 0; i < ary.Length; i++)
        {
            arys[i] = (T)ary[i];
        }
        return arys;
    }
    static object ForObjectArrayInt64(object[] ary)
    {
        for (int i = 0; i < ary.Length; i++)
        {
            if (!(ary[i] is Int64))
            {
                return ary;
            }
        }
        int [] arys = new int[ary.Length];
        for (int i = 0; i < ary.Length; i++)
        {
            arys[i] = System.Convert.ToInt32(ary[i]);
        }
        return arys;
    }
    static object ForObjectArray(object[] ary)
    {
        if (ary != null && ary.Length > 0)
        {
            var v0 = ary[0];
            if (v0 is string)
            {
                return ForObjectArray<string>(ary);
            }
            else if (v0 is float)
            {
                return ForObjectArray<float>(ary);
            }
            else if (v0 is Int32)
            {
                return ForObjectArray<Int32>(ary);
            }
            else if (v0 is Int64)
            {
                return ForObjectArrayInt64(ary);
            }
        }
        return ary;
    }
    static object DeserializeJToken(JToken t)
    {
        if (t.Type == JTokenType.String)
        {
            return t.ToString();
        }
        else if (t.Type == JTokenType.Integer)
        {
            return (int)t;
        }
        else if (t.Type == JTokenType.Float)
        {
            return (float)t;
        }
        else if (t.Type == JTokenType.Boolean)
        {
            return (bool)t;
        }
        else if (t.Type == JTokenType.Object)
        {
            return DeserializeDictionary((JObject)t);
        }
        else if (t.Type == JTokenType.Array)
        {
            JArray ja = (JArray)t;
            int c = ja.Count;
            object[] ary = new object[c];
            for (int i = 0; i < c; i++)
            {
                JToken a = ja[i];
                ary[i] = DeserializeJToken(a);
            }
            return ForObjectArray(ary);
        }
        return null;
    }
    static Dictionary<string, object> DeserializeDictionary(JObject items)
    {
        if (items == null)
            Debug.LogError("error");
        Dictionary<string, object> map = new Dictionary<string, object>();
        foreach (var item in items)
        {
            JToken v = item.Value;
            map[item.Key.ToString()] = DeserializeJToken(v);
        }
        return map;
    }

    public static LCHRoleData DeserializeRole(string str)
    {
        LCHRoleData _map = JsonConvert.DeserializeObject<LCHRoleData>(str);
        return _map;
    }
    public static LCHSkillData DeserializeSkill(string str)
    {
        LCHSkillData _map = JsonConvert.DeserializeObject<LCHSkillData>(str);
        for (int i = 0, c = _map.objs.Length; i < c; i++)
        {
            var o = _map.objs[i];
        }
        return _map;
    }
    
    public static Dictionary<string, object> DeserializeDictionary(string str)
    {
        JObject _map = JsonConvert.DeserializeObject<JObject>(str);
        return DeserializeDictionary(_map);
    }

    public static string[] ToStringList(object o)
    {
        object[] objs = (object[])o;
        int c = objs.Length;
        string[] res = new string[c];
        for (int i = 0; i < c; i++)
        {
            res[i] = objs[i].ToString() ;
        }
        return res;
    }
    

    public static object[] AddItem(object[] list, object id)
    {
        int c = list.Length;
        object[] res = new object[c + 1];
        for (int i = 0; i < c; i++)
        {
            res[i] = list[i];
        }
        res[c] = id;
        return res;
    }

    


}
