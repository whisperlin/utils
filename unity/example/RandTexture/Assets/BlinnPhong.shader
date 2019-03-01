﻿Shader "lin/Specular Blinn-Phong P"
{
	Properties
	{
		_Diffuse("Diffuse Color", Color) = (1,1,1,1)
		_Specular("Specular Color", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8, 256)) = 8
		_SpecularStrength("Specular",Range(0,1)) = 0.5
		_HGTex("HG",2D) = "writhe"{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
 

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 ambient : TEXCOORD2;
                float4 diff : COLOR0; // diffuse lighting color
            };

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;
            float _SpecularStrength;
			sampler2D _HGTex;
		 
                        
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                o.worldNormal = worldNormal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
 
                o.ambient = ShadeSH9(half4(worldNormal,1)) *_Diffuse ;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
	 
            	fixed3 ambient = i.ambient;
                // specular
                float3 worldNormal = normalize(i.worldNormal);
                //float3 lightDir = UnityWorldSpaceLightDir(i.worldPos);
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
       
                float3 viewDir = UnityWorldSpaceViewDir(i.worldPos);
                viewDir = normalize(viewDir);
                float3 halfDir = normalize(lightDir + viewDir);

                float nl = max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));
                float d = max(0, dot(halfDir, worldNormal));

				float testValue = tex2D(_HGTex, float2(d, _Gloss/256)).r;
                float3 spec = _LightColor0.rgb * _Specular.rgb * testValue * _SpecularStrength;

                //float3 spec = _LightColor0.rgb * _Specular.rgb * pow(i.halfD, _Gloss) * _SpecularStrength;
                //float3 spec = _LightColor0.rgb *FresnelTerm (_Specular.rgb ,nl ) *_SpecularStrength;


                //fixed3 lambert = 0.5 *  nl + 0.5;
           
                // diffuse
                //float3 diff = _LightColor0.rgb * _Diffuse.rgb * lambert ;
                float3 diff = _LightColor0.rgb * _Diffuse.rgb * nl ;
			 ;
                float3 c = ambient + spec + diff  ;
                float ndh = dot(halfDir, worldNormal);
           		 
                return fixed4(c, 1);
            }
            ENDCG
        }
    }
}