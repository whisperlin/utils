using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


 
public class IdInputDialog : EditorWindow
{
    public string[] options = new string[] {   "角色", "NPC", "怪物" };
    public static IdInputDialog Init()
    {
        IdInputDialog window = (IdInputDialog)EditorWindow.GetWindow(typeof(IdInputDialog));
        window.Show();
        return window;
         
    }
    public string id="";
    public int type = 0;
 
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("id:");
        id = GUILayout.TextField(id);
        GUILayout.EndHorizontal();

        type = EditorGUILayout.Popup("类型:", type, options);
        if (GUILayout.Button("确定"))
        {
            if (id.Length == 0)
            {
                EditorUtility.DisplayDialog("警告", "id不能为空", "确定");
                return;
            }
            SkillEditorData.Instance.skillsData.CreateRole( id, type);

            SkillEditorMainWindow .golbalWindow.OnRoleListModify();
            Close();

        }
        if (GUILayout.Button("取消"))
        {
            Close();
        }
    }
    private void OnLostFocus()
    {
        Focus();
    }
}
