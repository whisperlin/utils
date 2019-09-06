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

 
    //跳跃水平方向.
    Vector3 MoveDir = Vector3.zero;

    Vector3 beginPositon;

 
    float curTime = 0;
    public override void SetHitData(ObjDictionary data,Vector3 dir)
    {

        float ctrl_time = data.GetValueFloat("ctrl_time", 0.0f);
        float hit_back = data.GetValueFloat("hit_back", 0.0f);
        float hit_fly = data.GetValueFloat("hit_fly", 0.0f);
        //Debug.Log("hit_back = "+ hit_back);
        JumpTime = ctrl_time;
        jumpSpeed = hit_back;
        jumpHeight = hit_fly;
        MoveDir = dir;


    }

    public void initJump(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime = 0f;
        character.CrossFade(animName);
        beginPositon = character.GetCurPosition();
        character.SetCurForward(-MoveDir);
    }
    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        initJump(character, information);
    }
   
    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        Vector3 pos = character.GetCurPosition();
        Vector3 pos0 = pos;
        if (curTime <= JumpTime)
        {
            float t = curTime / JumpTime;
            float b = t - 1;
            float y = -b * b + 1;

            float h = jumpHeight * y;
            pos.y = beginPositon.y + h;

        }
        else if (curTime <= JumpTime * 2)
        {
            float t = (JumpTime * 2 - curTime) / JumpTime;
            float b = t - 1;
            float y = -b * b + 1;
            float h = jumpHeight * y;
            pos.y = beginPositon.y + h;
        }
        else
        {
            float v = 2f * jumpHeight / JumpTime;
            pos.y -= v * Time.deltaTime;
        }

        pos += MoveDir * jumpSpeed * Time.deltaTime;
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



