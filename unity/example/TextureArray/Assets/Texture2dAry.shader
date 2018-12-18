Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _TextureArray("TexArray", 2DArray) = "" {}
        _Index("Index",float)=0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma target 3.5
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Index;
            
            UNITY_DECLARE_TEX2DARRAY(_TextureArray);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 r = UNITY_SAMPLE_TEX2DARRAY(_TextureArray, float3(i.uv.x, i.uv.y, _Index));
                return r;
            }
            ENDCG
        }
    }
}