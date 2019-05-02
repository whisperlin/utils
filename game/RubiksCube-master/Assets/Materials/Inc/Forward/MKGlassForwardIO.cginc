﻿//Vertexshader Input and Output
#ifndef MK_GLASS_IO_FORWARD
	#define MK_GLASS_IO_FORWARD

	#include "UnityCG.cginc"
	#include "AutoLight.cginc"

/////////////////////////////////////////////////////////////////////////////////////////////
// INPUT
/////////////////////////////////////////////////////////////////////////////////////////////
	struct VertexInputForward
	{
		//vertex position - always needed
		float4 vertex : POSITION;
		//texcoords0 
		float4 texcoord0 : TEXCOORD0;

		#if MK_GLASS_FWD_BASE_PASS
			//ambient and lightmap0 texcoords
			#if UNITY_SHOULD_SAMPLE_SH || LIGHTMAP_ON
				float4 texcoord1 : TEXCOORD1;
			#endif
			#if DYNAMICLIGHTMAP_ON
				//dynammic lightmap texcoords
				float4 texcoord2 : TEXCOORD2;
			#endif
		#endif
		//use normals light is enabled
		half3 normal : NORMAL;
		//use tangents for special matrix calculation
		half4 tangent : TANGENT;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	/////////////////////////////////////////////////////////////////////////////////////////////
	// OUTPUT
	/////////////////////////////////////////////////////////////////////////////////////////////
	struct VertexOutputForward
	{
		float4 pos : SV_POSITION;
		float4 uv_Main : TEXCOORD0;

		float3 posWorld : TEXCOORD1;

		float4 normalWorld : TEXCOORD2;
		float4 tangentWorld : TEXCOORD3;
		half3 binormalWorld : TEXCOORD4;
				
		#ifdef MK_GLASS_FWD_BASE_PASS
			fixed3 aLight : COLOR1;
			#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
				float4 uv_Lm : TEXCOORD5;
			#endif
		#endif

		#if UNITY_VERSION >= 201810
			UNITY_LIGHTING_COORDS(6,7)
		#else
			UNITY_SHADOW_COORDS(6)
		#endif
		#if SHADER_TARGET >= 30
    		UNITY_FOG_COORDS(8)
		#endif

		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};
#endif