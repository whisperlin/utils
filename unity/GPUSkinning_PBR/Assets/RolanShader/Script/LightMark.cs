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
	void Update () {	
	if (Application.isPlaying == false)
        {
            LightRuning();
        }
        if (runingmode == true){
        	LightRuning();
        }
	}

	 void LightRuning(){
		rend = gameObject.GetComponent<Renderer> ();
		foreach(Material mat in rend.sharedMaterials)
		{
			if (Light0 !=null){
			LightPos0 = Light0.transform.forward;
			LightIntensity0 = Light0.GetComponent<Light>().intensity;
			mat.SetVector("LightDir0",LightPos0 );
			mat.SetFloat("LightIntensity0",LightIntensity0);
			mat.SetColor ("LightColor0",LightColor0);
			LightColor0 = Light0.GetComponent<Light>().color;
		}

			if (Light1 !=null){
			LightIntensity1 = Light1.GetComponent<Light>().intensity;
			LightPos1 = Light1.transform.forward;
			mat.SetVector("LightDir1",LightPos1 );
			mat.SetFloat("LightIntensity1",LightIntensity1);
			mat.SetColor ("LightColor1",LightColor1);
			LightColor1 = Light1.GetComponent<Light>().color;
		}

			if (PhongLighg !=null){
			LightIntensity2 = PhongLighg.GetComponent<Light>().intensity;
			LightColor2 = PhongLighg.GetComponent<Light>().color;
			LightPos2 = PhongLighg.transform.forward;
			LightPos2postion = PhongLighg.transform.position;
			mat.SetVector("LightDir2",LightPos2 );
			mat.SetVector("PointLightPosition",LightPos2postion );
			mat.SetFloat("LightIntensity2",LightIntensity2);
			mat.SetColor ("LightColor2",LightColor2);
		 }
		}
	}

	
}