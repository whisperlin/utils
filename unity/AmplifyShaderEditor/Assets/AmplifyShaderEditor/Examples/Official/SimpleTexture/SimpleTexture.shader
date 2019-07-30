// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SimpleTexture"
{
	Properties
	{
		_char_woodman_occlusion("char_woodman_occlusion", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		_char_woodman_metallic("char_woodman_metallic", 2D) = "white" {}
		_Normals("Normals", 2D) = "bump" {}
		_Albedo("Albedo", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_Normals;
			float2 uv_Albedo;
			float2 uv_char_woodman_metallic;
			float2 uv_char_woodman_occlusion;
		};

		uniform sampler2D _Normals;
		uniform sampler2D _Albedo;
		uniform sampler2D _char_woodman_metallic;
		uniform sampler2D _char_woodman_occlusion;

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			output.Normal = UnpackNormal( tex2D( _Normals,input.uv_Normals) );
			output.Albedo = tex2D( _Albedo,input.uv_Albedo).xyz;
			output.Metallic = tex2D( _char_woodman_metallic,input.uv_char_woodman_metallic).x;
			output.Occlusion = tex2D( _char_woodman_occlusion,input.uv_char_woodman_occlusion).x;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=20
237;92;1404;657;1129.298;496.2007;1.3;True;True
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;19,-73;True;Standard;ASESampleShaders/SimpleTexture;False;False;False;False;False;False;False;False;False;False;False;False;Back;On;LEqual;Opaque;0.5;True;True;0;False;Opaque;Geometry;0,0,0;0,0,0;0;0;0;0;0;0;0;0;0,0,0;0,0,0
Node;AmplifyShaderEditor.SamplerNode;3;-531.1996,616.6;Property;_char_woodman_occlusion;char_woodman_occlusion;-1;None;True;0;False;white;Auto;False;Object;-1;0,0;1
Node;AmplifyShaderEditor.SamplerNode;2;-575.8001,430.5;Property;_char_woodman_metallic;char_woodman_metallic;-1;None;True;0;False;white;Auto;False;Object;-1;0,0;1
Node;AmplifyShaderEditor.SamplerNode;1;-615.4003,-302.1997;Property;_Albedo;Albedo;-1;None;True;0;False;white;Auto;False;Object;-1;0,0;1
Node;AmplifyShaderEditor.SamplerNode;5;-552.6003,50.49994;Property;_Normals;Normals;-1;None;True;0;True;bump;Auto;True;Object;-1;0,0;1.0
WireConnection;0;0;1;0
WireConnection;0;1;5;0
WireConnection;0;3;2;0
WireConnection;0;5;3;0
ASEEND*/
//CHKSM=A7187B92B616A7565AE16D4D7593B149BEF12771
