Shader "lin/PhoneFur"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Specular("Specular Color", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8, 256)) = 8
		_SpecularStrength("Specular",Range(0,1)) = 0.5


		FUR_MULTIPLIER ("FUR_MULTIPLIER",Range(0,1)) = 1
		_FurLength("Fur Length", Range(.0002, 1)) = .25

		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5 // how "thick"
		_CutoffEnd("Alpha Cutoff end", Range(0,1)) = 0.5 // how thick they are at the end
		_EdgeFade("Edge Fade", Range(0,1)) = 0.4

		_Gravity("Gravity Direction", Vector) = (0,0,1,0)
		_GravityStrength("Gravity Strength", Range(0,1)) = 0.25
	}
		SubShader
	{
		Tags { "Queue" = "Transparent"   "IgnoreProjector" = "True" }
		Blend SrcAlpha OneMinusSrcAlpha
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

	uniform float FUR_MULTIPLIER;

	uniform float _FurLength;

	uniform fixed3 _Gravity;
	uniform fixed _GravityStrength;
 
	v2f vert(appdata v)
	{
		v2f o;

		fixed3 direction = lerp(v.normal, _Gravity * _GravityStrength + v.normal * (1 - _GravityStrength), FUR_MULTIPLIER);
		v.vertex.xyz += direction * _FurLength * FUR_MULTIPLIER * v.color.a;

		o.vertex = UnityObjectToClipPos(v.vertex);
		float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
		o.worldNormal = worldNormal;
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);

		float4 posWorld = mul(unity_ObjectToWorld, v.vertex);

		o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
		o.uv = v.uv;
		o.ambient = ShadeSH9(half4(worldNormal,1)) *_Color;
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

		float3 c = ambient + spec + diff;


		float alpha0 = step(lerp(_Cutoff, _CutoffEnd, FUR_MULTIPLIER), _Noise_var.a);

		float alpha = 1 - (FUR_MULTIPLIER * FUR_MULTIPLIER);
		alpha += dot(i.viewDirection, worldNormal) - _EdgeFade;

		
		alpha0 *= alpha;
		 
		 
		return fixed4(c, alpha0);
	}
		ENDCG
	}
	}
}

 
