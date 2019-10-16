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
            skill =  SkillEditorData.Instance.skillsData.GetSkill( SkillEditorData.Instance.CurSkillId);
     
        }
        if (null == skill)
            return;
        var buttonRect = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加对象", layourWidth80))
        {
            SkillEditorData.Instance.skillsData.SkillAddObject(  SkillEditorData.Instance.CurSkillId);
        }
        if (GUILayout.Button("添加碰撞体", layourWidth80))
        {
            SkillEditorData.Instance.skillsData.SkillAddBoxCollider(  SkillEditorData.Instance.CurSkillId);
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
            SkillEditorData.Instance.skillsData.SkillAddSound( SkillEditorData.Instance.CurSkillId);
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
                    SkillEditorData.Instance.skillsData.RemoveObject( skill.id, obj.id);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
        SpeceLine();
        GUILayout.Label("属性列表:");
  
        scrollViewPos4 = GUILayout.BeginScrollView(scrollViewPos4, GUILayout.Height(100));
        LCHObjectData selectObject = null;
        if (selectObjId > -1   )
        {
            selectObject = skill.GetObject(selectObjId);
            if (selectObject.type == 3)
            {
                var rect = EditorGUILayout.BeginHorizontal();
                //EditorGUI.BeginDisabledGroup(selectObject.type == 3);
                //selectObject.type
                GUILayout.Label("绑定对象", GUILayout.Width(80f));
                int bind0 = selectObject.propertys.GetValueInt("bind", 0);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(selectObject.propertys.GetValue<string>("bind_name", ""));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }
            if (selectObject.type != 4 )
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
        Dictionary<string, object> property_params = new Dictionary<string, object>();
        scrollViewPos5 = GUILayout.BeginScrollView(scrollViewPos5 );
        if (skill != null)
        {
            if (null != SkillEditorWindow.selectEvent && null != SkillEditorWindow.selectEventChannel)
            {
                int objId = SkillEditorWindow.selectEventChannel.objId;
                var _object_type = skill.GetObjectType(objId);
                
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
                    string [] objectNames =  ArrayHelper.emptyStringList;
                    int[] objectid = ArrayHelper.emptyIntList;
                    if (null != SkillEditorData.Instance.skill)
                    {
                        SkillEditorData.Instance.skill.GetAllObjectList(objId, ref sounds, ref soundIds,ref objectNames,ref objectid);
                    }
                    if (eEnable)
                    {
                        property_params["sounds"] = sounds;
                        property_params["soundIds"] = soundIds;
                        property_params["objectNames"] = objectNames;
                        property_params["objectid"] = objectid;
                        PropertyHelper.DrawPropertys(SkillEditorWindow.selectEvent, SkillEditorData.Instance.skillsData.GetEventTemp(), SkillEditorData.Instance.skillsData.GetEventNames(), property_params, _object_type);
                    }
                }
                else if (_type == LCHChannelType.Object)
                {
                    string [] anims = ArrayHelper.emptyStringList;
                    
                    if (null != SkillEditorData.Instance.skill)
                    {
                          anims = SkillEditorData.Instance.skill.GetAnimList(objId);
                    }
 
                    property_params["anims"] = anims;
                    PropertyHelper.DrawPropertys(SkillEditorWindow.selectEvent, SkillEditorData.Instance.skillsData.GetObjecctTemp(), SkillEditorData.Instance.skillsData.GetObjectNames(), property_params, _object_type);
                }
                else if (_type == LCHChannelType.RoleState)
                {
                    string[] anims = ArrayHelper.emptyStringList;

                    if (null != SkillEditorData.Instance.skill)
                    {
                        anims = SkillEditorData.Instance.skill.GetAnimList(objId);
                    }
                    
                    PropertyHelper.DrawPropertys(SkillEditorWindow.selectEvent, SkillEditorData.Instance.skillsData.GetRoleStateTemp(), SkillEditorData.Instance.skillsData.GetRoleStateNames(), null, _object_type);
                }
            }
             
        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();//1
    }

    public void OnColliderAdd(int objId,string str, object[] args)
    {
        SkillEditorData.Instance.skillsData.SkillAddBaseCollider( SkillEditorData.Instance.CurSkillId, objId,str);
    }
}
