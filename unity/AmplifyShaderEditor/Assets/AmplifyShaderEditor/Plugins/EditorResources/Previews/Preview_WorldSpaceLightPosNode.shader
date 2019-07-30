Shader "Hidden/WorldSpaceLightPosNode"
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
				return _WorldSpaceLightPos0;
			}
			ENDCG
		}
	}
}
