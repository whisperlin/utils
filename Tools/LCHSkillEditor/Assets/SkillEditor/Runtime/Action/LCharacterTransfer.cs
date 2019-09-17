using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterTransferData
{
    internal Transform target;
    internal float height;
    internal float speed;
}
public class LCharacterTransfer : LCharacterAction
{
    public string animName;
    public VirtualInput.KeyCode button;

    //跳跃水平移动速度.(服务器很可能只管水平的)

    public float jumpSpeed = 5f;
    //跳跃高度.
    public float jumpHeight = 5f;
    //跳跃到达最高点所用时间.
    float JumpTime = 2f;

 
    public Vector3 beginPositon;
    public Vector3 endPositoin;

    public override int GetPriority()
    {
        if (isJumpping)
        {
            return 20000;
        }
        return priority;
    }

    float curTime = 0f;
    bool isJumpping = false;
    public void initJump(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime = 0f;
        character.CrossFade(animName,0.05f);

        Vector3 dir = endPositoin - beginPositon;
        float length = Vector3.Distance(endPositoin, beginPositon);
        //跳跃的方向
        Vector3 forward = dir;
        forward.y = 0;
        forward.Normalize();
        character.SetCurForward(forward);

        JumpTime = length * 0.5f / jumpSpeed;
        beginPositon = character.GetCurPosition();
    }
    public override void beginAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        initJump(character, information);
        isJumpping = true;
    }

    public override bool OnTrigger(LCharacterColliderData cdata, Collider other, LChatacterInterface character, LChatacterInformationInterface information)
    {
        if (cdata.type == "tra")
        {
            LCharacterTransferData data = cdata.getData<LCharacterTransferData>();
            jumpSpeed = data.speed;
            jumpHeight = data.height;
            beginPositon = character.GetCurPosition();
            endPositoin = data.target.position;
            return true;
        }
        return false;
    }

    public override void doAction(LChatacterInterface character, LChatacterInformationInterface information)
    {
        Vector3 pos = character.GetCurPosition();
        Vector3 pos0 = pos;
        float t = curTime / JumpTime;
        float b = t - 1;
        float y = -b * b + 1;

        float h = jumpHeight * y;


        pos = Vector3.Lerp(beginPositon, endPositoin, t * 0.5f);
        pos += new Vector3(0f, h, 0f);
        character.SetCurPosition(pos);

    }

    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {

        curTime += Time.deltaTime;

        if (curTime <= JumpTime * 2)
        {
            //Debug.LogError("not finish LCharacterTransfer curTime " + curTime + " JumpTime" + (JumpTime*2));
            return false;
        }
       
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
        isJumpping = false;
        character.SetCurPosition(information.getGroundHight(endPositoin+Vector3.up));
    }
    public override ActionType GetActionType()
    {
        return ActionType.JUMP;
    }
}



 