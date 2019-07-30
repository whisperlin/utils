// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/WorldPosSlices"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MaskClipValue( "Mask Clip Value", Float ) = 0
		_Albedo("Albedo", 2D) = "white" {}
		_Thickness("Thickness", Float) = 0.5
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_Albedo;
			float3 worldPos;
			INTERNAL_DATA
		};

		uniform sampler2D _Albedo;
		uniform float _Thickness;
		uniform float _MaskClipValue = 0;

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			output.Albedo = tex2D( _Albedo,input.uv_Albedo).xyz;
			output.Alpha = 1;
			clip( ( frac( ( ( input.worldPos.y + ( input.worldPos.z * 0.1 ) ) * 5.0 ) ) - _Thickness ) - _MaskClipValue );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;568.1984;315.9;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;3;-574.5,275;Float;Constant;_Float0;Float 0;-1;0.1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-400.5,172;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleAddOpNode;4;-254.5,75;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;8;-486.5,365;Float;Constant;_Float1;Float 1;-1;5;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-251.5,278;Float;FLOAT;0.0;FLOAT;0.5
Node;AmplifyShaderEditor.FractNode;6;-83.5,140;Float;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;9;10.5,328;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.WorldPosInputsNode;2;-585.5,10;Float
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;275,-127;Float;True;2;Float;ASEMaterialInspector;Standard;ASESampleShaders/WorldPosSlices;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Masked;0;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;OBJECT;0.0;OBJECT;0,0,0;OBJECT;0,0,0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;10;-255.5,430;Float;Property;_Thickness;Thickness;-1;0.5;0;0
Node;AmplifyShaderEditor.SamplerNode;1;-54.5,-160;Float;Property;_Albedo;Albedo;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
WireConnection;5;0;2;3
WireConnection;5;1;3;0
WireConnection;4;0;2;2
WireConnection;4;1;5;0
WireConnection;7;0;4;0
WireConnection;7;1;8;0
WireConnection;6;0;7;0
WireConnection;9;0;6;0
WireConnection;9;1;10;0
WireConnection;0;0;1;0
WireConnection;0;9;9;0
ASEEND*/
//CHKSM=A48CFEC1DD01A1786C496B29EF86DEEB39E585FC
