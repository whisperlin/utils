﻿ 
Shader "Unlit/ExpBump"
{
	Properties {
		_BumpAmt  ("Distortion", range (0,1024)) = 10
 
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
 
	Category {
		// We must be transparent, so other objects are drawn before this one.
		Tags { "Queue"="Transparent+100" "RenderType"="Opaque" }
 
		SubShader {
			
			ZWrite Off
			Cull Off
			Fog {Mode Off}
			Lighting Off
 
			// This pass grabs the screen behind the object into a texture.
			// We can access the result in the next pass as _GrabTexture
			GrabPass {							
				Name "BASE"
				Tags { "LightMode" = "Always" }
 			}
 		
 			// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
 			// on to the screen
			Pass {
				Name "BASE"
				Tags { "LightMode" = "Always" }
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
 
				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord: TEXCOORD0;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
				};
 
				struct v2f {
					float4 vertex : POSITION;
					float4 uvgrab : TEXCOORD0;
					float2 uvbump : TEXCOORD1;

 
				};
 
				float _BumpAmt;
				float4 _BumpMap_ST;
 
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
 
					/*
					#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
					#else
					float scale = 1.0;
					#endif
					o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
					o.uvgrab.zw = o.vertex.zw;
					*/
 
					o.uvgrab = ComputeGrabScreenPos(o.vertex);
 
					o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );

 

					return o;
				}
 
				sampler2D _GrabTexture;
				float4 _GrabTexture_TexelSize;
				sampler2D _BumpMap;
 
 
				half4 frag( v2f i ) : COLOR
				{
					// calculate perturbed coordinates
					half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg; // we could optimize this by just reading the x & y without reconstructing the Z
 
					float2 offset =bump * _BumpAmt * _GrabTexture_TexelSize.xy;
 					//

 					
 
					i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

				 
 
					half4 col = tex2D(_GrabTexture,  i.uvgrab.xy/ i.uvgrab.w);
					
					return col;
				}
				ENDCG
			}
		}
 
	 
		SubShader {
			Blend DstColor Zero
			Pass {
				Name "BASE"
				SetTexture [_MainTex] {	combine texture }
			}
		}
	}
 
}