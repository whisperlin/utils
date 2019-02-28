Shader "Unlit/AddMark"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MarkTex("Mark", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
		_ClipValue("裁切值",Range(0,1)) = 0.5
		_MinClip("clip最小值",Range(0,1)) = 0.1
		_MaxClip("clip最大值",Range(0,1)) = 0.9
		_Edge("半径",Range(0,0.5)) = 0.1
		
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			blend srcAlpha One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _MarkTex;
			float4 _MarkTex_ST;
			
			float4 _Color;
			float _MinClip;
			float _ClipValue;
			float _MaxClip;

			float _Edge;
			float smooth(float v)
			{
				return (-(v*v)) + 1;
			}
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv)*_Color;
				fixed4 mark = tex2D(_MarkTex, i.uv);

				float r = _MaxClip - _MinClip;
			 
				//step(mark.r, _ClipValue)
				_ClipValue = _MinClip+_ClipValue*r;
				float v =  clamp(abs(_ClipValue - mark.r)/ _Edge,0,1);
				 
				col.a = col.a * smooth (v);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
