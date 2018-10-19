// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Test/RSM"
{
	Properties
	{
		
 
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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 pos : TEXCOORD0;
				float4 wnormal : TEXCOORD1;
			};
			fixed4 EncodeNormial(fixed3 n)
			{
				return fixed4( n *0.5 + fixed3(0.5,0.5,0.5)   ,1);
			}

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			v2f vert (appdata v)
			{
				v2f o;
 
				float4 wpos = mul(unity_ObjectToWorld, v.vertex);
				//o.vertex = UnityObjectToClipPos(v.vertex);

				float4 _pos = mul(_WorldToLight, wpos); 
				//_pos = mul(_WorldToLightInv, _pos); 
				//_pos = mul(_WorldToLight, _pos); 

 
				o.vertex = _pos; 
 
 
				o.wnormal =   normalize(  mul(unity_ObjectToWorld,v.normal) );
 
		 
				o.pos = o.vertex;
				return o;
			}
			struct RTMOut
			{
				fixed4 color0 : COLOR0;
				fixed4 color1 : COLOR1;
				fixed4 color2 : COLOR2;
			};
			RTMOut frag (v2f i) : SV_Target
			{
				float depth = i.pos.z /i.pos.w;

				RTMOut o;
				o.color0 = _Color;//反射颜色
 
				o.color1 = EncodeFloatRGBA(depth);//深度
 

				o.color2 =   EncodeNormial( i.wnormal );
				return o;
				 

			}
			ENDCG
		}
	}
}
