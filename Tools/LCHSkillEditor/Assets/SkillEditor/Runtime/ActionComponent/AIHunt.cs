using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LChatacter))]
public class AIHunt : MonoBehaviour
{
    public string animName = "Run";
    public int priority = 50;
    public float speed = 5f;
    LChatacter chatacter;
    LCharacterHunt a;

    private void OnEnable()
    {
        chatacter = GetComponent<LChatacter>();
        chatacter.EnableAI();
        a = new LCharacterHunt();
        a.priority = priority;
        a.animName = animName;
        a.speed = speed;
        chatacter.AddAction(a);
    }
    private void OnDisable()
    {
        chatacter.RemoveAciton(a);
  
    }
}


 
