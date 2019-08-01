#include "ColorGrading.cginc"

// Grain
half2 _Grain_Params1; // x: lum_contrib, y: intensity
half4 _Grain_Params2; // x: xscale, h: yscale, z: xoffset, w: yoffset
sampler2D _GrainTex;


float4 _DitheringCoords;

float3 UberSecondPass(half3 color, float2 uv)
{
 

     

    return color;
}
