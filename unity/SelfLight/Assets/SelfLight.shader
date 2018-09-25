// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SelfLight"
{
	Properties
	{
		_Color("自发光颜色", Color) = (0,0,0,1)
		_Power("强度", Range(1, 10)) = 5
		_Radius("半径", Range(0, 10)) = 3
  
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		Blend One One
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"


			struct v2f {
				half3 worldNormal : TEXCOORD0;
				float rim : TEXCOORD1;
				float4 pos : SV_POSITION;
			};
			float _Power;
			float _Radius;
			v2f vert(float4 vertex : POSITION, float3 normal : NORMAL)
			{
				v2f o;
				float3 pos = vertex + normal*_Radius;
				o.pos = UnityObjectToClipPos(float4(pos, 1.0));
				float3 viewDir = ObjSpaceViewDir(vertex);
				viewDir = normalize(viewDir);
				float f = max(0, dot(viewDir, normal));
				o.rim = f*f *_Power;
				o.worldNormal = normal;
				return o;
			}
			fixed4 _Color;
			fixed4 frag(v2f i) : SV_Target
			{
				return _Color * i.rim;
			}
				 

			ENDCG
		}
	}
}
