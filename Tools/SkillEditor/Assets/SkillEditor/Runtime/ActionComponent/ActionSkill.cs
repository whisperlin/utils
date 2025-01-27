﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LChatacter))]
public class ActionSkill : MonoBehaviour {


    public int priority = 100;

    
    public VirtualInput.KeyCode button = VirtualInput.KeyCode.Button0;
    public CdState state = CdState.NORMAL;
    public string cdName = "cd_attack";
    public CDData[] skills = new CDData[0];
    // Use this for initialization
    void Start () {

        if (skills.Length == 0)
            return;
        LChatacter chatacter = GetComponent<LChatacter>();
        CDParams _params =new CDParams();
        _params.cds = skills;
        chatacter.AddParam(cdName, _params);
        for (int i = 0; i < skills.Length; i++)
        {
            LCharacterSkillAction a = new LCharacterSkillAction();
            a.SkillId = skills[i].skillId;
            a.priority = priority;
            a.button = button;
            a.cdState = state;
            a.cdName = cdName;
            a.skillState = i;
            chatacter.AddAction(a);
        }

    }

	
	 
}
