Shader "Unlit/T4M 4 TextureLight"
{
	Properties
	{
		_Splat0("Layer 1", 2D) = "white" {}
		_Splat1("Layer 2", 2D) = "white" {}
		_Splat2("Layer 3", 2D) = "white" {}
		_Splat3("Layer 4", 2D) = "white" {}

		_Normal("normal", 2D) = "black" {}
		_Normal1("normal1", 2D) = "black" {}
		_Normal2("normal2", 2D) = "black" {}
		_Normal3("normal3", 2D) = "black" {}

		_Normal_strange("_Normal_strange", Vector) = (1,1,1,1)
		_LightDir("_LightDir", Vector) = (1,1,0,0)
		_Control("Control (RGBA)", 2D) = "white" {}
		_Lightmap("Lightmap", 2D) = "white" {}
		_Lightmap_Ctrl("Lightmap Ctrl", Range(0, 10)) = 1
		_LodDistance("Lod distance", Range(0, 500)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			sampler2D _Splat0;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _Splat3;

			sampler2D _Normal;
			sampler2D _Normal1;
			sampler2D _Normal2;
			sampler2D _Normal3;

			sampler2D _Control;

			float4 _LightDir;
			float4 _Normal_strange;
			float _Lightmap_Ctrl;

			#include "UnityCG.cginc"
			sampler2D _Lightmap;
			float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
			float4 _Control_ST;
			fixed _LodDistance;
			struct v2f
			{
				float4  pos : SV_POSITION;
				float2  uv[5] : TEXCOORD0;

				float4 posWorld  : TEXCOORD6;
				float3 normalDir : TEXCOORD7;

				UNITY_FOG_COORDS(8)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv[0] = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.uv[1] = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.uv[2] = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.uv[3] = TRANSFORM_TEX(v.texcoord, _Splat3);

				o.uv[4] = v.texcoord1.xy;// *unity_LightmapST.xy + unity_LightmapST.zw;

				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);

				UNITY_TRANSFER_FOG(o, o.pos);

				return o;
			}
			
			#define Emissive(_D_var) _D_var.rgb +specularValue * _D_var.a
			fixed4 frag (v2f IN) : SV_Target
			{
				fixed4 splat_control = tex2D(_Control, IN.uv[4]);
				fixed4 splat0 = tex2D(_Splat0, IN.uv[0]);
				fixed4 splat1 = tex2D(_Splat1, IN.uv[1]);
				fixed4 splat2 = tex2D(_Splat2, IN.uv[2]);
				fixed4 splat3 = tex2D(_Splat3, IN.uv[3]);

				fixed3 viewDistance = _WorldSpaceCameraPos.xyz - IN.posWorld.xyz;
				fixed dis = length(viewDistance) / _LodDistance;
				fixed4 norm  = tex2Dlod(_Normal, fixed4(IN.uv[0], 0, dis));
				fixed4 norm1 = tex2Dlod(_Normal1, fixed4(IN.uv[1],0, dis));
				fixed4 norm2 = tex2Dlod(_Normal2, fixed4(IN.uv[2],0, dis));
				fixed4 norm3 = tex2Dlod(_Normal3, fixed4(IN.uv[3],0, dis));

				fixed3 viewDirection = normalize(viewDistance);
				float3 normalDirection = IN.normalDir;
				fixed3 specularValue = pow(0.5*dot(IN.normalDir, normalize(viewDirection + fixed3(0, 0, -0.5))) + 0.5, 33.0);
				fixed4 c = (splat_control.r * splat0 + splat_control.g * splat1 + splat_control.b * splat2+ splat_control.a * splat3);
				c.rgb = Emissive(c);
				c.rgb *= tex2D(_Lightmap, IN.uv[4].xy) * _Lightmap_Ctrl;

				fixed4 normal = normalize(splat_control.r * norm * _Normal_strange.x + splat_control.g * norm1 * _Normal_strange.y + splat_control.b * norm2 * _Normal_strange.z + splat_control.a * norm3 * _Normal_strange.w);
				c.rgb = c.rgb * max(dot(normalize(_LightDir.xyz), normal), 0) * _LightDir.w;

				UNITY_APPLY_FOG(IN.fogCoord, c);
				return fixed4(c.rgb, 1.0);
			}
			ENDCG
		}
	}
}
