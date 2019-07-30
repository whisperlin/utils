Shader "Hidden/WorldPosInputsNode"
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
				float2 uvs;
				float f = ( 1 - sqrt( 1 - r ) ) / r;
				uvs.x = p.x;
				uvs.y = p.y;
				float3 vertexPos = float3( uvs, (f-1)*2);
				float3 worldPos = mul(unity_ObjectToWorld, vertexPos).xyz;
				return float4 (worldPos, 1);
			}
			ENDCG
		}
	}
}
