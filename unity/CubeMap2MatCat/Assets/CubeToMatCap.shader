// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CubeToMatCap" 
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MatCap("MatCap", 2D) = "white" {}
        _MatCapFactor("MatCapFactor", Range(0,5)) = 1
        _EnvTex("EnvTex (CubeMap)", Cube) = "_SkyBox" {}
        _EnvFactor("EnvStrength", Range(0,1)) = 0.8
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
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 twoUv : TEXCOORD0;//主纹理uv存xy,matCapUv存在zw,这算一种优化
                float4 vertex : SV_POSITION;
                float3 RefDir : TEXCOORD1;
                float4 vertex0 : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MatCap;
            half _MatCapFactor;
            samplerCUBE _EnvTex;
            half _EnvFactor;

            v2f vert (appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex* float4(2,2,1,1);

                o.twoUv.xy = TRANSFORM_TEX(v.uv, _MainTex);

                //matcap存的其实是：法线中xy分量作为uv，对应的光照颜色信息，即用xy去采样就好，注：xy必须是归一化法线中的分量，故z才没必要
                o.twoUv.zw = UnityObjectToWorldNormal(v.normal).xy;
                o.twoUv.zw = o.twoUv.zw * 0.5 + 0.5;//(-1,1)->(0,1)
                float3 wolrdN = UnityObjectToWorldNormal(v.normal);
                o.RefDir = reflect(-WorldSpaceViewDir(v.vertex), wolrdN);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
             
           		//fixed4 col = texCUBE(_EnvTex, _normal);
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.twoUv.xy);
                //fixed4 mapCapCol = tex2D(_MatCap, i.twoUv.zw);
                fixed4 reflection = texCUBE(_EnvTex, i.RefDir);
                //col.rgb = col.rgb * mapCapCol.rgb * _MatCapFactor + reflection.rgb * _EnvFactor;


                //fixed4 col = texCUBE(_EnvTex, i.RefDir);
                //fixed4 col  = fixed4(i.twoUv.xy,0,1);
                //fixed4 col = fixed4(z0,0,0,1);
                return reflection;
            }
            ENDCG
        }
    }
}
