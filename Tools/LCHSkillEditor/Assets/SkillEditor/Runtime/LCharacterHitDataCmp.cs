using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LCharacterHitDataCmp : MonoBehaviour {

    public int id;
    public LCharacterColliderData data = new LCharacterColliderData();
    public Collider _collider;
    public GameObject _colliderObj;
}
