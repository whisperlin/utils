Shader "Unlit/T4M 4 HardTexture"
{
	Properties{
		_Lightmap_Ctrl("Lightmap Ctrl", Range(0, 10)) = 1
		_Splat0("Layer1 (RGB)", 2D) = "white" {}
	_Splat1("Layer2 (RGB)", 2D) = "white" {}
	_Splat2("Layer3 (RGB)", 2D) = "white" {}
	_Splat3("Layer4 (RGB)", 2D) = "white" {}
	_Splat4("Layer5 (RGB)", 2D) = "white" {}
	_Splat5("Layer6 (RGB)", 2D) = "white" {}
	_Splat6("Layer7 (RGB)", 2D) = "white" {}
	_Splat7("Layer8 (RGB)", 2D) = "white" {}
	_Lightmap("Light map", 2D) = "white" {}
	_Mask("Mask (RGBA)", 2D) = "white" {}
	_Control("Control (RGBA)", 2D) = "white" {}
	_Control_Sec("Control_Sec (RGBA)", 2D) = "white" {}
	}
		SubShader{
		Pass{

		CGPROGRAM
#include "UnityCG.cginc"
#pragma vertex vert
#pragma target 3.0
#pragma fragment frag
#pragma multi_compile_fog
#pragma multi_compile SIX_LAYER EIGHT_LAYER
#pragma exclude_renderers xbox360 ps3
		sampler2D _Splat0;
	sampler2D _Splat1;
	sampler2D _Splat2;
	sampler2D _Splat3;
	sampler2D _Splat4;
	sampler2D _Splat5;
	sampler2D _Splat6;
	sampler2D _Splat7;

	sampler2D _Mask;
	sampler2D _Control;
	sampler2D _Control_Sec;

	sampler2D _Lightmap;

	float _Lightmap_Ctrl;

	struct v2f {
		float4  pos : SV_POSITION;
		float2  uv[3] : TEXCOORD0;

		float4 posWorld  : TEXCOORD3;
		float3 normalDir : TEXCOORD4;

		UNITY_FOG_COORDS(5)
	};

	float4 _Splat0_ST;
	float4 _Control_ST;
#ifdef LIGHTMAP_ON
	/// fixed4 unity_LightmapST;
	// sampler2D unity_Lightmap;
#endif

	uniform float TextureSize;

	v2f vert(appdata_full  v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv[0] = TRANSFORM_TEX(v.texcoord, _Splat0);
		o.uv[1] = TRANSFORM_TEX(v.texcoord, _Control);
		o.uv[2] = v.texcoord1.xy;// *unity_LightmapST.xy + unity_LightmapST.zw;

		o.normalDir = UnityObjectToWorldNormal(v.normal);
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);

		UNITY_TRANSFER_FOG(o, o.pos);

		return o;
	}

//#define Emissive(_D_var) (_D_var.rgb + (_D_var.a * (pow(0.5 * dot(normalDirection, normalize((viewDirection + _Sweizhi.rgb))) + 0.5, exp(((10.0*0.3) + 0.1)))*(_D_var.a*3.0))));
//#define Emissive(_D_var) _D_var.rgb +specularValue * _D_var.a

#define GetMaskColor(maskdiv, maskmod, axis, surf)\
	fixed2 channel = fixed2(lay4UV.x + maskdiv.axis, lay4UV.y + maskmod.axis);\
	fixed4 col = tex2D(surf, channel);\
	c.rgb = lerp(c.rgb, col.rgb, col.a);\

	float4 frag(v2f i) : COLOR
	{
		float2 Control_UV = i.uv[1].xy;
		fixed4 Mask = tex2D(_Control, Control_UV);
		fixed4 Mask1 = tex2D(_Control_Sec, Control_UV);

		fixed4 index = (Mask * 255) * 0.25;// / float4(4, 1, 1, 1);
		fixed4 frac1 = frac(index);
		fixed4 indexdiv4 = (index - frac1) * 0.25;
		fixed4 indexmod4 = (3 - frac1 * 4) * 0.25;

		fixed4 index1 = (Mask1 * 255) * 0.25;// / float4(4, 1, 1, 1);
		fixed4 frac11 = frac(index1);
		fixed4 indexdiv14 = (index1- frac11) * 0.25;
		fixed4 indexmod14 = (3 - frac11 * 4) * 0.25;
		//float4 stepIndex = (1 - step(0, -index));

		fixed3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
		float3 normalDirection = i.normalDir;

		fixed2 Control_UV_withScale = Control_UV * TextureSize;
		//float SizeInv = 1.0 / MaskScale;
		//float MaskTextureSize = SizeInv;

		//float2 CurrentMaskGrid = floor(i.uv[4].xy / MaskTextureSize); 
		// = floor(i.uv[4].xy / (1.0 / MaskScale));
		// = floor(i.uv[4].xy * MaskScale)
		fixed2 CurrentMaskGrid = floor(Control_UV_withScale);

		//float2 lay4UV = (i.uv[4].xy - CurrentMaskGrid * MaskTextureSize) * MaskScale * float2(0.25, 0.25)
		// = (i.uv[4].xy - CurrentMaskGrid * (1.0 / MaskScale)) * MaskScale * float2(0.25, 0.25) =
		// = (i.uv[4].xy * MaskScale - CurrentMaskGrid * (1.0 / MaskScale) * MaskScale) * float2(0.25, 0.25) =
		// = (i.uv[4].xy * MaskScale - CurrentMaskGrid) * float2(0.25, 0.25)
		fixed2 lay4UV = (Control_UV_withScale - CurrentMaskGrid) * fixed2(0.25, 0.25);

		//fixed2 channel1 = lay4UV + fixed2(indexdiv4.x, indexmod4.x);

		fixed3 specularValue = pow(0.5*dot(i.normalDir, normalize(viewDirection + fixed3(0, 0, -0.5))) + 0.5, 33.0);
		
		//fixed4 c = tex2D(_Splat0, i.uv[0].xy);
		fixed4 c = fixed4(0, 0, 0, 0);
		if (index.x != 0)
		{
			GetMaskColor(indexdiv4, indexmod4, x, _Splat0)
		}

		if (index.y != 0)
		{
			GetMaskColor(indexdiv4, indexmod4, y, _Splat1)
		}

		if (index.z != 0)
		{
			GetMaskColor(indexdiv4, indexmod4, z, _Splat2)
		}
		
		if (index.w != 0)
		{
			GetMaskColor(indexdiv4, indexmod4, w, _Splat3)
		}

		if (index1.x != 0)
		{
			GetMaskColor(indexdiv14, indexmod14, x, _Splat4)
		}

		if (index1.y != 0)
		{
			GetMaskColor(indexdiv14, indexmod14, y, _Splat5)
		}

		//return c;

#ifdef EIGHT_LAYER
		if (index1.z != 0)
		{
			fixed2 channel7 = fixed2(lay4UV.x + indexdiv14.z, lay4UV.y + indexmod14.z);
			fixed4 surf7 = tex2D(_Splat6, i.uv[0].xy);
			fixed4 channelC = tex2D(_Mask, channel7);
			c = lerp(c, surf7, channelC.a);// *(1 - step(0, -index.x)));
		}

		if (index1.w != 0)
		{
			fixed2 channel8 = fixed2(lay4UV.x + indexdiv14.w, lay4UV.y + indexmod14.w);
			fixed4 surf8 = tex2D(_Splat7, i.uv[0].xy);
			fixed4 channelD = tex2D(_Mask, channel8);
			c= lerp(c, surf8, channelD.a);// *(1 - step(0, -index.x)));
		}
#endif

		//c.rgb = Emissive(c);
		//c.rgb *= tex2D(_Lightmap, i.uv[2].xy) * _Lightmap_Ctrl;
		UNITY_APPLY_FOG(i.fogCoord, c);

		c.w = 0;

		

		return c;
	}
		ENDCG
	}
	}
}
