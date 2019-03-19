
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
#ifndef ROLANTIN_CORE_INCLUDED
#define ROLANTIN_CORE_INCLUDED
#include "UnityCG.cginc"

////pc mode
UnityGI GetUnityGI(float3 lightColor, float3 lightDirection, float3 normalDirection, float3 viewDirection, float3 viewReflectDirection, float attenuation, float roughness, float3 worldPos) {
	UnityLight light;
	light.color = lightColor;
	light.dir = lightDirection;
	light.ndotl = max(0.0h, dot(normalDirection, lightDirection));
	UnityGIInput d;
	d.light = light;
	d.worldPos = worldPos;
	d.worldViewDir = viewDirection;
	d.atten = attenuation;
	d.ambient = 0.0h;
	d.boxMax[0] = unity_SpecCube0_BoxMax;
	d.boxMin[0] = unity_SpecCube0_BoxMin;
	d.probePosition[0] = unity_SpecCube0_ProbePosition;
	d.probeHDR[0] = unity_SpecCube0_HDR;
	d.boxMax[1] = unity_SpecCube1_BoxMax;
	d.boxMin[1] = unity_SpecCube1_BoxMin;
	d.probePosition[1] = unity_SpecCube1_ProbePosition;
	d.probeHDR[1] = unity_SpecCube1_HDR;
	Unity_GlossyEnvironmentData ugls_en_data;
	ugls_en_data.roughness = roughness;
	ugls_en_data.reflUVW = viewReflectDirection;
	UnityGI gi = UnityGlobalIllumination(d, 1.0h, normalDirection, ugls_en_data);
	return gi;
}

////shaderforge mode
UnityGI GIdata(float3 lightColor, float3 lightDirection, float3 normalDirection, float3 posWorld,
	float3 viewDirection, float attenuation, float4 ambientOrLightmapUV, float gloss, float3 viewReflectDirection) {
	UnityLight light;
#ifdef LIGHTMAP_OFF
	light.color = lightColor;
	light.dir = lightDirection;
	light.ndotl = LambertTerm(normalDirection, light.dir);
#else
	light.color = half3(0.f, 0.f, 0.f);
	light.ndotl = 0.0f;
	light.dir = half3(0.f, 0.f, 0.f);
#endif
	UnityGIInput d;
	d.light = light;
	d.worldPos = posWorld.xyz;
	d.worldViewDir = viewDirection;
	d.atten = attenuation;
#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
	d.ambient = 0;
	d.lightmapUV = ambientOrLightmapUV;
#else
	d.ambient = ambientOrLightmapUV;
#endif
#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
	d.boxMin[0] = unity_SpecCube0_BoxMin;
	d.boxMin[1] = unity_SpecCube1_BoxMin;
#endif
#if UNITY_SPECCUBE_BOX_PROJECTION
	d.boxMax[0] = unity_SpecCube0_BoxMax;
	d.boxMax[1] = unity_SpecCube1_BoxMax;
	d.probePosition[0] = unity_SpecCube0_ProbePosition;
	d.probePosition[1] = unity_SpecCube1_ProbePosition;
#endif
	d.probeHDR[0] = unity_SpecCube0_HDR;
	d.probeHDR[1] = unity_SpecCube1_HDR;
	Unity_GlossyEnvironmentData ugls_en_data;
	ugls_en_data.roughness = 1.0 - gloss;
	ugls_en_data.reflUVW = viewReflectDirection;
	UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data);
	lightDirection = gi.light.dir;
	lightColor = gi.light.color;
	return gi;
}


#endif


