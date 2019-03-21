Shader "Hidden/SSS"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlurRadius("BlurRadius", Range(0,10)) = 1
		_InvWidth("_InvWidth", float) = 0.001
		_InvHeight("_InvHeight", float) = 0.001

	}
	SubShader
	{
 
		Cull Off ZWrite Off ZTest Off
		
		Stencil {
			Ref 5
			comp equal
			pass keep
		}
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
				float4 uv01 : TEXCOORD1;    //一个vector4存储两个纹理坐标  
				float4 uv23 : TEXCOORD2;    //一个vector4存储两个纹理坐标  
				float4 uv45 : TEXCOORD3;    //一个vector4存储两个纹理坐标  
				float4 vertex : SV_POSITION;
			};
			float _BlurRadius;
			float _InvWidth;
			float _InvHeight;
			v2f vert (appdata v)
			{
				float4 _offsets = float4(_BlurRadius*_InvWidth, 0,0,0);
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv01 = v.uv.xyxy + _offsets.xyxy * float4(1, 1, -1, -1);
				o.uv23 = v.uv.xyxy + _offsets.xyxy * float4(1, 1, -1, -1) * 2.0;
				o.uv45 = v.uv.xyxy + _offsets.xyxy * float4(1, 1, -1, -1) * 3.0;

				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = 0.4 * tex2D(_MainTex, i.uv);
				col += 0.15 * tex2D(_MainTex, i.uv01.xy);
				col += 0.15 * tex2D(_MainTex, i.uv01.zw);
				col += 0.10 * tex2D(_MainTex, i.uv23.xy);
				col += 0.10 * tex2D(_MainTex, i.uv23.zw);
				col += 0.05 * tex2D(_MainTex, i.uv45.xy);
				col += 0.05 * tex2D(_MainTex, i.uv45.zw);
				return col;
			}
			ENDCG
		}

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
				float4 uv01 : TEXCOORD1;    //一个vector4存储两个纹理坐标  
				float4 uv23 : TEXCOORD2;    //一个vector4存储两个纹理坐标  
				float4 uv45 : TEXCOORD3;    //一个vector4存储两个纹理坐标  
				float4 vertex : SV_POSITION;
			};
			float _BlurRadius;
			float _InvWidth;
			float _InvHeight;
			v2f vert(appdata v)
			{
				float4 _offsets = float4(0, _BlurRadius*_InvWidth, 0,0);
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv01 = v.uv.xyxy + _offsets.xyxy * float4(1, 1, -1, -1);
				o.uv23 = v.uv.xyxy + _offsets.xyxy * float4(1, 1, -1, -1) * 2.0;
				o.uv45 = v.uv.xyxy + _offsets.xyxy * float4(1, 1, -1, -1) * 3.0;

				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i): SV_Target
			{
				fixed4 col = 0.4 * tex2D(_MainTex, i.uv);
				col += 0.15 * tex2D(_MainTex, i.uv01.xy);
				col += 0.15 * tex2D(_MainTex, i.uv01.zw);
				col += 0.10 * tex2D(_MainTex, i.uv23.xy);
				col += 0.10 * tex2D(_MainTex, i.uv23.zw);
				col += 0.05 * tex2D(_MainTex, i.uv45.xy);
				col += 0.05 * tex2D(_MainTex, i.uv45.zw);
				return col;
			}
				ENDCG
		}
	}
}
