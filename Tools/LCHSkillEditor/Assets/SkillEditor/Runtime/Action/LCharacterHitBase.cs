using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LCharacterHitBase : LCharacterAction
{
     
    protected abstract void SetHitData(LCharacterHitData data, Vector3 dir);
    
    public override bool OnTrigger(LCharacterColliderData cdata, Collider other, LCharacterInterface character, LChatacterInformationInterface information)
    {
        if (cdata.type == "hit")
        {
            LCharacterHitData data = cdata.getData<LCharacterHitData>();
            ActionType status = (ActionType)data.value.GetValueInt("status", 0);
            if (GetActionType() == status)
            {

                if (!data.hittedObject.Contains(character.GetId()) && null != data.value)
                {
                    data.hittedObject.Add(character.GetId());

                    if (character.IsAI())
                    {
                        character.AddHaterd(data.characterId, 1);
                    }
                    float slow_motion = data.value.GetValueFloat("slow_motion", 0f);
                    if (data.firstHit)
                    {
                        if (data.cdState == CdState.HIT)
                        {
                            LCharacterInterface chr;
                            if (information.TryGetCharacter(data.characterId, out chr))
                            {
                                chr.updateCDState(data.cdName, data.skillState);
                            }
                            //    LCharacterInterface chr = information.GetCharacter(data.characterId);
                            
                        }
                        if (slow_motion > 0.0001f)
                        {
                            information.slowly(0.01f, slow_motion);
                            data.firstHit = true;
                        }
                    }
                    int hit_dir = data.value.GetValueInt("hit_dir",0);
                    Vector3 dir;
                    if (hit_dir == 0)
                    {
                        dir = other.transform.forward;
                        
                    }
                    else if (hit_dir == 1)
                    {
                        dir = character.GetCurPosition() - other.transform.position;
                    }
                    else if (hit_dir == 2)
                    {
                        LCharacterInterface target;
                        if (information.TryGetCharacter(data.characterId, out target))
                        {
                            dir = character.GetCurPosition() - target.GetCurPosition();
                        }
                        else
                        {
                            dir = Vector3.forward;
                        }
                            
                    }
                    else 
                    {
                        LCharacterInterface target;
                        if (information.TryGetCharacter(data.characterId, out target))
                        {
                            dir = target.GetCurPosition() - character.GetCurPosition();
                        }
                        else
                        {
                            dir = Vector3.forward;
                        }


                    }
                    dir.y = 0;
                    dir.Normalize();

                    SetHitData(data, dir);
                    return true;
                }
                
               
            }
        }
        return false;
    }
}
