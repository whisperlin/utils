using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class LightInformation : MonoBehaviour {

    public Light DirectionalLight;
    public Light PointLight;
    public Light Spotlight;

    public bool updateInRuntime = false;

    void UpdateLightData()
    {
        if (DirectionalLight)
        {
            Shader.SetGlobalColor("DirectionalLightColor", DirectionalLight.color);
            Shader.SetGlobalVector("DirectionalLightDir", DirectionalLight.transform.forward);
            Shader.SetGlobalFloat("DirectionalLightIntensity", DirectionalLight.intensity);
        }
        else
        {
            Shader.SetGlobalFloat("DirectionalLightIntensity", 0);
        }
        if (PointLight)
        {

            Shader.SetGlobalColor("PointLightColor", PointLight.color);
            Shader.SetGlobalVector("PointLightPosition", PointLight.transform.position);
            Shader.SetGlobalFloat("PointLightRange", PointLight.range);
            Shader.SetGlobalFloat("PointLightIntensity", PointLight.intensity);
        }
        else
        {
            Shader.SetGlobalFloat("PointLightIntensity", 0);
        }
        if (Spotlight)
        {
            Shader.SetGlobalColor("SpotlightColor", Spotlight.color);
            Shader.SetGlobalFloat("SpotlightSpotAngle0", Mathf.Cos(Spotlight.spotAngle * Mathf.Deg2Rad / 2.0f));
            Shader.SetGlobalFloat("SpotlightSpotAngle1", Mathf.Cos(Spotlight.spotAngle * Mathf.Deg2Rad / 4.0f));
            Shader.SetGlobalVector("SpotLightPosition", Spotlight.transform.position);
            Shader.SetGlobalVector("SpotDirection", Spotlight.transform.forward);
            Shader.SetGlobalFloat("SpotLightIntensity", Spotlight.intensity);
            Shader.SetGlobalFloat("SpotLightRange", Spotlight.range);
        }
        else
        {
            Shader.SetGlobalFloat("SpotLightRange", 0);
        }
    }
	// Use this for initialization
	void Start () {
        UpdateLightData();

    }
	
	// Update is called once per frame
	void Update () {
        if(updateInRuntime)
            UpdateLightData();
    }
}
