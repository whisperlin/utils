using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateSkillDialog : EditorWindow
{

    public string roleId;
    public static void Init(string roleId)
    {
        
        CreateSkillDialog window = (CreateSkillDialog)EditorWindow.GetWindow(typeof(CreateSkillDialog));
        window.roleId = roleId;
        window.Show();
    }

    string id = "";
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("id:");
        id = GUILayout.TextField(id);
        GUILayout.EndHorizontal();

 
        if (GUILayout.Button("确定"))
        {
            if (id.Length == 0)
            {
                EditorUtility.DisplayDialog("警告", "id不能为空", "确定");
                return;
            }
            EventManager.CallEvent((int)SkillEvent.CreateSkill, id  , roleId);
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
