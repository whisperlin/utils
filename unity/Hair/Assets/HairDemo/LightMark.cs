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


	private float LightIntensity0 = 1.0f;
	private float LightIntensity1 = 1.0f;
	private float LightIntensity2 = 1.0f;

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
			var _light = Light0.GetComponent<Light> () ;
			if (_light) {
				LightIntensity0 = _light.intensity;
				LightColor0 = _light.color;
				LightPos0 = _light.transform.forward;
			}

		}
		 
		if (Light1 != null) {
			var _light = Light1.GetComponent<Light> () ;
			if (_light) {
				LightIntensity1 = _light.intensity;
				LightColor1 = _light.color;
				LightPos1 = _light.transform.forward;	
			}
		}  
		if (PhongLighg != null ) {
			var _light = PhongLighg.GetComponent<Light> () ;
			if (_light) {
				LightIntensity2 = _light.intensity;
				LightColor2 = _light.color;
				LightPos2 = PhongLighg.transform.forward;
				LightPos2postion = PhongLighg.transform.position;
			}
		}
		 
 


		foreach(Material mat in rend.sharedMaterials)
		{
			mat.SetVector("LightDir0",LightPos0 );
			mat.SetFloat("LightIntensity0",LightIntensity0);
			mat.SetColor ("LightColor0",LightColor0);


			mat.SetVector("LightDir1",LightPos1 );
			mat.SetFloat("LightIntensity1",LightIntensity1);
			mat.SetColor ("LightColor1",LightColor1);



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
