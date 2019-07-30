Shader "Hidden/SamplerNode"
{
	Properties
	{
		_Sampler ("_Sampler", 2D) = "white" {}
		_B ("_UVs", 2D) = "white" {}
		_C ("_Level", 2D) = "white" {}
		_F ("_NormalScale", 2D) = "white" {}
		_CustomUVs ("_CustomUVs", Int) = 0
		_Unpack ("_Unpack", Int) = 0
		_LodType ("_LodType", Int) = 0
		_Default ("_Default", Int) = 0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"

			sampler2D _Sampler;
			sampler2D _B;
			sampler2D _C;
			sampler2D _F;
			int _CustomUVs;
			int _Unpack;
			int _LodType;
			int _Default;

			float4 frag( v2f_img i ) : SV_Target
			{
				//return (float)_Default/4.0;
				if( _Default == 1)
					return 1;
				else if( _Default == 2)
					return 0;
				else if( _Default == 3)
					return 0.5f;
				else if( _Default == 4)
				{
					float4 h = float4(0.5,0.5,1,1);
					if ( _Unpack == 1 ) {
						h.rgb = UnpackScaleNormal( h.xxyy, tex2D( _F, i.uv ).r );
					} 
					return h;
				}
				float2 uvs = i.uv;
				if ( _CustomUVs == 1 )
					uvs = tex2D( _B, i.uv ).xy;

				float n = tex2D( _F, i.uv ).r;

				float4 c = 0;
				if ( _LodType == 1 ) {
					float lod = tex2D( _C, i.uv ).r;
					c = tex2Dlod( _Sampler, float4(uvs,0,lod) );
				}
				else if ( _LodType == 2 ) {
					float bias = tex2D( _C, i.uv ).r;
					c = tex2Dbias( _Sampler, float4(uvs,0,bias) );
				}
				else {
					c = tex2D( _Sampler, uvs );
				}

				if ( _Unpack == 1 ) {
					c.rgb = UnpackScaleNormal( c, n );
				} 

				return c;
			}
			ENDCG
		}
	}
}
