Shader "Unlit/brdfRad"
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
		float LdotH = uv.x*2-1;
		float gloss = uv.y;
		half t = Pow5(1 - LdotH);   // ala Schlick interpoliation

		//F0 + t - F0  * t;
		//F0*(1 - t) + t;
		//return F0 + (1 - F0) * t;
		return float4(t,0,0,1);

	}
		ENDCG
	}
	}
}