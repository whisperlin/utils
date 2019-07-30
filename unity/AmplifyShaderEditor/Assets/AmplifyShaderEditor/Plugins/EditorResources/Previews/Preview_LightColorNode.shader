Shader "Hidden/LightColorNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			float4 frag(v2f_img i) : SV_Target
			{
				return _LightColor0;
			}
			ENDCG
		}
	}
}
