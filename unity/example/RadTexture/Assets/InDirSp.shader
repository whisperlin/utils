Shader "Hidden/InDirSp"
{
	Properties
	{

	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}


	 
	inline half4 Pow5(half4 x)
	{
		return x*x * x*x * x;
	}

	fixed4 frag(v2f i): SV_Target
	{
		float2 uv = i.uv;
		float NdotV = uv.x;
		float gloss = uv.y;

 
		float perceptualRoughness = 1.0 - gloss;
		float roughness = perceptualRoughness * perceptualRoughness;
		half surfaceReduction;
		surfaceReduction = 1.0 - 0.28*roughness*perceptualRoughness;
 

		half t0 = Pow5(1 - NdotV);
  
		return float4((1-t0)*surfaceReduction,t0*surfaceReduction, 0, 1);
		//return float4(pow(halfD, gross * 256),0, 0, 1);

	}
		ENDCG
	}
	}
}