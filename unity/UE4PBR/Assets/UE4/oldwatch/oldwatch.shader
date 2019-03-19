//经验模型


Shader "Physically-Based-Lighting" {
    Properties {
    _ExposeColor("_ExposeColor",Vector) = (1,1,1,1)
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex("Albedo-漫射贴图",2D) = "White"{}
    _BumpMap("Normal-法线贴图",2D) = "bump"{}
    _MetallicMap("MetallicMap",2D) = "black"{}
    _GlossMap("_GlossMap",2D) = "White"{}
     _SpecularMap("_SpecularMap",2D) = "White"{}
    _EmissionMap("_EmissionMap",2D) = "White"{}

    _IBL("IBL",Cube) =  "Sky"{}
    _SPL("SPL",Cube) =  "Sky"{}
    _SpecularColor ("Specular Color", Color) = (1,1,1,1)
    _SpecularPower("Specular Power", Range(0,10)) = 1
    _SpecularRange("Specular Gloss",  Range(0,40)) = 0
    _Metallic("Metallicness",Range(0,1)) = 1
    _Glossiness("Smoothness",Range(0,1)) = 1
    _Anisotropic("Anisotropic",  Range(-20,1)) = 0
    _Ior("Ior",  Range(1,4)) = 1.5
     _EmissionIntensity("EmissionIntensity",  Range(0,3)) = 0
    _UnityLightingContribution("Unity Reflection Contribution", Range(0,1)) = 1
    _AmbientLight("_AmbientLight", Color) = (1,1,1,1)
    IBL_Blur("IBL_Blur",  Range(0,40)) = 0
    IBL_Intensity("IBL", Range(0, 20)) = 1 
    SBL_Intensity("SPL", Range(0, 20)) = 1 
    [KeywordEnum(Gloss_R,Metallic_A)] _MetallicChannel("Metallic_Channel;", Float) = 0

    [KeywordEnum(BlinnPhong,Phong,Beckmann,Gaussian,GGX,TrowbridgeReitz,TrowbridgeReitzAnisotropic, Ward,U3DBlinnPhong)] _NormalDistModel("Normal Distribution Model;", Float) = 0
    [KeywordEnum(AshikhminShirley,AshikhminPremoze,Duer,Neumann,Kelemen,ModifiedKelemen,Cook,Ward,Kurt)]_GeoShadowModel("Geometric Shadow Model;", Float) = 0
    [KeywordEnum(None,Walter,Beckman,GGX,Schlick,SchlickBeckman,SchlickGGX,GeometrySchlickGGX,Implicit)]_SmithGeoShadowModel("Smith Geometric Shadow Model; None if above is Used;", Float) = 0
    [KeywordEnum(Schlick,SchlickIOR, SphericalGaussian,SCHICHU3D)]_FresnelModel("Normal Distribution Model;", Float) = 0
    [Toggle] _ENABLE_NDF ("Normal Distribution正态分布", Float) = 0
    [Toggle] _ENABLE_G ("Geometric Shadow", Float) = 0
    [Toggle] _ENABLE_F ("Fresnel菲涅尔", Float) = 0
    [Toggle] _ENABLE_D ("Diffuse漫射", Float) = 0
    
    [Toggle] _ENABLE_GAMMA ("允许伽马校正", Float) = 0
     _gama("伽马校正(2.2线性,1.5经验,1原型)", Range(0,2.2)) = 2.2

    }
    SubShader {
    Tags { 
            "RenderType"="Opaque"  "Queue"="Geometry"
        } 
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "../CGIncludes/RolantinCG.cginc"
		    #include "../CGIncludes/RolantinCORE.cginc"

			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#define _GLOSSYENV 1
			#pragma multi_compile_fwdbase_fullshadows
			#pragma multi_compile ENABLEGI_ON ENABLEGI_OFF
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON  
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile _MetallicChannel_Gloss_R _MetallicChannel_Metallic_A 

            #pragma multi_compile _NORMALDISTMODEL_BLINNPHONG _NORMALDISTMODEL_PHONG _NORMALDISTMODEL_BECKMANN _NORMALDISTMODEL_GAUSSIAN _NORMALDISTMODEL_GGX _NORMALDISTMODEL_TROWBRIDGEREITZ _NORMALDISTMODEL_TROWBRIDGEREITZANISOTROPIC _NORMALDISTMODEL_WARD _NORMALDISTMODEL_U3DBLINNPHONG
            #pragma multi_compile _GEOSHADOWMODEL_ASHIKHMINSHIRLEY _GEOSHADOWMODEL_ASHIKHMINPREMOZE _GEOSHADOWMODEL_DUER_GEOSHADOWMODEL_NEUMANN _GEOSHADOWMODEL_KELEMAN _GEOSHADOWMODEL_MODIFIEDKELEMEN _GEOSHADOWMODEL_COOK _GEOSHADOWMODEL_WARD _GEOSHADOWMODEL_KURT 
            #pragma multi_compile _SMITHGEOSHADOWMODEL_NONE _SMITHGEOSHADOWMODEL_WALTER _SMITHGEOSHADOWMODEL_BECKMAN _SMITHGEOSHADOWMODEL_GGX _SMITHGEOSHADOWMODEL_SCHLICK _SMITHGEOSHADOWMODEL_SCHLICKBECKMAN _SMITHGEOSHADOWMODEL_SCHLICKGGX _SMITHGEOSHADOWMODEL_GeometrySchlickGGX _SMITHGEOSHADOWMODEL_IMPLICIT
            #pragma multi_compile _FRESNELMODEL_SCHLICK _FRESNELMODEL_SCHLICKIOR _FRESNELMODEL_SPHERICALGAUSSIAN _FRESNELMODEL_SCHICHU3D 
            #pragma multi_compile  _ENABLE_NDF_OFF _ENABLE_NDF_ON
            #pragma multi_compile  _ENABLE_G_OFF _ENABLE_G_ON
            #pragma multi_compile  _ENABLE_F_OFF _ENABLE_F_ON
            #pragma multi_compile  _ENABLE_D_OFF _ENABLE_D_ON
            #pragma multi_compile  _ENABLE_GAMMA_OFF _ENABLE_GAMMA_ON
            #pragma target 3.0
           // #pragma only_renderers gles3 
            
float4 _Color;
float4 _ExposeColor;
float4 _SpecularColor;
float _SpecularPower;
float _SpecularRange;
float _Glossiness;
float _Metallic;
float _Anisotropic;
float _Ior;
float _NormalDistModel;
float _GeoShadowModel;
float _FresnelModel;
float _UnityLightingContribution;
float _gama;
float _EmissionIntensity;

samplerCUBE _IBL;
samplerCUBE _SPL;

sampler2D _EmissionMap;fixed4	_EmissionMap_ST;
sampler2D _GlossMap;fixed4	_GlossMap_ST;
sampler2D _MainTex;fixed4 _MainTex_ST;
sampler2D _BumpMap;fixed4 _BumpMap_ST;
sampler2D _MetallicMap;fixed4 _MetallicMap_ST;
sampler2D _SpecularMap; fixed4 _SpecularMap_ST;

struct VertexInput {
    float4 vertex : POSITION;       //local vertex position
    float3 normal : NORMAL;         //normal direction
    float4 tangent : TANGENT;       //tangent direction    
    float2 texcoord0 : TEXCOORD0;
    float2 texcoord1 : TEXCOORD1;
    float2 texcoord2 : TEXCOORD2;

};

struct VertexOutput {
    float4 pos : SV_POSITION;              //screen clip space position and depth
    float2 uv0 : TEXCOORD0;
    float2 uv1 : TEXCOORD1;
    float2 uv2 : TEXCOORD2;

//below we create our own variables with the texcoord semantic. 
    float3 normalDir : TEXCOORD3;         
    float3 posWorld : TEXCOORD4;        
    float3 tangentDir : TEXCOORD5;
    float3 bitangentDir : TEXCOORD6;
    LIGHTING_COORDS(7,8)                   //this initializes the unity lighting and shadow
    UNITY_FOG_COORDS(9)                    //this initializes the unity fog

	//using unity gi system
	#if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
	float4 ambientOrLightmapUV : TEXCOORD10;
	#endif
	
};

VertexOutput vert (VertexInput v) {
     VertexOutput o = (VertexOutput)0;           
	o.uv0 = v.texcoord0;
	o.uv1 = v.texcoord1;
	o.uv2 = v.texcoord2;

	#ifdef LIGHTMAP_ON
	    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	    o.ambientOrLightmapUV.zw = 0;
	#elif UNITY_SHOULD_SAMPLE_SH
	#endif
	#ifdef DYNAMICLIGHTMAP_ON
	    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
	#endif

     o.normalDir = UnityObjectToWorldNormal(v.normal);
     o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
     o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
     o.pos = UnityObjectToClipPos(v.vertex);
     o.posWorld = mul(unity_ObjectToWorld, v.vertex);
	
     UNITY_TRANSFER_FOG(o,o.pos);
     TRANSFER_VERTEX_TO_FRAGMENT(o)
     return o;
}



float4 frag(VertexOutput i) : COLOR {

      posWorld = i.posWorld;
//normal direction calculations
      i.normalDir = normalize(i.normalDir);
   //   posWorld = i.posWorld;
     fixed4 AlbedoTex =  tex2D(_MainTex,i.uv0)*_ExposeColor.b;
	 fixed4 SpecTex = tex2D(_SpecularMap, i.uv0);
     float3 BumpMap =UnpackNormal( tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
     fixed4 MetallicMap = tex2D(_MetallicMap,i.uv0);
     fixed4 EmissionMap = tex2D(_EmissionMap, TRANSFORM_TEX( i.uv0,_EmissionMap));
     MetallicMap.r=MetallicMap.r*_ExposeColor.r;
     MetallicMap.a=MetallicMap.a*_ExposeColor.a;
     float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
     float3 normalDirection = normalize(mul( BumpMap, tangentTransform ));
     float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
     float shiftAmount = dot(i.normalDir, viewDirection);
     normalDirection = shiftAmount < 0.0f ? normalDirection + viewDirection * (-shiftAmount + 1e-5f) : normalDirection;

//light calculations
     float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
     float3 lightReflectDirection = reflect( -lightDirection, normalDirection );
     float3 viewReflectDirection = normalize(reflect( -viewDirection, normalDirection ));
     float NdotL = max(0.0, dot( normalDirection, lightDirection ));
     float3 halfDirection = normalize(viewDirection+lightDirection); 
     float NdotH =  max(0.0,dot( normalDirection, halfDirection));
     float NdotV =  max(0.0,dot( normalDirection, viewDirection));
     float VdotH = max(0.0,dot( viewDirection, halfDirection));
     float LdotH =  max(0.0,dot(lightDirection, halfDirection)); 
     float LdotV = max(0.0,dot(lightDirection, viewDirection)); 
     float RdotV = max(0.0, dot( lightReflectDirection, viewDirection ));
     float attenuation = LIGHT_ATTENUATION(i);
     float3 attenColor = attenuation * _LightColor0.rgb;

     //get Unity Scene lighting data
     //UnityGI gi =  GetUnityGI(_LightColor0.rgb, lightDirection, normalDirection, viewDirection, viewReflectDirection, attenuation, 1- _Glossiness, i.posWorld.xyz);
	
    
    
    #ifdef _MetallicChannel_Gloss_R 
     fixed4 GlossMap = tex2D(_GlossMap,i.uv0);
    _Glossiness = _Glossiness * GlossMap.r * _ExposeColor.a;
    #elif _MetallicChannel_Metallic_A 
    _Glossiness = _Glossiness * MetallicMap.a;
    #endif
     _Metallic= MetallicMap.r * _Metallic;
     //diffuse color calculations
     float roughness = 1-(_Glossiness * _Glossiness);
     roughness = roughness * roughness;
      float f0 = F0(NdotL, NdotV, LdotH, roughness);
     float3 diffuseColor = (_Color.rgb * AlbedoTex + EmissionMap * _EmissionIntensity ) * (1.0 - _Metallic) ;
	 diffuseColor = 0;
	 float gloss_c = _Glossiness;
	 UnityGI gi = GIdata(_LightColor0.rgb, lightDirection, normalDirection, posWorld,viewDirection, attenuation, i.ambientOrLightmapUV, gloss_c, viewReflectDirection);

	 float3 indirectDiffuse = gi.indirect.diffuse.rgb*IBL_Intensity*_AmbientLight;
	 float3 indirectSpecular = gi.indirect.specular.rgb*SBL_Intensity;

	 

	 float3 SPLMap = SBL(_SPL, normalDirection, MetallicMap.r);
	 float3 IBLMap = IBL(_IBL, normalDirection);

	 //redefine
   // float3 indirectDiffuse =IBLMap ;

	//float3 indirectSpecular = SPLMap;
    
   // diffuseColor *= (f0 );
    // diffuseColor+=indirectDiffuse;

    //Specular calculations

     float3 specColor = lerp(_SpecularColor.rgb*SpecTex*_SpecularPower, _Color.rgb, _Metallic * 0.5) ;
     //return float4  (diffuseColor,1);

     float3 SpecularDistribution = specColor;
     float GeometricShadow = 1;
     float3 FresnelFunction = specColor;

     //Normal Distribution Function/Specular Distribution-----------------------------------------------------        
           
    #ifdef _NORMALDISTMODEL_BLINNPHONG 
         SpecularDistribution *=  BlinnPhongNormalDistribution(NdotH, _Glossiness,  max(1,_Glossiness * 40));
    #elif _NORMALDISTMODEL_U3DBLINNPHONG
	     SpecularDistribution *=  BlinnPhongU3D(NdotH, MetallicMap.a *specColor * _SpecularRange,  exp2( _Glossiness * 10.0 + 1.0 )) * _Glossiness ;
    #elif _NORMALDISTMODEL_PHONG
         SpecularDistribution *=  PhongNormalDistribution(RdotV, _Glossiness, max(1,_Glossiness * 40));
    #elif _NORMALDISTMODEL_BECKMANN
         SpecularDistribution *=  BeckmannNormalDistribution(roughness, NdotH);
    #elif _NORMALDISTMODEL_GAUSSIAN
         SpecularDistribution *=  GaussianNormalDistribution(roughness, NdotH);
    #elif _NORMALDISTMODEL_GGX
         SpecularDistribution *=  GGXNormalDistribution(roughness, NdotH);
    #elif _NORMALDISTMODEL_TROWBRIDGEREITZ
         SpecularDistribution *=  TrowbridgeReitzNormalDistribution(NdotH, roughness);
    #elif _NORMALDISTMODEL_TROWBRIDGEREITZANISOTROPIC
         SpecularDistribution *=  TrowbridgeReitzAnisotropicNormalDistribution(_Anisotropic,NdotH, dot(halfDirection, i.tangentDir), dot(halfDirection,  i.bitangentDir));
    #elif _NORMALDISTMODEL_WARD
         SpecularDistribution *=  WardAnisotropicNormalDistribution(_Anisotropic,NdotL, NdotV, NdotH, dot(halfDirection, i.tangentDir), dot(halfDirection,  i.bitangentDir));
    #else
        SpecularDistribution *=  GGXNormalDistribution(roughness, NdotH);
    #endif

     //Geometric Shadowing term----------------------------------------------------------------------------------
    #ifdef _SMITHGEOSHADOWMODEL_NONE
        #ifdef _GEOSHADOWMODEL_ASHIKHMINSHIRLEY
            GeometricShadow *= AshikhminShirleyGeometricShadowingFunction (NdotL, NdotV, LdotH);
        #elif _GEOSHADOWMODEL_ASHIKHMINPREMOZE
            GeometricShadow *= AshikhminPremozeGeometricShadowingFunction (NdotL, NdotV);
        #elif _GEOSHADOWMODEL_DUER
            GeometricShadow *= DuerGeometricShadowingFunction (lightDirection, viewDirection, normalDirection, NdotL, NdotV);
        #elif _GEOSHADOWMODEL_NEUMANN
            GeometricShadow *= NeumannGeometricShadowingFunction (NdotL, NdotV);
        #elif _GEOSHADOWMODEL_KELEMAN
            GeometricShadow *= KelemenGeometricShadowingFunction (NdotL, NdotV, LdotH,  VdotH);
        #elif _GEOSHADOWMODEL_MODIFIEDKELEMEN
            GeometricShadow *=  ModifiedKelemenGeometricShadowingFunction (NdotV, NdotL, roughness);
        #elif _GEOSHADOWMODEL_COOK
            GeometricShadow *= CookTorrenceGeometricShadowingFunction (NdotL, NdotV, VdotH, NdotH);
        #elif _GEOSHADOWMODEL_WARD
            GeometricShadow *= WardGeometricShadowingFunction (NdotL, NdotV, VdotH, NdotH);
        #elif _GEOSHADOWMODEL_KURT
            GeometricShadow *= KurtGeometricShadowingFunction (NdotL, NdotV, VdotH, roughness);
        #else           
            GeometricShadow *= ImplicitGeometricShadowingFunction (NdotL, NdotV);
        #endif
    ////SmithModelsBelow
    ////Gs = F(NdotL) * F(NdotV);
    #elif _SMITHGEOSHADOWMODEL_WALTER
        GeometricShadow *= WalterEtAlGeometricShadowingFunction (NdotL, NdotV, roughness);
    #elif _SMITHGEOSHADOWMODEL_BECKMAN
        GeometricShadow *= BeckmanGeometricShadowingFunction (NdotL, NdotV, roughness);
    #elif _SMITHGEOSHADOWMODEL_GGX
        GeometricShadow *= GGXGeometricShadowingFunction (NdotL, NdotV, roughness);
    #elif _SMITHGEOSHADOWMODEL_SCHLICK
        GeometricShadow *= SchlickGeometricShadowingFunction (NdotL, NdotV, roughness);
    #elif _SMITHGEOSHADOWMODEL_SCHLICKBECKMAN
        GeometricShadow *= SchlickBeckmanGeometricShadowingFunction (NdotL, NdotV, roughness);
    #elif _SMITHGEOSHADOWMODEL_SCHLICKGGX
        GeometricShadow *= SchlickGGXGeometricShadowingFunction (NdotL, NdotV, roughness);
    #elif _SMITHGEOSHADOWMODEL_IMPLICIT
        GeometricShadow *= ImplicitGeometricShadowingFunction (NdotL, NdotV);
    #elif  _SMITHGEOSHADOWMODEL_GeometrySchlickGGX 
        GeometricShadow *= GeometrySchlickGGX(NdotL, NdotV,roughness);
    #else
        GeometricShadow *= ImplicitGeometricShadowingFunction (NdotL, NdotV);
    #endif
     //Fresnel Function-------------------------------------------------------------------------------------------------

    
    #ifdef _FRESNELMODEL_SCHLICK
        //return float4(1,1,0,1);
        FresnelFunction *=  SchlickFresnelFunction(specColor, LdotH);
    #elif _FRESNELMODEL_SCHLICKIOR
    //  return float4(1,0,1,1);
        FresnelFunction *=  SchlickIORFresnelFunction(_Ior, LdotH);
    #elif _FRESNELMODEL_SPHERICALGAUSSIAN
    //  return float4(0,0,1,1);
        FresnelFunction *= SphericalGaussianFresnelFunction(LdotH, f0);
    #elif _FRESNELMODEL_SCHICHU3D
        //return float4(1,1,1,1);
        FresnelFunction = F_Schlick_U3D(NdotV,specColor);
    #else
        FresnelFunction *=  SchlickIORFresnelFunction(_Ior, LdotH); 
    #endif




    float3 DebugColor= float3(1,1,1);
    #ifdef _ENABLE_NDF_ON
     return float4(DebugColor* SpecularDistribution,1);
    #endif
    #ifdef _ENABLE_G_ON 
     return float4(DebugColor * GeometricShadow,1) ;
    #endif
    #ifdef _ENABLE_F_ON 
     return float4(DebugColor* FresnelFunction,1);
    #endif
    #ifdef _ENABLE_D_ON 
     return float4(DebugColor* diffuseColor,1);
    #endif

     //PBR
      float grazingTerm = saturate(roughness + _Metallic);
     float3 specularity = (SpecularDistribution * FresnelFunction * GeometricShadow ) / (4 * (  NdotL * NdotV));
    
     float3 unityIndirectSpecularity =  indirectSpecular  * _Metallic ; //* FresnelLerp(specColor,grazingTerm,NdotV)
 

     float3 lightingModel = (((diffuseColor + saturate(specularity)) + ( (_Metallic) * unityIndirectSpecularity )))* attenColor;
	// return float4(lightingModel, 1);
  //return  float4    (lightingModel,1);
     lightingModel *= NdotL;
     float4 finalDiffuse = float4(lightingModel  + indirectDiffuse * AlbedoTex,1);


  //gama校正    
	#ifdef _ENABLE_GAMMA_ON
	  finalDiffuse.rgb = saturate( GammaCorrection( finalDiffuse.rgb,_gama));
	#endif
     //UNITY_APPLY_FOG(i.fogCoord, finalDiffuse);
    return finalDiffuse;
}
ENDCG
}



 Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
					 CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define UNITY_PASS_FORWARDBASE

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"
			 #include "../CGIncludes/RolantinCG.cginc"

			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
				#include "../CGIncludes/RolantinCORE.cginc"
				#define _GLOSSYENV 1
			#pragma multi_compile_fwdbase_fullshadows
			#pragma multi_compile ENABLEGI_ON ENABLEGI_OFF
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON  
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

			#pragma multi_compile_fwdbase_fullshadows
			#pragma multi_compile _MetallicChannel_Gloss_R _MetallicChannel_Metallic_A 

			#pragma multi_compile _NORMALDISTMODEL_BLINNPHONG _NORMALDISTMODEL_PHONG _NORMALDISTMODEL_BECKMANN _NORMALDISTMODEL_GAUSSIAN _NORMALDISTMODEL_GGX _NORMALDISTMODEL_TROWBRIDGEREITZ _NORMALDISTMODEL_TROWBRIDGEREITZANISOTROPIC _NORMALDISTMODEL_WARD _NORMALDISTMODEL_U3DBLINNPHONG
			#pragma multi_compile _GEOSHADOWMODEL_ASHIKHMINSHIRLEY _GEOSHADOWMODEL_ASHIKHMINPREMOZE _GEOSHADOWMODEL_DUER_GEOSHADOWMODEL_NEUMANN _GEOSHADOWMODEL_KELEMAN _GEOSHADOWMODEL_MODIFIEDKELEMEN _GEOSHADOWMODEL_COOK _GEOSHADOWMODEL_WARD _GEOSHADOWMODEL_KURT 
			#pragma multi_compile _SMITHGEOSHADOWMODEL_NONE _SMITHGEOSHADOWMODEL_WALTER _SMITHGEOSHADOWMODEL_BECKMAN _SMITHGEOSHADOWMODEL_GGX _SMITHGEOSHADOWMODEL_SCHLICK _SMITHGEOSHADOWMODEL_SCHLICKBECKMAN _SMITHGEOSHADOWMODEL_SCHLICKGGX _SMITHGEOSHADOWMODEL_GeometrySchlickGGX _SMITHGEOSHADOWMODEL_IMPLICIT
			#pragma multi_compile _FRESNELMODEL_SCHLICK _FRESNELMODEL_SCHLICKIOR _FRESNELMODEL_SPHERICALGAUSSIAN _FRESNELMODEL_SCHICHU3D 
			#pragma multi_compile  _ENABLE_NDF_OFF _ENABLE_NDF_ON
			#pragma multi_compile  _ENABLE_G_OFF _ENABLE_G_ON
			#pragma multi_compile  _ENABLE_F_OFF _ENABLE_F_ON
			#pragma multi_compile  _ENABLE_D_OFF _ENABLE_D_ON
			#pragma multi_compile  _ENABLE_GAMMA_OFF _ENABLE_GAMMA_ON
			#pragma target 3.0
	 // #pragma only_renderers gles3 

float4 _Color;
float4 _ExposeColor;
float4 _SpecularColor;
float _SpecularPower;
float _SpecularRange;
float _Glossiness;
float _Metallic;
float _Anisotropic;
float _Ior;
float _NormalDistModel;
float _GeoShadowModel;
float _FresnelModel;
float _UnityLightingContribution;
float _gama;
float _EmissionIntensity;

samplerCUBE _IBL;
samplerCUBE _SPL;
sampler2D _SpecularMap; fixed4 _SpecularMap_ST;
sampler2D _EmissionMap; fixed4	_EmissionMap_ST;
sampler2D _GlossMap; fixed4	_GlossMap_ST;
sampler2D _MainTex; fixed4 _MainTex_ST;
sampler2D _BumpMap; fixed4 _BumpMap_ST;
sampler2D _MetallicMap; fixed4 _MetallicMap_ST;

struct VertexInput {
	float4 vertex : POSITION;       //local vertex position
	float3 normal : NORMAL;         //normal direction
	float4 tangent : TANGENT;       //tangent direction    
	float2 texcoord0 : TEXCOORD0;
	float2 texcoord1 : TEXCOORD1;
	float2 texcoord2 : TEXCOORD2;

};

struct VertexOutput {
	float4 pos : SV_POSITION;              //screen clip space position and depth
	float2 uv0 : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
	float2 uv2 : TEXCOORD2;

	//below we create our own variables with the texcoord semantic. 
		float3 normalDir : TEXCOORD3;
		float3 posWorld : TEXCOORD4;
		float3 tangentDir : TEXCOORD5;
		float3 bitangentDir : TEXCOORD6;
		LIGHTING_COORDS(7,8)                   //this initializes the unity lighting and shadow
		UNITY_FOG_COORDS(9)                    //this initializes the unity fog

		//using unity gi system

		#if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
		float4 ambientOrLightmapUV : TEXCOORD10;
		#endif

	};

	VertexOutput vert(VertexInput v) {
		 VertexOutput o = (VertexOutput)0;
		o.uv0 = v.texcoord0;
		o.uv1 = v.texcoord1;
		o.uv2 = v.texcoord2;

		#ifdef LIGHTMAP_ON
			o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			o.ambientOrLightmapUV.zw = 0;
		#elif UNITY_SHOULD_SAMPLE_SH
		#endif
		#ifdef DYNAMICLIGHTMAP_ON
			o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
		#endif

		 o.normalDir = UnityObjectToWorldNormal(v.normal);
		 o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
		 o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
		 o.pos = UnityObjectToClipPos(v.vertex);
		 o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		 float3 lightColor = _LightColor0.rgb;
		 UNITY_TRANSFER_FOG(o,o.pos);
		 TRANSFER_VERTEX_TO_FRAGMENT(o)
		 return o;
	}



	float4 frag(VertexOutput i) : COLOR {
		   float3 lightColor = _LightColor0.rgb;
		  posWorld = i.posWorld;
		  //normal direction calculations
				i.normalDir = normalize(i.normalDir);
				//   posWorld = i.posWorld;
				fixed4 SpecTex = tex2D(_SpecularMap, i.uv0);
					fixed4 AlbedoTex = tex2D(_MainTex,i.uv0)*_ExposeColor.b;
				  float3 BumpMap = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
				  fixed4 MetallicMap = tex2D(_MetallicMap,i.uv0);
				  fixed4 EmissionMap = tex2D(_EmissionMap, TRANSFORM_TEX(i.uv0,_EmissionMap));
				  MetallicMap.r = MetallicMap.r*_ExposeColor.r;
				  MetallicMap.a = MetallicMap.a*_ExposeColor.a;
				  float3x3 tangentTransform = float3x3(i.tangentDir, i.bitangentDir, i.normalDir);
				  float3 normalDirection = normalize(mul(BumpMap, tangentTransform));
				  float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				  float shiftAmount = dot(i.normalDir, viewDirection);
				  normalDirection = shiftAmount < 0.0f ? normalDirection + viewDirection * (-shiftAmount + 1e-5f) : normalDirection;

				  //light calculations
					   float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
					   float3 lightReflectDirection = reflect(-lightDirection, normalDirection);
					   float3 viewReflectDirection = normalize(reflect(-viewDirection, normalDirection));
					   float NdotL = max(0.0, dot(normalDirection, lightDirection));
					   float3 halfDirection = normalize(viewDirection + lightDirection);
					   float NdotH = max(0.0,dot(normalDirection, halfDirection));
					   float NdotV = max(0.0,dot(normalDirection, viewDirection));
					   float VdotH = max(0.0,dot(viewDirection, halfDirection));
					   float LdotH = max(0.0,dot(lightDirection, halfDirection));
					   float LdotV = max(0.0,dot(lightDirection, viewDirection));
					   float RdotV = max(0.0, dot(lightReflectDirection, viewDirection));
					   float attenuation = LIGHT_ATTENUATION(i);
					   float3 attenColor = attenuation * _LightColor0.rgb;
					   
					   //get Unity Scene lighting data
					   //UnityGI gi =  GetUnityGI(_LightColor0.rgb, lightDirection, normalDirection, viewDirection, viewReflectDirection, attenuation, 1- _Glossiness, i.posWorld.xyz);



					  #ifdef _MetallicChannel_Gloss_R 
					   fixed4 GlossMap = tex2D(_GlossMap,i.uv0);
					  _Glossiness = _Glossiness * GlossMap.r * _ExposeColor.a;
					  #elif _MetallicChannel_Metallic_A 
					  _Glossiness = _Glossiness * MetallicMap.a;
					  #endif
					   _Metallic = MetallicMap.r * _Metallic;
					   //diffuse color calculations
					   float roughness = 1 - (_Glossiness * _Glossiness);
					   roughness = roughness * roughness;
						float f0 = F0(NdotL, NdotV, LdotH, roughness);

						//what do you fuck to doo?   1.0 - _Metallic
					   float3 diffuseColor = (_Color.rgb * AlbedoTex + EmissionMap * _EmissionIntensity) * (1.0 - _Metallic);

					   float gloss_c = _Glossiness;
					   UnityGI gi = GIdata(lightColor, lightDirection, normalDirection, posWorld,viewDirection, attenuation, i.ambientOrLightmapUV, gloss_c, viewReflectDirection);

					   float3 indirectDiffuse =float3(0,0,0)+ gi.indirect.diffuse.rgb*IBL_Intensity*_AmbientLight;
					   float3 indirectSpecular = gi.indirect.specular.rgb*SBL_Intensity;



					   float3 SPLMap = SBL(_SPL, normalDirection, MetallicMap.r);
					   float3 IBLMap = IBL(_IBL, normalDirection);

					   //redefine
					 // float3 indirectDiffuse =IBLMap ;

					  //float3 indirectSpecular = SPLMap;

					 // diffuseColor *= (f0 );
					  // diffuseColor+=indirectDiffuse;

					  //Specular calculations

					   float3 specColor = lerp(_SpecularColor.rgb*SpecTex*_SpecularPower, _Color.rgb, _Metallic * 0.5);
					   //return float4  (diffuseColor,1);

					   float3 SpecularDistribution = specColor;
					   float GeometricShadow = 1;
					   float3 FresnelFunction = specColor;

					   //Normal Distribution Function/Specular Distribution-----------------------------------------------------        

					  #ifdef _NORMALDISTMODEL_BLINNPHONG 
						   SpecularDistribution *= BlinnPhongNormalDistribution(NdotH, _Glossiness,  max(1,_Glossiness * 40));
					  #elif _NORMALDISTMODEL_U3DBLINNPHONG
						   SpecularDistribution *= BlinnPhongU3D(NdotH, MetallicMap.a *_SpecularColor * _SpecularRange,  exp2(_Glossiness * 10.0 + 1.0)) * _Glossiness;
					  #elif _NORMALDISTMODEL_PHONG
						   SpecularDistribution *= PhongNormalDistribution(RdotV, _Glossiness, max(1,_Glossiness * 40));
					  #elif _NORMALDISTMODEL_BECKMANN
						   SpecularDistribution *= BeckmannNormalDistribution(roughness, NdotH);
					  #elif _NORMALDISTMODEL_GAUSSIAN
						   SpecularDistribution *= GaussianNormalDistribution(roughness, NdotH);
					  #elif _NORMALDISTMODEL_GGX
						   SpecularDistribution *= GGXNormalDistribution(roughness, NdotH);
					  #elif _NORMALDISTMODEL_TROWBRIDGEREITZ
						   SpecularDistribution *= TrowbridgeReitzNormalDistribution(NdotH, roughness);
					  #elif _NORMALDISTMODEL_TROWBRIDGEREITZANISOTROPIC
						   SpecularDistribution *= TrowbridgeReitzAnisotropicNormalDistribution(_Anisotropic,NdotH, dot(halfDirection, i.tangentDir), dot(halfDirection,  i.bitangentDir));
					  #elif _NORMALDISTMODEL_WARD
						   SpecularDistribution *= WardAnisotropicNormalDistribution(_Anisotropic,NdotL, NdotV, NdotH, dot(halfDirection, i.tangentDir), dot(halfDirection,  i.bitangentDir));
					  #else
						  SpecularDistribution *= GGXNormalDistribution(roughness, NdotH);
					  #endif

						  //Geometric Shadowing term----------------------------------------------------------------------------------
						 #ifdef _SMITHGEOSHADOWMODEL_NONE
							 #ifdef _GEOSHADOWMODEL_ASHIKHMINSHIRLEY
								 GeometricShadow *= AshikhminShirleyGeometricShadowingFunction(NdotL, NdotV, LdotH);
							 #elif _GEOSHADOWMODEL_ASHIKHMINPREMOZE
								 GeometricShadow *= AshikhminPremozeGeometricShadowingFunction(NdotL, NdotV);
							 #elif _GEOSHADOWMODEL_DUER
								 GeometricShadow *= DuerGeometricShadowingFunction(lightDirection, viewDirection, normalDirection, NdotL, NdotV);
							 #elif _GEOSHADOWMODEL_NEUMANN
								 GeometricShadow *= NeumannGeometricShadowingFunction(NdotL, NdotV);
							 #elif _GEOSHADOWMODEL_KELEMAN
								 GeometricShadow *= KelemenGeometricShadowingFunction(NdotL, NdotV, LdotH,  VdotH);
							 #elif _GEOSHADOWMODEL_MODIFIEDKELEMEN
								 GeometricShadow *= ModifiedKelemenGeometricShadowingFunction(NdotV, NdotL, roughness);
							 #elif _GEOSHADOWMODEL_COOK
								 GeometricShadow *= CookTorrenceGeometricShadowingFunction(NdotL, NdotV, VdotH, NdotH);
							 #elif _GEOSHADOWMODEL_WARD
								 GeometricShadow *= WardGeometricShadowingFunction(NdotL, NdotV, VdotH, NdotH);
							 #elif _GEOSHADOWMODEL_KURT
								 GeometricShadow *= KurtGeometricShadowingFunction(NdotL, NdotV, VdotH, roughness);
							 #else           
								 GeometricShadow *= ImplicitGeometricShadowingFunction(NdotL, NdotV);
							 #endif
								 ////SmithModelsBelow
								 ////Gs = F(NdotL) * F(NdotV);
								 #elif _SMITHGEOSHADOWMODEL_WALTER
									 GeometricShadow *= WalterEtAlGeometricShadowingFunction(NdotL, NdotV, roughness);
								 #elif _SMITHGEOSHADOWMODEL_BECKMAN
									 GeometricShadow *= BeckmanGeometricShadowingFunction(NdotL, NdotV, roughness);
								 #elif _SMITHGEOSHADOWMODEL_GGX
									 GeometricShadow *= GGXGeometricShadowingFunction(NdotL, NdotV, roughness);
								 #elif _SMITHGEOSHADOWMODEL_SCHLICK
									 GeometricShadow *= SchlickGeometricShadowingFunction(NdotL, NdotV, roughness);
								 #elif _SMITHGEOSHADOWMODEL_SCHLICKBECKMAN
									 GeometricShadow *= SchlickBeckmanGeometricShadowingFunction(NdotL, NdotV, roughness);
								 #elif _SMITHGEOSHADOWMODEL_SCHLICKGGX
									 GeometricShadow *= SchlickGGXGeometricShadowingFunction(NdotL, NdotV, roughness);
								 #elif _SMITHGEOSHADOWMODEL_IMPLICIT
									 GeometricShadow *= ImplicitGeometricShadowingFunction(NdotL, NdotV);
								 #elif  _SMITHGEOSHADOWMODEL_GeometrySchlickGGX 
									 GeometricShadow *= GeometrySchlickGGX(NdotL, NdotV,roughness);
								 #else
									 GeometricShadow *= ImplicitGeometricShadowingFunction(NdotL, NdotV);
								 #endif
									 //Fresnel Function-------------------------------------------------------------------------------------------------


									#ifdef _FRESNELMODEL_SCHLICK
										//return float4(1,1,0,1);
										FresnelFunction *= SchlickFresnelFunction(specColor, LdotH);
									#elif _FRESNELMODEL_SCHLICKIOR
									//  return float4(1,0,1,1);
										FresnelFunction *= SchlickIORFresnelFunction(_Ior, LdotH);
									#elif _FRESNELMODEL_SPHERICALGAUSSIAN
									//  return float4(0,0,1,1);
										FresnelFunction *= SphericalGaussianFresnelFunction(LdotH, f0);
									#elif _FRESNELMODEL_SCHICHU3D
										//return float4(1,1,1,1);
										FresnelFunction = F_Schlick_U3D(NdotV,specColor);
									#else
										FresnelFunction *= SchlickIORFresnelFunction(_Ior, LdotH);
									#endif



									float3 DebugColor = float3(1,1,1);
									#ifdef _ENABLE_NDF_ON
									 return float4(DebugColor* SpecularDistribution,1);
									#endif
									#ifdef _ENABLE_G_ON 
									 return float4(DebugColor * GeometricShadow,1);
									#endif
									#ifdef _ENABLE_F_ON 
									 return float4(DebugColor* FresnelFunction,1);
									#endif
									#ifdef _ENABLE_D_ON 
									 return float4(DebugColor* diffuseColor,1);
									#endif

									 //PBR
									  float grazingTerm = saturate(roughness + _Metallic);
									 float3 specularity = (SpecularDistribution * FresnelFunction * GeometricShadow)/ (4 * (NdotL * NdotV));
									 
									 float3 unityIndirectSpecularity = indirectSpecular * max(0.15,_Metallic); //* FresnelLerp(specColor,grazingTerm,NdotV)
								// return float4 (unityIndirectSpecularity,1);
									 //unityIndirectSpecularity = 0;
								
									 float3 lightingModel = (((diffuseColor + saturate(specularity) ) + (  ( _Metallic) * unityIndirectSpecularity)));
									 // return float4(lightingModel, 1);
								   //return  float4    (lightingModel,1);
									  lightingModel *= NdotL * attenColor;
									
									  float4 finalDiffuse = float4(lightingModel + indirectDiffuse * AlbedoTex,1)/2;


									  //gama校正    
										#ifdef _ENABLE_GAMMA_ON
										  finalDiffuse.rgb = saturate(GammaCorrection(finalDiffuse.rgb,_gama));
										#endif
										  //UNITY_APPLY_FOG(i.fogCoord, finalDiffuse);
										 return finalDiffuse;
            }
            ENDCG
        }















}
FallBack "Diffuse"
}