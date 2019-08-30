// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ProjectQ/GFX/Complex/GFX_Fluild_Add" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _power ("power", Range(0, 10)) = 0
        _fliuld ("fliuld", Range(-1, 1)) = 0
        _fliuld_power ("fliuld_power", Range(1, 5)) = 1
        _Diffuse ("Diffuse", 2D) = "white" {}
        _mask ("mask", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {

            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma target 2.0

			#include "UnityCG.cginc"
			#include "../../CGInclude/CGInclude.cginc"

            uniform half4 _Color;
            uniform fixed _power;
            uniform fixed _fliuld;
            uniform fixed _fliuld_power;
            uniform sampler2D _Diffuse; uniform half4 _Diffuse_ST;
            uniform sampler2D _mask; uniform half4 _mask_ST;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
				fixed4 vertexColor : COLOR;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
				fixed4 vertexColor : COLOR;
				MY_FOG_LIGHTMAP_COORDS(1)
				MY_FOGWAR_COORDS(2)
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
				o.uv0.xy = v.texcoord0;// TRANSFORM_TEX(v.texcoord0, _Diffuse);
				o.uv0.zw = TRANSFORM_TEX(v.texcoord0, _mask);
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				MY_TRANSFER_FOG(o, worldPos, o.pos.z);
                return o;
            }

			fixed4 frag(VertexOutput i) : COLOR
			{
                fixed4 _DiffuseTex = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                half2 duv = (i.uv0+(_Time.g * _fliuld) * fixed2(0,_fliuld_power));
                fixed4 _DiffuseTex_Dlta = tex2D(_Diffuse,TRANSFORM_TEX(duv, _Diffuse));
                fixed4 _maskTex = tex2D(_mask, i.uv0.zw);

				fixed power = _power* i.vertexColor.a  * _Color.a;
				fixed3 finalColor = _Color.rgb * _DiffuseTex.rgb * power * _maskTex.r*_DiffuseTex_Dlta.rgb;
   
                fixed4 finalRGBA = fixed4(finalColor,1);

				MY_APPLY_FOG_COLOR(i, finalRGBA, fixed4(0, 0, 0, 0), fixed4(0, 0, 0, 0));

                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
