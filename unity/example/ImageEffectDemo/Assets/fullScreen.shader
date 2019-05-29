Shader "Hidden/fullScreen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_SampleTex ("Texture", 2D) = "white" {}
		_Alpha("Alpha",Range(0,1)) = 0.5
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
			sampler2D _MainTex;float4 _MainTex_TexelSize;
			sampler2D _SampleTex;float4 _SampleTex_TexelSize;
			v2f vert (appdata v)
			{
				
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;

				#if UNITY_UV_STARTS_AT_TOP  
					if (_MainTex_TexelSize.y < 0)
						o.uv.y = 1 - o.uv.y;
					else
						o.uv.y = o.uv.y;
				#endif  


				float k1 = _MainTex_TexelSize.z/_MainTex_TexelSize.w;
				float k2 = _SampleTex_TexelSize.z/_SampleTex_TexelSize.w;

				if(k1>k2)//两边黑.
				{
					float k = k1/k2;
					o.uv.x = o.uv.x*k + (1-k)*0.5 ;
				}
				else//上下黑.
				{
					//k1 =  1.0/k1;
					//k2 = 1.0/k2;
					float k = k2/k1;
					o.uv.y = o.uv.y*k + (1-k)*0.5 ;
				}
				return o;
			}
			

		 
			float _Alpha ;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_SampleTex, i.uv);
 
				_Alpha  =  _Alpha* step(i.uv.x,1)*step(0,i.uv.x)  * step(i.uv.y,1)*step(0,i.uv.y);
			 
 
			 	//if(i.uv.x > 1 ||i.uv.x<0|| i.uv.y > 1 || i.uv.y<0)
			 	//	_Alpha = 0;
				return col*(1.0-_Alpha)+col2*_Alpha;
			}
			ENDCG
		}
	}
}
