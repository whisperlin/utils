// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Normal/SelfIllumin_BumpRoughModify" {
	Properties{
		_Color("Main Color", Color) = (0.82, 0.88, 0.965, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BumpMap("Bump", 2D) = "bump" {}
		_GrayTex("Gray Tex", 2D) = "white" {}
		_Darker("Darker", Range(0, 1)) = 1

		_OverlayColor("OverlayColor", Color) = (1, 1, 1, 1)
		_SpecColor("SpecColor", Color) = (1, 1, 1, 1)
		_Specular("Specular", Range(0, 10)) = 0.78
		_Shininess("Shininess", Range(0.01, 1)) = 0.025

		_LightDirect("Light Direct(XYZ), Weak Light 0 ~ 2(W)", Vector) = (6, 27, 14, 0.35)
		_LightAdd("Light Add", float) = 1.45
		_LightWhite("Light White", Range(0, 1)) = 0.13

		_FlashSpeed("【流光速度】", Range(0, 1)) = 0
		_FlashTex("【流光纹理】", 2D) = "white" {}
	}
	
	CGINCLUDE
		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _GrayTex;
		fixed4 _OverlayColor;

		float4 _MainTex_ST;
		float4 _BumpMap_ST;
		float4 _LightDirect;

		float _LightAdd;
		float _LightWhite;
		fixed _Darker;

		fixed4 _SpecColor;
		fixed _Specular;

		fixed _Shininess;
		fixed _FlashSpeed;
		uniform sampler2D _FlashTex;

	ENDCG

	SubShader{
			Tags{ "Queue" = "Geometry+5" }
			LOD 200

			Pass
			{
				NAME"BASE"
				CULL BACK
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				//#pragma multi_compile DISSON_OFF DISSIVE_ON

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
				};

				vertOut vert(VertexInput v) {
					vertOut o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.vertex = v.vertex;
					o.normalDir = normalize(mul(float4(v.normal, 0), unity_WorldToObject).xyz);
					o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
					o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);

					return o;
				}

				fixed4 frag(vertOut i) : COLOR0
				{
					fixed4 write =  fixed4(1, 1, 1, 1);
					fixed4 col = tex2D(_MainTex, i.uv);
					fixed4 bump = tex2D(_BumpMap, i.uv);
					fixed4 grayCol = tex2D(_GrayTex, i.uv);
					fixed4 roughCol = write* grayCol.b;

					fixed3x3 tangentTransform = fixed3x3(i.tangentDir, i.bitangentDir, i.normalDir);
					fixed3 normalLocal = UnpackNormal(bump).rgb;
					fixed3 normalDirection = normalize(mul(normalLocal, tangentTransform));
					fixed3 lightDir = normalize(_LightDirect.xyz);
					fixed3 viewDir = normalize(WorldSpaceViewDir(i.vertex));

					fixed NdotL = max(0.0, dot(lightDir, normalDirection));

					fixed3 reverseRough = (fixed3(1, 1, 1) - roughCol.rgb);
					fixed3 white = fixed3(_LightWhite, _LightWhite, _LightWhite) * reverseRough;
					col.rgb = col.rgb * (fixed3(1, 1, 1) - white) + white;

					fixed nh = saturate(dot(normalDirection, normalize(lightDir + viewDir)));
					nh = pow(nh, _Shininess * 128);

					fixed3 tmp = _Color.rgb * _LightAdd * (NdotL + _LightDirect.w) / (1 + _LightDirect.w) *(reverseRough + roughCol.rgb * (0.7 + nh * roughCol.a * 2));
                    col.rgb *= tmp;

					//局部染色
					 
					//col.rgb *= fixed3(1,1,1);
					//if (grayCol.r > 0.5)
					//col.rgb *= lerp(fixed3(1,1,1), _OverlayColor,step(0.5,grayCol.r) );
					col.rgb *= lerp(write.rgb, _OverlayColor,step(0.5,grayCol.r) );

					fixed3 spec = _SpecColor.rgb * nh * _Specular * roughCol.a * reverseRough;
					col.rgb += spec;

					col.a *= _Color.a;
					col.rgb = col.rgb * _Darker;

				 

					half2 flashUV = i.uv.xy + half2(0, _FlashSpeed) * _Time.y;
					fixed4 flashCol = tex2D(_FlashTex, flashUV);
					col.rgb = col.rgb +  write.xyz * max(0, flashCol.a) *step(0.001,_FlashSpeed);

					//half2 flashUV = i.uv.xy + half2(0, _FlashSpeed) * _Time.y;
					//fixed4 flashCol = tex2D(_FlashTex, flashUV);
					//col.rgb = col.rgb + step(0,_FlashSpeed)*fixed3(1, 1, 1) * max(0, flashCol.a);

					return col;
				}
					ENDCG
			}


		}
}
