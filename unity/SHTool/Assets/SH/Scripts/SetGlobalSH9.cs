using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetGlobalSH9 : MonoBehaviour {

    public SH9Data data;
    SH9Data curData;

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
        if (data != null && curData != data )
        {
            curData = data;
            for (int i = 0; i < 9; ++i)
            {
                string param = "g_sph" + i.ToString();
                Shader.SetGlobalVector(param, data.coefficients[i]);
            }
        }
    }
}
