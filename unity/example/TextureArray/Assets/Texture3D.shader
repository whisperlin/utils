Shader "Test/Texture3D"
{
    Properties
    {
        _MainTexture("Texture", 3D) = "" {}
        _Z("Z",float)=0
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag  

            #include "UnityCG.cginc"  

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_tan v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            sampler3D _MainTexture;
            float _Z;

            float4 frag(v2f i) : COLOR
            {
                return tex3D(_MainTexture, fixed3(i.uv.x, i.uv.y, _Z));
            }

            ENDCG
        }
    }
}