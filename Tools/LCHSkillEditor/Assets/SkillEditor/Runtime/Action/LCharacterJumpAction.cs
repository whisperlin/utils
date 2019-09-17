using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterJumpAction : LCharacterAction
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
 
    bool firFrame = true;
    float curTime = 0f;
    public  void initJump(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime = 0f;
        character.CrossFade(animName,0.05f);
        //跳跃的方向
        Vector3 forward;
        character.GetForward(out forward);
        Vector3 left = Vector3.Cross(forward, Vector3.up);
        MoveDir = (-left * VirtualInput.dir.x + forward * VirtualInput.dir.y).normalized;
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
 

        Vector3 pos = character.GetCurPosition();
        Vector3 pos0 = pos;
        float t = curTime / JumpTime;
        float b = t - 1;
        float y = -b * b + 1;

        float h = jumpHeight * y;

        pos.y = beginPositon.y + h;

        pos += MoveDir * jumpSpeed *Time.deltaTime;
        pos = information.tryMove(pos0, pos-pos0, false);
        character.SetCurPosition(pos);
 
    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime += Time.deltaTime;

        if (curTime <= JumpTime) {
            return false;
        }

        Vector3 pos;
        if (!information.getGroundHight( character.GetCurPosition()  ,out pos))
        {
            Debug.LogError("has error");
            pos = character.GetCurPosition();
        }
        float d = character.GetCurPosition().y - pos.y;
        bool inAir = d > 0.001f;
        return !inAir;
    }

    public override bool isQualified(LCharacterAction curAction, LChatacterInterface character, LChatacterInformationInterface information)
    {
       
        if (  VirtualInput.buttons[(int)button])
        {
            Vector3 pos;
            if (!information.getGroundHight(character.GetCurPosition(), out pos))
            {
                Debug.LogError("has error");
                pos = character.GetCurPosition();
            }

            
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



 