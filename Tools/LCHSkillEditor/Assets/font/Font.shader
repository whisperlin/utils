Shader "Mobile/Particles/Alpha Blended 1" {
	Properties{
	 _MainTex("Particle Texture", 2D) = "white" { }
	}
		SubShader{
		 Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane" }
		 Pass {
		  Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane" }
		  ZWrite Off
		  Cull Off
		  Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		#include "UnityCG.cginc"
		#pragma multi_compile_fog
		#define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))

		// uniforms
		float4 _MainTex_ST;

	// vertex shader input data
	struct appdata {
	  float3 pos : POSITION;
	  half4 color : COLOR;
	  float3 uv0 : TEXCOORD0;
	  UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	// vertex-to-fragment interpolators
	struct v2f {
	  fixed4 color : COLOR0;
	  float2 uv0 : TEXCOORD0;
	  UNITY_FOG_COORDS(1)
	  float4 pos : SV_POSITION;
	  UNITY_VERTEX_OUTPUT_STEREO
	};

	// vertex shader
	v2f vert(appdata IN) {
	  v2f o;
	  UNITY_SETUP_INSTANCE_ID(IN);
	  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	  half4 color = IN.color;
	  float3 eyePos = mul(UNITY_MATRIX_MV, float4(IN.pos,1)).xyz;
	  half3 viewDir = 0.0;
	  o.color = saturate(color);
	  // compute texture coordinates
	  o.uv0 = IN.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;

	  UNITY_TRANSFER_FOG(o, o.pos);

	  // transform position
	  o.pos = UnityObjectToClipPos(IN.pos);
	  return o;
	}

	// textures
	sampler2D _MainTex;

	// fragment shader
	fixed4 frag(v2f IN) : SV_Target {
	  fixed4 col;
	  fixed4 tex, tmp0, tmp1, tmp2;
	  // SetTexture #0
	  tex = tex2D(_MainTex, IN.uv0.xy);
	  col = tex * IN.color;

	  UNITY_APPLY_FOG(IN.fog, col);

	  return col;
	}

		// texenvs
		//! TexEnv0: 01010103 01010103 [_MainTex]
		ENDCG
		 }
	}
}