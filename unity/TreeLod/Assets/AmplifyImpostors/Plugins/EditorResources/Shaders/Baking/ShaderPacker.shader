// Amplify Impostors
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/ShaderPacker"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_A("A", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		ZWrite On
		ZTest LEqual
		ColorMask RGBA
		Blend Off
		Cull Off
		Offset 0,0


		Pass // Pack Depth 0
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _A;

			fixed4 frag (v2f_img i ) : SV_Target
			{
				float depth = tex2D( _A, i.uv ).r;
				#if UNITY_REVERSED_Z != 1
					depth = 1-depth;
				#endif
				fixed4 finalColor = (float4(tex2D( _MainTex, i.uv ).rgb , depth));
				return finalColor;
			}
			ENDCG
		}

		Pass // Fix Emission 1
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;

			fixed4 frag (v2f_img i ) : SV_Target
			{
				fixed4 finalColor = tex2D( _MainTex, i.uv );
				//#if !defined(UNITY_HDR_ON)
					finalColor.rgb = -log2(finalColor.rgb);
				//#endif
				return finalColor;
			}
			ENDCG
		}

		Pass // Render Only Alpha (for the inspector) 2
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;

			fixed4 frag (v2f_img i ) : SV_Target
			{
				fixed4 finalColor = tex2D( _MainTex, i.uv );
				return float4(0,0,0,finalColor.a);
			}
			ENDCG
		}

		Pass // 3
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			uniform float4x4 unity_GUIClipTextureMatrix;
			sampler2D _GUIClipTexture;

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 clipUV : TEXCOORD1;
			};

			v2f vert( appdata_t v )
			{
				v2f o;
				o.vertex = UnityObjectToClipPos( v.vertex );
				o.texcoord = TRANSFORM_TEX( v.texcoord.xy, _MainTex );
				float3 eyePos = UnityObjectToViewPos( v.vertex );
				o.clipUV = mul( unity_GUIClipTextureMatrix, float4( eyePos.xy, 0, 1.0 ) );
				return o;
			}

			fixed4 frag( v2f i ) : SV_Target
			{
				float2 fraction = sign(frac(i.texcoord * 5) * 2 - 1);
				float3 back = saturate(fraction.x*fraction.y) * 0.125 + 0.275 + 0.05;
				float4 c = tex2D( _MainTex, i.texcoord );
				c.rgb = lerp( back, c.rgb, c.a );

				c.a = tex2D( _GUIClipTexture, i.clipUV ).a;
				return c;
			}
			ENDCG
		}

		Pass // Copy Alpha 4
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _A;

			fixed4 frag (v2f_img i ) : SV_Target
			{
				float alpha = tex2D( _A, i.uv ).a;
				fixed4 finalColor = (float4(tex2D( _MainTex, i.uv ).rgb , alpha));
				return finalColor;
			}
			ENDCG
		}

		Pass // Fix albedo 5
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _A; //specular

			fixed4 frag (v2f_img i ) : SV_Target
			{
				float3 spec = tex2D( _A, i.uv ).rgb;
				float4 alb = tex2D( _MainTex, i.uv );
				alb.rgb = alb.rgb / (1-spec);
				return alb;
			}
			ENDCG
		}

		Pass // TGA BGR format 6
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;

			fixed4 frag(v2f_img i) : SV_Target
			{
				return tex2D(_MainTex, i.uv).bgra;
			}
			ENDCG
		}

		Pass // point sampling 7
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;

			fixed4 frag(v2f_img i) : SV_Target
			{
				return tex2Dlod(_MainTex, float4(i.uv, 0, 0));
			}
			ENDCG
		}

		Pass // point sampling alpha 8
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;

			fixed4 frag(v2f_img i) : SV_Target
			{
				fixed4 finalColor = tex2Dlod(_MainTex, float4(i.uv, 0, 0));
				return float4(0, 0, 0, finalColor.a);
			}
			ENDCG
		}

		Pass // transform normal 9
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4x4 _Matrix;

			fixed4 frag(v2f_img i) : SV_Target
			{
				fixed4 finalColor = tex2Dlod(_MainTex, float4(i.uv, 0, 0));
				finalColor.xyz = mul(_Matrix, float4(finalColor.xyz * 2 - 1,1)).xyz * 0.5 + 0.5;
				return finalColor;
			}
			ENDCG
		}
	}
}
