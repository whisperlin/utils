Shader "lin/Specular Blinn-Phong P"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_Diffuse("Diffuse Color", Color) = (1,1,1,1)
		_Specular("Specular Color", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8, 256)) = 8
		_SpecularStrength("Specular",Range(0,1)) = 0.5
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
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
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float3 worldPos : TEXCOORD0;
		float3 worldNormal : TEXCOORD1;
		float3 ambient : TEXCOORD2;
		float4 diff : COLOR0; // diffuse lighting color
		float2 uv:TEXCOORD3;
	};
	sampler2D _MainTex;
	float4 _MainTex_ST;
	fixed4 _Diffuse;
	fixed4 _Specular;
	float _Gloss;
	float _SpecularStrength;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
		o.worldNormal = worldNormal;
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);


		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.ambient = ShadeSH9(half4(worldNormal,1)) *_Diffuse;
		return o;
	}

	fixed4 frag(v2f i): SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv);

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

	float3 diff = _LightColor0.rgb * _Diffuse.rgb*col.rgb * nl;

	float3 c = ambient + spec + diff;

	return fixed4(c, 1);
	}
		ENDCG
	}
	}
}
