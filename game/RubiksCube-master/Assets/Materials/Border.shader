Shader "Unlit/Border"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _Border("Border",Float) = 0.8
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _Border;


            v2f vert (appdata v)
            {
                v2f o;
                o.pos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
               
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {


            	clip( 

            	max( 
            	max((_Border-1+i.uv.x),(_Border-i.uv.x)) ,
            	max((_Border-1+i.uv.y),(_Border-i.uv.y))
            	) 

            	);
                return _Color;
            }
            ENDCG
        }
    }
}
