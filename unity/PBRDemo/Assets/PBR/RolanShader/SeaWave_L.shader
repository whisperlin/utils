Shader "Rolan/SeaWave_L"
{
	Properties
	{
		_WaterTex ("WaterTex", 2D) = "black" {} 
		_WaterTexColor ("边缘颜色", Color) = (1,1,1,1)


		_BumpTex ("法线图", 2D) = "bump" {} 
		 av("Cubemap" , Cube) = "white" {}

		    _IBL_Diffuse("环境球", Cube) = "_Skybox" {}
        IBL_Intensity("环境球强度", Range(0, 10)) = 1 
        _AmbientLight("环境光", Color) = (0.5, 0.5, 0.5, 1) 
        SBL_Intensity("反射球强度", Range(0, 10)) = 0 
         _BlinnPhongSP("高光强度", Range(0, 10)) = 1 
        _BP_Gloss("高光范围", Range(0, 1)) = 0 


	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100
		//Cull Off
		Blend SrcAlpha OneMinusSrcAlpha


		Pass
		{   
			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog		
		 #include "../CGIncludes/RolantinCG.cginc"
			#pragma target 3.0
            #pragma multi_compile_instancing

			sampler2D _WaterTex; float4 _WaterTex_ST;
			sampler2D _BumpTex; float4 _BumpTex_ST;
			float4 _WaterTexColor;
			   uniform samplerCUBE av;
		


			struct VertexInput  
			{			 
			    float4 vertex: POSITION;
                float3 normal: NORMAL;
				float4 tangent : TANGENT;
                float2 texcoord0: TEXCOORD0;   
			};

			struct  VertexOutput
			{
			    float4 pos: SV_POSITION;
                float2 uv0: TEXCOORD0;              
                float4 posWorld: TEXCOORD1;
                float3 normalDir: TEXCOORD2;           
                float4 proj : TEXCOORD3; 

				float4 tSpace0 : TEXCOORD4;
				float4 tSpace1 : TEXCOORD5;
				float4 tSpace2 : TEXCOORD6;
			};


			VertexOutput vert(VertexInput v){			  
				VertexOutput o = (VertexOutput) 0;
				o.uv0 = TRANSFORM_TEX ( v.texcoord0 , _WaterTex) ;
			    o.normalDir = UnityObjectToWorldNormal(v.normal);          
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.proj = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
             
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;

			

				 return o;
			}

			float4 frag (VertexOutput i):COLOR{

			float3  no = UnpackNormal (tex2D (_BumpTex , TRANSFORM_TEX(float2 (i.uv0.x*_Time.x *1, (i.uv0.y +0.5)*_Time.x *1 ) ,_BumpTex)));
				float3 worldN = 0;
				worldN.x = dot(i.tSpace0.xyz,  no);
				worldN.y = dot(i.tSpace1.xyz,  no);
				worldN.z = dot(i.tSpace2.xyz,  no);
				worldN = normalize(worldN);



				float3 sb = SBL(av,worldN ,1);

			float3 light =  DirectionalLight(worldN)[0];
			 float3 HighLight = (Phong(LightDir0, no ,1,1) ) *light + sb  ;
			


			 float4 diff = tex2D (_WaterTex , TRANSFORM_TEX(i.uv0,_WaterTex)) * _WaterTexColor;
			 float3 maintex =( diff.rgb  *light )+ HighLight;

 			  return fixed4 (maintex,_WaterTexColor.a);

			}

		
			ENDCG
		}


	}
//	FallBack "Diffuse"
}
