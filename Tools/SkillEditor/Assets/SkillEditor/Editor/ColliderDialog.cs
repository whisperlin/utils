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
    public delegate void ColliderDialogFun(string str, object[] args);
    public ColliderDialogFun OnColliderDialogFun;

    public override void OnGUI(Rect rect)
    {
        spos = EditorGUILayout.BeginScrollView(spos);
        EditorGUILayout.BeginVertical();

        Collider [] cs = root.GetComponentsInChildren<Collider>();
        for (int i = 0; i < cs.Length; i++)
        {
            string name = cs[i].gameObject.name;
            if (EditorGUILayout.ToggleLeft(name, false,GUILayout.Width(350f)))
            {
                OnColliderDialogFun(name,data);
                editorWindow.Close();
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

     
}
