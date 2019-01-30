Shader "TA/PBRFont Blended" {
    Properties {
    	_MainTex ("Particle Texture", 2D) = "white" { }
    	_Noise ("消融图", 2D) = "white" { }
    	_NoiseSpeed ("NoiseSpeed", Range(0,0.5 )) = 0.1
    	 _EffectPower ("EffectPower", Range(0,1 )) = 0
    	 _beginAlpha("初始透明度",Range(0,1)) = 0.7
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
       
        _MoveDirect ("MoveDirect", Range(-0.1,0 )) = 0
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _Sky("环境球", Cube) = "_Skybox" {}
        _SkyPower("环境球强度",Range(0.5, 2)) = 1
        _RotationSpeed("天空盒旋转速度", Range(-30, 30)) = 30
        _alpha("透明度",Range(0,2)) = 1
        _LightDir0("光方向",Vector) = (0,0,1,1)

    }
    SubShader {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" }
        Pass {
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
 
           
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
            //#pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;

         
            uniform sampler2D _BumpMap; 
            uniform float4 _BumpMap_ST;
            uniform sampler2D _MainTex; 
            uniform float4 _MainTex_ST;
            uniform sampler2D _Noise; 
            uniform float4 _Noise_ST;

            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _RotationSpeed;
            uniform float _SkyPower;
            uniform float4 _LightDir0;

            uniform samplerCUBE _Sky;
            uniform float _alpha;
            uniform float _beginAlpha;
            uniform float _EffectPower;
            uniform float _MoveDirect;
            uniform float _NoiseSpeed;
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
            
 
        
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
 
       
                o.normalDir = UnityObjectToWorldNormal(v.normal);
               float4x4 mat = unity_ObjectToWorld;

               //只要旋转缩放.
               mat[3][0] = mat[3][1]  = mat[3][2] = 0; 
               mat[0][3] = mat[1][3]  = mat[2][3] = 0; 
 
 
                o.posWorld = mul(mat, v.vertex);
 
                o.pos = UnityObjectToClipPos( v.vertex );
 
                return o;
            }

            float3 RotateAroundYInDegrees (float3 vertex, float degrees)
	        {
	            float alpha = degrees * UNITY_PI / 180.0;
	            float sina, cosa;
	            sincos(alpha, sina, cosa);
	            float2x2 m = float2x2(cosa, -sina, sina, cosa);
	            return float3(mul(m, vertex.xz), vertex.y).xzy;
	        }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);

                _WorldSpaceCameraPos = float3(0,0,-1);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
 
               
             	float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0  + float2(_Time.y*_NoiseSpeed,0) , _Noise));
                //float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0 + float2(_Time.y*_NoiseSpeed,0), _Noise));
                _EffectPower = _EffectPower*2-1;
 				float _a = clamp(_Noise_var.r +_EffectPower ,0,1);
 				float _offset = _a-1;
 				_offset = 0;
 				float2 uv0 = i.uv0 + float2(_MoveDirect*_Noise_var.r * (1-_EffectPower),0);
 				 
  

                 float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX( uv0 , _MainTex));
                _MainTex_var.a =   lerp(_MainTex_var.a *_beginAlpha,_MainTex_var.a,_a );


                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(normalLocal);
         
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                //float3 lightDirection = normalize(_LightDir0.xyz);
                float3 lightDirection = normalize(_LightDir0.xyz);

                float3 lightColor = float4(1,1,1,1);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
 
                float3 attenColor =   lightColor.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                 
           
 
 
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
 
                float3 diffuseColor = _Color.rgb *_MainTex_var.rgb ;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );

                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
 
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
       
                half grazingTerm = saturate( gloss + specularMonochrome );

                viewReflectDirection = RotateAroundYInDegrees(viewReflectDirection,_Time.y*_RotationSpeed);
                float3 indirectSpecular = texCUBE(_Sky, viewReflectDirection);

                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                indirectSpecular *= _SkyPower;
 
                float3 specular =   _Color.rgb* indirectSpecular ;
 
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
 
 				
 
  				//_EffectPower
                
                float3 diffuse = NdotL  * diffuseColor;
 
  				float3 finalColor =   diffuse + specular;

                fixed4 finalRGBA = fixed4(finalColor, min(1, _MainTex_var.a*_alpha ));
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
