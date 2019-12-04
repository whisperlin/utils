using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LCharacter))]
public class ActionWalk : MonoBehaviour
{
    public string animName = "FightRun";
    public int priority = 10;
    public float speed = 10f;


    void Start()
    {

        LCharacter chatacter = GetComponent<LCharacter>();
        var a = new LCharacterWalkAction();
        a.priority = priority;
        a.speed = speed;
        a.animName = animName;
        chatacter.AddAction(a);

    }
}
