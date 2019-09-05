using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LChatacter : MonoBehaviour, LChatacterInterface
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
            //collders[i].isTrigger = true;
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
    // Use this for initialization
    void Start () {
        UpdateColliderLayer();
    }

    

    void AddCoroutine(IEnumerator fun)
    {
        StartCoroutine(fun);
    }

 
    void Update () {
 
        //CheckInterface();
        LChatacterAction.UpdateAction(ref curAction, actionList, charactor, information);
        if (null != curAction)
        {
            curAction.doAction(charactor, information);
        }
		
	}

    public void CrossFade(string anim_name)
    {
        if(null != animCtrl)
            animCtrl.CrossFade(anim_name);
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

    void OnTriggerEnter(Collider other)
    {
 
        LCharacterHitData hitData = other.gameObject.GetComponent<LCharacterHitData>();
        if (hitData != null)
        {
            if (!hitData.hittedObject.Contains(roleId))
            {
                hitData.hittedObject.Add(roleId);
                Vector3 dir = other.transform.forward;
                dir.y = 0;
                dir.Normalize();
                LChatacterAction.OnHit(other, hitData.value, dir, ref curAction, actionList, charactor, information);

            }
            else
            {
                //Debug.Log("has hit");
            }
        }
        else
        {
            //Debug.Log("hitData  null"  );
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
