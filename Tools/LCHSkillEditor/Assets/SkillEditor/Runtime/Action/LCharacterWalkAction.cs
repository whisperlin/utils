using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterWalkAction : LCharacterAction
{
    public string animName;

   
    public float speed = 10f;
    public override void beginAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        character.CrossFade(animName,0.05f);
    }

    public override void doAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        Vector3 forward = character.GetGameForward( );
        Vector3 left = Vector3.Cross(forward, Vector3.up);
        Vector3 MoveDir = (-left * VirtualInput.dir.x + forward* VirtualInput.dir.y).normalized;
        Vector3 dir =   MoveDir * Time.deltaTime * speed  ;
        Vector3 basePos = character.GetCurPosition();
        Vector3 pos = information.tryMove(basePos, dir,true);
        if (basePos.y - pos.y > 0.5f)
        {
            pos = information.tryMove(basePos, dir, true);
        }
        character.SetCurPosition(pos);
        character.SetCurForward(MoveDir);
    }

    public override bool isFinish(LCharacterInterface character, LChatacterInformationInterface information)
    {
        
        return !VirtualInput.isDirectKeyDown;
    }

    public override bool isQualified(LCharacterAction curAction, LCharacterInterface character, LChatacterInformationInterface information)
    {
        

        return VirtualInput.isDirectKeyDown;
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LCharacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LCharacterInterface character, LChatacterInformationInterface information)
    {

    }
}


 
