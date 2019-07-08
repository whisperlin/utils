using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DARainPreview : MonoBehaviour {
    private static float rdDefine = 0.8f;
    private static float mlDefine = 0.74f;
    private static float phDefine = 1.0f;


    private Vector4 Vec_RD = new Vector4(mlDefine, rdDefine, 0, 0);

    [Range(0, 1)]
    public float _raodong = rdDefine;
    private float __raodong = rdDefine;
    private float raodong
    {
        get
        {
            Debug.Log("OnEnable");
            return __raodong;
        }

        set
        {
            __raodong = value;
            Vec_RD.y = __raodong;
            Shader.SetGlobalVector("weather_intensity", Vec_RD);
        }
   }

    [Range(0, 1)]
    public float _mingliang = mlDefine;
    private float __mingliang = mlDefine;
    private float mingliang
    {
        get
        {
            Debug.Log("OnEnable");
            return __mingliang;
        }

        set
        {
            __mingliang = value;
            Vec_RD.x = __mingliang;
            Shader.SetGlobalVector("weather_intensity", Vec_RD);
        }
    }

    [Range(0, 10f)]
    public float _pinghuaRate = phDefine;
    private float __pinghuaRate = phDefine;
    private float pinghuaRate
    {
        get
        {
            Debug.Log("OnEnable");
            return __pinghuaRate;
        }

        set
        {
            __pinghuaRate = value;
            Shader.SetGlobalFloat("SmoothnessRate", __pinghuaRate);
        }
    }

	// Use this for initialization
	void Start () {
	
	}

    void OnEnable()
    {
        var texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/_Level0/RainFlowControl.png");
        Shader.EnableKeyword("RAIN_ENABLE");
        Shader.SetGlobalVector("weather_intensity", Vec_RD);
        Shader.SetGlobalTexture("_WeatherCtrlTex0", texture);



        Debug.Log("OnEnable");
    }

    void OnDisable()
    {
        Shader.DisableKeyword("RAIN_ENABLE");
        Shader.SetGlobalTexture("_WeatherCtrlTex0", null);
        Debug.Log("OnDisable");
    }

    // Update is called once per frame
    void Update () {
        Shader.SetGlobalFloat("FrameTime", Time.time);

        if (_raodong != __raodong)
        {
            raodong = _raodong;
        }
        if (_mingliang != __raodong)
        {
            mingliang = _mingliang;
        }
        if (_pinghuaRate != __pinghuaRate)
        {
            pinghuaRate = _pinghuaRate;
        }
    }
}
