// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TestBlend" {
 Properties {
  _Color ("Color", Color) = (1.0, 0.0, 0.0, 0.2)
 }
   SubShader {
      Tags { "Queue" = "Transparent" } 

     
         // draw after all opaque geometry has been drawn
      Pass { 
         Cull Off // draw front and back faces
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
         Blend Zero OneMinusSrcAlpha // multiplicative blending 
            // for attenuation by the fragment's alpha

         CGPROGRAM 
 		 uniform float4 _Color;
         #pragma vertex vert 
         #pragma fragment frag
 
         float4 vert(float4 vertexPos : POSITION) : SV_POSITION 
         {
            return UnityObjectToClipPos(vertexPos);
         }
 
         float4 frag(void) : COLOR 
         {
            return _Color; 
         }
 
         ENDCG  
      }

      Pass { 
         Cull Off // draw front and back faces
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
         Blend SrcAlpha One // additive blending to add colors

         CGPROGRAM 
 			 uniform float4 _Color;
         #pragma vertex vert 
         #pragma fragment frag
 
         float4 vert(float4 vertexPos : POSITION) : SV_POSITION 
         {
            return UnityObjectToClipPos(vertexPos);
         }
 
         float4 frag(void) : COLOR 
         {
            return _Color; 
         }
 
         ENDCG  
      }
   }
}