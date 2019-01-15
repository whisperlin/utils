
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
#ifndef ROLANTIN_CG_INCLUDED
#define ROLANTIN_CG_INCLUDED

#include "UnityCG.cginc"


 

float rcp_fun(float x)
{
	return 1/x;
}

 

uniform fixed3 LightDir0;
//uniform fixed3 LightDir1;
//uniform fixed3 LightDir2;


uniform fixed3 LightColor0;
//uniform fixed3 LightColor1;
//uniform fixed3 LightColor2;

//uniform fixed LightIntensity0;
//uniform fixed LightIntensity1;
//uniform fixed LightIntensity2;

uniform fixed3 posWorld;

uniform fixed CubemapIntensityIBL;

uniform fixed4 DiffColor;

//uniform float _NormalIntensity;

uniform fixed3 _AmbientLight;

uniform fixed _MetallicPower;

uniform fixed _GlossPower;

uniform fixed MatCap_Intensity;


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

float _Glossing;



float pow2(float x){
  
return x*x;
}

float Pow5(float x ){

  return x*x*x*x*x;
}

//UE Define
float  ClampedPow( float  X, float  Y)
{
    return pow(max(abs(X),0.000001f),Y);
}
float2  ClampedPow( float2  X, float2  Y)
{
    return pow(max(abs(X), float2 (0.000001f,0.000001f)),Y);
}
float3  ClampedPow( float3  X, float3  Y)
{
    return pow(max(abs(X), float3 (0.000001f,0.000001f,0.000001f)),Y);
}
float4  ClampedPow( float4  X, float4  Y)
{
    return pow(max(abs(X), float4 (0.000001f,0.000001f,0.000001f,0.000001f)),Y);
}

float  PhongShadingPow( float  X,  float  Y)
{
    return ClampedPow(X, Y);
}

//int ReverseBits32( int bits ){
//    return reversebits( bits );
//}

float radicalInverse_VdC(int bits) {
     bits = (bits << 16u) | (bits >> 16u);
     bits = ((bits & 0x55555555u) << 1u) | ((bits & 0xAAAAAAAAu) >> 1u);
     bits = ((bits & 0x33333333u) << 2u) | ((bits & 0xCCCCCCCCu) >> 2u);
     bits = ((bits & 0x0F0F0F0Fu) << 4u) | ((bits & 0xF0F0F0F0u) >> 4u);
     bits = ((bits & 0x00FF00FFu) << 8u) | ((bits & 0xFF00FF00u) >> 8u);
     return float(bits) * 2.3283064365386963e-10; // / 0x100000000
 }

 float2 Hammersley(int i, int N) {
     return float2(float(i)/float(N), radicalInverse_VdC(i));
 }


float GGX(float NdotV, float a)
{
  float k = (a * a) / 2;
  return NdotV / (NdotV * (1.0f - k) + k);
}

 float G_Smith(float a, float nDotV, float nDotL)
{
  return GGX(nDotL, a) * GGX(nDotV, a);
}


//end /UE Define____________________________________________________________________________________________________________________________________

//__________________________________________________________________BaseShaderDiffuse_______________________________________________________________________
//D_UE
float3 Diffuse_Lambert( float3 DiffuseColor ){
  return DiffuseColor * (1 / PI);
}

//Burley 2012, "Physically-Based Shading at Disney
float3 Diffuse_Burley( float3 DiffuseColor, float Roughness, float NoV, float NoL, float VoH ){
  float FD90 = 0.5 + 2 * VoH * VoH * Roughness;
  float FdV = 1 + (FD90 - 1) * Pow5( 1 - NoV );
  float FdL = 1 + (FD90 - 1) * Pow5( 1 - NoL );
  return DiffuseColor * ( (1 / PI) * FdV * FdL );
}

//Gotanda 2012, "Beyond a Simple Physically Based Blinn-Phong Model in Real-Time
float3 Diffuse_OrenNayar( float3 DiffuseColor, float Roughness, float NoV, float NoL, float VoH )
{
  float a = Roughness * Roughness;
  float s = a;// / ( 1.29 + 0.5 * a );
  float s2 = s * s;
  float VoL = 2 * VoH * VoH - 1;    // double angle identity
  float Cosri = VoL - NoV * NoL;
  float C1 = 1 - 0.5 * s2 / (s2 + 0.33);
  float C2 = 0.45 * s2 / (s2 + 0.09) * Cosri * ( Cosri >= 0 ? rcp_fun( max( NoL, NoV ) ) : 1 );
  return DiffuseColor / PI * ( C1 + C2 ) * ( 1 + Roughness * 0.5 );
}

//Gotanda 2014, "Designing Reflectance Models for New Consoles
float3 Diffuse_Gotanda( float3 DiffuseColor, float Roughness, float NoV, float NoL, float VoH )
{
  float a = Roughness * Roughness;
  float a2 = a * a;
  float F0 = 0.04;
  float VoL = 2 * VoH * VoH - 1;    // double angle identity
  float Cosri = VoL - NoV * NoL;
#if 1
  float a2_13 = a2 + 1.36053;
  float Fr = ( 1 - ( 0.542026*a2 + 0.303573*a ) / a2_13 ) * ( 1 - pow( 1 - NoV, 5 - 4*a2 ) / a2_13 ) * ( ( -0.733996*a2*a + 1.50912*a2 - 1.16402*a ) * pow( 1 - NoV, 1 + rcp_fun(39*a2*a2+1) ) + 1 );
  //float Fr = ( 1 - 0.36 * a ) * ( 1 - pow( 1 - NoV, 5 - 4*a2 ) / a2_13 ) * ( -2.5 * Roughness * ( 1 - NoV ) + 1 );
  float Lm = ( max( 1 - 2*a, 0 ) * ( 1 - Pow5( 1 - NoL ) ) + min( 2*a, 1 ) ) * ( 1 - 0.5*a * (NoL - 1) ) * NoL;
  float Vd = ( a2 / ( (a2 + 0.09) * (1.31072 + 0.995584 * NoV) ) ) * ( 1 - pow( 1 - NoL, ( 1 - 0.3726732 * NoV * NoV ) / ( 0.188566 + 0.38841 * NoV ) ) );
  float Bp = Cosri < 0 ? 1.4 * NoV * NoL * Cosri : Cosri;
  float Lr = (21.0 / 20.0) * (1 - F0) * ( Fr * Lm + Vd + Bp );
  return DiffuseColor / PI * Lr;
#else
  float a2_13 = a2 + 1.36053;
  float Fr = ( 1 - ( 0.542026*a2 + 0.303573*a ) / a2_13 ) * ( 1 - pow( 1 - NoV, 5 - 4*a2 ) / a2_13 ) * ( ( -0.733996*a2*a + 1.50912*a2 - 1.16402*a ) * pow( 1 - NoV, 1 + rcp_fun(39*a2*a2+1) ) + 1 );
  float Lm = ( max( 1 - 2*a, 0 ) * ( 1 - Pow5( 1 - NoL ) ) + min( 2*a, 1 ) ) * ( 1 - 0.5*a + 0.5*a * NoL );
  float Vd = ( a2 / ( (a2 + 0.09) * (1.31072 + 0.995584 * NoV) ) ) * ( 1 - pow( 1 - NoL, ( 1 - 0.3726732 * NoV * NoV ) / ( 0.188566 + 0.38841 * NoV ) ) );
  float Bp = Cosri < 0 ? 1.4 * NoV * Cosri : Cosri / max( NoL, 1e-8 );
  float Lr = (21.0 / 20.0) * (1 - F0) * ( Fr * Lm + Vd + Bp );
  return DiffuseColor / PI * Lr;
#endif
}



//__________________________________________________________________NDF法线分布函数(NDF)_______________________________________________________________________


//Specular_D_UE//GGX / Trowbridge-Reitz
inline float D_GGX( float Roughness, float NoH ){
   float a = Roughness * Roughness;
   float a2 = a * a;
   float d = ( NoH * a2 - NoH ) * NoH + 1; // 2 mad
   return a2 / ( PI*d*d );         // 4 mul, 1 rcp
}



//Blinn 1977, "Models of light reflection for computer synthesized pictures
inline float D_Blinn( float Roughness, float NoH ){
  float a = Roughness * Roughness;
  float a2 = a * a;
  float n = 2 / a2 - 2;
  return (n+2) / (2*PI) * PhongShadingPow( NoH, n );    // 1 mad, 1 exp, 1 mul, 1 log
}

//Beckmann 1963, "The scattering of electromagnetic waves from rough surfaces
inline float D_Beckmann( float Roughness, float NoH ){
  float a = Roughness * Roughness;
  float a2 = a * a;
  float NoH2 = NoH * NoH;
  return exp( (NoH2 - 1) / (a2 * NoH2) ) / ( PI * a2 * NoH2 * NoH2 );
}

//Anisotropic GGX, Burley 2012, "Physically-Based Shading at Disney
inline float D_GGXaniso( float RoughnessX, float RoughnessY, float NoH, float3 H, float3 X, float3 Y ){
  float ax = RoughnessX * RoughnessX;
  float ay = RoughnessY * RoughnessY;
  float XoH = dot( X, H );
  float YoH = dot( Y, H );
  float d = XoH*XoH / (ax*ax) + YoH*YoH / (ay*ay) + NoH*NoH;
  return 1 / ( PI * ax*ay * d*d );
}

//__________________________________________________________________Geometrical Attenuation Factor_______________________________________________________________________
//Specular_G_UE
float Vis_Schlick( float Roughness, float NoV, float NoL ){
  float k = pow2( Roughness ) * 0.5;
  float Vis_SchlickV = NoV * (1 - k) + k;
  float Vis_SchlickL = NoL * (1 - k) + k;
  return 0.25 / ( Vis_SchlickV * Vis_SchlickL );
}

//Neumann et al. 1999, "Compact metallic reflectance models
float Vis_Neumann( float NoV, float NoL ){
  return 1 / ( 4 * max( NoL, NoV ) );
}

//Kelemen 2001, "A microfacet based coupled specular-matte brdf model with importance sampling
float Vis_Kelemen( float VoH ){
  // constant to prevent NaN
  return rcp_fun( 4 * VoH * VoH + 1e-5);
}

//Smith 1967, "Geometrical shadowing of a random rough surface
float Vis_Smith( float Roughness, float NoV, float NoL )
{
  float a = pow2( Roughness );
  float a2 = a*a;

  float Vis_SmithV = NoV + pow2( NoV * (NoV - NoV * a2) + a2 );
  float Vis_SmithL = NoL + pow2( NoL * (NoL - NoL * a2) + a2 );
  return rcp_fun( Vis_SmithV * Vis_SmithL );
}

//Heitz 2014, "Understanding the Masking-Shadowing Function in Microfacet-Based BRDFs
float Vis_SmithJointApprox( float Roughness, float NoV, float NoL ){
  float a = pow2( Roughness );
  float Vis_SmithV = NoL * ( NoV * ( 1 - a ) + a );
  float Vis_SmithL = NoV * ( NoL * ( 1 - a ) + a );
  // Note: will generate NaNs with Roughness = 0.  MinRoughness is used to prevent this
  return 0.5 * rcp_fun( Vis_SmithV + Vis_SmithL );
}


//____________________________________________________________ImportanceSampleGGX_____________________________________________________________________________
float3 ImportanceSampleGGX( float2 Xi, float Roughness, float3 N )
{
  float a = Roughness * Roughness;
  float Phi = 2 * PI * Xi.x;
  float CosTheta = pow2( (1 - Xi.y) / ( 1 + (a*a - 1) * Xi.y ) );
  float SinTheta = pow2( 1 - CosTheta * CosTheta );
  float3 H;
  H.x = SinTheta * cos( Phi );
  H.y = SinTheta * sin( Phi );
  H.z = CosTheta;
  float3 UpVector = abs(N.z) < 0.999 ? float3(0,0,1) : float3(1,0,0);
  float3 TangentX = normalize( cross( UpVector, N ) );
  float3 TangentY = cross( N, TangentX );
  // Tangent to world space
  return TangentX * H.x + TangentY * H.y + N * H.z;
}

float3 SpecularIBL( float3 SpecularColor , float Roughness, float3 N, float3 V , samplerCUBE EnvMap  )
{
  float3 SpecularLighting = 0;
  const int NumSamples = 1024;
  for( int i = 0; i < NumSamples; i++ )
  {
    float2 Xi = Hammersley( i, NumSamples );
    float3 H = ImportanceSampleGGX( Xi, Roughness, N );
    float3 L = 2 * dot( V, H ) * H - V;
    float NoV = saturate( dot( N, V ) );
    float NoL = saturate( dot( N, L ) );
    float NoH = saturate( dot( N, H ) );
    float VoH = saturate( dot( V, H ) );
    if( NoL > 0 )
    {
     // float3 SampleColor = EnvMap.SampleLevel( EnvMapSampler , L, 0 ).rgb;
      float3 SampleColor = texCUBElod (EnvMap,float4(L,0)).rgb;
      float G = G_Smith( Roughness, NoV, NoL );
      float Fc = pow( 1 - VoH, 5 );
      float3 F = (1 - Fc) * SpecularColor + Fc;
      // Incident light = SampleColor * NoL
      // Microfacet specular = D*G*F / (4*NoL*NoV)
      // pdf = D * NoH / (4 * VoH)
      SpecularLighting += SampleColor * F * G * VoH / (NoH * NoV);
    }
  }
  return SpecularLighting / NumSamples;
}

//____________________________________________________________________________________________________________________________________________

inline half ROneMinusReflectivityFromMetallic(half metallic)
{
    // We'll need oneMinusReflectivity, so
    //   1-reflectivity = 1-lerp(dielectricSpec, 1, metallic) = lerp(1-dielectricSpec, 0, metallic)
    // store (1-dielectricSpec) in unity_ColorSpaceDielectricSpec.a, then
    //   1-reflectivity = lerp(alpha, 0, metallic) = alpha + metallic*(0 - alpha) = 
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
    half t = RPow5(1 - LdotH);  // ala Schlick interpoliation
    return specularColor + (1 - specularColor) * t;
}


//DirectionLight 1 Base Specular

inline half3 RFresnelLerp(half3 F0, half3 F90, half cosA)
{
    half t = RPow5(1 - cosA);   // ala Schlick interpoliation
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
//  c.rgb = (albedo * lightColor * diffFactor + specColor.rgb * spec * lightColor) * (atten);
//  c.a = alpha + spec * specColor.a;
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
    //float3 DIR0 = dir0 * LightIntensity0 * LightColor0;
     float3 DIR0 = dir0  * LightColor0;
    //float3 dir1 = max(0.0,dot( NorDir, -LightDir1 ));
    //float3 DIR1 = dir1 * LightIntensity1 * LightColor1 ;

    //float3 dir2 = max(dot( NorDir, -LightDir2 ) ,0.0);
    //float3 DIR2 = dir2 * LightIntensity2 * LightColor2 ;
    float3 finalLighing = DIR0 ;//+ DIR1 + DIR2 ;

    //float4x4 Full = float4x4((DIR0.xyzz),(DIR1.xyzz),(DIR2.xyzz),(finalLighing.xyzz));
    float4x4 Full = float4x4((DIR0.xyzz),(DIR0.xyzz),(DIR0.xyzz),(finalLighing.xyzz));
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
    //  atten = 1;
    //  lightDirection = normalize(p.xyz);
    // }
    // else {
       float3 lightvector = p - posWorld.xyz;
       float distance = length(lightvector);
       atten = 1/distance;
       lightDirection = normalize(lightvector);
//  }

    float3 finalPointlight = atten * saturate(dot(normaldir,lightDirection));
    
    return finalPointlight;
}





//———————————————————————————————————————————————————————————————TOOL——————————————————————————————————————————————————————————
//https://blog.selfshadow.com/sandbox/normals.html
////////////////normal calcular ---------------------------Normal--------------Normal-------------------------    
float3 blend_linear0(float4 n1, float4 n2){

float3 r  = normalize((fixed3 (n1.rg,1)*2 - 1 )+ (fixed3 (n2.rg,1)*2 - 1 ));
return r*0.5 + 0.5;


}   
float3 blend_linear(float4 n1, float4 n2){
    float3 r = (n1 + n2)*2 - 2;
    return normalize(r);
}

float3 blend_overlay(float4 n1, float4 n2){
    n1 = n1*4 - 2;
    float4 a = n1 >= 0 ? -1 : 1;
    float4 b = n1 >= 0 ?  1 : 0;
    n1 =  2*a + n1;
    n2 = n2*a + b;
    float3 r = n1*n2 - a;
    return normalize(r);
}

float3 blend_pd(float4 n1, float4 n2){
    n1 = n1*2 - 1;
    n2 = n2.xyzz*float4(2, 2, 2, 0) + float4(-1, -1, -1, 0);
    float3 r = n1.xyz*n2.z + n2.xyw*n1.z;
    return normalize(r);
}

float3 blend_whiteout(float4 n1, float4 n2){
    n1 = n1*2 - 1;
    n2 = n2*2 - 1;
    float3 r = float3(n1.xy + n2.xy, n1.z*n2.z);
    return normalize(r);
}

float3 blend_udn(float4 n1, float4 n2){
    float3 c = float3(2, 1, 0);
    float3 r;
    r = n2*c.yyz + n1.xyz;
    r =  r*c.xxx -  c.xxy;
    return normalize(r);
}

float3 blend_rnm(float4 n1, float4 n2){
    float3 t = n1.xyz*float3( 2,  2, 2) + float3(-1, -1,  0);
    float3 u = n2.xyz*float3(-2, -2, 2) + float3( 1,  1, -1);
    float3 r = t*dot(t, u) - u*t.z;
    return normalize(r);
}

float3 blend_unity(float4 n1, float4 n2){
    n1 = n1.xyzz*float4(2, 2, 2, -2) + float4(-1, -1, -1, 1);
    n2 = n2*2 - 1;
    float3 r;
    r.x = dot(n1.zxx,  n2.xyz);
    r.y = dot(n1.yzy,  n2.xyz);
    r.z = dot(n1.xyw, -n2.xyz);
    return normalize(r);
}


////////////////normal calcular ---------------------------NormalChannelSplit ---------------------------------------      
///specular and ao
float3 NormalSplitSA(fixed4 normalmap,out fixed Specular,out fixed AO){
    fixed2 normalrg = normalmap.rg;
    fixed3 realnormalmap = normalize(fixed3(normalrg,1))* 2-1;
     Specular = normalmap.g;
     AO = normalmap.a;
    return realnormalmap;
}
///metallic and gloss
float3 NormalSplitMG(fixed4 normalmap,out fixed metallic,out fixed gloss){
    fixed2 normalrg = normalmap.rg;
    fixed3 realnormalmap = normalize(fixed3(normalrg,1))* 2-1;
     metallic = normalmap.g;
     gloss = normalmap.a;
    return realnormalmap;
}

float3 NormalIntensity(float3 NormalMapTex){
     // float4 finalNormap = ((NormalMapTex.rgba.rg * _NormalIntensity),NormalMapTex.b,NormalMapTex.a);
     float2 finalNormap = NormalMapTex.rg *1;
     fixed3 o = fixed3(finalNormap.rg, NormalMapTex.b);
    return o;
}


float3 MatCap(float3 VertexNormal){ 
    float3 worldNorm = normalize(unity_WorldToObject[0].xyz * VertexNormal.x + unity_WorldToObject[1].xyz *VertexNormal.y + unity_WorldToObject[2].xyz * VertexNormal.z);
    worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
    return worldNorm.xyz*0.5+0.5; 
}


//float4 ColorSpace(float4 col){
  //  return pow(col , lerp (1/1.2, 1 / 2.2 , _Gama ));
//}

float3 GammaCorrection(float3 finalcolor,float gama){
  return pow(finalcolor,1/gama);
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


//----------------------------------------------------SUPER-SAMPLE-------------------------------------------------------------------------
//helper functions
float MixFunction(float i, float j, float x) {
   return  j * x + i * (1.0 - x);
} 
float2 MixFunction(float2 i, float2 j, float x){
   return  j * x + i * (1.0h - x);
}   
float3 MixFunction(float3 i, float3 j, float x){
   return  j * x + i * (1.0h - x);
}   
float MixFunction(float4 i, float4 j, float x){
   return  j * x + i * (1.0h - x);
} 
float sqrt(float x){
  return x*x; 
}
//------------------------------


//------------------------------------------------
//schlick functions
float SchlickFresnel(float i){
    float x = clamp(1.0-i, 0.0, 1.0);
    float x2 = x*x;
    return x2*x2*x;
}
float3 FresnelLerp (float3 x, float3 y, float d)
{
  float t = SchlickFresnel(d);  
  return lerp (x, y, t);
}

float3 SchlickFresnelFunction(float3 SpecularColor,float LdotH){
    return SpecularColor + (1 - SpecularColor)* SchlickFresnel(LdotH);
}

float SchlickIORFresnelFunction(float ior,float LdotH){
    float f0 = pow((ior-1)/(ior+1),2);
    return f0 +  (1 - f0) * SchlickFresnel(LdotH);
}
float SphericalGaussianFresnelFunction(float LdotH,float SpecularColor)
{ 
  float power = ((-5.55473 * LdotH) - 6.98316) * LdotH;
    return SpecularColor + (1 - SpecularColor)  * pow(2,power);
}

//__________________________________________________________________Fresbel
//u3d Schlick Fresnel
fixed3 F_Schlick_U3D(fixed NdotV,fixed3 F0){
   return F0+ (1 -F0) *pow(1.0-max(NdotV,0),5);
  //return F0 + (1 -F0) *pow (1 -  NdotV,F0);

}

//Specular_F_UE//Ue Schlick Fresnel
float3 F_Schlick( float3 SpecularColor, float VoH ){
  float Fc = Pow5( 1 - VoH );         // 1 sub, 3 mul
  //return Fc + (1 - Fc) * SpecularColor;   // 1 add, 3 mad
  // Anything less than 2% is physically impossible and is instead considered to be shadowing
  return saturate( 50.0 * SpecularColor.g ) * Fc + (1 - Fc) * SpecularColor;
}


//Standard Fresnel
float3 F_Fresnel( float3 SpecularColor, float VoH ){
  float3 SpecularColorSqrt = pow2( clamp( float3(0, 0, 0), float3(0.99, 0.99, 0.99), SpecularColor ) );
  float3 n = ( 1 + SpecularColorSqrt ) / ( 1 - SpecularColorSqrt );
  float3 g = pow2( n*n + VoH*VoH - 1 );
  return 0.5 * pow2( (g - VoH) / (g + VoH) ) * ( 1 + pow2( ((g+VoH)*VoH - 1) / ((g-VoH)*VoH + 1) ) );
}

//-----------------------------------------------



//-----------------------------------------------
//normal incidence reflection calculation
float F0 (float NdotL, float NdotV, float LdotH, float roughness){
// Diffuse fresnel
    float FresnelLight = SchlickFresnel(NdotL); 
    float FresnelView = SchlickFresnel(NdotV);
    float FresnelDiffuse90 = 0.5 + 2.0 * LdotH*LdotH * roughness;
   return  MixFunction(1, FresnelDiffuse90, FresnelLight) * MixFunction(1, FresnelDiffuse90, FresnelView);
}

//-----------------------------------------------



// ▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆ PBR_D-Normal Distribution Functions ▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆

float BlinnPhongNormalDistribution(float NdotH, float specularpower, float speculargloss){
    float Distribution = pow(max(0,NdotH),speculargloss) * specularpower;
    Distribution *= (2+specularpower) / (2*3.1415926535);
    return Distribution;
}

float BlinnPhongU3D(float NdotH, float specularpower, float speculargloss){
    float Distribution = pow(max(0,NdotH),speculargloss) * specularpower;
    // Distribution *= (2+specularpower) / (2*3.1415926535);
    return Distribution;
}


float PhongNormalDistribution(float RdotV, float specularpower, float speculargloss){
    float Distribution = pow(RdotV,speculargloss) * specularpower;
    Distribution *= (2+specularpower) / (2*3.1415926535);
    return Distribution;
}

float BeckmannNormalDistribution(float roughness, float NdotH)
{
    float roughnessSqr = roughness*roughness;
    float NdotHSqr = NdotH*NdotH;
    return max(0.000001,(1.0 / (3.1415926535*roughnessSqr*NdotHSqr*NdotHSqr))* exp((NdotHSqr-1)/(roughnessSqr*NdotHSqr)));
}

float GaussianNormalDistribution(float roughness, float NdotH)
{
    float roughnessSqr = roughness*roughness;
  float thetaH = acos(NdotH);
    return exp(-thetaH*thetaH/roughnessSqr);
}

float GGXNormalDistribution(float roughness, float NdotH)
{
    float roughnessSqr = roughness*roughness;
    float NdotHSqr = NdotH*NdotH;
    float TanNdotHSqr = (1-NdotHSqr)/NdotHSqr;
    return (1.0/3.1415926535) * sqrt(roughness/(NdotHSqr * (roughnessSqr + TanNdotHSqr)));
//    float denom = NdotHSqr * (roughnessSqr-1)

}

//UE4 采用GGX / Trowbridge-Reitz模型
float TrowbridgeReitzNormalDistribution(float NdotH, float roughness){
    float roughnessSqr = roughness*roughness;
    float Distribution = NdotH*NdotH * (roughnessSqr-1.0) + 1.0;
    return roughnessSqr / (3.1415926535 * Distribution*Distribution);
}

float TrowbridgeReitzAnisotropicNormalDistribution(float anisotropic, float NdotH, float HdotX, float HdotY){
  float aspect = pow2(1.0h-anisotropic * 0.9h);
  float X = max(0.001, sqrt(1.0-_Glossing)/aspect) * 5;
  float Y = max(0.001, sqrt(1.0-_Glossing)*aspect) * 5;
    return 1.0 / (3.1415926535 * X*Y * sqrt(sqrt(HdotX/X) + sqrt(HdotY/Y) + NdotH*NdotH));
}

float WardAnisotropicNormalDistribution(float anisotropic, float NdotL, float NdotV, float NdotH, float HdotX, float HdotY){
    float aspect = pow2(1.0h-anisotropic * 0.9h);
    float X = max(.001, sqrt(1.0-_Glossing)/aspect) * 5;
  float Y = max(.001, sqrt(1.0-_Glossing)*aspect) * 5;
    float exponent = -(sqrt(HdotX/X) + sqrt(HdotY/Y)) / sqrt(NdotH);
    float Distribution = 1.0 / ( 3.14159265 * X * Y * pow2(NdotL * NdotV));
    Distribution *= exp(exponent);
    return Distribution;
}


// ▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆ PBR_G-Geometric Shadowing Functions ▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆


float ImplicitGeometricShadowingFunction (float NdotL, float NdotV){
  float Gs =  (NdotL*NdotV);       
  return Gs;
}

float AshikhminShirleyGeometricShadowingFunction (float NdotL, float NdotV, float LdotH){
  float Gs = NdotL*NdotV/(LdotH*max(NdotL,NdotV));
  return  (Gs);
}

float AshikhminPremozeGeometricShadowingFunction (float NdotL, float NdotV){
  float Gs = NdotL*NdotV/(NdotL+NdotV - NdotL*NdotV);
  return  (Gs);
}

float DuerGeometricShadowingFunction (float3 lightDirection,float3 viewDirection, float3 normalDirection,float NdotL, float NdotV){
    float3 LpV = lightDirection + viewDirection;
    float Gs = dot(LpV,LpV) * pow(dot(LpV,normalDirection),-4);
    return  (Gs);
}

float NeumannGeometricShadowingFunction (float NdotL, float NdotV){
  float Gs = (NdotL*NdotV)/max(NdotL, NdotV);       
  return  (Gs);
}

float KelemenGeometricShadowingFunction (float NdotL, float NdotV, float LdotH, float VdotH){
//  float Gs = (NdotL*NdotV)/ (LdotH * LdotH);           //this
  float Gs = (NdotL*NdotV)/(VdotH * VdotH);       //or this?
  return   (Gs);
}

float ModifiedKelemenGeometricShadowingFunction (float NdotV, float NdotL, float roughness)
{
  float c = 0.797884560802865; // c = pow2(2 / Pi)
  float k = roughness * roughness * c;
  float gH = NdotV  * k +(1-k);
  return (gH * gH * NdotL);
}

float CookTorrenceGeometricShadowingFunction (float NdotL, float NdotV, float VdotH, float NdotH){
  float Gs = min(1.0, min(2*NdotH*NdotV / VdotH, 2*NdotH*NdotL / VdotH));
  return  (Gs);
}

float WardGeometricShadowingFunction (float NdotL, float NdotV, float VdotH, float NdotH){
  float Gs = pow( NdotL * NdotV, 0.5);
  return  (Gs);
}

float KurtGeometricShadowingFunction (float NdotL, float NdotV, float VdotH, float alpha){
  float Gs =  (VdotH*pow(NdotL*NdotV, alpha))/ NdotL * NdotV;
  return  (Gs);
}

//SmithModelsBelow
//Gs = F(NdotL) * F(NdotV);

float WalterEtAlGeometricShadowingFunction (float NdotL, float NdotV, float alpha){
    float alphaSqr = alpha*alpha;
    float NdotLSqr = NdotL*NdotL;
    float NdotVSqr = NdotV*NdotV;
    float SmithL = 2/(1 + pow2(1 + alphaSqr * (1-NdotLSqr)/(NdotLSqr)));
    float SmithV = 2/(1 + pow2(1 + alphaSqr * (1-NdotVSqr)/(NdotVSqr)));
  float Gs =  (SmithL * SmithV);
  return Gs;
}

float BeckmanGeometricShadowingFunction (float NdotL, float NdotV, float roughness){
    float roughnessSqr = roughness*roughness;
    float NdotLSqr = NdotL*NdotL;
    float NdotVSqr = NdotV*NdotV;
    float calulationL = (NdotL)/(roughnessSqr * pow2(1- NdotLSqr));
    float calulationV = (NdotV)/(roughnessSqr * pow2(1- NdotVSqr));
    float SmithL = calulationL < 1.6 ? (((3.535 * calulationL) + (2.181 * calulationL * calulationL))/(1 + (2.276 * calulationL) + (2.577 * calulationL * calulationL))) : 1.0;
    float SmithV = calulationV < 1.6 ? (((3.535 * calulationV) + (2.181 * calulationV * calulationV))/(1 + (2.276 * calulationV) + (2.577 * calulationV * calulationV))) : 1.0;
  float Gs =  (SmithL * SmithV);
  return Gs;
}

float GGXGeometricShadowingFunction (float NdotL, float NdotV, float roughness){
    float roughnessSqr = roughness*roughness;
    float NdotLSqr = NdotL*NdotL;
    float NdotVSqr = NdotV*NdotV;
    float SmithL = (2 * NdotL)/ (NdotL + pow2(roughnessSqr + ( 1-roughnessSqr) * NdotLSqr));
    float SmithV = (2 * NdotV)/ (NdotV + pow2(roughnessSqr + ( 1-roughnessSqr) * NdotVSqr));
  float Gs =  (SmithL * SmithV) ;
  return Gs;
}



float SchlickGeometricShadowingFunction (float NdotL, float NdotV, float roughness)
{
    float roughnessSqr = roughness*roughness;
  float SmithL = (NdotL)/(NdotL * (1-roughnessSqr) + roughnessSqr);
  float SmithV = (NdotV)/(NdotV * (1-roughnessSqr) + roughnessSqr);
  return (SmithL * SmithV); 
}


float SchlickBeckmanGeometricShadowingFunction (float NdotL, float NdotV, float roughness){
    float roughnessSqr = roughness*roughness;
    float k = roughnessSqr * 0.797884560802865;
    float SmithL = (NdotL)/ (NdotL * (1- k) + k);
    float SmithV = (NdotV)/ (NdotV * (1- k) + k);
  float Gs =  (SmithL * SmithV);
  return Gs;
}

float SchlickGGXGeometricShadowingFunction (float NdotL, float NdotV, float roughness){
    float k = roughness / 2;
    float SmithL = (NdotL)/ (NdotL * (1- k) + k);
    float SmithV = (NdotV)/ (NdotV * (1- k) + k);
  float Gs =  (SmithL * SmithV);
  return Gs;
}


float GeometrySchlickGGX(float NdotV, float k){
    float nom   = NdotV;
    float denom = NdotV * (1.0 - k) + k;
    return nom / denom;
}

float G_Func(float NdotL, float NdotV, float roughness) {
    float k = (roughness + 1) * (roughness + 1) * 0.125;
    float ggx1 = GeometrySchlickGGX(NdotV, k);
    float ggx2 = GeometrySchlickGGX(NdotL, k);
    return ggx1 * ggx2;
}


//--------------------------------------------------Wind---------------------------------------------

fixed4 TreeWind(fixed4 vertexColor,fixed3 normaldir,fixed leafWindPower,fixed4 leafWindDir,fixed leafWindAtt,fixed trunkWindPower,fixed trunkWindAtt){
    fixed a = (vertexColor.r * PI +_Time.y*leafWindPower);
    fixed b = sin (a *3)*0.2 + sin(a);
    fixed k = cos( a *5);
    fixed d = b -k ;
    fixed4 e = vertexColor.r * d *  (normalize(leafWindDir +  normaldir.xyzz)) * leafWindAtt;
    fixed f = _Time.y * trunkWindPower;
    fixed g = sin (f) *  trunkWindAtt * vertexColor.a;
    fixed h = cos (f) * 0.5 * trunkWindAtt * vertexColor.a;
    fixed3 i = fixed3(g,0,h);
    fixed4 j = e + i.xyzz;
    return j;
  }

  float4 VertexBillBoard(float4 vertex){
    float4 ori=mul(UNITY_MATRIX_MV,float4(0,0,0,1));
    float4 vt=vertex;
    vt.y=vt.z;
    vt.z=0;
    vt.xyz+=ori.xyz;
    vertex=mul(UNITY_MATRIX_P,vt) ;
    return vertex;
  }

#endif


