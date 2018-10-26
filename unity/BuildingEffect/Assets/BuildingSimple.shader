 

Shader "Unlit/BuildingSimple" {
	Properties{
		_Color("Main Color", Color) = (0.82, 0.88, 0.965, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BumpMap("Bump", 2D) = "bump" {}


		_ImageLight ("Texture", 2D) = "black" {}
		_ImageColor ("法术光颜色",Color) = (0.5,0.5,1,1)
		_Noise ("噪波", 2D) = "write" {}
		_Speed("噪波速度",Range(-2,2)) =  1
		_NoiseV ("噪波UV伸缩",Range(0.1,5)) =  1



		_OverlayColor("OverlayColor", Color) = (1, 1, 1, 1)
		_SpecColor("SpecColor", Color) = (1, 1, 1, 1)
		_Specular("Specular", Range(0, 10)) = 0.78
		_Shininess("Shininess", Range(0.01, 1)) = 0.025

		_LightDirect("Light Direct(XYZ), Weak Light 0 ~ 2(W)", Vector) = (6, 27, 14, 0.35)
		_LightAdd("Light Add", float) = 1.45
		_LightWhite("Light White", Range(0, 1)) = 0.13

 
 
	}
	
	CGINCLUDE
		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _BumpMap;


	 
		sampler2D _ImageLight;
		float4 _ImageLight_ST;
		sampler2D _Noise;
		float4 _Noise_ST;

		float4 _ImageColor;
		float _Speed;
		float _NoiseV;


 
		fixed4 _OverlayColor;

		float4 _MainTex_ST;
		float4 _BumpMap_ST;
		float4 _LightDirect;

		float _LightAdd;
		float _LightWhite;
 

		fixed4 _SpecColor;
		fixed _Specular;

		fixed _Shininess;
 
 

	ENDCG

	SubShader{
			Tags{ "Queue" = "Geometry+5" }
			LOD 200

			Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha
				NAME"BASE"
				CULL BACK
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				#pragma multi_compile DISSON_OFF DISSIVE_ON


				struct VertexInput {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float2 texcoord : TEXCOORD0;
				};

				struct vertOut {
					float4 pos:SV_POSITION;
					float2 uv: TEXCOORD0;
					float4 vertex : TEXCOORD1;
					float3 normalDir : TEXCOORD2;
					float3 tangentDir : TEXCOORD3;
					float3 bitangentDir : TEXCOORD4;
					float4 posWorld : TEXCOORD5;

					float2 uv1: TEXCOORD6;
					float2 uv2: TEXCOORD7;
				};

				vertOut vert(VertexInput v) {
					vertOut o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.vertex = v.vertex;
					o.normalDir = normalize(mul(float4(v.normal, 0), unity_WorldToObject).xyz);
					o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
					o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
					o.posWorld = mul(unity_ObjectToWorld, v.vertex);

					o.uv1 = float2(0.5,v.vertex.y*_NoiseV);
					o.uv2 = TRANSFORM_TEX(v.texcoord, _ImageLight);

					return o;
				}

				 
				fixed4 frag(vertOut i) : COLOR0
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					fixed4 bump = tex2D(_BumpMap, i.uv);
 
					fixed4 roughCol = fixed4(1, 1, 1, 1) ;

					float3x3 tangentTransform = float3x3(i.tangentDir, i.bitangentDir, i.normalDir);
					float3 normalLocal = UnpackNormal(bump).rgb;
					float3 normalDirection = normalize(mul(normalLocal, tangentTransform));
					float3 lightDir = normalize(_LightDirect.xyz);
					fixed3 viewDir = normalize(WorldSpaceViewDir(i.vertex));

					float NdotL = max(0.0, dot(lightDir, normalDirection));

					float3 reverseRough = (float3(1, 1, 1) - roughCol.rgb);
					float3 white = float3(_LightWhite, _LightWhite, _LightWhite) * reverseRough;
					col.rgb = col.rgb * (float3(1, 1, 1) - white) + white;

					fixed nh = saturate(dot(normalDirection, normalize(lightDir + viewDir)));
					nh = pow(nh, _Shininess * 128);

					float3 tmp = _Color.rgb * _LightAdd * (NdotL + _LightDirect.w) / (1 + _LightDirect.w) *(reverseRough + roughCol.rgb * (0.7 + nh * roughCol.a * 2));
                    col.rgb *= tmp;

			 

					fixed3 spec = _SpecColor.rgb * nh * _Specular * roughCol.a * reverseRough;
					col.rgb += spec;

					col.a *= _Color.a;
 

					fixed4 hr = tex2D(_Noise, i.uv1 + half2(0,_Time.y*_Speed));
					fixed4 il = tex2D(_ImageLight, i.uv2);
					float4 a0 =  il.g*hr.g;
					col.rgb =  col.rgb+ a0 * _ImageColor.rgb;
					col.a = max(col.a,a0);

					return col;
				}
					ENDCG
			}


		}
}
