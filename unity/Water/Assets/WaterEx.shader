// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'


Shader "Unlit/WaterEx"
{
	Properties
	{
		_NormalMap ("_NormalMap", 2D) = "white" {}
		_MaskMap ("_MaskMap", 2D) = "white" {}
		_ReflectionTex ("反射贴图", 2D) = "white" {} 

		_SparklingSpecularWidth ("_SparklingSpecularWidth", Range (0, 256)) = 256
 
		[HideInInspector]_SparklingSpecularPower ("_SparklingSpecularPower", Range (1, 256)) = 1
		[HideInInspector]_SparklingSpecularScale ("_SparklingSpecularScale", Range (0, 1)) = 1

		_SkyColor ("天空颜色", Color) = (1,1,1,1)
 
		_SunColor ("湖面深色", Color) = (1,0,0,1)
		_SunSpecular ("湖面高光色", Color) = (1,1,1,1)
		_SPPower ("反射强度", Range (0, 3)) = 1
		[HideInInspector]_SunPower ("_SunPower", Range (0, 1)) = 1
		[HideInInspector]_MaskTiling ("_MaskTiling", Range (0, 1)) = 1
		[HideInInspector]_MaskConstrast ("_MaskConstrast", Range (0, 1)) = 1
		[HideInInspector]_MaskIntensity ("_MaskIntensity", Range (0, 1)) = 1


		_Speed ("_Speed",  Range (0, 1)) = 1
		_Wave1 ("_Wave1", Vector) = (1, 1,1,1)
		_Wave2 ("_Wave2", Vector) = (0.9,0.6,1,1)
		_Wave3 ("_Wave3", Vector) = (0.8,0.5,1,1)
		_Wave4 ("_Wave4", Vector) = (0.7,0.4,1,1)
		 
	}

		 
 

	SubShader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent"  }
		LOD 100

		Pass
		{
			CGPROGRAM
 
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
            #include "Lighting.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{

				float4 xlv0 : TEXCOORD0;
				float4 uv_1_2 : TEXCOORD1;
				float4 uv_3_4 : TEXCOORD2;
				float3 xlv3 : TEXCOORD3;
				float3 xlv4 : TEXCOORD4;
				float4 lpos_w : TEXCOORD5;
 
				SHADOW_COORDS(6)
				UNITY_FOG_COORDS(7)

				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

 
			sampler2D _NormalMap;
			sampler2D _MaskMap;
			sampler2D _ReflectionTex;
			
 
			float4 _NormalMap_ST;
			float4 _MaskMap_ST;
			float4 _ReflectionTex_ST;


			
			//vs
			uniform  float _Speed;
			uniform  float4 _Wave1;
			uniform  float4 _Wave2;
			uniform  float4 _Wave3;
			uniform  float4 _Wave4;


			uniform  float _SparklingSpecularWidth;
 
			uniform  float _SparklingSpecularPower;
			uniform  float _SparklingSpecularScale;

			uniform  float4 _SunColor;
			uniform  float4 _SkyColor;
			uniform  float4 _SunSpecular;
			uniform  float _SunPower;
			uniform  float _MaskTiling;
			uniform  float _MaskConstrast;
			uniform  float _MaskIntensity;
			uniform float _SPPower;
 

			
			v2f vert (appdata v)
			{
				v2f o;
				//o.vertex = UnityObjectToClipPos(v.vertex);
			 
 
			   float4 vertex_2;
 
 
			   float4 uv0_wpos;
			   float4 _ware1;
			   float4 _ware2;
			   float3 _dis;
			   float4 _lpos_uv1_x;

			  //float4 tmpvar_10;
			  vertex_2.xzw =v.vertex.xzw;

			   //原来的浪
			  //vertex_2.y = (v.vertex.y - ((0.2 - 
			  //  ((sin((
			 //     (normalize(v.vertex.xyz) * _Time.z)
			 //   .x * 2.0)) * 0.2) * (1.0 -v.color.w))
			 //) *v.uv0.y));
			 //去掉波浪幅度控制
			 
			  float v0 =  (v.vertex.x + v.vertex.y + v.vertex.z)*15;
			  // vertex_2.y = v.vertex.y + sin( _Time.y + v0 ) * 0.2;
			  vertex_2.y = v.vertex.y;

			  o.vertex = UnityObjectToClipPos(vertex_2);
 			  //o.vertex = UnityObjectToClipPos(v.vertex);
 			 
		 
			  _lpos_uv1_x.xyz = vertex_2.xyz /vertex_2.w ;
			  uv0_wpos.xy =v.uv0.xy;

			  _lpos_uv1_x.w =v.uv1.x;

			   float2 _Wave1_w;
			  _Wave1_w.x = sin(_Wave1.w);
			  _Wave1_w.y = cos(_Wave1.w);
			   float2 _normal;
			  _normal = (v.vertex.xz * _NormalMap_ST.xy);
			   
			  _ware1.xy = (((_normal + _NormalMap_ST.zw) * _Wave1.y) + ((_Time.y*_Speed * _Wave1.x) * _Wave1_w));
			   float2 _wave2_2;
			  _wave2_2.x = sin(_Wave2.w);
			  _wave2_2.y = cos(_Wave2.w);
			  _ware1.zw = (((_normal + _NormalMap_ST.zw) * _Wave2.y) + ((_Time.y*_Speed * _Wave2.x) * _wave2_2));
			   float2 _wave3_w;
			  _wave3_w.x = sin(_Wave3.w);
			  _wave3_w.y = cos(_Wave3.w);
			  _ware2.xy = (((_normal + _NormalMap_ST.zw) * _Wave3.y) + ((_Time.y*_Speed * _Wave3.x) * _wave3_w));
			   float2 _wave4_w;
			  _wave4_w.x = sin(_Wave4.w);
			  _wave4_w.y = cos(_Wave4.w);
			  _ware2.zw = (((_normal + _NormalMap_ST.zw) * _Wave4.y) + ((_Time.y*_Speed * _Wave4.x) * _wave4_w));
			   float3 _distance  = (_WorldSpaceCameraPos - vertex_2.xyz);

			  float4 wpos;
			  wpos =mul(unity_ObjectToWorld, v.vertex); 
			  //mul(unity_ObjectToWorld, v.vertex); 

			  uv0_wpos.zw = TRANSFORM_TEX(wpos.xz , _MaskMap);
			  //uv0_wpos.zw = ((wpos.xz * _MaskMap_ST.xy) + _MaskMap_ST.zw);

 
			  _dis = mul(UNITY_MATRIX_P , _distance);
			  _dis.xy = (_dis.xy / _dis.z);
			  _dis.xy = (((_dis.xy + 
			    (    _Time.xx)
			  ) * _ReflectionTex_ST.xy) + _ReflectionTex_ST.zw);

			  //tmpvar_10 = (unity_WorldToShadow[0] * wpos);
			 
				o.color =v.color;
				o.xlv0 = uv0_wpos;

				o.uv_1_2 = _ware1;
				o.uv_3_4 = _ware2;
				o.xlv3 = normalize(_distance.zxy);
				o.xlv4 = _dis;
				o.lpos_w = _lpos_uv1_x;
				//o.xlv6 = tmpvar_10;			
				TRANSFER_SHADOW(o);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			float2 R_To_SkyUV(float3 r)
            {
            	return normalize(r).xz;
            }
			fixed4 frag (v2f i) : SV_Target
			{
 				;
				float4 ret_color;
				float alpha_2;
				float maskAlpha_3;
				float3 sun_4;
				float bright_5;
				float3 h_6;
				float3 final_sky_color;

				//float4 _normal0  = tex2D (_NormalMap, i.uv_1_2.xy);
				//return _normal0;
				float4 _normal1  = ((tex2D (_NormalMap, i.uv_1_2.xy) * 2.0) - 1.0);

				//xlv0

				float4 _normal4 = ((tex2D (_NormalMap, i.uv_1_2.zw) * 2.0) - 1.0);
				float4 _normal2 = ((tex2D (_NormalMap, i.uv_3_4.xy) * 2.0) - 1.0);
				float4 _normal3 = ((tex2D (_NormalMap, i.uv_3_4.zw) * 2.0) - 1.0);
 				//return _normal1;
				float4 _normal_final;
				_normal_final = normalize(((
				((_normal1 * _Wave1.z) + (_normal4 * _Wave2.z))
				+ 
				(_normal2 * _Wave3.z)
				) + (_normal3 * _Wave4.z)));




				float _distance;
				_distance = normalize((i.lpos_w.xyz - _WorldSpaceCameraPos)).z;
				float3 tmpvar_18;
				tmpvar_18.x = _SparklingSpecularWidth;
				tmpvar_18.y = _SparklingSpecularWidth;
				tmpvar_18.z = 1;

			
				float4 _sky_color;

				// my begin
				float3 posEyeSpace = mul(UNITY_MATRIX_MV,float4(i.lpos_w.xyz,1)).xyz;
				float3 dir = posEyeSpace - float3(0,0,0);
				float3 N = mul((float3x3)UNITY_MATRIX_MV,_normal_final);
                N = normalize(N);
                float3 R = reflect(dir,N);
               // o.uv2 = R_To_UV(R);
                _sky_color =  tex2D (_ReflectionTex, R_To_SkyUV(R));
                // my ....... end

               float3 dir2 = mul( unity_WorldToObject, _WorldSpaceLightPos0.xyz);
               dir2 = normalize(dir);
                float3 R2 = reflect(dir2,N);

               //高光波纹.
               float sp_ware = pow (  clamp (  dot (_normal_final.xyz, tmpvar_18) , 0.0, 1.0) ,  _SparklingSpecularPower);
               
				//return _sky_color;

				//按照距离来融合天空盒.
				//final_sky_color = (i.color.xyz + ((_sky_color.xyz * i.xlv0.x) * pow ((_distance + 0.15), 8.0)));
				final_sky_color = _sky_color.xyz * _SkyColor;

				float3 tmpvar_22;
				tmpvar_22 = normalize((float3(0.8374783, 0.1570272, 0.5234239) + i.xlv3));
				h_6 = tmpvar_22;
				bright_5 = max (0.0, h_6.z);
				float _p1 = pow (bright_5, _SunPower);
				float _p2 = pow ((_distance + 0.1), 16.0) ;

				sun_4 = ((float3(  _p1,_p1,_p1  ) * (_SunColor.xyz + ((sp_ware * _SunSpecular) * (100.0 * _SunSpecular.w)).xyz)) * float3(   _p2,_p2,_p2   ));
				//return float4(sun_4.xyz,1);
				 
				float2 P_24;
				P_24 = (i.xlv0.zw * _MaskTiling);
				float tmpvar_25;
				tmpvar_25 = tex2D (_MaskMap, P_24).x;
				maskAlpha_3 = tmpvar_25;
				alpha_2 = i.color.w;
				if ((i.color.w < 1.0)) {
				maskAlpha_3 = (((maskAlpha_3 - 0.5) * (_MaskConstrast + 1.0)) + 0.5);
				maskAlpha_3 = (maskAlpha_3 * _MaskIntensity);
				 float tmpvar_26;
				tmpvar_26 = (1.0 - clamp (pow (
				  (1.0 - i.color.w)
				, 
				  (8.0 * i.lpos_w.w)
				), 0.0, 1.0));
				alpha_2 = (clamp ((tmpvar_26 * i.xlv0.y), 0.0, 1.0) + (clamp (
				  (maskAlpha_3 * tmpvar_26)
				, 0.0, 1.0) * (1.0 - i.xlv0.y)));
				};
				_distance = clamp(_distance,0,1);
				float _distance_power = min (1.0, pow (_distance, 12.0));

				//return float4(sun_4,1);
				float3 final_color;
				 
				float3 _final_sum =  clamp (sun_4, 0.0, 1.0) ;
				float3 _sky1 = final_sky_color * _distance_power ;
				float3 _sky2 =  final_sky_color * (1.0 - _distance_power) * dot (_normal_final.xyz, i.xlv3);
				float3 _final_ware_color =  clamp ((sp_ware * _SparklingSpecularScale), 0.0, 1.0)*_distance*_SPPower;
				final_color =    clamp (sun_4, 0.0, 1.0)   +  ( _sky1 +_sky2)   +_final_ware_color  ;
			 

				//final_color =final_sky_color;
				//final_color = ((clamp (sun_4, 0.0, 1.0) + ((final_sky_color * _distance_power) +  ((final_sky_color * (1.0 - _distance_power)) * dot (_normal_final.xyz, i.xlv3))  ))  );
				//final_color = clamp (sun_4, 0.0, 1.0)   * _distance_power  ;

				//final_color =final_sky_color * _distance_power;
 				
				float4 _col;
 
				_col.xyz = final_color;
				_col.w = alpha_2;
				//return lerp(_col,float4(final_sky_color,1),_SPPower);
				return _col;
				 



			}
			ENDCG
		}
	}
}
