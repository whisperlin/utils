using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class MenuDialog : PopupWindowContent
{
 
    Vector2 spos = Vector2.zero;
 
    Dictionary<string, bool> state = new Dictionary<string, bool>();
    bool getState(string key)
    {
        try
        {
            return state[key];
        }
        catch (System.Exception e)
        {
            return false;
        }
        
    }
    void OnGUITramsformItem(int objId)
    {
        string key = "transform" + objId;
        string k0 = key + "pos";
        string k1 = key + "rot";
        string k2 = key + "scal";
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        state[k0] = EditorGUILayout.Foldout(getState(k0), "位移");
        if (state[k0])
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Space(12f);
            if (EditorGUILayout.ToggleLeft("Z", false, GUILayout.Width(300f)))
            {
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.PosZ);
                editorWindow.Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Space(12f);
            if (EditorGUILayout.ToggleLeft("Y", false, GUILayout.Width(300f)))
            {
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.PosY);
                editorWindow.Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Space(12f);
            if (EditorGUILayout.ToggleLeft("X", false, GUILayout.Width(300f)))
            {
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.PosX);
                editorWindow.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        state[k1] = EditorGUILayout.Foldout(getState(k1), "旋转");
        if (state[k1])
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Space(12f);
            if (EditorGUILayout.ToggleLeft("Y", false, GUILayout.Width(300f)))
            {
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel( SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.RotY);
                editorWindow.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        state[k2] = EditorGUILayout.Foldout(getState(k2), "缩放");
        if (state[k2])
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Space(12f);
            if (EditorGUILayout.ToggleLeft("Z", false, GUILayout.Width(300f)))
            {
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel( SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.ScaleZ);
                editorWindow.Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Space(12f);
            if (EditorGUILayout.ToggleLeft("Y", false, GUILayout.Width(300f)))
            {
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel( SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.ScaleY);
                editorWindow.Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Space(12f);
            if (EditorGUILayout.ToggleLeft("X", false, GUILayout.Width(300f)))
            {
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel( SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.ScaleX);
                editorWindow.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        if (EditorGUILayout.ToggleLeft("位移旋转缩放", false, GUILayout.Width(300f)))
        {
            SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.PosX);
            SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.PosY);
            SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.PosZ);
            SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.RotY,false);
            SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.ScaleX, false);
            SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.ScaleY, false);
            SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.ScaleZ,false);
            editorWindow.Close();
        }
        EditorGUILayout.EndHorizontal();

    }

    void OnGUIEventItem(int objId)
    {
        EditorGUILayout.BeginHorizontal();
 
        GUILayout.Space(20f);
        if (EditorGUILayout.ToggleLeft("触发事件", false,GUILayout.Width(300f)))
        {
            SkillEditorData.Instance.skillsData.SkillEventChannel( SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.Event);
            editorWindow.Close();
        }
        EditorGUILayout.EndHorizontal();
         
    }
    void OnGUIObjectStateItem(int objId)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        if (EditorGUILayout.ToggleLeft("动作/状态", false, GUILayout.Width(300f)))
        {
            SkillEditorData.Instance.skillsData.SkillEventChannel( SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.Object);
            editorWindow.Close();
        }
        EditorGUILayout.EndHorizontal();
    }

    void OnGUIRoleStateStateItem(int objId)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        if (EditorGUILayout.ToggleLeft("角色状态", false, GUILayout.Width(300f)))
        {
            SkillEditorData.Instance.skillsData.SkillEventChannel(SkillEditorData.Instance.CurSkillId, objId, LCHChannelType.RoleState);
            editorWindow.Close();
        }
        EditorGUILayout.EndHorizontal();
    }
    
    public override void OnGUI(Rect rect)
    {
        spos = EditorGUILayout.BeginScrollView(spos);
        if (null == SkillEditorMainWindow.golbalWindow)
        {
            editorWindow.Close();
            return;
        }
        if (  SkillEditorData.Instance.CurSkillId.Length == 0)
        {
            editorWindow.Close();
            return;
        }
        LCHSkillData skill = SkillEditorData.Instance.skillsData .GetSkill(SkillEditorData.Instance.CurSkillId);
        if (null == skill)
        {
            editorWindow.Close();
            return;
        }
        int objLen = skill.objs.Length;
         
        var layout100 =  GUILayout.Width(100);
        GUILayout.Label("添加轨迹类型", EditorStyles.boldLabel);
        state["rolebase"] = EditorGUILayout.Foldout(getState("rolebase"), "(-1)角色");
        if (state["rolebase"])
        {
            OnGUITramsformItem(-1);
            OnGUIObjectStateItem(-1);

            OnGUIRoleStateStateItem(-1);
        }

        for (int i = 0; i < skill.objs.Length; i++)
        {
            var _o = skill.objs[i];

            string name = "未知";
            if (_o.type == 4)
            {
                continue;
            }
            else if (_o.type == 1)
            {
                name = "(" + _o.id + ")" + _o.name + " 对象";
            }
            else if(_o.type == 2 ||_o.type == 3)
            {
                name = "(" + _o.id + ")" + _o.name + " 触发器";
            }
            string key = "name" + _o.id;
            state[key] = EditorGUILayout.Foldout(getState(key), name);
            if (state[key])
            {
                if (_o.type < 3)
                {
                    OnGUITramsformItem(_o.id);
                }
                if (_o.type == 2|| _o.type == 3)
                {
                    OnGUIEventItem(_o.id);
                }
                else
                {
                    OnGUIObjectStateItem(_o.id);
                }
                
            }
        }


         

        EditorGUILayout.EndScrollView();

    }

    public override void OnOpen()
    {
        //Debug.Log("Popup opened: " + this);
    }

    public override void OnClose()
    {
       // Debug.Log("Popup closed: " + this);
    }
}
 