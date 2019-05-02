//Light Calculations
#ifndef MK_GLASS_LIGHT
	#define MK_GLASS_LIGHT

	#include "../Common/MKGlassDef.cginc"

	////////////
	// LIGHT
	////////////
	void MKGlassLightMain(inout MKGlassSurface mkts, in VertexOutputForward o)
	{
		#if SHADER_TARGET >= 25
			fixed4 c;
			//skip diffuse lighting
			c.rgb = mkts.Color_Albedo;

			#if !defined(USING_DIRECTIONAL_LIGHT)
				c.rgb *= (mkts.Pcp.NdotL * mkts.Pcp.LightColorXAttenuation);
			#else
				//skip diffuse for directional
			#endif

			//Specular
			half spec;
			_Shininess *= mkts.Color_Specular.g;

			spec = GetSpecular(mkts.Pcp.NdotHV, _Shininess);
				
			mkts.Color_Specular = spec;
			mkts.Color_Specular = mkts.Color_Specular * _SpecColor * (_SpecularIntensity *  mkts.Color_Specular.r);

			//apply specular
			c.rgb += mkts.Color_Specular * mkts.Pcp.LightColor * _LightColor0.rgb;

			//apply alpha
			c.a = mkts.Alpha;

			mkts.Color_Out = c;
		#else
			mkts.Color_Out = fixed4(mkts.Color_Albedo.rgb, mkts.Color_Out.a);
		#endif
	}

	void MKGlassLightLMCombined(inout MKGlassSurface mkts, in VertexOutputForward o)
	{
		//apply lighting to surface
		MKGlassLightMain(mkts, o);
		
		#if SHADER_TARGET >= 25
			#ifdef MK_GLASS_FWD_BASE_PASS
				//add ambient light
				fixed3 amb = mkts.Color_Albedo * o.aLight;
				#if _MKGLASS_REFLECTIVE
					mkts.Color_Out.rgb = lerp(mkts.Color_Out.rgb + amb * _MainTint, mkts.Color_Out.rgb + amb * T_H, _ReflectIntensity);
				#else
					//mkts.Color_Out.rgb += amb;
				#endif
			#endif

			#ifdef MK_GLASS_FWD_BASE_PASS
				#if LIGHTMAP_ON || DYNAMICLIGHTMAP_ON
					half3 lm = 0;
					#ifdef LIGHTMAP_ON
							fixed4 lmBCT = UNITY_SAMPLE_TEX2D(unity_Lightmap, o.uv_Lm.xy);
							fixed3 bC = DecodeLightmap(lmBCT);
							//handle directional lightmaps
						#if DIRLIGHTMAP_COMBINED
							// directional lightmaps
							fixed4 bDT = UNITY_SAMPLE_TEX2D_SAMPLER (unity_LightmapInd, unity_Lightmap, o.uv_Lm.xy);
							lm = DecodeDirectionalLightmap (bC, bDT, o.normalWorld);

							#if defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN)
								lm = SubtractMainLightWithRealtimeAttenuationFromLightmap (lm, mkts.Pcp.LightAttenuation, lmBCT, o.normalWorld);
							#endif
						//handle not directional lightmaps
						#else
							lm = bC;
							#if defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN)
								lm = SubtractMainLightWithRealtimeAttenuationFromLightmap(lm, mkts.Pcp.LightAttenuation, lmBCT, o.normalWorld);
							#endif
						#endif
					#endif

					//handle dynamic lightmaps
					#ifdef DYNAMICLIGHTMAP_ON
						fixed4 lmRTCT = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, o.uv_Lm.zw);
						half3 rTC = DecodeRealtimeLightmap (lmRTCT);

						#ifdef DIRLIGHTMAP_COMBINED
							half4 rDT = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, o.uv_Lm.zw);
							lm += DecodeDirectionalLightmap (rTC, rDT, o.normalWorld);
						#else
							lm += rTC;
						#endif
					#endif

					//apply lightmap
					mkts.Color_Out.rgb += mkts.Color_Albedo * lm;
				#endif
			#endif
		#endif
	}

	void MKGlassLightFinal(inout MKGlassSurface mkts, in VertexOutputForward o)
	{
		#if SHADER_TARGET >= 25
			#if MK_GLASS_FWD_BASE_PASS
				//apply rim lighting
				mkts.Color_Emission += RimDefault(_RimSize, mkts.Pcp.VdotN, _RimColor.rgb, _RimIntensity);
				//apply rim lighting
				mkts.Color_Out.rgb += mkts.Color_Emission;
			#endif
		#else
			mkts.Color_Out = fixed4(mkts.Color_Out.rgb, mkts.Color_Out.a);
		#endif
	}
#endif