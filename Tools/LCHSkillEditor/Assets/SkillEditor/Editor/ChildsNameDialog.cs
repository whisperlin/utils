using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ChildsNameDialog : PopupWindowContent
{
    public static ChildsNameDialog Show(ChildsNameDialogCallbackFun callback,params object []args)
    {
        ChildsNameDialog w = new ChildsNameDialog();
        w.OnChildsNameDialogCallback = callback;
        w.data = args;
        return w;
    }
    public object[] data;
    public delegate void ChildsNameDialogCallbackFun(string str, object[] args);
    public ChildsNameDialogCallbackFun OnChildsNameDialogCallback;
    Vector2 spos = Vector2.zero;
    public override Vector2 GetWindowSize()
    {
        return new Vector3(350f, 300f);
    }
    
    Dictionary<Transform,bool> state = new Dictionary<Transform, bool>();
    public override void OnOpen()
    {
        state.Clear();
    }
    void OnItems(int space, Transform t)
    {
        bool open = false;
        state.TryGetValue(t, out open);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f * space);
        open = EditorGUILayout.Foldout(open, t.name);
        state[t] = open;
        if (EditorGUILayout.Toggle(false, GUILayout.Width(20f)))
        {
            OnChildsNameDialogCallback(t.name, data);
            editorWindow.Close();
        }
        GUILayout.EndHorizontal();
        if (open)
        {
            int childCount = t.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var t1 = t.GetChild(i);
                OnItems(space + 1, t1);
            }
        }
    }
    public override void OnGUI(Rect rect)
    {
        spos = EditorGUILayout.BeginScrollView(spos);
        EditorGUILayout.BeginVertical();
        var skill =SkillEditorData.Instance.skill;
        if (skill != null&&null != skill.role && null != skill.role.gameobject)
        {
            int childCount = skill.role.gameobject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var t = skill.role.gameobject.transform.GetChild(i);
                OnItems(0,t);
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }
}
