// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "lin/MulLayersFurVL"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Specular("Specular Color", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8, 256)) = 8
		_SpecularStrength("Specular",Range(0,1)) = 0.5


		//v.color.a ("v.color.a",Range(0,1)) = 1
		_FurLength("Fur Length", Range(.0002, 1)) = .25

		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5 // how "thick"
		_CutoffEnd("Alpha Cutoff end", Range(0,1)) = 0.5 // how thick they are at the end
		_EdgeFade("Edge Fade", Range(0,1)) = 0.4

		_Gravity("Gravity Direction", Vector) = (0,0,1,0)
		_GravityStrength("_Gravity Strength", Range(0,0.2)) = 0.033
		_Speed("Gravity Speed", Range(0,10)) = 1
		_Ambient("Ambient",Color) = (0.5,0.5,0.5,1)	
		_MinMapPower("MinMap范围",Range(0.1,1))= 1

 
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		//Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
	{
		Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Lighting.cginc"
		//#include "UnityLightingCommon.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 color : COLOR;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float3 worldPos : TEXCOORD0;
		float3 worldNormal : TEXCOORD1;
		//float3 ambient : TEXCOORD2;
		float4 color: TEXCOORD2;
		float3 viewDirection: TEXCOORD3;
		float2 uv:TEXCOORD4;
		float4 diff : COLOR0; // diffuse lighting color
		
	};
	sampler2D _MainTex;
	float4 _MainTex_ST;
	sampler2D _Noise;
	float4 _Noise_ST;
 

	uniform float _Cutoff;
	uniform float _CutoffEnd;
	uniform float _EdgeFade;

	
	fixed4 _Color;
	fixed4 _Specular;
	float _Gloss;
	float _SpecularStrength;

	//uniform float v.color.a;

	uniform float _FurLength;

	uniform float3 _Gravity;
	uniform float _GravityStrength;
	uniform float _Speed;
 
	uniform float _MinMapPower;
	v2f vert(appdata v)
	{
		v2f o;
		float t0 =   sin(_Time.y*_Speed) ;
		float3 _scare = float3(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][2]) / unity_ObjectToWorld[3][3];
		fixed3 direction = lerp(v.normal, _Gravity   * t0 + v.normal  , v.color.a*_GravityStrength);
		direction = direction / _scare;
		v.vertex.xyz += direction * _FurLength * v.color.a /** v.color.a*/;
		o.vertex = UnityObjectToClipPos(v.vertex);
		float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
		o.worldNormal = worldNormal;
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);
		float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
		o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
		o.uv = v.uv;
		//o.ambient = ShadeSH9(half4(worldNormal,1)) *_Color;
		o.color = v.color;
		return o;
	}
	fixed3 _Ambient;
	float4 DirectionalLightColor;
	float4 DirectionalLightDir;
	float DirectionalLightIntensity;


	float4 PointLightColor;
	float4 PointLightPosition;
	float PointLightRange;
	float PointLightIntensity;

	float4 SpotlightColor;
	float SpotLightIntensity;
	float SpotlightSpotAngle0;

	float SpotlightSpotAngle1;
	float4 SpotDirection;
	float4 SpotLightPosition;
	float SpotLightRange;



	inline float3 PointLight(float3 normaldir, float3 p, float3 posWorld, float LightRange)
	{
		float3 lightDirection;

		float3 toLight = p - posWorld.xyz;
		float distance = length(toLight);

		float atten = 1.0 - distance*distance / (LightRange*LightRange);
		atten = max(0, atten);
		lightDirection = normalize(toLight);
		float3 finalPointlight = atten * saturate(dot(normaldir, lightDirection));
		return finalPointlight;
	}

	inline float3 DirectionalLight(float3 normaldir, fixed3 lightDir)
	{
		float nl = max(0, dot(normaldir, -lightDir));
		return    nl;
	}

	float smooth_step(float a, float b, float x)
	{
		float t = saturate((x - a) / (b - a));
		return t * t*(3.0 - (2.0*t));
	}
	inline float3 Spotlight(float3 normaldir, float3 p, float3 posWorld, float LightRange, float3 SpotDirection, float spotlightSpotAngle0, float spotlightSpotAngle1)
	{

		float3 lightDirection;

		float3 toLight = p - posWorld.xyz;
		float distance = length(toLight);

		float atten = 1.0 - distance*distance / (LightRange*LightRange);
		atten = max(0, atten);
		lightDirection = normalize(toLight);

		float rho = max(0, dot(lightDirection, SpotDirection));

		float spotAtt0 = smooth_step(spotlightSpotAngle0, SpotlightSpotAngle1, rho);

		atten *= spotAtt0;

		float diff = max(0, dot(normaldir, lightDirection));
		return (diff * atten);


	}
	fixed4 frag(v2f i): SV_Target
	{
 
		float2 _uv0 = TRANSFORM_TEX(i.uv, _MainTex);
		float2 _uv1 = TRANSFORM_TEX(i.uv, _Noise);
		fixed4 col = tex2D(_MainTex, _uv0 ,ddx(_uv0)*_MinMapPower,ddy(_uv0)*_MinMapPower);
		fixed4 _Noise_var = tex2D(_Noise, _uv1);
  
		float3 worldNormal = normalize(i.worldNormal);
		 


		float3 DirDiff = DirectionalLightColor.rgb*DirectionalLight(worldNormal, DirectionalLightDir)*DirectionalLightIntensity;
		float3 PointLightDiff = PointLightColor*PointLight(worldNormal, PointLightPosition, i.worldPos, PointLightRange)*PointLightIntensity ;
		float3 SportLightDiff = Spotlight(worldNormal, SpotLightPosition, i.worldPos, SpotLightRange, -SpotDirection, SpotlightSpotAngle0, SpotlightSpotAngle1)*SpotlightColor*SpotLightIntensity;


	 
		float _cull = lerp(_Cutoff, _CutoffEnd, i.color.a);
		 

		float3 c = (_Ambient + DirDiff + PointLightDiff+ SportLightDiff) *_Color.rgb*col.rgb;
		//(1-i.color.a) 
		clip(_Noise_var.r - _cull);
		 
		return float4(c, 1-i.color.a);
		//return fixed4(c, 1- i.color.a);
		 
	}
		ENDCG
	}
	}
}

 
