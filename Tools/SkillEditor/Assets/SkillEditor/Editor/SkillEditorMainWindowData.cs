using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkillEditorMainWindow   {

    public static SkillEditorMainWindow  golbalWindow;
   
    LCHRoleData roleData = null;
    

    
    public LCHSkillData GetSkill()
    {
        if ( SkillEditorData.Instance.CurSkillId.Length == 0)
        {
            return null;
        }
        else
        {
            return  SkillEditorData.Instance.SkillsData.GetSkill( SkillEditorData.Instance.CurSkillId);
        }
    }


}
