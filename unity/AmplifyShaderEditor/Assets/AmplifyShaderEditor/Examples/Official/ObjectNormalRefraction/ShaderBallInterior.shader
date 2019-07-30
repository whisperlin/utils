// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/ShaderBallInterior"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_RubberDiffuse("RubberDiffuse", 2D) = "white" {}
		_RubberSpecular("RubberSpecular", 2D) = "white" {}
		_RubberNormal("RubberNormal", 2D) = "bump" {}
		_Smoothness("Smoothness", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _RubberNormal;
		uniform float4 _RubberNormal_ST;
		uniform sampler2D _RubberDiffuse;
		uniform float4 _RubberDiffuse_ST;
		uniform sampler2D _RubberSpecular;
		uniform float4 _RubberSpecular_ST;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_RubberNormal = i.uv_texcoord * _RubberNormal_ST.xy + _RubberNormal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _RubberNormal,uv_RubberNormal) );
			float2 uv_RubberDiffuse = i.uv_texcoord * _RubberDiffuse_ST.xy + _RubberDiffuse_ST.zw;
			o.Albedo = tex2D( _RubberDiffuse,uv_RubberDiffuse).xyz;
			float temp_output_6_0 = 0.0;
			float3 temp_cast_1 = temp_output_6_0;
			o.Emission = temp_cast_1;
			float2 uv_RubberSpecular = i.uv_texcoord * _RubberSpecular_ST.xy + _RubberSpecular_ST.zw;
			o.Specular = tex2D( _RubberSpecular,uv_RubberSpecular).xyz;
			o.Smoothness = _Smoothness;
			o.Occlusion = temp_output_6_0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=4001
344;100;1180;639;867.7996;24.79974;1;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-622,-216.5;Float;Property;_RubberDiffuse;RubberDiffuse;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/SceneTextures/RubberDiffuse.tif;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;48,0;Float;True;2;Float;ASEMaterialInspector;StandardSpecular;ASESampleShaders/ShaderBallInterior;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;OBJECT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;OBJECT;0.0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SamplerNode;3;-624,-32;Float;Property;_RubberNormal;RubberNormal;2;Assets/AmplifyShaderEditor/Examples/Assets/Textures/SceneTextures/RubberNormal.tif;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;2;-576,128;Float;Property;_RubberSpecular;RubberSpecular;1;Assets/AmplifyShaderEditor/Examples/Assets/Textures/SceneTextures/RubberSpecular.tif;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;6;-181.7996,36.20026;Float;Constant;_Float0;Float 0;4;0;0;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-313,388;Float;Property;_Smoothness;Smoothness;3;0;0;0
WireConnection;0;0;1;0
WireConnection;0;1;3;0
WireConnection;0;2;6;0
WireConnection;0;3;2;0
WireConnection;0;4;4;0
WireConnection;0;5;6;0
ASEEND*/
//CHKSM=A00237F251EEEAC448ECC158A2C36CECA7353197
