using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharactorFallAction : LCharacterAction
{
    public string animName;

    public float fallSpeed = 5f;

    public override void beginAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        //Debug.LogError("begin fall");
        character.CrossFade(animName,0.05f);
    }

    public override void doAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        var pos0 = character.GetCurPosition();

        Vector3 pos;
        if (!information.getGroundHight(pos0, out pos))
        {
            Debug.LogError("has error");
            pos = character.GetCurPosition();
        }
        
        bool inAir = pos0.y - pos.y > 0.01f;
        if (inAir)
        {
            pos0.y -= fallSpeed * Time.deltaTime;
            pos0.y = pos0.y > pos.y ? pos0.y : pos.y;
            character.SetCurPosition(pos0);
        }
    }

    public override bool isFinish(LCharacterInterface character, LChatacterInformationInterface information)
    {
        Vector3 pos;
        if (!information.getGroundHight(character.GetCurPosition(), out pos))
        {
            Debug.LogError("has error");
            pos = character.GetCurPosition();
        }
        
        bool inAir = character.GetCurPosition().y - pos.y > 0.01f;
        return !inAir;
    }

    public override bool isQualified(LCharacterAction curAction, LCharacterInterface character, LChatacterInformationInterface information)
    {
        Vector3 pos;
        if (!information.getGroundHight(character.GetCurPosition(), out pos))
        {
            Debug.LogError("has error");
            pos = character.GetCurPosition();
        }
 
        bool inAir = character.GetCurPosition().y - pos.y > 0.01f;
         
        return inAir;
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LCharacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        Vector3 pos;
        if (!information.getGroundHight(character.GetCurPosition(), out pos))
        {
            Debug.LogError("has error");
            pos = character.GetCurPosition();
        }
   
        character.SetCurPosition(pos);

        //Debug.LogError("end fall");

    }
}





 