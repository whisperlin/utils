using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LChatacter))]
public class ActionSkill : MonoBehaviour {

    //行为的优先级
    public int priority = 100;
    public string skillName = "s01";
    public VirtualInput.KeyCode button = VirtualInput.KeyCode.Button0;
    // Use this for initialization
    void Start () {

        LChatacter chatacter = GetComponent<LChatacter>();
        LCharacterSkillAction a = new LCharacterSkillAction();
        a.SkillId = skillName;
        a.priority = priority;
        a.button = button;
        chatacter.AddAction(a);

    }

     
}
