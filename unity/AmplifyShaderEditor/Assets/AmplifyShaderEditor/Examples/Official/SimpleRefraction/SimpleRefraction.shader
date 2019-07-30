// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SimpleRefraction"
{
	Properties
	{
		_Distortion("Distortion", Range( 0 , 1)) = 0.292
		[HideInInspector] __dirty( "", Int ) = 1
		_BrushedMetalNormal("BrushedMetalNormal", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ "_ScreenGrab0" }
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform sampler2D _ScreenGrab0;
		uniform sampler2D _BrushedMetalNormal;
		uniform float4 _BrushedMetalNormal_ST;
		uniform float _Distortion;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 screenPos4 = i.screenPos;
			screenPos4.w += 0.00000000001;
			screenPos4.xyzw /= screenPos4.w;
			float2 uv_BrushedMetalNormal = i.uv_texcoord * _BrushedMetalNormal_ST.xy + _BrushedMetalNormal_ST.zw;
			o.Emission = tex2D( _ScreenGrab0, ( screenPos4.xy + ( UnpackNormal( tex2D( _BrushedMetalNormal,uv_BrushedMetalNormal) ) * _Distortion ).xy ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=4102
344;100;1180;639;1010.48;398.6009;1.6;True;False
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;536.7999,-33.8;Float;True;2;Float;ASEMaterialInspector;Standard;ASESampleShaders/SimpleRefraction;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Translucent;0.5;True;True;0;False;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;OBJECT;0.0;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0;OBJECT;0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;36.62508,137.2995;Float;FLOAT2;0.0,0;FLOAT2;0.0,0,0,0
Node;AmplifyShaderEditor.ScreenColorNode;8;224.0004,85.8997;Float;Global;_ScreenGrab0;Screen Grab 0;-1;Object;-1;FLOAT2;0,0
Node;AmplifyShaderEditor.SamplerNode;29;-855.48,221.599;Float;Property;_BrushedMetalNormal;BrushedMetalNormal;-1;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;31;-749.975,387.8984;Float;Property;_Distortion;Distortion;-1;0.292;0;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-441.6739,287.2988;Float;FLOAT3;0.0,0,0;FLOAT;0.0,0,0
Node;AmplifyShaderEditor.ComponentMaskNode;36;-248.5805,285.0987;Float;True;True;False;True;FLOAT3;0,0,0
Node;AmplifyShaderEditor.ComponentMaskNode;39;-191.7806,65.19897;Float;True;True;False;False;FLOAT4;0,0,0,0
Node;AmplifyShaderEditor.ScreenPosInputsNode;4;-381.0954,24.99997;Float;True;False
WireConnection;0;2;8;0
WireConnection;30;0;39;0
WireConnection;30;1;36;0
WireConnection;8;0;30;0
WireConnection;32;0;29;0
WireConnection;32;1;31;0
WireConnection;36;0;32;0
WireConnection;39;0;4;0
ASEEND*/
//CHKSM=96B015F0AC732944B0AC05786421E1FB899B2F35