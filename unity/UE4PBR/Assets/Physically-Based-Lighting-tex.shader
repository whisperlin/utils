
Shader "Physically-Based-Lighting-base-tex" {
    Properties {
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _MainTex ("Base Color", 2D) = "white" {}
        _MetallicTex ("_MetallicTex", 2D) = "white" {}
        _SpecularPower("Specular Power", Range(0,1)) = 1
        _Ior("Ior",  Range(1,4)) = 1.5
        _Metallic("Metallicness",Range(0,1)) = 1
         
        [KeywordEnum(UNITY,Beckmann,Gaussian,GGX,TrowbridgeReitz,TrowbridgeReitzAnisotropic, Ward)] _NormalDistModel("Normal Distribution Model;", Float) = 0
        [Toggle(UNITY_DEFAULT_GEO)] UNITY_DEFAULT_GEO("UNITY DEFAULT GEO", Int) = 0
        [KeywordEnum( AshikhminShirley,AshikhminPremoze,Duer,Neumann,Kelemen,ModifiedKelemen,Cook,Ward,Kurt)]_GeoShadowModel("Geometric Shadow Model;", Float) = 0
		[KeywordEnum(None,Walter,Beckman,GGX,Schlick,SchlickBeckman,SchlickGGX, Implicit)]_SmithGeoShadowModel("Smith Geometric Shadow Model; None if above is Used;", Float) = 0
		[KeywordEnum(Schlick,SchlickIOR, SphericalGaussian)]_FresnelModel("Normal Distribution Model;", Float) = 0

		[KeywordEnum(unity,other)]_MetallicType("metallic Model;", Float) = 0

        _Anisotropic("Anisotropic",  Range(-20,1)) = 0
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
            #include "pbr.cginc"


             #pragma shader_feature UNITY_DEFAULT_GEO
            
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON


            #pragma multi_compile _NORMALDISTMODEL_UNITY  _NORMALDISTMODEL_BECKMANN  _NORMALDISTMODEL_GAUSSIAN _NORMALDISTMODEL_GGX _NORMALDISTMODEL_TROWBRIDGEREITZ _NORMALDISTMODEL_TROWBRIDGEREITZANISOTROPIC _NORMALDISTMODEL_WARD

            #pragma multi_compile  _GEOSHADOWMODEL_ASHIKHMINSHIRLEY _GEOSHADOWMODEL_ASHIKHMINPREMOZE _GEOSHADOWMODEL_DUER_GEOSHADOWMODEL_NEUMANN _GEOSHADOWMODEL_KELEMAN _GEOSHADOWMODEL_MODIFIEDKELEMEN _GEOSHADOWMODEL_COOK _GEOSHADOWMODEL_WARD _GEOSHADOWMODEL_KURT 
            #pragma multi_compile _SMITHGEOSHADOWMODEL_NONE _SMITHGEOSHADOWMODEL_WALTER _SMITHGEOSHADOWMODEL_BECKMAN _SMITHGEOSHADOWMODEL_GGX _SMITHGEOSHADOWMODEL_SCHLICK _SMITHGEOSHADOWMODEL_SCHLICKBECKMAN _SMITHGEOSHADOWMODEL_SCHLICKGGX _SMITHGEOSHADOWMODEL_IMPLICIT
            #pragma multi_compile  _FRESNELMODEL_UNITY  _FRESNELMODEL_SCHLICK _FRESNELMODEL_SCHLICKIOR _FRESNELMODEL_SPHERICALGAUSSIAN
            #pragma multi_compile  UNITY_METALLICTYPE   OTHER_METALLICTYPE
    

            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            sampler2D _MetallicTex;

            uniform float _SpecularPower;
            uniform float _Anisotropic;
            float _Ior;

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
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);

                float4 control_var =   tex2D(  _MetallicTex ,  i.uv0 );
                float _Metallic = control_var.r;
            	float _Gloss = control_var.a;

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
                float VdotH = max(0.0,dot( viewDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (_MainTex_var.rgb*_Color.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
 
             	
                 //Geometric Shadowing term----------------------------------------------------------------------------------

                #if UNITY_DEFAULT_GEO
             	 	 float SpecularDistribution = UnitySmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
             	#else
					#ifdef _SMITHGEOSHADOWMODEL_NONE

					 	#ifdef _GEOSHADOWMODEL_ASHIKHMINSHIRLEY
							float SpecularDistribution =  AshikhminShirleyGeometricShadowingFunction (NdotL, NdotV, LdotH);
					 	#elif _GEOSHADOWMODEL_ASHIKHMINPREMOZE
							float SpecularDistribution =  AshikhminPremozeGeometricShadowingFunction (NdotL, NdotV);
					 	#elif _GEOSHADOWMODEL_DUER
							float SpecularDistribution =  DuerGeometricShadowingFunction (lightDirection, viewDirection, normalDirection, NdotL, NdotV);
					 	#elif _GEOSHADOWMODEL_NEUMANN
							float SpecularDistribution =  NeumannGeometricShadowingFunction (NdotL, NdotV);
					 	#elif _GEOSHADOWMODEL_KELEMAN
							float SpecularDistribution =  KelemenGeometricShadowingFunction (NdotL, NdotV, LdotH,  VdotH);
					 	#elif _GEOSHADOWMODEL_MODIFIEDKELEMEN
							float SpecularDistribution =   ModifiedKelemenGeometricShadowingFunction (NdotV, NdotL, roughness);
					 	#elif _GEOSHADOWMODEL_COOK
							float SpecularDistribution =  CookTorrenceGeometricShadowingFunction (NdotL, NdotV, VdotH, NdotH);
					 	#elif _GEOSHADOWMODEL_WARD
							float SpecularDistribution =  WardGeometricShadowingFunction (NdotL, NdotV, VdotH, NdotH);
					 	#elif _GEOSHADOWMODEL_KURT
							float SpecularDistribution =  KurtGeometricShadowingFunction (NdotL, NdotV, VdotH, roughness);
					 	#else 			
				 			float SpecularDistribution =  ImplicitGeometricShadowingFunction (NdotL, NdotV);
				 		#endif

					////SmithModelsBelow
					////Gs = F(NdotL) * F(NdotV);

				  	#elif _SMITHGEOSHADOWMODEL_WALTER
						float SpecularDistribution =  WalterEtAlGeometricShadowingFunction (NdotL, NdotV, roughness);
					#elif _SMITHGEOSHADOWMODEL_BECKMAN
						float SpecularDistribution =  BeckmanGeometricShadowingFunction (NdotL, NdotV, roughness);
				 	#elif _SMITHGEOSHADOWMODEL_GGX
						float SpecularDistribution =  GGXGeometricShadowingFunction (NdotL, NdotV, roughness);
					#elif _SMITHGEOSHADOWMODEL_SCHLICK
						float SpecularDistribution =  SchlickGeometricShadowingFunction (NdotL, NdotV, roughness);
				 	#elif _SMITHGEOSHADOWMODEL_SCHLICKBECKMAN
						float SpecularDistribution =  SchlickBeckmanGeometricShadowingFunction (NdotL, NdotV, roughness);
				 	#elif _SMITHGEOSHADOWMODEL_SCHLICKGGX
						float SpecularDistribution =  SchlickGGXGeometricShadowingFunction (NdotL, NdotV, roughness);
					#elif _SMITHGEOSHADOWMODEL_IMPLICIT
						float SpecularDistribution =  ImplicitGeometricShadowingFunction (NdotL, NdotV);
					#else
						float SpecularDistribution =  ImplicitGeometricShadowingFunction (NdotL, NdotV);
				 	#endif

			 	#endif


                #ifdef _NORMALDISTMODEL_UNITY 
					   float GeometricShadow = UnityGGXTerm(NdotH, roughness);
			 	#elif _NORMALDISTMODEL_BECKMANN
			 		 float GeometricShadow = BeckmannNormalDistribution(roughness, NdotH);
			 	#elif _NORMALDISTMODEL_GAUSSIAN
					 float GeometricShadow =   GaussianNormalDistribution(roughness, NdotH);
			 	#elif _NORMALDISTMODEL_GGX
					float GeometricShadow =   GGXNormalDistribution(roughness, NdotH);
			 	#elif _NORMALDISTMODEL_TROWBRIDGEREITZ
					 float GeometricShadow =  TrowbridgeReitzNormalDistribution(NdotH, roughness);
			 	#elif _NORMALDISTMODEL_TROWBRIDGEREITZANISOTROPIC
					 float GeometricShadow =   TrowbridgeReitzAnisotropicNormalDistribution(_Anisotropic,NdotH, dot(halfDirection, i.tangentDir), dot(halfDirection,  i.bitangentDir),_Gloss);
				#elif _NORMALDISTMODEL_WARD
				 	 float GeometricShadow =   WardAnisotropicNormalDistribution(_Anisotropic,NdotL, NdotV, NdotH, dot(halfDirection, i.tangentDir), dot(halfDirection,  i.bitangentDir),_Gloss);
				#else
					float GeometricShadow =   GGXNormalDistribution(roughness, NdotH);
				#endif





				#if UNITY_DEFAULT_GEO
 
	                float specularPBL = (SpecularDistribution*GeometricShadow) * UNITY_PI;
	                #ifdef UNITY_COLORSPACE_GAMMA
	                    specularPBL = sqrt(max(1e-4h, specularPBL));
	                #endif
	                specularPBL = max(0, specularPBL * NdotL);
	                #if defined(_SPECULARHIGHLIGHTS_OFF)
	                    specularPBL = 0.0;
	                #endif
	               
	                specularPBL *= any(specularColor) ? 1.0 : 0.0;
	                float3 directSpecular = attenColor*specularPBL*UnityFresnelTerm(specularColor, LdotH);

               #else
      				#ifdef _FRESNELMODEL_UNITY
      					float3 FresnelFunction =  UnityFresnelTerm(specularColor, LdotH);
	                #elif _FRESNELMODEL_SCHLICK
						float3 FresnelFunction =  SchlickFresnelFunction(specularColor, LdotH);
					#elif _FRESNELMODEL_SCHLICKIOR
						float3 FresnelFunction =  SchlickIORFresnelFunction(_Ior, LdotH);
					#elif _FRESNELMODEL_SPHERICALGAUSSIAN
						float3 FresnelFunction = SphericalGaussianFresnelFunction(LdotH, specularColor);
				 	#else
						float3 FresnelFunction =  SchlickIORFresnelFunction(_Ior, LdotH);	
				 	#endif

                	float3 directSpecular = specularColor * ( SpecularDistribution * FresnelFunction * GeometricShadow) / (4  * (  NdotL * NdotV) );
 
				 	//NdotL有可能是负数.
			 	 	directSpecular = saturate(directSpecular) * _SpecularPower;

                #endif


                 

                //UNITY_METALLIC   OTHER_METALLIC

                #ifdef UNITY_METALLICTYPE
                	half surfaceReduction;
	                #ifdef UNITY_COLORSPACE_GAMMA
	                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
	                #else
	                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
	                #endif

                	half grazingTerm = saturate( gloss + specularMonochrome );
	                float3 indirectSpecular =  gi.indirect.specular * FresnelLerp(specularColor,grazingTerm,NdotV) *surfaceReduction;
                #else
                	float grazingTerm = saturate(roughness + _Metallic);
                	float3 indirectSpecular =  gi.indirect.specular * FresnelLerp(specularColor,grazingTerm,NdotV) * (1-roughness*roughness* roughness);
                #endif


                float3 specular = (directSpecular + indirectSpecular);
               
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));



                //
                float3 directDiffuse =  NdotL * attenColor;
                float3 indirectDiffuse  = gi.indirect.diffuse;

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
