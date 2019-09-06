using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public partial class SkillEditorMainWindow : EditorWindow
{

    void SpeceLine()
    {
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }
    [MenuItem("TA/技能编辑器/主窗口")]
    static void Init()
    {
        SkillEditorMainWindow window = (SkillEditorMainWindow)EditorWindow.GetWindow(typeof(SkillEditorMainWindow));
        window.Show();

        SkillEditorWindow.Init();

    }
 
    public void OnEnable()
    {
        SkillEditorData.Instance.Reset(); 
        toolbarInt = 0;
        if (! SkillEditorData.Instance.SkillsData.isInited())
        {
             SkillEditorData.Instance.SkillsData.Init(new EditorFileLoader());
        }
         SkillEditorData.Instance.SkillsData.EditorModel = true;
        golbalWindow = this;
        OnEnable0();
        OnEnable1();
    }
    public void OnDisable()
    {
        SkillEditorData.Instance.SkillsData.Release();
        SkillEditorData.Instance.ReleaseResource();
        try { OnDisable0(); } catch (Exception e) { Debug.LogError(e.ToString() + e.StackTrace); }
        try { OnDisable1() ;}catch (Exception e){Debug.LogError(e.ToString() + e.StackTrace);}
        golbalWindow = null;
    }
    UpdateLimiter limiter = new UpdateLimiter(2.0f);

    public void UpdateSkill()
    {
        if (null != SkillEditorWindow.CurActive && !SkillEditorWindow.CurActive.hasFocus)
        {
            if (Selection.activeTransform != null
            && SkillEditorWindow.autoApdateKeyFrame
            && null != SkillEditorData.Instance.skill
            )
            {
                var skillData = SkillEditorData.Instance.SkillsData.GetSkill(SkillEditorData.Instance.CurSkillId);
                object[] editorState = new object[skillData.channels.Count];
                for (int i = 0, l = skillData.channels.Count; i < l; i++)
                {
                    var ch = skillData.channels[i];
                    //ch.objId;
                    var ct = SkillEditorData.Instance.skill.GetObjectContainById(ch.objId);
                    if (ct.gameobject == Selection.activeTransform.gameObject)
                    {
                        int idx = ch.GetKeyframeIndex(SkillEditorWindow.curSelectFrame);
                        if (idx != -1)
                        {
                            var type = (LCHChannelType)ch.type;

                            switch (type)
                            {
                                case LCHChannelType.PosZ:
                                    {
                                        ch.values[idx] = ct.gameobject.transform.localPosition.z;
                                    }
                                    break;
                                case LCHChannelType.PosY:
                                    {
                                        ch.values[idx] = ct.gameobject.transform.localPosition.y;
                                    }
                                    break;
                                case LCHChannelType.PosX:
                                    {
                                        ch.values[idx] = ct.gameobject.transform.localPosition.x;
                                    }
                                    break;
                                case LCHChannelType.RotY:
                                    {
                                        ch.values[idx] = ct.gameobject.transform.localRotation.eulerAngles.y;
                                    }
                                    break;
                                case LCHChannelType.ScaleX:
                                    {
                                        ch.values[idx] = ct.gameobject.transform.localScale.x;
                                    }
                                    break;
                                case LCHChannelType.ScaleY:
                                    {
                                        ch.values[idx] = ct.gameobject.transform.localScale.y;
                                    }
                                    break;
                                case LCHChannelType.ScaleZ:
                                    {
                                        ch.values[idx] = ct.gameobject.transform.localScale.z;
                                    }
                                    break;

                            }
                        }
                    }

                }
            }
        }
        else
        {
            SkillEditorData.Instance.UpdateModul();
            SkillEditorData.Instance.UpdateAnimation();
        }
    }
    void Update()
    {
        //编辑器改代码的时候经常丢失...
        if (SkillEditorData.Instance.SkillsData.loader == null)
        {
            SkillEditorData.Instance.SkillsData.loader = new EditorFileLoader();
        }



    }
    void OnInspectorUpdate()
    {
        if (null != Camera.main)
        {
            Camera.main.transform.position = new Vector3(10f, 0, 0);
            Camera.main.transform.LookAt(Vector3.zero, Vector3.up);
        }


        Repaint();

        limiter.Update(AutoSaveFun);
    }
    void AutoSaveFun(params object[] args)
    {
        if (! SkillEditorData.Instance.SkillsData.isInited())
        {
             SkillEditorData.Instance.SkillsData.Init(new EditorFileLoader());
        }
        if ( SkillEditorData.Instance.CurRoleId.Length > 0)
        {
             SkillEditorData.Instance.SkillsData.GetRole( SkillEditorData.Instance.CurSkillId);
             SkillEditorData.Instance.SkillsData.SaveRole( SkillEditorData.Instance.CurRoleId);
        }
        if ( SkillEditorData.Instance.CurSkillId.Length > 0)
        {
             SkillEditorData.Instance.SkillsData.GetSkill( SkillEditorData.Instance.CurSkillId);
             SkillEditorData.Instance.SkillsData.SaveSkill( SkillEditorData.Instance.CurSkillId);
        }
    }


    public static int toolbarInt = 0;
    public string[] toolbarStrings = new string[] { "角色&技能", "技能属性" };
    void OnGUI()
    {

        if (! SkillEditorData.Instance.SkillsData.isInited())
        {
             SkillEditorData.Instance.SkillsData.Init(new EditorFileLoader());
        }
        var toolbarInt0 = GUILayout.Toolbar(  toolbarInt, toolbarStrings);
        if (toolbarInt0 != toolbarInt)
        {
            OnChangePange();
            toolbarInt = toolbarInt0;
        }
        
        SpeceLine();
        if (toolbarInt == 0)
        {
            OnGUI0();
        }
        else
        {
            OnGUI1();
        }

    }

    private void OnChangePange()
    {
        selectObjId = -1;
    }

    public Dictionary<string, object> GetEventTemp()
    {
        return  SkillEditorData.Instance.SkillsData.GetEventTemp();
    }

    public string[] GetEventNames()
    {
        return  SkillEditorData.Instance.SkillsData.GetEventNames();
    }
}

 