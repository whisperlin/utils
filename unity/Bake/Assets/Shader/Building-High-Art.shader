// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.7507569,fgcg:0.8665211,fgcb:0.9632353,fgca:1,fgde:0.2,fgrn:400,fgrf:2500,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33921,y:32660,varname:node_2865,prsc:2|normal-3747-OUT,rich5 out-6554-OUT;n:type:ShaderForge.SFN_Tex2d,id:1640,x:33292,y:32280,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:8867,x:32193,y:32878,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_6358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Cubemap,id:6955,x:32226,y:33143,ptovrint:False,ptlb:SpecIBL,ptin:_SpecIBL,varname:_CubeMap_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,pvfc:0|DIR-1220-OUT;n:type:ShaderForge.SFN_Slider,id:958,x:32445,y:33291,ptovrint:False,ptlb:SpecIBL Power,ptin:_SpecIBLPower,varname:_CubeDiff_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:0.87,max:10;n:type:ShaderForge.SFN_Multiply,id:2743,x:32937,y:33172,varname:node_2743,prsc:2|A-6955-RGB,B-958-OUT,C-8468-OUT;n:type:ShaderForge.SFN_Tex2d,id:8910,x:32397,y:32022,ptovrint:False,ptlb:Emssion Map,ptin:_EmssionMap,varname:node_3401,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9125,x:32748,y:32232,varname:node_9125,prsc:2|A-8910-RGB,B-1350-RGB;n:type:ShaderForge.SFN_Color,id:1350,x:32370,y:32281,ptovrint:False,ptlb:Emssion Color,ptin:_EmssionColor,varname:node_657,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_PBR,id:4864,x:33420,y:32602,varname:node_4864,prsc:2|MainTex-2201-OUT,Metallic_alpha-8867-A,Metallic_red-8867-R,DiffCubemap-6423-OUT,SpecCubemap-2743-OUT;n:type:ShaderForge.SFN_Tex2d,id:5127,x:31063,y:32079,ptovrint:False,ptlb:Albedo,ptin:_Albedo,varname:node_5127,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:5497,x:33610,y:32896,varname:node_5497,prsc:2|A-9125-OUT,B-4864-OUT;n:type:ShaderForge.SFN_RichShadow,id:6554,x:33677,y:33222,varname:node_6554,prsc:2|IN-5497-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:2003,x:30775,y:32902,varname:node_2003,prsc:2;n:type:ShaderForge.SFN_Transform,id:9545,x:30969,y:33008,varname:node_9545,prsc:2,tffrom:0,tfto:1|IN-2003-XYZ;n:type:ShaderForge.SFN_ComponentMask,id:7416,x:31201,y:33041,varname:node_7416,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-9545-XYZ;n:type:ShaderForge.SFN_Smoothstep,id:1154,x:31263,y:32826,varname:node_1154,prsc:2|A-9878-OUT,B-1183-OUT,V-7416-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9878,x:31052,y:32743,ptovrint:False,ptlb:Min,ptin:_Min,varname:node_9878,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:5382,x:31599,y:32702,varname:node_5382,prsc:2|A-7149-RGB,B-1154-OUT,T-7149-A;n:type:ShaderForge.SFN_Multiply,id:2201,x:32043,y:32489,varname:node_2201,prsc:2|A-7779-OUT,B-5382-OUT;n:type:ShaderForge.SFN_Color,id:7149,x:31294,y:32603,ptovrint:False,ptlb:shaodowColor,ptin:_shaodowColor,varname:node_7149,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,c1:1,c2:1,c3:1,c4:0;n:type:ShaderForge.SFN_ValueProperty,id:1183,x:30951,y:32686,ptovrint:False,ptlb:Max,ptin:_Max,varname:node_1183,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,v1:0;n:type:ShaderForge.SFN_Divide,id:7779,x:31784,y:32266,varname:node_7779,prsc:2|A-9096-OUT,B-7487-OUT;n:type:ShaderForge.SFN_Slider,id:7487,x:31419,y:32424,ptovrint:False,ptlb:To Linrar,ptin:_ToLinrar,varname:node_7487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:-1,cur:1,max:2.2;n:type:ShaderForge.SFN_Tex2d,id:6301,x:30849,y:31758,ptovrint:False,ptlb:AO,ptin:_AO,varname:node_6301,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False|UVIN-3317-UVOUT;n:type:ShaderForge.SFN_Multiply,id:9096,x:31450,y:32237,varname:node_9096,prsc:2|A-8468-OUT,B-2150-OUT;n:type:ShaderForge.SFN_TexCoord,id:3317,x:30627,y:31666,varname:node_3317,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Slider,id:6571,x:30627,y:31456,ptovrint:False,ptlb:AO Power,ptin:_AOPower,varname:node_6571,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Vector3,id:9212,x:31045,y:31850,varname:node_9212,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_Lerp,id:8468,x:31424,y:31895,varname:node_8468,prsc:2|A-6301-RGB,B-9212-OUT,T-6571-OUT;n:type:ShaderForge.SFN_Cubemap,id:706,x:32409,y:32650,ptovrint:False,ptlb:DiffIBL,ptin:_DiffIBL,varname:node_706,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,pvfc:0|DIR-6336-OUT;n:type:ShaderForge.SFN_Slider,id:983,x:32386,y:32994,ptovrint:False,ptlb:DiffIBL Power ,ptin:_DiffIBLPower,varname:node_983,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Multiply,id:6423,x:32754,y:32789,varname:node_6423,prsc:2|A-706-RGB,B-983-OUT;n:type:ShaderForge.SFN_NormalVector,id:6336,x:32193,y:32650,prsc:2,pt:True;n:type:ShaderForge.SFN_ViewReflectionVector,id:1220,x:32017,y:33108,varname:node_1220,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5526,x:33913,y:32138,varname:node_5526,prsc:2|A-1036-OUT,B-2395-OUT;n:type:ShaderForge.SFN_Append,id:1036,x:33595,y:32292,varname:node_1036,prsc:2|A-1640-R,B-1640-G;n:type:ShaderForge.SFN_Append,id:3747,x:33813,y:32454,varname:node_3747,prsc:2|A-5526-OUT,B-1640-B;n:type:ShaderForge.SFN_Slider,id:2395,x:33451,y:32120,ptovrint:False,ptlb:Normal Intensiy,ptin:_NormalIntensiy,varname:node_2395,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Multiply,id:7954,x:32816,y:32577,varname:node_7954,prsc:2|A-4549-OUT,B-8867-R;n:type:ShaderForge.SFN_Multiply,id:9825,x:33069,y:32831,varname:node_9825,prsc:2|A-3592-OUT,B-8867-A;n:type:ShaderForge.SFN_Slider,id:4549,x:32409,y:32474,ptovrint:False,ptlb:MetallicIn,ptin:_MetallicIn,varname:node_4549,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:1,max:1.1;n:type:ShaderForge.SFN_Slider,id:3592,x:32793,y:32994,ptovrint:False,ptlb:GlossIn,ptin:_GlossIn,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:2333,x:33286,y:33031,varname:node_2333,prsc:2|A-8231-OUT,B-2743-OUT;n:type:ShaderForge.SFN_Vector1,id:8231,x:33170,y:33270,varname:node_8231,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:8069,x:33467,y:32486,varname:node_8069,prsc:2,v1:1;n:type:ShaderForge.SFN_Color,id:125,x:31028,y:32300,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_125,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:2150,x:31238,y:32264,varname:node_2150,prsc:2|A-5127-RGB,B-125-RGB;proporder:125-5127-8910-1350-1640-2395-8867-706-983-6955-958-9878-7149-1183-7487-6301-6571-4549-3592;pass:END;sub:END;*/

Shader "DAFUHAO_Editor/Building-High-Art" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Albedo ("Albedo", 2D) = "white" {}
        _EmssionMap ("Emssion Map", 2D) = "white" {}
        _EmssionColor ("Emssion Color", Color) = (0,0,0,1)
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensiy ("Normal Intensiy", Range(0, 2)) = 1
        _Metallic ("Metallic", 2D) = "black" {}
        _DiffIBL ("DiffIBL", Cube) = "_Skybox" {}
        _DiffIBLPower ("DiffIBL Power ", Range(0, 3)) = 1
        _SpecIBL ("SpecIBL", Cube) = "_Skybox" {}
        _SpecIBLPower ("SpecIBL Power", Range(0, 10)) = 0.87
        _Min ("Min", Float ) = 0
        _shaodowColor ("shaodowColor", Color) = (1,1,1,0)
        _Max ("Max", Float ) = 0
        _ToLinrar ("To Linrar", Range(-1, 2.2)) = 1
        _AO ("AO", 2D) = "white" {}
        _AOPower ("AO Power", Range(0, 1)) = 1
        _MetallicIn ("MetallicIn", Range(0, 1.1)) = 1
        _GlossIn ("GlossIn", Range(0, 1)) = 1
        [HideInInspector]_SrcBlend("", Float) = 1
        [HideInInspector]_DstBlend("", Float) = 0
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
            Blend [_SrcBlend][_DstBlend]
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile __ BAKEMOD_ON
            #include "UnityCG.cginc"
            #pragma multi_compile __ AlphaBlendOn UseUnityShadow
            #pragma multi_compile_instancing
            #include "AutoLight.cginc"
            #include "../CGIncludes/DiffuseInfo.cginc"
            
            
            INSTANCING_START
            INSTANCING_PROP_UVST
            INSTANCING_END
            
            
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d11 gles3 
            #pragma target 3.0
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _Metallic; uniform float4 _Metallic_ST;
            uniform samplerCUBE _SpecIBL;
            uniform float _SpecIBLPower;
            uniform sampler2D _EmssionMap; uniform float4 _EmssionMap_ST;
            uniform float4 _EmssionColor;
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform float _Min;
            uniform float4 _shaodowColor;
            uniform float _Max;
            uniform float _ToLinrar;
            uniform sampler2D _AO; uniform float4 _AO_ST;
            uniform float _AOPower;
            uniform samplerCUBE _DiffIBL;
            uniform float _DiffIBLPower;
            uniform float _NormalIntensiy;
            uniform float4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                #ifdef BAKEMOD_ON
                float2 texcoord2 : TEXCOORD2;
                #endif
            UNITY_VERTEX_INPUT_INSTANCE_ID
            
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float4 viewPos : TEXCOORD3;
                float4 worldPos_2_Camera : TEXCOORD4;
                float3 normalDir : TEXCOORD5;
                float3 tangentDir : TEXCOORD6;
                float3 bitangentDir : TEXCOORD7;
                UNITY_FOG_COORDS(8)
            UNITY_VERTEX_INPUT_INSTANCE_ID
            
            #ifdef UseUnityShadow
            LIGHTING_COORDS(10, 11)
            #endif
            };
            #include "../CGIncludes/shadow.cginc"
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_INITIALIZE_OUTPUT(VertexOutput, o);
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                #ifdef UseUnityShadow
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                #endif
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                GET_CAMERA_POS_NORMAL(o.posWorld, o.viewPos, o.worldPos_2_Camera, o.normalDir);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                #ifdef BAKEMOD_ON
                o.pos.xy = v.texcoord2*2-float2(1,1);
                o.pos.z = 0.5;
                o.pos.w = 1;
                #endif
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
            UNITY_SETUP_INSTANCE_ID(i);
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = float3((float2(_BumpMap_var.r,_BumpMap_var.g)*_NormalIntensiy),_BumpMap_var.b);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
                float4 _EmssionMap_var = tex2D(_EmssionMap,TRANSFORM_TEX(i.uv0, _EmssionMap));
                float4 _Metallic_var = tex2D(_Metallic,TRANSFORM_TEX(i.uv0, _Metallic));
                float4 _AO_var = tex2D(_AO,TRANSFORM_TEX(i.uv1, _AO));
                float3 node_8468 = lerp(_AO_var.rgb,float3(1,1,1),_AOPower);
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(i.uv0, _Albedo));
                float node_1154 = smoothstep( _Min, _Max, mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb.g );
                float3 node_2743 = (texCUBE(_SpecIBL,viewReflectDirection).rgb*_SpecIBLPower*node_8468);
                float4 finalColor = FINAL_SHADOW_COLOR_SINGLE(((_EmssionMap_var.rgb*_EmssionColor.rgb)+PBRSpecular(normalDirection, 
                                    viewDirection,float3(0,0,0),_Metallic_var.a,_Metallic_var.r,(((node_8468*(_Albedo_var.rgb*_Color.rgb))/_ToLinrar)*lerp(_shaodowColor.rgb,float3(node_1154,node_1154,node_1154),_shaodowColor.a)),(texCUBE(_DiffIBL,normalDirection).rgb*_DiffIBLPower),node_2743)), i, normalDirection);
                finalColor.a *= 1;
                #ifdef BAKEMOD_ON
                float4  res = 1;
                res.rgb = (finalColor.rgb - _Albedo_var.rgb )*0.5 + 0.5;
                return res;
                #endif
                UNITY_APPLY_FOG(i.fogCoord, finalColor.rgb);
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
