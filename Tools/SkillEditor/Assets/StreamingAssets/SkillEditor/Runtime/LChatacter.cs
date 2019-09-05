using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LChatacter : MonoBehaviour {

    //如果是程序另行加载角色，请将其作为本对象的子节点。并且localPosition 为 0 0 0 
    [Header("角色")]
    public Animation animCtrl;
    public static SkillResourceLoader loader;

    public virtual void CheckLoader()
    {
        
    }

    public List<LChatacterAction> actionList = new List<LChatacterAction>();
    //当前行为.
    public LChatacterAction curAction;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (null != curAction)
        {
            //curAction.doAction();
        }
		
	}
}
