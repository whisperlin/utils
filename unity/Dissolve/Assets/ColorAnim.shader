 
Shader "Unlit/ColorAnim" {
    Properties {
        _Col0 ("Col0", Color) = (1,0.0,0.0,1)
        _Col1 ("Col1", Color) = (0.0,1,0.0,1)
        _Col2 ("Col2", Color) = (0.0,0.0,1.0,1)
        _Col3 ("Col3", Color) = (1,1,0,1)
        _speed ("_speed", Range(0, 1)) = 0.5384616
        _uvScale ("uvScale", Float) = 1
       _Emission ("Emission", Range(0,1)) = 0.5

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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
      
  
            uniform half4 _LightColor0;
            uniform half4 _Col0;
            uniform half4 _Col1;
            uniform half4 _Col2;
            uniform half4 _Col3;
            uniform half _speed;
            uniform half _uvScale;
            uniform half _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
 
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float2 uv : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
 
                o.pos = UnityObjectToClipPos( v.vertex );
                o.uv = v.texcoord0;
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float _proc = _speed*_Time.y - i.uv.y*_uvScale;\
                _proc = frac(_proc);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) ;//* attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light

                float3 diffuseColor = 0;


             

                half b25 = step(0.25,_proc);
                half b05 = step(0.5,_proc);
                half b75 = step(0.75,_proc);

                diffuseColor += lerp(_Col0.rgb,_Col1.rgb,_proc/0.25)*(1.0-b25);
                diffuseColor += lerp(_Col1.rgb,_Col2.rgb,(_proc-0.25)/0.25)*b25*(1.0-b05);
                diffuseColor += lerp(_Col2.rgb,_Col3.rgb,(_proc-0.5)/0.25)*b05*(1.0-b75);
                diffuseColor += lerp(_Col3.rgb,_Col0.rgb,(_proc-0.75)/0.25)*b75;
     
 
                float3 diffuse = (directDiffuse + indirectDiffuse+_Emission) * diffuseColor;
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
 
}
