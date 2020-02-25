Shader"MaterialPropertyDrawer/MaterialPropertyDrawerKeywordEnum"{
	Properties{
		[KeywordEnum(Red,Green,Blue)]_ColorMode("Color Mode",Float) = 0
		[Toggle(_MyToggle1)] _button("Toggle", Float) = 0

		[MaterialToggle] _mt("MaterialToggle", Float) = 0
	}
		SubShader{
			pass {
				Tags{"LightMode" = "ForwardBase"}
				CGPROGRAM
				#pragma multi_compile  _COLORMODE_RED _COLORMODE_GREEN _COLORMODE_BLUE
				//#pragma shader_feature _COLORMODE_RED _COLORMODE_GREEN _COLORMODE_BLUE
				#pragma multi_compile  _ _MyToggle1 
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct VertOut{
					float4 pos:SV_POSITION;
					float4 color:COLOR;
				};
				VertOut vert(appdata_base v)
				{

					VertOut o = (VertOut)0;



					o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
					o.color = float4(0,0,0,1);

					#if _COLORMODE_RED
						o.color = float4(1,0,0,1);

					#elif _COLORMODE_GREEN
						o.color = float4(0,1,0,1);

					#elif _COLORMODE_BLUE
						o.color = float4(0,0,1,1);
					#endif

					return o;
				}
				float _mt;
				float4 frag(VertOut i) :COLOR
				{
					if (_mt > 0)
					return float4(0,1,1,1);
#if _MyToggle1
					return float4(1, 1, 1, 1);
#endif
					return i.color;
				}
				ENDCG
	}//end pass
	}
}