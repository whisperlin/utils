Shader "Unlit/DiffuseLightMap"
{
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _LightTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
 
                 float2 texcoord3 : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID 
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                 float2 texcoord1 : TEXCOORD1;
                UNITY_FOG_COORDS(3)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

             sampler2D _LightTex;
            float4 _LightTex_ST;


            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.texcoord1 = TRANSFORM_TEX(v.texcoord3, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                fixed4 light = tex2D(_LightTex, i.texcoord1);
                light.rgb =  (light.rgb - 0.5)*2;

                 // #ifdef UNITY_COLORSPACE_GAMMA
                //	light.rgb = GammaToLinearSpace(light.rgb);
                //	col.rgb =   GammaToLinearSpace(col.rgb);
				//#endif

                col.rgb =  col.rgb +  light.rgb;

                //#ifdef UNITY_COLORSPACE_GAMMA
                //	col.rgb =  LinearToGammaSpace(col.rgb); 
				//#endif
                UNITY_APPLY_FOG(i.fogCoord, col);
                //UNITY_OPAQUE_ALPHA(col.a);
                return col;
            }
        ENDCG
    }
}

}

