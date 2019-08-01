Shader "TA/Emission"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Normal("自发光", 2D) = "white" {}
		_Color("颜色",Color) = (1,1,1,1)
		_Emission("Emission (Lightmapper)", Range(0,1)) = 0.0
		[Toggle(ClIP_ENABLE)] _WIND_ENABLE("裁切开启", Float) = 0
		_CutAlpha("CutAlpha", Range(0, 1)) = 0.5

	}

		SubShader
		{
			Tags { "Queue" = "AlphaTest" }
			Cull Off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
				#pragma multi_compile_fog
				#pragma multi_compile __ BRIGHTNESS_ON
				#pragma   multi_compile  _  ClIP_ENABLE
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc" //第三步// 
				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
					float2 uv2 : TEXCOORD1;
#else

#endif
					float3 normal : NORMAL;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
					float2 uv2 : TEXCOORD1;
#else
					LIGHTING_COORDS(5, 6)
#endif
					UNITY_FOG_COORDS(2)

					float3 normalWorld : TEXCOORD3;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				sampler2D _Normal;

				fixed _Emission;
				fixed _CutAlpha;

	#ifdef BRIGHTNESS_ON
				fixed3 _Brightness;
	#endif

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.uv;
					o.normalWorld = UnityObjectToWorldNormal(v.normal);
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
					o.uv2 = v.uv2 * unity_LightmapST.xy + unity_LightmapST.zw;
#else
					TRANSFER_VERTEX_TO_FRAGMENT(o);
#endif
					UNITY_TRANSFER_FOG(o, o.vertex);
					return o;
				}
				fixed4 _Color;
				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 c = tex2D(_MainTex, i.uv);
					fixed4 c0 = c * _Color;
#if ClIP_ENABLE
					clip(c.a - _CutAlpha);
#endif
					fixed4 e = tex2D(_Normal, i.uv);
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
					fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2));
					c.rgb = UNITY_LIGHTMODEL_AMBIENT * c.rgb + c.rgb * lm;
#else

					half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
					half nl = saturate(dot(i.normalWorld, lightDir));
					c.rgb = UNITY_LIGHTMODEL_AMBIENT * c.rgb + _LightColor0 * nl * c.rgb* LIGHT_ATTENUATION(i);

#endif

	#ifdef BRIGHTNESS_ON
					c.rgb = c.rgb * _Brightness * 2;
	#endif
					c.rgb += c0.rgb*_Emission*e.b;
					UNITY_APPLY_FOG(i.fogCoord, c);
					return c;
				}
				ENDCG
			}
		}
}

 