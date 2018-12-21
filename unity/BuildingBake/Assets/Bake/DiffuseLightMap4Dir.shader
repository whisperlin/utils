// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/DiffuseLightMap4Dir"
{
Properties {
     _MainTex ("Base (RGB)", 2D) = "white" {}
      _Metallic ("_Metallic", 2D) = "white" {}
     _LightTex ("Base (RGB)", 2D) = "white" {}
    
     //_rotDelta  ("_rot",Range(0,1))= 0
    // _rotation  ("_rotation",Vector)= (0,0,0.5,0)
     _SpecIBL ("SpecIBL", Cube) = "_Skybox" {}
      _SpecIBLPower ("SpecIBL Power", Range(0, 10)) =1
      _Grass ("_Grass", Range(1, 256)) =2
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
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
                float rotDelta  :TEXCOORD5;
                UNITY_FOG_COORDS(6)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Metallic;
            float4 _Metallic_ST;


             sampler2D _LightTex;
            float4 _LightTex_ST;


            uniform samplerCUBE _SpecIBL;
            uniform float _SpecIBLPower;

           
            float _Grass;
            uniform fixed3 DirectionLightDir0;
            uniform fixed3 DirectionLightColor0;
            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float3  forward = normalize(float3(unity_ObjectToWorld._m02,0,unity_ObjectToWorld._m22));


                float4 _rotation;
            	 
                float delta = 0;

 				
				//if (forward.z >= 0 && forward.x >= 0) {
					//_rotation =  float4(0,  0 ,  0.5, 0);
					//delta = dot(float3 (1, 0, 0), forward); 
				//} else if (forward.z <= 0 && forward.x >= 0) {
					//_rotation =  float4(0, 0.5, 0.5, 0);
					//delta = dot ( float3 (1, 0, 0), forward); 
				//} else if (forward.z <= 0 && forward.x <= 0) {
		 			//_rotation =  float4(0, 0.5,0.5, 0.5);
					//delta = dot ( float3 (-1, 0, 0), forward); 
				//} 
				//else //  if (forward.z >= 0 && forward.x <= 0)
				//{
					//_rotation =  float4(0, 0 ,0.5, 0.5);
					//delta = dot ( float3 (-1, 0, 0), forward); 
				//}

				//这两步等价于上面注释部分.
				_rotation =  float4(0, step(forward.z,0)*0.5 ,  0.5 ,step(forward.x,0)*0.5  );
				delta = dot ( float3 (-1+step(0,forward.x)*2, 0, 0), forward); 


				o.rotDelta = delta;
                o.vertex = UnityObjectToClipPos(v.vertex);
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
                fixed4 col = tex2D(_MainTex, i.texcoord);
                fixed3 light = tex2D(_LightTex, i.texcoord1).rgb;
                fixed3 light2 = tex2D(_LightTex, i.texcoord2).rgb;
                fixed4 ma= tex2D(_Metallic, i.texcoord); 
                light.rgb = lerp( light.rgb,light2,i.rotDelta);
                light =  (light - 0.5)*2;
                fixed3 speColor = texCUBE(_SpecIBL,i.viewReflectDirection).rgb*_SpecIBLPower;
				float3 indirectSpecular =   speColor.rgb*ma.r  * pow(i.NdotH, _Grass) *col.rgb;
                col.rgb =  col.rgb +  light.rgb +indirectSpecular;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
        ENDCG
    }
}

}

