using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class HitGrass : MonoBehaviour {

    [Header("碰撞半径")]
    public float radius = 1.2f;
    // Use this for initialization
    void Start () {
        
	}
    //_HitData0
    // Update is called once per frame
    void Update () {

       // Shader.EnableKeyword("_FADEPHYSICS_ON");
        //Debug.LogError("_FADEPHYSICS_ON");
        var v = transform.position;
        Shader.SetGlobalVector("_HitData0", new Vector4(v.x,v.y,v.z, radius));

    }
    private void OnDisable()
    {
      // Shader.DisableKeyword("_FADEPHYSICS_ON");
    }
}
