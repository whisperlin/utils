using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class LCharacterAirHit : LCharacterHitBase
{
    public string animNameFly;
    protected bool onGround = false;
    
    protected float air_delay = 0;
    protected float curTime = 0;
    protected float ctrl_time = 0;


    public virtual void initAirHit(LCharacterInterface character, LChatacterInformationInterface information)
    {
        character.ResetAndPlay(animNameFly);
        onGround = false;
       
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


    public override bool isFinish(LCharacterInterface character, LChatacterInformationInterface information)
    {
        float deltaTime = Time.deltaTime;
        if (air_delay > 0.00001f)
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

            return b;
        }
        else
        {
            return NoOnGround( character,  information);

 
        }
    }

    public virtual bool NoOnGround(LCharacterInterface character, LChatacterInformationInterface information)
    {
        return false;
    }
}



 