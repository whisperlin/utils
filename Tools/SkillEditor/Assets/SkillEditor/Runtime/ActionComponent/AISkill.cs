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
    public float cd = 0f;
    LChatacter chatacter;
    LCharacterSkillAI a = new LCharacterSkillAI();
    string cdName = "";
    // Use this for initialization
    void Start()
    {
        

    }
    private void OnEnable()
    {
        chatacter = GetComponent<LChatacter>();
        chatacter.EnableAI();


        CDParams _params = new CDParams();
        CDData[] cds = new CDData[1] { new CDData() };
        cds[0].cd = cd;
        _params.cds = cds;
        cdName = "cd_" + skillName;
        chatacter.AddParam(cdName, _params);

 
        a.SkillId = skillName;
        a.priority = priority;

        a.skillState = 0;
        a.cdState = CdState.NORMAL;
        a.cdName = cdName;
        chatacter.AddAction(a);
    }
    private void OnDisable()
    {
        chatacter.RemoveAciton(a);
        chatacter.RemoveParam(cdName);
    }



}


 