using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColliderDialog : PopupWindowContent
{
    Transform root;
    Vector2 spos = Vector2.zero;
    public static ColliderDialog Show( Transform r, ColliderDialogFun callback, params object[] args)
    {
        ColliderDialog w = new ColliderDialog();
        w.OnColliderDialogFun = callback;
        w.root = r;
        w.data = args;
        return w;
    }
    public object[] data;
    public delegate void ColliderDialogFun(int id,string str, object[] args);
    public ColliderDialogFun OnColliderDialogFun;
    ObjDictionary state = new ObjDictionary();
    public override void OnGUI(Rect rect)
    {
        spos = EditorGUILayout.BeginScrollView(spos);
        EditorGUILayout.BeginVertical();

        // SkillEditorData.Instance.skill.objs 
        {
            

            bool b;
            state["-1"] = b =  EditorGUILayout.Foldout(state.GetValue<bool>("-1", false), root.name);
            if (b)
            {
                Collider[] cs = root.GetComponentsInChildren<Collider>();
                for (int i = 0; i < cs.Length; i++)
                {
                    string name = cs[i].gameObject.name;
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(30);
                    if (EditorGUILayout.ToggleLeft(name, false, GUILayout.Width(350f)))
                    {
                        if (root == cs[i].transform)
                        {
                            name = "";
                        }
                        OnColliderDialogFun(-1, name, data);
                        editorWindow.Close();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
        }
        for (int j = 0, c = SkillEditorData.Instance.skill.subDatas[SkillEditorData.Instance.subSkillIndex].objs.Count; j < c; j++)
        {
            var o = SkillEditorData.Instance.skill.subDatas[SkillEditorData.Instance.subSkillIndex].objs[j];
            if (null != o.gameobject && o.objId == -1 || o.type == 1)
            {

                Collider[] cs = o.gameobject.GetComponentsInChildren<Collider>(true);
                if (cs.Length > 0)
                {
                    bool b;
                    state[o.objId.ToString()] = b = EditorGUILayout.Foldout(state.GetValue<bool>(o.objId.ToString(), false), o.gameobject.name);
                    if (b)
                    {
                        for (int i = 0; i < cs.Length; i++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(30);
                            string name = cs[i].gameObject.name;
                            
                            if (EditorGUILayout.ToggleLeft(name, false, GUILayout.Width(350f)))
                            {
                                if (cs[i].gameObject == o.gameobject)
                                {
                                    name = "";
                                }
                                OnColliderDialogFun(o.objId, name, data);
                                editorWindow.Close();
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                }
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

     
}
