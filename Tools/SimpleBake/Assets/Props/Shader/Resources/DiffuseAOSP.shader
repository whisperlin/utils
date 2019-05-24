Shader "TA/BumpedAOSpecular"
{
	Properties
	{
		_Color("颜色", Color) = (1, 1, 1, 1)
		_MainTex("主贴图", 2D) = "white" {}
		_Normal("法线", 2D) = "bump" {}
		//_NormalPower("法线强度",Range(0,1)) = 1
		//_NormalMark("法线模糊图",  2D) = "black" {}

		_Spec("高光色", Color) = (1, 1, 1, 1)

		_SpecMap("高光强度贴图", 2D) = "white" {}
		_SpPower("高光强度", Range(0,1)) = 1
		
 
		_GlossMap("高光平滑贴图", 2D) = "black" {}
		_Gloss("高光平滑", Range(0, 1)) = 0.5

		_AO("AO", 2D) = "white" {}
		//_IntensityColor("Intensity Color", Color) = (0, 0, 0, 0)

		_DifSC("漫反射色差",Range(0,0.5)) = 0  //因为pbr漫反射的颜色部分会贡献给高光，所以对比相对会弱点. 
		_BackColor("背光", Color) = (0, 0, 0, 1)

		//[KeywordEnum(Off, On)] _IsEmissive ("是否开启自发光", Float) = 0
		//_Emissive("自发光", 2D) = "black" {}
		//_EmissiveColor("自发光颜色", Color) = (1, 1, 1, 1)

		[KeywordEnum(Off, On)] _IsMetallic("是否开启金属度", Float) = 0
		environment_reflect("金属反射贴图", 2D) = "black" {}
		metallic_color("金属颜色", Color) = (1, 1, 1, 0)
		metallic_power("金属强度", Range(0,1)) = 1

		[KeywordEnum(Off, On)] _IsMetallic("是否开启金属度", Float) = 0
		metallic_ctrl_tex("金属控制贴图", 2D) = "white" {}


		_CtrlTex("控制图", 2D) = "white" {}


		[KeywordEnum(Off, On)] _IsMEmission("是否开启自发光", Float) = 0


		_Emission("自发光", Color) = (0.5, 0.5, 0.5, 1)
		//[KeywordEnum(Off, On)] _IsNormalSnow("是否开雪", Float) = 0
		//_SnowPower("法线雪强度", Range(0, 1)) = 1


		[Toggle]_snow_options("----------雪选项-----------",int) = 0

		_SnowNormalPower("  雪法线强度", Range(0.3, 1)) = 1
		//_SnowColor("雪颜色", Color) = (0.784, 0.843, 1, 1)
		_SnowEdge("  雪边缘过渡", Range(0.01, 0.3)) = 0.2
		//_SnowNoise("雪噪点", 2D) = "white" {}
		_SnowNoiseScale("  雪噪点缩放", Range(0.1, 20)) = 1.28
		//_SnowGloss("雪高光", Range(0, 1)) = 1

		//_SnowMeltPower("  雪_消融影响调节", Range(1, 2)) =  1
		_SnowLocalPower("  雪_法线影响调节", Range(-5, 0.3)) =  0


		[Toggle(HARD_SNOW)] HARD_SNOW("  硬边雪", Float) = 0
		[Toggle(MELT_SNOW)] MELT_SNOW("  消融雪", Float) = 0




		[Enum(UnityEngine.Rendering.CullMode)] _Cull("剪裁模式,Off为双面贴图", Float) = 2
		//_SnowPower()
		[Toggle(S_DEVELOP)] S_DEVELOP("开发者模式", Float) = 0
		[Toggle(S_BAKE)] S_BAKE("烘焙模式", Float) = 0
		[HideInInspector] _Shadow("Shadow", 2D) = "black" {}
		[HideInInspector] _ShadowFade("ShadowFade", 2D) = "black" {}
		[HideInInspector] _ShadowStrength("ShadowStrength", Range(0, 1)) = 1
 
		[HideInInspector] _RedCtrl("", 2D) = "white" {}
		[HideInInspector] _GreenCtrl("", 2D) = "white" {}
		[HideInInspector] _BlueCtrl("", 2D) = "white" {}
		[HideInInspector] _AlphaCtrl("", 2D) = "white" {}

	}

		SubShader
	{
		Cull[_Cull]
		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog
		#pragma multi_compile __ SHADOW_ON
		#pragma multi_compile __ BRIGHTNESS_ON
		#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
		#pragma multi_compile __ RAIN_ENABLE
		#pragma multi_compile _ISMETALLIC_OFF _ISMETALLIC_ON  
		#pragma multi_compile _ISMEMISSION_OFF   _ISMEMISSION_ON  


 
		#pragma multi_compile __ SNOW_ENABLE



		#pragma shader_feature S_BAKE
		#pragma shader_feature S_DEVELOP
		#pragma shader_feature HARD_SNOW
		#pragma shader_feature MELT_SNOW

 
 

		//#pragma multi_compile __ GLOBAL_SH9
 

		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "SceneWeather.inc"
		//#define GLOBAL_SH9 1
		//#if GLOBAL_SH9
		//#include "../SHGlobal.cginc"
		//#endif

		struct appdata
	{
		half4 vertex : POSITION;
		half2 uv : TEXCOORD0;
		half2 uv2 : TEXCOORD1;
		half3 normal : NORMAL;
		half4 tangent : TANGENT;
	};

	struct v2f
	{
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			half3 tspace0 : TEXCOORD2;
		half3 tspace1 : TEXCOORD3;
		half3 tspace2 : TEXCOORD4;
		float3 posWorld : TEXCOORD5;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
		half2 uv2 : TEXCOORD6;
#endif
#ifdef SHADOW_ON
		half2 shadow_uv : TEXCOORD7;
#endif
	};

	//sampler2D unity_NHxRoughness;
	fixed _CullSepe;
	fixed4 _Color;
	fixed4 _BackColor;

	sampler2D _MainTex;
	half4 _MainTex_ST;
	sampler2D _Normal;
	//half _NormalPower;
	fixed3 _Spec;
	half _Gloss;
	half metallic_power;


	fixed _SnowPower;
	fixed _SnowNormalPower;
	fixed4 _SnowColor;
	fixed _SnowEdge;
	sampler2D _SnowNoise;
	half _SnowNoiseScale;
	half _SnowGloss;
	half _SnowLocalPower;
	half _SnowMeltPower;
	 

	//fixed3 _IntensityColor;

//#ifdef _ISEMISSIVE_ON
//	sampler2D _Emissive;
//	fixed3 _EmissiveColor;
//#endif

	float _DifSC;
#ifdef SHADOW_ON
	sampler2D _Shadow, _ShadowFade;
	float4x4 shadow_projector;
	fixed _ShadowStrength;
	float4 _Shadow_TexelSize;
#endif

#ifdef BRIGHTNESS_ON
	fixed3 _Brightness;
#endif
	uniform sampler2D _AO;
	//uniform sampler2D _NormalMark;
 
	sampler2D _CtrlTex;
	sampler2D _SpecMap;
	sampler2D _GlossMap;
	half4 _Emission;


	sampler2D sam_environment_reflect;

	float _SpPower;
	v2f vert(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);

		o.posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;

		half3 normal = UnityObjectToWorldNormal(v.normal);
		half3 tangent = UnityObjectToWorldDir(v.tangent.xyz);
		half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
		half3 bitangent = cross(normal, tangent) * tangentSign;

		o.tspace0 = half3(tangent.x, bitangent.x, normal.x);
		o.tspace1 = half3(tangent.y, bitangent.y, normal.y);
		o.tspace2 = half3(tangent.z, bitangent.z, normal.z);

#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
		o.uv2 = v.uv2 * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

#ifdef SHADOW_ON
		half4 shadow_uv = mul(shadow_projector, mul(unity_ObjectToWorld, v.vertex));
		o.shadow_uv = (shadow_uv.xy / shadow_uv.w + float2(1, 1)) * 0.5;
#endif

 
			 

#if S_BAKE
	
	float2 uv0 = v.uv;
//#if defined(SHADER_API_D3D9)||defined(SHADER_API_D3D11)||defined(SHADER_API_D3D11_9X)
//	uv0.y = 1.0 - uv0.y;
//#endif
	
	o.pos.xy = uv0 * 2 - float2(1, 1);
	o.pos.z = 0.5;
	o.pos.w = 1;
 

	o.pos.y =   o.pos.y;

	 
#endif

		UNITY_TRANSFER_FOG(o, o.pos);
		return o;
	}

	//inline half2 Pow4 (half2 x) { return x *x*x*x; }
	float ArmBRDF(float roughness, float NdotH, float LdotH)
	{
		float n4 = roughness*roughness*roughness*roughness;
		float c = NdotH*NdotH   *   (n4 - 1) + 1;
		float b = 4 * 3.14*       c*c  *     LdotH*LdotH     *(roughness + 0.5);
		return n4 / b;

	}

	//这个是网易的。
	inline half2 ToRadialCoordsNetEase(half3 envRefDir)
	{
 
		half k = envRefDir.x / (length(envRefDir.xz) + 1E-06f);
		half2 normalY = { k, envRefDir.y };
		half2 latitude = acos(normalY) * 0.3183099f;
		half s = step(envRefDir.z, 0.0f);
		half u = s - ((s * 2.0f - 1.0f) * (latitude.x * 0.5f));
		return half2(u, latitude.y);
	}
	//这个是unity。
	inline float2 ToRadialCoords(float3 coords)
	{
		float3 normalizedCoords = normalize(coords);
		float latitude = acos(normalizedCoords.y);
		float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
		float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
		return float2(0.5, 1.0) - sphereCoords;
	}
 
	sampler2D environment_reflect;
	sampler2D metallic_ctrl_tex;
	float4 metallic_color;


	float ArmBRDFEnv(float roughness, float NdotV)
	{
		float a1 = (1 - max(roughness, NdotV));
		return a1*a1*a1;

	}
	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 c_base = tex2D(_MainTex, i.uv);
		
		fixed4 c = c_base * _Color;



		float4 nor_val = tex2D(_Normal, i.uv);
 
		fixed3 n = UnpackNormal(nor_val);
		//float4 nor_mark_val = tex2D(_NormalMark, i.uv);
		//_NormalPower = _NormalPower*( 1.0 - nor_mark_val );
		//n = lerp(float3(0,0,1),n,_NormalPower);

#if S_DEVELOP
		half4 _AO_varC = tex2D(_AO, i.uv);
		half _AO_var = _AO_varC.r  ;
#else
		
		half4 var_CtrlTex = tex2D(_CtrlTex, i.uv);
		half _AO_var = var_CtrlTex.b;
#endif

	

		
		//return _AO_var;
		fixed3 c0 = c*_AO_var;
		half3 normal;
		normal.x = dot(i.tspace0, n);
		normal.y = dot(i.tspace1, n);
		normal.z = dot(i.tspace2, n);

		normal = normalize(normal); 


 
	#if SNOW_ENABLE 


		#if   defined(HARD_SNOW) || defined(MELT_SNOW) 
		float snoize = tex2D(_SnowNoise, i.uv*_SnowNoiseScale).r;

		#endif

		#if MELT_SNOW
			half snl  =  snoize * _SnowMeltPower;
			 
		#else
			half snl  =  dot(normal, half3(0,1,0))   ;
			snl = (1.0-_SnowLocalPower)*snl + _SnowLocalPower;
		#endif
 
		fixed nt = smoothstep(_SnowPower,_SnowPower+_SnowEdge,snl);
 

	 	 #if HARD_SNOW
		 nt = step(snoize,nt);
		 #endif
		 //float nt2 = step(snoize,snoize);
		 c0.rgb = lerp(c0.rgb,_SnowColor.rgb,nt *_SnowColor.a);
		 half3 up0 = half3(i.tspace0.z,i.tspace1.z,i.tspace2.z);

		 normal = lerp( up0,normal , _SnowNormalPower );
 
	#endif

	#if RAIN_ENABLE 
		calc_weather_info(i.posWorld.xyz, normal, n, c0, normal, c0.rgb);
		 

	#endif
  
		half3 viewDir = normalize(UnityWorldSpaceViewDir(i.posWorld));
		half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
		fixed3 lightColor = _LightColor0;
 
#if S_DEVELOP
		 
		half4 var_specMapC = tex2D(_SpecMap, i.uv);
		half var_specMap = var_specMapC.r * _SpPower;
		half4 var_glossMapC = tex2D(_GlossMap, i.uv);
		half var_glossMap = var_glossMapC.r ;
		
#else
		
		half var_specMap = var_CtrlTex.r * _SpPower;
		half var_glossMap = var_CtrlTex.g;
#endif
		 
		
		
	
	
	_Gloss *= 1.0 - var_glossMap;


	#if RAIN_ENABLE  
		 
		_Gloss = saturate(_Gloss* get_smoothnessRate());

	#endif

	#if(SNOW_ENABLE)
		  
		 _Gloss = lerp(_Gloss,_SnowGloss,nt);
		 
		 
	#endif


 
	fixed3 specColor = _Spec ;
 
//#endif
	
	float perceptualRoughness = 1.0 - _Gloss;
	
	float roughness = perceptualRoughness * perceptualRoughness;				
	half3 reflDir = reflect(viewDir, normal);

	half nl = saturate(dot(normal, lightDir));
	half nv = saturate(dot(normal, viewDir));
	half2 rlPow4AndFresnelTerm = Pow4(half2(dot(reflDir, lightDir), 1 - nv));
	half rlPow4 = rlPow4AndFresnelTerm.x;
	half LUT_RANGE = 16.0 * step(0.001, nl);

	float3 halfDirection = normalize(viewDir + lightDir);
	float LdotH = saturate(dot(lightDir, halfDirection));
	float NdotH = saturate(dot(normal, halfDirection));

	float NdotV = abs(dot(normal, viewDir));

	float specular = ArmBRDF(roughness, NdotH, LdotH);
	#ifdef UNITY_COLORSPACE_GAMMA
		specular = sqrt(max(1e-4h, specular));
	#endif

	specular = max(0, specular * nl)*var_specMap;
 
//#if GLOBAL_SH9
//	fixed3 ambient = g_sh(half4(normal, 1))* c0.rgb;
//#else
	fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT * c0.rgb;
//#endif
	half nl0 = saturate(dot(normal, -lightDir));



	fixed3 diffuse = ambient + lightColor * lerp(_DifSC,1, nl) * c0.rgb + _BackColor.rgb *nl0  ;

	fixed3 spec = lightColor * specular * specColor;

#if S_BAKE

#else
float sp = 1;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
	fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2));
	diffuse *= lm;

	//if(lm.r<0.6)
	//	return float4(1,0,0,1);
	//sp *= step( 0.25, lm);
 
	sp *= smoothstep(0.2+_CullSepe,0.35+_CullSepe, lm);
#endif

#endif


#if defined(_ISMEMISSION_ON)||defined(_ISMETALLIC_ON) 
	#if S_DEVELOP
	half4 metallic_ctrl_texC = tex2D(metallic_ctrl_tex, i.uv);
	half _e = metallic_ctrl_texC.r;
	half _m = _e  *  metallic_power;
#else
	half _e = var_CtrlTex.a;
	half _m = var_CtrlTex.a*metallic_power;	
#endif

#endif


#if _ISMETALLIC_ON
	//金属 .  
	half3 viewReflectDirection = reflect(-viewDir, normal);
	
	half4 skyUV = half4(ToRadialCoords(viewReflectDirection),0, roughness*6);
	fixed4 localskyColor = tex2Dlod(environment_reflect, skyUV) ;
 
	fixed3 skyColor =  localskyColor.xyz * exp2(localskyColor.w * 14.48538f - 3.621346f);

	   


 
	skyColor *= c_base.rgb* _m;
	skyColor += ArmBRDFEnv(roughness, NdotV);
	skyColor *= metallic_color.rgb;
 
	diffuse *=   (1 - _m);
	spec  += skyColor;

#endif 







#if S_BAKE

#else
	spec *= sp;
#endif


	c.rgb = diffuse + spec ;

#ifdef _ISMEMISSION_ON
	c.rgb += c0.rgb*_Emission*_e;
#endif
	

#ifdef SHADOW_ON
	fixed shadow = 0;
	for (fixed j = -0.5; j <= 0.5; j += 1) {
		for (fixed k = -0.5; k <= 0.5; k += 1) {
			shadow += tex2D(_Shadow, i.shadow_uv + _Shadow_TexelSize.xy * float2(j, k)).r;
		}
	}
	shadow /= 4;

	fixed fade = tex2D(_ShadowFade, i.shadow_uv).r;
	shadow *= fade * _ShadowStrength;
	c.rgb = fixed3(0, 0, 0) * shadow + c.rgb * (1 - shadow);
#endif 

 
#if BRIGHTNESS_ON
	c.rgb = c.rgb * _Brightness * 2;
#endif



	// apply fog
	UNITY_APPLY_FOG(i.fogCoord, c);
	return c;
	}
		ENDCG
	}
	}
	CustomEditor "DiffuseAOSPGUI"
}
