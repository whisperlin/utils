// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:0,trmd:0,grmd:1,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:2,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33293,y:32666,varname:node_2865,prsc:2|diff-6059-OUT,spec-7736-A,gloss-1813-OUT,normal-5964-RGB,emission-2357-OUT,amdfl-1648-OUT,amspl-2015-OUT,clip-4604-OUT,voffset-6679-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32001,y:32512,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:31763,y:32632,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31790,y:32396,ptovrint:True,ptlb:Base Color(A-Specular),ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32446,y:32303,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:1813,x:31999,y:33026,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:0.7186728,max:1;n:type:ShaderForge.SFN_Slider,id:601,x:31167,y:34165,ptovrint:False,ptlb:HighLight Sharpness,ptin:_HighLightSharpness,varname:node_601,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tangent,id:8559,x:31494,y:34014,varname:node_8559,prsc:2;n:type:ShaderForge.SFN_Sin,id:6691,x:31494,y:34155,varname:node_6691,prsc:2|IN-601-OUT;n:type:ShaderForge.SFN_Add,id:1540,x:31684,y:34103,varname:node_1540,prsc:2|A-8559-OUT,B-6691-OUT;n:type:ShaderForge.SFN_Cross,id:1504,x:31872,y:34055,varname:node_1504,prsc:2|A-1373-OUT,B-1540-OUT;n:type:ShaderForge.SFN_NormalVector,id:1373,x:31704,y:33919,prsc:2,pt:False;n:type:ShaderForge.SFN_Normalize,id:5643,x:32070,y:34055,varname:node_5643,prsc:2|IN-1504-OUT;n:type:ShaderForge.SFN_Multiply,id:4448,x:32248,y:34133,varname:node_4448,prsc:2|A-5643-OUT,B-1013-OUT;n:type:ShaderForge.SFN_Vector1,id:1013,x:32070,y:34231,varname:node_1013,prsc:2,v1:-1;n:type:ShaderForge.SFN_Dot,id:9967,x:32479,y:34123,varname:node_9967,prsc:2,dt:0|A-4448-OUT,B-735-OUT;n:type:ShaderForge.SFN_Normalize,id:735,x:32298,y:34353,varname:node_735,prsc:2|IN-667-OUT;n:type:ShaderForge.SFN_Add,id:667,x:32119,y:34474,varname:node_667,prsc:2|A-1445-OUT,B-2261-OUT;n:type:ShaderForge.SFN_ViewVector,id:1445,x:31955,y:34388,varname:node_1445,prsc:2;n:type:ShaderForge.SFN_LightVector,id:2261,x:31965,y:34563,varname:node_2261,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9945,x:32695,y:34123,varname:node_9945,prsc:2|A-9967-OUT,B-9967-OUT;n:type:ShaderForge.SFN_OneMinus,id:5766,x:32884,y:34123,varname:node_5766,prsc:2|IN-9945-OUT;n:type:ShaderForge.SFN_Sqrt,id:3637,x:33065,y:34123,varname:node_3637,prsc:2|IN-5766-OUT;n:type:ShaderForge.SFN_Power,id:7978,x:33289,y:34123,varname:node_7978,prsc:2|VAL-3637-OUT,EXP-9511-OUT;n:type:ShaderForge.SFN_Vector1,id:9511,x:33087,y:34271,varname:node_9511,prsc:2,v1:100;n:type:ShaderForge.SFN_Multiply,id:2978,x:33638,y:34158,varname:node_2978,prsc:2|A-1997-OUT,B-7978-OUT,C-7901-OUT;n:type:ShaderForge.SFN_Slider,id:7901,x:33282,y:34354,ptovrint:False,ptlb:Highlight Intensity,ptin:_HighlightIntensity,varname:node_7901,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:0.525051,max:3;n:type:ShaderForge.SFN_LightVector,id:4887,x:33029,y:33791,varname:node_4887,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:6256,x:33029,y:33956,prsc:2,pt:True;n:type:ShaderForge.SFN_Dot,id:1997,x:33233,y:33868,varname:node_1997,prsc:2,dt:0|A-4887-OUT,B-6256-OUT;n:type:ShaderForge.SFN_Add,id:2524,x:32244,y:32557,varname:node_2524,prsc:2|A-6343-OUT,B-2978-OUT;n:type:ShaderForge.SFN_Divide,id:6059,x:32618,y:32555,varname:node_6059,prsc:2|A-2524-OUT,B-9639-OUT;n:type:ShaderForge.SFN_Slider,id:9639,x:32104,y:32790,ptovrint:False,ptlb:To Linear,ptin:_ToLinear,varname:node_9639,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:2.29305,max:3;n:type:ShaderForge.SFN_Slider,id:9506,x:34097,y:34352,ptovrint:False,ptlb:Dissolve Amount,ptin:_DissolveAmount,varname:node_3114,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:4550,x:34397,y:34120,ptovrint:False,ptlb:Dissolve Map,ptin:_DissolveMap,varname:node_8032,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:3,isnm:False;n:type:ShaderForge.SFN_OneMinus,id:257,x:34499,y:34380,varname:node_257,prsc:2|IN-9506-OUT;n:type:ShaderForge.SFN_RemapRange,id:3561,x:34707,y:34380,varname:node_3561,prsc:2,frmn:0,frmx:1,tomn:-0.6,tomx:0.6|IN-257-OUT;n:type:ShaderForge.SFN_Add,id:4604,x:34711,y:34115,varname:node_4604,prsc:2|A-3561-OUT,B-4550-R;n:type:ShaderForge.SFN_RemapRange,id:7017,x:35090,y:34165,varname:node_7017,prsc:2,frmn:0,frmx:1,tomn:-4,tomx:4|IN-4604-OUT;n:type:ShaderForge.SFN_Clamp,id:5505,x:35365,y:34166,varname:node_5505,prsc:2|IN-7017-OUT,MIN-5244-OUT,MAX-4207-OUT;n:type:ShaderForge.SFN_Vector1,id:5244,x:35213,y:34301,varname:node_5244,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:4207,x:35243,y:34397,varname:node_4207,prsc:2,v1:1;n:type:ShaderForge.SFN_OneMinus,id:195,x:35488,y:33993,varname:node_195,prsc:2|IN-5505-OUT;n:type:ShaderForge.SFN_Append,id:7169,x:35697,y:34018,varname:node_7169,prsc:2|A-195-OUT,B-9508-OUT;n:type:ShaderForge.SFN_Vector1,id:9508,x:35549,y:34247,varname:node_9508,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2d,id:243,x:35911,y:34095,ptovrint:False,ptlb:Ramp Map,ptin:_RampMap,varname:node_5948,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False|UVIN-7169-OUT;n:type:ShaderForge.SFN_VertexColor,id:3950,x:34478,y:33645,varname:node_3950,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9926,x:35312,y:33776,varname:node_9926,prsc:2|A-4188-TTR,B-7832-OUT,C-9506-OUT;n:type:ShaderForge.SFN_Time,id:4188,x:34977,y:33718,varname:node_4188,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7832,x:34799,y:33922,varname:node_7832,prsc:2|A-100-OUT,B-4550-RGB,C-3950-RGB;n:type:ShaderForge.SFN_Multiply,id:2357,x:34899,y:33246,varname:node_2357,prsc:2|A-195-OUT,B-243-RGB;n:type:ShaderForge.SFN_Sin,id:3574,x:35520,y:33524,varname:node_3574,prsc:2|IN-9926-OUT;n:type:ShaderForge.SFN_Slider,id:100,x:34278,y:33956,ptovrint:False,ptlb:Vertex Anim,ptin:_VertexAnim,varname:node_100,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Multiply,id:6679,x:35317,y:33202,varname:node_6679,prsc:2|A-3574-OUT,B-100-OUT;n:type:ShaderForge.SFN_Cubemap,id:9487,x:32787,y:32826,ptovrint:False,ptlb:node_9487,ptin:_node_9487,varname:node_9487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,pvfc:0;n:type:ShaderForge.SFN_Cubemap,id:9685,x:32767,y:33022,ptovrint:False,ptlb:SPL,ptin:_SPL,varname:node_9685,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,pvfc:0|DIR-2960-OUT;n:type:ShaderForge.SFN_Slider,id:1403,x:32533,y:33220,ptovrint:False,ptlb:SBL Intensity,ptin:_SBLIntensity,varname:node_1403,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:0,max:3;n:type:ShaderForge.SFN_Slider,id:2737,x:32567,y:33330,ptovrint:False,ptlb:IBL Intensity_copy,ptin:_IBLIntensity_copy,varname:_SPLIntensity_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:0,max:3;n:type:ShaderForge.SFN_Cubemap,id:9850,x:32980,y:33306,ptovrint:False,ptlb:IBL,ptin:_IBL,varname:_SPL_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,pvfc:0|DIR-4590-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:2960,x:32562,y:33022,varname:node_2960,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:4590,x:32807,y:33516,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:2015,x:33195,y:33200,varname:node_2015,prsc:2|A-1403-OUT,B-9685-RGB;n:type:ShaderForge.SFN_Multiply,id:1648,x:33292,y:33311,varname:node_1648,prsc:2|A-9850-RGB,B-2737-OUT;proporder:7736-6665-5964-1813-601-7901-9639-9506-4550-243-100-9685-1403-2737-9850;pass:END;sub:END;*/

Shader "DAFUHAO_Editor/Human-Hair-Effect-Art" {
    Properties {
        _MainTex ("Base Color(A-Specular)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Gloss ("Gloss", Range(0, 1)) = 0.7186728
        _HighLightSharpness ("HighLight Sharpness", Range(0, 1)) = 0
        _HighlightIntensity ("Highlight Intensity", Range(0, 3)) = 0.525051
        _ToLinear ("To Linear", Range(0, 3)) = 2.29305
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveMap ("Dissolve Map", 2D) = "bump" {}
        _RampMap ("Ramp Map", 2D) = "white" {}
        _VertexAnim ("Vertex Anim", Range(-10, 10)) = 0
        _SPL ("SPL", Cube) = "_Skybox" {}
        _SBLIntensity ("SBL Intensity", Range(0, 3)) = 0
        _IBLIntensity_copy ("IBL Intensity_copy", Range(0, 3)) = 0
        _IBL ("IBL", Cube) = "_Skybox" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            Stencil {
                Ref 128
                Pass Replace
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Gloss;
            uniform float _HighLightSharpness;
            uniform float _HighlightIntensity;
            uniform float _ToLinear;
            uniform float _DissolveAmount;
            uniform sampler2D _DissolveMap; uniform float4 _DissolveMap_ST;
            uniform sampler2D _RampMap; uniform float4 _RampMap_ST;
            uniform float _VertexAnim;
            uniform samplerCUBE _SPL;
            uniform float _SBLIntensity;
            uniform float _IBLIntensity_copy;
            uniform samplerCUBE _IBL;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_4188 = _Time;
                float4 _DissolveMap_var = tex2Dlod(_DissolveMap,float4(TRANSFORM_TEX(o.uv0, _DissolveMap),0.0,0));
                v.vertex.xyz += (sin((node_4188.a*(_VertexAnim*_DissolveMap_var.rgb*o.vertexColor.rgb)*_DissolveAmount))*_VertexAnim);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float4 _DissolveMap_var = tex2D(_DissolveMap,TRANSFORM_TEX(i.uv0, _DissolveMap));
                float node_4604 = (((1.0 - _DissolveAmount)*1.2+-0.6)+_DissolveMap_var.r);
                clip(node_4604 - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 1.0 - _Gloss; // Convert roughness to gloss
                float perceptualRoughness = _Gloss;
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
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 specularColor = float3(_MainTex_var.a,_MainTex_var.a,_MainTex_var.a);
                float specularMonochrome;
                float node_9967 = dot((normalize(cross(i.normalDir,(i.tangentDir+sin(_HighLightSharpness))))*(-1.0)),normalize((viewDirection+lightDirection)));
                float3 diffuseColor = (((_MainTex_var.rgb*_Color.rgb)+(dot(lightDirection,normalDirection)*pow(sqrt((1.0 - (node_9967*node_9967))),100.0)*_HighlightIntensity))/_ToLinear); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
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


                float3 indirectSpecular = (0 + (_SBLIntensity*texCUBE(_SPL,viewReflectDirection).rgb));
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
                indirectDiffuse += (texCUBE(_IBL,i.normalDir).rgb*_IBLIntensity_copy); // Diffuse Ambient Light
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float node_195 = (1.0 - clamp((node_4604*8.0+-4.0),0.0,1.0));
                float2 node_7169 = float2(node_195,0.0);
                float4 _RampMap_var = tex2D(_RampMap,TRANSFORM_TEX(node_7169, _RampMap));
                float3 emissive = (node_195*_RampMap_var.rgb);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
       