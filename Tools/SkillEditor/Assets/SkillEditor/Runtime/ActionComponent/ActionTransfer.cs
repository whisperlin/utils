using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LChatacter))]
public class ActionTransfer : MonoBehaviour {

    //行为的优先级
    public int priority = 2000;
    public string animName = "Jump1";

    public float speed = 10f;
    public float height = 5f;

    LChatacter chatacter;
    LCharacterTransfer a;


    private void OnEnable()
    {
        chatacter = GetComponent<LChatacter>();
        a = new LCharacterTransfer();
        a.priority = priority;
        a.jumpSpeed = speed;
        a.jumpHeight = height;
        a.animName = animName;
        chatacter.AddAction(a);
    }
    private void OnDisable()
    {
        chatacter.RemoveAciton(a);

    }
}
