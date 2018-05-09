// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NewSurfaceShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_EdgeColor("Edge Color", Color) = (0,0,0,1)
		_EdgeWidth("Edge width", Range(.002, 0.03)) = .005
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		//Edge detection pass
		Pass{
		Lighting Off Fog{ Mode Off }
		Cull Front

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest

		fixed4 _EdgeColor;
	fixed _EdgeWidth;

	struct a2v {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 Pos : POSITION;
	};

	v2f vert(a2v v) {
		v2f o;
		o.Pos = UnityObjectToClipPos(v.vertex);
		fixed3 norm = mul((float3x3)UNITY_MATRIX_MV, v.normal);
		norm.x *= UNITY_MATRIX_P[0][0];
		norm.y *= UNITY_MATRIX_P[1][1];
		o.Pos.xy += norm.xy * _EdgeWidth;
		return o;
	}

	half4 frag(v2f i) : COLOR{
		return _EdgeColor;
	}
		ENDCG
	}

		CGPROGRAM
		//Absolute path
#include "Assets/Aubergines Toon Shaders/Shaders/Includes/Aubergine_Lights.cginc"
		//Or you can use relative path as below, whatever suits you
		//#include "../../../Includes/Aubergine_Lights.cginc"
#pragma surface surf Aub_Toon

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
