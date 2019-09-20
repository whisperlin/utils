using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCharacterSkillAI : LCharacterSkillAction
{
    public override void beginAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        int targetId = character.GetTargetId();
        var c = information.GetCharacter(targetId);
        if (c != null)
        {
            var dir = c.GetCurPosition() - character.GetCurPosition();
            dir.y = 0;
            dir.Normalize();
            character.SetCurForward(dir);
        }
        base.beginAction(character, information);
    }
    public override bool isQualified(LCharacterAction curAction, LCharacterInterface character, LChatacterInformationInterface information)
    {

        var _param = character.GetSkillCDSkillParams(cdName);
        if (null == _param )
        {
            return true;
        }
        else if (!_param.CanUse(skillState))
        {
            return false;
        }
         
        int targetId = character.GetTargetId();
        if (targetId != -1)
        {
            LCharacterInterface target = information.GetCharacter(targetId);
            
            if(null== target ||target.IsDead())
                return false;
            var selPos = character.GetCurPosition();
            var targetPos = target.GetCurPosition();
            float dis = Vector3.Distance(selPos, targetPos);
            if (null != skillData && dis < skillData.skillRange )
            {
                return true;
            }
        }
        return false;
    }

     
}
