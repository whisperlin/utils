// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SubstanceExample"
{
	Properties
	{
		_TessPhongStrength( "Phong Strength", Range( 0, 1 ) ) = 0.5
		_TessValue( "Tessellation", Range( 1, 32 ) ) = 30
		Snow_Bricks_height("Snow_Bricks_height", 2D) = "white"{}
		[HideInInspector] __dirty( "", Int ) = 1
		_MaskClipValue( "Mask Clip Value", Float ) = 0.5
		Snow_Bricks_ambientOcclusion("Snow_Bricks_ambientOcclusion", 2D) = "white"{}
		Snow_Bricks_basecolor("Snow_Bricks_basecolor", 2D) = "white"{}
		Snow_Bricks_normal("Snow_Bricks_normal", 2D) = "white"{}
		Snow_Bricks_metallic("Snow_Bricks_metallic", 2D) = "white"{}
		Snow_Bricks_roughness("Snow_Bricks_roughness", 2D) = "white"{}
		_HeightStrength("Height Strength", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" }
		Cull Back
		CGPROGRAM
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction nolightmap tessphong:_TessPhongStrength 
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

		uniform sampler2D Snow_Bricks_normal;
		uniform float4 Snow_Bricks_basecolor_ST;
		uniform sampler2D Snow_Bricks_basecolor;
		uniform sampler2D Snow_Bricks_metallic;
		uniform sampler2D Snow_Bricks_roughness;
		uniform sampler2D Snow_Bricks_ambientOcclusion;
		uniform sampler2D Snow_Bricks_height;
		uniform float _HeightStrength;
		uniform float _MaskClipValue = 0.5;
		uniform float _TessPhongStrength;
		uniform float _TessValue;

		float4 tessFunction( )
		{
			return _TessValue;
		}

		void vertexDataFunc( inout appdata v )
		{
			float4 uvSnow_Bricks_basecolor = float4(v.texcoord * Snow_Bricks_basecolor_ST.xy + Snow_Bricks_basecolor_ST.zw, 0 ,0);
			float4 Snow_Bricks_height56 = tex2Dlod( Snow_Bricks_height, uvSnow_Bricks_basecolor);
			v.vertex.xyz += ( ( Snow_Bricks_height56 * _HeightStrength ) * float4( v.normal , 0.0 ) ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uvSnow_Bricks_basecolor = i.uv_texcoord * Snow_Bricks_basecolor_ST.xy + Snow_Bricks_basecolor_ST.zw;
			float3 Snow_Bricks_normal56 = UnpackNormal( tex2D( Snow_Bricks_normal, uvSnow_Bricks_basecolor) );
			o.Normal = Snow_Bricks_normal56;
			float4 Snow_Bricks_basecolor56 = tex2D( Snow_Bricks_basecolor, uvSnow_Bricks_basecolor);
			o.Albedo = Snow_Bricks_basecolor56.rgb;
			float4 Snow_Bricks_metallic56 = tex2D( Snow_Bricks_metallic, uvSnow_Bricks_basecolor);
			o.Metallic = Snow_Bricks_metallic56.r;
			float4 Snow_Bricks_roughness56 = tex2D( Snow_Bricks_roughness, uvSnow_Bricks_basecolor);
			o.Smoothness = ( 1.0 - Snow_Bricks_roughness56 ).r;
			float4 Snow_Bricks_ambientOcclusion56 = tex2D( Snow_Bricks_ambientOcclusion, uvSnow_Bricks_basecolor);
			o.Occlusion = Snow_Bricks_ambientOcclusion56.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5001
283;92;1007;650;517.222;47.25665;1;False;False
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;328.5,60.69992;Float;True;6;Float;ASEMaterialInspector;Standard;ASESampleShaders/SubstanceExample;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Custom;0.5;True;True;0;True;TransparentCutout;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;True;1;30;0;10;True;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;OBJECT;0.0;FLOAT3;0.0;FLOAT3;0.0,0,0;OBJECT;0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.OneMinusNode;50;100.78,171.6431;Float;COLOR;0.0
Node;AmplifyShaderEditor.RangedFloatNode;51;-355.22,353.6431;Float;Property;_HeightStrength;Height Strength;2;0;0;0;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;149.78,378.6431;Float;COLOR;0,0,0;FLOAT3;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-15.22,276.6431;Float;COLOR;0.0;FLOAT;0.0,0,0,0
Node;AmplifyShaderEditor.NormalVertexDataNode;53;-39.21997,423.6431;Float
Node;AmplifyShaderEditor.SubstanceSamplerNode;56;-409.222,42.74335;Float;Property;_SnowBricksSubstance;Snow Bricks Substance;1;0;7af3ece29374c234f9406a3bb35df76c;0;True;FLOAT2;0,0
WireConnection;0;0;56;0
WireConnection;0;1;56;1
WireConnection;0;3;56;3
WireConnection;0;4;50;0
WireConnection;0;5;56;4
WireConnection;0;11;54;0
WireConnection;50;0;56;2
WireConnection;54;0;52;0
WireConnection;54;1;53;0
WireConnection;52;0;56;5
WireConnection;52;1;51;0
ASEEND*/
//CHKSM=4818B0AFC9B5275D8ECE1395E3BCC25FC3EEFDDE