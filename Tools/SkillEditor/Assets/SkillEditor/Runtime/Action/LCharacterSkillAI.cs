using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCharacterSkillAI : LCharacterSkillAction
{
    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
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
    public override bool isQualified(LChatacterAction curAction, LChatacterInterface character, LChatacterInformationInterface information)
    {
        if (!character.CanUsedSkill(cdName, skillState))
        {
            return false;
        }
        int targetId = character.GetTargetId();
        if (targetId != -1)
        {
            LChatacterInterface target = information.GetCharacter(targetId);
            
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
