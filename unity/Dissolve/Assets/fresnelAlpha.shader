Shader "Shader Forge/fresnel Alpha"
{
	Properties
	{
		_fresnel_1("fresnel_1", Range(0, 1)) = 0.4686216
		_fresnel("fresnel", Float) = 20
		_InnerColor("node_11", Color) = (0.5,0.5,0.5,1)
		_mask("mask", 2D) = "white" {}
		_alpha("alpha", Range(0, 8)) = 1.372233
		_edge("边缘宽度", Range(0.01,0.35)) = 0.35
		_alphaEdge("半透宽度", Range(0.01,0.35)) = 0.35
		_edgeColor("消融边缘", Color) = (1,1,1,1)
		_light("light", Float) = 5
		[HideInInspector]_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader
	{
		Tags
	{
		"Queue" = "Transparent+200"
		"RenderType" = "Transparent"
	}
		Pass
	{
		Name "ForwardBase"
		Tags
	{
		"LightMode" = "ForwardBase"
	}
		Blend One One


		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		// #define UNITY_PASS_FORWARDBASE							// Our proj is force forward base.
#include "UnityCG.cginc"
		// #pragma multi_compile_fwdbase_fullshadows				// Shadow is not allowed.
		// #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 	// Should compile for all the platforms
		// #pragma target 3.0										// Target 3.0 is not fully supported by OpenGL ES 2.0

		uniform float4 _InnerColor;
	uniform float4 _mask_ST;
	uniform float4 _edgeColor;
	uniform float _fresnel_1;
	uniform float _fresnel;
	uniform float _alpha;
	uniform float _edge;
	uniform float _light;

	uniform sampler2D _mask;

	struct VertexInput
	{
		fixed4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 color :COLOR;
		float2 texcoord0 : TEXCOORD0;

	};

	struct VertexOutput
	{
		fixed4 pos : SV_POSITION;
		float4 posWorld : TEXCOORD1;
		float3 normalDir : TEXCOORD2;
		float2 uv0 : TEXCOORD0;
		float4 color : TEXCOORD3;
	};

	VertexOutput vert(VertexInput v)
	{
		VertexOutput o;
		o.uv0 = v.texcoord0;
		o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.color = v.color;
		return o;
	}

	fixed4 frag(VertexOutput i) : COLOR
	{
		i.normalDir = normalize(i.normalDir);
		float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
		/////// Normals:
		float3 normalDirection = i.normalDir;
 

		float markVar = (tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask)).rgb*_alpha).r;
		float node_476_if_leB = step(0.3,markVar);
 
		clip(node_476_if_leB - 0.5);
		////// Lighting:
		////// Emissive:
		float node_471_if_leA = step(markVar,_edge);
		float node_471_if_leB = step(_edge,markVar);
		float3 emissive = ((pow((_fresnel_1 + (1.0 - max(0,dot(normalDirection, viewDirection)))),_fresnel)*_InnerColor.rgb) + ((_edgeColor.rgb*(1.0*(node_476_if_leB - lerp((node_471_if_leA*0.0) + (node_471_if_leB*1.0),1.0,node_471_if_leA*node_471_if_leB))))*_light));

		float3 finalColor = emissive;
		finalColor *= i.color;
		/// Final Color:
		return fixed4(finalColor,0);
	}

		ENDCG
	}
	}
		FallBack "Diffuse"
		// CustomEditor "ShaderForgeMaterialInspector" // Can not be opened by this editor.
}
