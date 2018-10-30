Shader "lin/Specular Blinn-Phong V"
{
    Properties
    {
        _Diffuse ("Diffuse Color", Color) = (1,1,1,1)
        _Specular ("Specular Color", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(8, 256)) = 8
        _SpecularStrength("Specular",Range(0,1)) = 0.5
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
            //#include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 ambient : TEXCOORD0;

                float halfD : TEXCOORD1;
                float4 diff : COLOR0; // diffuse lighting color
            };

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;
            float _SpecularStrength;
                        
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);

                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = UnityWorldSpaceViewDir(worldPos);
                viewDir = normalize(viewDir);
                float3 halfDir = normalize(lightDir + viewDir);
                o.halfD = max(0, dot(halfDir, worldNormal));

                float nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _Diffuse * _LightColor0;


                o.ambient =  ShadeSH9(half4(worldNormal,1)) * _Diffuse;
                return o;
            }
            inline half Pow5_2(half x)
			{
				return x*x * x*x * x;
			}


			//捏菲尔算法
			inline half3 FresnelTerm2(half3 specularColor, half LdotH)
			{
				half t = Pow5(1 - LdotH);	// ala Schlick interpoliation
				return specularColor + (1 - specularColor) * t;
			}
            fixed4 frag (v2f i) : SV_Target
            {
            	fixed3 ambient = i.ambient;
              

                float3 spec = _LightColor0.rgb * _Specular.rgb * pow(i.halfD, _Gloss) * _SpecularStrength;
                //float3 spec = _LightColor0.rgb *FresnelTerm (_Specular.rgb ,i.halfD ) *_SpecularStrength;
                //FresnelTerm
       
                float3 diff = i.diff;

                float3 c = ambient + spec + diff  ;

           
                return fixed4(c, 1);
            }
            ENDCG
        }
    }
}