Shader "Unlit/SkinRenderBase"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainTexTin("漫射颜色",Color) = (1,1,1,1)
	 	metalness("金属强度",Range(0.,1)) = 0
		perceptualRoughness("高光强度",Range(0.02,1)) = 0
		IBLSampleMap("IBLSampleMap",CUBE) = "sky"{}
		IBL_Color("环境颜色",COLOR) = (1,1,1,1)
		IBL_Intensity("反射强度",Range(0,3)) = 1
		SPL_Color("反射颜色",Color) = (1,1,1,1)
		SPL_Intensity("SPL_Intensity",Range(0,3)) = 0
		SPLSampleMap("SPLSampleMap",CUBE) = "sky"

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "RolantinCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 nor:NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3	norr:TEXCOORD2;
				float3 posWorld : TEXCOORD4 ;

			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float metalness;

			float4 _MainTexTin;
			float perceptualRoughness;

			samplerCUBE IBLSampleMap;
			samplerCUBE SPLSampleMap;
			//float IBL_Intensity;
			float4 IBL_Color;

			float SPL_Intensity;
			float4 SPL_Color;



			v2f vert (appdata v)
			{
				v2f o;
				o.posWorld = mul(unity_ObjectToWorld , v.vertex) ;
				o.vertex =UnityObjectToClipPos ( v.vertex);
				//o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.norr = UnityObjectToWorldNormal(v.nor);
				UNITY_TRANSFER_FOG(o,o.vertex);

				return o;
			}


			float3 FresnelShick(float3 F0,float NdotV) {
					return (F0 + (1 - F0) * pow(1.0 - NdotV, 5.0));
			}

			float3 IBLSample (samplerCUBE IBLSampleMap,float3 N,float IBL_Intensity,float4 IBL_Color){
					float3 IBL =max(0, texCUBE(IBLSampleMap,N)*IBL_Intensity*IBL_Color);
					return IBL;
			}

			float3 FresnelRoughness(float perceptualRoughness,float3 F0,float NdotV) {
					return F0 + (max(1 - perceptualRoughness, F0) - F0) * pow(1.0 - NdotV, 5.0);
			}

			float calc_Geometry_GGX_IBL(float costheta, float roughness) {
					float a = roughness * roughness;
					float k = a / 2.0;
					float t = costheta * (1.0 - k) + k;
					return costheta / t;
			}

			float GeometrySmith(float3 n, float3 v, float3 l, float roughness) {
					float ndotv = max(dot(n, v), 0.0);
					float ndotl = max(dot(n, l), 0.0);
					float ggx1 = calc_Geometry_GGX_IBL(ndotv, roughness);
					float ggx2 = calc_Geometry_GGX_IBL(ndotl, roughness);
					return ggx1 * ggx2;
			}

			float3 EnvironmentBRDF(float3 F0,float NoV,float perceptualRoughness){
					float3 rf0 = F0;

					float  g = 1-perceptualRoughness;
						 //g = i.UV.y;
					 // NoV = i.UV.x;
					float4 t = float4( 1/0.96, 0.475, (0.0275 - 0.25 * 0.04)/0.96, 0.25 );
					t *= float4( g, g, g, g );
					t += float4( 0, 0, (0.015 - 0.75 * 0.04)/0.96, 0.75 );
					float a0 = t.x * min( t.y, exp2( -9.28 * NoV ) ) + t.z; float a1 = t.w;
					return saturate( a0 + rf0 * ( a1 - a0 ) );
			}

			float3 IBL_pdf(float3  n,float3 v,float3 F0,float ndotv,float perceptualRoughness,
									samplerCUBE SPLSampleMap,float4 SPL_Color,float SPL_Intensity) {

			float3 r = 2.0 * ndotv * n - v;
			//float fade = horizonFading(-dot(n, v), horizonFade) ;
			// SPL Envmap
			float3 ld =  max(0,texCUBElod(SPLSampleMap,float4(r, perceptualRoughness* 9.0)).rgb* SPL_Color * SPL_Intensity);
			//sampler  brdf map
			// float3 dfg = i.BRDF;
			// float2 IntegrateBRDFMap = IntegrateBRDF(i);
			//采公式版本
			//float3 specular = ld * (i.F0 * IntegrateBRDFMap.x + IntegrateBRDFMap.y);
			//采图版本
			// float3 specular = ld * (i.F0 * dfg.x + dfg.y);
			//近似值GGX版本
			// specular = ld * EnvDFGLazarov(i);
			//近似值BlinPhone版本

			return ld *EnvironmentBRDF(F0,ndotv,perceptualRoughness);
		}

			#define PI 3.1415926

		float DistributionGGX(float perceptualRoughness ,float NdotH ) {
			float roughness = perceptualRoughness;
			float a = roughness * roughness;
			float a2 = a * a;
			float ndoth = NdotH;
			float ndoth2 = ndoth * ndoth;
			float t = ndoth2 * (a2 - 1.0) + 1.0;
			float t2 = t * t;
			return a2 / (PI * t2);
		}








			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture

       float3 L = LightDir0;
       float3 N = i.norr;
				float3 V = normalize(_WorldSpaceCameraPos.xyz - i.posWorld) ;
        float3 H = normalize(-L + V);
				float NdotV = saturate(dot (N,V));
				float NdotL =saturate (dot(-L,N));
				float NdotH = saturate(dot(N,H));

				float3 atten = LightColor0 * LightIntensity0;


	      float function_G = GeometrySmith(N,V,-L,perceptualRoughness);


				fixed4 AlbedoColor = tex2D(_MainTex, i.uv)* _MainTexTin;

				float3 F0 = lerp(0.04, AlbedoColor, metalness);

				  float3 FresnelRough= FresnelRoughness(perceptualRoughness,F0,NdotV);

        float3   TRough = 1 - FresnelRough;
				 float3  kDRough =  TRough * (1.0 - metalness);

				float3 IBL =  IBLSample(IBLSampleMap,N,IBL_Intensity,IBL_Color) * AlbedoColor  * kDRough;



				 float3 function_F = FresnelShick(F0,NdotV);
			  	float3   T = 1 - function_F;
			  	float3  kD =  T * (1.0 - metalness);
				 float3 Diffuse = kD * AlbedoColor/PI;
				 float3 Specular =  DistributionGGX(perceptualRoughness, NdotH) * function_F * function_G / 4.0  *  NdotV * NdotL  + 0.001;
				 float3 Combine = ( Diffuse + Specular)* NdotL * atten;

				float3 c;
				c=  ((Combine  + IBL_pdf(N, V, F0,NdotV,perceptualRoughness,
					               SPLSampleMap,SPL_Color,SPL_Intensity)  + IBL)) ;




				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return float4(c,1);
			}
			ENDCG
		}
	}
}
