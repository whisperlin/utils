Shader "lin/SSS"
{
    Properties
    {
    	_MainTex ("Texture", 2D) = "white" {}
        _Diffuse ("Diffuse Color", Color) = (1,1,1,1)
        _Specular ("Specular Color", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(8, 256)) = 8
        _SpecularStrength("Specular",Range(0,1)) = 0.5
        _BumpMap("Normal Map", 2D) = "bump" {}

        _SSSLUTTex ("_SSSLUTTex", 2D) =  "white" {}
        _CurveFactor("曲率图大小", Range(0.001, 1)) = 0.01
        [Toggle] _ENABLE_SKIN ("Normal Distribution Enabled?", Float) = 0
 
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
            #pragma multi_compile  _ENABLE_SKIN_OFF _ENABLE_SKIN_ON  
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            //#include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
                float3 ambient : TEXCOORD4;
                float2 uv : TEXCOORD5;
                float4 diff : COLOR0; // diffuse lighting color
            };

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;
            float _SpecularStrength;

            sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4  _BumpMap_ST;
			sampler2D _SSSLUTTex;
			float _CurveFactor;
                        
            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                //o.worldNormal = worldNormal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);


                half3 wNormal = UnityObjectToWorldNormal(v.normal);
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

                o.ambient = ShadeSH9(half4(wNormal,1))  ;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
            	half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
                // transform normal from tangent to world space
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);


                half3 bnormal = UnpackNormal(tex2Dlod(_BumpMap, float4(i.uv,0,4 )));

                half3 blurNormalDirection;
                blurNormalDirection.x = dot(i.tspace0, bnormal);
                blurNormalDirection.y = dot(i.tspace1, bnormal);
                blurNormalDirection.z = dot(i.tspace2, bnormal);

            	fixed4 col = tex2D(_MainTex,  TRANSFORM_TEX(i.uv, _MainTex)  );
            	fixed3 ambient = i.ambient;
               
                fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
       
                float3 viewDir = UnityWorldSpaceViewDir(i.worldPos);
                viewDir = normalize(viewDir);
                float3 halfDir = normalize(lightDirection + viewDir);

                float nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                float d = max(0, dot(halfDir, worldNormal));

                fixed curve = fwidth(blurNormalDirection) / (fwidth(i.worldPos) * _CurveFactor);
                fixed NDotL = dot(blurNormalDirection, lightDirection);
                fixed4 sssColor = tex2D(_SSSLUTTex, float2(NDotL * 0.5 + 0.5, curve)) * _LightColor0;

                float3 diff =   col*_Diffuse.rgb   ;

				#ifdef _ENABLE_SKIN_ON
				  diff = diff * sssColor;
				  //return float4(0,1,0,1);
				  #else
				  //return float4(1,0,0,1);
				#endif
             
                float3 atten =  _LightColor0.rgb *nl;
                float3 c = (ambient  + atten)* diff  ;
      			//c = sssColor;
                return fixed4(c, 1);
            }
            ENDCG
        }
    }
}