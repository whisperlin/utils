using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionSkill))]
public class ActionSkillUI : Editor {

    private ActionSkill itm;
    EditorFileLoader editorLoader = null;
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            base.OnInspectorGUI();
            return;
        }
        else
        {
            base.OnInspectorGUI();
        }

            if (null == editorLoader)
            editorLoader = new EditorFileLoader();
 
        if (itm == null)
            itm = this.target as ActionSkill;

        
        
        //editorLoader.GetIds(SkillDataType.SKILL);
        //方法一
        //当Inspector 面板发生变化时保存数据
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
 
}
