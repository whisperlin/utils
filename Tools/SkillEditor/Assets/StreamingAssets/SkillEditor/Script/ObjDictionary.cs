using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDictionary : Dictionary<string,object>
{

    public T GetValue<T>(string key,T defaultValue) {
        try
        {
            return (T)this[key];
        }
        catch(System.Exception e)
        {
            this[key] = defaultValue;
            return (T)this[key];

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
