using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LChatacter))]
public class ActionWalk : MonoBehaviour
{
    public string animName = "Run";
    public int priority = 10;
   
    void Start()
    {

        LChatacter chatacter = GetComponent<LChatacter>();
        var a = new LCharacterWalkAction();
        a.priority = priority;
        a.animName = animName;
        chatacter.AddAction(a);

    }
}
