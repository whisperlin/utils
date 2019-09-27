Shader "Unlit/Selecter"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ctrl("ctrl",Range(0,1)) = 0.8
		_speed("_speed",Range(1,20)) = 8
	}
	SubShader
	{
		Tags {
			"IgnoreProjector" = "True"
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}
		LOD 100

		Pass
		{
			Blend One One
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
				float power : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
			};
			float _ctrl;
			float _speed;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.power = _ctrl +sin(_Time.y*_speed)*(1- _ctrl)*0.5;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb *= i.power;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
