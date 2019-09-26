using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LCharacter))]
public class ActionJump : MonoBehaviour
{

    //行为的优先级
    public int priority = 55;

    public string animName = "Jump1";

    [Header("跳跃水平移动速度.(服务器很可能只管水平的)")]
    public float jumpSpeed = 5f;
    [Header("跳跃高度")]
    public float jumpHeight = 5f;
    [Header("跳跃到达最高点所用时间(跳跃快慢)")]
    //跳跃到达最高点所用时间.
    public float JumpTime = 1f;

    [Header("连跳段数")]
    public int maxJumpCount = 2;

    public VirtualInput.KeyCode button = VirtualInput.KeyCode.Button9;
    // Use this for initialization
    LCharacter chatacter;
    LCharacterJumpAction a;
 

    private void OnEnable()
    {
        chatacter = GetComponent<LCharacter>();
        a = new LCharacterJumpAction();
        a.priority = priority;
        a.button = button;
        a.jumpSpeed = jumpSpeed;
        a.jumpHeight = jumpHeight;
        a.JumpTime = JumpTime;
        a.animName = animName;
        a.maxJumpCount = maxJumpCount;
        chatacter.AddAction(a);
    }
    private void OnDisable()
    {
        chatacter.RemoveAciton(a);

    }


}
 