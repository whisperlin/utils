Shader "Hidden/TexturePropertyNode"
{
	Properties
	{
		_Sampler ("_Sampler", 2D) = "white" {}
		_Array ("_Array", 2DArray) = "white" {}
		_Default ("_Default", Int) = 0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform UNITY_DECLARE_TEX2DARRAY( _Array );
			sampler2D _Sampler;
			int _Default;

			float4 frag( v2f_img i ) : SV_Target
			{
				if(_Default == 1)
				{
					return 1;
				}
				else if(_Default == 2)
				{
					return 0;
				} 
				else if(_Default == 3)
				{
					return 0.5f;
				}
				else if(_Default == 4)
				{
					return float4(0,0,1,1);
				}
				else if(_Default == 5) //texture array
				{
					return UNITY_SAMPLE_TEX2DARRAY( _Array, float3( i.uv, 0 ) );
				}
				else 
				{
					return tex2D( _Sampler, i.uv);
				}
			}
			ENDCG
		}
	}
}
