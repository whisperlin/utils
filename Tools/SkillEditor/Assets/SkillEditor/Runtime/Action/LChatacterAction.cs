using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class LChatacterAction {

    public enum ActionType
    {
        NORMAL,
        HIT_FLY,
        CTRL,
        HIT_BACK,
    }
    public int priority = 0;

    public abstract IEnumerator onInit(LChatacterRecourceInterface loader, LChatacterInterface character, AddCoroutineFun fun);
    public abstract bool isQualified(LChatacterAction curAction, LChatacterInterface character, LChatacterInformationInterface information);
    public abstract void beginAction(LChatacterInterface character, LChatacterInformationInterface information);
    public abstract void endAction(LChatacterInterface character, LChatacterInformationInterface information);
    public abstract void doAction(LChatacterInterface character, LChatacterInformationInterface information);
    public abstract bool isFinish(LChatacterInterface character, LChatacterInformationInterface information);


    public virtual void SetHitData(ObjDictionary data, Vector3 dir)
    {
         
    }
    public virtual ActionType GetActionType( )
    {
        return ActionType.NORMAL;
    }
 

    protected static void CheckInstance<T>(ref T t) where T : LChatacterAction,new ()
    {
        if (t == null)
            t = new T();
    }


    public static void UpdateAction(ref LChatacterAction curAction, List<LChatacterAction> actions, LChatacterInterface character, LChatacterInformationInterface information)
    {

        if (null != curAction)
        {
            if (curAction.isFinish(character, information))
            {
                curAction.endAction(character, information);
                curAction = null;
            }
            
        }
        LChatacterAction oldAciton = curAction;
        
        for (int i = 0, c = actions.Count; i < c; i++)
        {
            var a = actions[i];
            if ( (curAction==null || a.priority >= curAction.priority) && a.isQualified(curAction, character, information) )
            {
                curAction = a;
            }
        }
        if (curAction != oldAciton || (null == curAction ))
        {
            if(null != oldAciton)
                oldAciton.endAction(character, information);
            curAction.beginAction(character, information);
        }
        if (null != curAction)
        {
            curAction.doAction(character, information);
        }
        //beginAction
    }
    public static void OnHit
        (Collider collider, ObjDictionary data,Vector3 dir, ref LChatacterAction curAction, List<LChatacterAction> actions, LChatacterInterface character, LChatacterInformationInterface information)
    {
        ActionType status = (ActionType)data.GetValueInt("status", 0);
        for (int i = 0, c = actions.Count; i < c; i++)
        {
            var a = actions[i];
            if (status ==  a.GetActionType())
            {
                a.SetHitData(data,dir);
                if (null != curAction)
                {
                    curAction.endAction(character, information);
                }
                curAction = a;
                a.beginAction(character, information);
                break;
            }
        }
    }
    
}

 