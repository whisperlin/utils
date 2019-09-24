using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGUIInstanceObjWorldPosition : MonoBehaviour {

    MaterialPropertyBlock block;
    int instance_world_position  ;
    public bool alwayUpdate = false;
    MeshRenderer[] meshRenders;
    void initId()
    {
        instance_world_position = Shader.PropertyToID("instance_world_position");
        List<MeshRenderer> mrs = new List<MeshRenderer>();
        for (int i = 0, childCount = transform.childCount; i < childCount; i++)
        {
            var g = transform.GetChild(i).gameObject;
            var r = g.GetComponent<MeshRenderer>();
            if (null == r)
                continue;
            mrs.Add(r);
        }
        meshRenders = mrs.ToArray();
    }
    void UpdateData()
    {
        if( null == block ) block = new MaterialPropertyBlock();
        for (int i = 0, childCount = meshRenders.Length; i < childCount; i++)
        {   
            var r = meshRenders[i];
            if (null == r)
                continue;
            var g = r.gameObject;
            block.SetVector("instance_world_position", g.transform.position);
            r.SetPropertyBlock(block);
        }
    }
	void Start () {
        initId();
        UpdateData();
    }
	
	// Update is called once per frame
	void Update () {

        initId();
        UpdateData();

    }
}
