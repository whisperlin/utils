using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public  class ActionIdle : LChatacterAction
{
    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        character.CrossFade("Idle");
    }

    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        
    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        return false;
    }

    public override bool isQualified(LChatacterInterface character, LChatacterInformationInterface information)
    {
        return true;
    }
}
