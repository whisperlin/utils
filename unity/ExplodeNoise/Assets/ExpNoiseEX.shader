Shader "Unlit/ExpNoiseEX"
{
	Properties {
		_BumpAmt  ("Distortion", range (0,1024)) = 10
		_Power  ("_Power", range (0,1)) = 0.001
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

					half3 tspace0 : TEXCOORD2; // tangent.x, bitangent.x, normal.x
	                half3 tspace1 : TEXCOORD3; // tangent.y, bitangent.y, normal.y
	                half3 tspace2 : TEXCOORD4; // tangent.z, bitangent.z, normal.z
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

					half3 wNormal = UnityObjectToWorldNormal(v.normal);
	                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
 
	                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
	                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
	                // output the tangent space matrix
	                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
	                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
	                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

					return o;
				}
 
				sampler2D _GrabTexture;
				float4 _GrabTexture_TexelSize;
				sampler2D _BumpMap;
				float _Power;
 
				half4 frag( v2f i ) : COLOR
				{
					// calculate perturbed coordinates
					half3 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )); // we could optimize this by just reading the x & y without reconstructing the Z


					half3 worldNormal;
					worldNormal.x = dot(i.tspace0, bump);
					worldNormal.y = dot(i.tspace1, bump);
					worldNormal.z = dot(i.tspace2, bump);

					half3 localNormal =  normalize( mul(unity_WorldToObject, worldNormal));
					 
					float2 offset = localNormal.xz * _BumpAmt * _GrabTexture_TexelSize.xy;
 					//

 					
		 			//offset = bump *_Power;
					i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

					 //offset = bump *_Power;
					// i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
	
					 
					//float newUvX = i.uvgrab.x / i.uvgrab.w;
					//float newUvY = i.uvgrab.y / i.uvgrab.w;
 
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