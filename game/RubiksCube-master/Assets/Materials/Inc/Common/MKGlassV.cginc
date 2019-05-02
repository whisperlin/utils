//uniform variables
#ifndef MK_GLASS_V
	#define MK_GLASS_V

	/////////////////////////////////////////////////////////////////////////////////////////////
	// UNIFORM VARIABLES
	/////////////////////////////////////////////////////////////////////////////////////////////

	//enabled uniform variables only if needed

	//UNITY_INSTANCING_CBUFFER_START(MK_GLASS_PROPERTIES)
          
    //UNITY_INSTANCING_CBUFFER_END

	//Main
	#ifndef UNITY_STANDARD_INPUT_INCLUDED
		uniform fixed3 _Color;
	#endif
	#ifndef UNITY_STANDARD_INPUT_INCLUDED
		uniform sampler2D _MainTex;
	#endif
	uniform half _MainTint;
	#ifndef UNITY_STANDARD_INPUT_INCLUDED
		uniform float4 _MainTex_ST;
	#endif

	//Normalmap
	uniform sampler2D _BumpMap;

	uniform half _Distortion;

	//Light
	#ifndef UNITY_LIGHTING_COMMON_INCLUDED
		uniform fixed4 _LightColor0;
	#endif

	#if MK_GLASS_META_PASS || SHADER_TARGET >= 25
		//Emission
		uniform fixed3 _EmissionColor;
	#endif

	#if SHADER_TARGET >= 25
		//Specular
		uniform half _Shininess;
		#ifndef UNITY_LIGHTING_COMMON_INCLUDED
			uniform fixed3 _SpecColor;
		#endif
		uniform fixed _SpecularIntensity;

		//Rim
		uniform fixed3 _RimColor;
		uniform half _RimSize;
		uniform fixed _RimIntensity;
	#endif

	//Other
	uniform sampler2D _GrabTexture;
	uniform half4 _GrabTexture_TexelSize;
#endif