// Upgrade NOTE: replaced 'samplerRECT' with 'sampler2D'
// Upgrade NOTE: replaced 'texRECT' with 'tex2D'

Shader "Hidden/Deemo Radial Blur" {
Properties {
    _MainTex ("Input", 2D) = "white" {}
    _BlurStrength ("Blur Strength", Float) = 0.5
    _BlurWidth ("Blur Width", Float) = 0.5
    _Center ("Center", Vector) = (0.5, 0.5, 0, 0 )
}
    SubShader {
        Pass {
            ZTest Off Cull Off ZWrite Off
            Fog { Mode off }
       
    CGPROGRAM
   
    #pragma vertex vert_img
    #pragma fragment frag
    #pragma fragmentoption ARB_precision_hint_fastest
 
    #include "UnityCG.cginc"
 
    uniform sampler2D _MainTex;
    uniform half _BlurStrength;
    uniform half _BlurWidth;
    uniform half4 _Center;
 
    half4 frag (v2f_img i) : COLOR {
        half4 color = tex2D(_MainTex, i.uv);
       
        // some sample positions
        half samples[10] = {-0.08,-0.05,-0.03,-0.02,-0.01,0.01,0.02,0.03,0.05,0.08};
       
        //vector to the middle of the screen
        half2 dir = _Center.xy - i.uv;
       
        //distance to center
        half dist = sqrt(dir.x*dir.x + dir.y*dir.y);
       
        //normalize direction
        dir = dir/dist;
       
        //additional samples towards center of screen
        half4 sum = color;
        for(int n = 0; n < 10; n++)
        {
            sum += tex2D(_MainTex, i.uv + dir * samples[n] * _BlurWidth);
        }
       
        //eleven samples...
        sum *= 1.0/11.0;
       
        //weighten blur depending on distance to screen center
        half t = dist * _BlurStrength;
        t = clamp(t, 0.0, 1.0);
       
        //blend original with blur
        return lerp(color, sum, t);
    }
    ENDCG
        }
    }
}