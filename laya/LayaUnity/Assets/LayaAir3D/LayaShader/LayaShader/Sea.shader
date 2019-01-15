Shader "LayaAir3D/Addition/Sea"
{
    Properties
    {     

        spltex("_SPL",2D) =  "blue" {}
        displacement("displacement",2D) = "White" {}
        normalTexture("normalTexture",2D) = "Bump" {}
        foamTexture("foamTexture",2D) = "White" {}
        reflectionTex("reflectionTex",2D) = "White"{}
        wavesTexture("wavesTexture",2D) = "White" {}
        maskWavesDisplacement("maskWavesDisplacement",2D) = "black" {}

        spec("specular",Float) = 0.7
        reflectionIntensity("reflectionIntensity",Float) =0.4
        reflectionFresnel("reflectionFresnel-spcularhighlight",Float) = 1.83
        gloss("gloss",Float) = 60.8   
        fresnelColor("fresnelColor",Color) = (0,0,0,0)
        objScale("objScale.XZ",vector) = (500,-100,500,-100)
        waterColor("waterColor",Color) = (1,1,1,1)
        displacementSpeed("displacementSpeed",Range(0,0.5)) = 0.143
        displacementScale("displacementScale",Float) =0.12
        displacementIntensity("displacementIntensity",Range(0,0.3)) =0.0005
        normalIntensity("normalIntensity",Range(-3,3)) = 0.48
        wavesScale("wavesScale",Float ) = 0.31
        wavesDisplacementSpeed("wavesDisplacementSpeed",Float) = 0.21
         sceneZ("sceneZ",Float) =2.46
         partZ("partZ",Float) = 0
        displacementFoamIntensity("displacementFoamIntensity",Float)= 0.04
        shoreFoamIntensity("shoreFoamIntensity",Float) = 1.48
    	shoreLineOpacity("shoreLineOpacity",Float) = 1
        savesDisplacementFoamIntensity("savesDisplacementFoamIntensity",Float) = 0.03
        
        fadeLevel("fadeLevel",Float ) = 11.77
        shoreFoamDistance("shoreFoamDistance",Float ) = 0.16

        reflectionColor("reflectionColor",Color) = (1,1,1,1)
        wavesAmount("wavesAmount",Float) =0.01
        wavesSpeed("wavesSpeed",Float) = 0.28
        wavesIntensity("wavesIntensity",Float ) =0.36
        radialWaves("radialWaves",Float) =0.38
        waterDensity("waterDensity",Float) = 0.038
        shoreWaterOpacity("shoreWaterOpacity",Float ) = 0.45
        waveHeight("waveHeight",Float) = 1
        vHeight("vHeight",Float) = 1
        foamScale("foamScale",Float) = 1.13
        foamSpeed("foamSpeed",Float) =1.43
        skyBlur("skyBlur",Range(0,10))=2.09
        reflectionmapColor("reflectionmapColor",Color) = (1,1,1,1)
        
        
        
    }
    SubShader
    {
          Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent+1000"
        }

        Pass
        {

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            #include "../CGIncludes/RolantinCG.cginc" 


            struct vertexin
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
                float4 tangent : TANGENT;
            };

            struct vertexout
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normalDir:TEXCOORD3;
                float4 posWorld: TEXCOORD2;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
            };

            uniform float time;
            uniform float2 resolution;
            uniform float2 mouse;
            uniform float3 spectrum;
            float partZ;
            float sceneZ;
            float skyBlur;
            float4 fresnelColor;


            uniform sampler2D texture0;
            uniform sampler2D texture1;
            uniform sampler2D texture2;
            uniform sampler2D texture3;
            uniform sampler2D prevFrame;
            uniform sampler2D prevPass;
            uniform sampler2D foamTexture;float4 foamTexture_ST;
            uniform float foamScale;
            uniform float foamSpeed;

            uniform sampler2D reflectionTex;float4 reflectionTex_ST;
            float4 reflectionmapColor;

     
 		 	float shoreLineOpacity;
            uniform float3 objScale;

            uniform float4 waterColor;
            uniform float3 lightcolor;
            uniform float lightintensity;

            uniform float _RefractionIntensity;

            uniform sampler2D normalTexture;float4 normalTexture_ST;

            uniform float wavesSpeed;
            uniform float wavesScale;
 
            uniform float wavesIntensity;
            float displacementIntensity;

            uniform float normalIntensity;

            uniform float waterDensity;

            uniform float spec;
       
//          uniform float4 _WorldSpaceCameraPos;

            uniform float reflectionFresnel;
            uniform float gloss;
            uniform float shoreWaterOpacity;
            uniform sampler2D wavesTexture;float4 wavesTexture_ST;

            uniform sampler2D maskWavesDisplacement;float4 maskWavesDisplacement_ST;
            uniform sampler2D displacement;float4 displacement_ST;

            uniform float _UseMask;
            uniform float radialWaves;
            uniform float wavesAmount;
            uniform float wavesDisplacementSpeed;
            uniform float displacementScale;
            uniform float savesDisplacementFoamIntensity;
            uniform float displacementSpeed;
            uniform float displacementFoamIntensity;
            uniform float4 reflectionColor;
            uniform float reflectionIntensity;
            uniform float shoreFoamIntensity;
            uniform float shoreFoamDistance;
            uniform float fadeLevel;
            //uniform samplerCUBE  _SPL;
            uniform sampler2D spltex;
            
            vertexout vert (vertexin v)
            {  
                 vertexout o = (vertexout)0;

                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);


       			float uTime =  _Time.y;
               
                float mytime = uTime * wavesDisplacementSpeed;


                //float t = _TimeEditor.x;
                float2 uv = v.uv;
                ///scale obj x z coordinate
                float2 ScaleOBJ = (float2(objScale.r,objScale.b)*displacementScale * uv*0.1);

                float4 _Displacement_var2 = tex2Dlod(displacement, float4(ScaleOBJ+displacementSpeed*mytime.r*float2(-1,1),0,0));

                float2 node_1716 = ((ScaleOBJ+float2(0.5,0.5))*0.75) +displacementSpeed*mytime.r*float2(1.0,-1.0);

                float4 _Displacement_var = tex2Dlod(displacement,float4(TRANSFORM_TEX(node_1716,displacement),0,0));
            

                float3 lerpdis = lerp(_Displacement_var2.rgb,_Displacement_var2.rgb,0.5);

                float node_4826 = (1.0 - length((uv*2.0-1.0)));
                float2 node_3934 = float2(node_4826,node_4826);

                
////water mask vertex anim range
                float4 maskWavesDisplacement_var= tex2Dlod(maskWavesDisplacement,float4(TRANSFORM_TEX(float2(uv * -1.0),maskWavesDisplacement),0.0,0)) ;
               
                float2 node_2367 = ((lerp( uv, lerp( node_3934, (node_3934*dot(maskWavesDisplacement_var.rgb,float3(0.3,0.59,0.11))), 1 ),
                     radialWaves )*float2(objScale.r,objScale.b)*wavesAmount*0.1)
                    +(wavesDisplacementSpeed*mytime.r*(0*2.0-1.0))
                    *float2(1,1));

                 float4 wavesTexture_var = tex2Dlod(wavesTexture,float4(TRANSFORM_TEX(node_2367, wavesTexture),0.0,0));
               

                float node_8869 = dot(wavesTexture_var.rgb,float3(0.3,0.59,0.11));
        
         float node_3527 = cos(mytime+2) + sin(mytime+1);
             //  float3 vertex = v.vertex+ node_6532;
                 v.vertex.xyz += ((lerpdis+(node_8869*wavesIntensity))*float3(0,1,0)*displacementIntensity);
                o.uv = uv;
              //  v.vertex.xyz += maskWavesDisplacement_var.xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(  v.vertex);
                 o.normalDir = UnityObjectToWorldNormal(v.normal);

                
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }



            float3x3 computeTBN(vertexout i){
                  float3 dp1 = ddx(i.vertex);
                  float3 dp2 = ddy(i.vertex);
                  float2 duv1 = ddx( i.uv);
                  float2 duv2 = ddy( i.uv);
                  float3 dp2perp = cross( dp2,i.normalDir );
                  float3 dp1perp = cross( i.normalDir, dp1 );
                  float3 tangent = dp2perp * duv1.x + dp1perp * duv2.x;
                  float3 binormal = dp2perp * duv1.y + dp1perp * duv2.y;
                  float invmax = rsqrt( max( dot(tangent,tangent), dot(binormal,binormal) ) );
                  return float3x3( tangent * invmax, binormal * invmax, i.normalDir );
              }
                        
                float3 SBLsky(samplerCUBE CubeTexIBL,float3 viewReflectDirection ){
               return texCUBElod(CubeTexIBL,float4(viewReflectDirection,skyBlur)).rgb;
                }
                float3 SBLSky2(sampler2D texIBL,float3 viewReflectDirection ){
               
                return tex2D(texIBL,viewReflectDirection.xy*0.5).rgb;
                }



            fixed4 frag (vertexout i) : SV_Target
            {  
              float uTime =  _Time.y;
               // float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                // objScale = 1.0/recipObjScale;
               
                float3 normalDir = normalize(i.normalDir);
              //  i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
               // i.screenPos.y *= _ProjectionParams.x;

               // float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
               //  float sceneZ = 
              //  float partZ = max(0,i.projPos.z - _ProjectionParams.g);



           ///////////////////////////////////     
                float node_2375 = uTime/20 ;

                float node_8229 = (wavesSpeed*node_2375*1.61803398875);

                float2 node_5986 = (i.uv*float2(objScale.r,objScale.b)*wavesScale);

                float2 node_8298 = (node_5986+node_8229*float2(1,-1));
//////////////////////////////////////
                float3 node_6123 = UnpackNormal(tex2D(normalTexture,TRANSFORM_TEX(node_8298, normalTexture)));
///////////////////////////////////////////////////////////

                float2 node_9836 = ((node_5986+float2(0.5,0.5))*0.8);
                float2 node_7614 = (node_9836+node_8229*float2(-1,1));
              ///////////////////////////////////////
                float3 node_7755 = UnpackNormal(tex2D(normalTexture,TRANSFORM_TEX(node_7614, normalTexture)));


                float node_60 = (node_6123.r+node_7755.r);
                float node_6221 = (node_6123.g+node_7755.g);


                float node_5594 = (node_8229*0.6);
                float node_6903 = 0.1;
                float2 node_7203 = ((node_6903*node_5986)+node_5594*float2(-1,1));

                float3 node_3810 = UnpackNormal(tex2D(normalTexture,TRANSFORM_TEX(node_7203, normalTexture)));

                float2 node_117 = ((node_6903*node_9836)+node_5594*float2(1,-1));
                float3 node_2963 = UnpackNormal(tex2D(normalTexture,TRANSFORM_TEX(node_117, normalTexture)));
               




                float node_1671 = 1.0;
                float3 node_6060 = lerp(float3(0,0,1),float3((float2(node_60,node_6221)+(float2((node_3810.r+node_2963.r),(node_3810.g+node_2963.g))*0.5)),node_1671),normalIntensity);
               
              //  float2 sceneUVs = node_6060.rg*_RefractionIntensity*0.1; //float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_6060.rg*_RefractionIntensity*0.1);
               
              



                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir,normalDir);



/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = node_6060;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
           
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(-LightDir0.xyz);
              
////// Lighting:


     
                float node_8267 = 1.0;
                float3 h = normalize( lightDirection + viewDirection);
              //  float4 sceneuv = float4(normalize(h.xz,100,skyBlur));
                /////////////////////
                float4 node_7838 = tex2Dbias(reflectionTex,float4(normalize(h).xz,100,skyBlur));




                float node_9204 = uTime/20 ;

                float node_827 = (foamSpeed*node_9204);
                /////////////////////////

                float2 node_2254 = (float2(objScale.r,objScale.b)*i.uv*foamScale);



/////////////////////////////////////////

                float2 node_9810 = (node_2254+node_827*float2(-1,1));
                float4 node_837 = tex2D(foamTexture,TRANSFORM_TEX(node_9810, foamTexture));

                float2 node_1263 = (((node_2254+float2(0.5,0.5))*0.8)+node_827*float2(1,-1));
                float4 node_2220 = tex2D(foamTexture,TRANSFORM_TEX(node_1263, foamTexture));
//////////////////////////////
             

         
                float3 mylight = DirectionalLight(normalDirection)[0];



                float3 node_6257 = (node_837.rgb*node_2220.rgb*mylight);
                float node_3527 = uTime /20;
                float node_4826 = (1.0 - length((i.uv*2.0+-1.0)));
                float2 node_3934 = float2(node_4826,node_4826);
                float2 node_3646 = (i.uv*(-1.0));
                float _InverseDirection = 1;
                float4 deepmap = tex2D(maskWavesDisplacement,i.uv);
                float4 maskWavesDisplacement_var = tex2D(maskWavesDisplacement,TRANSFORM_TEX(node_3646, maskWavesDisplacement));
                float2 node_2367 = ((lerp( i.uv, lerp( node_3934, (node_3934*dot(maskWavesDisplacement_var.rgb,float3(0.3,0.59,0.11))), _UseMask ), radialWaves )*float2(objScale.r,objScale.b)*wavesAmount*0.1)+(wavesDisplacementSpeed*node_3527*(_InverseDirection*2.0+-1.0))*float2(1,1));
                float4 wavesTexture_var = tex2D(wavesTexture,TRANSFORM_TEX(node_2367, wavesTexture));
               

                float node_8869 = dot(wavesTexture_var.rgb,float3(0.3,0.59,0.11));

               

                float noh = dot(normalDirection,h) ;

                float3 SPLMap = SBLSky2(spltex,viewReflectDirection);
                 //float3 SPLMap = SBLsky(_SPL, viewReflectDirection);

                  float4 sceneColor = tex2Dbias(reflectionTex, float4(normalize(viewDirection).xz,100,skyBlur))*reflectionmapColor;
                  sceneColor.rgb = SPLMap;
                 node_7838.rgb = SPLMap;
               //  return float4 (SPLMap,1);
                  
                float node_9630 = uTime/20 ;
                float node_9000 = (displacementSpeed*node_9630);

                float2 node_3654 = (float2(objScale.r,objScale.b)*displacementScale*i.uv*0.1);
                float2 node_8500 = (node_3654+node_9000*float2(-1,1));
                float4 node_2033 = tex2D(displacement,TRANSFORM_TEX(node_8500, displacement));


                float2 node_1716 = (((node_3654+float2(0.5,0.5))*0.75)+node_9000*float2(1,-1));
                float4 node_7404 = tex2D(displacement,TRANSFORM_TEX(node_1716, displacement));
                float3 mixdis = lerp(node_2033.rgb,node_7404.rgb,0.5);
                float _UseReflection = 1;
     /////sp highlight
                float3 sp = (spec*pow(max(0,noh),gloss*10)*mylight);
    /////fresnel
                float3 waterFresnel = pow(1.0-max(0,dot(normalDirection, viewDirection)),reflectionFresnel*5)*reflectionIntensity * fresnelColor;

                float3 finalColor = ((saturate((1.0-(1.0-saturate((1.0-(1.0-(pow(saturate((node_8267 + ( (saturate((deepmap)/waterDensity)
                 - shoreWaterOpacity) * (0.0 - node_8267) ) / ((waterColor.rgb*9.0+1.0) - shoreWaterOpacity))),fadeLevel)*sceneColor.rgb*
           ((1.0 - saturate((deepmap*(sceneZ-partZ))/(waterDensity*3.3)))*0.75+0.25)))*
                (1.0-(lerp( reflectionColor.rgb, node_7838.rgb, _UseReflection )*waterFresnel)))))
                *(1.0-((((1.0 - saturate((deepmap*(sceneZ-partZ))/shoreFoamDistance))*node_6257*shoreFoamIntensity)
                +(node_8869*node_6257*savesDisplacementFoamIntensity))+(node_6257*dot(mixdis,float3(0.3,0.59,0.11))*displacementFoamIntensity)))))
                *mylight)+sp);

                float t = saturate((deepmap*(sceneZ-partZ))/shoreLineOpacity);
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,t),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
}
