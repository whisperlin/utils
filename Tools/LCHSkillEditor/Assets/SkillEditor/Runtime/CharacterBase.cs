using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharacterBase : MonoBehaviour
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
    public LCharacterInterface character;

 
 
    public void RemoveAciton(LCharacterAction action)
    {
        action.onRelease();
        actionList.Remove(action);
    }

    

    protected virtual void CheckInterface()
    {
        if (null == character)
        {
            Debug.LogError("character not set");
        }
        if (null == loader)
            loader = LChatacterRecourceLoader.Instance();
    }
    public void AddAction(LCharacterAction action)
    {
 
        StartCoroutine(action.onInit(loader, character, AddCoroutine));
        actionList.Add(action);
    }
    List<LCharacterAction> actionList = new List<LCharacterAction>();
    //当前行为.
    public LCharacterAction curAction;

    protected CampInformation campInformation;
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
    void Start()
    {
        CheckInterface();
        if (null == animCtrl)
            animCtrl = gameObject.GetComponentInChildren<Animation>();
        information.AddCharacter(character);
        UpdateColliderLayer();
    }

    void AddCoroutine(IEnumerator fun)
    {
        StartCoroutine(fun);
    }

    protected virtual void OnUpdate()
    {
        //UpdateSkillParams();
        
    }
    void Update()
    {
        OnUpdate();
        UpdateTrigger();
        LCharacterAction.UpdateAction(ref curAction, actionList, character, information);
    }

 
     
    LCharacterDictionary<LCharacterHitDataCmp> triggers = new LCharacterDictionary<LCharacterHitDataCmp>();
    void OnTriggerEnter(Collider other)
    {

        LCharacterHitDataCmp hitData = other.gameObject.GetComponent<LCharacterHitDataCmp>();
        if (hitData != null)
        {
            //Debug.LogError("OnTriggerEnter " + other.name);
            LCharacterAction.OnHit(other, hitData.data, ref curAction, actionList, character, information);
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
            LCharacterAction.OnHit(v._collider, v.data, ref curAction, actionList, character, information);
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
        for (int i = triggers.list.Count - 1; i >= 0; i--)
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


   
  

     
}

 