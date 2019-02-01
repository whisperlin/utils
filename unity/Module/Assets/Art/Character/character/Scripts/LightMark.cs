using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class LightMark : MonoBehaviour {

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
	void Update () {	
	    if (Application.isPlaying == false)
        {
            LightRuning();
        }
        if (runingmode == true){
        	LightRuning();
        }
	}

    void OnEnable()
    {
        LightRuning();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            LightRuning();
        }
    }

    void LightRuning(){
		rend = gameObject.GetComponent<Renderer> ();
		foreach(Material mat in rend.sharedMaterials)
		 {

            //rend = gameObject.GetComponent<Renderer>();
            if (Light0 != null){
            LightIntensity0 = Light0.GetComponent<Light>().intensity;       
            LightColor0 = Light0.GetComponent<Light>().color;
            LightPos0 = Light0.transform.forward;
         mat.SetVector("LightDir0", LightPos0);
          mat.SetFloat("LightIntensity0", LightIntensity0);
          mat.SetColor("LightColor0", LightColor0);
    }

            if (Light1 != null){
            LightPos1 = Light1.transform.forward;
            LightIntensity1 = Light1.GetComponent<Light>().intensity;
            LightColor1 = Light1.GetComponent<Light>().color;
          mat.SetVector("LightDir1", LightPos1);
          mat.SetFloat("LightIntensity1", LightIntensity1);
          mat.SetColor("LightColor1", LightColor1);
    }

            if (PhongLighg != null){
            LightIntensity2 = PhongLighg.GetComponent<Light>().intensity;      
            LightColor2 = PhongLighg.GetComponent<Light>().color;
            LightPos2 = PhongLighg.transform.forward;
           // LightPos2postion = PhongLighg.transform.position;
          mat.SetVector("LightDir2", LightPos2);
           //  rend.sharedMaterials.SetVector("PointLightPosition", LightPos2postion);
          mat.SetFloat("LightIntensity2", LightIntensity2);
          mat.SetColor("LightColor2", LightColor2); 
        }         
          if (Pointlight1 != null){
			var _light = Pointlight1.GetComponent<Light> ();
            LightIntensity3 = Pointlight1.GetComponent<Light>().intensity;      
            LightColor3 = Pointlight1.GetComponent<Light>().color;
            LightPos3 = Pointlight1.transform.forward;
            LightPos3postion = Pointlight1.transform.position;
			float _range = _light.range;
           //  rend.sharedMaterials.SetVector("LightDir3", LightPos3);
          mat.SetVector("PointLightPosition3", LightPos3postion);
          mat.SetFloat("LightIntensity3", LightIntensity3);
          mat.SetColor("LightColor3", LightColor3); 
			 mat.SetFloat("LightRange3", _range); 
        }         
          if (Pointlight2 != null){
			var _light = Pointlight2.GetComponent<Light> ();
            LightIntensity4 = Pointlight2.GetComponent<Light>().intensity;      
            LightColor4 = Pointlight2.GetComponent<Light>().color;
            LightPos4 = Pointlight2.transform.forward;
            LightPos4postion = Pointlight2.transform.position;
			float _range = _light.range;
           //  rend.sharedMaterials.SetVector("LightDir4", LightPos4);
          mat.SetVector("PointLightPosition4", LightPos4postion);
          mat.SetFloat("LightIntensity4", LightIntensity4);
          mat.SetColor("LightColor4", LightColor4); 
			mat.SetFloat("LightRange4", _range); 
        }         





		}
	}

	
}