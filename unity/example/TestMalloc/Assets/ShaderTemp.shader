Shader "Custom/Material Property Drawer Example"
{
    Properties
    {
        // Header creates a header text before the shader property.
        [Header(Material Property Drawer Example)]
        // Space creates vertical space before the shader property.
        [Space]

        _MainTex ("Main Tex", 2D) = "white" {}
        _SecondTex ("Second Tex", 2D) = "white" {}

        // Large amount of space
        [Space(50)]

        // Toggle displays a **float** as a toggle. 
        // The property value will be 0 or 1, depending on the toggle state. 
        // When it is on, a shader keyword with the uppercase property name +"_ON" will be set, 
        // or an explicitly specified shader keyword.
        [Toggle] _Invert ("Invert color?", Float) = 0

        // Will set "ENABLE_FANCY" shader keyword when set
        [Toggle(ENABLE_FANCY)] _Fancy ("Fancy?", Float) = 0

        // Enum displays a popup menu for a **float** property. 
        // You can supply either an enum type name 
        // (preferably fully qualified with namespaces, in case there are multiple types), 
        // or explicit name/value pairs to display. 
        // Up to **7** name/value pairs can be specified
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend Mode", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend Mode", Float) = 1
        [Enum(Off, 0, On, 1)] _ZWrite ("ZWrite", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 1

        // KeywordEnum displays a popup menu for a **float** property, and enables corresponding shader keyword. 
        // This is used with "#pragma multi_compile" in shaders, to enable or disable parts of shader code. 
        // Each name will enable "property name" + underscore + "enum name", uppercased, shader keyword. 
        // Up to **9** names can be provided.
        [KeywordEnum(None, Add, Multiply)] _Overlay ("Overlay mode", Float) = 0

        // PowerSlider displays a slider with a non-linear response for a Range shader property.
        // A slider with 3.0 response curve
        [PowerSlider(3.0)] _Shininess ("Shininess", Range (0.01, 1)) = 0.08
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        ZTest [_ZTest]
        Cull [_Cull]

        Pass
        {
            CGPROGRAM
            // Need to define _INVERT_ON shader keyword
            #pragma shader_feature _INVERT_ON
            // Need to define _INVERT_ON shader keyword
            #pragma shader_feature ENABLE_FANCY
            // No comma between features
            #pragma multi_compile _OVERLAY_NONE _OVERLAY_ADD _OVERLAY_MULTIPLY

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SecondTex;
            float4 _SecondTex_ST;
            float _Shininess;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.uv, _SecondTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv.xy);

                // Use #if, #ifdef or #if defined
                #if _INVERT_ON
                col = 1 - col;
                #endif

                // Use #if, #ifdef or #if defined
                #if ENABLE_FANCY
                col.r = 0.5;
                #endif

                fixed4 secCol = tex2D(_SecondTex, i.uv.zw);

                #if _OVERLAY_ADD
                col += secCol;
                #elif _OVERLAY_MULTIPLY
                col *= secCol;
                #endif

                col *= _Shininess;

                return col;
            }
            ENDCG
        }
    }
}
