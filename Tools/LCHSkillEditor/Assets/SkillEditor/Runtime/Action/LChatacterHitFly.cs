using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterHitFly : LCharacterHitBase
{
    public string animNameFly;
 

    //跳跃水平移动速度.(服务器很可能只管水平的)

    public float jumpSpeed = 5f;
    //跳跃高度.
    public float jumpHeight = 5f;
    //跳跃到达最高点所用时间.
    public float JumpTime = 2f;

    float hitDelta = 0.05f;
    //跳跃水平方向.
    Vector3 MoveDir = Vector3.zero;

    Vector3 beginPositon;
    string effect;
    GameObject effect_obj;

    float curTime = 0;
    float ctrl_time = 0f;
    bool onGround = false;
    bool upping = true;
    internal string animHitFlyDown;

    protected override  void SetHitData(LCharacterHitData data, Vector3 dir)
    {

        float hit_back_time = data.value.GetValueFloat("hit_back_time", 0.0f);
        float hit_back = data.value.GetValueFloat("hit_back", 0.0f)*0.5f;
        float hit_fly = data.value.GetValueFloat("hit_fly", 0.0f);
        ctrl_time = data.value.GetValueFloat("ctrl_time", 0.0f);
        //Debug.Log("hit_back = "+ hit_back);
        JumpTime = hit_back_time;
        jumpSpeed = hit_back;
        jumpHeight = hit_fly;
        MoveDir = dir;
        this.effect = data.effect;
        this.effect_obj = data.effect_obj;

    }

    public void initJump(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime = -hitDelta;
        character.CrossFade(animNameFly,0.05f);
        beginPositon = character.GetCurPosition();
        character.SetCurForward(-MoveDir);
        onGround = false;
        upping = true;
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
        if (onGround)
            return;
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
        if (upping && curTime > JumpTime )
        {
            upping = false;
            character.CrossFade(animHitFlyDown,0.2f);

        }
        pos = information.tryMove(pos0, pos - pos0, false);
         
            
        character.SetCurPosition(pos);

    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime += Time.deltaTime;
       if (onGround)
        {
 
            return curTime > ctrl_time;
        }
        else
        {
            if (curTime <= JumpTime)
            {
                return false;
            }
            var pos = information.getGroundHight(character.GetCurPosition());
            float d = character.GetCurPosition().y - pos.y;
            onGround = d < 0.0000001f;
            if (onGround)
                curTime = 0f;
            return false;
        }
        //animHitFlyDown



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

    }

    public override ActionType GetActionType( )
    {
        return ActionType.HIT_FLY;
    }
}



