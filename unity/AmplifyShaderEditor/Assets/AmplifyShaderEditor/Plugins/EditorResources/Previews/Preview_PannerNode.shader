Shader "Hidden/PannerNode"
{
	Properties
	{
		_A ("_UVs", 2D) = "white" {}
		_B ("_PanTime", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			Name "Panner" // 14 - UV panner node
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _A;
			sampler2D _B;
			float _UsingEditor;
			float _SpeedY;
			float _SpeedX;
			float _EditorTime;

			float4 frag(v2f_img i) : SV_Target
			{
				float time = _EditorTime;
				if ( _UsingEditor == 0 ) 
				{
					time = tex2D( _B, i.uv ).r;
				}

				return tex2D( _A, i.uv) + time * float4( _SpeedX, _SpeedY, 0, 0 );
			}
			ENDCG
		}
	}
}
