Shader "Custom/EudgeLightMap" {
	Properties {
		_MainColor ("Color" , Color) = (0,0,1,0.5)
		_SpecularColor("_SpecularColor" ,Color) = (1,1,1,0.7)
		_MaskPower("MaskPower", Range(0.1 , 1)) = 0.2
		_EdgePow("Edge Pow" , Range(0 , 1)) = 0.5
		_MainTex("Main Tex" , 2D) = "white"{}
		_MaskTex("Mask Tex" ,  2D) = "white" {}
		_SpeTex("Main Tex" , 2D) = "white"{}
		_speed_u("Speed_u" ,Range(0 , 2)) = 1.0
		_speed_v("Speed_v" ,Range(0 , 2)) = 1.0
	}

	SubShader {

	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "DisableBatching"="True"}
	
	Pass{
		Tags { "LightMode"="ForwardBase" }	
		
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off
		
		CGPROGRAM

		#include "UnityCG.cginc"

		#pragma vertex vert
		#pragma fragment frag

		#define UNITY_PASS_FORWARDBASE
        #pragma multi_compile_fwdbase

		float4 _MainColor;
		float4 _SpecularColor;
		sampler2D _CameraDepthTexture;
		float _EdgePow;
		sampler2D _MainTex;
		float4 _MainTex_ST;

		sampler2D _SpeTex;
		float4 _SpeTex_ST;
		
		sampler2D _MaskTex;
		float _speed_u;
		float _speed_v;
		float _MaskPower;

		struct a2v{
			float4 vertex:POSITION;
			float3 normal:NORMAL;
			float2 tex:TEXCOORD0;
		};

		struct v2f{
			float4 pos:POSITION;
			float4 scrPos:TEXCOORD0;
		
			float2 uv:TEXCOORD1;
		};

		v2f vert (a2v v )
		{
			v2f o;
			o.pos = UnityObjectToClipPos ( v.vertex );
			o.scrPos = ComputeScreenPos ( o.pos );
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; 
			half3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
			half3 worldNormal = UnityObjectToWorldNormal(v.normal); 
			o.uv = TRANSFORM_TEX(v.tex , _MainTex);
			COMPUTE_EYEDEPTH(o.scrPos.z);
			return o;
		}
	
		fixed4 frag ( v2f i ) : SV_TARGET
		{
			fixed mainTex = 1 - tex2D(_MainTex , i.uv).a;
			fixed mask = tex2D(_MaskTex , i.uv + float2(_Time.y*_speed_u, _Time.y*_speed_v)).r;

			fixed speTex =tex2D(_SpeTex, i.uv);
			//fixed4 finalColor = lerp(_MainColor , _SpecularColor , mainTex);
			//mask = mask*0.8+0.2;
			//finalColor=lerp(fixed4(0,0,0,0),finalColor,mask);
			//mask = saturate(mask*_MaskPower + mainTex);
			

		
			//获取深度图和clip space的深度值
			float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
			float partZ = i.scrPos.z;

			//比较和之前场景盾深度查，小于特定大小时绘画高光。。
 			float diff = 1-saturate((sceneZ-i.scrPos.z)*4 - _EdgePow);
			//最后通过插值混合颜色
			mask = mask + mainTex;
			mask = max(mask, speTex);
			mask = saturate(mask  + diff );
			fixed4 finalColor = lerp(_MainColor, _SpecularColor, mask);
			return finalColor;
		}

		ENDCG
		}
	}
}
