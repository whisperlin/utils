// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/DitheringFade"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MaskClipValue( "Mask Clip Value", Float ) = 0.74
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Specular("Specular", 2D) = "white" {}
		_Occlusion("Occlusion", 2D) = "white" {}
		_StartDitheringFade("Start Dithering Fade", Float) = 0
		_EndDitheringFade("End Dithering Fade", Float) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_Normal;
			float eyeDepth;
			float4 screenPosition;
		};

		uniform sampler2D _Normal;
		uniform sampler2D _Albedo;
		uniform sampler2D _Specular;
		uniform sampler2D _Occlusion;
		uniform fixed _StartDitheringFade;
		uniform fixed _EndDitheringFade;
		uniform float _MaskClipValue = 0.74;


		inline float Dither8x8Bayer( int x, int y )
		{
			const float dither[ 64 ] = {
				 1, 49, 13, 61,  4, 52, 16, 64,
				33, 17, 45, 29, 36, 20, 48, 32,
				 9, 57,  5, 53, 12, 60,  8, 56,
				41, 25, 37, 21, 44, 28, 40, 24,
				 3, 51, 15, 63,  2, 50, 14, 62,
				35, 19, 47, 31, 34, 18, 46, 30,
				11, 59,  7, 55, 10, 58,  6, 54,
				43, 27, 39, 23, 42, 26, 38, 22};
			int r = y * 8 + x;
			return (dither[r]-1) / 63;
		}


		void vertexDataFunc( inout appdata_full vertexData, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( vertexData.vertex.xyz ).z;
			o.screenPosition = ComputeScreenPos( UnityObjectToClipPos( vertexData.vertex ) );
		}

		void surf( Input input , inout SurfaceOutputStandardSpecular output )
		{
			output.Normal = UnpackNormal( tex2D( _Normal,input.uv_Normal) );
			output.Albedo = tex2D( _Albedo,input.uv_Normal).xyz;
			output.Specular = tex2D( _Specular,input.uv_Normal).xyz;
			output.Occlusion = tex2D( _Occlusion,input.uv_Normal).r;
			output.Alpha = 1;
			float temp_output_65_0 = ( _StartDitheringFade + _ProjectionParams.y );
			float2 clipScreen26 = ( input.screenPosition.xy / input.screenPosition.w ) * _ScreenParams.xy;
			clip( ( ( ( input.eyeDepth + -temp_output_65_0 ) / ( _EndDitheringFade - temp_output_65_0 ) ) - Dither8x8Bayer( fmod(clipScreen26.x, 8), fmod(clipScreen26.y, 8) ) ) - _MaskClipValue );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
392;92;1247;695;1700.839;777.1953;2.560208;True;False
Node;AmplifyShaderEditor.CommentaryNode;37;-609.7847,546.5101;Float;297.1897;243;Correction for near plane clipping;1;19
Node;AmplifyShaderEditor.CommentaryNode;36;-622.9761,91.39496;Float;1047.541;403.52;Scale depth from start to end;8;30;65;28;29;34;31;33;15
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;999.2003,-69.70003;Fixed;True;2;Fixed;ASEMaterialInspector;StandardSpecular;ASESampleShaders/DitheringFade;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Masked;0.74;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0.0,0,0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SamplerNode;46;401.5834,-312.5764;Float;Property;_Specular;Specular;3;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.WireNode;53;186.4047,-441.2574;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.WireNode;58;226.0047,-296.0571;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.WireNode;56;208.4047,-201.4573;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.ProjectionParams;19;-537.0848,595.81;Float
Node;AmplifyShaderEditor.WireNode;49;847.9045,48.44281;Float;FLOAT;0.0
Node;AmplifyShaderEditor.NegateNode;33;-107.1209,253.7414;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;51;857.5114,-90.67801;Float;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.WireNode;52;867.2929,-172.9061;Float;FLOAT4;0.0,0,0,0
Node;AmplifyShaderEditor.SamplerNode;45;333.2996,-474.4463;Float;Property;_Normal;Normal;2;None;True;0;True;bump;Auto;True;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;44;395.83,-659.0349;Float;Property;_Albedo;Albedo;1;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;48;401.1721,-125.1062;Float;Property;_Occlusion;Occlusion;4;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.WireNode;57;198.886,-393.4922;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.RangedFloatNode;31;-117.5356,335.0947;Float;Property;_EndDitheringFade;End Dithering Fade;6;1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;34;285.764,250.6948;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;29;99.26421,348.9946;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;101.9639,194.1952;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;30;-584.0002,374.09;Float;Property;_StartDitheringFade;Start Dithering Fade;5;0;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;-298.5083,377.6221;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;66;527.7935,196.322;Float;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;27;739.2949,117.1506;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-62.14664,-384.9758;Float;0;45;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.WireNode;67;852.93,-28.48083;Float;FLOAT4;0.0,0,0,0
Node;AmplifyShaderEditor.SurfaceDepthNode;15;-557.4172,189.5072;Float;0
Node;AmplifyShaderEditor.DitheringNode;26;577.071,244.1701;Float;1
WireConnection;0;0;52;0
WireConnection;0;1;51;0
WireConnection;0;3;67;0
WireConnection;0;5;49;0
WireConnection;0;9;27;0
WireConnection;46;1;58;0
WireConnection;53;0;47;0
WireConnection;58;0;47;0
WireConnection;56;0;47;0
WireConnection;49;0;48;1
WireConnection;33;0;65;0
WireConnection;51;0;45;0
WireConnection;52;0;44;0
WireConnection;45;1;57;0
WireConnection;44;1;53;0
WireConnection;48;1;56;0
WireConnection;57;0;47;0
WireConnection;34;0;28;0
WireConnection;34;1;29;0
WireConnection;29;0;31;0
WireConnection;29;1;65;0
WireConnection;28;0;15;0
WireConnection;28;1;33;0
WireConnection;65;0;30;0
WireConnection;65;1;19;2
WireConnection;66;0;34;0
WireConnection;27;0;66;0
WireConnection;27;1;26;0
WireConnection;67;0;46;0
ASEEND*/
//CHKSM=923BBA8F74EBBD7573B137ABB47349C4971B9CAD
