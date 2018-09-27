
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
#ifndef ROLANTIN_CG_INCLUDED
#define ROLANTIN_CG_INCLUDED

#include "UnityCG.cginc"

uniform fixed3 LightDir0;
uniform fixed3 LightDir1;
uniform fixed3 LightDir2;


uniform fixed3 LightColor0;
uniform fixed3 LightColor1;
uniform fixed3 LightColor2;

uniform fixed LightIntensity0;
uniform fixed LightIntensity1;
uniform fixed LightIntensity2;

uniform fixed3 posWorld;

uniform fixed CubemapIntensityIBL;

uniform fixed4 DiffColor;

uniform float _NormalIntensity;

uniform fixed3 _AmbientLight;

uniform fixed _MetallicPower;

uniform fixed _GlossPower;

uniform fixed MatCap_Intensity;

uniform fixed3 DirectionLight;

uniform fixed ColorSpaceValue;

uniform fixed4 DiffuseColor;

//uniform float _Gloss;

uniform float PointLightDistance;

//Ambient Indirect lgihting
uniform fixed3 AmbientColor;


uniform float IBL_Blur;
uniform float IBL_Intensity;

uniform float SBL_Intensity;

uniform float _BP_Gloss;

uniform float _Gama;

uniform float3 PointLightPosition;

uniform float2 AttenuationValue;

const float PI = 3.14159265359;


//____________________________________________________________________________________________________________________________________________

inline half ROneMinusReflectivityFromMetallic(half metallic)
{
	// We'll need oneMinusReflectivity, so
	//   1-reflectivity = 1-lerp(dielectricSpec, 1, metallic) = lerp(1-dielectricSpec, 0, metallic)
	// store (1-dielectricSpec) in unity_ColorSpaceDielectricSpec.a, then
	//	 1-reflectivity = lerp(alpha, 0, metallic) = alpha + metallic*(0 - alpha) = 
	//                  = alpha - metallic * alpha
	half oneMinusDielectricSpec = unity_ColorSpaceDielectricSpec.a;
	return oneMinusDielectricSpec - metallic * oneMinusDielectricSpec;
}


inline half3 RDiffuseAndSpecularFromMetallic (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
{
	specColor = lerp (unity_ColorSpaceDielectricSpec.rgb, albedo, metallic);
	oneMinusReflectivity = ROneMinusReflectivityFromMetallic(metallic);
	return albedo * oneMinusReflectivity;
}


inline half RSmithJointGGXVisibilityTerm(half NdL, half NdV, half k)
{
	half gL = NdL * (1 - k) + k;
	half gV = NdV * (1 - k) + k;
	return 1.0 / (gL * gV + 1e-5f); // This function is not intended to be running on Mobile,
									// therefore epsilon is smaller than can be represented by half
}


inline half RGGXTerm(half NdotH, half roughness)
{
	half a = roughness * roughness;
	half a2 = a * a;
	half d = NdotH * NdotH * (a2 - 1.f) + 1.f;
	return a2 / (UNITY_PI * d * d);
}


inline half RPow2(half x){
   return x*x;
}


inline half RPow5(half x)
{
	return x*x * x*x * x;
}


//捏菲尔算法
inline half3 RFresnelTerm(half3 specularColor, half LdotH)
{
	half t = RPow5(1 - LdotH);	// ala Schlick interpoliation
	return specularColor + (1 - specularColor) * t;
}


//DirectionLight 1 Base Specular

inline half3 RFresnelLerp(half3 F0, half3 F90, half cosA)
{
	half t = RPow5(1 - cosA);	// ala Schlick interpoliation
	return lerp(F0, F90, t);
}


fixed3 RolanWaterLight(fixed3 normalDir,fixed3 LightDir) {


    fixed3 L = normalize(-LightDir);
	half3 N = normalize(normalDir);
	half3 V = normalize(_WorldSpaceCameraPos.xyz - posWorld); 
	half3 H = normalize(L+V);
	half3 nh =saturate(dot(N,H));

	float spec = pow(nh, _MetallicPower * 128.0) * _GlossPower;

	return spec;
	//fixed4 c;
//	c.rgb = (albedo * lightColor * diffFactor + specColor.rgb * spec * lightColor) * (atten);
//	c.a = alpha + spec * specColor.a;
	//return c;
}



//____________________________________________________________________________________________________________________________________________
//Phong SP
//R= -L-2(N·-L)N & R = reflect(-L,N)
//F= (R·V)*s 
float3 Phong (float3 LightDir,float3 normalDir,float Metallicmap,float Glossmap){

 float3 L = normalize(-LightDir);
 float3 N = normalize(normalDir);
 float3 V = normalize(_WorldSpaceCameraPos.xyz -posWorld); 
    // -L-2(N·-L)N
 //float3 R = L - pow(dot(N,L) ,2 ) * N;
 float3 R = reflect(-L,N);
 float3 G =  saturate(dot(R,V));
    // smooth and highlight sp
 float specPow = exp2(_BP_Gloss * 10 + 1.0 ) * Glossmap;  
 //clamp min range
 float3 S = max(0, pow(G,specPow)*( Metallicmap + Glossmap/3)); 
 return S;
}

//BlinnPhong
// H = normalize(L+V) 
// F = (N·H)*s
float3 BlinnPhong(float3 LightDir,float3 normalDir,float spIntensity,float4 MetallicTex){
 float3 L = normalize(-LightDir);
 float3 N = normalize(normalDir);
 float3 V = normalize(_WorldSpaceCameraPos.xyz - posWorld); 
 float3 H = normalize(L+V);
 float3 F =saturate(dot(N,H));
 float specPow = exp2(_BP_Gloss * MetallicTex.a * 10 + 1 ) * _GlossPower;  
 float3 fianl =  max(0 ,pow(F,specPow)*spIntensity * MetallicTex.r ) ;
 return fianl;
}

// Diffuse Ambient Light    
float3 IBL(samplerCUBE CubeTexIBL , float3 normalDirection){	 
	  //blur cubemap
	  float3 CubeTex = texCUBElod(CubeTexIBL,float4(normalDirection,IBL_Blur)).rgb;
	  //final color
	  float3 F = _AmbientLight* CubeTex * IBL_Intensity; 
	  return F;   
}

float3 SBL(samplerCUBE CubeTexIBL,float3 normalDirection , float3 MetallicTex){
	  float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld);
	  float3 viewReflectDirection = reflect( - viewDirection, normalDirection);
// float3 indirectSpecular = (0 + (texCUBElod(_SBL,float4(viewReflectDirection,80.0)).rgb*0.5*_Metallic_var.r)) * SBL_Intensity;
      float3 SB = texCUBElod(CubeTexIBL,float4(viewReflectDirection,IBL_Blur)).rgb*0.5 * SBL_Intensity * MetallicTex ;
      float3 F = SB ;
	return SB;
}


//texCUBElod比较耗，合并减少一次.
 void SBLAndIBL(samplerCUBE CubeTexIBL , float3 normalDirection , float3 MetallicTex,out float3 sblColor , out float3 iblColor){	 
	  float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld);
	  float3 viewReflectDirection = reflect( - viewDirection, normalDirection);
	  float3 CubeTex = texCUBElod(CubeTexIBL,float4(normalDirection,IBL_Blur)).rgb;
	  //final color
	  iblColor = _AmbientLight* CubeTex * IBL_Intensity;
	  sblColor = CubeTex*0.5 * SBL_Intensity * MetallicTex ;   
}

 void SBLAndIBLSample(float3 CubeTex  , float3 normalDirection , float3 MetallicTex,out float3 sblColor , out float3 iblColor){	 
	  float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld);
	  float3 viewReflectDirection = reflect( - viewDirection, normalDirection);
	  //final color
	  iblColor = _AmbientLight* CubeTex * IBL_Intensity;
	  sblColor = CubeTex*0.5 * SBL_Intensity * MetallicTex ;   
}



//PBR SP
float3 PBR_SP(float3 normalDirection ,float3 lightDirection ,float3 Light,
	          float4 MetallicTex, float3 diffuseColor ,out float3 diffusefinalColor)
{
	//diffusefinalColor = diffuseColor;

	//diffusefinalColor = float3(1,1,1);
	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld);
    float3 halfDirection = normalize(viewDirection+lightDirection);
    float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
	float NdotL = saturate(dot(normalDirection, lightDirection));
    float LdotH = saturate(dot(lightDirection, halfDirection));
    
    // input metallic map (R - channel is metallic )
    float3 specularColor =  MetallicTex.r * _MetallicPower;
    // metallic map (A - channel is gloss)
    float3 gloss = MetallicTex.a * _GlossPower;
    // invent gloss range 
    float perceptualRoughness = 1.0 -  (MetallicTex.a*_GlossPower);
    float roughness = perceptualRoughness;

     float specularMonochrome;
    diffusefinalColor = RDiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
    specularMonochrome = 1.0-specularMonochrome;
 //    specularColor = lerp(unity_ColorSpaceDielectricSpec.rgb, diffuseColor, specularColor);
	// specularMonochrome = OneMinusReflectivityFromMetallic(specularColor);
	// diffusefinalColor = diffuseColor * ( specularMonochrome);
	
 // return float4(diffusefinalColor,.);


  //  diffuseColor = DiffuseAndSpecularFromMetallic(diffuseColor, specularColor, specularColor, specularMonochrome );
    float NdotV = abs(dot( normalDirection, viewDirection ));
    float NdotH = saturate(dot( normalDirection, halfDirection));
    float VdotH = saturate(dot( viewDirection, halfDirection ));

    float visTerm = RSmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
    float normTerm = RGGXTerm(NdotH, roughness);

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
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;

   float3 directSpecular = Light * specularPBL*RFresnelTerm(specularColor, LdotH) ;
   half grazingTerm = saturate( gloss + specularMonochrome );

//   float3 indirectSpecular = SBL;
//    float3 ins =indirectSpecular * FresnelLerp (specularColor, grazingTerm, NdotV);
    //indirectSpecular *= surfaceReduction;
    return directSpecular ;
}

//Attenuation = 1.0 / kc + kl*d + kq * (d*d)




inline float4x4 DirectionalLight(float3 normalDirection) {   
    float3 NorDir = normalize(normalDirection);

    float3 dir0 = max(0.0,dot( NorDir, -LightDir0 ));
    float3 DIR0 = dir0 * LightIntensity0 * LightColor0;

    float3 dir1 = max(0.0,dot( NorDir, -LightDir1 ));
    float3 DIR1 = dir1 * LightIntensity1 * LightColor1 ;

    float3 dir2 = max(dot( NorDir, -LightDir2 ) ,0.0);
    float3 DIR2 = dir2 * LightIntensity2 * LightColor2 ;
    float3 finalLighing = DIR0 + DIR1 + DIR2 ;

    float4x4 Full = float4x4((DIR0.xyzz),(DIR1.xyzz),(DIR2.xyzz),(finalLighing.xyzz));

    return  Full;
}



inline float Attenuation(float3 worldpos , float3 LightDirpos){
	/* 
	kc = 常数项通常是1.0，它的作用是保证分母永远不会比1小，因为它可以利用一定的距离增加亮度，这个结果不会影响到我们所寻找的。
	kl = 一次项用于与距离值相乘，这会以线性的方式减少亮度。
	kq = 二次项用于与距离的平方相乘，为光源设置一个亮度的二次递减。二次项在距离比较近的时候相比一次项会比一次项更小，但是当距离更远的时候比一次项更大。
	由于二次项的光会以线性方式减少，指导距离足够大的时候，就会超过一次项，之后，光的亮度会减少的更快。最后的效果就是光在近距离时，
	非常量，但是距离变远亮度迅速降低，最后亮度降低速度再次变慢。下面的图展示了在100以内的范围，这样的衰减效果。*/
   // AttenuationValue.x = 0.2
   // AttenuationValue y = 1.7; 
    // kd 
    // kc 1
    // kl 0.7
    // kq 1.8
    // kl2 = 7 * 0.7 / kd
    float d = distance(worldpos, LightDirpos);
	float atten =1 / (1 +AttenuationValue.x * d +AttenuationValue. y * ( d  * d)) ;
	return saturate(atten);
}


inline float3 PointLightAttenuation(float3 normalDirection,float3 Lightpos,float3 worldpos,float3 LightDirpos) {   
    float3 atten = Attenuation(worldpos,LightDirpos);
    float3 PointLight = max(dot(normalDirection, Lightpos) ,0) * atten;
    return PointLight;
}


inline float3 PointLight(float3 normaldir,float3 p){
	float3 lightDirection;
	float atten;
	
	// if(p.w == 0,0){
	// 	atten = 1;
	// 	lightDirection = normalize(p.xyz);
	// }
	// else {
	   float3 lightvector = p - posWorld.xyz;
	   float distance = length(lightvector);
	   atten = 1/distance;
	   lightDirection = normalize(lightvector);
//	}

	float3 finalPointlight = atten * saturate(dot(normaldir,lightDirection));
    
    return finalPointlight;
}





//———————————————————————————————————————————————————————————————TOOL——————————————————————————————————————————————————————————

 float3 NormalIntensity(float3 NormalMapTex){

	 // float4 finalNormap = ((NormalMapTex.rgba.rg * _NormalIntensity),NormalMapTex.b,NormalMapTex.a);

	 float2 finalNormap = NormalMapTex.rg *_NormalIntensity;
	 fixed3 o = fixed3(finalNormap.rg, NormalMapTex.b);
      return o;
}


float3 MatCap(float3 VertexNormal)
{ 
	float3 worldNorm = normalize(unity_WorldToObject[0].xyz * VertexNormal.x + unity_WorldToObject[1].xyz *VertexNormal.y + unity_WorldToObject[2].xyz * VertexNormal.z);
		   worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);

		  return worldNorm.xyz*0.5+0.5;
 
}


float4 ColorSpace(float4 col){
	return pow(col , lerp (1/1.2, 1 / 2.2 , _Gama ));
}

//———————————————————————————————————————————————————————————————TOOL——————————————————————————————————————————————————————————





//Get the reflect direction in world space;
//获取空间中的光照方向
fixed3 reflectD(float3 LightDir , float3 wNormal) {
   float3 ref = normalize (reflect (LightDir,wNormal));
   return ref;

}  



 half CAL(half3 a, half3 b)
{
#if (SHADER_TARGET < 30 || defined(SHADER_API_PS3))
	return saturate(dot(a, b));
#else
	return max(0.0h, dot(a, b));
#endif
}



inline fixed3 SpecularD(fixed3 n,fixed3 h,half Roughness,float3 normalDir,float3 LightDir){
   half k = 3.141952654 ;
   half u = Roughness * Roughness;
   fixed3 d = RPow2(PI*(RPow2(dot(n,h)) * (u - 1) + 1 ));
   return u/d;
}


inline fixed3 BDRF(fixed m,fixed Sc,fixed3 C_base){

//Specular D For the normal distribution function (NDF), we found Disney’s choice of GGX/Trowbridge-Reitz to bewellworththecost. 
//TheadditionalexpenseoverusingBlinn-Phongisfairlysmall, andthedistinct, natural appearance produced by the longer “tail” appealed to our artists.
//We also adopted Disney’s reparameterization of 
//α = Roughness2. 
//D(h) = α^2/π ((n·h)^2(α^2 −1) + 1)^2 


//Specular G We evaluated more options for the specular geometric attenuation term than any other. 
//In the end, we chose to use the Schlick model [19], but with k = α/2, so as to better ﬁt the Smith model 
//for GGX [21]. With this modiﬁcation, the Schlick model exactly matches Smith for α = 1 and is a fairly
// closeapproximationovertherange[0,1](showninFigure2). WealsochosetouseDisney’smodiﬁcation to reduce 
//“hotness” by remapping roughness using Roughness+1/2 before squaring. It’s important to note that this 
//adjustment is only used for analytic light sources; if applied to image-based lighting, the results at 
//glancing angles will be much too dark.

//	float m = metallic;
//	float Sc = specular;





/////Base
//	float3 L = normalize(-LightDir);
//	float3 N = normalize(normalDir);
//float3 V = normalize(_WorldSpaceCameraPos.xyz - posWorld); 
//	float3 H = normalize(L+V);
//	float3 R = reflect(-L,N);
//	float3 G =  saturate(dot(R,V));	
//	float3 final =  SpecularD(N,H,Roughness);
    fixed3 C_diff = ( 1 - m) * C_base;
    float3 C_spec = lerp (0.08 * Sc , C_base , m);
  // fixed3 d = C_diff / PI ;
/////SpecularD
  
   //half u = Roughness * Roughness;
//   fixed3 d = Pi*Pow2((Pow2(dot(n,h)) * (u - 1) + 1 ));
  // fixed3 dh = u/d;

/////SpecularG
 //  half k = Pow2(Roughness + 1);
 //  fixed3 ndv = dot(N,V);
 // fixed3 Gv = ndv / ndv * (1 - k ) + k;
 //  fixed3 Glvh = 

 float3 fianl = C_diff;

 return fianl;

}



#endif


