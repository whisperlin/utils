Shader "Rolan/Character/Aniso Circular Hair" {
	Properties {
		_MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		 [HideInInspector]_SpecularTex ("高光贴图", 2D) = "white" {}
		_SpecularMultiplier ("高光强度", float) = 1.0
		_SpecularColor ("高光颜色", Color) = (1,1,1,1)
		_AnisoOffset ("高光位置", Range(-1,1)) = 0.0
		_Cutoff ("Alpha Cut-Off Threshold", float) = 0.9
		_gloss ( "Gloss Multiplier", float) = 128.0
		  _BumpMap("法线贴图", 2D) = "bump" {}
		    _NormalIntensity("法线强度", Range(0, 3)) = 1 
		    	_emssion ("自发光", Color) = (0,0,0,1)

	}
	
	SubShader
	{	
		Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

	    Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
		Blend Off
		Cull Back
		ZWrite on
		
		CGPROGRAM
		#pragma vertex vert
        #pragma fragment frag
		  #include "../CGIncludes/RolantinCG.cginc"

	
		sampler2D _MainTex, _SpecularTex;
		float _AnisoOffset, _SpecularMultiplier, _gloss, _Cutoff;
		fixed4 _SpecularColor, _Color;
		 uniform sampler2D _BumpMap;


		struct  vertexin{ 
			float2 uv0:TEXCOORD0 ;
			float4 vertex:POSITION ;
			float4 normal:NORMAL;
			float4 tangent:TANGENT;	
		};

		struct vertexout{
			float4 pos : SV_POSITION;
			float2 uv0 : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
			float2 uv2 : TEXCOORD2;
			float4 posWorld : TEXCOORD3;
			float3 normalDir : TEXCOORD4;
			float3 tangentDir : TEXCOORD5;
			float3 bitangentDir : TEXCOORD6;
		};
			
		vertexout vert(vertexin v){

			vertexout o = (vertexout)0;
			o.uv0 = v.uv0;
		    o.posWorld =mul(unity_ObjectToWorld, v.vertex);	
			o.normalDir = UnityObjectToWorldNormal(v.normal);
			    o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);

                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
			o.pos = UnityObjectToClipPos(v.vertex);
			return o;

		}

		float4 frag (vertexout i):Color{
			fixed3 Albedo;
		

		   

           
		    float3 viewDir = normalize(_WorldSpaceCameraPos - i.posWorld);

		    float3 Light = DirectionalLight(i.normalDir)[3];

			fixed4 albedo = tex2D(_MainTex, i.uv0);

			Albedo = lerp(albedo.rgb,albedo.rgb*_Color.rgb,0.5);

			fixed Alpha = albedo.a;

			clip (Alpha - _Cutoff);






			fixed4 c;
			c.rgb = ((Albedo  )) ;
			return float4(c.rgb,Alpha);
		}
		ENDCG
	}

        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	    Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ZWrite off
		
		CGPROGRAM
		#pragma vertex vert
        #pragma fragment frag
	  #include "../CGIncludes/RolantinCG.cginc"

	
		sampler2D _MainTex, _SpecularTex;
		float _AnisoOffset, _SpecularMultiplier, _gloss, _Cutoff;
		fixed4 _SpecularColor, _Color;
		 uniform sampler2D _BumpMap;
		float4 _emssion;


		struct  vertexin{ 
			float2 uv0:TEXCOORD0 ;
			float4 vertex:POSITION ;
			float4 normal:NORMAL;
			float4 tangent:TANGENT;	
		};

		struct vertexout{
			float4 pos : SV_POSITION;
			float2 uv0 : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
			float2 uv2 : TEXCOORD2;
			float4 posWorld : TEXCOORD3;
			float3 normalDir : TEXCOORD4;
			float3 tangentDir : TEXCOORD5;
			float3 bitangentDir : TEXCOORD6;
		};
			
		vertexout vert(vertexin v){

			vertexout o = (vertexout)0;
			o.uv0 = v.uv0;
			  
		    o.posWorld =mul(unity_ObjectToWorld, v.vertex);	
			o.normalDir = UnityObjectToWorldNormal(v.normal);
			  o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);

                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
			o.pos = UnityObjectToClipPos(v.vertex);
			return o;

		}

		float4 frag (vertexout i):Color{
			fixed3 Albedo;
		

                 float3x3 tangentTransform = float3x3(i.tangentDir, i.bitangentDir, i.normalDir);         
                float3 _BumpMap_var = UnpackNormal (tex2D(_BumpMap,i.uv0));
            
                float3 normalLocal = NormalIntensity(_BumpMap_var);



                float3 normalDirection = normalize(mul(normalLocal, tangentTransform)); // Perturbed normals
               

		    float3 viewDir = normalize(_WorldSpaceCameraPos - i.posWorld);

		    float3 Light = DirectionalLight(normalDirection)[3];

			fixed4 albedo = tex2D(_MainTex, i.uv0);

			Albedo = lerp(albedo.rgb,albedo.rgb*_Color.rgb,0.5);

			fixed Alpha = albedo.a;

			//clip (Alpha - _Cutoff);




			fixed4 specvar = tex2D(_SpecularTex, i.uv0);
		

			fixed3 h = normalize(normalize(-LightDir0) + normalize(viewDir));
				fixed3 h2 = normalize(normalize(-LightDir2) + normalize(viewDir));
			//float NdotL = saturate(dot(i.normalDir, -LightDir0));

			fixed HdotA = dot(normalDirection, h);
			float aniso = max(0, sin(radians((HdotA + _AnisoOffset) * 180)));


			fixed HdotA2 = dot(normalDirection, h2);
			float aniso2 = max(0, sin(radians((HdotA2 + _AnisoOffset) * 180)));



			float spec = saturate(dot(normalDirection, h));
				float spec2 = saturate(dot(normalDirection, h2));
			

			spec = saturate(pow(lerp(spec, aniso, specvar.b),  specvar.g * _gloss) * specvar.r);
			spec = spec * _SpecularMultiplier;

			spec2 = saturate(pow(lerp(spec2, aniso2, specvar.b),  specvar.g * _gloss) * specvar.r);
			spec2 = spec2 * _SpecularMultiplier;

			fixed4 c;
			c.rgb = ((Albedo * Light * _Color) + ( spec * _SpecularColor * Light) + ( spec2 * _SpecularColor * Light) ) + (Albedo*_emssion)  ;
			return float4(c.rgb,Alpha);
		}
		ENDCG
	}
	

}
	FallBack "Transparent/Cutout/VertexLit"

}