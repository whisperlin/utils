// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33444,y:32898,varname:node_3138,prsc:2|normal-2932-RGB,rich5 out-5609-OUT;n:type:ShaderForge.SFN_Tex2d,id:824,x:32185,y:32793,ptovrint:False,ptlb:Splat0,ptin:_Splat0,varname:node_2508,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8014,x:32185,y:32983,ptovrint:False,ptlb:Splat1,ptin:_Splat1,varname:node_6401,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9937,x:32185,y:33189,ptovrint:False,ptlb:Splat2,ptin:_Splat2,varname:node_9165,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1400,x:32185,y:33383,ptovrint:False,ptlb:Splat3,ptin:_Splat3,varname:node_5154,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6355,x:32406,y:32721,ptovrint:False,ptlb:Control,ptin:_Control,varname:node_9874,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1576,x:32490,y:32932,varname:node_1576,prsc:2|A-6355-R,B-824-RGB;n:type:ShaderForge.SFN_Multiply,id:9097,x:32492,y:33048,varname:node_9097,prsc:2|A-6355-G,B-8014-RGB;n:type:ShaderForge.SFN_Multiply,id:6940,x:32492,y:33168,varname:node_6940,prsc:2|A-6355-B,B-9937-RGB;n:type:ShaderForge.SFN_Multiply,id:9590,x:32492,y:33302,varname:node_9590,prsc:2|A-6355-A,B-1400-RGB;n:type:ShaderForge.SFN_Add,id:6931,x:32777,y:33302,varname:node_6931,prsc:2|A-1576-OUT,B-9097-OUT,C-6940-OUT,D-9590-OUT;n:type:ShaderForge.SFN_Multiply,id:9826,x:33132,y:32929,varname:node_9826,prsc:2|A-4994-OUT,B-1784-OUT,C-1645-RGB;n:type:ShaderForge.SFN_RichShadow,id:5609,x:33198,y:33317,varname:node_5609,prsc:2|IN-9826-OUT;n:type:ShaderForge.SFN_Color,id:1645,x:32796,y:32752,ptovrint:False,ptlb:color,ptin:_color,varname:node_8587,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Enviment,id:1784,x:32796,y:32892,varname:node_1784,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:3292,x:31332,y:32808,ptovrint:False,ptlb:Splat4,ptin:_Splat4,varname:_Splat1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6170,x:31332,y:32998,ptovrint:False,ptlb:Splat5,ptin:_Splat5,varname:_Splat2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2109,x:31332,y:33204,ptovrint:False,ptlb:Splat6,ptin:_Splat6,varname:_Splat3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7963,x:31332,y:33398,ptovrint:False,ptlb:Splat7,ptin:_Splat7,varname:_Splat4,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3164,x:31518,y:32617,ptovrint:False,ptlb:Control2,ptin:_Control2,varname:_Control_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:7684,x:31750,y:32856,varname:node_7684,prsc:2|A-3164-R,B-3292-RGB;n:type:ShaderForge.SFN_Multiply,id:5900,x:31750,y:33016,varname:node_5900,prsc:2|A-3164-G,B-6170-RGB;n:type:ShaderForge.SFN_Multiply,id:803,x:31750,y:33167,varname:node_803,prsc:2|A-3164-B,B-2109-RGB;n:type:ShaderForge.SFN_Multiply,id:4497,x:31750,y:33308,varname:node_4497,prsc:2|A-3164-A,B-7963-RGB;n:type:ShaderForge.SFN_Add,id:382,x:31989,y:33076,varname:node_382,prsc:2|A-7684-OUT,B-5900-OUT,C-803-OUT,D-4497-OUT;n:type:ShaderForge.SFN_Add,id:4994,x:32953,y:33080,varname:node_4994,prsc:2|A-382-OUT,B-6931-OUT;n:type:ShaderForge.SFN_Tex2d,id:2932,x:33116,y:32680,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_2932,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;proporder:824-8014-9937-1400-6355-1645-3292-6170-2109-7963-3164-2932;pass:END;sub:END;*/

Shader "DAFUHAO_Editor/T4M_12" {
    Properties {
		_Diffuse("Diffuse", COLOR) = (1, 1, 1, 1)
        _Splat0 ("Splat0", 2D) = "white" {}
        _Splat1 ("Splat1", 2D) = "white" {}
        _Splat2 ("Splat2", 2D) = "white" {}
        _Splat3 ("Splat3", 2D) = "white" {}
        _Control ("Control", 2D) = "white" {}
        _color ("color", Color) = (0.5,0.5,0.5,1)
        _Splat4 ("Splat4", 2D) = "white" {}
        _Splat5 ("Splat5", 2D) = "white" {}
        _Splat6 ("Splat6", 2D) = "white" {}
        _Splat7 ("Splat7", 2D) = "white" {}
		_Lightmap("Lightmap", 2D) = "white" {}
        _Control2 ("Control2", 2D) = "white" {}
        _Normal ("Normal", 2D) = "white" {}
		_Splat_Normals("Normal array", 2DArray) = "white" {}
		_Normal_Idensity("_Normal_Idensity", Range(1,10)) = 1
		_Detail_Normal_Idensity("_Detail_Normal_Idensity", Range(1,10)) = 1
		_Lightmap_Scale("_Lightmap_Scale", Range(1,3)) = 1
		_NoLODDistance("No LOD Distance", float) = 200

		[Toggle(UseUnityShadow)] UseUnityShadow("使用unity阴影", Int) = 1

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
            #include "UnityCG.cginc"
            #pragma multi_compile __ AlphaBlendOn 
            #pragma shader_feature UseUnityShadow
            #pragma multi_compile_instancing
			#pragma multi_compile_fog
            
			#include "AutoLight.cginc"
			#include "Lighting.cginc"
            
            #pragma multi_compile_fwdbase
           // #pragma only_renderers gles3 
            #pragma target 3.0
            uniform sampler2D _Splat0; uniform float4 _Splat0_ST;
            uniform sampler2D _Splat1; uniform float4 _Splat1_ST;
            uniform sampler2D _Splat2; uniform float4 _Splat2_ST;
            uniform sampler2D _Splat3; uniform float4 _Splat3_ST;
            uniform sampler2D _Control; uniform float4 _Control_ST;
            uniform fixed4 _color;
            uniform sampler2D _Splat4; uniform float4 _Splat4_ST;
            uniform sampler2D _Splat5; uniform float4 _Splat5_ST;
            uniform sampler2D _Splat6; uniform float4 _Splat6_ST;
            uniform sampler2D _Splat7; uniform float4 _Splat7_ST;
            uniform sampler2D _Control2; uniform float4 _Control2_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;

			uniform sampler2D _Lightmap;
			uniform fixed _Lightmap_Scale;

			fixed4 _Diffuse;

			fixed _NoLODDistance;
			fixed _Normal_Idensity;
			fixed _Detail_Normal_Idensity;
			UNITY_DECLARE_TEX2DARRAY(_Splat_Normals);

            struct VertexInput {
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed4 tangent : TANGENT;
				fixed2 texcoord0 : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            
            };
            struct VertexOutput {
				fixed4 pos : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
				fixed4 posWorld : TEXCOORD1;
				fixed4 viewPos : TEXCOORD2;
				fixed4 worldPos_2_Camera : TEXCOORD3;
				fixed3 normalDir : TEXCOORD4;
				fixed3 tangentDir : TEXCOORD5;
				fixed3 bitangentDir : TEXCOORD6;
				fixed3 toCameraDistance:TEXCOORD7;
				UNITY_FOG_COORDS(8)
				#ifdef UseUnityShadow
				LIGHTING_COORDS(9, 10)
				#endif

				UNITY_VERTEX_INPUT_INSTANCE_ID
            
				
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_INITIALIZE_OUTPUT(VertexOutput, o);
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
               
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
				o.toCameraDistance = _WorldSpaceCameraPos - o.posWorld;

				 #ifdef UseUnityShadow
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                #endif
				UNITY_TRANSFER_FOG(o, o.pos);
                return o;
			}

#define AddWeightColor(index,c)\
				fixed3 _sub_normal_var##index = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY_LOD(_Splat_Normals, float3(TRANSFORM_TEX(i.uv0, _Splat##index), index), length(i.toCameraDistance) / _NoLODDistance));\
				normalDirection += float3(_sub_normal_var##index.rg * _Detail_Normal_Idensity,1) * c;\
				_Total_Color += tex2D(_Splat##index,TRANSFORM_TEX(i.uv0, _Splat##index)) * c;

            float4 frag(VertexOutput i) : COLOR {
            UNITY_SETUP_INSTANCE_ID(i);
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
				fixed3 _Normal_var = UnpackNormal(tex2D(_Normal, TRANSFORM_TEX(i.uv0, _Normal)));
				fixed3 normalDirection = _Normal_var;// // Perturbed normals
				normalDirection.xy *= _Normal_Idensity;
				normalDirection.z = 1;

				fixed4 _Total_Color = fixed4(0, 0, 0, 0);
////// Lighting:
				fixed4 _Control_var  = tex2D(_Control, TRANSFORM_TEX(i.uv0, _Control));
				fixed4 _Control2_var = tex2D(_Control2,TRANSFORM_TEX(i.uv0, _Control2));

				AddWeightColor(4, _Control2_var.r)
					AddWeightColor(5, _Control2_var.g)
					AddWeightColor(6, _Control2_var.b)
					AddWeightColor(7, _Control2_var.a)

					AddWeightColor(0, _Control_var.r)
					AddWeightColor(1, _Control_var.g)
					AddWeightColor(2, _Control_var.b)
					AddWeightColor(3, _Control_var.a)

					//_Total_Color *= tex2D(_Lightmap, TRANSFORM_TEX(i.uv0, _Control)) * _Lightmap_Scale;
				normalDirection = normalize(mul(normalDirection, tangentTransform));
				//normalDirection = normalize(normalDirection);

				float4 finalColor = float4(_Total_Color * _color.rgb,1);

				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 worldNormal = normalize(normalDirection);
                //内置函数写法
                //float3 worldLightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                finalColor.rgb *= (ambient + _LightColor0.rgb * _Diffuse.rgb * max(0,dot(worldNormal, worldLightDir)) ) ;


                #ifdef UseUnityShadow
               finalColor.rgb *= LIGHT_ATTENUATION(i);
				#endif
                //#if S_BOOL

                //FINAL_SHADOW_COLOR_SINGLE(_Total_Color * GetEnvirmentColor(normalDirection)*_color.rgb, i, normalDirection);
				UNITY_APPLY_FOG(i.fogCoord, finalColor.rgb); 

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
