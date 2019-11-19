
Shader "Shader Forge/PBR" {
    Properties {
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _MainTex ("Base Color", 2D) = "white" {}
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8

		[Toggle] _ENABLE_Rad("ENABLE_Rad", Float) = 0
 
		_IndirSP("_IndirSP", 2D) = "white" {}

		_BrdfRad("_BrdfRad", 2D) = "white" {}

    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog

			#pragma multi_compile  _ENABLE_RAD_OFF _ENABLE_RAD_ON
 
		
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
			uniform sampler2D _IndirSP;
			uniform sampler2D _BrdfRad;
			
            uniform float _Metallic;
            uniform float _Gloss;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
			float sqr(float x)
			{
				return x*x;
			}
			float GGXNormalDistribution(float roughness, float NdotH)
			{
				float roughnessSqr = roughness*roughness;
				float NdotHSqr = NdotH*NdotH;
				float TanNdotHSqr = (1 - NdotHSqr) / NdotHSqr;
				return (1.0 / 3.1415926535) * sqr(roughness / (NdotHSqr * (roughnessSqr + TanNdotHSqr)));
			}

            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));

				float NdotV = abs(dot(normalDirection, viewDirection));
				float NdotH = saturate(dot(normalDirection, halfDirection));
#ifdef _ENABLE_RAD_ON 
				
				float4 _BrdfRad_var = tex2D(_BrdfRad, float2( (LdotH+1)*0.5 , gloss));
#else 
				 
 
#endif

                float3 diffuseColor = (_MainTex_var.rgb*_Color.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
               
 

                float GeometricShadow = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float NormalDistribution = GGXTerm(NdotH, roughness);
				
                
				
 
               //F0*(1 - t) + t;
#ifdef _ENABLE_RAD_ON 
				float3 FresnelFunction = (specularColor *(1 - _BrdfRad_var.x) + _BrdfRad_var.x);
#else
				//float3 FresnelFunction = FresnelTerm(specularColor, LdotH);
				float3 FresnelFunction = FresnelTerm(specularColor, LdotH);
#endif
			

				float specularPBL = (GeometricShadow*NormalDistribution) * UNITY_PI;
				
				#ifdef FresnelFunction
					specularPBL = sqrt(max(1e-4h, specularPBL));
				#endif
					specularPBL = max(0, specularPBL * NdotL);
				#if defined(_SPECULARHIGHLIGHTS_OFF)
					specularPBL = 0.0;
				#endif
					
				specularPBL *= any(specularColor) ? 1.0 : 0.0;

				//float3 directSpecular = attenColor*specularPBL * UNITY_PI;
				//float3 directSpecular = attenColor*specularPBL;
				float3 directSpecular = attenColor*specularPBL*FresnelFunction;
 
				/**
				float3 brdf =   (specularColor *NormalDistribution * FresnelFunction * GeometricShadow) / (4 * (NdotL * NdotV))  ;
			 
				brdf = max(0, brdf);
				float3 directSpecular = attenColor*brdf;
				*/
 
				 

#ifdef _ENABLE_RAD_ON 

				float4 _IndirSP_var = tex2D(_IndirSP, float2(NdotV, gloss));
				half grazingTerm = saturate(gloss + specularMonochrome);
				float3 indirectSpecular = (gi.indirect.specular);
				indirectSpecular *= specularColor*_IndirSP_var.x + grazingTerm* _IndirSP_var.y;
#else
				half surfaceReduction;
				#ifdef UNITY_COLORSPACE_GAMMA
					surfaceReduction = 1.0 - 0.28*roughness*perceptualRoughness;
				#else
					surfaceReduction = 1.0 / (roughness*roughness + 1.0);
				#endif
				half grazingTerm = saturate(gloss + specularMonochrome);
				float3 indirectSpecular = (gi.indirect.specular);

				indirectSpecular *= FresnelLerp(specularColor, grazingTerm, NdotV);
				indirectSpecular *= surfaceReduction;
#endif
                float3 specular = (directSpecular + indirectSpecular);
               
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                //half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                //float nlPow5 = Pow5(1-NdotL);
                //float nvPow5 = Pow5(1-NdotV);


                //
                float3 directDiffuse =  NdotL * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;

                //return float4(specular,1);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;

 

/// Final Color:

				
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
