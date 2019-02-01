#ifndef DIFFUSEINFO_INCLUDED
#define DIFFUSEINFO_INCLUDED

#include "Lighting.cginc"
#include "UnityCG.cginc"

#define INSTANCING_PROP_FakeValue UNITY_DEFINE_INSTANCED_PROP(float4, _FakeValue)
#define INSTANCING_PROP_UVST UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
#define INSTANCING_PROP_UVST_BM UNITY_DEFINE_INSTANCED_PROP(float4, _BumpMap_ST)
#define INSTANCING_PROP_UVST_MT UNITY_DEFINE_INSTANCED_PROP(float4, _Metallic_ST)
#define INSTANCING_PROP_UVST_EM UNITY_DEFINE_INSTANCED_PROP(float4, _EmssionMap_ST)
#define INSTANCING_PROP_UVST_SELECT_ID UNITY_DEFINE_INSTANCED_PROP(float4, _Select_id)

#define INSTANCING_PROP_LeftVector_0 UNITY_DEFINE_INSTANCED_PROP(float4, LeftVector_0)
#define INSTANCING_PROP_LeftVector_1 UNITY_DEFINE_INSTANCED_PROP(float4, LeftVector_1)
#define INSTANCING_PROP_LeftVector_2 UNITY_DEFINE_INSTANCED_PROP(float4, LeftVector_2)

#define INSTANCING_PROP_RightVector_0 UNITY_DEFINE_INSTANCED_PROP(float4, RightVector_0)
#define INSTANCING_PROP_RightVector_1 UNITY_DEFINE_INSTANCED_PROP(float4, RightVector_1)
#define INSTANCING_PROP_RightVector_2 UNITY_DEFINE_INSTANCED_PROP(float4, RightVector_2)

#define 	INSTANCING_PROP_RoadLine_Color UNITY_DEFINE_INSTANCED_PROP(float4, _RoadLine_Color)
#define 	INSTANCING_PROP_RoadLine_SegSpace UNITY_DEFINE_INSTANCED_PROP(float4, _RoadLine_Segmen_Space)

#define TRANSFORM_TEX_INSTANC(tex) (tex.xy * UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _MainTex_ST).xy + UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _MainTex_ST).zw)
#define TRANSFORM_TEX_INSTANC_BM(tex) (tex.xy * UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _BumpMap_ST).xy + UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _BumpMap_ST).zw)
#define TRANSFORM_TEX_INSTANC_MT(tex) (tex.xy * UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _Metallic_ST).xy + UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _Metallic_ST).zw)
#define TRANSFORM_TEX_INSTANC_EM(tex) (tex.xy * UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _EmssionMap_ST).xy + UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _EmssionMap_ST).zw)
#define _SELECT_ID UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _Select_id)

#define MainTex_ST_Ins UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _MainTex_ST)

#define LeftVector_ins_0 UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, LeftVector_0)
#define LeftVector_ins_1 UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, LeftVector_1)
#define LeftVector_ins_2 UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, LeftVector_2)

#define RightVector_ins_0 UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, RightVector_0)
#define RightVector_ins_1 UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, RightVector_1)
#define RightVector_ins_2 UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, RightVector_2)

#define RoadLine_Color_Ins UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _RoadLine_Color)
#define RoadLine_SegSpace_Ins UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _RoadLine_Segmen_Space)

#define INSTANCING_START\
 UNITY_INSTANCING_BUFFER_START(INSTANC_BUF)\
 INSTANCING_PROP_FakeValue

#define INSTANCING_END UNITY_INSTANCING_BUFFER_END(INSTANC_BUF)


uniform float3 _CharacterLightPos;
uniform fixed3 _CharacterLightColor;
uniform half _CharacterLightRange;
uniform half _CharacterLightIntensity;

uniform fixed3 DirectionLightDir0;
uniform fixed3 DirectionLightDir1;
uniform fixed3 DirectionLightDir2;

uniform fixed3 DirectionLightColor0;
uniform fixed3 DirectionLightColor1;
uniform fixed3 DirectionLightColor2;

uniform fixed DirectionLightIntensity0;
uniform fixed DirectionLightIntensity1;
uniform fixed DirectionLightIntensity2;

uniform fixed3 NegativeLightColor0;
uniform fixed  NegativeLightIntensity;

uniform fixed3 DirectionLight;
uniform fixed3 GroundEnvirment;
uniform fixed3 GroundEnvirWithoutSky;

uniform fixed3 EnvimentColor;
uniform fixed3 _MidDiffuseColor;

uniform samplerCUBE  Envirment_Cubemap;
uniform samplerCUBE  Building_Cubemap;

float3 BlinnPhong(float3 normalDir, float spIntensity, float4 MetallicTex, fixed3 V, fixed _BP_Gloss) {
	float3 L = normalize(-DirectionLightDir0);
	float3 N = normalize(normalDir);
	float3 H = normalize(L + V);
	float3 F = saturate(dot(N, H));
	float specPow = exp2(_BP_Gloss * MetallicTex.a * 10 + 1);
	float3 fianl = max(0, pow(F, specPow)*spIntensity * MetallicTex.r);
	return fianl;
}

fixed3 GetEnvirmentColor(fixed3 normal)
{
	//float ndotl = dot(normal, DirectionLightDir0);
	//float negative = step(ndotl, 0);
	//float ndotl_abs = abs(ndotl);

	return 	texCUBE(Envirment_Cubemap, normal).xyz + DirectionLightColor1 * saturate(dot(normal, DirectionLightDir1)) +
		DirectionLightColor2 * saturate(dot(normal, DirectionLightDir2)) +
		DirectionLightColor0 * DirectionLightIntensity0 * saturate(dot(normal, DirectionLightDir0));

//#ifdef USING_SKYLIGHT
//	return texCUBE(Envirment_Cubemap, normal).xyz +
//		NegativeLightColor0 * NegativeLightIntensity * ndotl_abs * negative + DirectionLightColor0 * 
//		DirectionLightIntensity0 * ndotl_abs * (1 - negative);
//#else
//	return NegativeLightColor0 * NegativeLightIntensity * ndotl_abs * negative + DirectionLightColor0 *
//		DirectionLightIntensity0 * ndotl_abs * (1 - negative);
//#endif
}

inline float3 PBRSpecular(float3 normalDirection, float3 viewDirection,float3 _Metallic_var,float _Metallic_alpha,float _Metallic_red,
	float3 _MainTex_var, float3 _DiffCubemap, float3 _SpecCubemap)
{

	float3 lightDirection = DirectionLightDir0;
	
    float3 halfDirection = normalize(viewDirection+lightDirection);
    float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
	float ndotl = dot(normalDirection, lightDirection);
	float NdotL = saturate(ndotl);
    float LdotH = saturate(dot(lightDirection, halfDirection));
    float3 specularColor =  _Metallic_red;
	float3 lightColor = DirectionLightColor0 * DirectionLightIntensity0* NdotL;
    float3 gloss =_Metallic_alpha;
    float perceptualRoughness = 1.0 -  (_Metallic_alpha);
    float roughness = perceptualRoughness* perceptualRoughness;
     float specularMonochrome;
     float3 diffuseColor = _MainTex_var;
    diffuseColor = DiffuseAndSpecularFromMetallic( _MainTex_var, specularColor, specularColor, specularMonochrome );
    specularMonochrome = 1.0-specularMonochrome;
    float NdotV = abs(dot( normalDirection, viewDirection ));
    float NdotH = saturate(dot( normalDirection, halfDirection));

    float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
    float normTerm = GGXTerm(NdotH, roughness);


    float specularPBL = (visTerm*normTerm) * UNITY_PI;
	#ifdef UNITY_COLORSPACE_GAMMA
		specularPBL = sqrt(max(1e-4h, specularPBL));
	#endif
                
	specularPBL = max(0, specularPBL * NdotL);
    #if defined(_SPECULARHIGHLIGHTS_OFF)
		specularPBL = 0.0;
	#endif
    
	half surfaceReduction;
	#ifdef UNITY_COLORSPACE_GAMMA
		surfaceReduction = 1.0 - 0.28 * roughness * perceptualRoughness;
	#else
		surfaceReduction = 1.0/(roughness*roughness + 1.0);
    #endif

	specularPBL *= any(specularColor) ? 1.0 : 0.0;
	float3 directSpecular = DirectionLightColor0*DirectionLightIntensity0*specularPBL*FresnelTerm(specularColor, LdotH);

	half grazingTerm = saturate( gloss + specularMonochrome );
	float3 indirectSpecular = (( _SpecCubemap.rgb)  *_Metallic_red);
 
                    
    indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
    indirectSpecular *= surfaceReduction;

	#ifdef BAKEMOD_ON
		float3 specular = directSpecular ;
	#else
		float3 specular = (directSpecular + indirectSpecular);
	#endif
    
                  
 
	half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
    float nlPow5 = Pow5(1-NdotL);
    float nvPow5 = Pow5(1-NdotV);
    float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * DirectionLightColor0 * DirectionLightIntensity0;               
 
	float negative = step(ndotl, 0);//EnvimentColor //*_MainTex_var.rgb
	float ndotl_abs = abs(ndotl);
	float3  newdiff = (  directDiffuse + _DiffCubemap.rgb * NdotL * DirectionLightColor0 * DirectionLightIntensity0 + NegativeLightColor0 * NegativeLightIntensity * ndotl_abs * negative + 
		texCUBE(Envirment_Cubemap, normalDirection).xyz)  *diffuseColor ;
	

	return  newdiff  + specular ;
}


inline float3 PBRSimple(float3 normalDirection, float3 _MainTex_var, float3 _DiffCubemap)
{
    float3 _SpecCubemap = 0;
	float _Metallic_alpha = 1;
	float _Metallic_red = 0;
	float3 lightDirection = DirectionLightDir0;
	
	float ndotl = dot(normalDirection, lightDirection);
	float NdotL = saturate(ndotl);
    float3 specularColor =  _Metallic_red;
	float3 lightColor = DirectionLightColor0 * DirectionLightIntensity0* NdotL;

     float specularMonochrome;
     float3 diffuseColor = _MainTex_var;
    diffuseColor = DiffuseAndSpecularFromMetallic( _MainTex_var, specularColor, specularColor, specularMonochrome );
 
    // 微面元遮挡函数G
    //float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                     
 
 	float3 directDiffuse =  NdotL * DirectionLightColor0 * DirectionLightIntensity0;
	float negative = step(ndotl, 0);//EnvimentColor //*_MainTex_var.rgb
	float ndotl_abs = abs(ndotl);
	float3  newdiff = (  directDiffuse + _DiffCubemap.rgb + NegativeLightColor0 * NegativeLightIntensity * ndotl_abs * negative + 
		texCUBE(Envirment_Cubemap, normalDirection).xyz)  *diffuseColor ;
	

	return  newdiff ;
}

inline float3 PBRSimpleUpLight(float3 normalDirection, float3 _MainTex_var, float3 _DiffCubemap)
{
    float3 _SpecCubemap = 0;
	float _Metallic_alpha = 1;
	float _Metallic_red = 0;
	float3 lightDirection = float3(0,-1,0);
	//	float3 lightDirection = DirectionLightDir0;
	float ndotl = dot(normalDirection, lightDirection);
	float NdotL = saturate(ndotl);
    float3 specularColor =  _Metallic_red;
	float3 lightColor = DirectionLightColor0 * DirectionLightIntensity0* NdotL;

     float specularMonochrome;
     float3 diffuseColor = _MainTex_var;
    diffuseColor = DiffuseAndSpecularFromMetallic( _MainTex_var, specularColor, specularColor, specularMonochrome );
 
    // 微面元遮挡函数G
    //float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                     
 
 	float3 directDiffuse =  NdotL * DirectionLightColor0 * DirectionLightIntensity0;
	float negative = step(ndotl, 0);//EnvimentColor //*_MainTex_var.rgb
	float ndotl_abs = abs(ndotl);

	float3  newdiff = (  directDiffuse + _DiffCubemap.rgb + NegativeLightColor0 * NegativeLightIntensity * ndotl_abs  + 
		texCUBE(Envirment_Cubemap, normalDirection).xyz)  *diffuseColor ;
	

	return  newdiff ;
}
 

 
inline half4 Pow3 (half x)
{
    return x*x * x;
}
inline float3 PBRSpecularLow(float3 normalDirection, float3 viewDirection, float3 _Metallic_var, float _Metallic_alpha, float _Metallic_red,
	float3 _MainTex_var, float3 _DiffCubemap, float3 _SpecCubemap)
{

	float3 lightDirection = DirectionLightDir0;

	float3 halfDirection = normalize(viewDirection + lightDirection);
	float3 viewReflectDirection = reflect(-viewDirection, normalDirection);
	float ndotl = dot(normalDirection, lightDirection);
	float NdotL = saturate(ndotl);
	float LdotH = saturate(dot(lightDirection, halfDirection));
	float3 specularColor = _Metallic_red;
	float3 lightColor = DirectionLightColor0 * DirectionLightIntensity0* NdotL;
	float3 gloss = _Metallic_alpha;
	float perceptualRoughness = 1.0 - (_Metallic_alpha);
	float roughness = perceptualRoughness* perceptualRoughness;
	float specularMonochrome;
	float3 diffuseColor = _MainTex_var;
	diffuseColor = DiffuseAndSpecularFromMetallic(_MainTex_var, specularColor, specularColor, specularMonochrome);
	specularMonochrome = 1.0 - specularMonochrome;
	float NdotV = abs(dot(normalDirection, viewDirection));
	float NdotH = saturate(dot(normalDirection, halfDirection));
	float VdotH = saturate(dot(viewDirection, halfDirection));


	half fd90 = 0.5 + 2 * LdotH * LdotH * (1 - gloss);
	float nlPow5 = Pow5(1 - NdotL);
	float nvPow5 = Pow5(1 - NdotV);
	float3 directDiffuse = ((1 + (fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * DirectionLightColor0 * DirectionLightIntensity0;

	float negative = step(ndotl, 0);//EnvimentColor //*_MainTex_var.rgb
	float ndotl_abs = abs(ndotl);
	float3  newdiff = (directDiffuse + _DiffCubemap.rgb + NegativeLightColor0 * NegativeLightIntensity * ndotl_abs * negative +
		texCUBE(Envirment_Cubemap, normalDirection).xyz)  *diffuseColor;


	return  newdiff;
}



inline float3 PBRLow2(float3 normalDirection, float3 viewDirection,float3 _Metallic_var,float _Metallic_alpha,float _Metallic_red,
	float3 _MainTex_var,  float3 _DiffCubemap )
{
	float3 lightDirection = DirectionLightDir0;
    float3 halfDirection = normalize(viewDirection+lightDirection);
    float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
	float NdotL = saturate(dot(normalDirection, lightDirection));
    float LdotH = saturate(dot(lightDirection, halfDirection));
    float3 specularColor =  _Metallic_red;
	float3 lightColor = DirectionLightColor0 * DirectionLightIntensity0* NdotL;
    float3 gloss =_Metallic_alpha;
 
     float specularMonochrome;
     float3 diffuseColor = _MainTex_var;

    diffuseColor = DiffuseAndSpecularFromMetallic( _MainTex_var, specularColor, specularColor, specularMonochrome );
    //return float4(_Metallic_red*_Metallic_red ,0,0,1);
    //金屬部分要反光. 这档lod去掉了高光，金属按金属度简单补偿一个颜色。
    float _a = (1 +_Metallic_red  );
    diffuseColor = diffuseColor *_a*_a;
 	//diffuseColor = diffuseColor + _Metallic_red  *_Metallic_red * 0.7 ;
   
 
    float NdotV = abs(dot( normalDirection, viewDirection ));
    float NdotH = saturate(dot( normalDirection, halfDirection));

 
    //最传统diffuse.
    float3 directDiffuse =  NdotL * DirectionLightColor0 * DirectionLightIntensity0;
 	float3 indirectDiffuse =   _DiffCubemap.rgb;
	float3  newdiff = (  directDiffuse  + indirectDiffuse)  *diffuseColor ;


	return  newdiff    ;
}



float3 PointLight(float4 wPos, float3 wNormal)
{
	float3 lightPos = _CharacterLightPos.xyz;
	fixed3 diffColor = 0;
	half3 toLight = lightPos.xyz - wPos.xyz;
	half lengthSq = dot(toLight, toLight);
	half atten = (1.0 - smoothstep(0, 1, lengthSq / (_CharacterLightRange + 1.0))) * _CharacterLightIntensity;
	half diff = max(0, dot(normalize(wNormal), normalize(toLight)));
	diffColor += _CharacterLightColor.rgb * (diff * atten);
	return diffColor;
}

float3 PointLightWithSpec(float4 wPos, float3 wNormal,float specMask ,float shininess)
{
	fixed3 color = 0;
	half3 normal = normalize(wNormal);
	float3 lightPos = _CharacterLightPos.xyz;
	half3 lightDir = lightPos.xyz - wPos.xyz;
	half3 viewDir = _WorldSpaceCameraPos.xyz - wPos.xyz;

	half3 h = normalize(lightDir + viewDir);
	half nh = max(0, dot(normal, h));
	half spec = pow(nh, shininess);

	half lengthSq = dot(lightDir, lightDir);
	half atten = (1.0 - smoothstep(0, 1, lengthSq / (_CharacterLightRange + 1.0))) * _CharacterLightIntensity;
	half diff = max(0, dot(normal, normalize(lightDir)));
	color = _CharacterLightColor.rgb * ((diff + spec * specMask) * atten);
	return color;
}

float3 DirectionalLightWithSpec(float3 wPos, float3 wNormal, float wrap, float shininess)
{
	float3 viewDir = _WorldSpaceCameraPos.xyz - wPos.xyz;
	float3 nor = normalize(wNormal);
	float3 lightDir = normalize(DirectionLightDir0);

	half3 h = normalize(lightDir + viewDir);
	half nh = max(0, dot(wNormal, h));
	half spec = pow(nh, shininess);

	float3 diff = float3(max(0, (dot(nor, lightDir) + wrap) / (1 + wrap)),
		max(0, (dot(nor, normalize(DirectionLightDir1)) + wrap) / (1 + wrap)),
		max(0, (dot(nor, normalize(DirectionLightDir2)) + wrap) / (1 + wrap)));

	return DirectionLightColor0 * diff[0] + DirectionLightColor1 * diff[1] + DirectionLightColor2 * diff[2];

}

float GetSpec(float3 LightVec, float3 viewDir, float3 normalDirection, float _SpecIBLPower)
{
	float3 h = normalize(LightVec + viewDir);

	float nh = max(0, dot(normalDirection, h));
	return pow(nh, _SpecIBLPower);
}


//三个平行光
float3 DirectionalLight(float3 wNormal, float wrap)
{
	float3 nor = normalize(wNormal);
	float3 lightDir = normalize(DirectionLightDir0);

	float3 diff = float3(max(0, (dot(nor, lightDir) + wrap) / (1 + wrap)),
						 max(0, (dot(nor, normalize(DirectionLightDir1)) + wrap) / (1 + wrap)),
						 max(0, (dot(nor, normalize(DirectionLightDir2)) + wrap) / (1 + wrap)));

	return DirectionLightColor0 * diff[0] + DirectionLightColor1 * diff[1] + DirectionLightColor2 * diff[2];
}


// 通过索引和UV采样
float2 TexUVSplit(half horCount, half verCount, float2 uv0, half startIndex)
{
	startIndex = fmod(startIndex, horCount * verCount);
	half htIndex = floor(fmod(startIndex, horCount));
	half vtIndex = floor(startIndex / horCount);
	return float2((htIndex + uv0.x) / horCount, (vtIndex + uv0.y) / verCount);
}

float2 CalculateBezierValue(float ti, float2 vec0, float2 vec1, float2 vec2)
{
	float minus_ti = 1 - ti;
	return vec0 * ti * ti + vec1 * (2 * ti * minus_ti) + vec2 * minus_ti * minus_ti;
}

float2 CalculateBezierDir(float ti, float2 vec0, float2 vec1, float2 vec2)
{
	return 2 * vec0 * ti + 2 * vec1 - 4 * vec1 * ti - 2 * vec2 *(1 - ti);
}

float3 GetSpecularMaskColor(float3 worldPos, float3 normalDirection, float4 _Metallic_var, float _Gloss,
	float _Specular, float4 _Albedo_var, float _Emission, float4 _Color)
{
	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);

	float3 lightDirection = normalize(DirectionLightDir0);
	float3 halfDirection = normalize(viewDirection + lightDirection);

	///////// Gloss:
	float gloss = (_Metallic_var.a*_Gloss);
	float specPow = exp2(gloss * 10.0 + 1.0);
	////// Specular:
	float NdotL = saturate(dot(normalDirection, lightDirection));
	float node_5677 = (_Metallic_var.r  *_Specular);
	float3 specularColor = float3(node_5677, node_5677, node_5677);
	float3 directSpecular = DirectionLightColor0 * pow(max(0, dot(halfDirection, normalDirection)), specPow)*specularColor;
	float3 specular = directSpecular;
	/////// Diffuse:
	NdotL = max(0.0, dot(normalDirection, lightDirection));
	float3 directDiffuse = max(0.0, NdotL) * DirectionLightColor0;
	float3 diffuseColor = (_Albedo_var.rgb*_Color.rgb);
	float3 diffuse = directDiffuse * diffuseColor;
	/// Final Color:
	float3 F = diffuse + specular + _Emission;
	return F;
}

#endif

