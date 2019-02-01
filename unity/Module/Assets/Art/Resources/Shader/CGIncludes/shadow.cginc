#ifndef SHADOW_CGNINC
#define SHADOW_CGNINC

uniform float4x4 _CameraMatrix;
uniform float4x4 _WorldToCameraMatrix;

uniform sampler2D _ShadowTexture;
uniform float4 CameraPlane;
uniform float4 LightDirection;
uniform float ShadowAlpha;
uniform float4 BiasParameters;
uniform float4 FogVolumeDistance;
uniform float3 FogVolumeColor;
uniform float FogVolumeBias;

#define div_value  1

//  step(absZ, Depth) 结果0为阴影，取1的补数乘阴影强度，阴影强度越大颜色值越低，所以再取补数


#define GET_CAMERA_POS(worldpos, viewpos, worldpos_2_camera)\
		worldpos.xyz = CalcBias(o.Normal, worldpos.xyz, BiasParameters);\
		viewpos = mul(_CameraMatrix, worldpos);\
		worldpos_2_camera = CalcShadowmapPos(viewpos);

#define GET_CAMERA_POS_NORMAL(worldpos, viewpos, worldpos_2_camera, Normal)\
		worldpos.xyz = CalcBias(Normal, worldpos.xyz, BiasParameters);\
		viewpos = mul(_CameraMatrix, worldpos);\
		worldpos_2_camera = CalcShadowmapPos(viewpos);

inline float GetDepth(float4 worldPos_2_Camera)
{
	//float4 worldPosUV = UNITY_PROJ_COORD(worldPos_2_Camera);
	//float4 codeDepth = tex2Dproj(_ShadowTexture, worldPosUV);
	float4 codeDepth = tex2D(_ShadowTexture, worldPos_2_Camera.xy);
	return DecodeFloatRGBA(codeDepth);
}

inline float GetLightCameraDepth(float4 pos)
{
	return (pos.z / pos.w) *0.5 + 0.5;
}

inline float3 CalcBias(float3 normal, float3 pos, float4 parameters)
{
	float NdotL = saturate(1 - dot(normal, LightDirection));
	float bias = NdotL * parameters.x + parameters.y;
	return pos + normal * bias;
}

inline float4 CalcShadowmapPos(float4 uvGrab)
{
	//float4 uvGrab = mul(_CameraMatrix, wpos);
	//#if UNITY_UV_STARTS_AT_TOP  // Direct3D类似平台scale为-1；OpenGL类似平台为1。
	//				o.worldPos_2_Camera = float4(uvGrab.x + uvGrab.w, -uvGrab.y + uvGrab.w, 1, 1) * 0.5;
	//#else
	

	return float4(uvGrab.x, uvGrab.y, 1, 1);
	/*
	float4 pos = float4(uvGrab.x + uvGrab.w, uvGrab.y + uvGrab.w, 1, 1) * 0.5;
	//#endif
	pos.zw = uvGrab.zw;
	return pos;*/
}

#ifndef UseUnityShadow
#define CalcuShadowAlpha(col, shadowAlpha, envirColor, i, a, shadow_pow)  float4(col * lerp(envirColor, EnvimentColor * shadowAlpha, (1 - step(absZ, Depth)) * shadow_pow), a)
#else
#define CalcuShadowAlpha(col, shadowAlpha, envirColor, i, a, shadow_pow)  float4(col * lerp(envirColor, EnvimentColor * shadowAlpha, 1 - LIGHT_ATTENUATION(i)), shadowAlpha) //lerp(envirColor, EnvimentColor * shadowAlpha, LIGHT_ATTENUATION(i))
#endif
/*#ifdef UseUnityShadow
float4 FINAL_SHADOW_COLOR_SINGLE(float3 col, VertexOutput i, float3 normal ) 
{
	float NdotL = dot(DirectionLightDir0, normal);
	return float4((col * (NdotL > 0 ? LIGHT_ATTENUATION(i) : 1)), ShadowAlpha);
}

#else*/

float sigmoid(float x)
{
	return 1 - 1 / (1 + exp(-(pow(x, 5) + 0.5)));
}

float4 FINAL_SHADOW_COLOR_SINGLE(float3 col, VertexOutput i, float3 normal) 
{
	float4 worldPos_2_Camera = (i.worldPos_2_Camera + 1) * 0.5;
	float2 worldPosPow = abs((worldPos_2_Camera.xy - 0.5) * 2);
	float NdotL = dot(DirectionLightDir0, normal);
	float3 envirColor = lerp(EnvimentColor, float3(1, 1, 1), saturate(NdotL));
	float3 ndotl_Color = col * envirColor;

#ifndef BAKEMOD_ON
	float distance = length(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
	float fogLerp = saturate(1.0 - ((i.posWorld.y - FogVolumeDistance.x) / FogVolumeDistance.y)) * saturate(pow(max(0, distance - FogVolumeBias) / FogVolumeDistance.z, FogVolumeDistance.w));
#else
	float fogLerp = 0;
#endif

#ifndef UseUnityShadow
	if (NdotL < 0)
		return float4(lerp(ndotl_Color, FogVolumeColor, fogLerp), 1);// float4(col, 1);
#endif

	float Depth = GetDepth(worldPos_2_Camera);
	float absZ = GetLightCameraDepth(i.viewPos);
	float Shadow_value = step(worldPosPow.y, 1) * step(worldPosPow.x, 1);//saturate(smoothstep(1.3,0,worldPosPow.y) * smoothstep(1.3, 0, worldPosPow.x)*5);

	//return float4(saturate(Shadow_value * 3), 0, 0, 1);

#ifndef AlphaBlendOn
	float4 final = CalcuShadowAlpha(col, ShadowAlpha, envirColor, i, 1, Shadow_value);// *step(1, worldPosPow.y) * step(1, worldPosPow.x); // FINAL_SHADOW_COLOR(float4(col, 1), Depth, absZ, NdotL, ShadowAlpha);
#else
	float4 final = CalcuShadowAlpha(col, ShadowAlpha, envirColor, i, UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _FakeValue).a, Shadow_value);
#endif

#ifdef BLEND_ONEONE
	return float4(lerp(final.rgb, FogVolumeColor, fogLerp) - FogVolumeColor * fogLerp, final.a);
#else
	return float4(lerp(final.rgb, FogVolumeColor, fogLerp), final.a);
#endif
}
//#endif

#endif