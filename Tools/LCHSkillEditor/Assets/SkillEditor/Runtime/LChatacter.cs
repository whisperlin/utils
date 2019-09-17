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
        if (cds.ContainsKey(cdName))
        {
            var c  = cds[cdName];
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
    public void RemoveAciton(LCharacterAction action)
    {
        action.onRelease();
        actionList.Remove(action);
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
    public void AddAction(LCharacterAction action)
    {
        CheckInterface();
        StartCoroutine( action.onInit(loader,this, AddCoroutine));
        actionList.Add(action);
    }
    List<LCharacterAction> actionList = new List<LCharacterAction>();
    //当前行为.
    public LCharacterAction curAction;

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
        if (null == animCtrl)
            animCtrl = gameObject.GetComponentInChildren<Animation>();
        information.AddCharacter(this);
        UpdateColliderLayer();
    }

    void AddCoroutine(IEnumerator fun)
    {
        StartCoroutine(fun);
    }

 
    void Update () {
        UpdateCDParams();
        UpdateTrigger();
        LCharacterAction.UpdateAction(ref curAction, actionList, charactor, information);
	}

    public void CrossFade(string anim_name,float time = 0.05f)
    {
        if(null != animCtrl)
            animCtrl.CrossFade(anim_name, time);
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
    
    public void OnHit(Collider other, LCharacterColliderData hitData, Vector3 dir, ref LCharacterAction curAction, List<LCharacterAction> actionList , LChatacterInformationInterface information)
    {
 
        LCharacterAction.OnHit(other, hitData, ref curAction, actionList, this, information);

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
    LCharacterDictionary<LCharacterHitDataCmp> triggers = new LCharacterDictionary<LCharacterHitDataCmp>();
    void OnTriggerEnter(Collider other)
    {
       
        LCharacterHitDataCmp hitData = other.gameObject.GetComponent<LCharacterHitDataCmp>();
        if (hitData != null)
        {
            //Debug.LogError("OnTriggerEnter " + other.name);
            LCharacterAction.OnHit(other, hitData.data, ref curAction, actionList, this, information);
            hitData._collider = other;
            hitData._colliderObj = other.gameObject;
            triggers.Add(hitData._collider.GetInstanceID(), hitData);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        LCharacterHitDataCmp v;
        if (triggers.dictionary.TryGetValue(other.GetInstanceID(), out v))//避免 GetComponent每帧产生的gc。
        {
            LCharacterAction.OnHit(v._collider, v.data, ref curAction, actionList, this, information);
        }
            //LChatacterAction.OnHit(v._collider, v.data, ref curAction, actionList, this, information);
        //Debug.LogError("OnTriggerStay"+other.name);
    }
    private void OnTriggerExit(Collider other)
    {
        triggers.Remove(other.GetInstanceID());
        //Debug.LogError("OnTriggerExit ");
    }
    void UpdateTrigger()
    {
        for (  int i = triggers.list.Count - 1; i >=0 ; i--)
        {
            var v = triggers.list[i];
            if (!v._colliderObj.activeSelf)
            {
                // Debug.LogError("remove");
                //因为 OnTriggerExit有时很草蛋的不触发，用这个弥补
                triggers.Remove(v._colliderObj.GetInstanceID()); 
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
}
