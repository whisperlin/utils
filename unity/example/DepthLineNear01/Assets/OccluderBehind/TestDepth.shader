Shader "Unlit/TestDepth"
{
	Properties
	{
		//_Behind ("_Behind", 2D) = "white" {}
		_Test("Test",Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct v2f 
			{
				float4 pos : POSITION;
				float4 nz : TEXCOORD0;
				float4 screenPos:TEXCOORD1;
			};
            
			v2f vert( appdata_base v )
			{
				v2f o;
				o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
				o.nz.xyz = COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				o.screenPos = ComputeScreenPos ( o.pos );
				return o;
			}
            sampler2D _Behind;
			float _Test;
			fixed4 frag( v2f i ) : COLOR 
			{
				float behindDepth, occluderDepth;
				float3 behindNormal, occluderNormal;
				float2 screenUV = ( i.screenPos.xy / i.screenPos.w   )  ;
				
				float4 col0 = tex2D ( _Behind, screenUV );
				
				DecodeDepthNormal ( col0, behindDepth, behindNormal );
				
				if (behindDepth > 0  && behindDepth > i.nz.w)
				{

					return  behindDepth - i.nz.w  ;
				
				}
				clip(0.4-screenUV.x);
				return behindDepth  ;
			}
			ENDCG
		}
	}
}
