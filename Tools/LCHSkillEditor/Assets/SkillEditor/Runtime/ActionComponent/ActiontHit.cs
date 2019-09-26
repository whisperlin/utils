﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LCharacter))]
public class ActiontHit : MonoBehaviour
{

    //行为的优先级
    public int priority = 1000;
    //跳跃到达最高点所用时间.
    public float JumpTime = 1f;

    [Header("是否开启击飞")]
    public bool isHitFly = true;


    public string animHit = "HurtBack";
    public string animHitFly = "HurtBack";
    public string animHitFlyDown = "HurtBack";

    // Use this for initialization
    void Start()
    {
        LCharacter chatacter = GetComponent<LCharacter>();
        {
           
            LCharacterHit a = new LCharacterHit();
            a.priority = priority;
            a.animName = animHit;
            chatacter.AddAction(a);
        }
        {
 
            LCharacterHitBack a = new LCharacterHitBack();
            a.priority = priority;
            a.animName = animHit;
            chatacter.AddAction(a);
        }

        if (isHitFly)
        {
            {
                LCharacterHitFly a = new LCharacterHitFly();
                a.priority = priority;
                a.animNameFly = animHitFly;
                a.animHitFlyDown = animHitFlyDown;
                chatacter.AddAction(a);
            }




            {
                LCharacterHitDown a = new LCharacterHitDown();
                a.priority = priority;
                a.animNameFly = animHitFly;
                a.animHitFlyDown = animHitFlyDown;
                chatacter.AddAction(a);
            }
        }

    }


}
