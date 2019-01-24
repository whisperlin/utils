
float sqr(float x){
	return x*x; 
}

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
 //--------------------------



//-----------------------------------------------
//法线正态分布函数集.
inline float UnityGGXTerm (float NdotH, float roughness)
{
    float a2 = roughness * roughness;
    float d = (NdotH * a2 - NdotH) * NdotH + 1.0f; // 2 mad
    return UNITY_INV_PI * a2 / (d * d + 1e-7f); // This function is not intended to be running on Mobile,
                                            // therefore epsilon is smaller than what can be represented by half
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
    return (1.0/3.1415926535) * sqr(roughness/(NdotHSqr * (roughnessSqr + TanNdotHSqr)));
//    float denom = NdotHSqr * (roughnessSqr-1)

}

float TrowbridgeReitzNormalDistribution(float NdotH, float roughness){
    float roughnessSqr = roughness*roughness;
    float Distribution = NdotH*NdotH * (roughnessSqr-1.0) + 1.0;
    return roughnessSqr / (3.1415926535 * Distribution*Distribution);
}

float TrowbridgeReitzAnisotropicNormalDistribution(float anisotropic, float NdotH, float HdotX, float HdotY,float _Glossiness){
	float aspect = sqrt(1.0h-anisotropic * 0.9h);
	float X = max(.001, sqr(1.0-_Glossiness)/aspect) * 5;
 	float Y = max(.001, sqr(1.0-_Glossiness)*aspect) * 5;
    return 1.0 / (3.1415926535 * X*Y * sqr(sqr(HdotX/X) + sqr(HdotY/Y) + NdotH*NdotH));
}

float WardAnisotropicNormalDistribution(float anisotropic, float NdotL, float NdotV, float NdotH, float HdotX, float HdotY,float _Glossiness){
    float aspect = sqrt(1.0h-anisotropic * 0.9h);
    float X = max(.001, sqr(1.0-_Glossiness)/aspect) * 5;
 	float Y = max(.001, sqr(1.0-_Glossiness)*aspect) * 5;
    float exponent = -(sqr(HdotX/X) + sqr(HdotY/Y)) / sqr(NdotH);
    float Distribution = 1.0 / ( 3.14159265 * X * Y * sqrt(NdotL * NdotV));
    Distribution *= exp(exponent);
    return Distribution;
}


 

//--------------------------



//-----------------------------------------------
//几何函数集.




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
//	float Gs = (NdotL*NdotV)/ (LdotH * LdotH);           //this
	float Gs = (NdotL*NdotV)/(VdotH * VdotH);       //or this?
	return   (Gs);
}

float ModifiedKelemenGeometricShadowingFunction (float NdotV, float NdotL, float roughness)
{
	float c = 0.797884560802865; // c = sqrt(2 / Pi)
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
// Ref: http://jcgt.org/published/0003/02/03/paper.pdf
inline half UnitySmithJointGGXVisibilityTerm (half NdotL, half NdotV, half roughness)
{
#if 0
    // Original formulation:
    //  lambda_v    = (-1 + sqrt(a2 * (1 - NdotL2) / NdotL2 + 1)) * 0.5f;
    //  lambda_l    = (-1 + sqrt(a2 * (1 - NdotV2) / NdotV2 + 1)) * 0.5f;
    //  G           = 1 / (1 + lambda_v + lambda_l);

    // Reorder code to be more optimal
    half a          = roughness;
    half a2         = a * a;

    half lambdaV    = NdotL * sqrt((-NdotV * a2 + NdotV) * NdotV + a2);
    half lambdaL    = NdotV * sqrt((-NdotL * a2 + NdotL) * NdotL + a2);
    // Simplify visibility term: (2.0f * NdotL * NdotV) /  ((4.0f * NdotL * NdotV) * (lambda_v + lambda_l + 1e-5f));
    return 0.5f / (lambdaV + lambdaL + 1e-5f);  // This function is not intended to be running on Mobile,
                                                // therefore epsilon is smaller than can be represented by half
#else
    // Approximation of the above formulation (simplify the sqrt, not mathematically correct but close enough)
    half a = roughness;
    half lambdaV = NdotL * (NdotV * (1 - a) + a);
    half lambdaL = NdotV * (NdotL * (1 - a) + a);

    return 0.5f / (lambdaV + lambdaL + 1e-5f);
#endif
}
//SmithModelsBelow
//Gs = F(NdotL) * F(NdotV);

float WalterEtAlGeometricShadowingFunction (float NdotL, float NdotV, float alpha){
    float alphaSqr = alpha*alpha;
    float NdotLSqr = NdotL*NdotL;
    float NdotVSqr = NdotV*NdotV;
    float SmithL = 2/(1 + sqrt(1 + alphaSqr * (1-NdotLSqr)/(NdotLSqr)));
    float SmithV = 2/(1 + sqrt(1 + alphaSqr * (1-NdotVSqr)/(NdotVSqr)));
	float Gs =  (SmithL * SmithV);
	return Gs;
}

float BeckmanGeometricShadowingFunction (float NdotL, float NdotV, float roughness){
    float roughnessSqr = roughness*roughness;
    float NdotLSqr = NdotL*NdotL;
    float NdotVSqr = NdotV*NdotV;
    float calulationL = (NdotL)/(roughnessSqr * sqrt(1- NdotLSqr));
    float calulationV = (NdotV)/(roughnessSqr * sqrt(1- NdotVSqr));
    float SmithL = calulationL < 1.6 ? (((3.535 * calulationL) + (2.181 * calulationL * calulationL))/(1 + (2.276 * calulationL) + (2.577 * calulationL * calulationL))) : 1.0;
    float SmithV = calulationV < 1.6 ? (((3.535 * calulationV) + (2.181 * calulationV * calulationV))/(1 + (2.276 * calulationV) + (2.577 * calulationV * calulationV))) : 1.0;
	float Gs =  (SmithL * SmithV);
	return Gs;
}

float GGXGeometricShadowingFunction (float NdotL, float NdotV, float roughness){
    float roughnessSqr = roughness*roughness;
    float NdotLSqr = NdotL*NdotL;
    float NdotVSqr = NdotV*NdotV;
    float SmithL = (2 * NdotL)/ (NdotL + sqrt(roughnessSqr + ( 1-roughnessSqr) * NdotLSqr));
    float SmithV = (2 * NdotV)/ (NdotV + sqrt(roughnessSqr + ( 1-roughnessSqr) * NdotVSqr));
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


//------------------------------


//------------------------------------------------
//schlick functions

inline half3 UnityFresnelTerm (half3 F0, half cosA)
{
    half t = Pow5 (1 - cosA);   // ala Schlick interpoliation
    return F0 + (1-F0) * t;
}
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