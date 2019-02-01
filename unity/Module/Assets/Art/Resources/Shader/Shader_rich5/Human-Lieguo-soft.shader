Shader "Rolan/Human-Lieguo-soft" {
    Properties {
        _Color("漫射颜色", Color) = (1, 1, 1, 1) _MainTex("漫射贴图", 2D) = "white" {}
        _BumpMap("法线贴图", 2D) = "bump" {}
        _NormalIntensity("法线强度", Range(0, 3)) = 1 
        _Metallic("金属贴图", 2D) = "white" {}
        _MetallicPower("PBR金属强度", Range(0, 1)) = 0 
        _GlossPower("PBR高光强度", Range(0, 1)) = 1 
        IBL_Blur("环境球模糊强度", Range(0, 10)) = 1 
        [Toggle(_DISABLE_IBL_DIFFUSE)] _DISABLE_IBL_DIFFUSE("禁用环境球", Int) = 0
        _IBL_Diffuse("环境球", Cube) = "_Skybox" {}
        IBL_Intensity("环境球强度", Range(0, 10)) = 1 
        _AmbientLight("环境光", Color) = (0.5, 0.5, 0.5, 1) 
        SBL_Intensity("反射球强度", Range(0, 10)) = 0 
        _SkinColor("皮肤颜色", Color) = (1, 1, 1, 1) 
        _SkinIntensity("皮肤强度", Range(0, 20)) = 0 
        _SkinMap("皮肤贴图", 2D) = "white" {}
        _EmssionMap("自发光", 2D) = "white" {}
        _EmissionIntensity("自发光强度", Range(0, 2)) = 0
        _BlinnPhongSP("高光强度", Range(0, 10)) = 1 
        _BP_Gloss("高光范围", Range(0, 1)) = 0 
        AOpower("AO图金属图B通道",Range(0,1))=0
        GlobeLight("全局光强度", Range(0, 1)) = 1
        _Gama("Gama",Range(-1, 1)) = 0

        _Cutoff("透明", Range(0, 1)) = 0.5
         pointatten1("pointatten", Range(0, 100)) = 0 
        [Toggle(_ENABLE_CUT)] _ENABLE_CUT("透明提除", Int) = 1

   
    }
    SubShader {   
        Pass {
            Name "FORWARD"
            Tags {"LightMode" = "ForwardBase"  "Queue"="AlphaTest" "RenderType"="TreeTransparentCutout"}
            ZWrite on
         	Blend SrcAlpha OneMinusSrcAlpha

            Stencil {
                Ref 128
                Pass Replace
            }
            Cull Off          
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
           
            #include "../CGIncludes/RolantinCG.cginc"
            //  #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON

           
            #pragma shader_feature _ENABLE_CUT
            #pragma shader_feature _DISABLE_IBL_DIFFUSE

            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal
        //    #pragma shader_feature USE_LIGHTMASK_OFF USE_COMBINE_CHANNEL_ON USE_SPLIT_CHANNEL_ON
            #pragma target 3.0 
            uniform float4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap;
            uniform float4 _BumpMap_ST;
            uniform samplerCUBE _IBL_Diffuse;
            uniform sampler2D _Metallic;
            uniform float4 _Metallic_ST;
            uniform samplerCUBE _SBL;
            uniform sampler2D _EmssionMap;
            uniform float4 _EmssionMap_ST;
            uniform float _EmissionIntensity;
            uniform float4 _IBL_Color;
            uniform float AOpower;

            float _Cutoff;
            float4 _emissioncolor;
            float4 _SkinColor;
            float GlobeLight;
            float _emission;
            uniform float4 _Skin;
            uniform float _SkinIntensity;
            uniform sampler2D _SkinMap;
            uniform float4 _SkinMap_ST;

            uniform float _BlinnPhongSP;

            uniform fixed3 PointLightPosition3;
			uniform fixed3 PointLightPosition4;



uniform fixed3 LightColor3;
uniform fixed3 LightColor4;


uniform fixed LightIntensity3;
uniform fixed LightIntensity4;


      

         
            struct VertexInput {
                float4 vertex: POSITION;
                float3 normal: NORMAL;
                float4 tangent: TANGENT;
                float2 texcoord0: TEXCOORD0;
            };

            struct VertexOutput {
                float4 pos: SV_POSITION;
                float2 uv0: TEXCOORD0;
                float4 posWorld: TEXCOORD1;
                float3 normalDir: TEXCOORD2;
                float3 tangentDir: TEXCOORD3;
                float3 bitangentDir: TEXCOORD4;
            };

            VertexOutput vert(VertexInput v) {
                VertexOutput o = (VertexOutput) 0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            } 
            float4 frag(VertexOutput i) : COLOR {
             
                posWorld = i.posWorld.xyz;

                float3x3 tangentTransform = float3x3(i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
///////Mapping                
                float3 _BumpMap_var = UnpackNormal (tex2D(_BumpMap, TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 _Emssion_var =tex2D(_EmssionMap, TRANSFORM_TEX(i.uv0, _EmssionMap));
                float3 Emission = _Emssion_var* _EmissionIntensity;
                float3 normalLocal = NormalIntensity(_BumpMap_var);
                float3 normalDirection = normalize(mul(normalLocal, tangentTransform)); // Perturbed normals
                float3 BacknormalDirection = -normalDirection; // Perturbed normals
                float3 viewReflectDirection = reflect( - viewDirection, normalDirection);
                float4 _MainTex_var = ColorSpace (tex2D(_MainTex, TRANSFORM_TEX(i.uv0, _MainTex)));
                
                //clip(_MainTex_var.a - 0.5 *_Cutoff );

////// Lighting:  

              
                float3 pointlight = PointLight(normalDirection,PointLightPosition3 ,LightRange3) * LightColor3* LightIntensity3;

                float3 pointlight2 = PointLight(normalDirection,PointLightPosition4,LightRange4) * LightColor4 * LightIntensity4;
               // return  float4  (pointlight,1);
              
                float3 lightDirection = normalize(LightDir1);
                float3 halfDirection = normalize(viewDirection + lightDirection);
                float3 FullLight = DirectionalLight(normalDirection)[0] + DirectionalLight(normalDirection)[1]  +  DirectionalLight(normalDirection)[2];
               //  float3 FullLight = DirectionalLight(normalDirection)[0] + DirectionalLight(normalDirection)[1] + pointlight +  DirectionalLight(normalDirection)[2];
               // float3 SpLight =PointLight(normalDirection ,PointLightPosition);
               // float3 SkinLight = (DirectionalLight(-normalDirection)[1] ) + PointLight(-normalDirection ,PointLightPosition);

                ///////// Gloss:
                float4 _Metallic_var = tex2D(_Metallic, TRANSFORM_TEX(i.uv0, _Metallic));
                ////// Specular:
                float3 specularColor = _Metallic_var;
                float3 diffuseColor = (_MainTex_var.rgb * _Color.rgb); // Need this for specular when using metallic
                float3 MetallicDiff;
              
//HightLigtht
                //float3 indirectSpecular = SBL(_IBL_Diffuse, normalDirection , _Metallic_var.r);

                //HightLigtht
                #if defined(_DISABLE_IBL_DIFFUSE)
            
                    float3 indirectSpecular;
                    float3 IBLColor;
                    SBLAndIBLSample(_IBL_Color, normalDirection , _Metallic_var.r,indirectSpecular,IBLColor);

                #else
                     float3 indirectSpecular;
                    float3 IBLColor;
                    SBLAndIBL(_IBL_Diffuse, normalDirection , _Metallic_var.r,indirectSpecular,IBLColor);
                #endif

                float3 PbrSpecular = PBR_SP(normalDirection, normalize(-LightDir0), DirectionalLight(normalDirection)[0], _Metallic_var, diffuseColor, MetallicDiff);
              //  float3 HighLight = (Phong(LightDir2, normalDirection,_Metallic_var.r,_Metallic_var.a) * SpLight* LightIntensity2 * LightColor2)  * diffuseColor *  _BlinnPhongSP;
                 float3 HighLight = (Phong(LightDir2, normalDirection,_Metallic_var.r,_Metallic_var.a) )  * diffuseColor *  _BlinnPhongSP;
                float3 FinalSP = (PbrSpecular + indirectSpecular + HighLight);
//return  float4 (pointlight,1);
                /////// Diffuse:
                //   NdotL = dot( normalDirection, LightDir1 );
                float4 _SkinMap_var = tex2D(_SkinMap, TRANSFORM_TEX(i.uv0, _SkinMap));
                //float3 IBLColor = IBL(_IBL_Diffuse, normalDirection);
              //  float3 Skin = _SkinMap_var.rgb * ( (DirectionalLight(-normalDirection)[1] ) +PointLight(-normalDirection ,LightDir2))  * _SkinIntensity * _SkinColor;
                    float3 Skin = _SkinMap_var.rgb *  (DirectionalLight(-normalDirection)[1] ) * _SkinIntensity * _SkinColor;
                // Light wrapping
                //  float3 NdotLWrap = NdotL * ( 1.0 - Skin );
                //  float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap +Skin );
                //  NdotL = max(0.0,dot( normalDirection, lightDirection ));
                //  half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                //  float nlPow5 = Pow5(1-NdotLWrap);
                //  float nvPow5 = Pow5(1-NdotV);
                //
                //pointlight
                float3 p1 = diffuseColor*(pointlight+pointlight2);
              
                //return float4(p1,1);
                float3 directDiffuse = (  Skin + FullLight+p1    +  IBLColor+ Emission)* MetallicDiff* lerp (float3(1,1,1),_Metallic_var.b,AOpower) ;
                float3 pbl = (directDiffuse + FinalSP ) * GlobeLight;

                float b0 =  _Cutoff  ;
 
                float a0 = smoothstep(0,b0,_MainTex_var.a);
              	//float a0 = step(b0,_MainTex_var.a);
                return fixed4(pbl, a0);
            }
            ENDCG

        }




    }
    // CustomEditor "CharShaderEditor"
    FallBack "Diffuse"

}