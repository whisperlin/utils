using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterHunt : LCharacterAction
{
    public string animName;

    public float mixDistance = 1f;
    public float speed = 10f;
    public override void beginAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        character.CrossFade(animName,0.05f );
        //character.Play(animName);
    }

    public override void doAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        int targetId = character.GetTargetId();
        if (targetId != -1)
        {
            LCharacterInterface target = information.GetCharacter(targetId);
           
            if (null == target || target.IsDead())
                return ;
            var selPos = character.GetCurPosition();
            var targetPos = target.GetCurPosition();
            Vector3 MoveDir = targetPos - selPos;
            MoveDir.y = 0f;
             

            MoveDir.Normalize();
            Vector3 pos0 = MoveDir * Time.deltaTime * speed;
            Vector3 pos = information.tryMove(character.GetCurPosition(), pos0, true);
            character.SetCurPosition(pos);
            character.SetCurForward(MoveDir);

        }
    }

    public override bool isFinish(LCharacterInterface character, LChatacterInformationInterface information)
    {

        int targetId = character.GetTargetId();
        if (targetId != -1)
        {
            LCharacterInterface target = information.GetCharacter(targetId);
            
            if (null == target || target.IsDead())
                return true;

            var selPos = character.GetCurPosition();

            var targetPos = target.GetCurPosition();

            float f = Vector3.Distance(selPos, targetPos);
            if (f < mixDistance)
            {
                return true;
            }
            return false;
        }
        return true;
    }

    public override bool isQualified(LCharacterAction curAction, LCharacterInterface character, LChatacterInformationInterface information)
    {
        int targetId = character.GetTargetId();
        if (targetId != -1)
        {
            LCharacterInterface target = information.GetCharacter(targetId);
            
            if (null == target || target.IsDead())
                return false;
            var selPos = character.GetCurPosition();

            var targetPos = target.GetCurPosition();

            float f = Vector3.Distance(selPos, targetPos);
            if (f > mixDistance)
            {
                return true;
            }
            
             
             
        }
        return false;
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LCharacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LCharacterInterface character, LChatacterInformationInterface information)
    {

    }
}





 
