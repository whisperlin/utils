using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class LightMark : MonoBehaviour {

	public GameObject Light0 = null;
	public GameObject Light1= null;
	public GameObject PhongLighg= null;

	private Color LightColor0 = Color.white;
	private Color LightColor1 = Color.white;
	private Color LightColor2 = Color.white;

	private Vector3 LightPos0 = Vector3.zero;
	private Vector3 LightPos1 = Vector3.zero;
	private Vector3 LightPos2 = Vector3.zero;
	private Vector3 LightPos2postion  = Vector3.zero;


	private float LightIntensity0 = 0.0f;
	private float LightIntensity1 = 0.0f;
	private float LightIntensity2 = 0.0f;

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
		if (Light0 != null) {
			LightIntensity0 = Light0.GetComponent<Light>().intensity;
			LightColor0 = Light0.GetComponent<Light>().color;
			LightPos0 = Light0.GetComponent<Light>().transform.forward;
		}
		 
		if (Light1 != null) {
			LightIntensity1 = Light1.GetComponent<Light>().intensity;
			LightColor1 = Light1.GetComponent<Light>().color;
			LightPos1 = Light1.GetComponent<Light>().transform.forward;
		}  
		if (PhongLighg != null ) {
			LightIntensity2 = PhongLighg.GetComponent<Light>().intensity;
			LightColor2 = PhongLighg.GetComponent<Light>().color;
			LightPos2 = PhongLighg.GetComponent<Light>().transform.forward;
			LightPos2postion = PhongLighg.GetComponent<Light>().transform.position;
		}

 
		Shader.SetGlobalVector("LightDir0",LightPos0 );
		Shader.SetGlobalFloat("LightIntensity0",LightIntensity0);
		Shader.SetGlobalColor ("LightColor0",LightColor0);


		Shader.SetGlobalVector("LightDir1",LightPos1 );
		Shader.SetGlobalFloat("LightIntensity1",LightIntensity1);
		Shader.SetGlobalColor ("LightColor1",LightColor1);



		Shader.SetGlobalVector("LightDir2",LightPos2 );
		Shader.SetGlobalVector("PointLightPosition",LightPos2postion );
		Shader.SetGlobalFloat("LightIntensity2",LightIntensity2);
		Shader.SetGlobalColor ("LightColor2",LightColor2);

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
