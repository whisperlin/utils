// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Light Test" {
	Properties {
		_Color("Color", color) = (1.0,1.0,1.0,1.0)
	}
		SubShader {
		Tags { "RenderType" = "Opaque" }

		Pass {
		Tags { "LightMode" = "ForwardBase" }	// pass for 4 vertex lights, ambient light & first pixel light (directional light)

		CGPROGRAM
		// Apparently need to add this declaration 
#pragma multi_compile_fwdbase	

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

		uniform float4 _Color;

	struct vertexInput
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	struct vertexOutput
	{
		float4 pos : SV_POSITION;
		float4 posWorld : TEXCOORD0;
		float3 normalDir : TEXCOORD1;
		float3 lightDir : TEXCOORD2;
		float3 viewDir : TEXCOORD3;
		float3 vertexLighting : TEXCOORD4;
		LIGHTING_COORDS(5, 6)
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		output.pos = UnityObjectToClipPos(input.vertex);
		output.posWorld = mul(unity_ObjectToWorld, input.vertex);
		output.normalDir = normalize(mul(float4(input.normal, 0.0), unity_WorldToObject).xyz);
		output.lightDir = WorldSpaceLightDir(input.vertex);
		output.viewDir = WorldSpaceViewDir(input.vertex);
		output.vertexLighting = float3(0.0, 0.0, 0.0);

		// SH/ambient and vertex lights
#ifdef LIGHTMAP_OFF
		float3 shLight = ShadeSH9(float4(output.normalDir, 1.0));
		output.vertexLighting = shLight;
#ifdef VERTEXLIGHT_ON
		float3 vertexLight = Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, output.posWorld, output.normalDir);
		output.vertexLighting += vertexLight;
#endif // VERTEXLIGHT_ON
#endif // LIGHTMAP_OFF

		// pass lighting information to pixel shader
		TRANSFER_VERTEX_TO_FRAGMENT(output);

		return output;
	}

	float4 frag(vertexOutput input):COLOR {
		float3 normalDirection = normalize(input.normalDir);
		float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
		float3 lightDirection;
		float attenuation;

		if (0.0 == _WorldSpaceLightPos0.w) // directional light?
		{
			attenuation = 1.0; // no attenuation
			lightDirection = normalize(_WorldSpaceLightPos0.xyz);
		}
		else // point or spot light
		{
			float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
			float distance = length(vertexToLightSource);
			attenuation = 1.0 / distance; // linear attenuation 
			lightDirection = normalize(vertexToLightSource);
		}

		 

		// Because SH lights contain ambient, we don't need to add it to the final result
		float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.xyz;

		float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb * max(0.0, dot(normalDirection, lightDirection)) * 2;

		float3 specularReflection;
		if (dot(normalDirection, lightDirection) < 0.0)  // light source on the wrong side?
		{
			specularReflection = float3(0.0, 0.0, 0.0);  // no specular reflection
		}
		else // light source on the right side
		{
			specularReflection = attenuation * _LightColor0.rgb * _Color.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), 255);
		}

		return float4(input.vertexLighting + diffuseReflection + specularReflection, 1.0);
	}
		ENDCG
	}

		Pass {
		Tags { "LightMode" = "ForwardAdd" }		// pass for additional light sources
		ZWrite Off Blend One One Fog { Color(0,0,0,0) }	// additive blending

		CGPROGRAM
		// Apparently need to add this declaration
#pragma multi_compile_fwdadd

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

		uniform float4 _Color;

	struct vertexInput
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	struct vertexOutput
	{
		float4 pos : SV_POSITION;
		float4 posWorld : TEXCOORD0;
		float3 normalDir : TEXCOORD1;
		float3 lightDir : TEXCOORD2;
		float3 viewDir : TEXCOORD3;
		LIGHTING_COORDS(4, 5)
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		output.pos = UnityObjectToClipPos(input.vertex);
		output.posWorld = mul(unity_ObjectToWorld, input.vertex);
		output.normalDir = normalize(mul(float4(input.normal, 0.0), unity_WorldToObject).xyz);
		output.lightDir = WorldSpaceLightDir(input.vertex);
		output.viewDir = WorldSpaceViewDir(input.vertex);

		// pass lighting information to pixel shader
		vertexInput v = input;
		TRANSFER_VERTEX_TO_FRAGMENT(output);

		return output;
	}

	float4 frag(vertexOutput input):COLOR {
		float3 normalDirection = normalize(input.normalDir);
		float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
		float3 lightDirection;
		float attenuation;

		if (0.0 == _WorldSpaceLightPos0.w) // directional light?
		{
			attenuation = 1.0; // no attenuation
			lightDirection = normalize(_WorldSpaceLightPos0.xyz);
		}
		else // point or spot light
		{
			float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
			float distance = length(vertexToLightSource);
			attenuation = 1.0 / distance; // linear attenuation 
			lightDirection = normalize(vertexToLightSource);
		}

		// LIGHT_ATTENUATION not only compute attenuation, but also shadow infos
		//                attenuation = LIGHT_ATTENUATION(input);
		// Compare to directions computed from vertex
		//				viewDirection = normalize(input.viewDir);
		//				lightDirection = normalize(input.lightDir);

		float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb * max(0.0, dot(normalDirection, lightDirection)) * 2;

		float3 specularReflection;
		if (dot(normalDirection, lightDirection) < 0.0)  // light source on the wrong side?
		{
			specularReflection = float3(0.0, 0.0, 0.0);  // no specular reflection
		}
		else // light source on the right side
		{
			specularReflection = attenuation * _LightColor0.rgb * _Color.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), 255);
		}

		return float4(diffuseReflection + specularReflection, 1.0);
	}
		ENDCG
	}
	}
		FallBack "Diffuse"
}
