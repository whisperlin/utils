using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterJumpAction : LChatacterAction
{
    public string animName;
    public VirtualInput.KeyCode button;

    //跳跃水平移动速度.(服务器很可能只管水平的)
 
    public float jumpSpeed = 5f;
    //跳跃高度.
    public float jumpHeight = 5f;
    //跳跃到达最高点所用时间.
    public float JumpTime = 2f;

    public int maxJumpCount = 2;
    //跳跃水平方向.
    Vector3 MoveDir = Vector3.zero;
  
    Vector3 beginPositon;
 
    int jumpCount = 0;
    float beginTime = 0;
    bool firFrame = true;

    public  void initJump(LChatacterInterface character, LChatacterInformationInterface information)
    {
        beginTime = Time.realtimeSinceStartup;
        character.CrossFade(animName);
        //跳跃的方向
        Vector3 forward;
        character.GetForward(out forward);
        Vector3 left = Vector3.Cross(forward, Vector3.up);
        MoveDir = (left * VirtualInput.dir.x + forward * VirtualInput.dir.y).normalized;
        beginPositon = character.GetCurPosition();
    }
    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        jumpCount = 0;
        firFrame = true;
        initJump(character,information);
        
 
    }

    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        if (!firFrame && jumpCount < maxJumpCount && VirtualInput.IsButtonDown(button))
        {
            jumpCount ++;
            initJump(character, information);
        }
        firFrame = false;
        float curTime = Time.realtimeSinceStartup -beginTime;

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
            float v = 2f*jumpHeight / JumpTime;
            pos.y -= v * Time.deltaTime;
        }
       
        pos += MoveDir * jumpSpeed *Time.deltaTime;
        pos = information.tryMove(pos0, pos-pos0, false);
        character.SetCurPosition(pos);
 
    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        float curTime = Time.realtimeSinceStartup - beginTime;

        if (curTime <= JumpTime) {
            return false;
        }
        var pos = information.getGroundHight(character.GetCurPosition());
        float d = character.GetCurPosition().y - pos.y;
        bool inAir = d > 0.001f;
        return !inAir;
    }

    public override bool isQualified(LChatacterAction curAction, LChatacterInterface character, LChatacterInformationInterface information)
    {
       
        if (  VirtualInput.buttons[(int)button])
        {
            var pos = information.getGroundHight(character.GetCurPosition());
            bool inAir = character.GetCurPosition().y - pos.y > 0.001f;

            return !inAir;
        }

        return false;
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LChatacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LChatacterInterface character, LChatacterInformationInterface information)
    {

    }
}



 