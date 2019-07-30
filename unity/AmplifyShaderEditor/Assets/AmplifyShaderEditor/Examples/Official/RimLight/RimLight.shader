// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/RimLight"
{
	Properties
	{
		_Metallic("Metallic", 2D) = "white" {}
		_Occlusion("Occlusion", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		_RimColor("RimColor", Color) = (0,0,0,0)
		_Normals("Normals", 2D) = "bump" {}
		_Albedo("Albedo", 2D) = "white" {}
		_RimPower("RimPower", Range( 0 , 10)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_Normals;
			float2 uv_Albedo;
			float3 viewDir;
			float2 uv_Metallic;
			float2 uv_Occlusion;
		};

		uniform sampler2D _Normals;
		uniform sampler2D _Albedo;
		uniform fixed _RimPower;
		uniform fixed4 _RimColor;
		uniform sampler2D _Metallic;
		uniform sampler2D _Occlusion;

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			fixed3 tex2DNode3 = UnpackNormal( tex2D( _Normals,input.uv_Normals) );
			output.Normal = tex2DNode3;
			output.Albedo = tex2D( _Albedo,input.uv_Albedo).xyz;
			output.Emission = ( pow( ( 1.0 - saturate( dot( tex2DNode3 , normalize( input.viewDir ) ) ) ) , _RimPower ) * _RimColor ).rgb;
			output.Metallic = tex2D( _Metallic,input.uv_Metallic).x;
			output.Occlusion = tex2D( _Occlusion,input.uv_Occlusion).x;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;1795.216;183.8657;1.9;True;False
Node;AmplifyShaderEditor.SamplerNode;4;-208.9012,673.0024;Float;Property;_Occlusion;Occlusion;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.DotProductOpNode;21;-1137.603,334.4997;Float;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.SamplerNode;3;-1530.101,101.7998;Float;Property;_Normals;Normals;-1;None;True;0;True;bump;Auto;True;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.NormalizeNode;23;-1329.303,412.9002;Float;FLOAT3;0,0,0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;22;-1483.802,349.2998;Float;Tangent
Node;AmplifyShaderEditor.SamplerNode;1;-210.5004,83.20016;Float;Property;_Albedo;Albedo;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;2;-213.5002,481.3999;Float;Property;_Metallic;Metallic;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-358.2028,390.8992;Float;FLOAT;0;COLOR;0.0,0,0,0
Node;AmplifyShaderEditor.PowerNode;26;-600.6031,375.6995;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.ColorNode;25;-606.8026,555.4988;Float;Property;_RimColor;RimColor;-1;0,0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;28;-890.0026,467.6995;Float;Property;_RimPower;RimPower;-1;0;0;10
Node;AmplifyShaderEditor.SaturateNode;20;-962.4039,309.9996;Float;FLOAT;1.23
Node;AmplifyShaderEditor.OneMinusNode;5;-793.4009,352.2989;Float;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;147.2,339.6;Fixed;True;2;Fixed;;Standard;ASESampleShaders/RimLight;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0;FLOAT3;0;FLOAT;0;FLOAT;0;FLOAT;0;FLOAT3;0;FLOAT3;0,0,0;FLOAT;0;OBJECT;0,0,0;OBJECT;0,0,0;OBJECT;0,0,0;OBJECT;0;FLOAT3;0,0,0
WireConnection;21;0;3;0
WireConnection;21;1;23;0
WireConnection;23;0;22;0
WireConnection;27;0;26;0
WireConnection;27;1;25;0
WireConnection;26;0;5;0
WireConnection;26;1;28;0
WireConnection;20;0;21;0
WireConnection;5;0;20;0
WireConnection;0;0;1;0
WireConnection;0;1;3;0
WireConnection;0;2;27;0
WireConnection;0;3;2;0
WireConnection;0;5;4;0
ASEEND*/
//CHKSM=F234B289815B54C92B4DF03557A4A551FC395BA1
