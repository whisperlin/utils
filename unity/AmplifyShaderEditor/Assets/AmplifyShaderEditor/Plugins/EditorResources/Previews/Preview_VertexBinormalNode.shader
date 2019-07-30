Shader "Hidden/VertexBinormalNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 frag( v2f_img i ) : SV_Target
			{
				float2 p = 2 * i.uv - 1;
				float r = sqrt( dot(p,p) );
				r = saturate( r );
				//if ( r < 1 )
				//{
					float2 uvs;
					float f = ( 1 - sqrt( 1 - r ) ) / r;
					uvs.x = p.x;
					uvs.y = p.y;
					float3 vertexPos = float3( uvs, ( f - 1 ) * 2 );
					float3 normal = normalize(vertexPos);
					float3 worldNormal = UnityObjectToWorldNormal(normal);
					
					float3 tangent = normalize(float3( (1-f)*2, p.y*0.01, p.x ));
					float3 worldPos = mul(unity_ObjectToWorld, vertexPos).xyz;
					float3 worldTangent = UnityObjectToWorldDir(tangent);
					float tangentSign = -1;
					float3 worldBinormal = normalize( cross(worldNormal, worldTangent) * tangentSign);
					
					return float4(worldBinormal, 1);
				//}
				//else {
				//	return 0;
				//}
			}
			ENDCG
		}
	}
}
