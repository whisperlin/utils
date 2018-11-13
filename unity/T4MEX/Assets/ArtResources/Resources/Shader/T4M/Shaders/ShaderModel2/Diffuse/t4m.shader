// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:1,uamb:False,mssp:True,bkdf:True,hqlp:True,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33087,y:32757,varname:node_3138,prsc:2|diff-2554-OUT,spec-5163-OUT,gloss-9288-OUT,normal-3903-OUT;n:type:ShaderForge.SFN_Slider,id:5163,x:32388,y:32911,ptovrint:False,ptlb:metallic,ptin:_metallic,varname:node_5163,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:9288,x:32448,y:33047,ptovrint:False,ptlb:gloss,ptin:_gloss,varname:node_9288,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:3385,x:31212,y:31924,varname:node_3385,prsc:2|A-8735-OUT,B-4407-OUT,T-4370-R;n:type:ShaderForge.SFN_Tex2d,id:5589,x:30450,y:31490,ptovrint:False,ptlb:Base,ptin:_Base,varname:node_432,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:150,x:30473,y:31791,ptovrint:False,ptlb:r,ptin:_r,varname:node_459,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4370,x:30604,y:32349,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_8532,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2063,x:31497,y:32072,varname:node_2063,prsc:2|A-3385-OUT,B-2663-OUT,T-4370-G;n:type:ShaderForge.SFN_Tex2d,id:9817,x:30626,y:32145,ptovrint:False,ptlb:g,ptin:_g,varname:_g_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2907,x:31634,y:32440,varname:node_2907,prsc:2|A-2063-OUT,B-942-OUT,T-4370-B;n:type:ShaderForge.SFN_Tex2d,id:6803,x:30611,y:32618,ptovrint:False,ptlb:b,ptin:_b,varname:node_7544,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2541,x:31183,y:32603,ptovrint:False,ptlb:a,ptin:_a,varname:_a_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2554,x:31790,y:32556,varname:node_2554,prsc:2|A-2907-OUT,B-7775-OUT,T-4370-A;n:type:ShaderForge.SFN_Tex2d,id:3640,x:31092,y:32883,ptovrint:False,ptlb:Base_n,ptin:_Base_n,varname:node_3640,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:5511,x:31716,y:32918,varname:node_5511,prsc:2|A-3640-RGB,B-4708-RGB,T-4370-R;n:type:ShaderForge.SFN_Tex2d,id:4708,x:31310,y:33099,ptovrint:False,ptlb:r_N,ptin:_r_N,varname:_r_N_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:5011,x:31338,y:33301,ptovrint:False,ptlb:g_N,ptin:_g_N,varname:_g_N_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:6475,x:31360,y:33536,ptovrint:False,ptlb:b_N,ptin:_b_N,varname:_b_N_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:8344,x:31371,y:33777,ptovrint:False,ptlb:a_N,ptin:_a_N,varname:_a_N_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:2132,x:31833,y:33167,varname:node_2132,prsc:2|A-5511-OUT,B-5011-RGB,T-4370-G;n:type:ShaderForge.SFN_Lerp,id:7985,x:32018,y:33325,varname:node_7985,prsc:2|A-2132-OUT,B-6475-RGB,T-4370-B;n:type:ShaderForge.SFN_Lerp,id:6487,x:32133,y:33579,varname:node_6487,prsc:2|A-7985-OUT,B-8344-RGB,T-4370-A;n:type:ShaderForge.SFN_Tex2d,id:919,x:32235,y:33884,ptovrint:False,ptlb:F_N,ptin:_F_N,varname:node_919,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Add,id:3903,x:32477,y:33724,varname:node_3903,prsc:2|A-6487-OUT,B-919-RGB;n:type:ShaderForge.SFN_Slider,id:7201,x:30331,y:32033,ptovrint:False,ptlb:ri,ptin:_ri,varname:node_7201,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:1,max:2;n:type:ShaderForge.SFN_Divide,id:4407,x:30805,y:31817,varname:node_4407,prsc:2|A-150-RGB,B-7201-OUT;n:type:ShaderForge.SFN_Divide,id:2663,x:30789,y:32061,varname:node_2663,prsc:2|A-9817-RGB,B-5207-OUT;n:type:ShaderForge.SFN_Divide,id:942,x:30891,y:32701,varname:node_942,prsc:2|A-6803-RGB,B-8473-OUT;n:type:ShaderForge.SFN_Divide,id:7775,x:30884,y:33140,varname:node_7775,prsc:2|A-2541-RGB,B-7886-OUT;n:type:ShaderForge.SFN_Slider,id:7886,x:30300,y:33156,ptovrint:False,ptlb:ai,ptin:_ai,varname:_ri_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:5207,x:30195,y:32331,ptovrint:False,ptlb:gi,ptin:_gi,varname:node_5207,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:8473,x:30332,y:32786,ptovrint:False,ptlb:bi,ptin:_bi,varname:node_8473,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:4917,x:30450,y:31681,ptovrint:False,ptlb:Basei,ptin:_Basei,varname:node_4917,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:1,max:2;n:type:ShaderForge.SFN_Divide,id:8735,x:30837,y:31630,varname:node_8735,prsc:2|A-5589-RGB,B-4917-OUT;proporder:5163-9288-5589-3640-4917-150-4708-7201-9817-5011-5207-6803-6475-8473-2541-8344-7886-4370-919;pass:END;sub:END;*/

Shader "Rolan/t4m_old" {
    Properties {
        _metallic ("metallic", Range(0, 1)) = 0
        _gloss ("gloss", Range(0, 1)) = 0
        _Base ("Base", 2D) = "white" {}
        _Base_n ("Base_n", 2D) = "bump" {}
        _Basei ("Basei", Range(0.5, 2)) = 1
        _r ("r", 2D) = "white" {}
        _r_N ("r_N", 2D) = "bump" {}
        _ri ("ri", Range(0.5, 2)) = 1
        _g ("g", 2D) = "white" {}
        _g_N ("g_N", 2D) = "bump" {}
        _gi ("gi", Range(0.5, 2)) = 1
        _b ("b", 2D) = "white" {}
        _b_N ("b_N", 2D) = "bump" {}
        _bi ("bi", Range(0.5, 2)) = 1
        _a ("a", 2D) = "white" {}
        _a_N ("a_N", 2D) = "bump" {}
        _ai ("ai", Range(0.5, 2)) = 1
        _mask ("mask", 2D) = "white" {}
        _F_N ("F_N", 2D) = "bump" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque" "Queue"="Geometry-100"
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float _metallic;
            uniform float _gloss;
            uniform sampler2D _Base; uniform float4 _Base_ST;
            uniform sampler2D _r; uniform float4 _r_ST;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform sampler2D _g; uniform float4 _g_ST;
            uniform sampler2D _b; uniform float4 _b_ST;
            uniform sampler2D _a; uniform float4 _a_ST;
            uniform sampler2D _Base_n; uniform float4 _Base_n_ST;
            uniform sampler2D _r_N; uniform float4 _r_N_ST;
            uniform sampler2D _g_N; uniform float4 _g_N_ST;
            uniform sampler2D _b_N; uniform float4 _b_N_ST;
            uniform sampler2D _a_N; uniform float4 _a_N_ST;
            uniform sampler2D _F_N; uniform float4 _F_N_ST;
            uniform float _ri;
            uniform float _ai;
            uniform float _gi;
            uniform float _bi;
            uniform float _Basei;
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
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Base_n_var = UnpackNormal(tex2D(_Base_n,TRANSFORM_TEX(i.uv0, _Base_n)));
                float3 _r_N_var = UnpackNormal(tex2D(_r_N,TRANSFORM_TEX(i.uv0, _r_N)));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float3 _g_N_var = UnpackNormal(tex2D(_g_N,TRANSFORM_TEX(i.uv0, _g_N)));
                float3 _b_N_var = UnpackNormal(tex2D(_b_N,TRANSFORM_TEX(i.uv0, _b_N)));
                float3 _a_N_var = UnpackNormal(tex2D(_a_N,TRANSFORM_TEX(i.uv0, _a_N)));
                float3 _F_N_var = UnpackNormal(tex2D(_F_N,TRANSFORM_TEX(i.uv0, _F_N)));
                float3 normalLocal = (lerp(lerp(lerp(lerp(_Base_n_var.rgb,_r_N_var.rgb,_mask_var.r),_g_N_var.rgb,_mask_var.g),_b_N_var.rgb,_mask_var.b),_a_N_var.rgb,_mask_var.a)+_F_N_var.rgb);
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
                float gloss = 1.0 - _gloss; // Convert roughness to gloss
                float perceptualRoughness = _gloss;
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
                float3 specularColor = _metallic;
                float specularMonochrome;
                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));
                float4 _r_var = tex2D(_r,TRANSFORM_TEX(i.uv0, _r));
                float4 _g_var = tex2D(_g,TRANSFORM_TEX(i.uv0, _g));
                float4 _b_var = tex2D(_b,TRANSFORM_TEX(i.uv0, _b));
                float4 _a_var = tex2D(_a,TRANSFORM_TEX(i.uv0, _a));
                float3 diffuseColor = lerp(lerp(lerp(lerp((_Base_var.rgb/_Basei),(_r_var.rgb/_ri),_mask_var.r),(_g_var.rgb/_gi),_mask_var.g),(_b_var.rgb/_bi),_mask_var.b),(_a_var.rgb/_ai),_mask_var.a); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float _metallic;
            uniform float _gloss;
            uniform sampler2D _Base; uniform float4 _Base_ST;
            uniform sampler2D _r; uniform float4 _r_ST;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform sampler2D _g; uniform float4 _g_ST;
            uniform sampler2D _b; uniform float4 _b_ST;
            uniform sampler2D _a; uniform float4 _a_ST;
            uniform sampler2D _Base_n; uniform float4 _Base_n_ST;
            uniform sampler2D _r_N; uniform float4 _r_N_ST;
            uniform sampler2D _g_N; uniform float4 _g_N_ST;
            uniform sampler2D _b_N; uniform float4 _b_N_ST;
            uniform sampler2D _a_N; uniform float4 _a_N_ST;
            uniform sampler2D _F_N; uniform float4 _F_N_ST;
            uniform float _ri;
            uniform float _ai;
            uniform float _gi;
            uniform float _bi;
            uniform float _Basei;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
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
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Base_n_var = UnpackNormal(tex2D(_Base_n,TRANSFORM_TEX(i.uv0, _Base_n)));
                float3 _r_N_var = UnpackNormal(tex2D(_r_N,TRANSFORM_TEX(i.uv0, _r_N)));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float3 _g_N_var = UnpackNormal(tex2D(_g_N,TRANSFORM_TEX(i.uv0, _g_N)));
                float3 _b_N_var = UnpackNormal(tex2D(_b_N,TRANSFORM_TEX(i.uv0, _b_N)));
                float3 _a_N_var = UnpackNormal(tex2D(_a_N,TRANSFORM_TEX(i.uv0, _a_N)));
                float3 _F_N_var = UnpackNormal(tex2D(_F_N,TRANSFORM_TEX(i.uv0, _F_N)));
                float3 normalLocal = (lerp(lerp(lerp(lerp(_Base_n_var.rgb,_r_N_var.rgb,_mask_var.r),_g_N_var.rgb,_mask_var.g),_b_N_var.rgb,_mask_var.b),_a_N_var.rgb,_mask_var.a)+_F_N_var.rgb);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 1.0 - _gloss; // Convert roughness to gloss
                float perceptualRoughness = _gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _metallic;
                float specularMonochrome;
                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));
                float4 _r_var = tex2D(_r,TRANSFORM_TEX(i.uv0, _r));
                float4 _g_var = tex2D(_g,TRANSFORM_TEX(i.uv0, _g));
                float4 _b_var = tex2D(_b,TRANSFORM_TEX(i.uv0, _b));
                float4 _a_var = tex2D(_a,TRANSFORM_TEX(i.uv0, _a));
                float3 diffuseColor = lerp(lerp(lerp(lerp((_Base_var.rgb/_Basei),(_r_var.rgb/_ri),_mask_var.r),(_g_var.rgb/_gi),_mask_var.g),(_b_var.rgb/_bi),_mask_var.b),(_a_var.rgb/_ai),_mask_var.a); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float _metallic;
            uniform float _gloss;
            uniform sampler2D _Base; uniform float4 _Base_ST;
            uniform sampler2D _r; uniform float4 _r_ST;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform sampler2D _g; uniform float4 _g_ST;
            uniform sampler2D _b; uniform float4 _b_ST;
            uniform sampler2D _a; uniform float4 _a_ST;
            uniform float _ri;
            uniform float _ai;
            uniform float _gi;
            uniform float _bi;
            uniform float _Basei;
            struct VertexInput {
                float4 vertex : POSITION;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));
                float4 _r_var = tex2D(_r,TRANSFORM_TEX(i.uv0, _r));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float4 _g_var = tex2D(_g,TRANSFORM_TEX(i.uv0, _g));
                float4 _b_var = tex2D(_b,TRANSFORM_TEX(i.uv0, _b));
                float4 _a_var = tex2D(_a,TRANSFORM_TEX(i.uv0, _a));
                float3 diffColor = lerp(lerp(lerp(lerp((_Base_var.rgb/_Basei),(_r_var.rgb/_ri),_mask_var.r),(_g_var.rgb/_gi),_mask_var.g),(_b_var.rgb/_bi),_mask_var.b),(_a_var.rgb/_ai),_mask_var.a);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _metallic, specColor, specularMonochrome );
                float roughness = _gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
