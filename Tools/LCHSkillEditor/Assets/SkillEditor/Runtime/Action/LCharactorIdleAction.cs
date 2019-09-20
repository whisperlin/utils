using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public  class LCharactorIdleAction : LCharacterAction
{
   
    public string animName;
    
    public override void beginAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        //character.ResetAndPlay(animName);
        character.CrossFade(animName,0.05f);
        //Debug.LogError("play idle "+ animName);
        //character.Play(animName);
    }

    public override void doAction(LCharacterInterface character, LChatacterInformationInterface information)
    {

        character.CrossFade(animName, 0.05f);


    }

    public override bool isFinish(LCharacterInterface character, LChatacterInformationInterface information)
    {
        return false;
    }

    public override bool isQualified(LCharacterAction curAction,LCharacterInterface character, LChatacterInformationInterface information)
    {
        return true;
         
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LCharacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        
    }
}
