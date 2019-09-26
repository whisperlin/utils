using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LCharacter))]
public class ActionIdle : MonoBehaviour {

    public string animName = "Idle";
    public int priority = 5;
    void Start()
    {

        LCharacter chatacter = GetComponent<LCharacter>();
        var a = new LCharactorIdleAction();
        a.animName = animName;
        a.priority = priority;
        chatacter.AddAction(a);

    }
}
