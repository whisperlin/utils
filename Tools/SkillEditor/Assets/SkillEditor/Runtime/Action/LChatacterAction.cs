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
        JUMP,
    }
    public virtual int GetPriority()
    {
        return priority;
    }
    public int priority = 0;

    public abstract IEnumerator onInit(LChatacterRecourceInterface loader, LChatacterInterface character, AddCoroutineFun fun);
    public abstract bool isQualified(LChatacterAction curAction, LChatacterInterface character, LChatacterInformationInterface information);
    public abstract void beginAction(LChatacterInterface character, LChatacterInformationInterface information);
    public abstract void endAction(LChatacterInterface character, LChatacterInformationInterface information);
    public abstract void doAction(LChatacterInterface character, LChatacterInformationInterface information);
    public abstract bool isFinish(LChatacterInterface character, LChatacterInformationInterface information);


    public virtual bool OnTrigger(LCharacterColliderData cdata, Collider other, LChatacterInterface character, LChatacterInformationInterface information)
    {
        //return true 则消息不再向下传递
        return false;
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

    public virtual void onRelease()
    {
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
            if ( (curAction==null || a.GetPriority() > curAction.GetPriority()) && a.isQualified(curAction, character, information) )
            {
                if (character.IsAI())
                {
                    if (null != curAction)
                    {
                        Debug.Log(curAction.ToString()+ " " +curAction.priority + " " +a.ToString() + "  " + a.priority);
                    }
                }
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
    public  static void OnHit
        (Collider collider, LCharacterColliderData cdata, ref LChatacterAction curAction, List<LChatacterAction> actions, LChatacterInterface character, LChatacterInformationInterface information)
    {
        for (int i = 0, c = actions.Count; i < c; i++)
        {
            var a = actions[i];
            if (a.OnTrigger(cdata,collider,character,information))
            {
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
        /*public static void OnHit
            (Collider collider, LCharacterColliderData cdata, Vector3 dir, ref LChatacterAction curAction, List<LChatacterAction> actions, LChatacterInterface character, LChatacterInformationInterface information)
        {



            if (cdata.type == "hit")
            {
                LCharacterHitData data = cdata.getData<LCharacterHitData>();

                ActionType status = (ActionType)data.value.GetValueInt("status", 0);
                float slow_motion = data.value.GetValueFloat("slow_motion", 0f);
                if (data.firstHit)
                {
                    if (data.cdState == CdState.HIT)
                    {
                        LChatacterInterface chr = information.GetCharacter(data.characterId);
                        chr.updateCDState(data.cdName, data.skillState);
                    }
                    if (slow_motion > 0.0001f)
                    {

                        information.slowly(0.01f, slow_motion);
                        data.firstHit = true;
                    }
                }

                for (int i = 0, c = actions.Count; i < c; i++)
                {
                    var a = actions[i];
                    if (status == a.GetActionType())
                    {
                        a.SetHitData(data, dir);
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

        }*/

    }

 