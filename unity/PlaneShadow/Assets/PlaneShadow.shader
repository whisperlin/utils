Shader "Unlit/PlaneShadow"
{
	Properties
	{
		 
	}
	SubShader
	{
		Tags { "Queue" = "Transparent-1" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		LOD 100

		Pass
		{

			Stencil {
				Ref 6
				Comp NotEqual
				Pass Replace
		 
			}
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
 
			};

			struct v2f
			{
	 
 
				UNITY_FOG_COORDS(0)
				float4 vertex : SV_POSITION;
			};

			half4 PlaneShaderDirectLight0;
			half PlaneShaderHeight;

 
			float4 PlaneShadowColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				
				 
				float4 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
				float3 shdowDir = normalize(PlaneShaderDirectLight0.xyz);
				shdowDir.y = abs(shdowDir.y);
				float h0 = abs(worldPos.y - PlaneShaderHeight);
				worldPos.xz += shdowDir.xz* h0/shdowDir.y;
				worldPos.y = PlaneShaderHeight;
				o.vertex = mul(UNITY_MATRIX_VP, worldPos);

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
	 
				UNITY_APPLY_FOG(i.fogCoord, PlaneShadowColor);
				return PlaneShadowColor;
			}
			ENDCG
		}
	}
}
