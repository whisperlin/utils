// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Cube"
{
    Properties
    {
         _Color ("_Color", Color) = (1.0, 0.0, 0.0, 1)
         _Border("_Border",Float) = 0.03
         _Gloss("_Gloss",Range(16,266)) = 16
		//_CubAlpha("Alpha", Float) = 0.2
		//_CubBump("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+900" } 
        LOD 100

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
	
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

           #include "UnityCG.cginc"
            #include "Lighting.cginc"
			float4 _Color; 
		 	float _CubAlpha;
		 	float _Border;
			sampler2D _CubBump;
			float4 _CubBump_ST;
            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
				float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
              
                float4 vertex : SV_POSITION;

                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                 float2 uv : TEXCOORD2;

				half3 tspace0 : TEXCOORD3;
                half3 tspace1 : TEXCOORD4;
                half3 tspace2 : TEXCOORD5;
            };

        

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                half3 wNormal = UnityObjectToWorldNormal(v.normal);
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

                o.worldNormal = wNormal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                return o;
            }
            float ArmBRDF(float roughness ,float NdotH ,float LdotH)
			{
				float n4 = roughness*roughness*roughness*roughness;
				float c = NdotH*NdotH   *   (n4-1) +1;
				float b = 4*3.14*       c*c  *     LdotH*LdotH     *(roughness+0.5);
				return n4/b;

			}
			float _Gloss;
            fixed4 frag (v2f i) : SV_Target
            {
            	half3 tnormal = UnpackNormal(tex2D(_CubBump, i.uv  ));
				//half3 tnormal = UnpackNormal(tex2D(_CubBump, TRANSFORM_TEX(i.uv, _CubBump)));
            	//float3 worldNormal = normalize(i.worldNormal);
               
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);
       
                float3 viewDir = UnityWorldSpaceViewDir(i.worldPos);
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				//fixed3 lightDir = -viewDir;
				viewDir = normalize(viewDir);
                float3 halfDir = normalize(lightDir + viewDir);
                float ndv = dot(worldNormal,viewDir);
                float nl = max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));
                float d = max(0, dot(halfDir, worldNormal));
                float p1 = pow(d, _Gloss);
                float3 spec =   p1 ;


                // diffuse
                float3 diff =   _Color.rgb  ;

                float3 c =   max(spec ,0) +diff ; 

                _Color.rgb = c;

                
                _Color.a = lerp(1,_CubAlpha,ndv);

                float b = 
            	max( 
            	max((_Border-1+i.uv.x),(_Border-i.uv.x)) ,
            	max((_Border-1+i.uv.y),(_Border-i.uv.y))
            	) ;
                return float4(0,0,0,1)*step(0,b)+_Color*step(b,0);
                //_Color.a = 1;
            	return _Color; 
            }
            ENDCG
        }
    }
}
