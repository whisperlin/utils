using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicesData
{
    public static LayerMask ground;
}
public abstract class LCharacterAction {

    public enum ACTIONATA
    {
        ROLE_STATE = 0,
        //...
        USER_DATA = 100,
    }
    public enum ActionType
    {
        NORMAL,
        HIT_FLY,
        CTRL,
        HIT_BACK,
        HIT_DOWN,
        JUMP,
        
    }
    public virtual int GetPriority()
    {
        return priority;
    }
    public int priority = 0;

    public abstract IEnumerator onInit(LChatacterRecourceInterface loader, LCharacterInterface character, AddCoroutineFun fun);
    public abstract bool isQualified(LCharacterAction curAction, LCharacterInterface character, LChatacterInformationInterface information);
    public abstract void beginAction(LCharacterInterface character, LChatacterInformationInterface information);
    public abstract void endAction(LCharacterInterface character, LChatacterInformationInterface information);
    public abstract void doAction(LCharacterInterface character, LChatacterInformationInterface information);
    public abstract bool isFinish(LCharacterInterface character, LChatacterInformationInterface information);


    public virtual bool CanStopByTrigger(LCharacterColliderData cdata, Collider other, LCharacterInterface character, LChatacterInformationInterface information)
    {
        var v = character.GetData((int)ACTIONATA.ROLE_STATE);
        if (v == null)
        {
            return true;
        }
        if (cdata.type == "hit")
        {
            //如果是攻击
            LCharacterHitData data = cdata.getData<LCharacterHitData>();
            long state = System.Convert.ToInt64(v);
            //如果角色拥有霸体状态.
            if (state == 1 || state == 2)
            {
                //这里加角色检查
                //可以让主角打中人或者 霸体状态被打中才发生减速.
                float slow_motion = data.value.GetValueFloat("slow_motion", 0f);
                if (data.firstHit)
                {
                    if (data.cdState == CdState.HIT)
                    {
                        LCharacterInterface chr = information.GetCharacter(data.characterId);
                        chr.updateCDState(data.cdName, data.skillState);
                    }
                    if (slow_motion > 0.0001f)
                    {
                        information.slowly(0.01f, slow_motion);
                        data.firstHit = true;
                    }
                   
                }
                return false;
            }
        }
        return true;
    }

    public virtual bool OnTrigger(LCharacterColliderData cdata, Collider other, LCharacterInterface character, LChatacterInformationInterface information)
    {
        //return true 则消息不再向下传递
        return false;
    }
    public virtual ActionType GetActionType( )
    {
        return ActionType.NORMAL;
    }

    

    protected static void CheckInstance<T>(ref T t) where T : LCharacterAction,new ()
    {
        if (t == null)
            t = new T();
    }

    public virtual void onRelease()
    {
    }

    public static void UpdateAction(ref LCharacterAction curAction, List<LCharacterAction> actions, LCharacterInterface character, LChatacterInformationInterface information)
    {

        if (null != curAction)
        {
            if (curAction.isFinish(character, information))
            {

                //if (character.IsAI())
                   // Debug.Log("end by self" + curAction.ToString());
                curAction.endAction(character, information);


                curAction = null;
            }
            
        }
        LCharacterAction oldAciton = curAction;
        
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
            if (null != oldAciton)
            {
                
                oldAciton.endAction(character, information);
            }
         
            curAction.beginAction(character, information);
        }
        if (null != curAction)
        {
             
            curAction.doAction(character, information);
        }
        //beginAction
    }
    public  static void OnHit
        (Collider collider, LCharacterColliderData cdata, ref LCharacterAction curAction, List<LCharacterAction> actions, LCharacterInterface character, LChatacterInformationInterface information)
    {
        //if (character.IsAI())
         //   Debug.Log(curAction.ToString() + " check stop");
        if (curAction != null && !curAction.CanStopByTrigger(cdata, collider, character, information))
        {
            return;
        }
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
       

    }

 