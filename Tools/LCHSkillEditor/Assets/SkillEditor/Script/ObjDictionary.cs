using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDictionary : Dictionary<string,object>
{

    public ObjDictionary Copy()
    {
        ObjDictionary d = new ObjDictionary();
        foreach (var p in this)
        {
            d[p.Key] = p.Value;
        }
        return d;
    }

    public T GetValue<T>(string key,T defaultValue) {
        try
        {
            object v = this[key];
            return (T)v;
        }
        catch(System.Exception e)
        {
            //object v = this[key];
            //Debug.Log(v.GetType());
            this[key] = defaultValue;
            return (T)this[key];

        }
        
    }
    public float GetValueFloat(string key, float defaultValue)
    {
        try
        {
            return (float)this[key];
        }
        catch (System.Exception e)
        {
            if (this.ContainsKey(key))
            {
                float i = System.Convert.ToSingle(this[key]);
                this[key] = i;
                return i;
            }
            else
            {
                this[key] = defaultValue;
            }

            return defaultValue;

        }

    }
    public int GetValueInt(string key, int defaultValue)
    {
        try
        {
            return  (int)this[key];
        }
        catch (System.Exception e)
        {
            if (this.ContainsKey(key))
            {
                int i  = System.Convert.ToInt32(this[key]);
                this[key] = i;
                return i;
            }
            else
            {
                this[key] = defaultValue;
            }
            
            return defaultValue;

        }

    }
}
