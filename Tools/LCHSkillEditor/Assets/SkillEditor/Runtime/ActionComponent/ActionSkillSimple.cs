using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LChatacter))]
public class ActionSkillSimple : MonoBehaviour {

    //行为的优先级
    public int priority = 100;
    public string skillName = "s01";
    public VirtualInput.KeyCode button = VirtualInput.KeyCode.Button0;
    public float cd = 0f;
    // Use this for initialization
    void Start () {

        LChatacter chatacter = GetComponent<LChatacter>();
        LCharacterSkillAction a = new LCharacterSkillAction();

        CDParams _params = new CDParams();
        CDData[] cds = new CDData[1] { new CDData()};
        cds[0].cd = cd;
        _params.cds = cds;
        string cdName = "cd_" + skillName;
        chatacter.AddParam(cdName, _params);

        a.SkillId = skillName;
        a.priority = priority;
        a.button = button;
        a.skillState = 0;
        a.cdState = CdState.NORMAL;
        a.cdName = cdName;
        chatacter.AddAction(a);

    }

     
}
