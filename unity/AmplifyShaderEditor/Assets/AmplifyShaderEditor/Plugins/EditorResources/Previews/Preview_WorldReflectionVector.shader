Shader "Hidden/WorldReflectionVector"
{
	Properties
	{
		_A ("_TangentNormal", 2D) = "white" {}
	}
	SubShader
	{
		Pass //not connected
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 frag(v2f_img i) : SV_Target
			{
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
				float3 worldRefl = -worldViewDir;
				worldRefl = reflect( worldRefl, worldNormal );
				
				return float4((worldRefl), 1);
			}
			ENDCG
		}

		Pass //connected
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _A;

			float4 frag(v2f_img i) : SV_Target
			{
				float radius = 1;
				float2 p = (2 * i.uv - 1) / radius;
				float r = sqrt( dot(p,p) );
				float3 vertexPos;
				float3 worldViewDir;
				float3 normal;
				float3 worldRefl;
				float2 sphereUVs = i.uv;
				float3 tangentNormal;
				r = saturate( r );

				float2 uvs;
				float f = ( 1 - sqrt( 1 - r ) ) / r;
				uvs.x = p.x;
				uvs.y = p.y;

				vertexPos = float3( uvs, (f-1)*2);
				worldViewDir = normalize(float3(0,0,-5) - vertexPos);
				normal = normalize(float3( uvs, (f-1)*2));

				float3 tangent = normalize(float3( 1-f, p.y*0.01, p.x ));
				float3 worldPos = mul(unity_ObjectToWorld, vertexPos).xyz;
				float3 worldNormal = UnityObjectToWorldNormal(normal);
				float3 worldTangent = UnityObjectToWorldDir(tangent);
				float tangentSign = -1;
				float3 worldBinormal = normalize( cross(worldNormal, worldTangent) * tangentSign);
				float4 tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				float4 tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				float4 tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				worldRefl = -worldViewDir;

				sphereUVs.x = atan2(vertexPos.x, -vertexPos.z) / (UNITY_PI) + 0.5;
				tangentNormal = tex2Dlod(_A, float4(sphereUVs,0,0)).xyz;

				worldRefl = reflect( worldRefl, half3( dot( tSpace0.xyz, tangentNormal ), dot( tSpace1.xyz, tangentNormal ), dot( tSpace2.xyz, tangentNormal ) ) );
				
				return float4((worldRefl), 1);
			}
			ENDCG
		}
	}
}
