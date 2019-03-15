


#define EPSILON_COEF  1e-4
#define M_PI   3.14159265
#define  M_2PI   (2.0 * 3.14159265)
#define M_INV_PI   0.31830988
#define M_INV_LOG2   1.442695
#define M_GOLDEN_RATIO  1.618034


uniform float nbSamples;
uniform float environment_exposure;

samplerCUBE environment_texture;
uniform float maxLod;
//: param custom {
//:   "default": 1.3,
//:   "label": "Horizon Fading",
//:   "min": 0.0,
//:   "max": 2.0,
//:   "group": "Common Parameters"
//: }
uniform float horizonFade;


//random
float fibonacci1D(int i)
{
 
  return frac(   (  1.0 + i ) * M_GOLDEN_RATIO);
 
}
float2 fibonacci2D(int i, int nbSamples)
{
  return float2(
    (float(i)+0.5) / float(nbSamples),
    fibonacci1D(i)
  );
}





struct LocalVectors {
  float3 vertexNormal;
  float3 tangent; // world
  float3 bitangent;//world
  float3 normal;//world
  float3  eye;//world space eye direct
};


float3 generateDiffuseColor(float3 baseColor, float metallic)
{
  return baseColor * (1.0 - metallic);
}

float3 generateSpecularColor(float specularLevel, float3 baseColor, float metallic)
{
  float  s = 0.08 * specularLevel;
  return lerp(float3(s,s,s), baseColor, metallic);
}

//Remove AO and shadows on glossy metallic surfaces (close to mirrors)
float specularOcclusionCorrection(float diffuseOcclusion, float metallic, float roughness)
{
  return lerp(diffuseOcclusion, 1.0, metallic * (1.0 - roughness) * (1.0 - roughness));
}


//pbr



 

float normal_distrib(
  float ndh,
  float Roughness)
{
  // use GGX / Trowbridge-Reitz, same as Disney and Unreal 4
  // cf http://blog.selfshadow.com/publications/s2013-shading-course/karis/s2013_pbs_epic_notes_v2.pdf p3
  float alpha = Roughness * Roughness;
  float tmp = alpha / max(1e-8,(ndh*ndh*(alpha*alpha-1.0)+1.0));
  return tmp * tmp * M_INV_PI;
}

float3 fresnel(
  float vdh,
  float3 F0)
{
  // Schlick with Spherical Gaussian approximation
  // cf http://blog.selfshadow.com/publications/s2013-shading-course/karis/s2013_pbs_epic_notes_v2.pdf p3
  float sphg = exp2((-5.55473*vdh - 6.98316) * vdh);
  return F0 + (float3(1.0,1.0,1.0) - F0) * sphg;
}

float G1(
  float ndw, // w is either Ln or Vn
  float k)
{
  // One generic factor of the geometry function divided by ndw
  // NB : We should have k > 0
  return 1.0 / ( ndw*(1.0-k) +  k );
}

float visibility(
  float ndl,
  float ndv,
  float Roughness)
{
  // Schlick with Smith-like choice of k
  // cf http://blog.selfshadow.com/publications/s2013-shading-course/karis/s2013_pbs_epic_notes_v2.pdf p3
  // visibility is a Cook-Torrance geometry function divided by (n.l)*(n.v)
  float k = max(Roughness * Roughness * 0.5, 1e-5);
  return G1(ndl,k)*G1(ndv,k);
}

float3 cook_torrance_contrib(
  float vdh,
  float ndh,
  float ndl,
  float ndv,
  float3 Ks,
  float Roughness)
{
  // This is the contribution when using importance sampling with the GGX based
  // sample distribution. This means ct_contrib = ct_brdf / ggx_probability
  return fresnel(vdh,Ks) * (visibility(ndl,ndv,Roughness) * vdh * ndl / ndh );
}

float3 importanceSampleGGX(float2 Xi, float3 T, float3 B, float3 N, float roughness)
{
  float a = roughness*roughness;
  float cosT = sqrt((1.0-Xi.y)/(1.0+(a*a-1.0)*Xi.y));
  float sinT = sqrt(1.0-cosT*cosT);
  float phi = 2.0*M_PI*Xi.x;
  return
    T * (sinT*cos(phi)) +
    B * (sinT*sin(phi)) +
    N *  cosT;
}

float probabilityGGX(float ndh, float vdh, float Roughness)
{
  return normal_distrib(ndh, Roughness) * ndh / (4.0*vdh);
}

float distortion(float3 Wn)
{
  // Computes the inverse of the solid angle of the (differential) pixel in
  // the cube map pointed at by Wn
  float sinT = sqrt(1.0-Wn.y*Wn.y);
  return sinT;
}

float computeLOD(float3 Ln, float p)
{
  return max(0.0, (maxLod-1.5) - 0.5 * log2(float(nbSamples) * p * distortion(Ln)));
}



//Horizon fading trick from http://marmosetco.tumblr.com/post/81245981087
float horizonFading(float ndl, float horizonFade)
{
  float horiz = clamp(1.0 + horizonFade * ndl, 0.0, 1.0);
  return horiz * horiz;
}



float3 envSampleLOD(float3 dir, float lod)
{
	return texCUBElod(environment_texture,float4(dir,lod)).rgb*environment_exposure;
  // WORKAROUND: Intel GLSL compiler for HD5000 is bugged on OSX:
  // https://bugs.chromium.org/p/chromium/issues/detail?id=308366
  // It is necessary to replace atan(y, -x) by atan(y, -1.0 * x) to force
  // the second parameter to be interpreted as a float

  //float2 pos = M_INV_PI * float2(atan(-dir.z, -1.0 * dir.x), 2.0 * asin(dir.y));
  //pos = 0.5 * pos + float2(0.5);
  //pos.x += environment_rotation;
  //return textureLod(environment_texture, pos, lod).rgb * environment_exposure;*/
}

float3 pbrComputeSpecular(LocalVectors vectors, float3 specColor, float roughness)
{
  float3 radiance = float3(0,0,0);
  float ndv = dot(vectors.eye, vectors.normal);

  for(int i=0; i<nbSamples; ++i)
  {
    float2 Xi = fibonacci2D(i, nbSamples);
    float3 Hn = importanceSampleGGX(
      Xi, vectors.tangent, vectors.bitangent, vectors.normal, roughness);
    float3 Ln = -reflect(vectors.eye,Hn);

    float fade = horizonFading(dot(vectors.vertexNormal, Ln), horizonFade);

    float ndl = dot(vectors.normal, Ln);
    ndl = max( 1e-8, ndl );
    float vdh = max(1e-8, dot(vectors.eye, Hn));
    float ndh = max(1e-8, dot(vectors.normal, Hn));
    float lodS = roughness < 0.01 ? 0.0 : computeLOD(Ln, probabilityGGX(ndh, vdh, roughness));
    radiance += fade * envSampleLOD(Ln, lodS) *
      cook_torrance_contrib(vdh, ndh, ndl, ndv, specColor, roughness);
  }
  // Remove occlusions on shiny reflections
  radiance /= float(nbSamples);

  return radiance;
}

 
 