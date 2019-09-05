using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public partial class SkillEditorMainWindow
{
    private Vector2 scrollViewPos3 = Vector2.zero;
    private Vector2 scrollViewPos4 = Vector2.zero;
    private Vector2 scrollViewPos5 = Vector2.zero;

    
    public void OnEnable1()
    {
        EventManager.AddEvent((int)SkillEvent.OnRoleListModify, OnSkillObjectModify);

        //EventManager.AddEvent((int)SkillEvent.OnBind, OnSkillObjectModify);
    }
    public void OnDisable1()
    {
        //EventManager.AddEvent((int)SkillEvent.OnRoleListModify, OnSkillObjectModify);
        //OnSkillObjectModify
        EventManager.RemoveEvent((int)SkillEvent.OnRoleListModify, OnSkillObjectModify);
    }

    private void OnSkillObjectModify(object[] args)
    {
        
    }
    public void UpdateSound(LCHObjectData obj)
    {
        AudioClip g = (AudioClip)EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath<AudioClip>(obj.propertys.GetValue<string>("mod", "")), typeof(AudioClip), false);
        if (null == g)
        {
            obj.propertys["mod"] = "";
            obj.propertys["mod_name"] = "";
        }
        else
        {
            obj.propertys["mod"] = AssetDatabase.GetAssetPath(g);
            obj.propertys["mod_name"] = g.name;
        }
    }
    public void UpdateGameObject(LCHObjectData obj )
    {
        GameObject g =   (GameObject)EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath<GameObject>(obj.propertys.GetValue<string>("mod","")), typeof(GameObject),false);
        if (null == g)
        {
            obj.propertys["mod"] = "";
            obj.propertys["mod_name"] = "";
        }
        else 
        {
            obj.propertys["mod"] = AssetDatabase.GetAssetPath(g);
            obj.propertys["mod_name"] = g.name;
        }
 
    }
    int objIndex = 0;
 

    string[] bindingStr = new string[] {"无绑定","角色根节点","挂点(骨骼)" };
    int selectObjId = -1;
   
    void OnBindNameCallBack(string name, object[] args)
    {
        ObjDictionary propertys = (ObjDictionary)args[0];
        propertys["bind_name"] = name;
    }
    public void OnGUI1() {

        var layourWidth30 = GUILayout.Width(30f);
        var layourWidth80 = GUILayout.Width(80f);
        var layourWidth20 = GUILayout.Width(20f);
        var layourWidth100 = GUILayout.Width(100f);
        var layourWidth200 = GUILayout.Width(200f);
        Color selectColor = new Color(0.4f, 0.4f, 1.0f);
        if (null == roleData)
            return;

        if ( SkillEditorData.Instance.CurSkillId.Length ==0)
        {
            return;
        }
        LCHSkillData skill = null;
 
        if ( SkillEditorData.Instance.CurSkillId.Length > 0)
        {
            skill =  SkillEditorData.Instance.SkillsData.GetSkill( SkillEditorData.Instance.CurSkillId);
     
        }
        if (null == skill)
            return;
        var buttonRect = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加对象", layourWidth80))
        {
            EventManager.CallEvent((int)SkillEvent.SkillAddObject,  SkillEditorData.Instance.CurSkillId);
        }
        if (GUILayout.Button("添加碰撞体", layourWidth80))
        {
            EventManager.CallEvent((int)SkillEvent.SkillAddBoxCollider,  SkillEditorData.Instance.CurSkillId);
        }

        EditorGUI.BeginDisabledGroup(null == anim);
        if (GUILayout.Button("原碰撞体", layourWidth80))
        {

            PopupWindow.Show(buttonRect ,ColliderDialog.Show(anim.transform, OnColliderAdd,SkillEditorData.Instance.CurSkillId));
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("音效", layourWidth80))
        {
            EventManager.CallEvent((int)SkillEvent.SkillAddSound, SkillEditorData.Instance.CurSkillId);
        }
         

        EditorGUILayout.EndHorizontal();
        GUILayout.Label("对象列表:");
        
        scrollViewPos3 = GUILayout.BeginScrollView(scrollViewPos3, GUILayout.Height(120));

        {
            var rect = EditorGUILayout.BeginHorizontal();
         
            bool b0 = selectObjId == -1;
            if (b0)
            {
                EditorGUI.DrawRect(rect, selectColor);
            }
            if (EditorGUILayout.Toggle("", b0, layourWidth20))
            {
                selectObjId = -1;
            }

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField("角色", layourWidth80);
            EditorGUILayout.ObjectField(anim, typeof(Animation), false);
            GUILayout.Button("-", layourWidth30);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        if (skill != null)
        {
            for (int i = 0; i < skill.objs.Length; i++)
            {
                LCHObjectData obj = skill.objs[i];
                var rect = EditorGUILayout.BeginHorizontal();
                bool b0 = selectObjId == obj.id;
                if (b0)
                {
                    EditorGUI.DrawRect(rect, selectColor);
                }
                if (EditorGUILayout.Toggle("", b0, layourWidth20))
                {
                    selectObjId = obj.id;
                }
                obj.name = GUILayout.TextField(obj.name, layourWidth80);
                
                if (obj.type == 1)
                {
                    UpdateGameObject(obj);
                }
                else if (obj.type == 2|| obj.type == 3)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.TextField("触发器");
                    EditorGUI.EndDisabledGroup();
                }
                else if (obj.type == 4)
                {
                    UpdateSound(obj);
                }
                if (GUILayout.Button("-", layourWidth30))
                {
                    if (selectObjId == obj.id)
                        selectObjId = -1;
                    EventManager.CallEvent((int)SkillEvent.RemoveObject, skill.id, obj.id);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
        SpeceLine();
        GUILayout.Label("属性列表:");
  
        scrollViewPos4 = GUILayout.BeginScrollView(scrollViewPos4, GUILayout.Height(200));
        LCHObjectData selectObject = null;
        if (selectObjId > -1   )
        {
            selectObject = skill.GetObject(selectObjId);
            if (selectObject.type != 4)
            {
                var rect = EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(selectObject.type == 3);
                //selectObject.type
                GUILayout.Label("绑定方式", GUILayout.Width(80f));
                int bind0 = selectObject.propertys.GetValueInt("bind", 0);
                int bind = EditorGUILayout.Popup(bind0, bindingStr, GUILayout.Width(100f));
                selectObject.propertys["bind"] = bind;
                if (bind == 2)
                {
                    selectObject.propertys["bind_name"] = EditorGUILayout.TextField(selectObject.propertys.GetValue<string>("bind_name", ""));
                    if (GUILayout.Button(".."))
                    {
                        PopupWindow.Show(rect, ChildsNameDialog.Show(OnBindNameCallBack, selectObject.propertys));
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }
            
        }
        GUILayout.EndScrollView();
        SpeceLine();
        GUILayout.Label("事件属性:");
        GUILayout.BeginHorizontal();
        //1

        scrollViewPos5 = GUILayout.BeginScrollView(scrollViewPos5, GUILayout.Height(200));
        if (skill != null)
        {
            if (null != SkillEditorWindow.selectEvent && null != SkillEditorWindow.selectEventChannel
                )
            {
                int objId = SkillEditorWindow.selectEventChannel.objId;
                var _object = skill.GetObject(objId);
                
                LCHChannelType _type = (LCHChannelType)SkillEditorWindow.selectEventChannel.type;
                if (_type == LCHChannelType.Event)
                {
                    GUILayout.BeginHorizontal();//2
                    PropertyHelper.DrawPropertyTips("激活状态");
                    bool eEnable = EditorGUILayout.Toggle(SkillEditorWindow.selectEvent.GetValue<bool>("enable", true));
                    SkillEditorWindow.selectEvent["enable"] = eEnable;
                    GUILayout.EndHorizontal();//2
                    string[] sounds = ArrayHelper.emptyStringList;
                    int[] soundIds = ArrayHelper.emptyIntList;
                    if (null != SkillEditorData.Instance.skill)
                    {
                        
                        SkillEditorData.Instance.skill.GetSoundList(objId, ref sounds, ref soundIds);
                    }
                    if (eEnable)
                        PropertyHelper.DrawPropertys(SkillEditorWindow.selectEvent, SkillEditorData.Instance.SkillsData.GetEventTemp(), SkillEditorData.Instance.SkillsData.GetEventNames(),null,sounds,soundIds);
                }
                else if (_type == LCHChannelType.Object)
                {
                    string [] anims = ArrayHelper.emptyStringList;
                    
                    if (null != SkillEditorData.Instance.skill)
                    {
                          anims = SkillEditorData.Instance.skill.GetAnimList(objId);
                    }
                    PropertyHelper.DrawPropertys(SkillEditorWindow.selectEvent, SkillEditorData.Instance.SkillsData.GetObjecctTemp(), SkillEditorData.Instance.SkillsData.GetObjectNames(), anims, null,null);
                }
                else if (_type == LCHChannelType.RoleState)
                {
                    string[] anims = ArrayHelper.emptyStringList;

                    if (null != SkillEditorData.Instance.skill)
                    {
                        anims = SkillEditorData.Instance.skill.GetAnimList(objId);
                    }
                    PropertyHelper.DrawPropertys(SkillEditorWindow.selectEvent, SkillEditorData.Instance.SkillsData.GetRoleStateTemp(), SkillEditorData.Instance.SkillsData.GetRoleStateNames(), null, null, null);
                }
            }
             
        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();//1
    }

    public void OnColliderAdd(string str, object[] args)
    {
        EventManager.CallEvent((int)SkillEvent.SkillAddBaseCollider, SkillEditorData.Instance.CurSkillId, str);
    }
}
