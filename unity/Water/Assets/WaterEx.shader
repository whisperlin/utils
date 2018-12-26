// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'


Shader "Unlit/WaterEx"
{
	Properties
	{
		_GTexcolor ("全局颜色(A海面贴图透明)", Color) = (1,1,1,0.2)
		 _Alpha ("透明度",Range(0,1)) = 0
		_WaterTex ("WaterTex", 2D) = "black" {} 

		_WaveTex ("波浪图", 2D) = "black" {} //海浪
		_waveIntensity ("波浪亮度" , Range(0,100) ) =1 
		_WaveColor ("波浪颜色", Color) = (1,1,1,1) 
		_BumpTex ("法线图", 2D) = "bump" {} 
		 _DepthMark("深度贴图",2D) = "white"{}
		 _DepthPower("深度强度",float) = 0.5
		 _NormalIntensity("法线强度", Range(-3, 3)) = 1 

		//_GTex ("渐变图", 2D) = "white" {} //海水渐变
		_WaterTexColor ("边缘颜色", Color) = (0.5,0.5,0.5,0.5) 
		 _DeepColor("海底部颜色", color) = (1, 1, 1, 1)
		 _TopColor("海顶部颜色", color) = (1, 1, 1, 1)

		_NoiseTex ("噪点图", 2D) = "white" {} //海浪躁波
		_WaterSpeed("水速度", float) = 0.74  //海水速度
		_WaveSpeed("波浪速度", float) = -12.64 //海浪速度
		_WaveRange("波浪范围", float) = 0.3
		_NoiseRange("噪点范围", float) = 6.43
		_WaveDelta("波浪时间", float) = 2.43
		_Refract("反射噪点", float) = 0.07		
		
		_Range("范围", vector) = (0.13, 1.53, 0.37, 0.78)

	

		 _cubemapsky("sky" , 2D ) = "white" {} 
		 skyIntensiy("天空强度",float) = 1

 
	
		_AmbientLight("环境光", Color) = (0.5, 0.5, 0.5, 1) 

		 _Gama("Gama",Range(-1, 1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100
		//Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		//GrabPass{}
		zwrite off

		Pass
		{   
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog		
			#pragma multi_compile_instancing
			
			 #include "UnityCG.cginc"
			#pragma target 3.0


			sampler2D _GTex;
			sampler2D _WaterTex;
			float4 _WaterTex_ST;
			sampler2D _BumpTex;
			sampler2D _CameraDepthTexture;
			sampler2D _DepthMark;
			//sampler2D _GrabTexture;
			//half4 _GrabTexture_TexelSize;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			sampler2D _WaveTex;
			float4 _Range;
			half _WaterSpeed;
			half _WaveSpeed;
			fixed _WaveDelta;
			half _WaveRange;
			fixed _Refract;
			half _Specular;
			fixed _Glossxx;
			sampler2D  _cubemapsky;
			half _NoiseRange;
			float4 _WaterTex_TexelSize;
			
		
			fixed4 _GTexcolor;
			fixed4 _WaterTexColor;
			half _Alpha;
			half _waveIntensity;
			float skyIntensiy;
			float _DepthPower;

			fixed4  _DeepColor;
			fixed4 _TopColor;
			fixed4 _WaveColor;


			struct VertexInput  
			{			 
			    float4 vertex: POSITION;
                float3 normal: NORMAL;
				float4 tangent : TANGENT;
                float2 texcoord0: TEXCOORD0;  
                float2 texcoord1: TEXCOORD1;
                float4 color:COLOR;
 
			};

			struct  VertexOutput
			{
			    float4 pos: SV_POSITION;
                float2 uv0: TEXCOORD0; 
                float2 uv1:  TEXCOORD8;            
                float4 posWorld: TEXCOORD1;
                float3 normalDir: TEXCOORD2;           
                float4 proj : TEXCOORD3; 

				float4 tSpace0 : TEXCOORD4;
				float4 tSpace1 : TEXCOORD5;
				float4 tSpace2 : TEXCOORD6;
				float4 vcolor : TEXCOORD9;
				UNITY_FOG_COORDS(7)
 
			};
			uniform float _Gama;
			uniform float _NormalIntensity;
			uniform float4 _AmbientLight;
			float4 ColorSpace(float4 col){
				return pow(col , lerp (1/1.2, 1 / 2.2 , _Gama ));
			}
			 float3 NormalIntensity(float3 NormalMapTex){

				 // float4 finalNormap = ((NormalMapTex.rgba.rg * _NormalIntensity),NormalMapTex.b,NormalMapTex.a);

				 float2 finalNormap = NormalMapTex.rg *_NormalIntensity;
				 fixed3 o = fixed3(finalNormap.rg, NormalMapTex.b);
			      return o;
			}
			VertexOutput vert(VertexInput v){			  
				VertexOutput o = (VertexOutput) 0;
	 

				o.uv0 = TRANSFORM_TEX ( v.texcoord0 , _WaterTex) ;
				o.uv1 = v.texcoord1;
			    o.normalDir = UnityObjectToWorldNormal(v.normal);          
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.proj = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
             
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				o.vcolor = v.color;  
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;

				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, o.posWorld.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, o.posWorld.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, o.posWorld.z);
				UNITY_TRANSFER_FOG(o,o.pos);


                COMPUTE_EYEDEPTH(o.proj.z);

				 return o;
			}

			float4 frag (VertexOutput i):COLOR{
		
 				fixed2 UV = i.uv0;
				float4 proj = i.proj;
 

				float2 uv = proj.xy/proj.w;
				#if UNITY_UV_STARTS_AT_TOP
				if(_WaterTex_TexelSize.y<0)
					uv.y = 1 - uv.y;
					#endif


				


				float4 water = (tex2D(_WaterTex,UV + float2 (_WaterSpeed * _Time.x,0)) + tex2D( _WaterTex, float2 (1- UV.y , UV.x ) + float2 (_WaterSpeed * _Time.x ,0)))/2 * _WaterTexColor;
			
			    float4 offsetColor = (tex2D(_BumpTex,UV + float2(_WaterSpeed*_Time.x,0))+tex2D(_BumpTex, float2(1- UV.y,UV.x) + float2(_WaterSpeed*_Time.x,0)))/2;
			    half2 offset = UnpackNormal(offsetColor).xy * _Refract;

			   half deltaDepth =pow ((tex2D(_DepthMark, i.uv1).r *_DepthPower ),2);

			   // half deltaDepth =pow  ((i.vcolor *_DepthPower ),2) ;





			//    half m_depth = LinearEyeDepth(tex2Dproj (_CameraDepthTexture, proj).r);



			 //  half deltaDepth = m_depth - proj.z;

			    fixed4 noiseColor = tex2D(_NoiseTex, TRANSFORM_TEX(i.uv0, _NoiseTex));

			 
			   // half4 bott = tex2D(_GrabTexture, uv+offset);

			 //   fixed4 waterColor = ColorSpace (tex2D(_GTex, float2(min(_Range.y, deltaDepth)/_Range.y,1)) * _GTexcolor);


			    float3 gradient = ColorSpace (lerp(_TopColor,_DeepColor,min(_Range.y, deltaDepth)/_Range.y) * _GTexcolor);


			    fixed4 waveColor = tex2D(_WaveTex, float2(1-min(_Range.z, deltaDepth)/_Range.z+_WaveRange*sin(_Time.x*_WaveSpeed+noiseColor.r*_NoiseRange),1)+offset);
			    waveColor.rgb *= (1-(sin(_Time.x*_WaveSpeed+noiseColor.r*_NoiseRange)+1)/2)*noiseColor.r* _waveIntensity * _WaveColor;

			    fixed4 waveColor2 = tex2D(_WaveTex, float2(1-min(_Range.z, deltaDepth)/_Range.z+_WaveRange*sin(_Time.x*_WaveSpeed+_WaveDelta+noiseColor.r*_NoiseRange),1)+offset);
			    waveColor2.rgb *= (1-(sin(_Time.x*_WaveSpeed+_WaveDelta+noiseColor.r*_NoiseRange)+1)/2)*noiseColor.r * _waveIntensity * _WaveColor;
			
			    half water_A = 1-min(_Range.z, deltaDepth)/_Range.z;
		    	half water_B = min(_Range.w, deltaDepth)/_Range.w;

/////////Normal
				float4 bumpColor = (tex2D(_BumpTex, UV + offset + float2(_WaterSpeed*_Time.x, 0)) + tex2D(_BumpTex,  offset + float2(_WaterSpeed*_Time.x, 0))) / 2;
		
				float3 normal = UnpackNormal(bumpColor).rgb;
				float3 newnormal =  NormalIntensity(normal);




				float3 worldN = 0;
				worldN.x = dot(i.tSpace0.xyz, newnormal);
				worldN.y = dot(i.tSpace1.xyz, newnormal);
				worldN.z = dot(i.tSpace2.xyz, newnormal);
				worldN = normalize(worldN);
////////Albedo				
			    float3 albedo =  gradient * water_B ;	
			    albedo = albedo * (1 - water.a * water_A) + water.rgb * water.a*water_A ;
			    albedo =albedo + (waveColor.rgb+waveColor2.rgb) * water_A;
			    albedo = albedo * _AmbientLight;
////////Lighting
			    //fixed3 Light = DirectionalLight(worldN)[0];
////////Specular	
			     float3 worldRefl = _WorldSpaceCameraPos - i.posWorld; 


			    float3 _ref =reflect(-worldRefl ,  worldN);

            //    fixed3 sbl = SBL(_Cubemap,worldN ,fixed3(1,1,1));

                float3 sbl = tex2Dbias (_cubemapsky,float4 (normalize(_ref).xz,100,0));
              

           //     fixed3 sbl = texCUBE (_Cubemap , )
			
			 //   sbl =  texCUBE(_Cubemap,IN.worldRefl).rgb;
           //    float3 blendsp = RolanWaterLight(worldN,LightDir0) ;		    				 
				float3 specLight =lerp( albedo ,sbl ,skyIntensiy ) ;

			//	fixed3 SPL = SBL( _Cubemap,worldN, fixed3(1,1,1));

				float alpha = ((min(_Range.x, deltaDepth)/_Range.x)  ) * _Alpha  ;

				//float3 final = albedo * Light * ( Light * 0.8 + 0.2 )  + sbl + (water* _GTexcolor.a) ;
				float3 final = albedo  + specLight + (water* _GTexcolor.a) ;
				fixed4 ending = fixed4 (final,alpha );
				UNITY_APPLY_FOG(i.fogCoord, ending);

				return ending;

			//	return float4 (deltaDepth ,deltaDepth ,deltaDepth ,1);
				 
				 //return specLight * _Linear + float4(albedo * _AddAlbedo, 0);

			}

		
			ENDCG
		}


	}
//	FallBack "Diffuse"
}
