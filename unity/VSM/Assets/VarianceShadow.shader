Shader "Unlit/VarianceShadow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"  "DisableBatching" = "True"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "Shadow.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 lvuv : TEXCOORD1;
				float4 lv_pos : TEXCOORD2;
				float dep : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float4 wpos = mul(unity_ObjectToWorld, v.vertex); 
				float4 lv_pos =   mul(_WorldToLight, wpos); 
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float2 uv =  float2( (lv_pos.x+1)/2  , (lv_pos.y+1)/2);
				//uv.x = 1 - uv.x;
				uv.y = 1-uv.y;
				o.lvuv = uv;
				o.dep =  lv_pos.z / lv_pos.w;
				return o;
			}
			

	
			float4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float4 col = tex2D(_MainTex, i.uv);
				float d = i.dep;
		 		float s = chebyshevUpperBound(d,i.lvuv);
				return col *s;
			}
			ENDCG
		}
	}
}
