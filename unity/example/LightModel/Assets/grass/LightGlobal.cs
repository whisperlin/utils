using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class LightGlobal : MonoBehaviour
{

    //#if UNITY_EDITOR
    public GameObject Light0;
    public GameObject Light1;
    public GameObject PhongLighg;

    private Color LightColor0;
    private Color LightColor1;
    private Color LightColor2;

    private Vector3 LightPos0;
    private Vector3 LightPos1;
    private Vector3 LightPos2;
    private Vector3 LightPos2postion;

    private float LightIntensity0;
    private float LightIntensity1;
    private float LightIntensity2;

    public bool runingmode;

    private Renderer rend;

    void Start()
    {
        LightRuning();
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying == false)
        {
            LightRuning();
        }
        if (runingmode == true)
        {
            LightRuning();
        }
    }

    void LightRuning()
    {
        //rend = gameObject.GetComponent<Renderer>();
        if (Light0 != null)
        {
            LightIntensity0 = Light0.GetComponent<Light>().intensity;
            LightColor0 = Light0.GetComponent<Light>().color;
            LightPos0 = Light0.transform.forward;
            Shader.SetGlobalVector("LightDir0", LightPos0);
            Shader.SetGlobalFloat("LightIntensity0", LightIntensity0);
            Shader.SetGlobalColor("LightColor0", LightColor0);

        }

        if (Light1 != null)
        {
            LightPos1 = Light1.transform.forward;
            LightIntensity1 = Light1.GetComponent<Light>().intensity;
            LightColor1 = Light1.GetComponent<Light>().color;
            Shader.SetGlobalVector("LightDir1", LightPos1);
            Shader.SetGlobalFloat("LightIntensity1", LightIntensity1);
            Shader.SetGlobalColor("LightColor1", LightColor1);
        }

        if (PhongLighg != null)
        {
            LightIntensity2 = PhongLighg.GetComponent<Light>().intensity;
            LightColor2 = PhongLighg.GetComponent<Light>().color;
            LightPos2 = PhongLighg.transform.forward;
            LightPos2postion = PhongLighg.transform.position;
            Shader.SetGlobalVector("LightDir2", LightPos2);
            Shader.SetGlobalVector("PointLightPosition", LightPos2postion);
            Shader.SetGlobalFloat("LightIntensity2", LightIntensity2);
            Shader.SetGlobalColor("LightColor2", LightColor2);
        }
        
       /* Debug.LogError("=============");
        Debug.LogError(LightPos0.ToString("f3") + " " + LightIntensity0 + " " + (LightColor0 * 255).ToString("f3"));
        Debug.LogError(LightPos1.ToString("f3") + " " + LightIntensity1 + " " + (LightColor1 * 255).ToString("f3"));
        Debug.LogError(LightPos2.ToString("f3") + " " + LightIntensity2 + " " + (LightColor2 * 255).ToString("f3") + " " + LightPos2postion.ToString("f3"));*/
    }
}