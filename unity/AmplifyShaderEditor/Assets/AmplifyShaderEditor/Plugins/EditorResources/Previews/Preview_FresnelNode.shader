Shader "Hidden/FresnelNode"
{
	Properties
	{
		_A ("_Normal", 2D) = "white" {}
		_B ("_Bias", 2D) = "white" {}
		_C ("_Scale", 2D) = "white" {}
		_D ("_Power", 2D) = "white" {}
	}
	SubShader
	{
		Pass //not connected
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 p = 2 * i.uv - 1;
				float r = sqrt( dot(p,p) );
				r = saturate( r );

				float2 uvs;
				float f = ( 1 - sqrt( 1 - r ) ) / r;
				uvs.x = p.x;
				uvs.y = p.y;
				float3 vertexPos = float3( uvs, ( f - 1 ) * 2 );
				float3 normal = normalize(vertexPos);
				float3 worldNormal = UnityObjectToWorldNormal(normal);
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				float fresnel = (b + s*pow(s - dot( worldNormal, worldViewDir ) , pw));
				return fresnel;
			}
			ENDCG
		}

		Pass //connected
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 p = 2 * i.uv - 1;
				float r = sqrt( dot(p,p) );
				r = saturate( r );

				float2 uvs;
				float f = ( 1 - sqrt( 1 - r ) ) / r;
				uvs.x = p.x;
				uvs.y = p.y;
				float3 vertexPos = float3( uvs, ( f - 1 ) * 2 );
				float3 worldNormal = tex2D( _A, i.uv );
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				float fresnel = (b + s*pow(s - dot( worldNormal, worldViewDir ) , pw));
				return fresnel;
			}
			ENDCG
		}
	}
}
