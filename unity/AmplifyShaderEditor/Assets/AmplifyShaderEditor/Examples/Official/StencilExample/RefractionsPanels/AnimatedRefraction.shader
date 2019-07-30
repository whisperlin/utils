// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/AnimatedRefraction"
{
	Properties
	{
		_PortalColor("PortalColor", Color) = (0.003838672,0.5220588,0.243292,0)
		[HideInInspector] __dirty( "", Int ) = 1
		_Distortion("Distortion", Range( 0 , 1)) = 0.292
		_BrushedMetalNormal("BrushedMetalNormal", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+100" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ "_ScreenGrab0" }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform sampler2D _ScreenGrab0;
		uniform sampler2D _BrushedMetalNormal;
		uniform float4 _BrushedMetalNormal_ST;
		uniform float _Distortion;
		uniform float4 _PortalColor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 screenPos39 = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
			float scale39 = -1.0;
			#else
			float scale39 = 1.0;
			#endif
			float halfPosW39 = screenPos39.w * 0.5;
			screenPos39.y = ( screenPos39.y - halfPosW39 ) * _ProjectionParams.x* scale39 + halfPosW39;
			screenPos39.w += 0.00000000001;
			screenPos39.xyzw /= screenPos39.w;
			float4 normalizedClip = screenPos39;
			float2 uv_BrushedMetalNormal = i.uv_texcoord * _BrushedMetalNormal_ST.xy + _BrushedMetalNormal_ST.zw;
			float cos33 = cos( _Time.y );
			float sin33 = sin( _Time.y );
			float2 rotator33 = mul(uv_BrushedMetalNormal - float2( 0.5,0.5 ), float2x2(cos33,-sin33,sin33,cos33)) + float2( 0.5,0.5 );
			o.Emission = ( tex2Dproj( _ScreenGrab0, UNITY_PROJ_COORD( ( normalizedClip + float4( ( UnpackNormal( tex2D( _BrushedMetalNormal,rotator33) ) * _Distortion ) , 0.0 ) ) ) ) * _PortalColor ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=4102
276;100;911;639;920.2786;293.8012;1.3;True;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-128.7738,261.0988;Float;FLOAT3;0.0,0,0;FLOAT;0.0,0,0
Node;AmplifyShaderEditor.SamplerNode;29;-542.58,195.399;Float;Property;_BrushedMetalNormal;BrushedMetalNormal;-1;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;31;-437.0751,361.6983;Float;Property;_Distortion;Distortion;-1;0.292;0;1
Node;AmplifyShaderEditor.SimpleAddOpNode;30;36.62508,137.2995;Float;FLOAT4;0.0,0,0,0;FLOAT3;0.0
Node;AmplifyShaderEditor.RotatorNode;33;-831.5794,259.398;Float;FLOAT2;0,0;FLOAT2;0.5,0.5;FLOAT;0.0
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-1142.884,213.6985;Float;0;29;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.Vector2Node;35;-1137.684,344.9991;Float;Constant;_Vector0;Vector 0;-1;0.5,0.5
Node;AmplifyShaderEditor.TimeNode;36;-1194.883,489.299;Float
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;704.4994,-13;Float;True;2;Float;ASEMaterialInspector;Standard;ASESampleShaders/AnimatedRefraction;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Transparent;0.5;True;True;100;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;FLOAT;0.0;OBJECT;0.0;FLOAT3;0,0,0;FLOAT3;0.0,0,0;OBJECT;0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;504.2176,117.4984;Float;COLOR;0.0,0,0,0;COLOR;0.0,0,0,0
Node;AmplifyShaderEditor.ColorNode;37;231.2177,270.9001;Float;Property;_PortalColor;PortalColor;-1;0.003838672,0.5220588,0.243292,0
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-226.9862,-12.90162;Float;normalizedClip;-1;False;FLOAT4;0.0,0,0,0
Node;AmplifyShaderEditor.ScreenColorNode;8;224.0004,85.8997;Float;Global;_ScreenGrab0;Screen Grab 0;-1;Object;-1;FLOAT4;0,0
Node;AmplifyShaderEditor.GrabScreenPosition;39;-475.6781,-63.70117;Float;True
WireConnection;32;0;29;0
WireConnection;32;1;31;0
WireConnection;29;1;33;0
WireConnection;30;0;21;0
WireConnection;30;1;32;0
WireConnection;33;0;34;0
WireConnection;33;1;35;0
WireConnection;33;2;36;2
WireConnection;0;2;38;0
WireConnection;38;0;8;0
WireConnection;38;1;37;0
WireConnection;21;0;39;0
WireConnection;8;0;30;0
ASEEND*/
//CHKSM=A33B0BBB4524D4AAE1AD6302C62CD9D695719B58