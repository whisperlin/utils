//include file for important calculations during rendering
#ifndef MK_GLASS_INC
	#define MK_GLASS_INC

	#include "../Common/MKGlassDef.cginc"

	/////////////////////////////////////////////////////////////////////////////////////////////
	// INC
	/////////////////////////////////////////////////////////////////////////////////////////////
	//normal in world space
	inline half3 WorldNormal(half3 encodedNormal, half3x3 tbn)
	{
		//local.z = sqrt(1.0 - dot(local, local));
		#if !defined(UNITY_NO_DXT5nm)
			encodedNormal.z = 1.0 - 0.5 * dot(encodedNormal.xy, encodedNormal.xy); //approximation
		#endif
		return normalize(mul(encodedNormal, tbn));
	}

	inline half3 EncodeNormalMap(sampler2D normalMap, float2 uv)
	{
		half4 encode = tex2D(normalMap, uv);
		#if defined(UNITY_NO_DXT5nm)
			return encode.rgb * 2.0 - 1.0;
		#else
			return half3(2.0 * encode.a - 1.0, 2.0 * encode.g - 1.0, 0.0);
		#endif
	}

	//specular blinn phong
	inline half GetSpecular(half ndhv, half shine)
	{
		//doublesided spec
		return pow(ndhv, shine * SHINE_MULT);
	}

	//Rim with smooth interpolation
	inline half3 RimDefault(half size, half3 vdn, fixed3 col, fixed intensity)
	{
		fixed r = pow ((1.0 - saturate(vdn)), size);
		return r * intensity * col.rgb;
	}
#endif