Shader "Unlit/Building"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//_MainTex ("Texture", 2D) = "white" {}
		_ImageLight ("Texture", 2D) = "black" {}
		_ImageColor ("法术光颜色",Color) = (0.5,0.5,1,1)
		_Noise ("噪波", 2D) = "write" {}
		_Speed("噪波速度",Range(-2,2)) =  1
		_NoiseV ("噪波UV伸缩",Range(0.1,5)) =  1
		_BuildingTopColor("建筑高亮色",Color) = (0.5,0.5,1,0.5)
		_ColorPower("颜色强度",Range(0,5)) = 2 
		_GrayPower("_GrayPower", Range(1,5)) =  1
		_AlphaPower("AlphaPower",Range(0,2)) = 1
		//[KeywordEnum(TRADITIONAL, ADDITIVE)] _BLN("魔法光混合方式", Float) = 0
 
		 [Toggle(DIFFUSE_ON)] _FANSE_ ("漫反射", Int) = 1
		 [Toggle(_ALPHA_GRAY_ON)] _HUDU ("灰度alpha", Int) = 1


		  //[KeywordEnum(COLOR, GRAY)] _ALPHA_BLN("_ALPHA_BLN", Float) = 0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			//Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog _ALPHA_GRAY_ON

			#pragma shader_feature DIFFUSE_ON
 
			//#pragma shader_feature  _BLN_TRADITIONAL _BLN_ADDITIVE 

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			float4 _BuildingTopColor;
			float4 _ImageColor;
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				//UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
			};

			sampler2D _Noise;
			float4 _Noise_ST;


			sampler2D _ImageLight;
			float4 _ImageLight_ST;



			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _Speed;
			float _NoiseV;
			float _ColorPower;
			float _AlphaPower;
			float _GrayPower;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = float2(0.5,v.vertex.y*_NoiseV);
				//o.uv1 = TRANSFORM_TEX(v.uv, _Noise);
				o.uv2 = TRANSFORM_TEX(v.uv, _ImageLight);
				 float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                o.worldNormal = worldNormal;
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 hr = tex2D(_Noise, i.uv1 + half2(0,_Time.y*_Speed));

				fixed4 il = tex2D(_ImageLight, i.uv2);

				float nl = max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));

				float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
				#if DIFFUSE_ON
				float4 final = _BuildingTopColor*nl;
				#else
					float4 final = _BuildingTopColor;
				#endif

				final.rgb *= grey;
				#if _ALPHA_GRAY_ON  
					final.a = grey;
				#endif

				//final.a = pow(grey,_GrayPower);
				final.rgb *=_ColorPower;
				final.a *= _AlphaPower;
				//final.a  = min(final.a ,1) + hr;

			 
			 

				final.rgb =  final.rgb+ il.g*hr.g * _ImageColor.rgb;


				return final;
				 
			}
			ENDCG
		}
	}
}
