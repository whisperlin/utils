Shader "DAFUHAO_Editor/Terrain-HeightMap-Base"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_TerrainHeightMap("_TerrainHeightMap", 2D) = "white" {}
		_TerrainTangentMap("_TerrainTangentMap", 2D) = "white" {}
		_TerrainLocalNormalMap("_TerrainLocalNormalMap", 2D) = "bump" {}
		_TerrainNormalMap("_TerrainNormalMap", 2D) = "bump" {}

		_TerrainHeightMean("TerrainHeightMean", Float) = 1
		_TerrainHeightMaxDiv("TerrainHeightMaxDiv", Float) = 1
		_TerrainMapSize("TerrainMapSize", Float) = 1024

		_TerrainHeightOffset("TerrainHeightOffset", Float) = 0

		_ToLinear("To Linear", Range(0, 2.2)) = 1
		[HideInInspector]_TerrainLocalPosition("_TerrainLocalPosition", Vector) = (0,0,0,0)
		[HideInInspector]_Albedo_ST("_Albedo_ST", Vector) = (1,1,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "DiffuseInfo.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;

				float4 viewPos : TEXCOORD2;
				float4 worldPos_2_Camera : TEXCOORD3;
				float3 normalDir : TEXCOORD4;
				float3 tangentDir : TEXCOORD6;
				float3 bitangentDir : TEXCOORD7;

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_FOG_COORDS(5)
			};

 

			sampler2D _TerrainHeightMap;
			sampler2D _TerrainNormalMap;
			sampler2D _TerrainTangentMap;
			sampler2D _TerrainLocalNormalMap;
			float _TerrainHeightMean;
			float _TerrainHeightMaxDiv;
			float _TerrainMapSize;
			

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			uniform float4 _Color;
			uniform float _ToLinear;
			uniform float4 _Albedo_ST;
			float _TerrainHeightOffset;

			float4x4 _TerrainLToWMatrix;


			
			INSTANCING_START
			INSTANCING_PROP_UVST
			UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainLocalPosition)
			INSTANCING_END			

			#define GET_CAMERA_POS_NORMAL(worldpos, viewpos, worldpos_2_camera, Normal)\
				worldpos.xyz = CalcBias(Normal, worldpos.xyz, BiasParameters);\
				viewpos = mul(_CameraMatrix, worldpos); 
				//worldpos_2_camera = CalcShadowmapPos(viewpos);
			VertexOutput vert (appdata v)
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_INITIALIZE_OUTPUT(VertexOutput, o);
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				float4 localPos = v.vertex + UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _TerrainLocalPosition);
				
				//float4 posWorld = mul(unity_ObjectToWorld, localPos);
				float4 posWorld = mul(_TerrainLToWMatrix, localPos);

				float2 worldUV = localPos.xz / _TerrainMapSize;
				float4 var1 = tex2Dlod(_TerrainHeightMap, float4(worldUV, 0, 0));
					
				float height = DecodeFloatRGBA(var1);

				float3 localNormal = UnpackNormal(tex2Dlod(_TerrainLocalNormalMap, float4(worldUV, 0, 0)));

				posWorld.y = (height - 0.5) * 2.0 * _TerrainHeightMaxDiv + _TerrainHeightMean + _TerrainHeightOffset;
				o.vertex = UnityWorldToClipPos(posWorld);

				v.normal = localNormal;
				o.normalDir = UnityObjectToWorldNormal(localNormal);

				v.tangent = tex2Dlod(_TerrainTangentMap, float4(worldUV, 0, 0));
				v.tangent = v.tangent * 2 - 1;
				o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);

				o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);

				o.uv = worldUV;

				//GET_CAMERA_POS_NORMAL(posWorld, o.viewPos, o.worldPos_2_Camera, o.normalDir);							

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (VertexOutput i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				// sample the texture
				fixed4 _Albedo_var =  tex2D(_MainTex, i.uv);

				float3x3 tangentTransform = float3x3(i.tangentDir, i.bitangentDir, i.normalDir);

				float3 normalTan = UnpackNormal(tex2D(_TerrainNormalMap, i.uv));
				float3 normalDirection = normalize(mul(normalTan, tangentTransform));



				float4 finalColor =  float4(  ((_Albedo_var.rgb / _ToLinear)*GetEnvirmentColor(normalDirection)*_Color.rgb) ,1 );
				//float4 finalColor = FINAL_SHADOW_COLOR_SINGLE(((_Albedo_var.rgb / _ToLinear)*GetEnvirmentColor(normalDirection)*_Color.rgb), i, normalDirection);
								
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, finalColor.rgb);
				return finalColor;
			}
			ENDCG
		}
	}
}
