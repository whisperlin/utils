// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "lin/MulLayersFur"
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
 
	}
		SubShader
	{
		Tags { "Queue" = "Transparent"   "IgnoreProjector" = "True" }
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
		float3 ambient : TEXCOORD2;
		float4 color: TEXCOORD3;
		float3 viewDirection: TEXCOORD4;
		float2 uv:TEXCOORD5;
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
		o.ambient = ShadeSH9(half4(worldNormal,1)) *_Color;
		o.color = v.color;
		return o;
	}

	fixed4 frag(v2f i): SV_Target
	{
		
		fixed4 col = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
		fixed4 _Noise_var = tex2D(_Noise, TRANSFORM_TEX(i.uv, _Noise));
		fixed3 ambient = i.ambient*col;
		// specular
		float3 worldNormal = normalize(i.worldNormal);
		//float3 lightDir = UnityWorldSpaceLightDir(i.worldPos);
		fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

		float3 viewDir = UnityWorldSpaceViewDir(i.worldPos);
		viewDir = normalize(viewDir);
		float3 halfDir = normalize(lightDir + viewDir);

		float nl = max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));
		float d = max(0, dot(halfDir, worldNormal));
		float3 spec = _LightColor0.rgb * _Specular.rgb * pow(d, _Gloss) * _SpecularStrength;
 
		float3 diff = _LightColor0.rgb * _Color.rgb*col.rgb * nl;


	 
		float _cull = lerp(_Cutoff, _CutoffEnd, i.color.a);
		//o.Alpha = step(_Cutoff, c.a);
		//float alpha0  = step(lerp(_Cutoff, _CutoffEnd, i.color.a), col.a);

		//float alpha = 1 - (i.color.a * i.color.a);
		//alpha += dot(viewDir, worldNormal) - _EdgeFade;

		//alpha0 *= alpha;


		float3 c = ambient + spec + diff;
		//(1-i.color.a) 
		clip(_Noise_var.r - _cull);
		 
		return float4(c, 1-i.color.a);
		//return fixed4(c, 1- i.color.a);
		 
	}
		ENDCG
	}
	}
}

 
