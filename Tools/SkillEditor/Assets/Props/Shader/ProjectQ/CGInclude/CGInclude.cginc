#ifndef CG_INCLUDE
#define CG_INCLUDE

//----------------------------------------------------------------------------------------------------------------------------------
float4 _GlobalParameter;

#if defined(SHADOW_LOW) || defined(SHADOW_HIGH)
	sampler2D _GlobalShadowMapTexture;
	float4 _GlobalShadowParameter;
	uniform float4x4 _GlobalShadowMatrix;
#endif

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	float4 _GlobalMyFogParams;
	half4 _GlobalMyFogUpColor;
#endif

#ifdef FOGWAR_ON 
	sampler2D _GlobalFogOfWarTexture;
	float4 _GlobalFogWarParameter;
	float4 _GlobalFogWarParameter1;
#endif

//TA-------------------------------
	#if defined(MY_REPLACE_ICE_ON)
		sampler2D _GlobalReplaceICETexture;
	#endif
	#if defined(MY_REPLACE_STONE_ON)
		sampler2D _GlobalReplaceSTONETexture;
	#endif
	
//TA-------------------------------

//-----------------------------------------------------------------------------------------------------------------------------------
#if !defined(LIGHTMAP_OFF) || defined(FORCE_USE_LIGHTMAP) || defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	#define MY_FOG_LIGHTMAP_COORDS(idx) float4 fogLightmapCoord : TEXCOORD##idx;
#else
	#define MY_FOG_LIGHTMAP_COORDS(idx)
#endif

//-----------------------------------------------------------------------------------------------------------------------------------
#if defined(FOGWAR_ON)
	#define MY_FOGWAR_COORDS_PACKED(idx, vectype) vectype fogWarCoord : TEXCOORD##idx;
	#define MY_FOGWAR_COORDS(idx) MY_FOGWAR_COORDS_PACKED(idx, float4)
	#define MY_TRANSFER_FOGOFWAR1(o,worldpos) o.fogWarCoord.x = clamp((worldpos.x - _GlobalFogWarParameter.x) * _GlobalFogWarParameter.z, 0.1, 0.9); o.fogWarCoord.y = clamp((worldpos.z - _GlobalFogWarParameter.y) * _GlobalFogWarParameter.z, 0.1, 0.9)
	#define MY_TRANSFER_FOGOFWAR2(o,worldpos) o.fogWarCoord.z = clamp((worldpos.x - _GlobalFogWarParameter1.x) * _GlobalFogWarParameter1.z, 0, 1); o.fogWarCoord.w = clamp((worldpos.z - _GlobalFogWarParameter1.y) * _GlobalFogWarParameter1.w, 0, 1)
	#define MY_TRANSFER_FOGOFWAR(o,worldpos) MY_TRANSFER_FOGOFWAR1(o,worldpos); MY_TRANSFER_FOGOFWAR2(o,worldpos)

	#define MY_APPLY_FOGOFWAR1(coord,col) float4 fogOfWarTex = tex2D(_GlobalFogOfWarTexture, coord.fogWarCoord.xy); float4 fogOfWarTex1 = tex2D(_GlobalFogOfWarTexture, coord.fogWarCoord.zw)
	#define MY_APPLY_FOGOFWAR2(coord,col) float fogOfWar = lerp(fogOfWarTex.g, fogOfWarTex.r, _GlobalFogWarParameter.w); col.rgb *= max(fogOfWar,fogOfWarTex1.b);
	#define MY_APPLY_FOGOFWAR(coord,col) MY_APPLY_FOGOFWAR1(coord,col); MY_APPLY_FOGOFWAR2(coord,col)
#else
	#define MY_FOGWAR_COORDS(idx)
	#define MY_TRANSFER_FOGOFWAR(o,worldpos)
	#define MY_APPLY_FOGOFWAR(coord,col)
#endif

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	#define MY_CALC_FOG_FACTOR1(worldpos) float3 myFogViewDir = normalize(UnityWorldSpaceViewDir(worldpos.xyz)); float myFogMixFactor = dot(float3(0, -1, 0), myFogViewDir)

	#if defined(FOG_LINEAR)
		// factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
		#define MY_CALC_FOG_FACTOR2(clippos_z) float myFogFactor = max((clippos_z) * unity_FogParams.z + unity_FogParams.w, myFogMixFactor * _GlobalMyFogParams.x)
	#elif defined(FOG_EXP)
		// factor = exp(-density*z)
		#define MY_CALC_FOG_FACTOR2(clippos_z) float myFogFactor = unity_FogParams.y * (clippos_z); myFogFactor = max(exp2(-myFogFactor), myFogMixFactor * _GlobalMyFogParams.x)
	#elif defined(FOG_EXP2)
		// factor = exp(-(density*z)^2)
		#define MY_CALC_FOG_FACTOR2(clippos_z) float myFogFactor = unity_FogParams.x * (clippos_z); myFogFactor = max(exp2(-myFogFactor*myFogFactor), myFogMixFactor * _GlobalMyFogParams.x)
	#else
		#define MY_CALC_FOG_FACTOR2(clippos_z) float myFogFactor = 0.0
	#endif

	#define MY_CALC_FOG_FACTOR(worldpos,clippos_z) MY_CALC_FOG_FACTOR1(worldpos); MY_CALC_FOG_FACTOR2(UNITY_Z_0_FAR_FROM_CLIPSPACE(clippos_z))
	#define MY_TRANSFER_FOG1(o) o.fogLightmapCoord.x = myFogFactor; o.fogLightmapCoord.y = myFogMixFactor * _GlobalMyFogParams.y

	//#define MY_FOG_COORDS_PACKED(idx, vectype) vectype fogCoord : TEXCOORD##idx;

	

	#define MY_FOG_LERP_COLOR1(col,fogUpCol,fogLowCol,fogFac) half3 myFogColor = lerp((fogLowCol).rgb, (fogUpCol).rgb, saturate(fogFac.y))
	#define MY_FOG_LERP_COLOR(col,fogUpCol,fogLowCol,fogFac)  MY_FOG_LERP_COLOR1(col,fogUpCol,fogLowCol,fogFac); col.rgb = lerp((myFogColor).rgb, (col).rgb, saturate(fogFac.x))

	#define MY_APPLY_FOG_COLOR1(coord,col,fogUpCol,fogLowCol) MY_FOG_LERP_COLOR(col,fogUpCol,fogLowCol,coord)
#endif

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)

	#define MY_APPLY_FOG_COLOR(coord,col,fogUpCol,fogLowCol) MY_APPLY_FOG_COLOR1(coord.fogLightmapCoord,col,fogUpCol,fogLowCol)
	#if defined(FOGWAR_ON)
		#define MY_TRANSFER_FOG(o,worldpos,clippos_z) MY_CALC_FOG_FACTOR(worldpos,clippos_z); MY_TRANSFER_FOG1(o); MY_TRANSFER_FOGOFWAR(o,worldpos)
		#define MY_APPLY_FOG1(coord,col,fogUpCol,fogLowCol) MY_APPLY_FOG_COLOR1(coord.fogLightmapCoord,col,fogUpCol,fogLowCol); MY_APPLY_FOGOFWAR(coord,col)
	#else
		#define MY_TRANSFER_FOG(o,worldpos,clippos_z) MY_CALC_FOG_FACTOR(worldpos,clippos_z); MY_TRANSFER_FOG1(o);
		#define MY_APPLY_FOG1(coord,col,fogUpCol,fogLowCol) MY_APPLY_FOG_COLOR1(coord.fogLightmapCoord,col,fogUpCol,fogLowCol) 
	#endif

	#ifdef UNITY_PASS_FORWARDADD
		#define MY_APPLY_FOG(coord,col) MY_APPLY_FOG1(coord,col,fixed4(0,0,0,0),fixed4(0,0,0,0))
	#else
		#define MY_APPLY_FOG(coord,col) MY_APPLY_FOG1(coord,col,_GlobalMyFogUpColor,unity_FogColor)
	#endif

#else
	#define MY_TRANSFER_FOG(o,outpos,clippos_z)
	#define MY_APPLY_FOG(coord,col)
	#define MY_APPLY_FOG_COLOR(coord,col,fogUpCol,fogLowCol)
#endif



//-----------------------------------------------------------------------------------------------------------------------------------
#if defined(SHADOW_LOW) || defined(SHADOW_HIGH)
	inline void ShadowVert(in appdata_full v, in float3 worldPos, inout float4 shadow_coord)
	{
		float4x4 matWVP = mul(_GlobalShadowMatrix, unity_ObjectToWorld);
		shadow_coord = mul(matWVP, v.vertex);
		half2 shadowUV = shadow_coord.xy / shadow_coord.w * 0.5 + 0.5;
	#if UNITY_UV_STARTS_AT_TOP 
		shadowUV.y = 1 - shadowUV.y;
	#endif 
		shadow_coord.xy = shadowUV;

	#if defined(SHADOW_HIGH)
		float3 normalWorld = UnityObjectToWorldNormal(v.normal);
		fixed3 _lightDir = normalize(UnityWorldSpaceLightDir(worldPos.xyz)); 
		fixed diff = (dot(normalWorld, _lightDir)) > 0;
		shadow_coord.z = diff;
	#endif
	}

	inline void ShadowVert1(in appdata_full v, in float occShadow, in float3 worldPos, inout float4 shadow_coord)
	{
		float4x4 matWVP = mul(_GlobalShadowMatrix, unity_ObjectToWorld);
		shadow_coord = mul(matWVP, v.vertex);
		half rw = 1 / shadow_coord.w;
		half2 shadowUV = shadow_coord.xy * rw * 0.5 + 0.5;
	#if UNITY_UV_STARTS_AT_TOP 
		shadowUV.y = 1 - shadowUV.y;
	#endif 
		shadow_coord.xy = shadowUV;

	#if defined(SHADOW_HIGH)
		#if defined(UNITY_REVERSED_Z)
			shadow_coord.w = 1 - shadow_coord.z * rw;
		#else
			shadow_coord.w = shadow_coord.z * rw;// *0.5 + 0.5;
		#endif
			shadow_coord.w = lerp(1, shadow_coord.w, occShadow);
	#endif	

	#if defined(SHADOW_HIGH)
		float3 normalWorld = UnityObjectToWorldNormal(v.normal);
		fixed3 _lightDir = normalize(UnityWorldSpaceLightDir(worldPos.xyz));
		fixed diff = (dot(normalWorld, _lightDir)) > 0;
		shadow_coord.z = diff;
	#endif
	}


	inline void ShadowFrag(in float4 _ShadowCoord, inout float atten)
	{
	#if defined(SHADOW_LOW) || defined(SHADOW_HIGH)
		float2 shadowUV = _ShadowCoord.xy; 
		float4 shadowMapTex = tex2D(_GlobalShadowMapTexture, shadowUV);
	#if defined(SHADOW_LOW)
		_ShadowCoord.z *= (_ShadowCoord.x > 0.01 && _ShadowCoord.y > 0.01 && _ShadowCoord.x < 0.99 && _ShadowCoord.y < 0.99);
	#endif
		atten = 1.0 - shadowMapTex.r * _ShadowCoord.z;

		atten = atten * _GlobalParameter.x + (1 - _GlobalParameter.x);
	#endif
	}

	inline void ShadowFrag1(in float4 _ShadowCoord, inout float atten)
	{
	#if defined(SHADOW_LOW) || defined(SHADOW_HIGH)
		float2 shadowUV = _ShadowCoord.xy;
		float4 shadowMapTex = tex2D(_GlobalShadowMapTexture, shadowUV);
		
	#if  defined(SHADOW_HIGH)
		fixed bZero = (shadowMapTex.g < _ShadowCoord.w);
		atten = 1.0 - shadowMapTex.r * _ShadowCoord.z * bZero;
	#else
		_ShadowCoord.z *= (_ShadowCoord.x > 0.01 && _ShadowCoord.y > 0.01 && _ShadowCoord.x < 0.99 && _ShadowCoord.y < 0.99);
		atten = 1.0 - shadowMapTex.r * _ShadowCoord.z;
	#endif 

		atten = atten * _GlobalParameter.x + (1 - _GlobalParameter.x);
	#endif
	}
#endif
//-----------------------------------------------------------------------------------------------------------------------------------

	inline half3 MyDecodeLightmapDoubleLDR(fixed4 color)
	{
#ifdef UNITY_COLORSPACE_GAMMA
		return 2.0 * color.rgb;
#else
		return unity_Lightmap_HDR.x * color.rgb;
#endif
	}

	inline void ClacShadowAtten(in fixed3 color, inout float atten)
	{
#if defined(SHADOW_HIGH)
		float ll = dot(color.xyz, float3(0.3, 0.6, 0.1));
		float t = max(ll - _GlobalShadowParameter.x, 0) * _GlobalShadowParameter.y;
		atten = lerp(1, atten, t);
#endif
	}

	inline fixed3 LightMapFrag1(in float2 _LightMapCoord, in sampler2D _LightMapTexture)
	{
		fixed3 lm = MyDecodeLightmapDoubleLDR(tex2D(_LightMapTexture, _LightMapCoord.xy));
		return lm.xyz;
	}

	inline fixed3 LightMapFrag2(in float2 _LightMapCoord, in float light_t, in sampler2D _LightMapTexture)
	{
		fixed3 lm = MyDecodeLightmapDoubleLDR(tex2D(_LightMapTexture, _LightMapCoord.xy));
		lm.xyz = lerp(fixed3(1, 1, 1), lm.xyz, light_t);
		return lm.xyz;
	}

	inline fixed3 LightMapAtten(in fixed3 lm, inout float atten)
	{
		ClacShadowAtten(lm, atten);
		return lm.xyz;
	}

#ifndef LIGHTMAP_OFF

	inline fixed3 LightMapFrag(in float2 _LightMapCoord)
	{
		fixed3 lm = MyDecodeLightmapDoubleLDR(UNITY_SAMPLE_TEX2D(unity_Lightmap, _LightMapCoord.xy));
		return lm.xyz;
	}
	
#endif

//TA-------------------------------
#if defined(MY_REPLACE_ICE_ON) || defined(MY_REPLACE_STONE_ON)
	inline void My_Replace(in float2 uv,in fixed rcol,in fixed scol,inout fixed3 appcol)
	{
		fixed l = 0;
		fixed srcol= rcol+scol;
		fixed4 replaceTex = 0;
		#if defined(MY_REPLACE_OFF)
			replaceTex = 0;
		#elif defined(MY_REPLACE_ICE_ON)
			l=0.7;
			replaceTex = tex2D(_GlobalReplaceICETexture, uv*6);
		#elif  defined(MY_REPLACE_STONE_ON)
			l=1;
			replaceTex = tex2D(_GlobalReplaceSTONETexture, uv*6);
			srcol = (1- srcol)*(1- srcol)*(1- srcol)*(1- srcol)*replaceTex.r*replaceTex.r;
		#else
		#endif
		appcol = lerp(appcol,replaceTex+srcol,l);
	}
#endif

#if defined(MY_REPLACE_ICE_ON) || defined(MY_REPLACE_STONE_ON)
	#define MY_REPLACE_TEX(uv,rc,sc,col) My_Replace(uv,rc,sc,col);
#else
	#define MY_REPLACE_TEX(uv,rc,sc,col);
#endif
//TA_Character-------------------------------

#if defined(CHA_HIGH) || defined(CHA_MID)
	inline void CHA_NormalVert(in float4 vertex, in float3 normal, in float4 tangent, inout half3 normal_t,inout half3 binormal_t,inout half4 tangent_t,inout half4 eyeDir_t, inout half4 worldPos_t,inout fixed4 vertex_t )
	{
		vertex_t = UnityObjectToClipPos(vertex );
		eyeDir_t.xyz = normalize( mul(unity_ObjectToWorld, vertex) -  _WorldSpaceCameraPos);
		eyeDir_t.w = 1;
		half3 normalWorld = UnityObjectToWorldNormal(normal);
		half4 tangentWorld = float4(UnityObjectToWorldDir(tangent.xyz),tangent.w);  
		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);  
		normal_t = tangentToWorld[2];
		binormal_t = tangentToWorld[1];  
		tangent_t.xyz = tangentToWorld[0]; 
		tangent_t.w = 1;
		worldPos_t = mul(unity_ObjectToWorld, vertex);    

	} 

	inline void CHA_NormalFrag(in float4 vertex,in float3 normal,in float3 binormal,in float4 tangent,in fixed4 eyeDir,in float3 normalTex,inout fixed nh_t,inout fixed diff_t,inout fixed3 normal_t)
	{
	#ifndef USING_DIRECTIONAL_LIGHT
		fixed3 lightDir = WorldSpaceLightDir( vertex );     
	#else  
		fixed3 lightDir = _WorldSpaceLightPos0.xyz;         
	#endif
   		normal_t = normalize(tangent.xyz * normalTex.x + binormal * normalTex.y + normal * normalTex.z); 
		diff_t = saturate(dot(normal_t,lightDir)); 
		nh_t = saturate (dot (normal_t, -eyeDir.xyz));
	} 

	#define MY_CHA_NORMAL_V2F float4 pos : POSITION;half3 normal : NORMAL;half3 binormal : BINORMAL; half4 tangent   :TANGENT; float4 uv : TEXCOORD0;half4 eyeDir : TEXCOORD1;half4 worldPos: TEXCOORD3;
	#define MY_CHA_NORMAL_VERT(IN,OUT) 	CHA_NormalVert(IN.vertex, IN.normal, IN.tangent, OUT.normal,OUT.binormal,OUT.tangent, OUT.eyeDir,OUT.worldPos,OUT.pos);
	#define MY_CHA_FRAG(IN,normalMap,nh,diff,Miancolor,s,_normal) fixed3 _normal;float nh;float diff;float3 normalTex = UnpackNormal(tex2D(normalMap, IN.uv.xy)); fixed4 Miancolor = tex2D(_MainTex, IN.uv.xy)*s; CHA_NormalFrag(IN.pos,IN.normal,IN.binormal,IN.tangent,IN.eyeDir,normalTex,nh,diff,_normal);
#else
	#define MY_CHA_NORMAL_V2F float4 pos : POSITION;float4 uv : TEXCOORD0;half4 eyeDir : TEXCOORD1;half3 normal : NORMAL; half4 worldPos: TEXCOORD3;
	#define MY_CHA_NORMAL_VERT(IN,OUT)  OUT.pos = UnityObjectToClipPos(IN.vertex ); OUT.eyeDir = fixed4(normalize( mul(unity_ObjectToWorld, IN.vertex) -  _WorldSpaceCameraPos),1); OUT.normal = UnityObjectToWorldNormal(IN.normal);
	#define MY_CHA_FRAG(IN,normalMap,nh,diff,Miancolor,s,_normal)fixed3 _normal;fixed4 Miancolor = tex2D(_MainTex, IN.uv.xy)*s*1.5; fixed nh = saturate (dot (IN.normal, -IN.eyeDir.xyz));
//	#define MY_NORMAL_COORDS
#endif

//TA-------------------------------

#define MY_APPLY_FINAL_COLOR(col,atten) col.rgb = col * (2.0 - _GlobalParameter.y) * atten;// lerp(col, fixed4(0, 0, 0, 0), _GlobalParameter.y) * atten;

#if !defined(LIGHTMAP_OFF) || defined(FORCE_USE_LIGHTMAP)
	//#define MY_LIGHTMAP_COORDS(idx) float2 lightmapCoord : TEXCOORD##idx;
	#define MY_TRANSFER_LIGHTMAP(o,v) o.fogLightmapCoord.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	#define MY_TRANSFER_CUSLIGHTMAP(o,coord,st) o.fogLightmapCoord.zw = coord.xy * st.xy + st.zw;
	#define MY_APPLY_FINAL_LIGHTMAP(IN,col) float atten = 1.0; col.rgb *= LightMapAtten(LightMapFrag(IN.fogLightmapCoord.zw), atten); MY_APPLY_FINAL_COLOR(col,atten);
	#define MY_SAMPLE_LIGHTMAP(IN,lm) lm.rgb = LightMapFrag(IN.fogLightmapCoord.zw);
	#define MY_SAMPLE_CUSLIGHTMAP(IN,lm,tex) lm.rgb = LightMapFrag1(IN.fogLightmapCoord.zw, tex);
	#define MY_SAMPLE_CUSLIGHTMAP_T(IN,lm,tex,t) lm.rgb = LightMapFrag2(IN.fogLightmapCoord.zw, t, tex);
#else
	#define MY_TRANSFER_LIGHTMAP(o,v)
	#define MY_TRANSFER_CUSLIGHTMAP(o,coord,st)
	#define MY_APPLY_FINAL_LIGHTMAP(IN,col) float atten = 1.0; MY_APPLY_FINAL_COLOR(col,atten);
	#define MY_SAMPLE_LIGHTMAP(IN,lm)
	#define MY_SAMPLE_CUSLIGHTMAP(IN,lm,tex)
	#define MY_SAMPLE_CUSLIGHTMAP_T(IN,lm,tex,t)
#endif

#if defined(SHADOW_LOW) || defined(SHADOW_HIGH)
	#define MY_SHADOW_COORDS(idx) float4 shadowCoord : TEXCOORD##idx;
	#define MY_TRANSFER_SHADOW(v,o,occ,worldpos) ShadowVert1(v, occ, worldpos.xyz, o.shadowCoord)
	#define MY_TRANSFER_TERSHADOW(v,o,worldpos) ShadowVert(v, worldpos.xyz, o.shadowCoord)
	
	#if !defined(LIGHTMAP_OFF) || defined(FORCE_USE_LIGHTMAP)
		#define MY_APPLY_FINAL_LIGHTMAP_SHADOW(IN,col) float atten = 1.0; ShadowFrag1(IN.shadowCoord, atten); col.rgb *= LightMapAtten(LightMapFrag(IN.fogLightmapCoord.zw),atten); MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_LIGHTMAP_TERSHADOW(IN,col) float atten = 1.0; ShadowFrag(IN.shadowCoord, atten); col.rgb *= LightMapAtten(LightMapFrag(IN.fogLightmapCoord.zw),atten); MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_CUSLIGHTMAP_SHADOW(IN,col,t,tex) float atten = 1.0; ShadowFrag1(IN.shadowCoord, atten); col.rgb *= LightMapAtten(LightMapFrag2(IN.fogLightmapCoord.zw,t,tex),atten); MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_CUSLIGHTMAP_TERSHADOW(IN,col,tex) float atten = 1.0; ShadowFrag(IN.shadowCoord, atten); col.rgb *= LightMapAtten(LightMapFrag1(IN.fogLightmapCoord.zw,tex),atten); MY_APPLY_FINAL_COLOR(col,atten)

		#define MY_APPLY_FINAL_LIGHT_SHADOW(IN,col,light) float atten = 1.0; ShadowFrag1(IN.shadowCoord, atten); col.rgb *= LightMapAtten(light.rgb,atten); MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_LIGHT_TERSHADOW(IN,col,light) float atten = 1.0; ShadowFrag(IN.shadowCoord, atten); col.rgb *= LightMapAtten(light.rgb,atten); MY_APPLY_FINAL_COLOR(col,atten)
	#else
		#define MY_APPLY_FINAL_LIGHTMAP_SHADOW(IN,col) UNITY_LIGHT_ATTENUATION(atten,IN,IN.worldPos); col = lerp(UNITY_LIGHTMODEL_AMBIENT*col,col*(_LightColor0+UNITY_LIGHTMODEL_AMBIENT),atten*0.5);// MY_APPLY_FINAL_COLOR(col,atten) ;
		#define MY_APPLY_FINAL_LIGHTMAP_TERSHADOW(IN,col) float atten = 1.0; ShadowFrag(IN.shadowCoord, atten); MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_CUSLIGHTMAP_SHADOW(IN,col,t,tex) MY_APPLY_FINAL_LIGHTMAP_SHADOW(IN,col)
		#define MY_APPLY_FINAL_CUSLIGHTMAP_TERSHADOW(IN,col,tex) MY_APPLY_FINAL_LIGHTMAP_TERSHADOW(IN,col)

		#define MY_APPLY_FINAL_LIGHT_SHADOW(IN,col,light) float atten = 1.0; ShadowFrag1(IN.shadowCoord, atten); MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_LIGHT_TERSHADOW(IN,col,light) float atten = 1.0; ShadowFrag(IN.shadowCoord, atten); MY_APPLY_FINAL_COLOR(col,atten)
	#endif
#else
	#define MY_SHADOW_COORDS(idx)
	#define MY_TRANSFER_SHADOW(v,o,occ,worldpos)
	#define MY_TRANSFER_TERSHADOW(v,o,worldpos)
	
	#if !defined(LIGHTMAP_OFF) || defined(FORCE_USE_LIGHTMAP)
		#define MY_APPLY_FINAL_LIGHTMAP_SHADOW(IN,col) float atten = 1.0; col.rgb *= LightMapAtten(LightMapFrag(IN.fogLightmapCoord.zw),atten); MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_LIGHTMAP_TERSHADOW(IN,col) MY_APPLY_FINAL_LIGHTMAP_SHADOW(IN,col)
		#define MY_APPLY_FINAL_CUSLIGHTMAP_SHADOW(IN,col,t,tex) float atten = 1.0; col.rgb *= LightMapAtten(LightMapFrag2(IN.fogLightmapCoord.zw,t, tex),atten); MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_CUSLIGHTMAP_TERSHADOW(IN,col,tex) float atten = 1.0; col.rgb *= LightMapAtten(LightMapFrag1(IN.fogLightmapCoord.zw, tex),atten); MY_APPLY_FINAL_COLOR(col,atten)

		#define MY_APPLY_FINAL_LIGHT_SHADOW(IN,col,light) float atten = 1.0; col.rgb *= LightMapAtten(light,atten); MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_LIGHT_TERSHADOW(IN,col,light) MY_APPLY_FINAL_LIGHT_SHADOW(IN,col,light)
	#else
		#define MY_APPLY_FINAL_LIGHTMAP_SHADOW(IN,col) float atten = 1.0; MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_LIGHTMAP_TERSHADOW(IN,col) MY_APPLY_FINAL_LIGHTMAP_SHADOW(IN,col)
		#define MY_APPLY_FINAL_CUSLIGHTMAP_SHADOW(IN,col,t,tex) MY_APPLY_FINAL_LIGHTMAP_SHADOW(IN,col)
		#define MY_APPLY_FINAL_CUSLIGHTMAP_TERSHADOW(IN,col,tex) MY_APPLY_FINAL_CUSLIGHTMAP_SHADOW(IN,col,0,tex)

		#define MY_APPLY_FINAL_LIGHT_SHADOW(IN,col,light) float atten = 1.0; MY_APPLY_FINAL_COLOR(col,atten)
		#define MY_APPLY_FINAL_LIGHT_TERSHADOW(IN,col,light) MY_APPLY_FINAL_LIGHT_SHADOW(IN,col,light)
	#endif
#endif	


//-----------------------------------------------------------------------------------------------------------------------------------
#if defined(NORMAL_LOW) || defined(NORMAL_HIGH)
	#define MY_NORMAL_COORDS half4 lightDirCoord: NORMAL;
	#define MY_NORMALSPEC_COORDS half4 lightDirCoord: NORMAL; half3 viewDirCoord: TANGENT;
#else
	#define MY_NORMAL_COORDS
	#define MY_NORMALSPEC_COORDS
#endif

#if defined(NORMAL_LOW) || defined(NORMAL_HIGH)

	inline void NormalVert(in float3 normal, in float4 tangent, in float4 vertex, in float3 worldPos, inout half4 lightDir_t)
	{
		float3 binormal = cross(normal, tangent.xyz) * tangent.w;
		float3x3 rotation = float3x3(tangent.xyz, binormal, normal);
		lightDir_t.xyz = mul(rotation, ObjSpaceLightDir(vertex));

		lightDir_t.w = length(worldPos - _WorldSpaceCameraPos);
	}

	inline float3 NormalFrag(in float3 normal, in float3 light, in float4 ness, in half4 lightDir_t)  
	{
		float diff = max(ness.y, abs(dot((normal), normalize(lightDir_t.xyz))));
		diff = diff * ness.x + (1.0f - ness.x);
		diff *= (1.0f - ness.z);
		return lerp(light.rgb * diff, light.rgb, min(lightDir_t.w * 0.01, 1));//_LightColor0.rgb
	}

	inline void NormalSpecVert(in float3 normal, in float4 tangent, in float4 vertex, in float3 worldPos, inout half4 lightDir_t, inout half3 viewDir_t)
	{

		float3 binormal = cross(normal, tangent.xyz) * tangent.w;
		float3x3 rotation = float3x3(tangent.xyz, binormal, normal);
		lightDir_t.xyz = mul(rotation, ObjSpaceLightDir(vertex));
		viewDir_t = mul(rotation, ObjSpaceViewDir(vertex));

		lightDir_t.w = length(worldPos - _WorldSpaceCameraPos);
	}

	inline float3 NormalSpecFrag(in float3 col, in float3 normal, in float3 light, in float4 ness, in float met, in float3 gloss, in half4 lightDir_t, in half3 viewDir_t)
	{
		half3 lightDir = normalize(lightDir_t.xyz);
		half3 viewDir = normalize(viewDir_t.xyz);

		float diff = max(ness.y, abs(dot((normal), lightDir)));
		diff = diff * ness.x + (1.0f - ness.x);
		diff *= (1.0f - ness.z);
		float3 difuse = lerp(light.rgb * diff, light.rgb, min(lightDir_t.w * 0.01, 1));

	#if defined(NORMAL_HIGH)
			float3 refl = reflect(-lightDir, normal);
			float nh = dot(normal, viewDir);
			float mh = nh;
			float fh = nh*nh*nh;
			fh *= fh*fh*fh;
			nh = saturate(1-abs(2*nh-0.7));
			//float3 spec = light.rgb * pow(nh, 64);// *gloss;

			nh *= nh*nh;
			nh *= nh;
			fixed nf = lerp(mh*0.5,nh+fh,met);

			float3 spec = saturate(light.rgb * nf * gloss* ness.w*2); 

		spec = lerp(spec, float3(0, 0, 0), min(lightDir_t.w * 0.01, 1));    

		return (col * difuse) +spec;// +(col * UNITY_LIGHTMODEL_AMBIENT.rgb);
	#else
		return (col * difuse);
	#endif

	}

	#define MY_TRANSFER_NORMAL(v, o, worldpos) NormalVert(v.normal, v.tangent, v.vertex, worldpos, o.lightDirCoord);
	#define MY_APLLY_NORMAL(col, IN, normalMap, uv, n, light) float3 normalTex = UnpackNormal(tex2D(normalMap, uv.xy)); col.xyz *= NormalFrag(normalTex, light, n, IN.lightDirCoord);

	#define MY_TRANSFER_NORMALSPEC(v, o, worldpos) NormalSpecVert(v.normal, v.tangent, v.vertex, worldpos, o.lightDirCoord, o.viewDirCoord);
	#if defined(NORMAL_HIGH)
		#define MY_APLLY_NORMALSPEC(col, IN, normalMap, uvn, specMap, uvs, n, m, light) float3 normalTex = UnpackNormal(tex2D(normalMap, uvn.xy)); float3 specTex = tex2D(specMap, uvs.xy); col.xyz = NormalSpecFrag(col, normalTex, light, n, m, specTex.rgb, IN.lightDirCoord, IN.viewDirCoord);
	#else
		#define MY_APLLY_NORMALSPEC(col, IN, normalMap, uvn, specMap, uvs, n, m, light) float3 normalTex = UnpackNormal(tex2D(normalMap, uvn.xy));										  col.xyz = NormalSpecFrag(col, normalTex, light, n, m, fixed3(1,1,1), IN.lightDirCoord, IN.viewDirCoord);
	#endif
	    #define MY_APLLY_NORMALSPEC1(col, IN, normalMap, uvn, n, g, m, light) float3 normalTex = UnpackNormal(tex2D(normalMap, uvn.xy));               									  col.xyz = NormalSpecFrag(col, normalTex, light, n, m, g, IN.lightDirCoord, IN.viewDirCoord);
#else
	#define MY_TRANSFER_NORMAL(v, o, worldpos)
	#define MY_APLLY_NORMAL(col, IN, normalMap, uv, n, light) 

	#define MY_TRANSFER_NORMALSPEC(v, o, worldpos)
	#define MY_APLLY_NORMALSPEC(col, IN, normalMap, uvn, specMap, uvs, n, m, light)
	#define MY_APLLY_NORMALSPEC1(col, IN, normalMap, uvn, n, m, g, light)
#endif
//-----------------------------------------------------------------------------------------------------------------------------------

#endif