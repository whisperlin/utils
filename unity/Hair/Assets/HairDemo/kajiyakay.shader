// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//basic phong CG shader with spec map


Shader "CG Shaders/Alternative Lighting/Kajiya Kay"
{
	Properties
	{
		_diffuseColor("Diffuse Color", Color) = (1,1,1,1)
		_diffuseMap("Diffuse", 2D) = "white" {}	
		_alphaPower ("Alpha Power", Range(0.0, 3.0)) = 1.0
		_alphaPower (" ", Float) = 1.0
		_normalMap("Normal / Specular (A)", 2D) = "bump" {}
		_specularPower ("Specular Power 1", Range(0.0, 50.0)) = 10
		_specularPower (" ", Float) = 10
		_specularColor("Specular Color 1", Color) = (1,1,1,1)
		_primaryShift ("Specular Shift 1 ", Range(-1.0, 1.0)) = 1
		_primaryShift (" ", Float) = 1
		_specularPower2 ("Specular Power 2", Range(0.0, 50.0)) = 10
		_specularPower2 (" ", Float) = 10
		_specularColor2("Specular Color 2", Color) = (1,1,1,1)
		_secondaryShift ("Specular Shift 2 ", Range(-1.0, 1.0)) = 1
		_secondaryShift (" ", Float) = 1		
		//adding a property for the AO texture
		_AmbientMap("Ambient Occlusion", 2D) = "white" {}
		
		
	}
	SubShader
	{
	
		
			
		
		Pass
		{
		
			Tags  { "Queue" = "AlphaTest"  "RenderType" = "TransparentCutout" }
			Cull Off   
			ZWrite On
			Alphatest GEqual  0.3
			ZTest Less
			ColorMask 0   
			//Tags { "LightMode" = "ForwardBase" } 
            
			CGPROGRAM
			
			#pragma vertex vShader
			#pragma fragment pShader
			#include "UnityCG.cginc"
			
			uniform sampler2D _diffuseMap;
			uniform half4 _diffuseMap_ST;	
			uniform fixed _alphaPower;
			
			struct app2vert {
				float4 vertex 	: 	POSITION;
				fixed2 texCoord : 	TEXCOORD0;
				
			};
			struct vert2Pixel
			{
				float4 pos 						: 	SV_POSITION;
				fixed2 uvs						:	TEXCOORD0;
			};
			
			vert2Pixel vShader(app2vert IN)
			{
				vert2Pixel OUT;
				float4x4 WorldViewProjection = UNITY_MATRIX_MVP;							
				OUT.pos = mul(WorldViewProjection, IN.vertex);
				OUT.uvs = IN.texCoord;		
				return OUT;
			}
			
			fixed4 pShader(vert2Pixel IN): COLOR
			{
				
				half2 diffuseUVs = TRANSFORM_TEX(IN.uvs, _diffuseMap);
				fixed4 texSample = tex2D(_diffuseMap, diffuseUVs);
				texSample.w = pow(texSample.w, _alphaPower);
				return texSample;
			}
			
			ENDCG
		}	
		
		Pass
		{
			Tags { "LightMode" = "ForwardBase" "Queue" = "AlphaTest" "RenderType" = "TransparentCutout"} 
			
			
			Cull Off
			ZWrite On
			ZTest Equal
            
			CGPROGRAM
			
			#pragma vertex vShader
			#pragma fragment pShader
			#include "UnityCG.cginc"
			#pragma multi_compile_fwdbase
			#pragma target 3.0
			
			uniform fixed3 _diffuseColor;
			uniform sampler2D _diffuseMap;
			uniform half4 _diffuseMap_ST;			
			uniform fixed4 _LightColor0; 
			uniform half _specularPower;
			uniform fixed3 _specularColor;
			uniform fixed _primaryShift;
			uniform half _specularPower2;			
			uniform fixed3 _specularColor2;
			uniform fixed _secondaryShift;
			uniform sampler2D _normalMap;
			uniform half4 _normalMap_ST;
			uniform fixed _alphaPower;
			uniform sampler2D _AmbientMap;
			
			struct app2vert {
				float4 vertex 	: 	POSITION;
				fixed2 texCoord : TEXCOORD0;
				fixed2 texCoord1 : TEXCOORD1;
				fixed4 normal 	:	NORMAL;
				fixed4 tangent : TANGENT;
				
			};
			struct vert2Pixel
			{
				float4 pos 						: 	SV_POSITION;
				fixed4 uvs						:	TEXCOORD0;
				fixed3 normalDir						:	TEXCOORD1;	
				fixed3 binormalDir					:	TEXCOORD2;	
				fixed3 tangentDir					:	TEXCOORD3;	
				half3 posWorld						:	TEXCOORD4;	
				fixed3 viewDir						:	TEXCOORD5;
				fixed3 lighting						:	TEXCOORD6;
			};
			
			fixed lambert(fixed3 N, fixed3 L)
			{
				//slightly modified 
				return saturate(lerp(0.25, 1.0, dot(N, L)));
			}
			fixed frensel(fixed3 V, fixed3 N, half P)
			{	
				return pow(1 - saturate(dot(V,N)), P);
			}
			fixed phong(fixed3 R, fixed3 L)
			{
				return pow(saturate(dot(R, L)), _specularPower);
			}
			fixed2 kajiyakay(fixed3 V, fixed3 T, fixed3 B, fixed3 N, fixed3 L, fixed shift)
			{
				fixed3 H = normalize(V + L);
				//using the binormal instead of Tangent since that goes root to tip
				fixed3 binormalDir = normalize(cross(T, N));	

				//shift the tangent via spec map
				shift -= 0.5;
				fixed3 tangent1 = normalize(binormalDir + (_primaryShift + shift) * N);
				fixed3 tangent2 = normalize(binormalDir + (_secondaryShift + shift) * N);
				
				fixed2 spec = fixed2(0,0);
				
				//2 shifted specular terms, retuned as x,y components
				fixed dotTH = dot(tangent1, H);
				half sinTH = sqrt(1.0 - dotTH * dotTH);
				fixed dirAtten = smoothstep(-1.0, 0.0, dotTH);	
				spec.x = dirAtten * pow(sinTH, _specularPower);
				
				dotTH = dot(tangent2, H);
				sinTH = sqrt(1.0 - dotTH * dotTH);
				dirAtten = smoothstep(-1.0, 0.0, dotTH);	
				spec.y = dirAtten * pow(sinTH, _specularPower2);
				
				return saturate(spec);
			}
			vert2Pixel vShader(app2vert IN)
			{
				vert2Pixel OUT;
				float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
				float4x4 WorldInverseTranspose = unity_WorldToObject; 
				float4x4 World = unity_ObjectToWorld;
							
				OUT.pos = mul(WorldViewProjection, IN.vertex);
				//pass both sets along 
				OUT.uvs.xy = IN.texCoord.xy;	
				OUT.uvs.zw = IN.texCoord1.xy;						
				OUT.normalDir = normalize(mul(IN.normal, WorldInverseTranspose).xyz);
				OUT.tangentDir = normalize(mul(IN.tangent, WorldInverseTranspose).xyz);
				OUT.binormalDir = normalize(cross(OUT.normalDir, OUT.tangentDir)); 
				OUT.posWorld = mul(World, IN.vertex).xyz;
				OUT.viewDir = normalize( _WorldSpaceCameraPos - OUT.posWorld );

				//vertex lights
				fixed3 vertexLighting = fixed3(0.0, 0.0, 0.0);
				#ifdef VERTEXLIGHT_ON
				 for (int index = 0; index < 4; index++)
					{    						
						half3 vertexToLightSource = half3(unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index]) - OUT.posWorld;
						fixed attenuation  = (1.0/ length(vertexToLightSource)) *.5;	
						fixed3 diffuse = unity_LightColor[index].xyz * lambert(OUT.normalDir, normalize(vertexToLightSource)) * attenuation;
						vertexLighting = vertexLighting + diffuse;
					}
				vertexLighting = saturate( vertexLighting );
				#endif
				OUT.lighting = vertexLighting ;
				
				return OUT;
			}
			
			fixed4 pShader(vert2Pixel IN): COLOR
			{
				half2 normalUVs = TRANSFORM_TEX(IN.uvs.xy, _normalMap);
				fixed4 normalD = tex2D(_normalMap, normalUVs);
				normalD.xyz = (normalD.xyz * 2) - 1;
			
				//half3 normalDir = half3(2.0 * normalSample.xy - float2(1.0), 0.0);
				//deriving the z component
				//normalDir.z = sqrt(1.0 - dot(normalDir, normalDir));
               // alternatively you can approximate deriving the z component without sqrt like so:  
				//normalDir.z = 1.0 - 0.5 * dot(normalDir, normalDir);
				fixed3 normalDir = normalD.xyz;	
				fixed specMap = normalD.w;
				normalDir = normalize((normalDir.x * IN.tangentDir) + (normalDir.y * IN.binormalDir) + (normalDir.z * IN.normalDir));
				
				fixed3 ambientL = UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				half3 pixelToLightSource =_WorldSpaceLightPos0.xyz - (IN.posWorld*_WorldSpaceLightPos0.w);
				fixed attenuation  = lerp(1.0, 1.0/ length(pixelToLightSource), _WorldSpaceLightPos0.w);				
				fixed3 lightDirection = normalize(pixelToLightSource);
				
				fixed diffuseL =  lambert(normalDir, lightDirection);			
				
				fixed3 diffuse = _LightColor0.xyz * diffuseL* attenuation;				
				diffuse = saturate(IN.lighting + ambientL + diffuse);		
				//specular highlight - use the spec map as the shift texture
				fixed2 specularHighlight =kajiyakay(IN.viewDir, IN.tangentDir, IN.binormalDir, normalDir, lightDirection, specMap);
	
				
				fixed4 outColor;							
				half2 diffuseUVs = TRANSFORM_TEX(IN.uvs.xy, _diffuseMap);
				fixed4 texSample = tex2D(_diffuseMap, diffuseUVs);
				texSample.w = pow(texSample.w, _alphaPower);
				fixed3 diffuseC =  texSample.xyz * _diffuseColor.xyz;
				fixed3 specular = (specularHighlight.x * _specularColor) + (specularHighlight.y * specMap * _specularColor2);
				outColor = fixed4( diffuse * (specular + diffuseC),texSample.w);
				outColor.xyz *= tex2D(_AmbientMap, IN.uvs.zw).x;
				return outColor;
			}
			
			ENDCG
		}	
		
		Pass
		{
			Tags { "LightMode" = "ForwardAdd" "Queue" = "AlphaTest" "RenderType" = "TransparentCutout"} 
			
			Cull Off
			ZWrite On
			ZTest Equal
			Blend One One 
            
			CGPROGRAM
			#pragma vertex vShader
			#pragma fragment pShader
			#include "UnityCG.cginc"
			#pragma target 3.0
			
			uniform fixed3 _diffuseColor;
			uniform sampler2D _diffuseMap;
			uniform half4 _diffuseMap_ST;			
			uniform fixed4 _LightColor0; 
			uniform half _specularPower;
			uniform fixed3 _specularColor;
			uniform fixed _primaryShift;
			uniform half _specularPower2;			
			uniform fixed3 _specularColor2;
			uniform fixed _secondaryShift;
			uniform sampler2D _normalMap;
			uniform half4 _normalMap_ST;	
			uniform fixed _alphaPower;
			uniform sampler2D _AmbientMap;		
			
			
			struct app2vert {
				float4 vertex 	: 	POSITION;
				fixed2 texCoord : TEXCOORD0;
				fixed2 texCoord1 : TEXCOORD1;
				fixed4 normal 	:	NORMAL;
				fixed4 tangent : TANGENT;
			};
			struct vert2Pixel
			{
				float4 pos 						: 	SV_POSITION;
				fixed4 uvs						:	TEXCOORD0;	
				fixed3 normalDir						:	TEXCOORD1;	
				fixed3 binormalDir					:	TEXCOORD2;	
				fixed3 tangentDir					:	TEXCOORD3;	
				half3 posWorld						:	TEXCOORD4;	
				fixed3 viewDir						:	TEXCOORD5;
				fixed4 lighting					:	TEXCOORD6;	
			};
			
			fixed lambert(fixed3 N, fixed3 L)
			{
				//slightly modified 
				return saturate(lerp(0.25, 1.0, dot(N, L)));
			}			
			fixed phong(fixed3 R, fixed3 L)
			{
				return pow(saturate(dot(R, L)), _specularPower);
			}
			fixed2 kajiyakay(fixed3 V, fixed3 T, fixed3 B, fixed3 N, fixed3 L, fixed shift)
			{
				fixed3 H = normalize(V + L);
				//using the binormal instead of Tangent since that goes root to tip
				fixed3 binormalDir = normalize(cross(T, N));	

				//shift the tangent via spec map
				shift -= 0.5;
				fixed3 tangent1 = normalize(binormalDir + (_primaryShift + shift) * N);
				fixed3 tangent2 = normalize(binormalDir + (_secondaryShift + shift) * N);
				
				fixed2 spec = fixed2(0,0);
				
				//2 shifted specular terms, retuned as x,y components
				fixed dotTH = dot(tangent1, H);
				half sinTH = sqrt(1.0 - dotTH * dotTH);
				fixed dirAtten = smoothstep(-1.0, 0.0, dotTH);	
				spec.x = dirAtten * pow(sinTH, _specularPower);
				
				dotTH = dot(tangent2, H);
				sinTH = sqrt(1.0 - dotTH * dotTH);
				dirAtten = smoothstep(-1.0, 0.0, dotTH);	
				spec.y = dirAtten * pow(sinTH, _specularPower2);
				
				return saturate(spec);
			}
			vert2Pixel vShader(app2vert IN)
			{
				vert2Pixel OUT;
				float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
				float4x4 WorldInverseTranspose = unity_WorldToObject; 
				float4x4 World = unity_ObjectToWorld;
				
				OUT.pos = mul(WorldViewProjection, IN.vertex);
				//pass both sets along 
				OUT.uvs.xy = IN.texCoord.xy;	
				OUT.uvs.zw = IN.texCoord1.xy;		
				
				OUT.normalDir = normalize(mul(IN.normal, WorldInverseTranspose).xyz);
				OUT.tangentDir = normalize(mul(IN.tangent, WorldInverseTranspose).xyz);
				OUT.binormalDir = normalize(cross(OUT.normalDir, OUT.tangentDir)); 
				OUT.posWorld = mul(World, IN.vertex).xyz;
				OUT.viewDir = normalize( _WorldSpaceCameraPos - OUT.posWorld );
				return OUT;
			}
			fixed4 pShader(vert2Pixel IN): COLOR
			{
				half2 normalUVs = TRANSFORM_TEX(IN.uvs.xy, _normalMap);
				fixed4 normalD = tex2D(_normalMap, normalUVs);
				normalD.xyz = (normalD.xyz * 2) - 1;
				
				//half3 normalDir = half3(2.0 * normalSample.xy - float2(1.0), 0.0);
				//deriving the z component
				//normalDir.z = sqrt(1.0 - dot(normalDir, normalDir));
               // alternatively you can approximate deriving the z component without sqrt like so: 
				//normalDir.z = 1.0 - 0.5 * dot(normalDir, normalDir);
				
				fixed3 normalDir = normalD.xyz;	
				fixed specMap = normalD.w;
				normalDir = normalize((normalDir.x * IN.tangentDir) + (normalDir.y * IN.binormalDir) + (normalDir.z * IN.normalDir));
								
				//Fill lights
				half3 pixelToLightSource = _WorldSpaceLightPos0.xyz - (IN.posWorld*_WorldSpaceLightPos0.w);
				fixed attenuation  = lerp(1.0, 1.0/ length(pixelToLightSource), _WorldSpaceLightPos0.w);				
				fixed3 lightDirection = normalize(pixelToLightSource);
				
				fixed diffuseL =  lambert(normalDir, lightDirection);			
				fixed3 diffuse = _LightColor0.xyz * diffuseL * attenuation;
			
				//specular highlight - use the spec map as the shift texture
				fixed2 specularHighlight =kajiyakay(IN.viewDir, IN.tangentDir, IN.binormalDir, normalDir, lightDirection, specMap);
	
				fixed4 outColor;							
				half2 diffuseUVs = TRANSFORM_TEX(IN.uvs.xy, _diffuseMap);
				fixed4 texSample = tex2D(_diffuseMap, diffuseUVs);
				texSample.w = pow(texSample.w, _alphaPower);
				fixed3 diffuseC =  texSample.xyz * _diffuseColor.xyz;
				fixed3 specular = (specularHighlight.x * _specularColor) + (specularHighlight.y * specMap * _specularColor2);
				outColor = fixed4( diffuse * (specular + diffuseC)* texSample.w,texSample.w);
				outColor.xyz *= tex2D(_AmbientMap, IN.uvs.zw).x;
				return outColor;
			}
			
			ENDCG
		}	
		
		Pass
		{
			Tags { "LightMode" = "ForwardBase" "Queue" = "Transparent" "RenderType" = "Transparent"} 
			
			Cull Front
			ZWrite Off
			ZTest Less
			Blend SrcAlpha OneMinusSrcAlpha
            
			CGPROGRAM
			
			#pragma vertex vShader
			#pragma fragment pShader
			#include "UnityCG.cginc"
			#pragma multi_compile_fwdbase
			#pragma target 3.0
			
			uniform fixed3 _diffuseColor;
			uniform sampler2D _diffuseMap;
			uniform half4 _diffuseMap_ST;			
			uniform fixed4 _LightColor0; 
			uniform half _specularPower;
			uniform fixed3 _specularColor;
			uniform fixed _primaryShift;
			uniform half _specularPower2;			
			uniform fixed3 _specularColor2;
			uniform fixed _secondaryShift;
			uniform sampler2D _normalMap;
			uniform half4 _normalMap_ST;
			uniform fixed _alphaPower;
			uniform sampler2D _AmbientMap;	
			
			struct app2vert {
				float4 vertex 	: 	POSITION;
				fixed2 texCoord : TEXCOORD0;
				fixed2 texCoord1 : TEXCOORD1;
				fixed4 normal 	:	NORMAL;
				fixed4 tangent : TANGENT;
				
			};
			struct vert2Pixel
			{
				float4 pos 						: 	SV_POSITION;
				fixed4 uvs						:	TEXCOORD0;
				fixed3 normalDir						:	TEXCOORD1;	
				fixed3 binormalDir					:	TEXCOORD2;	
				fixed3 tangentDir					:	TEXCOORD3;	
				half3 posWorld						:	TEXCOORD4;	
				fixed3 viewDir						:	TEXCOORD5;
				fixed3 lighting						:	TEXCOORD6;
			};
			
			fixed lambert(fixed3 N, fixed3 L)
			{
				//slightly modified 
				return saturate(lerp(0.25, 1.0, dot(N, L)));
			}
			fixed frensel(fixed3 V, fixed3 N, half P)
			{	
				return pow(1 - saturate(dot(V,N)), P);
			}
			fixed phong(fixed3 R, fixed3 L)
			{
				return pow(saturate(dot(R, L)), _specularPower);
			}
			fixed2 kajiyakay(fixed3 V, fixed3 T, fixed3 B, fixed3 N, fixed3 L, fixed shift)
			{
				fixed3 H = normalize(V + L);
				//using the binormal instead of Tangent since that goes root to tip
				fixed3 binormalDir = normalize(cross(T, N));	

				//shift the tangent via spec map
				shift -= 0.5;
				fixed3 tangent1 = normalize(binormalDir + (_primaryShift + shift) * N);
				fixed3 tangent2 = normalize(binormalDir + (_secondaryShift + shift) * N);
				
				fixed2 spec = fixed2(0,0);
				
				//2 shifted specular terms, retuned as x,y components
				fixed dotTH = dot(tangent1, H);
				half sinTH = sqrt(1.0 - dotTH * dotTH);
				fixed dirAtten = smoothstep(-1.0, 0.0, dotTH);	
				spec.x = dirAtten * pow(sinTH, _specularPower);
				
				dotTH = dot(tangent2, H);
				sinTH = sqrt(1.0 - dotTH * dotTH);
				dirAtten = smoothstep(-1.0, 0.0, dotTH);	
				spec.y = dirAtten * pow(sinTH, _specularPower2);
				
				return saturate(spec);
			}
			vert2Pixel vShader(app2vert IN)
			{
				vert2Pixel OUT;
				float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
				float4x4 WorldInverseTranspose = unity_WorldToObject; 
				float4x4 World = unity_ObjectToWorld;
							
				OUT.pos = mul(WorldViewProjection, IN.vertex);
				//pass both sets along 
				OUT.uvs.xy = IN.texCoord.xy;	
				OUT.uvs.zw = IN.texCoord1.xy;							
				OUT.normalDir = normalize(mul(IN.normal, WorldInverseTranspose).xyz);
				OUT.tangentDir = normalize(mul(IN.tangent, WorldInverseTranspose).xyz);
				OUT.binormalDir = normalize(cross(OUT.normalDir, OUT.tangentDir)); 
				OUT.posWorld = mul(World, IN.vertex).xyz;
				OUT.viewDir = normalize( _WorldSpaceCameraPos - OUT.posWorld );

				//vertex lights
				fixed3 vertexLighting = fixed3(0.0, 0.0, 0.0);
				#ifdef VERTEXLIGHT_ON
				 for (int index = 0; index < 4; index++)
					{    						
						half3 vertexToLightSource = half3(unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index]) - OUT.posWorld;
						fixed attenuation  = (1.0/ length(vertexToLightSource)) *.5;	
						fixed3 diffuse = unity_LightColor[index].xyz * lambert(OUT.normalDir, normalize(vertexToLightSource)) * attenuation;
						vertexLighting = vertexLighting + diffuse;
					}
				vertexLighting = saturate( vertexLighting );
				#endif
				OUT.lighting = vertexLighting ;
				
				return OUT;
			}
			
			fixed4 pShader(vert2Pixel IN): COLOR
			{
				half2 normalUVs = TRANSFORM_TEX(IN.uvs.xy, _normalMap);
				fixed4 normalD = tex2D(_normalMap, normalUVs);
				normalD.xyz = (normalD.xyz * 2) - 1;
			
				//half3 normalDir = half3(2.0 * normalSample.xy - float2(1.0), 0.0);
				//deriving the z component
				//normalDir.z = sqrt(1.0 - dot(normalDir, normalDir));
               // alternatively you can approximate deriving the z component without sqrt like so:  
				//normalDir.z = 1.0 - 0.5 * dot(normalDir, normalDir);
				fixed3 normalDir = normalD.xyz;	
				fixed specMap = normalD.w;
				normalDir = normalize((normalDir.x * IN.tangentDir) + (normalDir.y * IN.binormalDir) + (normalDir.z * IN.normalDir));
				
				fixed3 ambientL = UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				half3 pixelToLightSource =_WorldSpaceLightPos0.xyz - (IN.posWorld*_WorldSpaceLightPos0.w);
				fixed attenuation  = lerp(1.0, 1.0/ length(pixelToLightSource), _WorldSpaceLightPos0.w);				
				fixed3 lightDirection = normalize(pixelToLightSource);
				
				fixed diffuseL =  lambert(normalDir, lightDirection);
				
				fixed3 diffuse = _LightColor0.xyz * diffuseL* attenuation;
				diffuse = saturate(IN.lighting + ambientL + diffuse);
		
				//specular highlight - use the spec map as the shift texture
				fixed2 specularHighlight =kajiyakay(IN.viewDir, IN.tangentDir, IN.binormalDir, normalDir, lightDirection, specMap);
	
				
				fixed4 outColor;							
				half2 diffuseUVs = TRANSFORM_TEX(IN.uvs.xy, _diffuseMap);
				fixed4 texSample = tex2D(_diffuseMap, diffuseUVs);
				fixed3 diffuseC =  texSample.xyz * _diffuseColor.xyz;
				fixed3 specular = (specularHighlight.x * _specularColor) + (specularHighlight.y * specMap * _specularColor2);
				outColor = fixed4( diffuse * (specular + diffuseC),texSample.w);
				outColor.xyz *= tex2D(_AmbientMap, IN.uvs.zw).x;
				return outColor;
			}
			
			ENDCG
		}	
		
		Pass
		{
			Tags { "LightMode" = "ForwardBase" "Queue" = "Transparent" "RenderType" = "Transparent"} 
			
			Cull Back
			ZWrite Off
			ZTest Less
			Blend SrcAlpha OneMinusSrcAlpha
            
			CGPROGRAM
			
			#pragma vertex vShader
			#pragma fragment pShader
			#include "UnityCG.cginc"
			#pragma multi_compile_fwdbase
			#pragma target 3.0
			
			uniform fixed3 _diffuseColor;
			uniform sampler2D _diffuseMap;
			uniform half4 _diffuseMap_ST;			
			uniform fixed4 _LightColor0; 
			uniform half _specularPower;
			uniform fixed3 _specularColor;
			uniform fixed _primaryShift;
			uniform half _specularPower2;			
			uniform fixed3 _specularColor2;
			uniform fixed _secondaryShift;
			uniform sampler2D _normalMap;
			uniform half4 _normalMap_ST;
			uniform fixed _alphaPower;
			uniform sampler2D _AmbientMap;	
			
			struct app2vert {
				float4 vertex 	: 	POSITION;
				fixed2 texCoord : TEXCOORD0;
				fixed2 texCoord1 : TEXCOORD1;
				fixed4 normal 	:	NORMAL;
				fixed4 tangent : TANGENT;
				
			};
			struct vert2Pixel
			{
				float4 pos 						: 	SV_POSITION;
				fixed4 uvs						:	TEXCOORD0;
				fixed3 normalDir						:	TEXCOORD1;	
				fixed3 binormalDir					:	TEXCOORD2;	
				fixed3 tangentDir					:	TEXCOORD3;	
				half3 posWorld						:	TEXCOORD4;	
				fixed3 viewDir						:	TEXCOORD5;
				fixed3 lighting						:	TEXCOORD6;
			};
			
			fixed lambert(fixed3 N, fixed3 L)
			{
				//slightly modified 
				return saturate(lerp(0.25, 1.0, dot(N, L)));
			}
			fixed frensel(fixed3 V, fixed3 N, half P)
			{	
				return pow(1 - saturate(dot(V,N)), P);
			}
			fixed phong(fixed3 R, fixed3 L)
			{
				return pow(saturate(dot(R, L)), _specularPower);
			}
			fixed2 kajiyakay(fixed3 V, fixed3 T, fixed3 B, fixed3 N, fixed3 L, fixed shift)
			{
				fixed3 H = normalize(V + L);
				//using the binormal instead of Tangent since that goes root to tip
				fixed3 binormalDir = normalize(cross(T, N));	

				//shift the tangent via spec map
				shift -= 0.5;
				fixed3 tangent1 = normalize(binormalDir + (_primaryShift + shift) * N);
				fixed3 tangent2 = normalize(binormalDir + (_secondaryShift + shift) * N);
				
				fixed2 spec = fixed2(0,0);
				
				//2 shifted specular terms, retuned as x,y components
				fixed dotTH = dot(tangent1, H);
				half sinTH = sqrt(1.0 - dotTH * dotTH);
				fixed dirAtten = smoothstep(-1.0, 0.0, dotTH);	
				spec.x = dirAtten * pow(sinTH, _specularPower);
				
				dotTH = dot(tangent2, H);
				sinTH = sqrt(1.0 - dotTH * dotTH);
				dirAtten = smoothstep(-1.0, 0.0, dotTH);	
				spec.y = dirAtten * pow(sinTH, _specularPower2);
				
				return saturate(spec);
			}
			vert2Pixel vShader(app2vert IN)
			{
				vert2Pixel OUT;
				float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
				float4x4 WorldInverseTranspose = unity_WorldToObject; 
				float4x4 World = unity_ObjectToWorld;
							
				OUT.pos = mul(WorldViewProjection, IN.vertex);
				//pass both sets along 
				OUT.uvs.xy = IN.texCoord.xy;	
				OUT.uvs.zw = IN.texCoord1.xy;					
				OUT.normalDir = normalize(mul(IN.normal, WorldInverseTranspose).xyz);
				OUT.tangentDir = normalize(mul(IN.tangent, WorldInverseTranspose).xyz);
				OUT.binormalDir = normalize(cross(OUT.normalDir, OUT.tangentDir)); 
				OUT.posWorld = mul(World, IN.vertex).xyz;
				OUT.viewDir = normalize( _WorldSpaceCameraPos - OUT.posWorld );

				//vertex lights
				fixed3 vertexLighting = fixed3(0.0, 0.0, 0.0);
				#ifdef VERTEXLIGHT_ON
				 for (int index = 0; index < 4; index++)
					{    						
						half3 vertexToLightSource = half3(unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index]) - OUT.posWorld;
						fixed attenuation  = (1.0/ length(vertexToLightSource)) *.5;	
						fixed3 diffuse = unity_LightColor[index].xyz * lambert(OUT.normalDir, normalize(vertexToLightSource)) * attenuation;
						vertexLighting = vertexLighting + diffuse;
					}
				vertexLighting = saturate( vertexLighting );
				#endif
				OUT.lighting = vertexLighting ;
				
				return OUT;
			}
			
			fixed4 pShader(vert2Pixel IN): COLOR
			{
				half2 normalUVs = TRANSFORM_TEX(IN.uvs.xy, _normalMap);
				fixed4 normalD = tex2D(_normalMap, normalUVs);
				normalD.xyz = (normalD.xyz * 2) - 1;
			
				//half3 normalDir = half3(2.0 * normalSample.xy - float2(1.0), 0.0);
				//deriving the z component
				//normalDir.z = sqrt(1.0 - dot(normalDir, normalDir));
               // alternatively you can approximate deriving the z component without sqrt like so:  
				//normalDir.z = 1.0 - 0.5 * dot(normalDir, normalDir);
				fixed3 normalDir = normalD.xyz;	
				fixed specMap = normalD.w;
				normalDir = normalize((normalDir.x * IN.tangentDir) + (normalDir.y * IN.binormalDir) + (normalDir.z * IN.normalDir));
				
				fixed3 ambientL = UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				half3 pixelToLightSource =_WorldSpaceLightPos0.xyz - (IN.posWorld*_WorldSpaceLightPos0.w);
				fixed attenuation  = lerp(1.0, 1.0/ length(pixelToLightSource), _WorldSpaceLightPos0.w);				
				fixed3 lightDirection = normalize(pixelToLightSource);
				
				fixed diffuseL =  lambert(normalDir, lightDirection);
				
				fixed3 diffuse = _LightColor0.xyz * diffuseL* attenuation;
				diffuse = saturate(IN.lighting + ambientL + diffuse);
		
				//specular highlight - use the spec map as the shift texture
				fixed2 specularHighlight =kajiyakay(IN.viewDir, IN.tangentDir, IN.binormalDir, normalDir, lightDirection, specMap);
	
				
				fixed4 outColor;							
				half2 diffuseUVs = TRANSFORM_TEX(IN.uvs.xy, _diffuseMap);
				fixed4 texSample = tex2D(_diffuseMap, diffuseUVs);
				fixed3 diffuseC =  texSample.xyz * _diffuseColor.xyz;
				fixed3 specular = (specularHighlight.x * _specularColor) + (specularHighlight.y * specMap * _specularColor2);
				outColor = fixed4( diffuse * (specular + diffuseC),texSample.w);
				outColor.xyz *= tex2D(_AmbientMap, IN.uvs.zw).x;
				return outColor;
			}
			
			ENDCG
		}	
		
		
		
	}
}