Shader "Unlit/ExpNoiseSimple"
{
	Properties {
		_BumpAmt  ("Distortion", range (0,1024)) = 10
		_MarkMap ("Normalmap", 2D) = "bump" {}
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
				float4 _MarkMap_ST;
 
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
 
					o.uvbump = TRANSFORM_TEX( v.texcoord, _MarkMap );

 

					return o;
				}
 
				sampler2D _GrabTexture;
				float4 _GrabTexture_TexelSize;
				sampler2D _MarkMap;
	 
 
				half4 frag( v2f i ) : COLOR
				{
					// calculate perturbed coordinates
					half2 bump = tex2D( _MarkMap, i.uvbump ).rr;
 
					
 					

 					
 					float2 offset =bump * _BumpAmt * _GrabTexture_TexelSize.xy;
					//i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

				 	float2 uv2 =  i.uvgrab.xy/ i.uvgrab.w;


				 	if(bump.r>0)
				 	{
					 	float2 dir =   normalize( i.uvbump - float2(0.5,0.5));
					 	uv2 = offset*dir  / i.uvgrab.w + uv2;
					 	//uv2 = offset  / i.uvgrab.w + uv2;
					 	//return float4(offset * dir,0,1);
					 	//return float4(dir,0,1);
				 	}


 					




					half4 col = tex2D(_GrabTexture,  uv2);
					
					return col;
				}
				ENDCG
			}
		}
 
	 
		 
	}
 
}
