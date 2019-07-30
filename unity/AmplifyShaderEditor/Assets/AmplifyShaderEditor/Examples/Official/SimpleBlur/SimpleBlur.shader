// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SimpleBlurOFF"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MainSample("MainSample", 2D) = "white" {}
		_ToggleBlur("Toggle Blur", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 4.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_MainSample;
			float2 texcoord_0;
			float2 texcoord_1;
			float2 texcoord_2;
			float2 texcoord_3;
		};

		uniform sampler2D _MainSample;
		uniform float _ToggleBlur;

		void vertexDataFunc( inout appdata_full vertexData, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = vertexData.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_1.xy = vertexData.texcoord.xy * float2( 1,1 ) + float2( 0,0.01 );
			o.texcoord_2.xy = vertexData.texcoord.xy * float2( 1,1 ) + float2( 0.01,0 );
			o.texcoord_3.xy = vertexData.texcoord.xy * float2( 1,1 ) + float2( 0.01,0.01 );
		}

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			output.Emission = lerp( tex2D( _MainSample,input.uv_MainSample) , ( ( ( ( tex2D( _MainSample,input.texcoord_0) * 0.4 ) + ( tex2D( _MainSample,input.texcoord_1) * 0.2 ) ) + ( tex2D( _MainSample,input.texcoord_2) * 0.2 ) ) + ( tex2D( _MainSample,input.texcoord_3) * 0.2 ) ) , step( 0.5 , _ToggleBlur ) ).xyz;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;1798.802;1222.398;1.6;True;False
Node;AmplifyShaderEditor.RangedFloatNode;28;-618.9985,632.5013;Float;Constant;_Float3;Float 3;-1;0.2;0;0
Node;AmplifyShaderEditor.SamplerNode;5;-1185.401,-944.6984;Float;Property;_MainSample;MainSample;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-525.3992,219.1006;Float;FLOAT4;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-509.7991,483.0006;Float;FLOAT4;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-346.0005,-372.3999;Float;FLOAT4;0,0,0,0;FLOAT4;0,0,0,0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-110,104.6;Float;FLOAT4;0.0,0,0,0;FLOAT4;0,0,0,0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1153.901,-581.9991;Float;0;-1;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1168.701,-290.3001;Float;0;-1;FLOAT2;1,1;FLOAT2;0,0.01
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-1114.1,-2.479553E-05;Float;0;-1;FLOAT2;1,1;FLOAT2;0.01,0
Node;AmplifyShaderEditor.SamplerNode;8;-940.7994,448.3;Float;Property;_TextureSample2;Texture Sample 2;-1;None;True;0;False;white;Auto;False;Instance;5;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-1165.801,406.3;Float;0;-1;FLOAT2;1,1;FLOAT2;0.01,0.01
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-515.0012,-569.9993;Float;FLOAT4;0.0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-521.4998,-241.0996;Float;FLOAT4;0.0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-690.4989,-456.8986;Float;Constant;_Float0;Float 0;-1;0.4;0;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-657.999,-112.3987;Float;Constant;_Float1;Float 1;-1;0.2;0;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-682.6975,319.201;Float;Constant;_Float2;Float 2;-1;0.2;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-274.2003,-49.30003;Float;FLOAT4;0.0,0,0,0;FLOAT4;0,0,0,0
Node;AmplifyShaderEditor.SamplerNode;7;-898.8001,53.70006;Float;Property;_TextureSample1;Texture Sample 1;-1;None;True;0;False;white;Auto;False;Instance;5;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;6;-947.9997,-324.3;Float;Property;_TextureSample0;Texture Sample 0;-1;None;True;0;False;white;Auto;False;Instance;5;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.LerpOp;31;513.3992,-325.1979;Float;FLOAT4;0.0,0,0,0;FLOAT4;0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;748.0002,-409.6999;Float;True;4;Float;ASEMaterialInspector;Standard;ASESampleShaders/SimpleBlurOFF;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0,0,0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.StepOpNode;35;367.199,74.30202;Float;FLOAT;0.5;FLOAT;0.0
Node;AmplifyShaderEditor.SamplerNode;29;-904.9965,-660.8977;Float;Property;_TextureSample3;Texture Sample 3;-1;None;True;0;False;white;Auto;False;Instance;5;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;32;-13.60084,205.202;Float;Property;_ToggleBlur;Toggle Blur;1;0;0;1
WireConnection;23;0;7;0
WireConnection;23;1;27;0
WireConnection;24;0;8;0
WireConnection;24;1;28;0
WireConnection;13;0;21;0
WireConnection;13;1;22;0
WireConnection;15;0;14;0
WireConnection;15;1;24;0
WireConnection;8;1;12;0
WireConnection;21;0;29;0
WireConnection;21;1;25;0
WireConnection;22;0;6;0
WireConnection;22;1;26;0
WireConnection;14;0;13;0
WireConnection;14;1;23;0
WireConnection;7;1;11;0
WireConnection;6;1;10;0
WireConnection;31;0;5;0
WireConnection;31;1;15;0
WireConnection;31;2;35;0
WireConnection;0;2;31;0
WireConnection;35;1;32;0
WireConnection;29;1;9;0
ASEEND*/
//CHKSM=A55C99F6CA751BD731B9A90D217E2A575E93A14D
