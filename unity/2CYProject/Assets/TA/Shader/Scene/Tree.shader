 

Shader "TA/Scene/Tree"
{
	Properties
	{
		_MainTex ("主贴图", 2D) = "white" {}
		_Color("颜色",Color) = (1,1,1,1)
		_AlphaCut("半透明剔除",Range(0,1))=0.2
		_Wind("风向",Vector) = (1,0.5,0,0)
		_Speed("速度",Range(0,5)) = 2
		_Ctrl("空间各向差异",Range(0,3.14)) = 0

		[Toggle(WIND_ENABLE)] _WIND_ENABLE("开启摇动", Float) = 1
	}

	SubShader
	{
		Cull  Off
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma shader_feature WIND_ENABLE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
				float2 uv2 : TEXCOORD1;
#else
				float3 normal : NORMAL;
#endif
				float4 color: COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
				float2 uv2 : TEXCOORD1;
#else
				float3 normalWorld : TEXCOORD1;
#endif

				float4 color: TEXCOORD2;
				UNITY_FOG_COORDS(3)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			half _AlphaCut;
			half4 _Wind;
			half _Speed;
			half _Ctrl;
			half4 _Color;
			v2f vert (appdata v)
			{

				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

				//_Wind
				o.vertex = mul(unity_ObjectToWorld, v.vertex);
#if WIND_ENABLE
				float s = sin(_Time.y*_Speed + (o.vertex.x+ o.vertex.z) *_Ctrl);
				//float c = 1 - abs(s);
				o.vertex.xyz = o.vertex.xyz + float3(_Wind.x,0, _Wind.y)  * v.color.g * s ;
#endif
				o.vertex = mul(UNITY_MATRIX_VP, o.vertex);
				o.uv = v.uv;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
				o.uv2 = v.uv2 * unity_LightmapST.xy + unity_LightmapST.zw;
#else
				o.normalWorld = UnityObjectToWorldNormal(v.normal);
#endif
				o.color = v.color;

				
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				c.rgb *= _Color.rgb;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
				fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2));
				c.rgb *= lm;
#else
				half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				//鍙岄潰鍙戝厜.
				half nl = saturate(dot(i.normalWorld, lightDir)) + saturate(dot(i.normalWorld, -lightDir));
				c.rgb = UNITY_LIGHTMODEL_AMBIENT * c.rgb + _LightColor0 * nl * c.rgb;
#endif
				//return i.color;
				clip(c.a - _AlphaCut);

				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}
	}
}