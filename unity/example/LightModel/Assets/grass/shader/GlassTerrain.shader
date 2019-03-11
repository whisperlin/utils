// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:33410,y:32550,varname:node_4013,prsc:2|diff-741-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32165,y:32696,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:6006,x:32255,y:32519,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_6006,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,taginstsco:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:741,x:32424,y:32694,varname:node_741,prsc:2|A-6006-RGB,B-1304-RGB;proporder:1304-6006;pass:END;sub:END;*/

Shader "Shader Forge/GlassTerrain" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
		_Speed("Speed",Range(0,10)) = 1
		_DirX("_DirX",Range(0,10)) = 1
		_DirY("_DirY",Range(0,10)) = 0
		_Ambient("»·¾³É«",Color) = (0.5,0.5,0.5,1)
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
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Color;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;

			uniform float _Speed;
			uniform float _DirX;
			uniform float _DirY;
			uniform float4 _Ambient;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
				float4 color : COLOR;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
				v.vertex.xyz += float3(_DirX,0, _DirY)*sin(_Time.y)*v.color.r;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
			float4 DirectionalLightColor;
			float4 DirectionalLightDir;
			float DirectionalLightIntensity;


			float4 PointLightColor;
			float4 PointLightPosition;
			float PointLightRange;
			float PointLightIntensity;

			float4 SpotlightColor;
			float SpotLightIntensity;
			float SpotlightSpotAngle0;

			float SpotlightSpotAngle1;
			float4 SpotDirection;
			float4 SpotLightPosition;
			float SpotLightRange;



			inline float3 PointLight(float3 normaldir, float3 p, float3 posWorld, float LightRange)
			{
				float3 lightDirection;

				float3 toLight = p - posWorld.xyz;
				float distance = length(toLight);

				float atten = 1.0 - distance*distance / (LightRange*LightRange);
				atten = max(0, atten);
				lightDirection = normalize(toLight);
				float3 finalPointlight = atten * saturate(dot(normaldir, lightDirection));
				return finalPointlight;
			}

			inline float3 DirectionalLight(float3 normaldir, fixed3 lightDir)
			{
				float nl = max(0, dot(normaldir, -lightDir));
				return    nl;
			}

			float smooth_step(float a, float b, float x)
			{
				float t = saturate((x - a) / (b - a));
				return t * t*(3.0 - (2.0*t));
			}
			inline float3 Spotlight(float3 normaldir, float3 p, float3 posWorld, float LightRange, float3 SpotDirection, float spotlightSpotAngle0, float spotlightSpotAngle1)
			{

				float3 lightDirection;

				float3 toLight = p - posWorld.xyz;
				float distance = length(toLight);

				float atten = 1.0 - distance*distance / (LightRange*LightRange);
				atten = max(0, atten);
				lightDirection = normalize(toLight);

				float rho = max(0, dot(lightDirection, SpotDirection));

				float spotAtt0 = smooth_step(spotlightSpotAngle0, SpotlightSpotAngle1, rho);

				atten *= spotAtt0;

				float diff = max(0, dot(normaldir, lightDirection));
				return (diff * atten);


			}
			
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
				float3 lightDirection = normalize(-DirectionalLightDir);
 
 
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * DirectionalLightColor.xyz*DirectionalLightIntensity;
				float3 pointDiffuse = PointLight(normalDirection, PointLightPosition, i.posWorld, PointLightRange)*PointLightColor*PointLightIntensity;
				float3 SportLightDiff = Spotlight(normalDirection, SpotLightPosition, i.posWorld, SpotLightRange, -SpotDirection, SpotlightSpotAngle0, SpotlightSpotAngle1)*SpotlightColor*SpotLightIntensity ;


                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += _Ambient.rgb; // Ambient Light
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 diffuseColor = (_Diffuse_var.rgb*_Color.rgb);
                float3 diffuse = (directDiffuse + pointDiffuse + SportLightDiff + indirectDiffuse) * diffuseColor;
				
/// Final Color:
                float3 finalColor = diffuse;
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
