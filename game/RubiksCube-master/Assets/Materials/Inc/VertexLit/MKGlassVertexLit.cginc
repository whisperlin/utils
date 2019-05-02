// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

//vertex and fragment shader for vertexlit passes
#ifndef MK_GLASS_VertexLit
	#define MK_GLASS_VertexLit

	/////////////////////////////////////////////////////////////////////////////////////////////
	// VERTEX SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	VertexOutputVertexLit vertvl (VertexInputVertexLit v)
	{
		VertexOutputVertexLit o;
		UNITY_INITIALIZE_OUTPUT(VertexOutputVertexLit, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.pos = UnityObjectToClipPos (v.vertex);

		o.uv_Main.xy = TRANSFORM_TEX (v.texcoord, _MainTex);

		#if _MK_VERTEXLMM_LIT || _MK_VERTEXLMRGBM_LIT
			o.uv_Lm.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		#endif

		#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
		#else
		float scale = 1.0;
		#endif

		o.uv_Refraction.xy = (float2(o.pos.x, o.pos.y*scale) + o.pos.w) * 0.5;
		#ifdef UNITY_SINGLE_PASS_STEREO
			o.uv_Refraction.xy = TransformStereoScreenSpaceTex(o.uv_Refraction.xy, o.pos.w);
		#endif
		o.uv_Refraction.zw = o.pos.zw;
		return o;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////
	// FRAGMENT SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	fixed4 fragvl (VertexOutputVertexLit o) : SV_Target
	{
		half zF;
		#ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE
			zF = UNITY_Z_0_FAR_FROM_CLIPSPACE(o.uv_Refraction.z);
		#else
			zF = o.uv_Refraction.z;
		#endif
		half4 texelSize;
				
		/*
		#if FAST_MODE	
			texelSize = _GrabTexture_TexelSize;
		#else
			texelSize = _GrabTexture_TexelSize;
		#endif
		*/
				
		texelSize.xyzw = 0.00125;

		half3 encodedNormalMap = EncodeNormalMap(_BumpMap, o.uv_Main);
		half2 offsetN = SHINE_MULTXX * encodedNormalMap.xy * _Distortion * texelSize.xy;
		o.uv_Refraction.xy = offsetN * zF + o.uv_Refraction.xy;

		fixed3 refraction = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(o.uv_Refraction)).rgb;

		fixed4 c = fixed4(0,0,0,0);
		c = fixed4(tex2D(_MainTex, o.uv_Main).rgb * _Color, 1);

		c.rgb = lerp(refraction, c*refraction, _MainTint);

		#if _MK_VERTEXLMM_LIT || _MK_VERTEXLMRGBM_LIT
			fixed4 lmt = UNITY_SAMPLE_TEX2D(unity_Lightmap, o.uv_Lm.xy);
			fixed3 lm = (8.0 * lmt.a) * lmt.rgb;
			c.rgb += lm;
		#endif

		c.a = 0.625;

		return c;
	}
#endif