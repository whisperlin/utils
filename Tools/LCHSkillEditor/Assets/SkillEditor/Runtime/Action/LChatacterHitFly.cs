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
    float air_delay = 0f;
    internal string animHitFlyDown;
    bool beBreak = false;
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


    public override bool CanStopByTrigger(LCharacterColliderData cdata, Collider other, LChatacterInterface character, LChatacterInformationInterface information)
    {
        //暂时倒地不能攻击
        if (onGround)
        {
            return false;
        }
        if (!base.CanStopByTrigger(cdata, other, character, information))
        {
            return false;
        }
        if (cdata.type == "hit")
        {
            LCharacterHitData data = cdata.getData<LCharacterHitData>();
            ActionType status = (ActionType)data.value.GetValueInt("status", 0);
            if (status ==  ActionType.HIT_FLY  || status == ActionType.HIT_DOWN)
            {
                beBreak = true;
                return true;
            }
            float slow_motion = data.value.GetValueFloat("slow_motion", 0f);
            float air_delay = data.value.GetValueFloat("air_delay", 0f);
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
                character.ResetAndPlay(animNameFly );
                this.air_delay = air_delay;
 
            }
            return false;

        }
        return true;
    }

    public void initJump(LChatacterInterface character, LChatacterInformationInterface information)
    {
        curTime = -hitDelta;
        character.ResetAndPlay(animNameFly );
        beginPositon = character.GetCurPosition();
        character.SetCurForward(-MoveDir);
        onGround = false;
        upping = true;
        beBreak = false;
        endPos = character.GetCurPosition();
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
        Vector3 pos = endPos;
 
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
    Vector3 endPos;
    public override bool isFinish(LChatacterInterface character, LChatacterInformationInterface information)
    {
        float deltaTime = Time.deltaTime;
        
        if(air_delay>0.00001f)
        {
            if (this.air_delay > deltaTime)
            {
                this.air_delay -= deltaTime;
                return false;
            }
            else
            {
                deltaTime -= air_delay;
                air_delay = 0f;
            }
        }
 
        curTime += deltaTime;
       if (onGround)
        {
            bool b = curTime > ctrl_time;
            //if(b)
            //    Debug.LogError("end on ground");
            return b;
        }
        else
        {
            if (curTime <= JumpTime)
            {
                return false;
            }
            Vector3 pos;
            if (!information.getGroundHight(character.GetCurPosition(), out pos))
            {
                Debug.LogError("has error");
                character.SetCurPosition(endPos);

                information.getGroundHight(endPos, out pos);
 
            }
            else
            {
                endPos = character.GetCurPosition();
            }
             
            float d = character.GetCurPosition().y - pos.y;
            onGround = d < 0.0000001f;
            Debug.Log(" onGround " + onGround);
            if (onGround)
                curTime = 0f;
            return false;
        }
 
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
        if(!beBreak)
            character.SetCurPosition(endPos);
    }

    public override ActionType GetActionType( )
    {
        return ActionType.HIT_FLY;
    }
}



