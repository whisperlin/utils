using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterHitFly : LChatacterAction
{
    public string animName;
 

    //跳跃水平移动速度.(服务器很可能只管水平的)

    public float jumpSpeed = 5f;
    //跳跃高度.
    public float jumpHeight = 5f;
    //跳跃到达最高点所用时间.
    public float JumpTime = 2f;

    float hitDelta = 0.1f;
    //跳跃水平方向.
    Vector3 MoveDir = Vector3.zero;

    Vector3 beginPositon;
    string effect;
    GameObject effect_obj;

    float curTime = 0;
    public override void SetHitData(LCharacterHitData data,Vector3 dir )
    {

        float ctrl_time = data.value.GetValueFloat("ctrl_time", 0.0f);
        float hit_back = data.value.GetValueFloat("hit_back", 0.0f);
        float hit_fly = data.value.GetValueFloat("hit_fly", 0.0f);
        //Debug.Log("hit_back = "+ hit_back);
        JumpTime = ctrl_time;
        jumpSpeed = hit_back;
        jumpHeight = hit_fly;
        MoveDir = dir;
        this.effect = data.effect;
        this.effect_obj = data.effect_obj;

    }

    public void initJump(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime = -hitDelta;
        character.CrossFade(animName);
        beginPositon = character.GetCurPosition();
        character.SetCurForward(-MoveDir);
    }
    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        initJump(character, information);
        if (effect_obj != null)
        {
            GameObject g = GameObject.Instantiate(effect_obj);
            g.transform.position = beginPositon;
            g.transform.forward = -MoveDir;
        }
    }
   
    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        Vector3 pos = character.GetCurPosition();
        Vector3 pos0 = pos;
        float J2 = JumpTime * 2;
        if (curTime < 0)
        {
   
            return;
        }
        float t = curTime / JumpTime;
        float b = t - 1;
        float y = -b * b + 1;

        float h = jumpHeight * y;

        pos.y = beginPositon.y + h;

        pos += MoveDir * jumpSpeed * Time.deltaTime;
        /*if (curTime > JumpTime * 2)
        {
            Vector3 gh = information.getGroundHight(pos);
            //Debug.LogError("pos.y =  " + pos.y + " g "+gh.y + " " + (gh.y <= pos.y));
            
        }*/
        pos = information.tryMove(pos0, pos - pos0, false);
         
            
        character.SetCurPosition(pos);

    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime += Time.deltaTime;
        if (curTime <= JumpTime)
        {
            return false;
        }
        var pos = information.getGroundHight(character.GetCurPosition());
        float d = character.GetCurPosition().y - pos.y;
        bool inAir = d > 0.001f;
        return !inAir;
    }

    public override bool isQualified(LChatacterAction curAction, LChatacterInterface character, LChatacterInformationInterface information)
    {

        

        return false;
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LChatacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LChatacterInterface character, LChatacterInformationInterface information)
    {

    }

    public override ActionType GetActionType( )
    {
        return ActionType.HIT_FLY;
    }
}



