﻿#ifndef __VSM_CGINC__
#define  __VSM_CGINC__



#include "UnityCG.cginc"
uniform float4x4 _WorldToLight;


sampler2D _ShadowTex;
float4 _ShadowTex_ST;


uniform float _MaxDepthDelta;


float chebyshevUpperBound( float dis ,float2 uv)
{
	float4 c = tex2D(_ShadowTex,  uv);
	float2 moments = float2(DecodeFloatRG(c.rg),DecodeFloatRG(c.ba)   );;
	if (   moments.x-dis <=0.001)
		return 1.0 ;
	float variance = moments.y - (moments.x*moments.x);
	variance = max(variance, _MaxDepthDelta );
	float delta = dis - moments.x;
	float p_max = variance / (variance + delta*delta);
	return p_max;
} 

#endif