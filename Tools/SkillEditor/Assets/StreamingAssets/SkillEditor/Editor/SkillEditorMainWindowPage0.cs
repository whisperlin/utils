using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public partial class SkillEditorMainWindow
{
    public string[] options = new string[] { "所有", "角色", "NPC", "怪物" };
    int index = -1;
    public int roleIndex = 0;
    public int selSkillInt = 0;
 
    public string[] roleIds = new string[] { };
 

    public void OnRoleListModify(params object[] arg)
    {
         
        List<string> str =  SkillEditorData.Instance.SkillsData.GetAllRoles();
        roleIds = str.ToArray(); 
        //Repaint();

    }
    //OnRoleListModify
    public void OnEnable0()
    {
        List<string> str =  SkillEditorData.Instance.SkillsData.GetAllRoles();
        roleIds = str.ToArray();
        EventManager.AddEvent((int)SkillEvent.OnRoleListModify, OnRoleListModify);

    }
    public void OnDisable0()
    {
        EventManager.RemoveEvent((int)SkillEvent.OnRoleListModify, OnRoleListModify);
    }
    Vector2 scrollViewPos;
    Vector2 scrollViewPos1;
    Vector2 scrollViewPos2;

    void LoadRole(string _id)
    {
        roleData =  SkillEditorData.Instance.SkillsData.GetRole(_id);
        string path = roleData.mod;
        anim = null;
        if (path.Length > 0)
        {
            anim = AssetDatabase.LoadAssetAtPath<Animation>(path);
            if (null != anim)
                roleData.mod_name = anim.gameObject.name;
        }
 
    }
    void SaveAminForRole(Animation anim0)
    {
        if (anim0 != anim)
        {
            anim = anim0;
            if (null != anim0)
            {
                string path = "";
                if (PrefabUtility.GetPrefabType(anim.gameObject) == PrefabType.PrefabInstance)
                {
                    UnityEngine.Object parentObject = EditorUtility.GetPrefabParent(anim.gameObject);
                    path = AssetDatabase.GetAssetPath(parentObject);
                }
                else
                {
                    path = AssetDatabase.GetAssetPath(anim.gameObject);
                }

                if (path.Length == 0)
                {
                    roleData.mod = "";
                    roleData.mod_name = "";
                    anim = null;
                }
                else
                {
                    roleData.mod = path;
                    roleData.mod_name = anim.gameObject.name;
                    SkillEditorData.Instance.SkillsData.SaveRole(SkillEditorData.Instance.CurRoleId);
                }

            }
            
        }
        if (null == anim0 && null != roleData)
        {
            roleData.mod = "";
            roleData.mod_name = "";
        }
        
    }
    
    void CheckRoleUpdate()
    {
        if (null ==  SkillEditorData.Instance.SkillsData || roleIds.Length < 1)
        {
             SkillEditorData.Instance.CurRoleId = "";
            return;
        }
        if (roleIndex < 0 || roleIndex >= roleIds.Length)
            roleIndex = 0;
         SkillEditorData.Instance.CurRoleId = roleIds[roleIndex];
        if (roleData == null)
        {
            LoadRole( SkillEditorData.Instance.CurRoleId);
            //GUI.FocusControl("FocusControl01");
        }
        else
        {
            string _id0 = roleData.id;
            if (_id0 !=  SkillEditorData.Instance.CurRoleId)
            {
                 SkillEditorData.Instance.SkillsData.SaveRole(_id0);
                LoadRole( SkillEditorData.Instance.CurRoleId);
                GUI.FocusControl("FocusControl01");
            }
        }
    }
    void DeleteRole()
    {
        if (null != roleData)
        {
            string id = (string)roleData.id;
            roleData = null;
             SkillEditorData.Instance.SkillsData.DeleteRole(id);
        }
    }
    void DeleteSkill()
    {
        if (null != roleData)
        {
            string id = (string)roleData.id;
            //roleData = null;
            //skills.DeleteRole(id);

            if ( SkillEditorData.Instance.CurSkillId.Length > 0)
            {
                 SkillEditorData.Instance.SkillsData.DeleteSkill( SkillEditorData.Instance.CurSkillId);
                //roleData.skills
                //EventManager.CallEvent((int)SkillEvent.DeleteSkill,  SkillEditorData.Instance.CurSkillId);
 

                roleData = null;
            }
        }
    }
    Animation anim = null;

 

    public void OnGUI0()
    {
        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("FocusControl01");
        if (GUILayout.Button("创建角色"))
        {
            IdInputDialog.Init();
        }
        if (GUILayout.Button("删除角色"))
        {
            DeleteRole();
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();

        index = EditorGUILayout.Popup("过滤:", index + 1, options) - 1;

        scrollViewPos = GUILayout.BeginScrollView(scrollViewPos, GUILayout.Height(120));
        roleIndex = GUILayout.SelectionGrid(roleIndex, roleIds, 1);
        GUILayout.EndScrollView();

        SpeceLine();
        SaveAminForRole((Animation)EditorGUILayout.ObjectField(anim, typeof(Animation),false));
        SpeceLine();


        CheckRoleUpdate();
        GUILayout.Label("角色属性:");
        scrollViewPos2 = GUILayout.BeginScrollView(scrollViewPos2, GUILayout.Height(120));

        if( SkillEditorData.Instance.CurRoleId.Length>0)
        {
            PropertyHelper.DrawPropertys(roleData.propertys,  SkillEditorData.Instance.SkillsData.GetPropertyTemp(),  SkillEditorData.Instance.SkillsData.GetPropertyNames());
        }
       
        GUILayout.EndScrollView();
        SpeceLine();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("创建技能"))
        {
            if (null != roleData)
            {
                CreateSkillDialog.Init(roleData.id);
            }

        }
        if (GUILayout.Button("删除技能"))
        {
            DeleteSkill();
            
        
            //skills.GetSkill();
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("技能列表:");
        scrollViewPos2 = GUILayout.BeginScrollView(scrollViewPos2, GUILayout.Height(120));
        if ( SkillEditorData.Instance.CurRoleId.Length > 0)
        {
            selSkillInt = GUILayout.SelectionGrid(selSkillInt, roleData.skills, 1);
            CheckSkillUpdate(selSkillInt);
             
            
        }
 
        
        GUILayout.EndScrollView();
    }
    
    private void CheckSkillUpdate(int idx)
    {
        if (roleData  == null|| roleData.skills.Length == 0)
        {
             SkillEditorData.Instance.CurSkillId = "";
            return;
        }
         SkillEditorData.Instance.CurSkillId = roleData.skills[idx];
        
    }
}
