Shader "Perview/Texture3dArray"
{
    Properties {
       _TextureArray("TexArray", 2DArray) = "" {}
        _Index("Index",float)=0
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float2 uv_TextureArray;
      };
     UNITY_DECLARE_TEX2DARRAY(_TextureArray);
     float _Index;
      void surf (Input IN, inout SurfaceOutput o) {
      	  o.Albedo = UNITY_SAMPLE_TEX2DARRAY(_TextureArray, float3(IN.uv_TextureArray.x, IN.uv_TextureArray.y, _Index)).rgb;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }
