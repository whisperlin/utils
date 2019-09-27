using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PropertyHelper  {

     
    public static void DrawPropertys(Dictionary<string,object> propertys , Dictionary<string,object> temps, string[] keys ,Dictionary<string,object> _params,int objType = 0)
    {
        //string [] anims = null ,string [] sounds = null ,int []soundIds = null
        if (null == propertys)
            return;
        for (int i = 0, len = keys.Length; i < len; i++)
        {
            var name = keys[i];
            var r = temps[name];
            Dictionary<string, object> temp = (Dictionary<string, object>)r;

            //objType

            if (temp.ContainsKey("type_limit") )
            {
                int type_limit = System.Convert.ToInt32(temp["type_limit"]);
                if (objType != type_limit)
                    continue;
            }
            if (temp.ContainsKey("limit") && temp.ContainsKey("limit_items"))
            {
                string lkey = temp["limit"].ToString();
                if (!propertys.ContainsKey(lkey))
                {
                    propertys[lkey] = 0;
                }
                int lvalue = System.Convert.ToInt32(propertys[lkey]);

                int[] limits = (int[])temp["limit_items"];
                bool visible = false;
                for (int n = 0; n < limits.Length; n++)
                {
                    if (limits[n] == lvalue)
                    {
                        visible = true;
                        break;
                    }
                }
                if (!visible)
                {
                    continue;
                }
            }
            //新属性，默认初始化值。
            if (!propertys.ContainsKey(name))
            {
                propertys[name] = temp["defalut"];
            }
            //以后要优化速度改枚举。
            string type = (string)temp["type"];

            GUILayout.BeginHorizontal();
            DrawPropertyTips(temp);
            
            if (type == "int")
            {
                DrawPropertyInt(propertys, name);
            }
            else if (type == "float")
            {
                DrawPropertyFloat(propertys, name);
            }
            else if (type == "string")
            {
                DrawPropertyString(propertys, name);
            }
            else if (type == "list")
            {
                DrawPropertyList(propertys, name, (string [])temp["items"]);
            }
            else if (type == "bool")
            {
                DrawPropertyBool(propertys, name);
            }
            else if (type == "anim")
            {
                DrawPropertyPopList(propertys, name, (string [])_params["anims"]);
            }
            else if(type== "sound")
            {
                DrawPropertyPopList(propertys, name, (string[])_params["sounds"], (int [])_params["soundIds"]);
            }
            else if (type == "effect")
            {
                DrawPropertyPopList(propertys, name, (string[])_params["objectNames"], (int[])_params["objectid"]);
            }
 
            GUILayout.EndHorizontal();
        }
    }
    static void DrawPropertyTips(Dictionary<string, object> p)
    {
        GUILayout.Label(p["tip"].ToString(),GUILayout.Width(80f));
    }

    public static void DrawPropertyTips(string name)
    {
        GUILayout.Label(name, GUILayout.Width(80f));
    }


    static void DrawPropertyInt(Dictionary<string, object> p ,string key)
    {
         
            p[key] = EditorGUILayout.IntField(System.Convert.ToInt32(p[key]));
        
        
    }
    static void DrawPropertyFloat(Dictionary<string, object> p, string key)
    {
        try
        {
            var v = p[key];
            float f = System.Convert.ToSingle(v);
            p[key] = EditorGUILayout.FloatField(f);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString()+"\n"+e.StackTrace);
        }
        

    }
    static void DrawPropertyList(Dictionary<string, object> p, string key,string[] items)
    {
        var o = p[key];
        p[key] = EditorGUILayout.Popup(System.Convert.ToInt32(o), items) ;
    }
    static void DrawPropertyString(Dictionary<string, object>p, string key)
    {
        p[key] = EditorGUILayout.TextField( (string)p[key]);
    }

    static void DrawPropertyBool(Dictionary<string, object> p, string key)
    {
        p[key] = EditorGUILayout.Toggle(System.Convert.ToBoolean(p[key]));
    }


    
    static void DrawPropertyPopList(Dictionary<string, object> p, string key, string[] items)
    {
        string o = p[key].ToString() ;
        int index = 0;
        for (int i = 1, c = items.Length; i < c; i++)
        {
            if (items[i] == o)
            {
                index = i;
                break;
            }
        }
        index = EditorGUILayout.Popup(index, items);
        if (index == 0)
        {
            p[key] = "";
        }
        else
        {
            p[key] = items[index];
        }
       
    }


    static void DrawPropertyPopList(Dictionary<string, object> p, string key, string[] items,int [] ids )
    {
        var o = p[key];
        int id = 0;
        try
        {
            id = (int)o;
        }
        catch (System.Exception e0)
        {
            try
            {
                id = (int)((long)o);
            }
            catch (System.Exception e1)
            {
                id = -2;
            }
            
        }
        int index = 0;
        for (int i = 1, c = ids.Length; i < c; i++)
        {
            if (ids[i] == id)
            {
                index = i;
                break;
            }
        }
        index = EditorGUILayout.Popup(index, items);
        p[key] = ids[index];

    }

}
