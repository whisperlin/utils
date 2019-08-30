using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEditorData {
    protected SkillEditorData() { }
    static SkillEditorData instacne = new SkillEditorData();
    //public AssetDatabaseHelper databaseHelper = new AssetDatabaseHelper();
    public static SkillEditorData Instance
    {
        get
        {
            return instacne;
        }
    }
    public SkillModule _skillsData = new SkillModule();
    public string CurSkillId = "";
    public string CurRoleId = "";
    public SkillModule SkillsData { get { return _skillsData; } }
    public LCHSkill skill;
    public float curTime = 0;
    public bool playing = false;
    EditorSkillResourceLoader loader;
    public void ReleaseResource()
    {
        //编辑器才会删主模型。
        if (null != skill)
        {
#if UNITY_EDITOR
            skill.ReleaseRoleModule();
#endif
            skill.Release();
            skill = null;
        }
    }
    public void UpdateModul()
    {
        if (loader == null)
            loader = new EditorSkillResourceLoader( );
        if (CurRoleId.Length == 0)
            return;
        if (CurSkillId.Length == 0)
            return;
        var role = SkillsData.GetRole(CurRoleId);
        var s = SkillsData.GetSkill(CurSkillId);
        if (skill != null && skill.SkillData != s)
        {
            ReleaseResource();
        }
        if (null == skill)
        {
            skill = new LCHSkill(loader, role, s);
        }
#if UNITY_EDITOR
        skill.CheckModulUpdate();
#endif
        skill.SetObjectParent();

    }
    public void UpdateAnimation()
    {
        if (null == skill)
            return;
        skill.ComputeAnim(curTime);
    }
    
 
    public void Reset()
    {
        _skillsData = new SkillModule();
    }

    

    public  void CheckUpdate()
    {
         

    }
}
