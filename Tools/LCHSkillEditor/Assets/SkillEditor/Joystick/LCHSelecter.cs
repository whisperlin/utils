using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCHSelecter : MonoBehaviour {
    public LCharacterInterface character;
    MeshRenderer mr;
    Material mat;
	// Use this for initialization
	void Start () {
        mr = GetComponent<MeshRenderer>();
        mat = mr.sharedMaterial;
        mr.enabled = false;
    }
    Vector3 delta = new Vector3(0f, 0.05f, 0f);
	// Update is called once per frame
	void LateUpdate () {

        int id = character.GetTargetId();
        if (id < 0)
        {
            mr.enabled = false;
            return;
        }
        LCharacterInterface chr;
        if (CharacterBase.information.TryGetCharacter(id, out chr))
        {
            mr.enabled = true;
            transform.position = chr.GetCurPosition() + delta;
            if (VirtualInput.isTargetting)
            {
                mat.SetFloat("_ctrl", 0.6f); ;
            }
            else
            {
                mat.SetFloat("_ctrl", 1f); ;
            }
        }
        else
        {
            mr.enabled = false;
        }
    }
}
