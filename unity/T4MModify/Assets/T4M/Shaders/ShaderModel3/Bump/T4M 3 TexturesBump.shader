Shader "T4MShaders/ShaderModel3/BumpSpec/T4M 3 Textures Bump Spec Water" {
Properties {
	_SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
	 
	_Splat0 ("Layer 1 (R)", 2D) = "white" {}
	_SpecColor0("��һ��߹�ɫ", Color) = (1, 1, 1, 1)
	_Splat1 ("Layer 2 (G)", 2D) = "white" {}
    _SpecColor1("�ڶ���߹�ɫ", Color) = (1, 1, 1, 1)
	_Splat2 ("Layer 3 (B)", 2D) = "white" {}
 
	_BumpSplat0 ("Layer1Normalmap", 2D) = "bump" {}
	_BumpSplat1 ("Layer2Normalmap", 2D) = "bump" {}
	_BumpSplat2 ("Layer3Normalmap", 2D) = "bump" {}
	_Control ("Control (RGBA)", 2D) = "white" {}
	//_MainTex ("Never Used", 2D) = "white" {}
	_TopColor("ǳˮɫ", Color) = (0.619, 0.759, 1, 1)
	_ButtonColor("��ˮɫ", Color) = (0.35, 0.35, 0.35, 1)
	_Gloss("ˮ�߹�����", Range(0,1)) = 0.5
	_WaveNormalPower("ˮ����ǿ��",Range(0,1)) = 1
	_WaveScale("ˮ��������", Range(0.02,0.15)) = .07
	_WaveSpeed("ˮ�����ٶ�", Vector) = (19,9,-16,-7)
	_SpecColor2("ˮ�߹�ɫ", Color) = (1, 1, 1, 1)
	[KeywordEnum(Off, On)] _IsMetallic("�Ƿ���������", Float) = 0
	metallic_power("���ǿ��", Range(0,1)) = 1
	metallic_color("�����ɫ", Color) = (1, 1, 1, 0)
	_Shininess("�Ĳ�߹����", Vector) = (0.078125,0.078125,0.078125,0.078125)
} 

SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert
#pragma target 3.0

#include "UnityCG.cginc"

struct Input {
	float3 worldPos;
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;
};

void vert (inout appdata_full v) {

	float3 T1 = float3(1, 0, 1);
	float3 Bi = cross(T1, v.normal);
	float3 newTangent = cross(v.normal, Bi);
	
	normalize(newTangent);

	v.tangent.xyz = newTangent.xyz;
	
	if (dot(cross(v.normal,newTangent),Bi) < 0)
		v.tangent.w = -1.0f;
	else
		v.tangent.w = 1.0f;
}

sampler2D _Control;
sampler2D _BumpSplat0, _BumpSplat1, _BumpSplat2;
sampler2D _Splat0,_Splat1,_Splat2;
 

uniform float _WaveNormalPower;
uniform float4 _WaveSpeed;
uniform float _WaveScale;
float metallic_power;
float3 metallic_color;
float _Gloss;
float4 _TopColor;
float4	_ButtonColor;
fixed4 _Shininess;


half4 _SpecColor0;
half4 _SpecColor1;
half4 _SpecColor2;

inline float2 ToRadialCoords(float3 coords)
{
	float3 normalizedCoords = normalize(coords);
	float latitude = acos(normalizedCoords.y);
	float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
	float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
	return float2(0.5, 1.0) - sphereCoords;
}
void surf (Input IN, inout SurfaceOutput o) {


	

	half4 splat_control = tex2D (_Control, IN.uv_Control);
	half3 col;
	half4 splat0 = tex2D (_Splat0, IN.uv_Splat0);
	half4 splat1 = tex2D (_Splat1, IN.uv_Splat1);
	

	 

	
	
	
	col  = splat_control.r * splat0.rgb;
	o.Normal = splat_control.r * UnpackNormal(tex2D(_BumpSplat0, IN.uv_Splat0));
	o.Gloss = splat0.a * splat_control.r ;
	o.Specular = _Shininess.r * splat_control.r;

	col += splat_control.g * splat1.rgb;
	o.Normal += splat_control.g * UnpackNormal(tex2D(_BumpSplat1, IN.uv_Splat1));
	o.Gloss += splat1.a * splat_control.g;
	o.Specular += _Shininess.g * splat_control.g;
	
	
	

	 

	half4 temp = IN.worldPos.xzxz * _WaveScale + _WaveSpeed * _WaveScale * _Time.y;
		temp.xy *= float2(.4, .45);

	half3 bump1 = UnpackNormal(tex2D(_BumpSplat2, temp.xy)).rgb;
	half3 bump2 = UnpackNormal(tex2D(_BumpSplat2, temp.zw)).rgb;
	half3 bump = (bump1 + bump2) * 0.5;
	half3 baseNormal = half3(0, 0, 1);
	
	half3 waterNormal = o.Normal + splat_control.b * lerp(baseNormal,bump ,  _WaveNormalPower)  ;
	float inWater = splat_control.b;
	metallic_power = metallic_power*inWater;
	//o.Normal += splat_control.a * baseNormal;
	o.Normal = waterNormal;
	 
	fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(IN.worldPos));
	half3 viewReflectDirection = reflect(-worldViewDir, waterNormal);
	half2 skyUV = half2(ToRadialCoords(viewReflectDirection) );
	fixed4 localskyColor = tex2D(_Splat2, skyUV);

	float3 waterColor = lerp(_TopColor,_ButtonColor, splat_control.b);
	//half4 splat3 = tex2D (_Splat3, IN.uv_Splat3);
	col += splat_control.b * (1-metallic_power)*waterColor;
	//col += splat_control.a * localskyColor.rgb;
	_SpecColor =  _SpecColor0*splat_control.r + _SpecColor1*splat_control.g + _SpecColor2*splat_control.b  ;

	o.Gloss +=   _Gloss*splat_control.b;
	o.Specular += _Shininess.a * splat_control.b;
	o.Emission =  localskyColor.rgb * metallic_power  * splat_control.b;
	o.Albedo = col;
	o.Alpha = 0.0;
}
ENDCG  
}
FallBack "Specular"
}