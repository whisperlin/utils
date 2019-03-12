// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "ArmPhysically-Based-Lighting" {
    Properties {
   _BumpMap("Normal Map", 2D) = "bump" {}
	_Glossiness("Glossiness",Range(0,1)) = 1
 
	_Metallic("Metallicness",Range(0,1)) = 1
	 
	 
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
            #pragma multi_compile_fwdbase_fullshadows
             #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

            #pragma multi_compile _NORMALDISTMODEL_BLINNPHONG _NORMALDISTMODEL_PHONG _NORMALDISTMODEL_BECKMANN _NORMALDISTMODEL_GAUSSIAN _NORMALDISTMODEL_GGX _NORMALDISTMODEL_TROWBRIDGEREITZ _NORMALDISTMODEL_TROWBRIDGEREITZANISOTROPIC _NORMALDISTMODEL_WARD
            #pragma multi_compile _GEOSHADOWMODEL_ASHIKHMINSHIRLEY _GEOSHADOWMODEL_ASHIKHMINPREMOZE _GEOSHADOWMODEL_DUER_GEOSHADOWMODEL_NEUMANN _GEOSHADOWMODEL_KELEMAN _GEOSHADOWMODEL_MODIFIEDKELEMEN _GEOSHADOWMODEL_COOK _GEOSHADOWMODEL_WARD _GEOSHADOWMODEL_KURT 
            #pragma multi_compile _SMITHGEOSHADOWMODEL_NONE _SMITHGEOSHADOWMODEL_WALTER _SMITHGEOSHADOWMODEL_BECKMAN _SMITHGEOSHADOWMODEL_GGX _SMITHGEOSHADOWMODEL_SCHLICK _SMITHGEOSHADOWMODEL_SCHLICKBECKMAN _SMITHGEOSHADOWMODEL_SCHLICKGGX _SMITHGEOSHADOWMODEL_IMPLICIT
            #pragma multi_compile _FRESNELMODEL_SCHLICK _FRESNELMODEL_SCHLICKIOR _FRESNELMODEL_SPHERICALGAUSSIAN
            #pragma multi_compile  _ENABLE_NDF_OFF _ENABLE_NDF_ON
            #pragma multi_compile  _ENABLE_G_OFF _ENABLE_G_ON
            #pragma multi_compile  _ENABLE_F_OFF _ENABLE_F_ON
            #pragma multi_compile  _ENABLE_D_OFF _ENABLE_D_ON
            #pragma multi_compile  _ENABLE_MC_OFF _ENABLE_MC_ON
            #pragma multi_compile  _ENABLE_DIFF_OFF _ENABLE_DIFF_ON
            #pragma multi_compile  _DEBUGMOD_OFF _DEBUGMOD_ON



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
float4 frag(VertexOutput i) : COLOR {
 
	 half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv0));
 
     float3 wNormal = i.normalDir;
     float3 wTangent = i.tangentDir;
     float3 wBitangent = i.bitangentDir;

     half3 tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
     half3 tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
     half3 tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
 
      float3 normalDirection ;
      normalDirection.x = dot(tspace0, tnormal);
      normalDirection.y = dot(tspace1, tnormal);
      normalDirection.z = dot(tspace2, tnormal);
 
      float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
 
 
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

      float roughness = 1-(_Glossiness * _Glossiness);
      return ArmBRDF(roughness,NdotH,LdotH);
}
ENDCG
}
}
FallBack "Legacy Shaders/Diffuse"
}