Shader "Hidden/LinearMaterial"
{
	Properties
	{ 
		_MainTex( "Texture", any ) = "" {} 
		_BackGround( "Back", 2D) = "white" {}
	}

	SubShader
	{
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _BackGround;
			uniform float4 _MainTex_ST;
			uniform float4 _BackGround_ST;
			float _DrawSphere;
			float _InvertedZoom;
			//float _IsLinear;
			float4 _Mask;

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
				float3 back = tex2D( _BackGround, ( i.texcoord - 0.5) * 2 * _InvertedZoom).b;// *0.25 + 0.25;
				float alpha = 1;
				float2 p = 2 * i.texcoord - 1;
				float r = sqrt( dot( p,p ) );

				if ( _DrawSphere == 1 )
					alpha = saturate( ( 1 - r )*( 45 * _InvertedZoom + 5 ) );

				float4 c = 0;
				c = tex2D( _MainTex, i.texcoord );
				c.rgb *= _Mask.rgb;

				if ( !IsGammaSpace() )
					c.rgb = LinearToGammaSpace( c.rgb );
				if(_Mask.w == 1)
					c.rgb = lerp( back, c.rgb, c.a * alpha);
				else
					c.rgb *= alpha;

				c.a = tex2D( _GUIClipTexture, i.clipUV ).a;
				return c;
			}
			ENDCG
		}
	}
	Fallback Off
}
