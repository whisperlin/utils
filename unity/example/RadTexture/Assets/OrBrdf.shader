// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Or Physically-Based-Lighting" {
    Properties {
   _BumpMap("Normal Map", 2D) = "bump" {}
	_Glossiness("Glossiness",Range(0,1)) = 1
 
	_Metallic("Metallicness",Range(0,1)) = 1
	 
	 [KeywordEnum(ARM, FULL,OR1,OR2)] _TYPE("SPE TYPE", Float) = 0
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
			#include "optimized-ggx.cginc"
            #pragma multi_compile_fwdbase_fullshadows
       

			#pragma multi_compile  _TYPE_ARM _TYPE_FULL _TYPE_OR1 _TYPE_OR2



            #pragma target 3.0
            
			float _Glossiness;
			float _Metallic;


struct VertexInput {
    float4 vertex : POSITION;       //local vertex position
    float3 normal : NORMAL;         //normal direction
    float4 tangent : TANGENT;       //tangent direction    
    float2 texcoord0 : TEXCOORD0;   //uv coordinates
    float2 texcoord1 : TEXCOORD1;   //lightmap uv coordinates
};

struct VertexOutput {
    float4 pos : SV_POSITION;              //screen clip space position and depth
    float2 uv0 : TEXCOORD0;                //uv coordinates
    float2 uv1 : TEXCOORD1;                //lightmap uv coordinates
 

//below we create our own variables with the texcoord semantic. 
    float3 normalDir : TEXCOORD3;          //normal direction   
    float3 posWorld : TEXCOORD4;          //normal direction   
    float3 tangentDir : TEXCOORD5;
    float3 bitangentDir : TEXCOORD6;
    LIGHTING_COORDS(7,8)                   //this initializes the unity lighting and shadow
    UNITY_FOG_COORDS(9)                    //this initializes the unity fog
     #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
      float4 ambientOrLightmapUV : TEXCOORD10;
     #endif
};

VertexOutput vert (VertexInput v) {
     VertexOutput o = (VertexOutput)0;           
     o.uv0 = v.texcoord0;
     o.uv1 = v.texcoord1;
     #ifdef LIGHTMAP_ON
        o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        o.ambientOrLightmapUV.zw = 0;
    #elif UNITY_SHOULD_SAMPLE_SH
    #endif
    #ifdef DYNAMICLIGHTMAP_ON
        o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif

     o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
     o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
     o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
     o.pos = UnityObjectToClipPos(v.vertex);
     o.posWorld = mul(unity_ObjectToWorld, v.vertex);
 

     UNITY_TRANSFER_FOG(o,o.pos);
     TRANSFER_VERTEX_TO_FRAGMENT(o)
     return o;
}
sampler2D _BumpMap;
float  _BumpMap_ST;

float ArmBRDF(float roughness ,float NdotH ,float LdotH)
{
	float n4 = roughness*roughness*roughness*roughness;
	float c = NdotH*NdotH   *   (n4-1) +1;
	float b = 4*3.14*       c*c  *     LdotH*LdotH     *(roughness+0.5);
	return n4/b;

}
float ArmBRDFEnv(float roughness ,float NdotV )
{
	float a1 = (1-max(roughness,NdotV));
	return a1*a1*a1;

}
 
 


float4 frag(VertexOutput i) : COLOR {
 
	 half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv0));
 
     float3 wNormal =  normalize ( i.normalDir);
     float3 wTangent = normalize(i.tangentDir);
     float3 wBitangent = normalize(i.bitangentDir);

     half3 tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
     half3 tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
     half3 tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
 
      float3 normalDirection ;
      normalDirection.x = dot(tspace0, tnormal);
      normalDirection.y = dot(tspace1, tnormal);
      normalDirection.z = dot(tspace2, tnormal);
 
      float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
 
 
      float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz  - i.posWorld.xyz,_WorldSpaceLightPos0.w));

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

      float roughness = 1-(_Glossiness * _Glossiness);
      
#if _TYPE_ARM 
	  return ArmBRDF(roughness, NdotH, LdotH);
#elif _TYPE_FULL
	  float F0 = _Metallic;
	 
	  return LightingFuncGGX_REF(normalize(normalDirection), normalize(viewDirection), normalize(lightDirection) , roughness, F0);
#elif _TYPE_OR1
	  float F0 = _Metallic;
	  return LightingFuncGGX_OPT1(normalize(normalDirection), normalize(viewDirection), normalize(lightDirection), roughness, F0);
 
#elif _TYPE_OR2
	  float F0 = _Metallic;
	  return LightingFuncGGX_OPT2(normalize(normalDirection), normalize(viewDirection), normalize(lightDirection), roughness, F0);
#endif
 
  	  //return ArmBRDFEnv(roughness,NdotV);
     
}
ENDCG
}
}
FallBack "Legacy Shaders/Diffuse"
}