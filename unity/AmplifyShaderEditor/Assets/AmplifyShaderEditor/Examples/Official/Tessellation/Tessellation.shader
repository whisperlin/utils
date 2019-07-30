// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Tessellation"
{
	Properties
	{
		_TessValue( "Tessellation", Range( 1, 32 ) ) = 10
		_TessMin( "Tess Min Distance", Float ) = 1
		_TessMax( "Tess Max Distance", Float ) = 2
		[HideInInspector] __dirty( "", Int ) = 1
		_DisplacementTex("DisplacementTex", 2D) = "white" {}
		_Albedo("Albedo", 2D) = "white" {}
		_Displacement("Displacement", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction nolightmap 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _DisplacementTex;
		uniform float4 _DisplacementTex_ST;
		uniform float _Displacement;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata v )
		{
			float4 uv_DisplacementTex = float4(v.texcoord * _DisplacementTex_ST.xy + _DisplacementTex_ST.zw, 0 ,0);
			v.vertex.xyz += ( ( tex2Dlod( _DisplacementTex,uv_DisplacementTex) * float4( v.normal , 0.0 ) ) * _Displacement ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = tex2D( _Albedo,uv_Albedo).xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=4102
344;100;1180;639;835;92.5;1;True;False
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;True;6;Float;ASEMaterialInspector;Standard;ASESampleShaders/Tessellation;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;True;0;10;1;2;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;OBJECT;0.0;FLOAT3;0.0;FLOAT3;0.0,0,0;OBJECT;0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SamplerNode;1;-772,172.5;Float;Property;_DisplacementTex;DisplacementTex;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/Sand/Sand_height.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.NormalVertexDataNode;3;-755,403.5;Float
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-426,290.5;Float;FLOAT4;0.0,0,0,0;FLOAT3;0.0,0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-221,310.5;Float;FLOAT4;0.0,0,0,0;FLOAT;0.0,0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;4;-522,483.5;Float;Property;_Displacement;Displacement;2;0;0;1
Node;AmplifyShaderEditor.SamplerNode;2;-476,-29.5;Float;Property;_Albedo;Albedo;1;Assets/AmplifyShaderEditor/Examples/Assets/Textures/Sand/Sand_basecolor.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
WireConnection;0;0;2;0
WireConnection;0;11;6;0
WireConnection;5;0;1;0
WireConnection;5;1;3;0
WireConnection;6;0;5;0
WireConnection;6;1;4;0
ASEEND*/
//CHKSM=7C7045FFC33E656A7297BEF0CF2B1B4DB43AF5F4