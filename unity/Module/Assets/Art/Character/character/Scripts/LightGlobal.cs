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
        public GameObject Pointlight1;
                public GameObject Pointlight2;

    private Color LightColor0;
    private Color LightColor1;
    private Color LightColor2;
      private Color LightColor3;
        private Color LightColor4;

    private Vector3 LightPos0;
    private Vector3 LightPos1;
    private Vector3 LightPos2;
       private Vector3 LightPos3;
          private Vector3 LightPos4;

    private Vector3 LightPos3postion;
    private Vector3 LightPos4postion;
    private float LightIntensity0;
    private float LightIntensity1;
    private float LightIntensity2;
    private float LightIntensity3;
    private float LightIntensity4;

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
        if (runingmode == true){
        	LightRuning();
        }
    }

    void LightRuning()
    {

            //rend = gameObject.GetComponent<Renderer>();
            if (Light0 != null){
            LightIntensity0 = Light0.GetComponent<Light>().intensity;       
            LightColor0 = Light0.GetComponent<Light>().color;
            LightPos0 = Light0.transform.forward;
            Shader.SetGlobalVector("LightDir0", LightPos0);
            Shader.SetGlobalFloat("LightIntensity0", LightIntensity0);
            Shader.SetGlobalColor("LightColor0", LightColor0);
    }

            if (Light1 != null){
            LightPos1 = Light1.transform.forward;
            LightIntensity1 = Light1.GetComponent<Light>().intensity;
            LightColor1 = Light1.GetComponent<Light>().color;
            Shader.SetGlobalVector("LightDir1", LightPos1);
            Shader.SetGlobalFloat("LightIntensity1", LightIntensity1);
            Shader.SetGlobalColor("LightColor1", LightColor1);
    }

            if (PhongLighg != null){
            LightIntensity2 = PhongLighg.GetComponent<Light>().intensity;      
            LightColor2 = PhongLighg.GetComponent<Light>().color;
            LightPos2 = PhongLighg.transform.forward;
           // LightPos2postion = PhongLighg.transform.position;
            Shader.SetGlobalVector("LightDir2", LightPos2);
           // Shader.SetGlobalVector("PointLightPosition", LightPos2postion);
            Shader.SetGlobalFloat("LightIntensity2", LightIntensity2);
            Shader.SetGlobalColor("LightColor2", LightColor2); 
        }         
          if (Pointlight1 != null){
			var _light = Pointlight1.GetComponent<Light> ();
            LightIntensity3 = Pointlight1.GetComponent<Light>().intensity;      
            LightColor3 = Pointlight1.GetComponent<Light>().color;
            LightPos3 = Pointlight1.transform.forward;
            LightPos3postion = Pointlight1.transform.position;
			float _range = _light.range;
           // Shader.SetGlobalVector("LightDir3", LightPos3);
            Shader.SetGlobalVector("PointLightPosition3", LightPos3postion);
            Shader.SetGlobalFloat("LightIntensity3", LightIntensity3);
            Shader.SetGlobalColor("LightColor3", LightColor3); 
			Shader.SetGlobalFloat("LightRange3", _range); 
        }         
          if (Pointlight2 != null){
			var _light = Pointlight2.GetComponent<Light> ();
            LightIntensity4 = Pointlight2.GetComponent<Light>().intensity;      
            LightColor4 = Pointlight2.GetComponent<Light>().color;
            LightPos4 = Pointlight2.transform.forward;
            LightPos4postion = Pointlight2.transform.position;
			float _range = _light.range;
           // Shader.SetGlobalVector("LightDir4", LightPos4);
            Shader.SetGlobalVector("PointLightPosition4", LightPos4postion);
            Shader.SetGlobalFloat("LightIntensity4", LightIntensity4);
            Shader.SetGlobalColor("LightColor4", LightColor4); 
			Shader.SetGlobalFloat("LightRange4", _range); 
        }         
    }
}