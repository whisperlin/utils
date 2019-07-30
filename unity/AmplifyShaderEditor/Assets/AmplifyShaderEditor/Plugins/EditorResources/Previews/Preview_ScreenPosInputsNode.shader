Shader "Hidden/ScreenPosInputsNode"
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
				/*if ( r < 1 )
				{*/
					float2 uvs;
					float f = ( 1 - sqrt( 1 - r ) ) / r;
					uvs.x = p.x;
					uvs.y = p.y;
					float3 worldPos = float3( uvs, (f-1)*2);
					float4 change = float4(worldPos, 0);
					return float4 (ComputeScreenPos(change));
				/*}
				else {
					return 0;
				}*/
			}
			ENDCG
		}
	}
}
