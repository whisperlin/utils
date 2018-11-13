Shader "Unlit/T4M 8 HardTexture"
{
	Properties{
	_Splat0("Layer1 (RGB)", 2D) = "white" {}
	_Splat1("Layer2 (RGB)", 2D) = "white" {}
	_Splat2("Layer3 (RGB)", 2D) = "white" {}
	_Splat3("Layer4 (RGB)", 2D) = "white" {}
	_Control("Control (RGBA)", 2D) = "white" {}

	_Surf0("surf_1 (RGB)", 2D) = "black" {}
	_Surf1("surf_2 (RGB)", 2D) = "black" {}
	_Surf2("surf_3 (RGB)", 2D) = "black" {}
	_Surf3("surf_4 (RGB)", 2D) = "black" {}
	_Surf_Scale("surf_scale", Vector) = (1,1,1,1)
	_Mask_Scale("mask_Scale", Vector) = (1,1,1,1)
	_MainTex("Never Used", 2D) = "white" {}
	}
		SubShader{
		Pass{

		CGPROGRAM
#include "UnityCG.cginc"
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
#pragma exclude_renderers xbox360 ps3
	sampler2D _Splat0;
	sampler2D _Splat1;
	sampler2D _Splat2;
	sampler2D _Splat3;
	sampler2D _Control;

	sampler2D _Surf0;
	sampler2D _Surf1;
	sampler2D _Surf2;
	sampler2D _Surf3;


	struct v2f {
		float4  pos : SV_POSITION;
#ifdef LIGHTMAP_ON
		float2  uv[6] : TEXCOORD0;
#endif
#ifdef LIGHTMAP_OFF
		float2  uv[5] : TEXCOORD0;
#endif
	};

	float4 _Splat0_ST;
	float4 _Splat1_ST;
	float4 _Splat2_ST;
	float4 _Splat3_ST;
	float4 _Control_ST;
#ifdef LIGHTMAP_ON
	/// fixed4 unity_LightmapST;
	// sampler2D unity_Lightmap;
#endif

	float4 _Surf_Scale;
	float4 _Mask_Scale;

	v2f vert(appdata_full  v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv[0] = TRANSFORM_TEX(v.texcoord, _Splat0);
		o.uv[1] = TRANSFORM_TEX(v.texcoord, _Splat1);
		o.uv[2] = TRANSFORM_TEX(v.texcoord, _Splat2);
		o.uv[3] = TRANSFORM_TEX(v.texcoord, _Splat3);
		o.uv[4] = TRANSFORM_TEX(v.texcoord, _Control);
#ifdef LIGHTMAP_ON
		o.uv[5] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
		return o;
	}

	float4 frag(v2f i) : COLOR
	{
		float4 Mask = tex2D(_Control, i.uv[4].xy);
	
		const float4 maskCoord[16] = {
			float4(0, 0.75, 0.25, 1),
			float4(0, 0.5, 0.25, 0.75),
			float4(0, 0.25, 0.25, 0.5),
			float4(0, 0, 0.25, 0.25),
				
			float4(0.25, 0.75, 0.5, 1),
			float4(0.25, 0.5, 0.5, 0.75),
			float4(0.25, 0.25, 0.5, 0.5),
			float4(0.25, 0, 0.5, 0.25),

			float4(0.5, 0.75, 0.75, 1),
			float4(0.5, 0.5, 0.75, 0.75),
			float4(0.5, 0.25, 0.75, 0.5),
			float4(0.5, 0, 0.75, 0.25),
	
			float4(0.75, 0.75, 1, 1),
			float4(0.75, 0.5, 1, 0.75),
			float4(0.75, 0.25, 1, 0.5),
			float4(0.75, 0, 1, 0.25)
		};

		int4 index = clamp(floor(Mask * 255), 0, 16);
		fixed4 coord1 = maskCoord[index.x];
		fixed4 coord2 = maskCoord[index.y];
		fixed4 coord3 = maskCoord[index.z];
		fixed4 coord4 = maskCoord[index.w];

		float4 surf1 = tex2D(_Surf0,i.uv[0].xy);
		float4 surf2 = tex2D(_Surf1,i.uv[1].xy);
		float4 surf3 = tex2D(_Surf2,i.uv[2].xy);
		float4 surf4 = tex2D(_Surf3,i.uv[3].xy);

		float MaskScale = 128;
		//float SizeInv = 1.0 / MaskScale;
		//float MaskTextureSize = SizeInv;

		//float2 CurrentMaskGrid = floor(i.uv[4].xy / MaskTextureSize); 
		// = floor(i.uv[4].xy / (1.0 / MaskScale));
		// = floor(i.uv[4].xy * MaskScale)
		float2 CurrentMaskGrid = floor(i.uv[4].xy * MaskScale);
	
		//float2 lay4UV = (i.uv[4].xy - CurrentMaskGrid * MaskTextureSize) * MaskScale * float2(0.25, 0.25)
		// = (i.uv[4].xy - CurrentMaskGrid * (1.0 / MaskScale)) * MaskScale * float2(0.25, 0.25) =
		// = (i.uv[4].xy * MaskScale - CurrentMaskGrid * (1.0 / MaskScale) * MaskScale) * float2(0.25, 0.25) =
		// = (i.uv[4].xy * MaskScale - CurrentMaskGrid) * float2(0.25, 0.25)
		float2 lay4UV = (i.uv[4].xy * MaskScale - CurrentMaskGrid) * float2(0.25, 0.25);

		fixed4 c = fixed4(surf1.rgb, 1);
		/*if (index.x != 0)
		{
			float4 channelA = tex2D(_Splat0, lay4UV + coord1.xy);
			c.rgb = surf1.rgb * channelA.a;
		}*/

		//if (index.y != 0)
		{
			fixed4 channelB = tex2D(_Splat1, lay4UV + coord2.xy);
			c.rgb = lerp(c.rgb, surf2.rgb, channelB.a * (1 - step(0, -index.y)));
			//c.rgb = lerp(c.rgb, channelB.rgb, channelB.a * (1 - step(0, -index.y)));
		}

		//if (index.z != 0)
		{
			fixed4 channelC = tex2D(_Splat2, lay4UV + coord3.xy);
			c.rgb = lerp(c.rgb, surf3.rgb, channelC.a * (1 - step(0, -index.z)));
			//c.rgb = lerp(c.rgb, channelC.rgb, channelC.a * (1 - step(0, -index.z)));
		}

		//if (index.w != 0)
		{
			fixed4 channelD = tex2D(_Splat3, lay4UV + coord4.xy);
			c.rgb = lerp(c.rgb, surf4.rgb, channelD.a * (1 - step(0, -index.w)));
			//c.rgb = lerp(c.rgb, channelD.rgb, channelD.a * (1 - step(0, -index.w)));
		}

		c.w = 0;

		return c;
		}
		ENDCG
	}
	}
}
