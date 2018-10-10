Shader "Test/VSM"
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
			#include "Shadow.cginc"
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
				float4 vertex : SV_POSITION;
				float4 pos : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
 
				float4 wpos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				 
				//o.vertex = mul(UNITY_MATRIX_VP, wpos); 
				o.vertex = mul(_WorldToLight, wpos); 
				o.pos = o.vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float depth = i.pos.z /i.pos.w;
				float moment1 = depth;
				float moment2 = depth * depth;

				float dx = ddx(depth);
				float dy = ddy(depth);
				moment2 += 0.25*(dx*dx+dy*dy);
				return float4(EncodeFloatRG(moment1).rg,EncodeFloatRG(moment2).rg);
			}
			ENDCG
		}
	}
}
