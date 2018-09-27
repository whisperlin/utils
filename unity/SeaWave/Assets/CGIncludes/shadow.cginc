#ifndef SHADOW_CGNINC
#define SHADOW_CGNINC

uniform float4x4 _CameraMatrix;
uniform float4x4 _WorldToCameraMatrix;

uniform sampler2D _ShadowTexture;
uniform float4 CameraPlane;
uniform float4 LightDirection;

uniform float4 BiasParameters;

#define div_value  1

#ifndef AlphaBlendOn

#ifdef Ground
#define FINAL_SHADOW_COLOR(col, Depth, absZ, NdotL) col * (1 - step(Depth, absZ) * 0.5)
#else
#define FINAL_SHADOW_COLOR(col, Depth, absZ, NdotL) col * (NdotL > 0 ? (1 - step(Depth, absZ) * 0.5) : 1)
#endif

#else

#ifdef Ground
#define FINAL_SHADOW_COLOR(col, Depth, absZ, NdotL)\
	float4((col * (1 - step(Depth, absZ) * 0.5)).xyz, UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _FakeValue).a);
#else
#define FINAL_SHADOW_COLOR(col, Depth, absZ, NdotL)\
	float4((col * (NdotL > 0 ? (1 - step(Depth, absZ) * 0.5) : 1)).xyz, UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _FakeValue).a);
#endif

#endif

//float4((col * (1 - step(Depth, absZ) * 0.5)).xyz, UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _FakeValue).a);

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
	return (pos.z / pos.w);// *0.5 + 0.5;
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
	

	return (float4(uvGrab.x, uvGrab.y, 1, 1) + 1) * 0.5;
	/*
	float4 pos = float4(uvGrab.x + uvGrab.w, uvGrab.y + uvGrab.w, 1, 1) * 0.5;
	//#endif
	pos.zw = uvGrab.zw;
	return pos;*/
}

#ifdef UseUnityShadow
float4 FINAL_SHADOW_COLOR_SINGLE(float3 col, VertexOutput i, float3 normal)
{
	float NdotL = dot(DirectionLightDir0, normal);
	
	return float4(col * (NdotL > 0 ? LIGHT_ATTENUATION(i) : 1), 1);
}

#else
float4 FINAL_SHADOW_COLOR_SINGLE(float3 col, VertexOutput i, float3 normal)
{
	float4 worldPos_2_Camera = i.worldPos_2_Camera;
	if (worldPos_2_Camera.x > 1 || worldPos_2_Camera.y > 1 || worldPos_2_Camera.x < 0 || worldPos_2_Camera.y < 0)
		return float4(col,1);

	float NdotL = dot(DirectionLightDir0, normal);
	float Depth = GetDepth(worldPos_2_Camera);
	float absZ = GetLightCameraDepth(i.viewPos);
	return FINAL_SHADOW_COLOR(float4(col, 1), Depth, absZ, NdotL);
}
#endif

#endif