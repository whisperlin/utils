// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/DApprox"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_Roughness("Roughness",Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;

				float3 worldPos : TEXCOORD1;
                half3 worldNormal : TEXCOORD2;
	 			
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Roughness;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
 				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			half D_Approx( half Roughness, half RoL )
			{
				half a = Roughness * Roughness;
				half a2 = a * a;
				float rcp_a2 = rcp(a2);
				half c = 0.72134752 * rcp_a2 + 0.39674113;
				return rcp_a2 * exp2( c * RoL - c );
			}
			fixed4 frag (v2f i) : SV_Target
			{
                half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                half3 worldRefl = reflect(-worldViewDir, i.worldNormal);

                half RoL = max(0, dot(worldRefl, _WorldSpaceLightPos0.xyz));
                float d = D_Approx(_Roughness,RoL);
				 
		 		return float4(d,d,d,1);
			 
			}
			ENDCG
		}
	}
}
