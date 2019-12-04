using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LCharacter))]
public class ActionDie : MonoBehaviour
{

    //行为的优先级
    public int priority = 1000000;

    public string animName = "Die";

    [Header("跳跃水平移动速度.(服务器很可能只管水平的)")]
    public float jumpSpeed = 5f;
    [Header("跳跃高度")]
    public float jumpHeight = 5f;
    [Header("跳跃到达最高点所用时间(跳跃快慢)")]
    //跳跃到达最高点所用时间.
    public float JumpTime = 1f;

    [Header("死亡落地后多久删除")]
    public float destroyTime = 2f;


    
    // Use this for initialization
    LCharacter chatacter;
    LCharacterDieAction a;


     
    private void OnEnable()
    {
        chatacter = GetComponent<LCharacter>();
        a = new LCharacterDieAction();
        a.priority = priority;
 
        a.jumpSpeed = jumpSpeed;
        a.jumpHeight = jumpHeight;
        a.JumpTime = JumpTime;
        a.animName = animName;
        a.destroyTime = destroyTime;
        chatacter.AddAction(a);
        
    }
    private void OnDisable()
    {
        chatacter.RemoveAciton(a);

    }


}
 