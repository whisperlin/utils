Shader "GPUSkinning/GPUSkinning_PBR1"
{
    Properties
    {

    	 [KeywordEnum(TOP, MID, LOW)] _SHADER_LEVEL("效果等级", Float) = 1
        _Color("漫射颜色", Color) = (1, 1, 1, 1) _MainTex("漫射贴图", 2D) = "white" {}
        _BumpMap("法线贴图", 2D) = "bump" {}
        _NormalIntensity("法线强度", Range(0, 3)) = 1 

        //_DISABLE_IBL_DIFFUSE
        _MetallicTex("金属贴图", 2D) = "black" {}
        _MetallicPower("PBR金属强度", Range(0, 1)) =1
        _GlossPower("PBR高光强度", Range(0, 3)) =1
           powersp ("高光强度", Range(0, 3)) = 1


        IBL_Blur("环境球模糊强度", Range(0, 10)) = 6
         [Toggle(_DISABLE_IBL_DIFFUSE)] _DISABLE_IBL_DIFFUSE("禁用环境球", Int) = 0
        _IBL_Diffuse("环境球", Cube) = "_Skybox" {}
		_IBL_Color("Lod环境球颜色", Color) =  (0.25, 0.25, 0.25, 1)
        IBL_Intensity("环境球强度", Range(0, 10)) = 0.2
        _AmbientLight("环境光", Color) =(1, 0.96, 0.83, 1)
        SBL_Intensity("反射球强度", Range(0, 10)) = 0.4

        _EmssionMap("自发光", 2D) = "white" {}
        _EmissionIntensity("自发光强度", Range(0, 2)) = 0.3
        _BlinnPhongSP("高光强度", Range(0, 10)) = 0.3 
        _BP_Gloss("高光范围", Range(0, 1)) = 0.06 
        GlobeLight("全局光强度", Range(0, 1)) = 1
        _Gama("Gama",Range(-1, 1)) = 0
         [Toggle(_ENABLE_CUT)] _ENABLE_CUT("透明提除", Int) = 1
        _Cutoff("透明", Range(0, 1)) = 0
         _MatCap("材质捕获图", 2D)  = "black" {}
     	_MatCapFactor("材质捕获强度", Range(0,2)) = 0.45


     	 [Toggle(S_SUB_COLOR)] S_SUB_COLOR("开启局部变色", Int) = 1
 		_ColorMark("变色遮罩", 2D) = "white" {}
 		_SubColor("局部变色", Color) =  (0.25, 0.25, 0.25, 1)
 
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
		//Cull Off
		CGPROGRAM

		// compile directives
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma target 3.0
		#pragma exclude_renderers gles
		#pragma shader_feature _NORMALMAP
		#pragma shader_feature _ENABLE_CUT
		#pragma shader_feature _DISABLE_IBL_DIFFUSE
		#pragma multi_compile _SHADER_LEVEL_TOP _SHADER_LEVEL_MID _SHADER_LEVEL_LOW

		#pragma shader_feature S_SUB_COLOR
		#pragma shader_feature _ALPHATEST_ON
		#pragma shader_feature _EMISSION
		#pragma shader_feature _METALLICGLOSSMAP
		#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

		//#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
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
		void myvert (inout appdata_vert v, out float2 uv) 
	    {
		   uv = TRANSFORM_TEX(v.uv0, _MainTex); // Always source from uv0
		   {
				float4 normal = float4(v.normal, 0);
				float4 tangent = float4(v.tangent.xyz, 0);

				float4 pos = skin1(v.vertex, v.uv1, v.uv2);
				normal = skin1(normal, v.uv1, v.uv2);
				tangent = skin1(tangent, v.uv1, v.uv2);

				v.vertex = pos;
				v.normal = normal.xyz;
				v.tangent = float4(tangent.xyz, v.tangent.w);
		   }
		}

        #include "GPUPBR.cginc"


 
	    
 
		


		 
		  
		



		ENDCG
	}
 
        UsePass "Standard/SHADOWCASTER"
    }

    FallBack Off
    //CustomEditor "GPUSkinningStandardShaderGUI"
}