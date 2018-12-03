using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCore;

[ExecuteInEditMode]
public class VirtualLight : MonoBehaviour {
    Transform mOwnTran;
    static readonly int DirectionLightDir0 = Shader.PropertyToID("DirectionLightDir0");
    static readonly int DirectionLightDir1 = Shader.PropertyToID("DirectionLightDir1");
    static readonly int DirectionLightDir2 = Shader.PropertyToID("DirectionLightDir2");

    static readonly int DirectionLightColor0 = Shader.PropertyToID("DirectionLightColor0");
    static readonly int DirectionLightColor1 = Shader.PropertyToID("DirectionLightColor1");
    static readonly int DirectionLightColor2 = Shader.PropertyToID("DirectionLightColor2");

    static readonly int DirectionLightIntensity0 = Shader.PropertyToID("DirectionLightIntensity0");
    static readonly int DirectionLightIntensity1 = Shader.PropertyToID("DirectionLightIntensity1");
    static readonly int DirectionLightIntensity2 = Shader.PropertyToID("DirectionLightIntensity2");

    static readonly int LightDir0 = Shader.PropertyToID("LightDir0");
    static readonly int LightDir1 = Shader.PropertyToID("LightDir1");
    static readonly int LightDir2 = Shader.PropertyToID("LightDir2");

    static readonly int LightColor0 = Shader.PropertyToID("LightColor0");
    static readonly int LightColor1 = Shader.PropertyToID("LightColor1");
    static readonly int LightColor2 = Shader.PropertyToID("LightColor2");

    static readonly int LightIntensity0 = Shader.PropertyToID("LightIntensity0");
    static readonly int LightIntensity1 = Shader.PropertyToID("LightIntensity1");
    static readonly int LightIntensity2 = Shader.PropertyToID("LightIntensity2");

    static readonly int Envirment_Cubemap = Shader.PropertyToID("Envirment_Cubemap");

    static readonly int NegativeLightColor0 = Shader.PropertyToID("NegativeLightColor0");
    static readonly int NegativeLightIntensity = Shader.PropertyToID("NegativeLightIntensity");

    static readonly int GroundEnvirment = Shader.PropertyToID("GroundEnvirment");
    static readonly int GroundEnvirWithoutSky = Shader.PropertyToID("GroundEnvirWithoutSky");

    static readonly int ShaddowAlpha = Shader.PropertyToID("ShadowAlpha");
	//_ShadowPower
    public int Index = 0;
    

    [Color("灯光颜色")]
    public Color color = Color.white;

    [Slider("灯光强度", 0, 10)]
    public float Intensity = 1;

    [Color("灯光反向颜色")]
    public Color mNegativeLightColor0 = Color.black;

    [Slider("灯光反向强度", 0, 10)]
    public float mNegativeLightIntensity = 0;

    [Color("天空颜色"),OnValueChanged("SkyChange")]
    public Color mEnvirment_Skycolor = Color.black;

    [Color("中间颜色"), OnValueChanged("SkyChange")]
    public Color mEnvirment_MidColor = Color.black;

    [Color("地面颜色"), OnValueChanged("SkyChange")]
    public Color mEnvirment_GroundColor = Color.white;

	[Slider("阴影alpha", 0, 1)]
	public float shaddowAlpha = 0.8f;
    public void SkyChange()
    {
        OnResetCubemap();
    }

    //[Button("reset cubemap"), Click("OnResetCubemap")]
    //public string resetCubemap;

    public Cubemap mCubemap = null;

    public void OnResetCubemap()
    {
        if (Index > 0)
            return;

        if (mCubemap == null)
            mCubemap = new Cubemap(2, TextureFormat.RGB24, false);

        Color[] xzPixels = new Color[4] { mEnvirment_MidColor, mEnvirment_MidColor, mEnvirment_MidColor, mEnvirment_MidColor };
        Color[] yPositivePixels = new Color[4] { mEnvirment_Skycolor, mEnvirment_Skycolor, mEnvirment_Skycolor, mEnvirment_Skycolor };
        Color[] yNavigatePixels = new Color[4] { mEnvirment_GroundColor, mEnvirment_GroundColor, mEnvirment_GroundColor, mEnvirment_GroundColor };

        mCubemap.SetPixels(xzPixels, CubemapFace.NegativeX, 0);
        mCubemap.SetPixels(xzPixels, CubemapFace.PositiveX, 0);
        mCubemap.SetPixels(xzPixels, CubemapFace.NegativeZ, 0);
        mCubemap.SetPixels(xzPixels, CubemapFace.PositiveZ, 0);

        mCubemap.SetPixels(yPositivePixels, CubemapFace.PositiveY, 0);
        mCubemap.SetPixels(yNavigatePixels, CubemapFace.NegativeY, 0);


        mCubemap.Apply();
    }

    // Use this for initialization
    void Start () {
        mOwnTran = this.transform;

        OnResetCubemap();
	}
	
	// Update is called once per frame
	void Update () {

#if UNITY_EDITOR
        if (Application.isPlaying)
            Shader.DisableKeyword("UseUnityShadow");
        else
            Shader.EnableKeyword("UseUnityShadow");

		//Shader.DisableKeyword("UseUnityShadow");
#endif
		Shader.SetGlobalFloat (ShaddowAlpha, shaddowAlpha);
        switch (Index)
        {
            case 0:
                Shader.SetGlobalVector(DirectionLightDir0, -mOwnTran.forward);
                Shader.SetGlobalVector(DirectionLightColor0, color);
                Shader.SetGlobalFloat(DirectionLightIntensity0, Intensity * 2);

//                 Shader.SetGlobalVector(LightDir0, mOwnTran.forward);
//                 Shader.SetGlobalVector(LightColor0, color);
//                 Shader.SetGlobalFloat(LightIntensity0, Intensity);

                Shader.SetGlobalVector(NegativeLightColor0, mNegativeLightColor0);
                Shader.SetGlobalFloat(NegativeLightIntensity, mNegativeLightIntensity);

                Shader.SetGlobalTexture(Envirment_Cubemap, mCubemap);


                float ndotl = Vector3.Dot(Vector3.down, transform.forward);

                Vector3 vLightColor = new Vector3(color.r, color.g, color.b);
                Vector3 vEnvirmentSkyVec = new Vector3(mEnvirment_Skycolor.r, mEnvirment_Skycolor.g, mEnvirment_Skycolor.b);
                Vector3 vGroundColor = vEnvirmentSkyVec + vLightColor * ndotl * this.Intensity;

                Shader.SetGlobalVector(GroundEnvirment, vGroundColor);
                Shader.SetGlobalVector(GroundEnvirWithoutSky, vLightColor * ndotl * this.Intensity);

                break;
            case 1:
                Shader.SetGlobalVector(DirectionLightDir1, -mOwnTran.forward);
                Shader.SetGlobalVector(DirectionLightColor1, color);
                Shader.SetGlobalFloat(DirectionLightIntensity1, Intensity * 2);

                Shader.SetGlobalVector(LightDir1, mOwnTran.forward);
                Shader.SetGlobalVector(LightColor1, color);
                Shader.SetGlobalFloat(LightIntensity1, Intensity * 2);
                break;
            case 2:
                Shader.SetGlobalVector(DirectionLightDir2, -mOwnTran.forward);
                Shader.SetGlobalVector(DirectionLightColor2, color);
                Shader.SetGlobalFloat(DirectionLightIntensity2, Intensity * 2);

                Shader.SetGlobalVector(LightDir2, mOwnTran.forward);
                Shader.SetGlobalVector(LightColor2, color);
                Shader.SetGlobalFloat(LightIntensity2, Intensity * 2);
                break;
        }
	}
}
