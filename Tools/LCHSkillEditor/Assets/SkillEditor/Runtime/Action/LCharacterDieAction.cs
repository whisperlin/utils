using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterDieAction : LCharacterAction
{
    public string animName ;
  

    //跳跃水平移动速度.(服务器很可能只管水平的)

    public float jumpSpeed = 5f;
    //跳跃高度.
    public float jumpHeight = 5f;
    //跳跃到达最高点所用时间.
    public float JumpTime = 2f;

 
    //跳跃水平方向.
    Vector3 MoveDir = Vector3.zero;

    Vector3 beginPositon;

    int jumpCount = 0;

    bool firFrame = true;
    int state = 0;
    float curTime = 0f;
    public  float destroyTime;

    public void initJump(LCharacterInterface character, LChatacterInformationInterface information)
    {
        curTime = 0f;
        character.CrossFade(animName, 0.05f);
        //跳跃的方向
        Vector3 forward = character.GetGameForward();
        Vector3 left = Vector3.Cross(forward, Vector3.up);
        MoveDir = -character.GetCurXZForward();
        beginPositon = character.GetCurPosition();
    }
    public override void beginAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        state = 1;
        jumpCount = 0;
        firFrame = true;
        initJump(character, information);


    }

    public override void doAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        curTime += Time.deltaTime;
        if (state == 2)
        {
            if (destroyTime > 0 && curTime > destroyTime)
            {
                character.DestroySelf();
            }
            return;
        }
            
        
        firFrame = false;


        Vector3 pos = character.GetCurPosition();
        Vector3 pos0 = pos;
        float t = curTime / JumpTime;
        float b = t - 1;
        float y = -b * b + 1;

        float h = jumpHeight * y;

        pos.y = beginPositon.y + h;

        pos += MoveDir * jumpSpeed * Time.deltaTime;
        pos = information.tryMove(pos0, pos - pos0, false);
        character.SetCurPosition(pos);


        Vector3 gpos;
        if (!information.getGroundHight(character.GetCurPosition(), out gpos))
        {
            gpos = character.GetCurPosition();
        }
        float d = character.GetCurPosition().y - gpos.y;
        bool inAir = d > 0.001f;
        if (!inAir)
        {
            state = 2;
            curTime = 0f;
        }

    }

    public override bool isFinish(LCharacterInterface character, LChatacterInformationInterface information)
    {



        return false;
    }

    public override bool isQualified(LCharacterAction curAction, LCharacterInterface character, LChatacterInformationInterface information)
    {
       
        return character.IsDead() ;
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LCharacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        state = 0;
    }
}




 