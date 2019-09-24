Shader "BB2"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Albedo("Albedo", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_Albedo;
		};

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			//Calculate new billboard vertex position and normal;
			float3 upCamVec = normalize ( UNITY_MATRIX_V._m10_m11_m12 );
			float3 forwardCamVec = -normalize ( UNITY_MATRIX_V._m20_m21_m22 );
			float3 rightCamVec = normalize( UNITY_MATRIX_V._m00_m01_m02 );
			float4x4 rotationCamMatrix = float4x4( rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1 );
			v.normal = normalize( mul( v.normal, rotationCamMatrix ));
			//This unfortunately must be made to take non-uniform scaling into account;
			//Transform to world coords, apply rotation and transform back to local;
			v.vertex = mul( v.vertex , unity_ObjectToWorld );
			v.vertex = mul( v.vertex , rotationCamMatrix );
			v.vertex = mul( v.vertex , unity_WorldToObject );
		}
		uniform sampler2D _Albedo; 
		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _Albedo_var = tex2D(_Albedo, i.uv_Albedo);
 
			o.Albedo = _Albedo_var;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}