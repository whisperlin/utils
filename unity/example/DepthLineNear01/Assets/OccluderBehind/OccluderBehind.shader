Shader "Hidden/OccluderBehind"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Behind ("_Behind", 2D) = "white" {}
		_Occluder ("_Occluder", 2D) = "white" {}
		_PatternTex ("_PatternTex", 2D) = "white" {}

		
		_Color("Color",Color) = (0.5,0.5,1,1)
		_PatternWeight("_PatternWeight",Range(0,1)) = 1
		_PatternScale("_PatternScale",Range(0,1)) = 1 
		 
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 pos : SV_POSITION;
			};
			sampler2D _MainTex;
			sampler2D _Behind;
			sampler2D _Occluder;
			sampler2D _PatternTex;
			float4 _Color;
			float _PatternWeight;
			float  _PatternScale;
			v2f vert ( appdata_img v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos ( v.vertex );
				o.uv = v.texcoord.xy;
				o.uv1 = o.uv;



				//o.uv1.y = 1- o.uv1.y;
				return o;
			}
			fixed4 frag ( v2f i ) : COLOR
			{
				float behindDepth, occluderDepth;
				float3 behindNormal, occluderNormal;
				float4 col0 = tex2D ( _Behind, i.uv1 );
				float4 col1 = tex2D ( _Occluder, i.uv1 );
			 
				//return col0;
				//return DecodeFloatRG(col0.zw);
				DecodeDepthNormal ( col0, behindDepth, behindNormal );
				
				DecodeDepthNormal ( col1, occluderDepth, occluderNormal );
				//return behindDepth*0.5 ;
				fixed4 scene = tex2D ( _MainTex, i.uv );
				fixed4 pattern = tex2D ( _PatternTex, ( i.uv + _SinTime.w / 100 ) / _PatternScale );
				if (behindDepth > 0 && occluderDepth > 0 && behindDepth > occluderDepth)
				{
					//return float4(1,0,0,1);
					float factor = 0.1 + 0.9 * pow ( max ( dot ( float3 ( 0, 0, 1 ), behindNormal ), 0.0 ), 1.2 );
					return fixed4 ( lerp ( scene, _Color, lerp ( factor, factor * pattern.r, _PatternWeight ) ) );
				}
 				else
					return scene;
			}
			ENDCG
		}
	}
}
