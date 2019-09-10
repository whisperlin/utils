using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LChatacter : MonoBehaviour, LChatacterInterface
{
    [Header("所属阵营")]
    public int camp = 0;
    //如果是程序另行加载角色，请将其作为本对象的子节点。并且localPosition 为 0 0 0 
    [Header("角色")]
    public Animation animCtrl;
    [Header("碰撞体（被其它阵营攻击到碰撞体则会被击中）")]
    public Collider[] collders = new Collider[0];
    public static LChatacterRecourceInterface loader;
    public static LChatacterInformationInterface information = CharactorData.Instance();
    public LChatacterInterface charactor;

    static int _roleId = 0;

    Dictionary<string, CDParams> cds = new Dictionary<string, CDParams>();
    List<CDParams> cdsList = new List<CDParams>();
    public void AddParam(string cdName, CDParams _params)
    {
        cdsList.Add(_params);
        cds[cdName] = _params;
    }
    public void UpdateCDParams()
    {
        for (int i = 0, l = cds.Count; i < l; i++)
        {
            cdsList[i].update();
        }
    }
    public bool CanUsedSkill(string cdName, int state)
    {
        return cds[cdName].CanUse(state);
    }
    public void updateCDState(string cdName, int skillState)
    {
        cds[cdName].updateState(skillState);
    }
    int roleId;
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
        return false;
    }
    
    void CheckInterface()
    {
        if (null == charactor)
            charactor = this;
        if (null == loader)
            loader = LChatacterRecourceLoader.Instance();
    }
    public void AddAction(LChatacterAction action)
    {
        CheckInterface();
        StartCoroutine( action.onInit(loader,this, AddCoroutine));
        actionList.Add(action);
    }
    List<LChatacterAction> actionList = new List<LChatacterAction>();
    //当前行为.
    public LChatacterAction curAction;

    CampInformation campInformation;
    void UpdateColliderLayer()
    {
        if (camp > GlobalCamp.globalCampInformations.Length)
        {
            camp = GlobalCamp.globalCampInformations.Length - 1;
        }
        if (camp < 0)
            camp = 0;
        campInformation = GlobalCamp.globalCampInformations[camp];
        for (int i = 0; i < collders.Length; i++)
        {
            collders[i].gameObject.layer = campInformation.self;

            Rigidbody rb = collders[i].gameObject.GetComponent<Rigidbody>();
            if (null == rb)
                rb = collders[i].gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
    // Use this for initialization
    void Start () {
        information.AddCharacter(this);
        UpdateColliderLayer();
    }

    void AddCoroutine(IEnumerator fun)
    {
        StartCoroutine(fun);
    }

 
    void Update () {
        UpdateCDParams();
        LChatacterAction.UpdateAction(ref curAction, actionList, charactor, information);
	}

    public void CrossFade(string anim_name)
    {
        if(null != animCtrl)
            animCtrl.CrossFade(anim_name,0.05f);
    }
    public void Play(string anim_name)
    {
        if (null != animCtrl)
            animCtrl.Play(anim_name);
    }
    public void ResetAndPlay(string anim_name)
    {
        if (null != animCtrl)
        {
            animCtrl[anim_name].time = 0f;
            animCtrl.Play(anim_name);
        }
            
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

    public void GetForward(out Vector3 forward )
    {
        var f = Camera.main.transform.forward;
        forward = new Vector3(f.x, 0, f.z).normalized;
    }

    public void SetCurPosition(Vector3 pos )
    {
        transform.position = pos;
    }
    public void SetCurForward( Vector3 forward)
    {
        transform.forward = forward;
    }
 
    public int  GetAttackLayer()
    {
        return campInformation.attack;
    }
    Dictionary<int, int> hatredMap = new Dictionary<int, int>();
    
    public void OnHit(Collider other, LCharacterHitData hitData, Vector3 dir, ref LChatacterAction curAction, List<LChatacterAction> actionList , LChatacterInformationInterface information)
    {
        if (this.IsAI())
        {
            this.AddHaterd(hitData.characterId,1);
        }
        LChatacterAction.OnHit(other, hitData, dir, ref curAction, actionList, this, information);
    }
    public int GetTargetId()
    {
        int targetCharacterId = -1;
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
        return targetCharacterId;
    }
    public  void AddHaterd(int otherCharacterId, int v)
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
    void OnTriggerEnter(Collider other)
    {
        LCharacterHitDataCmp hitData = other.gameObject.GetComponent<LCharacterHitDataCmp>();
        if (hitData != null)
        {
            if (!hitData.data.hittedObject.Contains(roleId)&&null != hitData.data.value)
            {
                hitData.data.hittedObject.Add(roleId);
                Vector3 dir = other.transform.forward;
                dir.y = 0;
                dir.Normalize();
                charactor.OnHit( other, hitData.data, dir, ref curAction, actionList, information);
            }
        }
 
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
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
    public float SkillCDTime(string skillId)
    {
        return 0f;
    }
}
