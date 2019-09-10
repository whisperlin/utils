using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCharacterHitData
{

    public int characterId;
    public HashSet<int> hittedObject = new HashSet<int>();

    public ObjDictionary value;
    public bool firstHit;//是否击中的第一个对象，用于产生动作停滞。
    public string cdName;
    public int skillState;
    public CdState cdState;
    public string effect = null;
    public GameObject effect_obj = null;
}