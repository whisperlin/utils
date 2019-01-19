using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class LightGlobal1 : MonoBehaviour
{

    //#if UNITY_EDITOR
    public GameObject Light0;


    private Color LightColor0;
    

    private Vector3 LightPos0;
 
    private float LightIntensity0;

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

            rend = gameObject.GetComponent<Renderer>();
            if (Light0 != null){
            LightIntensity0 = Light0.GetComponent<Light>().intensity;       
            LightColor0 = Light0.GetComponent<Light>().color;
            LightPos0 = Light0.transform.forward;
			rend.sharedMaterial.SetVector("LightDir0", LightPos0);
			rend.sharedMaterial.SetFloat("LightIntensity0", LightIntensity0);
			rend.sharedMaterial.SetColor("LightColor0", LightColor0);
    }

    }
}