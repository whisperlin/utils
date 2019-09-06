using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LChatacter))]
public class AIHunt : MonoBehaviour
{
    public string animName = "Run";
    public int priority = 50;
    public float speed = 5f;
    void Start()
    {

        LChatacter chatacter = GetComponent<LChatacter>();
        chatacter.EnableAI();
        var a = new LChatacterHunt();
        a.priority = priority;
        a.animName = animName;
        a.speed = speed;
        chatacter.AddAction(a);

    }
}


 
