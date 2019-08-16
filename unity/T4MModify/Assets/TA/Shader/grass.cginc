// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

#ifndef ________GRASS_____________
#define  ________GRASS_____________

#include "LCHCommon.cginc"
#include "height-fog.cginc"
struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
	float2 uv2 : TEXCOORD1;
#else
	float3 normal : NORMAL;
#endif
	float4 color: COLOR;


	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
	UNITY_VERTEX_INPUT_INSTANCE_ID

	float2 uv : TEXCOORD0;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
	float2 uv2 : TEXCOORD1;
#else

#endif
	float3 normalWorld : TEXCOORD5;
	float4 color: TEXCOORD2;
	float4 wpos:TEXCOORD3;
	UNITY_FOG_COORDS_EX(4)
	float4 pos : SV_POSITION;

	//float4 dis : TEXCOORD10;
};

sampler2D _MainTex;
sampler2D _EmissionTex;
half _AlphaCut;
half4 _Wind;
half _Speed;
half _Ctrl;
half4 _Color;
half _Emission;

#if _FADEPHYSICS_ON
float4 _HitData0;
void isHit(float4 hitData, float4 worldPos , out half4 normalDir, out half result)
{
	normalDir.xyz = worldPos.xyz - hitData.xyz;
	//ÀÆ∆Ωæ‡¿Î.
	normalDir.w = length(normalDir.xz);
	normalDir.xz = normalDir.xz* (hitData.w - normalDir.w) / hitData.w;
	result = step(normalDir.w, hitData.w)*step(hitData.y - hitData.w, worldPos.y);
}
#endif


 

UNITY_INSTANCING_CBUFFER_START(GrassProperties)
UNITY_DEFINE_INSTANCED_PROP(float4, instance_world_position)
UNITY_INSTANCING_CBUFFER_END

void grass_move(inout float4 vertex ,in float4 color)
{
#if _FADEPHYSICS_ON
	half4 normalDir0;
	half s0;

	isHit(_HitData0, UNITY_ACCESS_INSTANCED_PROP(instance_world_position), normalDir0, s0);



	float s = sin(_Time.y*_Speed + (vertex.x + vertex.z) *_Ctrl) * (1 - s0);
	vertex.xyz = vertex.xyz + float3(_Wind.x, 0, _Wind.y)  * color.g * s;
	vertex.xz = vertex.xz + s0 * color.g * 5
		* normalDir0.rb;

	//o. dis = normalDir0.w;
#else
	float s = sin(_Time.y*_Speed + (vertex.x + vertex.z) *_Ctrl);
	vertex.xyz = vertex.xyz + float3(_Wind.x, 0, _Wind.y)  * color.g * s;
#endif

}
v2f vert(appdata v)
{
	

	v2f o;

	UNITY_INITIALIZE_OUTPUT(v2f, o);
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);

#if _ENABLE_BILLBOARD_Y

#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
	BillboardY(v.vertex);
#else
	BillboardY(v.vertex, v.normal);

#endif


#endif
	//_Wind
	o.wpos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
	o.pos = o.wpos;

 
	grass_move(o.pos,v.color);


	o.pos = mul(UNITY_MATRIX_VP, o.pos);
	o.uv = v.uv;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
	o.uv2 = v.uv2 * unity_LightmapST.xy + unity_LightmapST.zw;


#endif
	o.normalWorld = UnityObjectToWorldNormal(v.normal);
	o.color = v.color;
 

	UNITY_TRANSFER_FOG_EX(o, o.pos, o.wpos, o.normalWorld);

	
	return o;
}


#endif