using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LCharacter))]
public class ActionFall : MonoBehaviour
{
    public string animName = "Jump2";
    public int priority = 50;
    public float fallSpeed = 5f;
    void Start()
    {

        LCharacter chatacter = GetComponent<LCharacter>();
        var a = new LCharactorFallAction();
        a.priority = priority;
        a.animName = animName;
        a.fallSpeed = fallSpeed;
        chatacter.AddAction(a);

    }
}
 
