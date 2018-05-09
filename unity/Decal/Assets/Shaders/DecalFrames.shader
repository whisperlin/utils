// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EJoy/DecalFrames" {
	Properties
	{
		_Color("Main Color", Color) = (0,0,0,0.5)
		_MainTex("MainTex", 2D) = "" {}
		_FrameU("FrameU" ,Float) = 0
		_FrameV("FrameV" ,Float) = 0
		_FrameSizeU("Frame Size U" ,Float) = 1
		_FrameSizeV("Frame Size U" ,Float) = 1
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
	float _FrameU;
	float _FrameV;
	float _FrameSizeU;
	float _FrameSizeV;
	fixed4 frag(v2f i) : SV_Target
	{

		float4  uvproj = UNITY_PROJ_COORD(i.uv);

		float2 uv2 = uvproj.xy / uvproj.w;
		uv2.x = saturate(uv2.x);
		uv2.x = _FrameU + uv2.x * _FrameSizeU;
		uv2.y = saturate(uv2.y);
		uv2.y = _FrameV + uv2.y * _FrameSizeV;
		fixed4 texS = tex2D(_MainTex, uv2);

		return texS;
	}
		ENDCG
	}
	}
}