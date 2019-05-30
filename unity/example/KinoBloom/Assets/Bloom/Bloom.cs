//
// Kino/Bloom v2 - Bloom filter for Unity
//

using UnityEngine;

 
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Bloom : MonoBehaviour
{

	 
    public float thresholdGamma {
        get { return Mathf.Max(_threshold, 0); }
        set { _threshold = value; }
    }

    public float thresholdLinear {
        get { return GammaToLinear(thresholdGamma); }
        set { _threshold = LinearToGamma(value); }
    }


	[Header("颜色剔除值")]
	[Range(0,2)]
	public float _threshold = 0.5f;

	[Header("过渡渐变阈值")]
    [ Range(0, 1)]
	public float softKnee = 0.5f;

	[Header("半径(重复采样次数)")]
    [ Range(1, 5)]
	public int radius = 4;


	[Header("强度")]
	[Range(0,3)]
	public float intensity = 0.8f;

 
	[HideInInspector]
	[System.NonSerialized]
	public int debugMode = 0;


 

     



    [SerializeField, HideInInspector]
    Shader _shader;

    Material _material;

    const int kMaxIterations = 16;
    RenderTexture[] _blurBuffer1 = new RenderTexture[kMaxIterations];
    RenderTexture[] _blurBuffer2 = new RenderTexture[kMaxIterations];

    float LinearToGamma(float x)
    {

        return Mathf.LinearToGammaSpace(x);
   
    }

    float GammaToLinear(float x)
    {
     
        return Mathf.GammaToLinearSpace(x);
    
    }

    

    void OnEnable()
    {
        var shader = _shader ? _shader : Shader.Find("TA/Bloom");
        _material = new Material(shader);
        _material.hideFlags = HideFlags.DontSave;
    }

    void OnDisable()
    {
        DestroyImmediate(_material);
    }

	/*void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		OnEffect (source,destination);
	}*/

	RenderTexture myRenderTexture;
	void OnPreRender()
	{

		myRenderTexture = RenderTexture.GetTemporary(Screen.width,Screen.height,24);
		myRenderTexture.filterMode = FilterMode.Trilinear;
		myRenderTexture.antiAliasing = 2;
		Camera.main.targetTexture = myRenderTexture;
	}
	void OnPostRender()
	{
		Camera.main.targetTexture = null;
		RenderTexture source = myRenderTexture;
		OnEffect (source,  null as RenderTexture);
		RenderTexture.ReleaseTemporary(myRenderTexture);
	}

	const int prefilterPass = 0;
	const int downSample = 1;
	const int upSample = 2;

	const int finalPass = 3;
	const int finalPassGood = 4;
    void OnEffect(RenderTexture source, RenderTexture destination)
    {
        var useRGBM = Application.isMobilePlatform;

        // source texture size
        var tw = source.width;
        var th = source.height;

        // halve the texture size for the low quality mode
        //if (!_highQuality)
        {
            tw /= 2;
            th /= 2;
        }
			
        var rtFormat = useRGBM ?
            RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;

        var logh = Mathf.Log(th, 2) + radius - 8;
        var logh_i = (int)logh;
        var iterations = Mathf.Clamp(logh_i, 1, kMaxIterations);

        var lthresh = thresholdLinear;
        _material.SetFloat("_Threshold", lthresh);

        var knee = lthresh * softKnee + 1e-5f;
        var curve = new Vector3(lthresh - knee, knee * 2, 0.25f / knee);
        _material.SetVector("_Curve", curve);

  
        _material.SetFloat("_PrefilterOffs",   0.0f);

        _material.SetFloat("_SampleScale", 0.5f + logh - logh_i);
		_material.SetFloat("_Intensity", intensity);

		if (debugMode == 1) {
			Graphics.Blit(source, destination, _material, prefilterPass);
			return;
		}
        var prefiltered = RenderTexture.GetTemporary(tw, th, 0, rtFormat);
       
	
		Graphics.Blit(source, prefiltered, _material, prefilterPass);

        // construct a mip pyramid
        var last = prefiltered;
        for (var level = 0; level < iterations; level++)
        {
            _blurBuffer1[level] = RenderTexture.GetTemporary(
                last.width / 2, last.height / 2, 0, rtFormat
            );
				
			Graphics.Blit(last, _blurBuffer1[level], _material, downSample);

            last = _blurBuffer1[level];
        }

 
        for (var level = iterations - 2; level >= 0; level--)
        {
            var basetex = _blurBuffer1[level];
            _material.SetTexture("_BaseTex", basetex);

            _blurBuffer2[level] = RenderTexture.GetTemporary(
                basetex.width, basetex.height, 0, rtFormat
            );

			Graphics.Blit(last, _blurBuffer2[level], _material, upSample);
            last = _blurBuffer2[level];
        }

 
        _material.SetTexture("_BaseTex", source);
		if (debugMode == 2) 
			Graphics.Blit(last, destination);
		else
			Graphics.Blit(last, destination, _material, finalPass);

        // release the temporary buffers
        for (var i = 0; i < kMaxIterations; i++)
        {
            if (_blurBuffer1[i] != null)
                RenderTexture.ReleaseTemporary(_blurBuffer1[i]);

            if (_blurBuffer2[i] != null)
                RenderTexture.ReleaseTemporary(_blurBuffer2[i]);

            _blurBuffer1[i] = null;
            _blurBuffer2[i] = null;
        }

        RenderTexture.ReleaseTemporary(prefiltered);
    }


}
 
