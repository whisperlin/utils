using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterHitBack : LCharacterHitBase
{
    public string animName;


    //跳跃水平移动速度.(服务器很可能只管水平的)

    public float ctrl_time = 5f;

    public float hit_back_speed = 2f;

    public float hit_back_time = 2f;


    //跳跃水平方向.
    Vector3 MoveDir = Vector3.zero;

    Vector3 beginPositon;
    Vector3 endPoition;
    string effect;
    GameObject effect_obj;

    float curTime = 0;
    
    protected override void SetHitData(LCharacterHitData data, Vector3 dir)
    {
        float ctrl_time = data.value.GetValueFloat("ctrl_time", 0.0f);
        float hit_back = data.value.GetValueFloat("hit_back", 0.0f);
        float hit_back_time = data.value.GetValueFloat("hit_back_time", 0.0f);
        this.ctrl_time = ctrl_time;
        this.hit_back_speed = hit_back;
        this.hit_back_time = hit_back_time;
        this.effect = data.effect;
        this.effect_obj = data.effect_obj;

        MoveDir = dir;
    }


    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        
        curTime = 0f;
        //character.CrossFade(animName);
        character.ResetAndPlay(animName );
        //character.Play(animName);
        //Debug.Log("play "+ animName);
        beginPositon = character.GetCurPosition();
        endPoition = beginPositon += MoveDir * hit_back_time * hit_back_speed;
        if (effect_obj != null)
        {

            GameObject g = GlobalEffectPool.Instacne().pools.GetObject(effect_obj);
            g.transform.position = beginPositon;
            g.transform.forward = -MoveDir;
            AutoReleaseEffect eff = g.AddComponent<AutoReleaseEffect>();
            g.SetActive(true);

        }


        //effectId = 
    }

    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {

        /*Vector3 test;
        if (!information.getGroundHight(character.GetCurPosition(), out test))
        {
            Debug.LogError("error");
        }*/
        if (curTime <= hit_back_time)
        {
            Vector3 dir = MoveDir * Time.deltaTime * hit_back_speed;
            Vector3 pos = information.tryMove(character.GetCurPosition(), dir, true);
            character.SetCurPosition(pos);
        }
        else
        {
            endPoition.y = character.GetCurPosition().y; ;
            Vector3 pos = information.tryMove(endPoition, Vector3.zero, true);
        }
 
        /*if (!information.getGroundHight(character.GetCurPosition(), out test))
        {
            Debug.LogError("error");
        }*/
 
    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime += Time.deltaTime;

        if (curTime <= hit_back_time + ctrl_time)
        {
            return false;
        }
        //Debug.Log("hit finish  at " + curTime);
        return true;
    }

    public override bool isQualified(LCharacterAction curAction, LChatacterInterface character, LChatacterInformationInterface information)
    {



        return false;
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LChatacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        /*if (character.IsAI())
        {
            Debug.Log("end hit at "+ curTime);
        }*/
    }

    public override ActionType GetActionType()
    {
        return ActionType.HIT_BACK;
    }
}



