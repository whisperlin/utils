using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public  class LCharactorIdleAction : LCharacterAction
{
   
    public string animName;
    
    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        //character.ResetAndPlay(animName);
        character.CrossFade(animName,0.05f);
        //Debug.LogError("play idle "+ animName);
        //character.Play(animName);
    }

    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {

        character.CrossFade(animName, 0.05f);


    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        return false;
    }

    public override bool isQualified(LCharacterAction curAction,LChatacterInterface character, LChatacterInformationInterface information)
    {
        return true;
         
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LChatacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        
    }
}
