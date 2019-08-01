using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetGlobalSH9 : MonoBehaviour {

    public SH9Data data;
    SH9Data curData;
    [Header("角色虚拟光方向")]
    public Vector3 VirtualDirectLight0;

    [Header("场景漫反射差")]
    [Range(0f,1f)]
    public float _DifSC = 0.0f;

    [Header("场景背光")]
    public Color _BackColor = Color.black;

    [Header("场景背光强度")]
    [Range(0f,1f)]
    public float sss_scatter0 = 0.0f;

    [Header("高光剔除")]
    [Range(-0.1f, 0.6f)]
    public float _CullSepe = 0.0f;

    // Use this for initialization
    void Start () {
        curData = null;
        setSH9Global();
    }
	// Update is called once per frame
	void Update () {
        setSH9Global();
    }
    private void setSH9Global()
    {
        //VirtualDirectLight0
        if (data != null && curData != data )
        {
            curData = data;
            for (int i = 0; i < 9; ++i)
            {
                string param = "g_sph" + i.ToString();
                Shader.SetGlobalVector(param, data.coefficients[i]);
            }
        }
        var v = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(VirtualDirectLight0.x, VirtualDirectLight0.y, VirtualDirectLight0.z)), Vector3.one).MultiplyVector(Vector3.back);
        v.Normalize();
        //Vector3.forward
        Shader.SetGlobalVector("VirtualDirectLight0", v);
        Shader.SetGlobalFloat("_DifSC", _DifSC);
        Shader.SetGlobalColor("_BackColor", _BackColor);
        Shader.SetGlobalFloat("sss_scatter0", sss_scatter0);
        Shader.SetGlobalFloat("_CullSepe", _CullSepe);







    }
}
