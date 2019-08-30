Shader "ProjectQ/GFX/Add Self" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_AlphaTex ("Alpha Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	  Blend One OneMinusSrcColor
	//ColorMask RGB
	Cull Off Lighting Off ZWrite Off
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			//#pragma multi_compile_particles
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "../CGInclude/CGInclude.cginc"

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			fixed4 _TintColor;
			fixed4 _ShadowColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				MY_FOG_LIGHTMAP_COORDS(1)
				MY_FOGWAR_COORDS(2)
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o = (v2f)0;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				MY_TRANSFER_FOG(o, worldPos, o.vertex.z);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 tex = tex2D(_MainTex, i.texcoord);
				fixed4 alpha = tex2D(_AlphaTex, i.texcoord);
				fixed4 col = 2.0f * _ShadowColor * tex*_ShadowColor.a + tex.r*tex.b*tex.b*_TintColor*_TintColor.a;
				col *= i.color*i.color.a*alpha.r;
				MY_APPLY_FOG_COLOR(i, col, fixed4(0, 0, 0, 0), fixed4(0,0,0,0)); // fog towards black due to our blend mode
				return col;
			}
			ENDCG 
		}
	}	
}
}
