Shader "lin/ReflectShadowMap"
{
    Properties
    {
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        _Specular ("Specular Color", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(8, 256)) = 8
        _SpecularStrength("Specular",Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
			#include "Shadow.cginc"
			#include "ReflectShadowMap.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
				
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 ambient : TEXCOORD2;
                float4 diff : COLOR0; // diffuse lighting color
				float2 lvuv : TEXCOORD4;
				float dep : TEXCOORD5;
 
            };

            fixed4 _Color;
            fixed4 _Specular;
            float _Gloss;
            float _SpecularStrength;
                        
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                o.worldNormal = worldNormal;
                float4 wpos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = wpos;

				float4 lv_pos =   mul(_WorldToLight, wpos);
				o.dep =  lv_pos.z / lv_pos.w;
                o.ambient = ShadeSH9(half4(worldNormal,1)) *_Color ;
               
                return o;
            }
            float4 GetWorldPositionFromDepth( float2 uv_depth )
			{    
			        float depth =  DecodeFloatRGBA( tex2D(_ShadowTex,  uv_depth) )  ; 
			        float2 uv =  uv_depth;
			        uv.y = 1- uv.y;
			        float4 H = float4(uv.xy*2.0-1.0, depth, 1.0);
			        float4 D = mul(_WorldToLightInv,H);
			        return D/D.w;
			}
            fixed4 frag (v2f i) : SV_Target
            {
            	fixed3 ambient = i.ambient;
                // specular
                float3 worldNormal = normalize(i.worldNormal);
                //float3 lightDir = UnityWorldSpaceLightDir(i.worldPos);
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
       
                float3 viewDir = UnityWorldSpaceViewDir(i.worldPos);
                viewDir = normalize(viewDir);
                float3 halfDir = normalize(lightDir + viewDir);


                float d = max(0, dot(halfDir, worldNormal));
                float3 spec = _LightColor0.rgb * _Specular.rgb * pow(d, _Gloss) * _SpecularStrength;

                float nl = max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));
                //fixed3 lambert = 0.5 *  nl + 0.5;
           
                // diffuse
                //float3 diff = _LightColor0.rgb * _Color.rgb * lambert ;
                float3 diff = _LightColor0.rgb * _Color.rgb * nl ;

                //shadow Map
				float dep = i.dep;
                float s = ShadowMap(dep,i.lvuv,0.5);

                // Indirect Light
				
				
				fixed3 amblentColor = fixed3(0,0,0);
				 for(float j = 0; j < 8; j++) 
				 {
					for(float  k = 0 ; k < 8 ; k++)
					{
						float2 uv0 = float2(j/8,k/8);
						float4 _wn = tex2D(_RSM_NORMAL,  uv0) * 2 - float4(1,1,1,0) ;   //世界法线.
						_wn = normalize(_wn);

						float3 _col = tex2D(_RSM_LIGHT,  uv0);//漫反光点颜色.

						float4 word  = GetWorldPositionFromDepth(uv0);

						//这部放到前面去了.
						//反光点漫射一次. 一般还会乘多一个材质反射强度.
						//这步移到MRT去了.
						//float nl = max(0,dot(-_G_WorldSpaceLightDir,_wn));
						//_col = _col*nl;

						//反射点到当前点的方向.
						float3 _dir2 =  i.worldPos  -  word ;
						float dis = length(_dir2);

						_dir2 =  normalize(_dir2) ;
						//当前点的间接光照漫反射. 
						float nl2 = max(0,dot(-_dir2,i.worldNormal)) ;
						_col = _col*nl2  / dis ;

						amblentColor += _col   ;

					}
				 }

                float3 c =  ambient +  ( spec + diff )*s  +  float4(amblentColor,1) * _InvPower; ;

           
                return fixed4(c, 1);
            }
            ENDCG
        }
    }
}