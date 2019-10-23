// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ShadrTest" {
	Properties{
		_Color("Tint",Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 1  //声明外部控制开关
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 0  //声明外部控制开关
		[Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 0  //声明外部控制开关
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0  //声明外部控制开关
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 1  //声明外部控制开关
	}

		SubShader{
			Tags {"Queue" = "Geometry" "RenderType" = "Opaque"}
			LOD 200
			Blend[_SrcBlend][_DstBlend] //获取值应用
			ZWrite[_ZWrite] //获取值应用
			ZTest[_ZTest] //获取值应用
			Cull[_Cull] //获取值应用

			Pass {
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag

					#include "UnityCG.cginc"

					struct appdata_t {
						float4 vertex : POSITION;
						float2 texcoord : TEXCOORD0;
						fixed4 color : COLOR;
					};

					struct v2f {
						float4 vertex : SV_POSITION;
						half2 texcoord : TEXCOORD0;
						fixed4 color : TEXCOORD1;
					};

					sampler2D _MainTex;
					float4 _MainTex_ST;
					fixed4 _Color;

					v2f vert(appdata_t v)
					{
						v2f o;
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						o.color = v.color;
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						fixed4 col = tex2D(_MainTex, i.texcoord)*_Color * i.color;
						return col;
					}
				ENDCG
			}
		}

}