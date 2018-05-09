// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EJoy/Decal"  {
	Properties
	{
		_Color("Main Color", Color) = (0,0,0,0.5)
		_MainTex("MainTex", 2D) = "" {}
	}

		Subshader
	{
		Tags{ "Queue" = "Transparent" }
		Pass
	{
		ZWrite Off
		Fog{ Mode Off }
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		Offset -1, -1

		CGPROGRAM
#pragma vertex vert  
#pragma fragment frag  
#include "UnityCG.cginc"  

		struct v2f
	{
		float4 uv : TEXCOORD0;
		float4 pos : SV_POSITION;

	};

	float4x4 unity_Projector;
	v2f vert(float4 vertex : POSITION)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(vertex);
		o.uv = mul(unity_Projector, vertex);
		return o;
	}

	sampler2D _MainTex;
	float4 _MainTex_ST;
	fixed4 _Color;
	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 texS = tex2Dproj(_MainTex, UNITY_PROJ_COORD(i.uv))*_Color;

	return texS;
	}
		ENDCG
	}
	}
}



