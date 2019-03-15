Shader "Unlit/Substance"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Metallic("Metallic",Range(0,1)) = 0.5
		_SpecularLevel("SpecularLevel",Range(0,1)) = 0.5
		_Roughness("Roughness",Range(0,1)) = 0.5

 
		nbSamples("nbSamples",Range(16,256)) = 16
		horizonFade("horizonFade",Range(0,2)) = 1.3
		environment_exposure("environment_exposure",Range(0,1)) = 0.5

		 environment_texture("environment_texture", CUBE) = ""{} 

		 maxLod("maxLod",Range(0,8)) = 8

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "substance.cginc"
			float _Metallic;
			float _SpecularLevel;
			float _Roughness;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent :TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;

				float3 worldPos : TEXCOORD1;

                float3 worldNormal : TEXCOORD2;
                float3 worldTangent: TEXCOORD3;
 				float3 worldBitangent :TEXCOORD4;

 				float3 normal : TEXCOORD5;
 				float3 tangent : TEXCOORD6;
 				float3 bitangent : TEXCOORD7;
 				float3 ambient : TEXCOORD8;

				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _NbSamples;
			v2f vert (appdata v)
			{
				v2f o; 
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));

				o.normal = v.normal;

                o.worldNormal = worldNormal;
                o.worldTangent = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.worldBitangent = normalize(cross(o.worldNormal, o.worldTangent) * v.tangent.w);
              
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
 				o.ambient = ShadeSH9(half4(o.worldPos,1))  ;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
 
				 LocalVectors vectors ;
				 vectors.vertexNormal = i.normal;

				 vectors.normal = normalize(i.worldNormal);
				 vectors.tangent = normalize(i.worldTangent);
				 vectors.bitangent = normalize(i.worldBitangent);
				 vectors.eye =  normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				 //Trick to remove black artefacts Backface ? place the eye at the opposite - removes black zones
				  if (dot(vectors.eye, vectors.normal) < 0.0) {
				    vectors.eye = reflect(vectors.eye, vectors.normal);
				  }

				 float3 worldNormal = normalize(i.worldNormal);
                //float3 lightDir = UnityWorldSpaceLightDir(i.worldPos);
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
       
                float3 viewDir = UnityWorldSpaceViewDir(i.worldPos);
                viewDir = normalize(viewDir);
                float3 halfDir = normalize(lightDir + viewDir);

                float NdotL = max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));
                float NdotH = max(0, dot(halfDir, worldNormal));
                float3 atten = NdotL*_LightColor0.rgb;

				fixed3 baseColor;
				float metallic;
				float specularLevel;
				float occlusion;// OA
				float emissive;
				float roughness;
				// sample the texture
				baseColor = tex2D(_MainTex, i.uv).rgb;
				metallic = _Metallic;
				occlusion = 1;
				emissive = i.ambient*baseColor;
				roughness = _Roughness;
				specularLevel = _SpecularLevel;

				float3 diffColor = generateDiffuseColor(baseColor, metallic);
				float3 specColor = generateSpecularColor(specularLevel, baseColor, metallic);
				float specOcclusion = specularOcclusionCorrection(occlusion, metallic, roughness);
				float diffuseShading  = occlusion* atten;
				//specularShadingOutput(specOcclusion * pbrComputeSpecular(vectors, specColor, roughness));
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);

				float3 spe = specOcclusion * pbrComputeSpecular(vectors, specColor, roughness);
				//return float4(spe,1);
				return float4(emissive + diffColor*diffuseShading + spe,1);
			}
			ENDCG
		}
	}
}
