using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LCharacter))]
public class AIHunt : MonoBehaviour
{
    public string animName = "Run";
    public int priority = 50;
    public float speed = 5f;

    public float mixDistance = 0.8f;
    LCharacter chatacter;
    LCharacterHunt a;

    private void OnEnable()
    {
        chatacter = GetComponent<LCharacter>();
        chatacter.EnableAI();
        a = new LCharacterHunt();
        a.priority = priority;
        a.animName = animName;
        a.speed = speed;
        a.mixDistance = mixDistance;
        chatacter.AddAction(a);
    }
    private void OnDisable()
    {
        chatacter.RemoveAciton(a);
  
    }
}


 
