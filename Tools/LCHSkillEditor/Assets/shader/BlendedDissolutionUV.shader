// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4013,x:34116,y:32603,varname:node_4013,prsc:2|emission-2763-OUT,alpha-5178-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:33317,y:33234,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:3129,x:32440,y:32661,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_3129,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ca3832068ddb6d0468b6c503b93430e0,ntxv:0,isnm:False|UVIN-7679-OUT;n:type:ShaderForge.SFN_If,id:5832,x:32439,y:33069,varname:node_5832,prsc:2|A-2069-OUT,B-7116-R,GT-710-OUT,EQ-710-OUT,LT-4848-OUT;n:type:ShaderForge.SFN_If,id:1928,x:32437,y:33369,varname:node_1928,prsc:2|A-949-R,B-7116-R,GT-710-OUT,EQ-710-OUT,LT-4848-OUT;n:type:ShaderForge.SFN_VertexColor,id:949,x:31640,y:33012,varname:node_949,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9519,x:31730,y:32942,ptovrint:False,ptlb:Margin,ptin:_Margin,varname:node_9519,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.05;n:type:ShaderForge.SFN_Add,id:2069,x:32015,y:32971,varname:node_2069,prsc:2|A-9519-OUT,B-949-R;n:type:ShaderForge.SFN_Vector1,id:710,x:31962,y:33431,varname:node_710,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:4848,x:31933,y:33555,varname:node_4848,prsc:2,v1:0;n:type:ShaderForge.SFN_Subtract,id:148,x:32656,y:33261,varname:node_148,prsc:2|A-5832-OUT,B-1928-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2881,x:32639,y:33466,ptovrint:False,ptlb:MarginIntensity,ptin:_MarginIntensity,varname:node_2881,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:50;n:type:ShaderForge.SFN_Multiply,id:2827,x:32918,y:33295,varname:node_2827,prsc:2|A-148-OUT,B-2881-OUT;n:type:ShaderForge.SFN_Add,id:723,x:33114,y:33119,varname:node_723,prsc:2|A-5832-OUT,B-2827-OUT;n:type:ShaderForge.SFN_TexCoord,id:1644,x:30681,y:32614,varname:node_1644,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:1315,x:31009,y:32648,varname:node_1315,prsc:2,spu:0,spv:1|UVIN-1644-UVOUT,DIST-2891-OUT;n:type:ShaderForge.SFN_Time,id:607,x:30362,y:32763,varname:node_607,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2891,x:30601,y:32784,varname:node_2891,prsc:2|A-5238-OUT,B-607-T;n:type:ShaderForge.SFN_ValueProperty,id:5238,x:30392,y:32706,ptovrint:False,ptlb:VSpeed,ptin:_VSpeed,varname:node_5238,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Panner,id:2475,x:31121,y:32474,varname:node_2475,prsc:2,spu:1,spv:0|UVIN-1644-UVOUT,DIST-4567-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6982,x:30497,y:32371,ptovrint:False,ptlb:USpeed,ptin:_USpeed,varname:_node_5238_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Time,id:4733,x:30497,y:32425,varname:node_4733,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4567,x:30802,y:32431,varname:node_4567,prsc:2|A-6982-OUT,B-4733-T;n:type:ShaderForge.SFN_Lerp,id:7679,x:31396,y:32500,varname:node_7679,prsc:2|A-2475-UVOUT,B-1315-UVOUT,T-6968-OUT;n:type:ShaderForge.SFN_Vector1,id:6968,x:31196,y:32736,varname:node_6968,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Tex2d,id:7116,x:31691,y:33384,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:_Diffuse_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7ad82874593ce794a89ca9794733e6f3,ntxv:0,isnm:False|UVIN-3955-OUT;n:type:ShaderForge.SFN_Multiply,id:2519,x:33412,y:32821,varname:node_2519,prsc:2|A-3129-A,B-723-OUT;n:type:ShaderForge.SFN_Multiply,id:5178,x:33751,y:32907,varname:node_5178,prsc:2|A-2519-OUT,B-949-A,C-1304-A;n:type:ShaderForge.SFN_Multiply,id:2763,x:33363,y:32631,varname:node_2763,prsc:2|A-4461-OUT,B-723-OUT,C-1304-RGB;n:type:ShaderForge.SFN_ValueProperty,id:9400,x:32512,y:32411,ptovrint:False,ptlb:DiffuseIntensity,ptin:_DiffuseIntensity,varname:node_9400,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:4461,x:32910,y:32447,varname:node_4461,prsc:2|A-9400-OUT,B-3129-RGB;n:type:ShaderForge.SFN_TexCoord,id:6169,x:30149,y:33518,varname:node_6169,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:9104,x:30477,y:33552,varname:node_9104,prsc:2,spu:0,spv:1|UVIN-6169-UVOUT,DIST-8223-OUT;n:type:ShaderForge.SFN_Time,id:7567,x:29830,y:33667,varname:node_7567,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8223,x:30069,y:33688,varname:node_8223,prsc:2|A-6452-OUT,B-7567-T;n:type:ShaderForge.SFN_ValueProperty,id:6452,x:29860,y:33610,ptovrint:False,ptlb:NoiseVSpeed,ptin:_NoiseVSpeed,varname:_VSpeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Panner,id:84,x:30589,y:33378,varname:node_84,prsc:2,spu:1,spv:0|UVIN-6169-UVOUT,DIST-3791-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5423,x:29965,y:33275,ptovrint:False,ptlb:NoiseUSpeed,ptin:_NoiseUSpeed,varname:_USpeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Time,id:6281,x:29965,y:33329,varname:node_6281,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3791,x:30270,y:33335,varname:node_3791,prsc:2|A-5423-OUT,B-6281-T;n:type:ShaderForge.SFN_Lerp,id:3955,x:30864,y:33404,varname:node_3955,prsc:2|A-84-UVOUT,B-9104-UVOUT,T-4949-OUT;n:type:ShaderForge.SFN_Vector1,id:4949,x:30664,y:33640,varname:node_4949,prsc:2,v1:0.5;proporder:1304-3129-9400-5238-6982-7116-9519-2881-6452-5423;pass:END;sub:END;*/

Shader "<effect>/BlendedDissolutionUV" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
        _DiffuseIntensity ("DiffuseIntensity", Float ) = 1
        _VSpeed ("VSpeed", Float ) = 0
        _USpeed ("USpeed", Float ) = 1
        _Noise ("Noise", 2D) = "white" {}
        _Margin ("Margin", Float ) = 0.05
        _MarginIntensity ("MarginIntensity", Float ) = 50
        _NoiseVSpeed ("NoiseVSpeed", Float ) = 0
        _NoiseUSpeed ("NoiseUSpeed", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
			Fog { Mode off }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Margin;
            uniform float _MarginIntensity;
            uniform float _VSpeed;
            uniform float _USpeed;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _DiffuseIntensity;
            uniform float _NoiseVSpeed;
            uniform float _NoiseUSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_4733 = _Time + _TimeEditor;
                float4 node_607 = _Time + _TimeEditor;
                float2 node_7679 = lerp((i.uv0+(_USpeed*node_4733.g)*float2(1,0)),(i.uv0+(_VSpeed*node_607.g)*float2(0,1)),0.5);
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(node_7679, _Diffuse));
                float4 node_6281 = _Time + _TimeEditor;
                float4 node_7567 = _Time + _TimeEditor;
                float2 node_3955 = lerp((i.uv0+(_NoiseUSpeed*node_6281.g)*float2(1,0)),(i.uv0+(_NoiseVSpeed*node_7567.g)*float2(0,1)),0.5);
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_3955, _Noise));
                float node_5832_if_leA = step((_Margin+i.vertexColor.r),_Noise_var.r);
                float node_5832_if_leB = step(_Noise_var.r,(_Margin+i.vertexColor.r));
                float node_4848 = 0.0;
                float node_710 = 1.0;
                float node_5832 = lerp((node_5832_if_leA*node_4848)+(node_5832_if_leB*node_710),node_710,node_5832_if_leA*node_5832_if_leB);
                float node_1928_if_leA = step(i.vertexColor.r,_Noise_var.r);
                float node_1928_if_leB = step(_Noise_var.r,i.vertexColor.r);
                float node_723 = (node_5832+((node_5832-lerp((node_1928_if_leA*node_4848)+(node_1928_if_leB*node_710),node_710,node_1928_if_leA*node_1928_if_leB))*_MarginIntensity));
                float3 emissive = ((_DiffuseIntensity*_Diffuse_var.rgb)*node_723*_Color.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,((_Diffuse_var.a*node_723)*i.vertexColor.a*_Color.a));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
