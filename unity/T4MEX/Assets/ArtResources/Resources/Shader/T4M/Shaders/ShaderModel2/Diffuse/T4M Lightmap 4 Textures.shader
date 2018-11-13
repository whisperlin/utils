// Upgrade NOTE: upgraded instancing buffer 'MyProperties' to new syntax.

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


//调节光照贴图亮度
Shader "T4MShaders/ShaderModel2/Lightmap/T4M 4 Textures"
{
    Properties {
		_Color ("Main Color", Color) = (1,1,1,1)

		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}
		_Splat3 ("Layer 4", 2D) = "white" {}

		_Tiling3("_Tiling3 x/y", Vector)=(1,1,0,0)
		_Tiling4("_Tiling4 x/y", Vector)=(1,1,0,0)

		_Control ("Control (RGBA)", 2D) = "white" {}

        _MainTex ("MainTex (RGBA)", 2D) = "white" {}
        _Lightmap("Lightmap (RGB)", 2D) = "white" {}

		_Intensity("Intensity", Float) = 1
    }

    SubShader {
        Tags { "SplatCount" = "4" "RenderType"="Opaque" "Queue" = "Geometry-50"}
        LOD 200
        ZWrite On
		ZTest Off

		//ZWrite Off  
		//Blend SrcAlpha OneMinusSrcAlpha

        Pass {
			//Tags { "LightMode" = "VertexLM" }
			
			//Blend One One ZWrite Off
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Back

            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct appdata members uv_Texture1,uv_Texture2,uv_Texture3,uv_Texture4)
#pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
			#pragma multi_compile_instancing
			
			sampler2D _MainTex;
			sampler2D _Control;

			sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
			float4 _Splat0_ST;
			float4 _Splat1_ST; 
			//float4 _Splat2_ST;
			//float4 _Splat3_ST;

			float4 _Tiling3;
			float4 _Tiling4;
            
            sampler2D _Lightmap;
            float4 _Lightmap_ST;

			//fixed4 _Color;
			//float _Intensity;

			UNITY_INSTANCING_BUFFER_START(MyProperties)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
#define _Color_arr MyProperties
				UNITY_DEFINE_INSTANCED_PROP(float, _Intensity)
#define _Intensity_arr MyProperties
				//UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
			UNITY_INSTANCING_BUFFER_END(MyProperties)

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float2 uv_Texture1 : TEXCOORD2;
				float2 uv_Texture2 : TEXCOORD3;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            struct v2f {
				float4 pos : SV_POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float2 uv2_Lightmap : TEXCOORD1;
				float2 uv_Texture1 : TEXCOORD2;
				float2 uv_Texture2 : TEXCOORD3;

                UNITY_FOG_COORDS(6)

				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float4 _MainTex_ST;

            v2f vert (appdata IN)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.pos = UnityObjectToClipPos(IN.vertex);
				o.uv_MainTex = TRANSFORM_TEX(IN.uv, _MainTex);
				o.uv2_Lightmap = TRANSFORM_TEX(IN.uv2, _Lightmap);

				o.uv_Texture1 = TRANSFORM_TEX(IN.uv_Texture1, _Splat0);
				o.uv_Texture2 = TRANSFORM_TEX(IN.uv_Texture2, _Splat1);

                UNITY_TRANSFER_FOG(o, o.pos);
				
				return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                //fixed4 c = tex2D (_MainTex, i.uv_MainTex) * UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);

                fixed4 c = tex2D (_Control, i.uv_MainTex);
		
				fixed3 lay1 = tex2D (_Splat0, i.uv_Texture1);
				fixed3 lay2 = tex2D (_Splat1, i.uv_Texture2);
				fixed3 lay3 = tex2D (_Splat2, i.uv_MainTex * _Tiling3.xy);
				fixed3 lay4 = tex2D (_Splat3, i.uv_MainTex * _Tiling4.xy);

				c.rgb = (lay1 * c.r + lay2 * c.g + lay3 * c.b + lay4 * c.a);

				fixed4 lm = tex2D (_Lightmap, i.uv2_Lightmap);
                c.rgb *= (unity_Lightmap_HDR.x * pow(lm.a, unity_Lightmap_HDR.y)) * lm.rgb;

				c.rgb *= UNITY_ACCESS_INSTANCED_PROP(_Intensity_arr, _Intensity);
				c.a = 1;

                UNITY_APPLY_FOG(i.fogCoord, c);

                return c;
            }

            ENDCG
        }
    }

	FallBack "Diffuse"
}
