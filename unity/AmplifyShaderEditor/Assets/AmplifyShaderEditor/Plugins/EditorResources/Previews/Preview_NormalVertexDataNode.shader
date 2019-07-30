Shader "Hidden/NormalVertexDataNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : SV_Target
			{
				float2 p = 2 * i.uv - 1;
				float r = sqrt( dot(p,p) );
				r = saturate( r );
				//if ( r < 1 )
				//{
					float2 uvs;
					float f = ( 1 - sqrt( 1 - r ) ) / r;
					uvs.x = p.x * f;
					uvs.y = p.y * f;
					float3 normal = normalize(float3( uvs, (f-1))*2);

					return float4(normal, 1);
				//}
				//else {
				//	return 0;
				//}
			}
			ENDCG
		}
	}
}
