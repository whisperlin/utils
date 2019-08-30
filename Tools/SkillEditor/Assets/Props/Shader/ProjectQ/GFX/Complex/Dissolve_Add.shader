// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ProjectQ/GFX/Complex/Dissolve_Add" {
    Properties {
        _Diffuse_Textures ("Diffuse_Textures", 2D) = "white" {}
        _Diffuse_Color ("Diffuse_Color", Color) = (1,1,1,1)
        _Dessolve_Alpha ("Dessolve_Textures(alpha)", 2D) = "white" {}
        _Dissolve ("Dissolve", Range(0, 2)) = 0
        _Dissolve_Age ("Dissolve_Age", Range(0, 1)) = 0
        _Age_Color ("Age_Color", Color) = (1,1,1,1)
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
            #pragma target 2.0
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "../../CGInclude/CGInclude.cginc"

            uniform sampler2D _Diffuse_Textures; uniform half4 _Diffuse_Textures_ST;
            uniform half4 _Diffuse_Color;
            uniform sampler2D _Dessolve_Alpha; uniform half4 _Dessolve_Alpha_ST;
            uniform fixed _Dissolve;
            uniform fixed _Dissolve_Age;
            uniform half4 _Age_Color;

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
				o.uv0.xy = TRANSFORM_TEX(v.texcoord0, _Diffuse_Textures);
				o.uv0.zw = TRANSFORM_TEX(v.texcoord0, _Dessolve_Alpha);
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				MY_TRANSFER_FOG(o, worldPos, o.pos.z);
                return o;
            }

			fixed4 frag(VertexOutput i) : COLOR
			{
				fixed4 _Diffuse_Tex = tex2D(_Diffuse_Textures,i.uv0.xy);
				float4 _Dessolve_Tex = tex2D(_Dessolve_Alpha, i.uv0.zw);
				_Dissolve += 1-i.vertexColor.a;
				fixed _clip = _Dessolve_Tex.r-_Dissolve; 
                 clip(_clip);
				fixed3 c = i.vertexColor.rgb * _Diffuse_Tex.rgb * (_Diffuse_Color.rgb * _Diffuse_Color.a * 2.0);
				c *= i.vertexColor.a*_Dessolve_Tex.r;
				fixed3 finalrgb = c*((_clip-_Dissolve_Age)>0) + (c.r+c.g+c.b)*(step(_clip, _Dissolve_Age))*_Age_Color;
				MY_APPLY_FOG_COLOR(i, c, fixed4(0, 0, 0, 0), fixed4(0, 0, 0, 0));
                return fixed4(finalrgb,1);
            }
            ENDCG
        }
    }
    FallBack "Legacy Shaders/Diffuse"
}
