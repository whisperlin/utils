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
            if (SkillEditorData.Instance.skillsData.CreateSkill(id, roleId))
            {
                SkillEditorData.Instance.skillsData.SkillAddBoxCollider(id);
                //SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(id, -1, LCHChannelType.PosX);
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(id, -1, LCHChannelType.PosY);
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(id, -1, LCHChannelType.PosZ);

                //SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(id, 0, LCHChannelType.PosX);
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(id, 0, LCHChannelType.PosY);
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(id, 0, LCHChannelType.PosZ);


                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(id, 0, LCHChannelType.ScaleX, false);
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(id, 0, LCHChannelType.ScaleY, false);
                SkillEditorData.Instance.skillsData.SkillLerpFloatChannel(id, 0, LCHChannelType.ScaleZ, false);


                SkillEditorData.Instance.skillsData.SkillEventChannel(id, -1, LCHChannelType.Object);
                SkillEditorData.Instance.skillsData.SkillEventChannel(id, 0, LCHChannelType.Event);
            }
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
