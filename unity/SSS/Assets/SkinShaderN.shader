Shader "Utils/SkinShaderN" 
{
	Properties 
	{
		_MainTint ("Global Tint", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_CurveScale ("Curvature Scale", Range(0.001, 0.09)) = 0.01
		_CurveAmount ("Curvature Amount", Range(0, 1)) = 0.5
		_BumpBias ("Normal Map Blur", Range(0, 5)) = 2.0
		_BRDF ("BRDF Ramp", 2D) = "white" {}
		_FresnelVal ("Fresnel Amount", Range(0.01, 0.3)) = 0.05
		_RimPower ("Rim Falloff", Range(0, 5)) = 2 
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_SpecIntensity ("Specular Intensity", Range(0, 1)) = 0.4
		_SpecWidth ("Specular Width", Range(0, 1)) = 0.2
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
 
 
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM
		// compile directives
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma multi_compile_fog
		#pragma multi_compile_fwdbase
		#include "HLSLSupport.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityShaderUtilities.cginc"
 
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"

 
		#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
		#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

 
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _BRDF;
		float4 _MainTint;
		float4 _RimColor;
		float _CurveScale;
		float _BumpBias;
		float _CurveAmount;
		float _FresnelVal;
		float _RimPower;
		float _SpecIntensity;
		float _SpecWidth;
		
		struct SurfaceOutputSkin
		{
		    fixed3 Albedo;
            fixed3 Normal;
            fixed3 Emission;
            fixed3 Specular;
            fixed Gloss;
            fixed Alpha;
            float Curvature;//曲率
            fixed3 BlurredNormals;//模糊的法向量
		};
		
		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
			half3 internalSurfaceTtoW0; 
			half3 internalSurfaceTtoW1; 
			half3 internalSurfaceTtoW2;
			 //可获取新法线
		};
		
	    inline fixed4 LightingSkinShader (SurfaceOutputSkin s, fixed3 lightDir, fixed3 viewDir, fixed atten)
        {
        	//Get all vectors for lighting
            viewDir = normalize ( viewDir );
            lightDir = normalize ( lightDir );
            s.Normal = normalize ( s.Normal );
            float NbdotL = dot ( s.BlurredNormals, lightDir );
            float3 halfDirection = normalize ( lightDir + viewDir );
			
			//Create BRDF and Faked SSS
			float3 brdf = tex2D(_BRDF, float2((NbdotL * 0.5 + 0.5)* atten, s.Curvature)).rgb;
			
			//Create Fresnel and Rim lighting
			float fresnel = saturate(pow(1-dot(viewDir, halfDirection),5.0));
			fresnel += _FresnelVal * (1 - fresnel);
			float rim = saturate(pow(1-dot(viewDir, s.BlurredNormals),_RimPower)) * fresnel;
			
			//Create Spec 
			float specBase = max(0,dot(s.Normal, halfDirection));
			float spec = pow (specBase, s.Specular*128.0) * s.Gloss;
			
			//Final Color
            fixed4 c;
            c.rgb = (s.Albedo * brdf * _LightColor0.rgb * atten) + (spec + (rim * _RimColor));
            c.a = 1.0f;
            return c;

        }
		

		void surf (Input IN, inout SurfaceOutputSkin o) 
		{
			//Get our texture information
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed3 normals = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			//normalBlur模糊后的法线
			//tex2Dbias用来获得皮肤上精彩的柔和的漫反射光照。这允许我们偏移或者调高或调低纹理分辨率等级//http://http.developer.nvidia.com/Cg/tex2Dbias.html

			float3 normalBlur = UnpackNormal ( tex2Dbias ( _BumpMap, float4 ( IN.uv_MainTex, 0.0, _BumpBias ) ) );


			//curvature计算曲率
			//WorldNormalVector通过输入的点及这个点的法线值,来计算它在世界坐标中的方向
			//fwidth 绝对值abs(ddx(x)) + abs(ddy(x)).only supported in pixel shaders
			//ddx(x) 	返回关于屏幕坐标x轴的偏导数
			//fwidth模型表面向量变化的快慢程度//http://http.developer.nvidia.com/Cg/fwidth.html
			//length计算向量的欧几里得长度//http://http.developer.nvidia.com/Cg/length.html

			//Calculate Curvature
			float curvature = length ( fwidth ( WorldNormalVector ( IN, normalBlur ) ) ) 
			                  / length ( fwidth ( IN.worldPos ) ) * _CurveScale;
			
			//apply all our information to our SurfaceOutput
			o.Normal = normals;
			o.BlurredNormals = normalBlur;
			o.Albedo = c.rgb * _MainTint;
			o.Curvature = curvature; 
			o.Specular = _SpecWidth;
			o.Gloss = _SpecIntensity;
			o.Alpha = c.a;
		}
		

 
 
		struct v2f_surf {
		  UNITY_POSITION(pos);
		  float2 pack0 : TEXCOORD0; // _MainTex
		  float4 tSpace0 : TEXCOORD1;
		  float4 tSpace1 : TEXCOORD2;
		  float4 tSpace2 : TEXCOORD3;
		  fixed3 vlight : TEXCOORD4; // ambient/SH/vertexlights
		  UNITY_SHADOW_COORDS(5)
		  UNITY_FOG_COORDS(6)
		 
		  UNITY_VERTEX_INPUT_INSTANCE_ID
		  UNITY_VERTEX_OUTPUT_STEREO
		};
 
	 
		float4 _MainTex_ST;

 
		v2f_surf vert_surf (appdata_full v) {
			  UNITY_SETUP_INSTANCE_ID(v);
			  v2f_surf o;
			  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
			  UNITY_TRANSFER_INSTANCE_ID(v,o);
			  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			  o.pos = UnityObjectToClipPos(v.vertex);
			  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
			  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
			  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
			  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
			  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
			  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
			  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
			 
			 
			  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
			  float3 shlight = ShadeSH9 (float4(worldNormal,1.0));
			  o.vlight = shlight;
			  #else
			  o.vlight = 0.0;
		  		#endif
		 
		 

		  	  UNITY_TRANSFER_SHADOW(o,v.texcoord1.xy); // pass shadow coordinates to pixel shader
		  	  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
		  return o;
		}

		// fragment shader
		fixed4 frag_surf (v2f_surf IN) : SV_Target {
			  UNITY_SETUP_INSTANCE_ID(IN);
			  // prepare and unpack data
			  Input surfIN;
			  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
			  surfIN.uv_MainTex.x = 1.0;
			  surfIN.worldPos.x = 1.0;
			  surfIN.worldNormal.x = 1.0;
			  surfIN.uv_MainTex = IN.pack0.xy;
			  float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
			  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
			  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
			  surfIN.worldNormal = 0.0;
			  surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
			  surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
			  surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
			  surfIN.worldPos = worldPos;

			  #ifdef UNITY_COMPILER_HLSL
			  SurfaceOutputSkin o = (SurfaceOutputSkin)0;
			  #else
			  SurfaceOutputSkin o;
			  #endif

			  o.Albedo = 0.0;
			  o.Emission = 0.0;
			  o.Specular = 0.0;
			  o.Alpha = 0.0;
			  fixed3 normalWorldVertex = fixed3(0,0,1);
			  o.Normal = fixed3(0,0,1);

			  // call surface function
			  surf (surfIN, o);

			  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
			  fixed4 c = 0;
			  float3 worldN;
			  worldN.x = dot(IN.tSpace0.xyz, o.Normal);
			  worldN.y = dot(IN.tSpace1.xyz, o.Normal);
			  worldN.z = dot(IN.tSpace2.xyz, o.Normal);
			  worldN = normalize(worldN);
			  o.Normal = worldN;
		 
		  	  c.rgb += o.Albedo * IN.vlight;

			  // realtime lighting: call lighting function
		  	  c += LightingSkinShader (o, lightDir, worldViewDir, atten);

		 

		 

			  UNITY_APPLY_FOG(IN.fogCoord, c); // apply fog
			  UNITY_OPAQUE_ALPHA(c.a);
		  	return c;
		}
 
ENDCG
 
}
 
	} 
	FallBack "Diffuse"
}
