using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SetShaderLightInformation : MonoBehaviour {


    [Header("角色虚拟光方向")]
    public Vector3 ShaderDirectLight0 = new Vector3(50f,-30f,0f);
    [Header("地面高度")]
    public float groundHeight = 0f;
    [Header("阴影颜色")]
    public Color shadowColor = new Color(0.2f, 0.2f, 0.4f, 0.4f);

    // Use this for initialization
    void Start () {

        

    }
	
	// Update is called once per frame
	void Update () {

        var v = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(ShaderDirectLight0.x, ShaderDirectLight0.y, ShaderDirectLight0.z)), Vector3.one).MultiplyVector(Vector3.back);
        v.Normalize();
        Shader.SetGlobalVector("PlaneShaderDirectLight0", v);
        Shader.SetGlobalFloat("PlaneShaderHeight", groundHeight);
        Shader.SetGlobalColor("PlaneShadowColor", shadowColor);

    }
}
