Shader "Hidden/IntNode"
{
	Properties {
		_InputFloat ("_InputInt", Int) = 0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			int _InputFloat;

			float4 frag( v2f_img i ) : SV_Target
			{
				return _InputFloat;
			}
			ENDCG
		}
	}
}
