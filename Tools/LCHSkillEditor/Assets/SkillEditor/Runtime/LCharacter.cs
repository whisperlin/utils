using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LCharacter : CharacterBase, LCharacterInterface
{

    
    protected override void CheckInterface()
    {
        character = this;
        if (null == loader)
            loader = LChatacterRecourceLoader.Instance();
    }
    public int GetAttackLayer()
    {
        return campInformation.attack;
    }
    protected override void OnUpdate()
    {
        UpdateSkillParams();
    }
    public int hp = 10000;
    public int GetHp()
    {
        return hp;
    }
    public void SetHp(int hp)
    {
        this.hp = hp;
    }
    Dictionary<int, int> hatredMap = new Dictionary<int, int>();
    public void AddHaterd(int otherCharacterId, int v)
    {
        if (hatredMap.ContainsKey(otherCharacterId))
        {
            hatredMap[otherCharacterId] = hatredMap[otherCharacterId] + v;
        }
        else
        {
            hatredMap[otherCharacterId] = v;
        }
    }
    
    Dictionary<string, SkillParams> cds = new Dictionary<string, SkillParams>();
    List<SkillParams> cdsList = new List<SkillParams>();
    public void AddParam(string cdName, SkillParams _params)
    {
        if (cds.ContainsKey(cdName))
        {
            var c = cds[cdName];
            cdsList.Remove(c);
            cds.Remove(cdName);
        }
        cdsList.Add(_params);
        cds[cdName] = _params;
    }
    public void RemoveParam(string cdName)
    {
        if (cds.ContainsKey(cdName))
        {
            var c = cds[cdName];
            cdsList.Remove(c);
            cds.Remove(cdName);
        }
    }
    public void UpdateSkillRange(string cdName, float skillRange,float skillWidth)
    {
        cds[cdName].skillRange = skillRange;
        cds[cdName].skillWidth = skillWidth;
    }
    public void UpdateSkillParams()
    {
        for (int i = 0, l = cds.Count; i < l; i++)
        {
            cdsList[i].update();
        }
    }


    public SkillParams GetSkillCDSkillParams(string cdName)
    {
        SkillParams cd = null;
        cds.TryGetValue(cdName, out cd);
        return cd;

    }

    public void updateCDState(string cdName, int skillState)
    {
        cds[cdName].updateState(skillState);
    }

    public Vector3 GetCurForward()
    {
        return transform.forward.normalized;
    }

    public Vector3 GetCurXZForward()
    {
        Vector3 f = transform.forward;
        f.y = 0;
        f.Normalize();
        return f;
    }

    public void CrossFade(string anim_name, float time = 0.05f)
    {
        if (null != animCtrl)
            animCtrl.CrossFade(anim_name, time);
    }
    public void Play(string anim_name)
    {
        if (null != animCtrl)
            animCtrl.Play(anim_name);
    }
    public void SetAnimationLoop(string animName, bool v)
    {
        if(v)
            animCtrl[animName].wrapMode = WrapMode.Loop;
        else
            animCtrl[animName].wrapMode = WrapMode.Default;
    }
    public void ResetAndPlay(string anim_name)
    {
        if (null != animCtrl)
        {
            animCtrl[anim_name].time = 0f;
            animCtrl.Play(anim_name);
        }
    }
    bool isAI = false;
    public bool IsAI()
    {
        return isAI;
    }

    public void EnableAI()
    {
        isAI = true; ;
    }

    int roleId;
    static int _roleId = 0;
    void Awake()
    {
        roleId = _roleId;
        _roleId++;
    }
    
    public int GetId()
    {
        return roleId;
    }
    public int GetCamp()
    {
        return camp;
    }
    public bool IsDead()
    {
        return hp<=0;
    }


    public Vector3 GetCurPosition()
    {
        return transform.position;
    }

    public Quaternion GetCurLocalRot()
    {
        return transform.rotation;
    }

    public Vector3 GetCurLoaclScale()
    {
        return transform.localScale;
    }

    public Animation GetRoleAnimation()
    {
        return animCtrl;
    }

    public Renderer[] GetRoleRender()
    {
        return animCtrl.gameObject.GetComponentsInChildren<Renderer>();
    }
    
    public Vector3 GetGameForward( )
    {
        var f = Camera.main.transform.forward;
        return  new Vector3(f.x, 0, f.z).normalized;
    }

    public void SetCurPosition(Vector3 pos)
    {
        transform.position = pos;
    }
    public void SetCurForward(Vector3 forward)
    {
        transform.forward = forward;
    }

    public void OnHit(Collider other, LCharacterColliderData hitData, Vector3 dir, ref LCharacterAction curAction, List<LCharacterAction> actionList, LChatacterInformationInterface information)
    {

        LCharacterAction.OnHit(other, hitData, ref curAction, actionList, this, information);

    }

    int targetCharacterId = -1;
    public void SetTargetId(int id)
    {
        targetCharacterId = id;
    }
    public int GetTargetId()
    {
        if (IsAI())
        {
            if (hatredMap.Count > 0)
            {
                var e = hatredMap.GetEnumerator();
                int maxHutred = 0;
                while (e.MoveNext())
                {
                    int characterId = e.Current.Key;
                    int hutred = e.Current.Value;
                    if (hutred > maxHutred)
                    {
                        targetCharacterId = characterId;
                    }
                }
                e.Dispose();
            }
        }
        else
        {
        }
        return targetCharacterId;
    }
    

    Dictionary<int, object> characterData = new Dictionary<int, object>();
    public object GetData(int key)
    {
        object r = null;
        characterData.TryGetValue(key, out r);
        return r; ;
    }

    public void SetData(int key, object value)
    {
        characterData[key] = value;
    }

    public void OnHit(LCharacterHitData data)
    {
        int _hp = data.value.GetValueInt("hp", 0);
        int _mp = data.value.GetValueInt("mp", 0);

        this.hp -= (int)_hp;
        this.hp -= (int)_mp;
        if (null != HitFontManager.globalMgr)
        {
            HitFontManager.globalMgr.CreateFont(0, _hp,character.GetCurPosition());
        }
        

    }

    public void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }

    private void OnDestroy()
    {
        information.RemoveCharacter(this);
    }
    
     
}
