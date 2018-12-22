Shader "SkinTest03" {
    Properties {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" {}
        _BlurTex ("污迹", 2D) = "white" {}
        _BumpMap ("法线", 2D) = "bump" {}
        _BumpMapblur ("污迹法线", 2D) = "bump" {}
        _BumpMapMask ("BumpMapMask", 2D) = "white" {}
        _Specular ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularTex ("SpecularTex", 2D) = "white" {}
        _Gloss ("Gloss", Range(0.1, 1)) = 1
        _Curvature ("Curvature", Range(0.0, 1)) = 0.5
        _LookUp ("LookUp", 2D) = "white" {}
        _scattering ("Scattering" , vector ) = (0,0,0,0)
        _Translucency_var ("Translucency_var", 2D) = "white" {}
        _SubColor ("SubColor", Color) = (1, 1, 1, 1)
        _Fresnel ("Fresnel", Range(0.1, 1)) = 0.5
        _RimPower("RimPower", Range(0.1, 0.8)) = 0.5
        _RimColor("Rim Color", Color) = (1, 1, 1, 1)
        _RimTex("Rim (RGB)", 2D) = "white" {}

        _FrontRimPower("Front RimPower", Range(0.1, 0.8)) = 0.5
        _FrontRimColor("Front Rim Color", Color) = (1, 1, 1, 1)
        _FrontRimTex("Front Rim (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
        
        Pass { 
            Tags { "LightMode"="ForwardBase" }
        
            CGPROGRAM
            
            #pragma multi_compile_fwdbase   
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            fixed4 _Color;
            fixed4 _SubColor;
            sampler2D _MainTex;
            sampler2D _BlurTex;
            float4 _MainTex_ST;
            sampler2D _BumpMapMask;
            sampler2D _BumpMap;
            sampler2D _BumpMapblur;
            float4 _BumpMap_ST;
            fixed4 _Specular;
            float _Gloss;
            sampler2D _LookUp;
            float _Curvature;
            float4 _scattering;
            sampler2D _Translucency_var;
            float _Fresnel;
            float _RimPower;
            float4 _RimColor;
            uniform sampler2D _RimTex;

            float _FrontRimPower;
            float4 _FrontRimColor;
            uniform sampler2D _FrontRimTex;

            uniform sampler2D _SpecularTex;
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 TtoW0 : TEXCOORD1;  
                float4 TtoW1 : TEXCOORD2;  
                float4 TtoW2 : TEXCOORD3; 
                SHADOW_COORDS(4)
            };
            
            v2f vert(a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
             
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

                TANGENT_SPACE_ROTATION;
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;  
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);  
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);  
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w; 
                
                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);  
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);  
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);  
                
                TRANSFER_SHADOW(o);
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
                fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                
                fixed3 worldNormal = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                worldNormal = normalize(half3(dot(i.TtoW0.xyz, worldNormal), dot(i.TtoW1.xyz, worldNormal), dot(i.TtoW2.xyz, worldNormal)));
                fixed3 bump1 = UnpackNormal(tex2D(_BumpMapblur, i.uv.zw));
                bump1 = normalize(half3(dot(i.TtoW0.xyz, bump1), dot(i.TtoW1.xyz, bump1), dot(i.TtoW2.xyz, bump1)));

                fixed3 bumpMark = tex2D(_BumpMapMask, i.uv.xy);
                float3 rs = worldNormal;
                float3 rn = lerp(worldNormal,bump1,bumpMark.r);
                float3 gn = lerp(worldNormal,bump1,bumpMark.g);
                float3 bn = lerp(worldNormal,bump1,bumpMark.b);
                float3 nlingh = float3(dot(rn, lightDir),dot(gn, lightDir),dot(bn, lightDir));

                fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;

                /*Blur*/
                albedo += tex2D(_BlurTex, i.uv.xy) * 64;
                albedo += tex2D(_BlurTex, i.uv.xy) * 32;
                albedo += tex2D(_BlurTex, i.uv.xy) * 16;
                albedo += tex2D(_BlurTex, i.uv.xy) * 8;
                albedo += tex2D(_BlurTex, i.uv.xy) * 4;
                albedo += tex2D(_BlurTex, i.uv.xy) * 2;
                albedo /= 256;
                albedo*= _Color.rgb;
                
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                
                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, nlingh/*dot(worldNormal, lightDir)*/);
                
                fixed3 halfDir = normalize(lightDir + viewDir);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

                /*Specular*/
                float3 SpecularTex =  tex2D(_SpecularTex,i.uv.xy);

                half nl = saturate(dot(worldNormal, lightDir));
                 half3 halfDir1 = normalize( lightDir + viewDir);
                 half nh = BlinnTerm(worldNormal, halfDir1);
                 half nv = saturate(dot(worldNormal, viewDir));
                 half2 rlPow4AndFresnelTerm = Pow4 (half2(nh, 1-nv)); 
                 half rlPow4 = rlPow4AndFresnelTerm.x;
                 half fresnelTerm = rlPow4AndFresnelTerm.y;
                 half grazingTerm = saturate(_Gloss );
                 half LUT_RANGE = 16.0; 
                 half specular1 = tex2D(unity_NHxRoughness, half2(rlPow4, _Gloss)).UNITY_ATTEN_CHANNEL * LUT_RANGE;
                 half3 finalspecular = specular1 * _Specular * SpecularTex ;          // + o.diffColor*nl             
                 finalspecular*= _LightColor0.rgb ;
                 half3 c = diffuse ;
                 c *=  lerp (_Specular, 1, fresnelTerm *  _Fresnel);
                 finalspecular += c;


                 /**/
                float NdotLBlurredUnclamped = max(-0.5,dot(worldNormal, lightDir));  
                half3 diffuseLookUp = tex2Dlod(_LookUp, float4((NdotLBlurredUnclamped * 0.4 + 0.5), _Curvature, 0, 0));  

                float attenuation = LIGHT_ATTENUATION(i);  
                float3 attenColor = attenuation * _LightColor0.xyz;  

                fixed3 Translucency_var = tex2D(_Translucency_var, i.uv.xy).rgb ;
                
                half3 transLightDir = lightDir + worldNormal * _scattering.y;  
                half transDot = dot(-transLightDir, viewDir);  
                transDot = exp2((saturate(transDot)-1) * _scattering.x) * Translucency_var.g * _scattering.z;  
                half3 lightScattering = transDot * _SubColor /** attenColor*/;  

                //透光。。
                //背面透光就好了，为什么还要正面再透光.
                /*rim*/
                float3 rim = (1 - dot(viewDir, worldNormal))*_RimPower * _RimColor *tex2D(_RimTex, i.uv.xy);
                float3 frontrim = (dot(viewDir, worldNormal))*_FrontRimPower * _FrontRimColor *tex2D(_FrontRimTex, i.uv.xy);

                UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

                return fixed4(ambient + diffuse /* + specular*/ +  finalspecular + diffuseLookUp  * albedo  + lightScattering + rim + frontrim, 1.0);
                //return fixed4 (specular1 * _Specular.rgb+diffuse + diffuseLookUp * albedo + lightScattering,1.0);

            }
            
            ENDCG
        }
        }
}