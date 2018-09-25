using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	private Renderer rend;

	void OnEnable()
	{
        #if UNITY_EDITOR
		UnityEditor.EditorApplication.update += Update;
#endif
    }

	void Start () {
	//	Light0.GetComponent<Light>().intensity;
	//	Light1.GetComponent<Light>().intensity;
	//	PhongLighg.GetComponent<Light>().intensity;
		//float a = Light0.inten
	

		//this.transform = 

	}
	
	// Update is called once per frame
	void Update () {		
		rend = gameObject.GetComponent<Renderer> ();
		LightIntensity0 = Light0.GetComponent<Light>().intensity;
		LightIntensity1 = Light1.GetComponent<Light>().intensity;
		LightIntensity2 = PhongLighg.GetComponent<Light>().intensity;

		LightColor0 = Light0.GetComponent<Light>().color;
		LightColor1 = Light1.GetComponent<Light>().color;
		LightColor2 = PhongLighg.GetComponent<Light>().color;

		LightPos0 = Light0.transform.forward;
		foreach(Material mat in rend.sharedMaterials)
		{
			mat.SetVector("LightDir0",LightPos0 );
			mat.SetFloat("LightIntensity0",LightIntensity0);
			mat.SetColor ("LightColor0",LightColor0);

			LightPos1 = Light1.transform.forward;
			mat.SetVector("LightDir1",LightPos1 );
			mat.SetFloat("LightIntensity1",LightIntensity1);
			mat.SetColor ("LightColor1",LightColor1);

			LightPos2 = PhongLighg.transform.forward;
			LightPos2postion = PhongLighg.transform.position;
			mat.SetVector("LightDir2",LightPos2 );
			mat.SetVector("PointLightPosition",LightPos2postion );
			mat.SetFloat("LightIntensity2",LightIntensity2);
			mat.SetColor ("LightColor2",LightColor2);

		}

	}

	void OnDisable()
	{
		 #if UNITY_EDITOR
		UnityEditor.EditorApplication.update -= Update;
#endif
	}

	void OnDestroy()
	{
		 #if UNITY_EDITOR
		UnityEditor.EditorApplication.update -= Update;
#endif
	}
}
