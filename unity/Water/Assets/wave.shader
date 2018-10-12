Shader "Water/wave"
{
	Properties
	{
		_WaveMap ("_WaveMap", 2D) = "white" {}
		_MaskMap ("_MaskMap", 2D) = "white" {}

		_Speed ("_Speed", Range (0.01, 1)) = 1
		_Intensity ("_Intensity", Range (0.01, 1)) = 1
		_Amplitude ("_Amplitude", Range (0.01, 1)) = 1
		_VertexWaveAmplitudeSpeed ("_VertexWaveAmplitudeSpeed", Range (0.01, 1)) = 1
	}
	SubShader
	{
		Tags {"Queue"="Transparent+100" "RenderType"="Transparent"  }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		//Blend One One
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			#include "AutoLight.cginc"
            #include "Lighting.cginc"

            //vs:
			uniform   float4 _VertexWaveAmplitudeSpeed;

			//ps:
			uniform  float _Intensity;
			uniform  float _Amplitude;
			uniform  float _Speed;

			struct appdata
			{
				float4 color : COLOR;
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal  : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 xlv_COLOR  : TEXCOORD1;
				float3 xlv_normal : TEXCOORD2;
				UNITY_FOG_COORDS(3)

				float4 vertex : SV_POSITION;
			};

			sampler2D _WaveMap;
			float4 _WaveMap_ST;

			sampler2D _MaskMap;
			float4 _MaskMap_ST;





			
			v2f vert (appdata v)
			{
				v2f o;
				float4 vertex_2;
				vertex_2.w = v.vertex.w;
				float _offset;
				_offset = ((normalize(v.vertex.xyz) * _Time.y) * 10.0).x;

				//vertex_2.x = (v.vertex.x + (sin(
				//    (_offset * _VertexWaveAmplitudeSpeed.z)
				//  ) * _VertexWaveAmplitudeSpeed.x));
				//  vertex_2.y = (v.vertex.y + 0.5);

				 vertex_2.xzw =v.vertex.xzw;
				 float v0 =  (v.vertex.x + v.vertex.y + v.vertex.z)*15;
			   	vertex_2.y = v.vertex.y + sin( _Time.y + v0 ) * 0.2;

				
				//vertex_2.z = (v.vertex.z + (sin(
				//    (_offset * _VertexWaveAmplitudeSpeed.w)
				//  ) * _VertexWaveAmplitudeSpeed.y));

				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.vertex = UnityObjectToClipPos(vertex_2);
				o.xlv_COLOR = v.color;
				o.uv = TRANSFORM_TEX(v.uv, _WaveMap);
				o.xlv_normal = v.normal;



				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

			 	float3 _LightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float mask_1;
				float wave1_2;
				float wave0_3;
				float _offset;
				_offset = (_Time.y * _Speed);
				float2 _uv2;
				float2 _uv_mid = (i.uv + float2(0.0, 0.5));
				_uv2 = (_uv_mid + ((float2(0.0, 1.0) * 
				sin(_offset)
				) * _Amplitude));

				float2 _uv3;
				_uv3 = (_uv_mid + ((float2(0.0, 1.0) * 
				sin((_offset + 3.14))
				) * _Amplitude));
 
				wave0_3 = tex2D (_WaveMap, _uv2).w;
				wave1_2 = tex2D (_WaveMap, _uv3).w;

				mask_1 = tex2D (_MaskMap,  i.uv).x;


				float4 final_color;
				final_color.xyz = float3(1.0, 1.0, 1.0);
				final_color.w = (((
				((wave0_3 * (i.xlv_COLOR.w * (
				  sin((_offset + 0.5495))
				 + 1.0))) + (wave1_2 * (i.xlv_COLOR.w * (
				  sin((_offset + 3.768))
				 + 1.0))))
				* 
				(dot (i.xlv_normal, _LightDirection) + 1.0)
				) * _Intensity) * clamp ((mask_1 * 2.0), 0.0, 1.0));


				UNITY_APPLY_FOG(i.fogCoord, final_color);

				return final_color;

			}
			ENDCG
		}
	}
}
