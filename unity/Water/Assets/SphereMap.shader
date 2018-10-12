// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Texgen_SphereMap_FragMine" {
   Properties {
           _Reflectivity ("Reflectivity", Range (0,1)) = 0.5
        _MainTex("Base", 2D) = "white"
        _Environment ("Environment", 2D) = "white"
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            sampler2D _Environment;
            float4 _MainTex_ST;
            float _Reflectivity;
            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                float2  uv2:TEXCOORD1;
            } ;
            
            //I:入射向量 N:法线向量  R:反射向量
            float3 reflect(float3 I,float3 N)
            {
                return ( I - 2.0*N *dot(N,I) ) ;
            }

            float2 R_To_UV2(float3 r)
            {
            	return normalize(r).xz;
            }
            //
            float2 R_To_UV(float3 r)
            {
                float interim = 2.0 * sqrt(r.x * r.x + r.y * r.y + (r.z + 1.0) * (r.z + 1.0)); 
               	return float2(r.x/interim+0.5,r.y/interim+0.5);
            }
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

                float3 posEyeSpace = mul(UNITY_MATRIX_MV,v.vertex).xyz;
                float3 dir = posEyeSpace - float3(0,0,0);
                float3 N = mul((float3x3)UNITY_MATRIX_MV,v.normal);
                N = normalize(N);
                float3 R = reflect(dir,N);
                o.uv2 = R_To_UV(R);
                return o;
            }
            float4 frag (v2f i) : COLOR
            {
                float4 reflectiveColor = tex2D(_Environment,i.uv2);
                float4 decalColor = tex2D(_MainTex,i.uv);
                float4 outp = lerp(decalColor,reflectiveColor,_Reflectivity);
                return outp;
            }
            ENDCG
        }
    }
}