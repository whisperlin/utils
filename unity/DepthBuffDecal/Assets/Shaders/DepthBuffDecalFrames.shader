

Shader "Dase/DecaleFrames" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_FrameU("FrameU" ,Float) = 0
		_FrameV("FrameV" ,Float) = 0
		_FrameSizeU("Frame Size U" ,Float) = 1
		_FrameSizeV("Frame Size U" ,Float) = 1
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"  "DisableBatching" = "True" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass{
		CGPROGRAM
#include "UnityCG.cginc"

#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};
		struct v2f
		{
			float2 uv : TEXCOORD0;
			float3 worldDirection : TEXCOORD1;
			float4 screenPosition : TEXCOORD2;
			float4 vertex : SV_POSITION;
		};
		sampler2D _CameraDepthTexture;
		sampler2D _MainTex;
		float4 _MainTex_ST;

		float _FrameU;
		float _FrameV;
		float _FrameSizeU;
		float _FrameSizeV;

		v2f vert(appdata v)
		{
			v2f o;

			o.worldDirection = mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos;

			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;

			o.screenPosition = o.vertex;
#if defined(SHADER_API_D3D9) || defined(SHADER_API_D3D11)  || defined(SHADER_API_D3D11_9X)
			o.screenPosition.y = -o.screenPosition.y;
#endif
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			float perspectiveDivide = 1.0f / i.screenPosition.w;
			float3 direction = i.worldDirection * perspectiveDivide;
			float2 screenUV = (i.screenPosition.xy * perspectiveDivide) * 0.5f + 0.5f;		
			float depth = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, screenUV)));

			float3 worldspace = direction * depth + _WorldSpaceCameraPos;
			float4 worldPosition = float4(worldspace.xyz,1.0f);
			float4 localPosition = mul(unity_WorldToObject,worldPosition);
			

			float _t = step(abs(localPosition.x), 0.5f) *step(abs(localPosition.y), 0.5f)*step(abs(localPosition.z), 0.5f);
			localPosition.x = localPosition.x + 0.5f;
			localPosition.y = localPosition.y + 0.5f;

			localPosition.x = _FrameU + localPosition.x * _FrameSizeU;
			localPosition.y = _FrameV + localPosition.y * _FrameSizeV;

			fixed4 finalColor = tex2D(_MainTex, localPosition.xy);
			finalColor.a = finalColor.a*_t;
			return finalColor;
		}

			ENDCG
		}
	}

}