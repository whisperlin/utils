using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LChatacter))]
public class AISkill : MonoBehaviour
{

    //行为的优先级
    public int priority = 100;
    public string skillName = "s01";
    
    // Use this for initialization
    void Start()
    {
        LChatacter chatacter = GetComponent<LChatacter>();
        chatacter.EnableAI();
        LCharacterSkillAI a = new LCharacterSkillAI();
        a.SkillId = skillName;
        a.priority = priority;
 
        chatacter.AddAction(a);

    }



}


 