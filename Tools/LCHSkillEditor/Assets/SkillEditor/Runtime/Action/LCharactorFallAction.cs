using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharactorFallAction : LCharacterAction
{
    public string animName;

    public float fallSpeed = 5f;

    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        //Debug.LogError("begin fall");
        character.CrossFade(animName,0.05f);
    }

    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        var pos0 = character.GetCurPosition();
        var pos = information.getGroundHight(pos0);
        bool inAir = pos0.y - pos.y > 0.01f;
        if (inAir)
        {
            pos0.y -= fallSpeed * Time.deltaTime;
            pos0.y = pos0.y > pos.y ? pos0.y : pos.y;
            character.SetCurPosition(pos0);
        }
    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        var pos = information.getGroundHight(character.GetCurPosition());
        bool inAir = character.GetCurPosition().y - pos.y > 0.01f;
        return !inAir;
    }

    public override bool isQualified(LCharacterAction curAction, LChatacterInterface character, LChatacterInformationInterface information)
    {
       
        var pos = information.getGroundHight(character.GetCurPosition());
        bool inAir = character.GetCurPosition().y - pos.y > 0.01f;
         
        return inAir;
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LChatacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        var pos = information.getGroundHight(character.GetCurPosition());
        character.SetCurPosition(pos);

        //Debug.LogError("end fall");

    }
}





 