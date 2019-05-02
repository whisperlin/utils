//surface input and output
#ifndef MK_GLASS_SURFACE_IO
	#define MK_GLASS_SURFACE_IO

	/////////////////////////////////////////////////////////////////////////////////////////////
	// MKGLASS SURFACE
	/////////////////////////////////////////////////////////////////////////////////////////////

	//Dynamic precalc struct
	struct MKGlassPCP
	{
		#if SHADER_TARGET >= 25
			half3 HV;
			half VdotN;
			half NdotHV;
			half NdotL;
		#endif
		float4 UvRefraction;
		half3 NormalDirection;
		#if SHADER_TARGET >= 25
			half3 LightDirection;
			half3 LightColor;
			half3 LightColorXAttenuation;
			half LightAttenuation;
			half3 ViewDirection;
		#endif
	};

	//dynamic surface struct
	struct MKGlassSurface
	{
		MKGlassPCP Pcp;
		fixed4 Color_Out;
		fixed3 Color_Albedo;
		fixed3 Color_Refraction;
		fixed Alpha;
		#if SHADER_TARGET >= 25
			half3 Color_Emission;
			fixed3 Color_Specular;
		#endif
	};
#endif