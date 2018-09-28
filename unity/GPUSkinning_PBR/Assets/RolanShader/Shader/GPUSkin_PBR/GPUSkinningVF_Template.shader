Shader "GPUSkinning/GPUSkinningVF_Template"
{
    Properties
    {
        _Color("漫射颜色", Color) = (1, 1, 1, 1) _MainTex("漫射贴图", 2D) = "white" {}
        _BumpMap("法线贴图", 2D) = "bump" {}
        _NormalIntensity("法线强度", Range(0, 3)) = 1 
        _Metallic("金属贴图", 2D) = "white" {}
        _MetallicPower("PBR金属强度", Range(0, 1)) = 0 
        _GlossPower("PBR高光强度", Range(0, 1)) = 1 
        IBL_Blur("环境球模糊强度", Range(0, 10)) = 1 
        _IBL_Diffuse("环境球", Cube) = "_Skybox" {}
		_IBL_Color("Lod环境球颜色", Color) = (0.25, 0.25, 0.25, 1)
        IBL_Intensity("环境球强度", Range(0, 10)) = 1 
        _AmbientLight("环境光", Color) = (0.5, 0.5, 0.5, 1) 
        SBL_Intensity("反射球强度", Range(0, 10)) = 0 
        _SkinColor("皮肤颜色", Color) = (1, 1, 1, 1) 
        _SkinIntensity("皮肤强度", Range(0, 20)) = 0 
        _SkinMap("皮肤贴图", 2D) = "white" {}
        _EmssionMap("自发光", 2D) = "white" {}
        _EmissionIntensity("自发光强度", Range(0, 2)) = 0
        _BlinnPhongSP("高光强度", Range(0, 10)) = 1 
        _BP_Gloss("高光范围", Range(0, 1)) = 0 
        GlobeLight("全局光强度", Range(0, 1)) = 1
        _Gama("Gama",Range(-1, 1)) = 0
        _Cutoff("透明", Range(0, 1)) = 0.5
 
 
        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 300
 
        // It seems Blend command is getting overridden later
        // in the processing of  Surface shader.
        // Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
 
    
	// ------------------------------------------------------------
	// Surface shader code generated out of a CGPROGRAM block:
	

	// ---- forward rendering base pass:
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM
		// compile directives
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma target 3.0
		#pragma exclude_renderers gles
		#pragma shader_feature _NORMALMAP
		#pragma shader_feature _ALPHATEST_ON
		#pragma shader_feature _EMISSION
		#pragma shader_feature _METALLICGLOSSMAP
		#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
		#pragma skip_variants _PARALLAXMAP _DETAIL_MULX2 _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
		#pragma multi_compile_instancing
		#pragma multi_compile ROOTON_BLENDOFF ROOTON_BLENDON_CROSSFADEROOTON ROOTON_BLENDON_CROSSFADEROOTOFF ROOTOFF_BLENDOFF ROOTOFF_BLENDON_CROSSFADEROOTON ROOTOFF_BLENDON_CROSSFADEROOTOFF
		#pragma multi_compile_fwdbase nodynlightmap nolightmap
 
		 
		#define UNITY_PASS_FORWARDBASE
 

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl
		#define WorldNormalVector(data,normal) normal
 
        #include "GPUSkinningSurface.cginc"
		#include "GPUSkinningInclude.cginc"
		#include "RolantinCG_REMOVE_REDEFINE.cginc"
 
 

        uniform samplerCUBE _IBL_Diffuse;
        uniform float4 _IBL_Color;


        //uniform samplerCUBE _SBL;
        uniform sampler2D _EmssionMap;
        uniform float4 _EmssionMap_ST;
        uniform float _EmissionIntensity;


        float4 _emissioncolor;
        float4 _SkinColor;
        float GlobeLight;
        float _emission;
        uniform float4 _Skin;
        uniform float _SkinIntensity;
        uniform sampler2D _SkinMap;
        uniform float4 _SkinMap_ST;

        uniform float _BlinnPhongSP;

        #if USE_COMBINE_CHANNEL_ON 
        sampler2D _LightMask;
        #elif USE_SPLIT_CHANNEL_ON 
        sampler2D _LightMask_R,
        _LightMask_G,
        _LightMask_B,
        _LightMask_A;
        #endif

 
	    void myvert (inout appdata_vert v, out float2 uv) 
	    {
		  uv = TRANSFORM_TEX(v.uv0, _MainTex); // Always source from uv0
		   {
				float4 normal = float4(v.normal, 0);
				float4 tangent = float4(v.tangent.xyz, 0);

				float4 pos = skin2(v.vertex, v.uv1, v.uv2);
				normal = skin2(normal, v.uv1, v.uv2);
				tangent = skin2(tangent, v.uv1, v.uv2);

				v.vertex = pos;
				v.normal = normal.xyz;
				v.tangent = float4(tangent.xyz, v.tangent.w);
		   }
		}
 
		struct VertexOutput {
		        float4 pos: SV_POSITION;
		        float3 normal: NORMAL;
		        float4 tangent: TANGENT;
		        float2 uv0: TEXCOORD0;
		        float4 posWorld: TEXCOORD1;
		        float3 normalDir: TEXCOORD2;
		        float3 tangentDir: TEXCOORD3;
		        float3 bitangentDir: TEXCOORD4;
		};

		// vertex shader
		VertexOutput vert_surf (appdata_vert v) {
			  UNITY_SETUP_INSTANCE_ID(v);
			  VertexOutput o;
			   float2 uv;
			  myvert (v, o.uv0);
			  o.pos = UnityObjectToClipPos(v.vertex);
			   
		  return o;
		}

		// fragment shader
		fixed4 frag_surf (VertexOutput i) : SV_Target {
		  float4 _MainTex_var = ColorSpace (tex2D(_MainTex, TRANSFORM_TEX(i.uv0, _MainTex)));
		  return _MainTex_var;
		}

		ENDCG
	}
 
        UsePass "Standard/SHADOWCASTER"
    }

    FallBack Off
    //CustomEditor "GPUSkinningStandardShaderGUI"
}