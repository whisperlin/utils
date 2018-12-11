// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/DiffuseLightMap4DirTree"
{
Properties {
     _MainTex ("Base (RGB)", 2D) = "white" {}
     _LightTex ("Base (RGB)", 2D) = "white" {}
    
     _rotDelta  ("_rot",Range(0,1))= 0
     _rotation  ("_rotation",Vector)= (0,0,0.5,0)
 
      _Alphatest ("Alpha test", Range(0, 3)) = 1

      	leafWindPower("leafWindPower ", Float) = 1.5
		leafWindDir("leafWindDir" , Vector) = (1,0.5,0.5,0)
		leafWindAtt("leafWindAtt ", Float) = 0.03
		trunkWindPower("trunkWindPower ", Float) = 0.5
		trunkWindAtt("trunkWindAtt ", Float) = 1

		LodInv("LodInv ", Range(0.05, 0.3 )) =  0.09
		LodMax("LodMax ", Range(3, 9 )) =  4
}

SubShader {
    Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
    LOD 100
    Blend SrcAlpha OneMinusSrcAlpha
    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"


            fixed leafWindPower;
			fixed4 leafWindDir;
			fixed leafWindAtt;
			fixed trunkWindPower;
			fixed trunkWindAtt;

             #define PI 3.14
			fixed4 TreeWind(fixed4 vertexColor, fixed3 normaldir, fixed leafWindPower, fixed4 leafWindDir, fixed leafWindAtt, fixed trunkWindPower, fixed trunkWindAtt) {
				fixed a = (vertexColor.r * PI + _Time.y*leafWindPower);
				fixed b = sin(a * 3)*0.2 + sin(a);
				fixed k = cos(a * 5);
				fixed d = b - k;
				fixed4 e = vertexColor.r * d *  (normalize(leafWindDir + normaldir.xyzz)) * leafWindAtt;

				fixed f = _Time.y * trunkWindPower;
				fixed g = sin(f) *  trunkWindAtt * vertexColor.r;
				fixed h = cos(f) * 0.5 * trunkWindAtt * vertexColor.r;
				fixed3 i = fixed3(g, 0, h);
				fixed4 j = e + i.xyzz;
				return j;
			}

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
				 float2 texcoord1 : TEXCOORD1;
                 //float2 texcoord3 : TEXCOORD2;
                 float3 normal :NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float NdotH :TEXCOORD3;
                float3 viewReflectDirection :TEXCOORD4;
                float4 posWorld : TEXCOORD5;
                UNITY_FOG_COORDS(6)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
 
            uniform float _Alphatest;

            sampler2D _LightTex;
            float4 _LightTex_ST;

            float4 _rotation;
            float _rotDelta;
            uniform fixed3 DirectionLightDir0;
            uniform fixed3 DirectionLightColor0;

            uniform float LodInv;
            uniform float LodMax;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float4 windv = TreeWind(v.color, v.normal, leafWindPower, leafWindDir, leafWindAtt, trunkWindPower, trunkWindAtt);

                o.vertex = UnityObjectToClipPos(v.vertex + windv);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _MainTex) / 2;
                o.texcoord2 = o.texcoord1 ;
                o.texcoord1.x += _rotation.x;
                o.texcoord1.y += _rotation.y;

                o.texcoord2.x += _rotation.z;
                o.texcoord2.y += _rotation.w;

                float3 normalDirection = UnityObjectToWorldNormal(v.normal);
                float3 worldPos =  mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - worldPos);
				float3 lightDirection = normalize(DirectionLightDir0);
 
                float3 halfDirection = normalize(viewDirection + lightDirection);
                o.NdotH = saturate(dot( normalDirection, halfDirection));
 
                o. viewReflectDirection = reflect( -viewDirection, normalDirection );
                //_rotDelta
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            	float3 viewDist = _WorldSpaceCameraPos.xyz - i.posWorld.xyz;
                //fixed4 col = tex2D(_MainTex, i.texcoord);
                fixed4 col = tex2Dlod(_MainTex, float4(i.texcoord,0 ,(int)min(LodMax ,length(viewDist)*LodInv )   ));

                clip((col.a*_Alphatest) - 0.5);
                fixed3 light = tex2D(_LightTex, i.texcoord1).rgb;
                fixed3 light2 = tex2D(_LightTex, i.texcoord2).rgb;
                light.rgb = lerp( light.rgb,light2,_rotDelta);
                light =  (light - 0.5)*2;

                col.rgb =  col.rgb +  light.rgb  ;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
        ENDCG
    }
}

}

