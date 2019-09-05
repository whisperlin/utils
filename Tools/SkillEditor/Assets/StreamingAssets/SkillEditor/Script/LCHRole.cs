using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCHRoleData
{
    public string id = "";
    public Dictionary<string, object> propertys = new Dictionary<string, object>();
    public string mod = "";
    public string mod_name = "";
    public string [] skills  = new string[0];

    internal void DeleteSkill(string skillId)
    {
        skills = ArrayHelper.DeleteItem(skills, skillId);
    }
}
