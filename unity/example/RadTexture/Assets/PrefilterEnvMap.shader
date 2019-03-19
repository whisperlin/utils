
Shader "Unlit/PrefilterEnvMap"
{
	Properties
	{

	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}
	#define EPSILON_COEF  1e-4
	#define M_PI   3.14159265
	#define PI   3.14159265
	#define  M_2PI   (2.0 * 3.14159265)
	#define M_INV_PI   0.31830988
	#define M_INV_LOG2   1.442695
	#define M_GOLDEN_RATIO  1.618034
	float RadicalInverse_VdC(uint bits)
	{
		bits = (bits << 16u) | (bits >> 16u);
		bits = ((bits & 0x55555555u) << 1u) | ((bits & 0xAAAAAAAAu) >> 1u);
		bits = ((bits & 0x33333333u) << 2u) | ((bits & 0xCCCCCCCCu) >> 2u);
		bits = ((bits & 0x0F0F0F0Fu) << 4u) | ((bits & 0xF0F0F0F0u) >> 4u);
		bits = ((bits & 0x00FF00FFu) << 8u) | ((bits & 0xFF00FF00u) >> 8u);
		return float(bits) * 2.3283064365386963e-10; // / 0x100000000
	}

	float2 Hammersley(int i, int spp)
	{
		return float2(float(i) / float(spp), RadicalInverse_VdC(i));
	}
	
	float G_Smith(float roughness, float NdotV, float NdotL)
	{
		float roughnessSqr = roughness*roughness;
		float NdotLSqr = NdotL*NdotL;
		float NdotVSqr = NdotV*NdotV;
		float SmithL = (2 * NdotL) / (NdotL + sqrt(roughnessSqr + (1 - roughnessSqr) * NdotLSqr));
		float SmithV = (2 * NdotV) / (NdotV + sqrt(roughnessSqr + (1 - roughnessSqr) * NdotVSqr));
		float Gs = (SmithL * SmithV);
		return Gs;
	}
	float3 ImportanceSampleGGX(float2 Xi, float Roughness, float3 N)
	{
		float a = Roughness * Roughness;
		float Phi = 2 * PI * Xi.x;
		float CosTheta = sqrt((1 - Xi.y) / (1 + (a*a - 1) * Xi.y));
		float SinTheta = sqrt(1 - CosTheta * CosTheta);
		float3 H;
		H.x = SinTheta * cos(Phi);
		H.y = SinTheta * sin(Phi);
		H.z = CosTheta;
		float3 UpVector = abs(N.z) < 0.999 ? float3(0, 0, 1) : float3(1, 0, 0);
		float3 TangentX = normalize(cross(UpVector, N));
		float3 TangentY = cross(N, TangentX);
		// Tangent to world space
		return TangentX * H.x + TangentY * H.y + N * H.z;
	}
	float2 IntegrateBRDF(float Roughness, float NoV)
	{
		float3 V;
		V.x = sqrt(1.0f - NoV * NoV); // sin
		V.y = 0;
		V.z = NoV; // cos
		float A = 0;
		float B = 0;
		float3 N = float3(0, 0, 1);
		const uint NumSamples = 1024;
		for (uint i = 0; i < NumSamples; i++)
		{
			float2 Xi = Hammersley(i, NumSamples);
			float3 H = ImportanceSampleGGX(Xi, Roughness, N);
			float3 L = 2 * dot(V, H) * H - V;
			float NoL = saturate(L.z);
			float NoH = saturate(H.z);
			float VoH = saturate(dot(V, H));
			if (NoL > 0)
			{
				float G = G_Smith(Roughness, NoV, NoL);
				float G_Vis = G * VoH / (NoH * NoV);
				float Fc = pow(1 - VoH, 5);
				A += (1 - Fc) * G_Vis;
				B += Fc * G_Vis;
			}
		}
		return float2(A, B) / NumSamples;
	}

	
	fixed4 frag(v2f i): SV_Target
	{
		float2 uv = i.uv;
		float roughness = uv.x;
		float NoV = uv.y;


		return float4(IntegrateBRDF(roughness, NoV),0,1);

	}
		ENDCG
	}
	}
}


