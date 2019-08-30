// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ProjectQ/GFX/Complex/Additive_Mask" 
{
	Properties 
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_Mask ("Mask ( R Channel )", 2D) = "white" {}
	}

	Category 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off
	
		SubShader 
		{
			Pass 
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				#include "UnityCG.cginc"
				#include "../../CGInclude/CGInclude.cginc"

				sampler2D _MainTex;
				sampler2D _Mask;
				fixed4 _TintColor;
			
				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 texcoordMask : TEXCOORD1;
					MY_FOG_LIGHTMAP_COORDS(2)
					MY_FOGWAR_COORDS(3)
				};
			
				float4 _MainTex_ST;
				float4 _Mask_ST;

				float4 _Center;
				float4 _Scale;
				float4 _Normal;

				uniform float4x4 _Camera2World;

				v2f vert (appdata_t v)
				{
 
					v2f o = (v2f)0;
					o.vertex = UnityObjectToClipPos(v.vertex);

					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.texcoordMask = TRANSFORM_TEX(v.texcoord,_Mask);

					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					//MY_TRANSFER_FOG(o, worldPos, o.pos.z);
					return o;
				}
			
				fixed4 frag (v2f i) : SV_Target
				{
				    fixed4 c = tex2D(_MainTex, i.texcoord);
					c.a *= tex2D(_Mask, i.texcoordMask).r;
					fixed4 finalRGBA = 2.0f * i.color * _TintColor * c;

					//MY_APPLY_FOG_COLOR(i, finalRGBA, fixed4(0, 0, 0, 0), fixed4(0, 0, 0, 0));

					return finalRGBA;
				}
				ENDCG 
			}
		}
	}
//	 FallBack "Diffuse"
}
