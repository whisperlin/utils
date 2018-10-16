#ifndef _________WORLD_DECAL________________
#define _________WORLD_DECAL________________
#include "UnityCG.cginc"
uniform float4x4 WorldToGlobalDecal;
sampler2D GlobalDecalTex;

float4 GetColorFromGlobalDecal(float4 worldPos)
{
	float2 decalUV =  (mul(WorldToGlobalDecal,worldPos)+float2(1,1))*0.5;
	return tex2D(GlobalDecalTex,decalUV);
}


#endif



