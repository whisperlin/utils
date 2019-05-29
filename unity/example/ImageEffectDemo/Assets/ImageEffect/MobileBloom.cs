//、using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MobileBloom : System.Object
{
	//public bool goole = false;
	[Range(1,3)]
    public int downSample = 1;

	[Range(1f,5f)]
    //采样率  
    public float samplerScale = 1;
    //高亮部分提取阈值  
    public Color colorThreshold = Color.gray;
    //Bloom泛光颜色  
    public Color bloomColor = Color.white;
    //Bloom权值  
    [Range(0.0f, 3.0f)]
    public float bloomFactor = 1.0f;

 

    [HideInInspector]
    public Material material = null;
    public void Init()
    {
        material = new Material(Shader.Find("TA/MobileBloom"));
    }
	public void Release()
	{
		
	}

	void BloomFast(RenderTexture source, RenderTexture destination)
	{
		int w = Screen.width >> downSample;
		int h = Screen.height >> downSample;
		//申请两块RT，并且分辨率按照downSameple降低  
		RenderTexture temp1 = RenderTexture.GetTemporary(w, h, 0  );
		RenderTexture temp2 = RenderTexture.GetTemporary(Screen.width/2, Screen.height/2, 0 );

		//直接将场景图拷贝到低分辨率的RT上达到降分辨率的效果  
		Graphics.Blit(source, temp1);
		material.SetFloat ("samplerScale", samplerScale);

		//根据阈值提取高亮部分,使用pass0进行高亮提取  
		material.SetVector("_colorThreshold", colorThreshold);
		Graphics.Blit(temp1, temp2, material, 0);

		//高斯模糊，两次模糊，横向纵向，使用pass1进行高斯模糊  
		material.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
		Graphics.Blit(temp2, temp1, material, 1);
		material.SetVector("_offsets", new Vector4(samplerScale, 0, 0, 0));
		Graphics.Blit(temp1, temp2, material, 3);

		//Bloom，将模糊后的图作为Material的Blur图参数  
		material.SetTexture("_BlurTex", temp2);
		material.SetVector("_bloomColor", bloomColor);
		material.SetFloat("_bloomFactor", bloomFactor);

		//使用pass2进行景深效果计算，清晰场景图直接从source输入到shader的_MainTex中  
		Graphics.Blit(source, destination, material, 2);

		//释放申请的RT  
		RenderTexture.ReleaseTemporary(temp1);
		RenderTexture.ReleaseTemporary(temp2);
	}


	void Bloom(RenderTexture source, RenderTexture destination)
	{

		RenderTexture [] temp1 = new RenderTexture[downSample];
		RenderTexture [] temp2 = new RenderTexture[downSample];

	 
		for (int i = 0; i < downSample; i++) {
			 
			int w = Screen.width >> i+1;
			int h = Screen.height >> i+1;
			temp1[i] = RenderTexture.GetTemporary(w, h, 0  );
			temp2 [i] = RenderTexture.GetTemporary (w, h, 0);
		}

		//直接将场景图拷贝到低分辨率的RT上达到降分辨率的效果  
		//Graphics.Blit(source, temp2[0]);
		material.SetFloat ("samplerScale", samplerScale);
		//根据阈值提取高亮部分,使用pass0进行高亮提取  
		material.SetVector("_colorThreshold", colorThreshold);
		Graphics.Blit(source, temp1[0], material, 0);


		for (int i = 1; i < downSample; i++) {

			Graphics.Blit(temp1[i-1], temp1[i], material, 1);
		}

		if(downSample>1)
			Graphics.Blit(temp1[downSample-1], temp2[downSample-2], material, 3);
		else
			Graphics.Blit(temp1[0], temp2[0], material, 3);
		for (int i = downSample-2; i > 0 ; i--) {
			Graphics.Blit(temp2[i], temp2[i-1], material, 3);
		}
			


		//Bloom，将模糊后的图作为Material的Blur图参数  
		material.SetTexture("_BlurTex", temp2[0]);
		material.SetVector("_bloomColor", bloomColor);
		material.SetFloat("_bloomFactor", bloomFactor);

		//使用pass2进行景深效果计算，清晰场景图直接从source输入到shader的_MainTex中  
		Graphics.Blit(source, destination, material, 2);


		for (int i = 1; i < downSample; i++) {
			RenderTexture.ReleaseTemporary(temp1[i]);
			RenderTexture.ReleaseTemporary(temp2[i]);
		}
		//释放申请的RT  

	}



    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
		if (!material)
			return;

		Bloom (source,destination);
	 
		//BloomFast (source,destination);

    }
}