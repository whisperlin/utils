using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LCharacterHitDown : LCharacterAirHit
{
 
    //跳跃水平移动速度.(服务器很可能只管水平的)

    
    //跳跃高度.
    public float jumpHeight = 5f;
    //跳跃到达最高点所用时间.
  

    float hitDelta = 0.05f;
    //跳跃水平方向.
    Vector3 MoveDir = Vector3.zero;

 
    string effect;
    GameObject effect_obj;

 
 
    bool upping = true;
 
    internal string animHitFlyDown;
    

    protected override void SetHitData(LCharacterHitData data, Vector3 dir)
    {
        float hit_down = data.value.GetValueFloat("hit_down", 0.0f);
        float hit_back = data.value.GetValueFloat("hit_back", 0.0f) * 0.5f;
        ctrl_time = data.value.GetValueFloat("ctrl_time", 0.0f);
        MoveDir = Vector3.down*hit_down+dir*hit_back;
        this.effect = data.effect;
        this.effect_obj = data.effect_obj;
    }
 
    public override bool CanStopByTrigger(LCharacterColliderData cdata, Collider other, LCharacterInterface character, LChatacterInformationInterface information)
    {
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
            if (status == ActionType.HIT_FLY || status == ActionType.HIT_DOWN)
            {
                return true;
            }
            float slow_motion = data.value.GetValueFloat("slow_motion", 0f);
            float air_delay = data.value.GetValueFloat("air_delay", 0f);
            if (data.firstHit)
            {
                if (data.cdState == CdState.HIT)
                {
                    LCharacterInterface chr;
                    if (information.TryGetCharacter(data.characterId, out chr))
                    {
                        chr.updateCDState(data.cdName, data.skillState);
                    }
                        
                   
                }
                if (slow_motion > 0.0001f)
                {
                    information.slowly(0.01f, slow_motion);
                    data.firstHit = true;
                }
                character.ResetAndPlay(animNameFly);
                this.air_delay = air_delay;
            }
            return false;
        }
        return true;
    }

    public override void initAirHit(LCharacterInterface character, LChatacterInformationInterface information)
    {
        base.initAirHit(character, information);
        curTime = -hitDelta;
        var fw = -MoveDir;
        fw.y = 0;
        fw.Normalize();
        character.SetCurForward(fw);
        upping = true;
        endPos = character.GetCurPosition();
    }
    public override void beginAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        initAirHit(character, information);
        if (effect_obj != null)
        {
            GameObject g = GameObject.Instantiate(effect_obj);
            g.transform.position = character.GetCurPosition();
            g.transform.forward = -MoveDir;
        }
    }
    public override void doAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
        
        if (onGround)
            return;
 
        var pos0 = character.GetCurPosition();
        var pos = information.tryMove(pos0, MoveDir*Time.deltaTime, false);
        character.SetCurPosition(pos);

    }
    Vector3 endPos;

    public override bool  NoOnGround(LCharacterInterface character, LChatacterInformationInterface information)
    {
        Vector3 pos;
        if (!information.getGroundHight(character.GetCurPosition(), out pos))
        {
            Debug.LogError("has error");
            pos = character.GetCurPosition();
        }
        else
        {
            endPos = character.GetCurPosition();
        }
        float d = character.GetCurPosition().y - pos.y;
        onGround = d < 0.0000001f;
        if (onGround)
        {
            character.CrossFade(animHitFlyDown, 0.2f);
            curTime = 0f;

        }
        return false;
    }
     

    public override bool isQualified(LCharacterAction curAction, LCharacterInterface character, LChatacterInformationInterface information)
    {


        return false;
    }

    public override IEnumerator onInit(LChatacterRecourceInterface loader, LCharacterInterface character, AddCoroutineFun fun)
    {
        yield return null;
    }

    public override void endAction(LCharacterInterface character, LChatacterInformationInterface information)
    {
 
        character.SetCurPosition(endPos);
    }

    public override ActionType GetActionType()
    {
        return ActionType.HIT_DOWN;
    }
}


 