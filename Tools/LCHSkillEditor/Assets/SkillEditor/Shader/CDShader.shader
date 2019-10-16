// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "TA/UI/CD Shader" {
	Properties{
		[PerRendererData]_MainTex("MainTex", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_TextureRotator("Texture Rotator", Range(0, 1)) = 1
		[MaterialToggle] _FillClockwise("Fill Clockwise", Float) = 1
		[HideInInspector]_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader{
			Tags {
				"IgnoreProjector" = "True"
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
				"CanUseSpriteAtlas" = "True"
				"PreviewType" = "Plane"
			}
			Pass {
				Name "FORWARD"
				Tags {
					"LightMode" = "ForwardBase"
				}
				Blend One OneMinusSrcAlpha
				Cull Off
				ZWrite Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#define UNITY_PASS_FORWARDBASE
				#pragma multi_compile _ PIXELSNAP_ON
				#include "UnityCG.cginc"
				#pragma multi_compile_fwdbase
				#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2
				#pragma target 3.0
				uniform sampler2D _MainTex;
							uniform float4 _MainTex_ST;
				uniform float4 _Color;
				uniform float _TextureRotator;
				uniform fixed _FillClockwise;

							static const float TAU = float(6.283185);

				struct VertexInput {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float2 texcoord0 : TEXCOORD0;
				};

				struct VertexOutput {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					float4 posWorld : TEXCOORD1;
					float3 normalDir : TEXCOORD2;
					float3 tangentDir : TEXCOORD3;
					float3 bitangentDir : TEXCOORD4;
				};

				VertexOutput vert(VertexInput v) {
					VertexOutput o = (VertexOutput)0;
					o.uv0 = v.texcoord0;
					o.normalDir = UnityObjectToWorldNormal(v.normal);
					o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
					o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
					o.posWorld = mul(unity_ObjectToWorld, v.vertex);
					o.pos = UnityObjectToClipPos(v.vertex);
					#ifdef PIXELSNAP_ON
						o.pos = UnityPixelSnap(o.pos);
					#endif

					return o;
				}

				float4 frag(VertexOutput i) : COLOR {
					i.normalDir = normalize(i.normalDir);
					float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
									float2 clockCounterDirection = _FillClockwise ? float2(1, -1) : float2(1, 1);
									float2 startPoint = (-1 * (i.uv0 - 0.5)) * clockCounterDirection;

					float cutoffRotator_cos = cos(TAU);
					float cutoffRotator_sin = sin(TAU);
									float2x2 cutoffRotationMatrix = float2x2(cutoffRotator_cos, -cutoffRotator_sin, cutoffRotator_sin, cutoffRotator_cos);
									float2 cutoffRotator = mul(startPoint, cutoffRotationMatrix);

									float atan2Mask = atan2(cutoffRotator.g, cutoffRotator.r);
									float atan2MaskNormalized = (atan2Mask / TAU) + 0.5;
									float atan2MaskRotatable = atan2MaskNormalized - _TextureRotator;
									float finalMask = (1.0 - ceil(atan2MaskRotatable));
					clip(finalMask - 0.5);

									return _MainTex_var;
				}

				ENDCG
			}
		}

			FallBack "Diffuse"
}