using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCHFellowCharacter : MonoBehaviour {
    public LCharacterInterface character;
    public Vector3 forward;
    public Mesh dirMesh;
    public Mesh pointMesh;
    public MeshFilter filter;
    public MeshRenderer meshRender;

    public SkillParams.TYPE type;

    // Use this for initialization
    void Start () {
		
	}

    internal float skillRange;
    internal GameObject rangeGamObject;
    internal Mesh randMesh;

    // Update is called once per frame
    void LateUpdate () {
        if (null == character)
        {
            Debug.LogError("ret");
            return;
            
        }
        if (type == SkillParams.TYPE.DRAG_DIR)
        {
            transform.position = character.GetCurPosition() + Vector3.up * 0.1f;
            transform.forward = forward;
            
        }
        else
        {
            transform.position = character.GetCurPosition() +  forward + Vector3.up * 0.1f;
             

        }
       
        rangeGamObject.transform.position = character.GetCurPosition() + Vector3.up * 0.1f;
        
    }
}
