Shader "Hidden/Post FX/Uber Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AutoExposure ("", 2D) = "" {}
        _BloomTex ("", 2D) = "" {}
        _GrainTex ("", 2D) = "" {}
        _LogLut ("", 2D) = "" {}
        _UserLut ("", 2D) = "" {}
        _Vignette_Mask ("", 2D) = "" {}
    }

    CGINCLUDE

        #pragma target 3.0

        #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
        #pragma multi_compile __ DEPTH_OF_FIELD DEPTH_OF_FIELD_COC_VIEW
        #pragma multi_compile __ BLOOM  
        #pragma multi_compile __ COLOR_GRADING COLOR_GRADING_LOG_VIEW
        #pragma multi_compile __ VIGNETTE_CLASSIC VIGNETTE_MASKED
 

        #include "UnityCG.cginc"
        #include "Bloom.cginc"
        #include "ColorGrading.cginc"
 

        // Auto exposure / eye adaptation
        sampler2D _AutoExposure;

 


        // Depth of field
        sampler2D_float _CameraDepthTexture;
        sampler2D _DepthOfFieldTex;
        float4 _DepthOfFieldTex_TexelSize;
        float2 _DepthOfFieldParams; // x: distance, y: f^2 / (N * (S1 - f) * film_width * 2)

        // Bloom
        sampler2D _BloomTex;
        float4 _BloomTex_TexelSize;
        half2 _Bloom_Settings; // x: sampleScale, y: bloom.intensity

 

        // Color grading & tonemapping
        sampler2D _LogLut;
        half3 _LogLut_Params; // x: 1 / lut_width, y: 1 / lut_height, z: lut_height - 1
        half _ExposureEV; // EV (exp2)

        // User lut
        sampler2D _UserLut;
        half4 _UserLut_Params; // @see _LogLut_Params

        // Vignette
        half3 _Vignette_Color;
        half2 _Vignette_Center; // UV space
        half4 _Vignette_Settings; // x: intensity, y: smoothness, z: roundness, w: rounded
        sampler2D _Vignette_Mask;
        half _Vignette_Opacity; // [0;1]

        struct VaryingsFlipped
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float2 uvSPR : TEXCOORD1; // Single Pass Stereo UVs
            float2 uvFlipped : TEXCOORD2; // Flipped UVs (DX/MSAA/Forward)
            float2 uvFlippedSPR : TEXCOORD3; // Single Pass Stereo flipped UVs
        };

        VaryingsFlipped VertUber(AttributesDefault v)
        {
            VaryingsFlipped o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord.xy;
            o.uvSPR = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy, _MainTex_ST);
            o.uvFlipped = v.texcoord.xy;

        #if UNITY_UV_STARTS_AT_TOP
            if (_MainTex_TexelSize.y < 0.0)
                o.uvFlipped.y = 1.0 - o.uvFlipped.y;
        #endif

            o.uvFlippedSPR = UnityStereoScreenSpaceUVAdjust(o.uvFlipped, _MainTex_ST);

            return o;
        }

        half4 FragUber(VaryingsFlipped i) : SV_Target
        {
            float2 uv = i.uv;
            half autoExposure = tex2D(_AutoExposure, uv).r;

            half3 color = (0.0).xxx;
         

            //
            // HDR effects
            // ---------------------------------------------------------

            // Chromatic Aberration
            // Inspired by the method described in "Rendering Inside" [Playdead 2016]
            // https://twitter.com/pixelmager/status/717019757766123520
            
			color = tex2D(_MainTex, i.uvSPR).rgb;

            // Apply auto exposure if any
            color *= autoExposure;

            // Gamma space... Gah.
            #if UNITY_COLORSPACE_GAMMA
            {
                color = GammaToLinearSpace(color);
            }
            #endif

            // Depth of field
            #if DEPTH_OF_FIELD
            {
                half4 dof = tex2D(_DepthOfFieldTex, i.uvFlippedSPR);
                color = color * dof.a + dof.rgb * autoExposure;
            }
            #elif DEPTH_OF_FIELD_COC_VIEW
            {
                // Calculate the radiuses of CoC.
                half4 src = tex2D(_DepthOfFieldTex, uv);
                float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uvFlippedSPR));
                float coc = (depth - _DepthOfFieldParams.x) * _DepthOfFieldParams.y / depth;
                coc *= 80;

                // Visualize CoC (white -> red -> gray)
                half3 rgb = lerp(half3(1, 0, 0), half3(1.0, 1.0, 1.0), saturate(-coc));
                rgb = lerp(rgb, half3(0.4, 0.4, 0.4), saturate(coc));

                // Black and white image overlay
                rgb *= AcesLuminance(color) + 0.5;

                // Gamma correction
                #if !UNITY_COLORSPACE_GAMMA
                {
                    rgb = GammaToLinearSpace(rgb);
                }
                #endif

                color = rgb;
            }
            #endif

            // HDR Bloom
            #if BLOOM  
            {
                half3 bloom = UpsampleFilter(_BloomTex, i.uvFlippedSPR, _BloomTex_TexelSize.xy, _Bloom_Settings.x) * _Bloom_Settings.y;
                color += bloom;

                
                
            }
            #endif

            // Procedural vignette
            #if VIGNETTE_CLASSIC
            {
                half2 d = abs(uv - _Vignette_Center) * _Vignette_Settings.x;
                d.x *= lerp(1.0, _ScreenParams.x / _ScreenParams.y, _Vignette_Settings.w);
                d = pow(d, _Vignette_Settings.z); // Roundness
                half vfactor = pow(saturate(1.0 - dot(d, d)), _Vignette_Settings.y);
                color *= lerp(_Vignette_Color, (1.0).xxx, vfactor);
            }

            // Masked vignette
            #elif VIGNETTE_MASKED
            {
                half vfactor = tex2D(_Vignette_Mask, uv).a;
                half3 new_color = color * lerp(_Vignette_Color, (1.0).xxx, vfactor);
                color = lerp(color, new_color, _Vignette_Opacity);
            }
            #endif

            // HDR color grading & tonemapping
            #if COLOR_GRADING
            {
                color *= _ExposureEV; // Exposure is in ev units (or 'stops')

                half3 colorLogC = saturate(LinearToLogC(color));
                color = ApplyLut2d(_LogLut, colorLogC, _LogLut_Params);
            }
            #elif COLOR_GRADING_LOG_VIEW
            {
                color *= _ExposureEV;
                color = saturate(LinearToLogC(color));
            }
            #endif

            //
            // All the following effects happen in LDR
            // ---------------------------------------------------------

            color = saturate(color);

            // Back to gamma space if needed
            #if UNITY_COLORSPACE_GAMMA
            {
                color = LinearToGammaSpace(color);
            }
            #endif


 

            // Done !
            return half4(color, 1.0);
        }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        // (0)
        Pass
        {
            CGPROGRAM

                #pragma vertex VertUber
                #pragma fragment FragUber

            ENDCG
        }
    }
}
