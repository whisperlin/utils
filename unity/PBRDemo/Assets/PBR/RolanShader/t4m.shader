// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:2,spmd:1,trmd:0,grmd:1,uamb:False,mssp:True,bkdf:True,hqlp:True,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32545,y:32172,varname:node_3138,prsc:2|diff-6582-OUT,normal-3020-OUT,custl-6582-OUT;n:type:ShaderForge.SFN_Tex2d,id:150,x:30379,y:31589,ptovrint:False,ptlb:r,ptin:_r,varname:node_459,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4370,x:30561,y:32942,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_8532,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9817,x:30398,y:31939,ptovrint:False,ptlb:g,ptin:_g,varname:_g_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6803,x:30349,y:32268,ptovrint:False,ptlb:b,ptin:_b,varname:node_7544,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2541,x:30355,y:32699,ptovrint:False,ptlb:a,ptin:_a,varname:_a_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4708,x:31153,y:32398,ptovrint:False,ptlb:r_N,ptin:_r_N,varname:_r_N_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:5011,x:31178,y:32579,ptovrint:False,ptlb:g_N,ptin:_g_N,varname:_g_N_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:6475,x:31200,y:32814,ptovrint:False,ptlb:b_N,ptin:_b_N,varname:_b_N_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:8344,x:31211,y:33055,ptovrint:False,ptlb:a_N,ptin:_a_N,varname:_a_N_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:7201,x:30277,y:31793,ptovrint:False,ptlb:ri,ptin:_ri,varname:node_7201,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:1,max:2;n:type:ShaderForge.SFN_Divide,id:4407,x:30764,y:31727,varname:node_4407,prsc:2|A-150-RGB,B-7201-OUT;n:type:ShaderForge.SFN_Divide,id:2663,x:30784,y:32021,varname:node_2663,prsc:2|A-9817-RGB,B-5207-OUT;n:type:ShaderForge.SFN_Divide,id:942,x:30807,y:32317,varname:node_942,prsc:2|A-6803-RGB,B-8473-OUT;n:type:ShaderForge.SFN_Divide,id:7775,x:30757,y:32665,varname:node_7775,prsc:2|A-2541-RGB,B-7886-OUT;n:type:ShaderForge.SFN_Slider,id:7886,x:30309,y:32625,ptovrint:False,ptlb:ai,ptin:_ai,varname:_ri_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:5207,x:30270,y:32127,ptovrint:False,ptlb:gi,ptin:_gi,varname:node_5207,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:8473,x:30270,y:32445,ptovrint:False,ptlb:bi,ptin:_bi,varname:node_8473,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:1,max:2;n:type:ShaderForge.SFN_Multiply,id:4933,x:31095,y:31621,varname:node_4933,prsc:2|A-4407-OUT,B-4370-R;n:type:ShaderForge.SFN_Multiply,id:5860,x:31095,y:31776,varname:node_5860,prsc:2|A-2663-OUT,B-4370-G;n:type:ShaderForge.SFN_Multiply,id:7391,x:31095,y:31933,varname:node_7391,prsc:2|A-942-OUT,B-4370-B;n:type:ShaderForge.SFN_Multiply,id:5501,x:31095,y:32132,varname:node_5501,prsc:2|A-7775-OUT,B-4370-A;n:type:ShaderForge.SFN_Add,id:5312,x:31501,y:31835,varname:node_5312,prsc:2|A-4933-OUT,B-5860-OUT,C-7391-OUT,D-5501-OUT;n:type:ShaderForge.SFN_Multiply,id:3975,x:31629,y:32386,varname:node_3975,prsc:2|A-5011-RGB,B-4370-G;n:type:ShaderForge.SFN_Multiply,id:8257,x:31640,y:32556,varname:node_8257,prsc:2|A-6475-RGB,B-4370-B;n:type:ShaderForge.SFN_Multiply,id:1423,x:31655,y:32732,varname:node_1423,prsc:2|A-8344-RGB,B-4370-A;n:type:ShaderForge.SFN_Multiply,id:1016,x:31641,y:32174,varname:node_1016,prsc:2|A-4708-RGB,B-4370-R;n:type:ShaderForge.SFN_Add,id:3020,x:31987,y:32481,varname:node_3020,prsc:2|A-1016-OUT,B-3975-OUT,C-8257-OUT,D-1423-OUT;n:type:ShaderForge.SFN_Multiply,id:6582,x:32088,y:32139,varname:node_6582,prsc:2|A-3090-OUT,B-5245-OUT,C-4281-RGB,D-5312-OUT;n:type:ShaderForge.SFN_LightColor,id:4281,x:31793,y:31923,varname:node_4281,prsc:2;n:type:ShaderForge.SFN_LightAttenuation,id:5245,x:31873,y:32056,varname:node_5245,prsc:2;n:type:ShaderForge.SFN_LightVector,id:4831,x:32045,y:31833,varname:node_4831,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9127,x:32069,y:31954,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:3090,x:32241,y:31905,varname:node_3090,prsc:2,dt:0|A-4831-OUT,B-9127-OUT;proporder:150-4708-7201-9817-5011-5207-6803-6475-8473-2541-8344-7886-4370;pass:END;sub:END;*/

Shader "Rolan/t4m" {
    Properties {


        AmbientColor("AmbientColor" ,Color ) = (1,1,1,1)
        _r ("r", 2D) = "white" {}
        _r_N ("r_N", 2D) = "bump" {}
        _ri ("ri", Range(0.5, 2)) = 1
        _g ("g", 2D) = "white" {}
        _g_N ("g_N", 2D) = "bump" {}
        _gi ("gi", Range(0.5, 2)) = 1
        _b ("b", 2D) = "white" {}
        _b_N ("b_N", 2D) = "bump" {}
        _bi ("bi", Range(0.5, 2)) = 1
        _a ("a", 2D) = "white" {}
        _a_N ("a_N", 2D) = "bump" {}
        _ai ("ai", Range(0.5, 2)) = 1
        _F_N ("F_N", 2D) = "bump" {}
        _mask ("T4Mmask", 2D) = "white" {}
        _blenderColor("季节混合颜色" , Color) = (1,1,1,1)
        _blenderIntensity("季节混合强度" , Range(0,1)) = 0
        _snowColor("雪颜色" , Color ) = (1,1,1,1)
        _snowmap("雪漫射" , 2D) = "white" {}
        _snowNormal("雪法线" , 2D) = "white" {}
        _Noise("雪噪点图" , 2D) = "white" {}
        _Alpha ("雪透明", Range(0, 2)) = 1
        _MetallicPower("雪高光强度", Range(0, 10)) = 0
        _BP_Gloss("雪高光范围", Range(0, 10)) = 0.37 
        _ShadowIntensity("阴影颜色" , Range(0,1)) =  0.5
      
     

    }
    SubShader {
        Tags {"RenderType"="Opaque" "Queue"="Geometry-100"}
        Pass {
        Tags {"LightMode"="ForwardBase"}

	
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           // #define UNITY_PASS_FORWARDBASE 
           #include "../CGIncludes/RolantinCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows         
            #pragma multi_compile_fog        
            #pragma target 3.0
            uniform sampler2D _r; uniform float4 _r_ST;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform sampler2D _g; uniform float4 _g_ST;
            uniform sampler2D _b; uniform float4 _b_ST;
            uniform sampler2D _a; uniform float4 _a_ST;
            uniform sampler2D _r_N; uniform float4 _r_N_ST;
            uniform sampler2D _g_N; uniform float4 _g_N_ST;
            uniform sampler2D _b_N; uniform float4 _b_N_ST;
            uniform sampler2D _a_N; uniform float4 _a_N_ST;
            uniform float _ri;
            uniform float _ai;
            uniform float _gi;
            uniform float _bi;
            uniform float4 _blenderColor;
            uniform float _blenderIntensity;
            uniform sampler2D _F_N; uniform float4 _F_N_ST;
            float _ShadowIntensity;
            float atten;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                SHADOW_COORDS(2)            
                UNITY_FOG_COORDS(9)
               
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
              
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);           
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                UNITY_LIGHT_ATTENUATION(atten, i, i.posWorld.xyz);             
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _r_N_var = UnpackNormal(tex2D(_r_N,TRANSFORM_TEX(i.uv0, _r_N)));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float3 _g_N_var = UnpackNormal(tex2D(_g_N,TRANSFORM_TEX(i.uv0, _g_N)));
                float3 _b_N_var = UnpackNormal(tex2D(_b_N,TRANSFORM_TEX(i.uv0, _b_N)));
                float3 _a_N_var = UnpackNormal(tex2D(_a_N,TRANSFORM_TEX(i.uv0, _a_N)));
                float3 _F_N_var = UnpackNormal(tex2D(_F_N,TRANSFORM_TEX(i.uv0, _F_N)));
                float3 normalLocal = ((_r_N_var.rgb*_mask_var.r)+(_g_N_var.rgb*_mask_var.g)+(_b_N_var.rgb*_mask_var.b)+(_a_N_var.rgb*_mask_var.a)) +  _F_N_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
             
              
                float3 Light =  DirectionalLight(normalDirection)[0];              
                float4 _r_var = tex2D(_r,TRANSFORM_TEX(i.uv0, _r));
                float4 _g_var = tex2D(_g,TRANSFORM_TEX(i.uv0, _g));
                float4 _b_var = tex2D(_b,TRANSFORM_TEX(i.uv0, _b));
                float4 _a_var = tex2D(_a,TRANSFORM_TEX(i.uv0, _a));
                float3 diff =(((_r_var.rgb/_ri)*_mask_var.r)+((_g_var.rgb/_gi)*_mask_var.g)+((_b_var.rgb/_bi)*_mask_var.b)+((_a_var.rgb/_ai)*_mask_var.a));
              //  float3 diffgama = ColorSpace(diff);
              //  float gamacolor = _Gama = 1 * _blenderColor.a;
              
/// Final Color:
                float3 finalColor = AmbientColor * Light * lerp( atten ,float3(1,1,1),_ShadowIntensity)  * lerp (_blenderColor ,diff,1-_blenderIntensity);
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);

                return finalRGBA;
            }
            ENDCG

        }

       Pass {
           Tags {
            "LightMode"="ForwardBase"
            "Queue"="Transparent"
            "RenderType"="Transparent"

            }
           // Blend One One     
            Blend SrcAlpha OneMinusSrcAlpha                 
            CGPROGRAM
            #include "AutoLight.cginc"
             #include "../CGIncludes/RolantinCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
          
           // #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            
            #pragma target 3.0         
            uniform sampler2D _snowmap;  uniform float4 _snowmap_ST;
            uniform sampler2D _Noise;  uniform float4 _Noise_ST; 
            uniform sampler2D _snowNormal; uniform float4 _snowNormal_ST;
            uniform sampler2D _F_N; uniform float4 _F_N_ST;
            uniform float _Alpha;
         
               uniform float4 _snowColor;
               fixed atten;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD6;
                float3 bitangentDir : TEXCOORD5;
                UNITY_FOG_COORDS(9)
                SHADOW_COORDS(2)  
          
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);               
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);           

                return o;
            }
            float4 frag(VertexOutput i) : COLOR {         
                posWorld = i.posWorld.xyz;
                UNITY_LIGHT_ATTENUATION(atten, i, i.posWorld.xyz);    
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float _noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));

                float3 _snowmap_var = tex2D(_snowmap,TRANSFORM_TEX(i.uv0, _snowmap));

                float3 _snowglob_var = tex2D(_F_N,TRANSFORM_TEX(i.uv0,_F_N));

                float3 _snowNormal_var =  UnpackNormal(tex2D(_snowNormal,TRANSFORM_TEX(i.uv0, _snowNormal)) +  tex2D(_F_N,TRANSFORM_TEX(i.uv0,_F_N))); 

               // clip(_noise_var -1 * _Alpha);               
                float3 normalDirection = normalize(mul( _snowNormal_var, tangentTransform )); // Perturbed normals 
                float3 Light = DirectionalLight(normalDirection)[0];
                float3 PhongSP = Phong(LightDir0,normalDirection,_MetallicPower,1) ;         
                float3 diffuseColor = (_snowmap_var * (_snowColor*_noise_var)* AmbientColor + PhongSP) * Light * atten ;
               // float3 attenpower = lerp (diffuseColor ,atten,0.5 ) * Light * _snowColor * AmbientColor;
                fixed4 finalRGBA = fixed4(diffuseColor,_Alpha *_noise_var );    
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
              //  return float4(diffuseColor,1);           
                return finalRGBA;
            }
            ENDCG
        }
        
    }
    FallBack "Diffuse"
}









