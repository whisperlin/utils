using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterWalkAction : LChatacterAction
{
    public string animName;

   
    public float speed = 10f;
    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        character.CrossFade(animName);
    }

    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        Vector3 forward;
        character.GetForward(out forward);
        Vector3 left = Vector3.Cross(forward, Vector3.up);
        Vector3 MoveDir = (left * VirtualInput.dir.x + forward* VirtualInput.dir.y).normalized;
        Vector3 pos0 =   MoveDir * Time.deltaTime * speed  ;
        Vector3 pos = information.tryMove(character.GetCurPosition(),pos0,true);
        character.SetCurPosition(pos);
        character.SetCurForward(MoveDir);
    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        
        return !VirtualInput.isDirectKeyDown;
    }

    public override bool isQualified(LChatacterAction curAction, LChatacterInterface character, LChatacterInformationInterface information)
    {
        

        return VirtualInput.isDirectKeyDown;
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LChatacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LChatacterInterface character, LChatacterInformationInterface information)
    {

    }
}


 
