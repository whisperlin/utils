//surface calculations
#ifndef MK_GLASS_SURFACE
	#define MK_GLASS_SURFACE

	void PreCalcParameters(inout MKGlassSurface mkts)
	{
		#if SHADER_TARGET >= 25
			mkts.Pcp.NdotL = max(0.0 , dot(mkts.Pcp.NormalDirection, mkts.Pcp.LightDirection));
			mkts.Pcp.VdotN = max(0.0, dot(mkts.Pcp.ViewDirection, mkts.Pcp.NormalDirection));
			mkts.Pcp.HV = normalize(mkts.Pcp.LightDirection + mkts.Pcp.ViewDirection);
			mkts.Pcp.NdotHV = max(0.0, dot(mkts.Pcp.NormalDirection, mkts.Pcp.HV));
		#endif
	}

	//get surface refraction and mix with albedo
	inline void MixAlbedoRefraction(inout fixed3 a, in fixed3 r, half lv)
	{
		a = lerp(r, a*r, lv);
	}

	//get surface color based on blendmode and color source
	inline void SurfaceAlbedo(out fixed3 albedo, out fixed alpha, float2 uv)
	{
		fixed3 c = tex2D(_MainTex, uv).rgb * _Color;
		albedo = c.rgb;
		alpha = 1.0;
	}

	inline void SurfaceRefraction(out fixed3 refraction, float4 uv)
	{
		refraction = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(uv)).rgb;
	}

	//only include initsurface when not meta pass
	#ifndef MK_GLASS_META_PASS
		/////////////////////////////////////////////////////////////////////////////////////////////
		// INITIALIZE SURFACE
		/////////////////////////////////////////////////////////////////////////////////////////////
		MKGlassSurface InitSurface(
			#if defined(MK_GLASS_FWD_BASE_PASS) || defined(MK_GLASS_FWD_ADD_PASS)
				in VertexOutputForward o
			#endif
		)
		{
			//Init Surface
			MKGlassSurface mkts;
			UNITY_INITIALIZE_OUTPUT(MKGlassSurface,mkts);

			//get refraction coords
			mkts.Pcp.UvRefraction = float4(o.uv_Main.zw, o.normalWorld.w, o.tangentWorld.w);

			half zF;
			#ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE
				zF = UNITY_Z_0_FAR_FROM_CLIPSPACE(mkts.Pcp.UvRefraction.z);
			#else
				zF = mkts.Pcp.UvRefraction.z;
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

			//get normal direction
			//NormalMap extraction
			mkts.Pcp.NormalDirection = WorldNormal(encodedNormalMap, half3x3(o.tangentWorld.xyz, o.binormalWorld.xyz, o.normalWorld.xyz));

			half2 offsetN = SHINE_MULTXX * encodedNormalMap.xy * _Distortion * texelSize.xy;
			mkts.Pcp.UvRefraction.xy = offsetN * zF + mkts.Pcp.UvRefraction.xy;

			//init albedo surface color
			SurfaceAlbedo(mkts.Color_Albedo, mkts.Alpha, o.uv_Main);

			//init refraction color
			SurfaceRefraction(mkts.Color_Refraction, mkts.Pcp.UvRefraction);

			//mixing albedo, detail and refraction together
			MixAlbedoRefraction(mkts.Color_Albedo, mkts.Color_Refraction, _MainTint);

			//apply alpha 
			mkts.Color_Out.a = mkts.Alpha;

			#if SHADER_TARGET >= 25
				//view direction
				mkts.Pcp.ViewDirection = normalize(_WorldSpaceCameraPos - o.posWorld).xyz;

				//lightdirection and attenuation
				#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
					mkts.Pcp.LightDirection  = normalize(_WorldSpaceLightPos0.xyz - o.posWorld.xyz);
				#else
					mkts.Pcp.LightDirection = normalize(_WorldSpaceLightPos0.xyz);
				#endif

				UNITY_LIGHT_ATTENUATION(atten, o, o.posWorld.xyz);
				mkts.Pcp.LightAttenuation = atten;
				mkts.Pcp.LightColor = _LightColor0.rgb;
				mkts.Pcp.LightColorXAttenuation = mkts.Pcp.LightColor * mkts.Pcp.LightAttenuation;

				//init precalc
				PreCalcParameters(mkts);

				mkts.Color_Specular = 1.0;

				//apply emission color using a map
				mkts.Color_Emission = _EmissionColor * mkts.Color_Albedo;
			#endif
			return mkts;
		}
	#endif
#endif