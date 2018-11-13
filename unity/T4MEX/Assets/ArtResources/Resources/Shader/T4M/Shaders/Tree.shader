Shader "Unlit/Tree" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5

    _Diffuse ("Diffuse Color", Color) = (1,1,1,1)
    _Specular ("Specular Color", Color) = (1,1,1,1)
    _Gloss ("Gloss", Range(8, 256)) = 8
    _SpecularStrength("Specular",Range(0,1)) = 0.5
    _Emssion ("Emssion", Range(0, 1)) = 0.5
}
SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 100

    Lighting Off

    Pass {
    	
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;

                //UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;

                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 ambient : TEXCOORD3;
                
                
                UNITY_FOG_COORDS(4)
                //UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;
            float _SpecularStrength;
            float _Emssion;
            v2f vert (appdata_t v)
            {
                v2f o;
                //UNITY_SETUP_INSTANCE_ID(v);
               // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);


                float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                o.worldNormal = worldNormal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.ambient = ShadeSH9(half4(worldNormal,1))  ;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {


            	fixed4 _mainColor = tex2D(_MainTex, i.texcoord);
                clip(_mainColor.a - _Cutoff);

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
                float3 spec = _LightColor0.rgb * _Specular.rgb * pow(d, _Gloss) * _SpecularStrength;

                 
                float3 diff = _LightColor0.rgb *_mainColor.rgb* _Diffuse.rgb *  (_Emssion +  nl*(1-_Emssion)) ;
                float3 c = ambient + spec + diff  ;
               // float3 c = ambient*_Diffuse + spec + diff  ;
          
                float4 finalColor =  fixed4(c, 1);
            	UNITY_APPLY_FOG(i.fogCoord, finalColor);
                
 
                return finalColor;
            }
        ENDCG
    }
}
	FallBack "Diffuse"
}
