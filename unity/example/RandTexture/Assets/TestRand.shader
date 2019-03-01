Shader "Hidden/TestRand"
{
	Properties
	{
	 
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
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
 

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float halfD = uv.x;
				float gross = uv.y;
				

				return float4(pow(halfD, gross*256),0, 0, 1);
				
			}
			ENDCG
		}
	}
}
