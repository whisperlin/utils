// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/ScreenSpaceDetail"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Checkers("Checkers", 2D) = "white" {}
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
			float2 uv_Albedo;
			float4 screenPos;
		};

		uniform sampler2D _Albedo;
		uniform sampler2D _Checkers;

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			float2 FLOATToFLOAT2110=max( input.screenPos.w , 0.01 );
			output.Albedo = ( tex2D( _Albedo,input.uv_Albedo) * tex2D( _Checkers,( ( input.screenPos.xy / FLOATToFLOAT2110 ) * float2( 8,6 ) )) ).xyz;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=20
237;92;1404;657;1030.102;260.201;1;True;False
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;True;Standard;ASESampleShaders/ScreenSpaceDetail;False;False;False;False;False;False;False;False;False;False;False;False;Back;On;LEqual;Opaque;0.5;True;True;0;False;Opaque;Geometry;0,0,0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0,0,0;0,0,0;0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-168.5,-76;0.0,0,0,0;0.0,0,0,0
Node;AmplifyShaderEditor.Vector2Node;10;-968.3,187;Constant;_Vector0;Vector 0;-1;8,6
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-678.3,53.99998;0.0,0;0.0,0
Node;AmplifyShaderEditor.SamplerNode;1;-549.5,-257;Property;_Albedo;Albedo;-1;None;True;0;True;white;Auto;False;Object;-1;0,0;1.0
Node;AmplifyShaderEditor.SamplerNode;2;-468.9,52.8;Property;_Checkers;Checkers;-1;None;True;0;False;white;Auto;False;Object;-1;0,0;1.0
Node;AmplifyShaderEditor.ScreenPosInputsNode;4;-1305.3,-147
Node;AmplifyShaderEditor.ComponentMaskNode;6;-1011.3,-113;True;True;False;False;0,0,0,0
Node;AmplifyShaderEditor.SimpleDivideOpNode;11;-895.6021,-6.200958;0.0,0;0.0
Node;AmplifyShaderEditor.SimpleMaxOp;13;-1105.102,17.79907;0.0;0.01
WireConnection;0;0;8;0
WireConnection;8;0;1;0
WireConnection;8;1;2;0
WireConnection;9;0;11;0
WireConnection;9;1;10;0
WireConnection;2;0;9;0
WireConnection;6;0;4;0
WireConnection;11;0;6;0
WireConnection;11;1;13;0
WireConnection;13;0;4;4
ASEEND*/
//CHKSM=DCFC7B2D9FAD3E85EC66AEF9FDFCBB10F2484768
