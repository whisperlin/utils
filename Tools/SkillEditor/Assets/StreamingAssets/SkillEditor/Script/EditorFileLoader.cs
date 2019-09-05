using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorFileLoader : SkillDataLoader
{
    static string root = "Assets\\SkillEditor\\Data\\";
    public string LoadFile(string id, SkillDataType type)
    {
        string path = "";
        if (type == SkillDataType.ROLE)
        {
            path = root + id + ".role";
        }
        else if (type == SkillDataType.SKILL)
        {
            path = root + id + ".skill";
        }
        if (System.IO.File.Exists(path))
        {
            return System.IO.File.ReadAllText(path);
        }
        return "";

    }

    public void SaveFile(string id, string text,SkillDataType type)
    {
        string path = "";
        if (type == SkillDataType.ROLE)
        {
            path = root + id + ".role";
            System.IO.File.WriteAllText(path, text);
        }
        else if (type == SkillDataType.SKILL)
        {
            path = root + id + ".skill";
            System.IO.File.WriteAllText(path, text);
        }
    }
    public List<string> GetIds(SkillDataType type)
    {
        List<string> res = new List<string>();
        System.IO.DirectoryInfo TheFolder = new System.IO.DirectoryInfo(root);
        foreach (System.IO.FileInfo NextFile in TheFolder.GetFiles())
        {
            if (type == SkillDataType.ROLE)
            {
                if (NextFile.Extension == ".role")
                {
                    res.Add(NextFile.Name.Substring(0, NextFile.Name.Length-5));
                }
            }
            else if (type == SkillDataType.SKILL)
            {
                if (NextFile.Extension == ".skill")
                {
                    res.Add(NextFile.Name.Substring(0, NextFile.Name.Length - 6));
                }
            }
        }
        return res;
    }

    public bool Exists(string id, SkillDataType type)
    {
        string path = "";
        if (type == SkillDataType.ROLE)
        {
            path = root + id + ".role";
         
            return System.IO.File.Exists(path);
        }
        else if (type == SkillDataType.SKILL)
        {
            path = root + id + ".skill";
            return System.IO.File.Exists(path);
        }
        return false;
    }

    public string loadPropertys()
    {
        string path = "Assets\\SkillEditor\\temp\\temps.json";
        return System.IO.File.ReadAllText(path);
    }
    
     
   


    public void DeleveFile(string id, SkillDataType type)
    {
        string path = "";
        if (type == SkillDataType.ROLE)
        {
            path = root + id + ".role";

            System.IO.File.Delete(path);
        }
        else if (type == SkillDataType.SKILL)
        {
            path = root + id + ".skill";
            System.IO.File.Delete(path);
        }
    }
}
