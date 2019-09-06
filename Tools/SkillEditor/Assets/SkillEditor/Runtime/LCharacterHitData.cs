using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCharacterHitData : MonoBehaviour {

    public int characterId;
    public HashSet<int> hittedObject = new HashSet<int>();

    public ObjDictionary value;
}
