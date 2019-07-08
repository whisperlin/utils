Shader "YuLongZhi/CharacterLowDark"
{
	Properties
	{
		_MainTex ("Main", 2D) = "white" {}

		[HideInInspector] _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		[HideInInspector] _RimPow ("Rim Pow", Range(0, 10)) = 1
		[HideInInspector] _HighLight ("High Light", Range(0, 1)) = 0
		[HideInInspector] _DissolveTex ("Dissolve Tex", 2D) = "white" {}
		[HideInInspector] _Dissolve ("Dissolve", Range(0, 1)) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "AlphaTest+2"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile __ RIM_ON
			#pragma multi_compile __ DISSOLVE_ON
			#include "UnityCG.cginc"

			struct appdata
			{
				fixed4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
				fixed3 normal : NORMAL;
			};

			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				half3 worldNormal : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
			};

			sampler2D _MainTex;
			fixed4 _MainTex_ST;

#ifdef RIM_ON
			float3 _RimColor;
			float _RimPow;
#endif

			float _HighLight;

#ifdef DISSOLVE_ON
			sampler2D _DissolveTex;
			float _Dissolve;
#endif

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);

				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
#ifdef DISSOLVE_ON
				fixed4 dissolve = tex2D(_DissolveTex, i.uv);
				float dis = _Dissolve * 1.05;
				float dis_width = 0.03;
				float dis_param_a = 0.0;
                float dis_param_b = 1.0;
                float dis_step_a = step((dissolve.r + dis_width), dis);
                float dis_step_b = step(dis, (dissolve.r + dis_width));
                float dis_lerp_a = lerp((dis_step_a * dis_param_a) + (dis_step_b * dis_param_b), dis_param_a, dis_step_a * dis_step_b);
                clip(dis_lerp_a - 0.5);
#endif
				fixed4 col = tex2D(_MainTex, i.uv);

#ifdef RIM_ON
				fixed3 rim = pow(1.0 - max(0, dot(worldNormal, worldViewDir)), _RimPow) * _RimColor;
				col.rgb += rim;
#endif

				col.rgb += _HighLight;

#ifdef DISSOLVE_ON
				float dis_border = 3.32;
				float3 dis_c = float3(1, 0.424, 0.047);
				float dis_step_c = step(dissolve.r, dis);
                float dis_step_d = step(dis, dissolve.r);
                col.rgb = (col.rgb + ((dis_c * (dis_lerp_a - lerp((dis_step_c * dis_param_a) + (dis_step_d * dis_param_b), dis_param_a, dis_step_c * dis_step_d))) * dis_border));
#endif

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}