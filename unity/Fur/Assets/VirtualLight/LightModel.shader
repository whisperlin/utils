Shader "Unlit/LightModel"
{
	Properties
	{
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
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float3 worldPos : TEXCOORD0;
			float3 worldNormal : TEXCOORD1;
			float3 ambient : TEXCOORD2;
			float4 diff : COLOR0; // diffuse lighting color
		};

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



			o.ambient = ShadeSH9(half4(worldNormal,1)) *_Diffuse;
			return o;
		}

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



		inline float3 PointLight(float3 normaldir, float3 p,float3 posWorld, float LightRange)
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

		inline float3 DirectionalLight(float3 normaldir, fixed3 lightDir )
		{
			float nl = max(0, dot(normaldir, -lightDir));
			return    nl ;
		}

		float smooth_step(float a, float b, float x)
		{
			float t = saturate((x - a) / (b - a));
			return t * t*(3.0 - (2.0*t));
		}
		inline float3 Spotlight(float3 normaldir, float3 p, float3 posWorld, float LightRange, float3 SpotDirection, float spotlightSpotAngle0, float spotlightSpotAngle1 )
		{

			float3 lightDirection;

			float3 toLight = p - posWorld.xyz;
			float distance = length(toLight);

			float atten = 1.0 - distance*distance / (LightRange*LightRange);
			atten = max(0, atten);
			lightDirection = normalize(toLight);

			float rho = max(0, dot(lightDirection, SpotDirection));
 
			float spotAtt0 =smooth_step( spotlightSpotAngle0, SpotlightSpotAngle1, rho);
 
			atten *= spotAtt0 ;
 
			float diff = max(0, dot(normaldir, lightDirection));
			return (diff * atten);


		}
		 


		 


		fixed4 frag(v2f i): SV_Target
		{
			fixed3 ambient = i.ambient;
			// specular
			float3 worldNormal = normalize(i.worldNormal);
 
			fixed3 lightDir = normalize(DirectionalLightDir);

		 
			float3 DirDiff = DirectionalLightColor.rgb*DirectionalLight(worldNormal, lightDir)*DirectionalLightIntensity*_Diffuse.rgb;
			float3 PointLightDiff = PointLightColor*PointLight(worldNormal,PointLightPosition,i.worldPos,PointLightRange)*PointLightIntensity *_Diffuse.rgb ;
			float3 SportLightDiff = Spotlight(worldNormal, SpotLightPosition, i.worldPos, SpotLightRange, -SpotDirection,  SpotlightSpotAngle0, SpotlightSpotAngle1)*SpotlightColor*SpotLightIntensity*_Diffuse.rgb ;
			
			//return float4(SportLightDiff, 1);
			float3 c = ambient + DirDiff + PointLightDiff +SportLightDiff;
	 
 
			return fixed4(c, 1);
		}
		ENDCG
	}
	}
}