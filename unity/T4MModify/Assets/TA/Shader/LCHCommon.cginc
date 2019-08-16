#ifndef LCH_COMMON_CGINC
#define LCH_COMMON_CGINC

#include "UnityCG.cginc"
//[Toggle(MyToggle2)] _MyToggle2("MyToggle2", Float) = 0
//[KeywordEnum(One, Two, Three)] _MyEnum("MyEnum", Float) = 0
//#pragma shader_feature MyToggle2
//#pragma multi_compile _MYENUM_ONE _MYENUM_TWO _MYENUM_THREE

//

//half3 viewDir = normalize(UnityWorldSpaceViewDir(i.posWorld));
//half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
//UnityObjectToClipPos
//half3 viewReflectDirection = reflect(-viewDir, normal);
struct WPAttribute
{
	float4 wpos;//worldPos.
	float4 pos;//projectPos
};

WPAttribute ToProjectPos(float4 pos)
{
	WPAttribute output = (WPAttribute)0;
	output.wpos = mul(unity_ObjectToWorld, pos);
	output.pos = mul(UNITY_MATRIX_VP, output.wpos);
	return output;
}

struct WVPAttribute
{
	float4 wpos;//worldPos.
	float4 vpos;//viewPos;
	float4 pos;//projectPos
};

WVPAttribute ToProjectPosFull(float4 pos)
{
	WVPAttribute output = (WVPAttribute)0;
	output.wpos = mul(unity_ObjectToWorld, pos);
	output.vpos = mul(UNITY_MATRIX_V, output.wpos);
	output.pos = mul(UNITY_MATRIX_P, output.vpos);
	return output;
}


struct NTBYAttribute
{
	float3 normal  ;
	float3 tangent  ;
	float3 bitangent  ;
};


NTBYAttribute GetWorldNormalTangentBitangent(in float3 normal ,in float4 tangent)
{
	NTBYAttribute o = (NTBYAttribute)0;
	o.normal = UnityObjectToWorldNormal(normal);
	o.tangent = normalize(mul(unity_ObjectToWorld, float4(tangent.xyz, 0.0)).xyz);
	half tangentSign = tangent.w * unity_WorldTransformParams.w;
	o.bitangent = normalize(cross(o.normal, o.tangent) * tangentSign);
	return o; 
}



float3x3  GetNormalTranform(in float3 wNormalDir, in float3 wTangentDir,in float3 wBitangentDir)
{
	return   float3x3(wTangentDir, wBitangentDir, wNormalDir);;
}


#define NORMAL_TANGENT_BITANGENT_COORDS(idx1,idx2,idx3) \
half3 normal : TEXCOORD##idx1;\
half3 tangent : TEXCOORD##idx2;\
half3 bitangent : TEXCOORD##idx3;
 
 
//half3 _normal_val = UnpackNormalRG(e);
//float3x3 tangentTransform = GetNormalTranform(i.normal, i.tangent, i.bitangent);
//half3 normal = normalize(mul(_normal_val, tangentTransform));
inline fixed3 UnpackNormalRG(fixed4 packednormal)
{
	fixed3 normal;
	normal.xy = packednormal.xy * 2 - 1;
	normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
	return normal;
}

void Billboard(inout float4 vertex, inout float3 normal)
{
	float3 upCamVec = normalize(UNITY_MATRIX_V._m10_m11_m12);
	float3 forwardCamVec = -normalize(UNITY_MATRIX_V._m20_m21_m22);
	float3 rightCamVec = normalize(UNITY_MATRIX_V._m00_m01_m02);
	float4x4 rotationCamMatrix = float4x4(rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1);
	normal = normalize(mul(normal, rotationCamMatrix));
	//This unfortunately must be made to take non-uniform scaling into account;
	//Transform to world coords, apply rotation and transform back to local;
	vertex = mul(vertex, unity_ObjectToWorld);
	vertex = mul(vertex, rotationCamMatrix);
	vertex = mul(vertex, unity_WorldToObject);

}
void BillboardY(inout float4 vertex, inout float3 normal)
{
	//float3 upCamVec = normalize ( UNITY_MATRIX_V._m10_m11_m12 );
	float3 upCamVec = float3(0, 1, 0);
	float3 forwardCamVec = -normalize(UNITY_MATRIX_V._m20_m21_m22);
	float3 rightCamVec = normalize(UNITY_MATRIX_V._m00_m01_m02);
	float4x4 rotationCamMatrix = float4x4(rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1);
	normal = normalize(mul(normal, rotationCamMatrix));
	//This unfortunately must be made to take non-uniform scaling into account;
	//Transform to world coords, apply rotation and transform back to local;
	vertex = mul(vertex, unity_ObjectToWorld);
	vertex = mul(vertex, rotationCamMatrix);
	vertex = mul(vertex, unity_WorldToObject);


}

void BillboardY(inout float4 vertex )
{
	float3 upCamVec = float3(0, 1, 0);
	float3 forwardCamVec = -normalize(UNITY_MATRIX_V._m20_m21_m22);
	float3 rightCamVec = normalize(UNITY_MATRIX_V._m00_m01_m02);
	float4x4 rotationCamMatrix = float4x4(rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1);
 
	//This unfortunately must be made to take non-uniform scaling into account;
	//Transform to world coords, apply rotation and transform back to local;
	vertex = mul(vertex, unity_ObjectToWorld);
	vertex = mul(vertex, rotationCamMatrix);
	vertex = mul(vertex, unity_WorldToObject);
 
}

 
#endif