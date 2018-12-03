// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33921,y:32660,varname:node_2865,prsc:2|rich5 out-6554-OUT;n:type:ShaderForge.SFN_Tex2d,id:8867,x:33267,y:32275,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_6358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9125,x:33581,y:31936,varname:node_9125,prsc:2|A-5127-RGB,B-1350-RGB;n:type:ShaderForge.SFN_Color,id:1350,x:32615,y:32271,ptovrint:False,ptlb:Emssion Color,ptin:_EmssionColor,varname:node_657,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Tex2d,id:5127,x:31063,y:32079,ptovrint:False,ptlb:Albedo,ptin:_Albedo,varname:node_5127,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:5497,x:33513,y:32892,varname:node_5497,prsc:2|A-9125-OUT,B-8096-OUT;n:type:ShaderForge.SFN_RichShadow,id:6554,x:33728,y:33163,varname:node_6554,prsc:2|IN-5497-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:2003,x:30775,y:32902,varname:node_2003,prsc:2;n:type:ShaderForge.SFN_Transform,id:9545,x:30969,y:33008,varname:node_9545,prsc:2,tffrom:0,tfto:1|IN-2003-XYZ;n:type:ShaderForge.SFN_ComponentMask,id:7416,x:31201,y:33041,varname:node_7416,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-9545-XYZ;n:type:ShaderForge.SFN_Smoothstep,id:1154,x:31263,y:32826,varname:node_1154,prsc:2|A-9878-OUT,B-1183-OUT,V-7416-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9878,x:31052,y:32743,ptovrint:False,ptlb:Min,ptin:_Min,varname:node_9878,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:5382,x:31599,y:32702,varname:node_5382,prsc:2|A-7149-RGB,B-1154-OUT,T-7149-A;n:type:ShaderForge.SFN_Multiply,id:2201,x:32043,y:32489,varname:node_2201,prsc:2|A-7779-OUT,B-5382-OUT;n:type:ShaderForge.SFN_Color,id:7149,x:31294,y:32603,ptovrint:False,ptlb:shaodowColor,ptin:_shaodowColor,varname:node_7149,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,c1:1,c2:1,c3:1,c4:0;n:type:ShaderForge.SFN_ValueProperty,id:1183,x:30951,y:32686,ptovrint:False,ptlb:Max,ptin:_Max,varname:node_1183,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,v1:0;n:type:ShaderForge.SFN_Divide,id:7779,x:31784,y:32266,varname:node_7779,prsc:2|A-9096-OUT,B-7487-OUT;n:type:ShaderForge.SFN_Slider,id:7487,x:31419,y:32424,ptovrint:False,ptlb:To Linrar,ptin:_ToLinrar,varname:node_7487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:-1,cur:1,max:2.2;n:type:ShaderForge.SFN_Multiply,id:9096,x:31450,y:32237,varname:node_9096,prsc:2|A-8468-OUT,B-2150-OUT;n:type:ShaderForge.SFN_Slider,id:6571,x:30730,y:31734,ptovrint:False,ptlb:AO Power,ptin:_AOPower,varname:node_6571,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Vector3,id:9212,x:31012,y:31981,varname:node_9212,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_Lerp,id:8468,x:31424,y:31895,varname:node_8468,prsc:2|A-8749-RGB,B-9212-OUT,T-6571-OUT;n:type:ShaderForge.SFN_Slider,id:983,x:31694,y:33108,ptovrint:False,ptlb:DiffIBL Power ,ptin:_DiffIBLPower,varname:node_983,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Multiply,id:6423,x:32214,y:32777,varname:node_6423,prsc:2|A-4994-RGB,B-983-OUT;n:type:ShaderForge.SFN_Color,id:125,x:31028,y:32300,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_125,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:2150,x:31238,y:32264,varname:node_2150,prsc:2|A-5127-RGB,B-125-RGB;n:type:ShaderForge.SFN_PBRLod2,id:8096,x:33504,y:32592,varname:node_8096,prsc:2|MainTex-2201-OUT,Metallic_alpha-8867-A,Metallic_red-8867-R,DiffCubemap-6423-OUT;n:type:ShaderForge.SFN_Color,id:4994,x:31886,y:32865,ptovrint:False,ptlb:DiffIBL,ptin:_DiffIBL,varname:node_4994,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:8749,x:31055,y:31293,ptovrint:False,ptlb:AO,ptin:_AO,varname:node_6301,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False|UVIN-306-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:306,x:30847,y:31264,varname:node_306,prsc:2,uv:1,uaff:False;proporder:125-5127-1350-8867-983-9878-7149-1183-7487-6571-4994-8749;pass:END;sub:END;*/

Shader "DAFUHAO_Editor/Building-High-Art-Lod2-no-sp" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Albedo ("Albedo", 2D) = "white" {}
        _EmssionColor ("Emssion Color", Color) = (0,0,0,1)
        _Metallic ("Metallic", 2D) = "black" {}
        _DiffIBLPower ("DiffIBL Power ", Range(0, 3)) = 1
        _Min ("Min", Float ) = 0
        _shaodowColor ("shaodowColor", Color) = (1,1,1,0)
        _Max ("Max", Float ) = 0
        _ToLinrar ("To Linrar", Range(-1, 2.2)) = 1
        _AOPower ("AO Power", Range(0, 1)) = 1
        _DiffIBL ("DiffIBL", Color) = (0.5,0.5,0.5,1)
        _AO ("AO", 2D) = "white" {}
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
            #pragma only_renderers gles3 
            #pragma target 3.0
            uniform sampler2D _Metallic; uniform float4 _Metallic_ST;
            uniform float4 _EmssionColor;
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform float _Min;
            uniform float4 _shaodowColor;
            uniform float _Max;
            uniform float _ToLinrar;
            uniform float _AOPower;
            uniform float _DiffIBLPower;
            uniform float4 _Color;
            uniform float4 _DiffIBL;
            uniform sampler2D _AO; uniform float4 _AO_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
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
                UNITY_FOG_COORDS(6)
            UNITY_VERTEX_INPUT_INSTANCE_ID
            
            #ifdef UseUnityShadow
            LIGHTING_COORDS(8, 9)
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
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(i.uv0, _Albedo));
                float4 _Metallic_var = tex2D(_Metallic,TRANSFORM_TEX(i.uv0, _Metallic));
                float4 _AO_var = tex2D(_AO,TRANSFORM_TEX(i.uv1, _AO));
                float node_1154 = smoothstep( _Min, _Max, mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb.g );
                float4 finalColor = FINAL_SHADOW_COLOR_SINGLE(((_Albedo_var.rgb*_EmssionColor.rgb)+PBRLow2(normalDirection, 
                                    viewDirection,float3(0,0,0),_Metallic_var.a,_Metallic_var.r,(((lerp(_AO_var.rgb,float3(1,1,1),_AOPower)*(_Albedo_var.rgb*_Color.rgb))/_ToLinrar)*lerp(_shaodowColor.rgb,float3(node_1154,node_1154,node_1154),_shaodowColor.a)),(_DiffIBL.rgb*_DiffIBLPower))), i, normalDirection);
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
