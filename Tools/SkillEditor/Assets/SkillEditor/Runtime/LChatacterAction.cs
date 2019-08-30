using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LChatacterAction  {

    public  int priority = 0;
    public abstract bool isQualified(LChatacterInterface character,LChatacterInformationInterface information);
    public abstract void beginAction(LChatacterInterface character, LChatacterInformationInterface information);
    public abstract void doAction(LChatacterInterface character, LChatacterInformationInterface information);
    public abstract bool isFinish(LChatacterInterface character, LChatacterInformationInterface information);
    
    public static void UpdateAction(LChatacterAction curAction,  List<LChatacterAction> actions,  LChatacterInterface character, LChatacterInformationInterface information)
    {

    }
    public static void OnEvent(int _event,LChatacterAction curAction, List<LChatacterAction> actions, LChatacterInterface character, LChatacterInformationInterface information)
    {
    }
}

 