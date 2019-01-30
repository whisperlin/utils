
Shader "Unlit/Fresnel" {
    Properties {
        _Color ("Color", Color) = (0.524824,0.4636678,0.9852941,1)
        _Ior ("Ior", Range(1, 4)) =1.5
        [KeywordEnum(Schlick,SchlickIOR, SphericalGaussian,Unity)]_FresnelModel("Normal Distribution Model;", Float) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 100
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
            #include "math.cginc"

            #include "pbr.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            #pragma multi_compile _FRESNELMODEL_SCHLICK _FRESNELMODEL_SCHLICKIOR _FRESNELMODEL_SPHERICALGAUSSIAN _FRESNELMODEL_UNITY
            uniform float4 _LightColor0;
            uniform float4 _Color;
            uniform float _Ior;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
               // LIGHTING_COORDS(2,3)
                //UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                //UNITY_TRANSFER_FOG(o,o.pos);
                //TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection); 
                float NdotH =  max(0.0,dot( normalDirection, halfDirection));

                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                //float attenuation = LIGHT_ATTENUATION(i);
               // float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                 float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 directDiffuse = max( 0.0, NdotL) ;
          
                float NdV = max(0,dot(normalDirection, viewDirection));

                float3 FresnelFunction = 1;

              
                #ifdef _FRESNELMODEL_SCHLICK
					FresnelFunction =  SchlickFresnelFunction(_Color, LdotH) ;
				#elif _FRESNELMODEL_SCHLICKIOR
					FresnelFunction =  _Color*SchlickIORFresnelFunction(_Ior, LdotH);
				#elif _FRESNELMODEL_SPHERICALGAUSSIAN
					FresnelFunction = _Color*SphericalGaussianFresnelFunction(LdotH, _Color);
			 	#else
					FresnelFunction = UnityFresnelTerm(_Color, LdotH);
			 	#endif
			 	 
            
 
                fixed4 finalRGBA = fixed4(FresnelFunction,1);
                //UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
