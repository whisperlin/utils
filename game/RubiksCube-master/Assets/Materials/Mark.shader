Shader "Unlit/NewUnlitShader"
{
    Properties
    {
         
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Blend Zero One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
				
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
              
                float4 vertex : SV_POSITION;
            };

        

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
          
                return float4(0,0,0,0);
            }
            ENDCG
        }
    }
}
