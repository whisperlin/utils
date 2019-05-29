Shader "TA/MobileBloom" {

	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BlurTex("Blur", 2D) = "white"{}
	}

	CGINCLUDE
	#include "UnityCG.cginc"  

		struct v2f_threshold
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f_blur
	{
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;
		float4 uv45 : TEXCOORD3;
	};
	struct v2f_blur3
	{
		float4 pos : SV_POSITION;
		float4 uv01 : TEXCOORD0;
		float4 uv23 : TEXCOORD1;
		float4 uv45 : TEXCOORD2;
		float4 uv67 : TEXCOORD3;
	};

	struct v2f_bloom
	{
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
	};

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	sampler2D _BlurTex;
	float4 _BlurTex_TexelSize;
	float4 _offsets;
	half4 _colorThreshold;
	half4 _bloomColor;
	half _bloomFactor;
	half samplerScale;

	v2f_threshold vert_threshold(appdata_img v)
	{
		v2f_threshold o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		#if UNITY_UV_STARTS_AT_TOP  
			if (_MainTex_TexelSize.y < 0)
				o.uv.y = 1 - o.uv.y;
			else
				o.uv.y = o.uv.y;
		#endif    
		return o;
	}

	fixed4 frag_threshold(v2f_threshold i) : SV_Target
	{
		fixed4 color = tex2D(_MainTex, i.uv);
		color = saturate(color - _colorThreshold);
		//color = pow(color,2);
		return color;
	}

	v2f_blur vert_blur(appdata_img v)
	{
		v2f_blur o;
		_offsets *= _MainTex_TexelSize.xyxy;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;

		o.uv01 = v.texcoord.xyxy + _offsets.xyxy * float4(1, 1, -1, -1);
		o.uv23 = v.texcoord.xyxy + _offsets.xyxy * float4(1, 1, -1, -1) * 2.0;
		o.uv45 = v.texcoord.xyxy + _offsets.xyxy * float4(1, 1, -1, -1) * 3.0;

		return o;
	}

	fixed4 frag_blur(v2f_blur i) : SV_Target
	{
		fixed4 color = fixed4(0,0,0,0);
		color += 0.40 * tex2D(_MainTex, i.uv);
		color += 0.15 * tex2D(_MainTex, i.uv01.xy);
		color += 0.15 * tex2D(_MainTex, i.uv01.zw);
		color += 0.10 * tex2D(_MainTex, i.uv23.xy);
		color += 0.10 * tex2D(_MainTex, i.uv23.zw);
		color += 0.05 * tex2D(_MainTex, i.uv45.xy);
		color += 0.05 * tex2D(_MainTex, i.uv45.zw);
		return color;
	}


	v2f_blur vert_blur2(appdata_img v)
	{
		v2f_blur o;
	 
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		//samplerScale = samplerScale+0.5;
		o.uv01.xyzw = v.texcoord.xyxy +  (float4(1, 1, 1, -1)  * _MainTex_TexelSize.xyxy * samplerScale );
		o.uv23.xyzw = v.texcoord.xyxy +  (float4(-1, -1, -1, 1) * _MainTex_TexelSize.xyxy * samplerScale ) ;
		 

		return o;
	}

	fixed4 frag_blur2(v2f_blur i) : SV_Target
	{
		fixed4 color = fixed4(0,0,0,0);
		color +=0.5 * tex2D(_MainTex, i.uv);
		color += 0.125 * tex2D(_MainTex, i.uv01.xy);
		color += 0.125 * tex2D(_MainTex, i.uv01.zw);
		color += 0.125 * tex2D(_MainTex, i.uv23.xy);
		color += 0.125 * tex2D(_MainTex, i.uv23.xy);

 
		return color;
	}



	v2f_blur3 vert_blur3(appdata_img v)
	{
		v2f_blur3 o;
	 
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
 
		//samplerScale = samplerScale+0.5;
		o.uv01.xyzw = v.texcoord.xyxy +  (float4(0.5, 0.5, 0.5, -0.5)  * _MainTex_TexelSize.xyxy * samplerScale );
		o.uv23.xyzw = v.texcoord.xyxy +  (float4(-0.5, -0.5, -0.5, 0.5) * _MainTex_TexelSize.xyxy * samplerScale ) ;
		 

		o.uv45.xyzw = v.texcoord.xyxy +  (float4(0, 1, 1, 0)  * _MainTex_TexelSize.xyxy * samplerScale );
		o.uv67.xyzw = v.texcoord.xyxy +  (float4(0, -1, -1, 0) * _MainTex_TexelSize.xyxy * samplerScale ) ;
		return o;
	}

	fixed4 frag_blur3(v2f_blur3 i) : SV_Target
	{
		fixed4 color = fixed4(0,0,0,0); 
 
		color += 0.1666666667 * tex2D(_MainTex, i.uv01.xy);
		color += 0.1666666667 * tex2D(_MainTex, i.uv01.zw);
		color += 0.1666666667 * tex2D(_MainTex, i.uv23.xy);
		color += 0.1666666667 * tex2D(_MainTex, i.uv23.xy);


		color += 0.0833333333 * tex2D(_MainTex, i.uv45.xy);
		color += 0.0833333333 * tex2D(_MainTex, i.uv45.zw);
		color += 0.0833333333 * tex2D(_MainTex, i.uv67.xy);
		color += 0.0833333333 * tex2D(_MainTex, i.uv67.xy);

 
		return color;
	}

	 

	//sum += texture2D(inputImageTexture, blurCoordinates[0]) * 0.204164;
	//sum += texture2D(inputImageTexture, blurCoordinates[1]) * 0.304005;
	//sum += texture2D(inputImageTexture, blurCoordinates[2]) * 0.304005;
	//sum += texture2D(inputImageTexture, blurCoordinates[3]) * 0.093913;
	//sum += texture2D(inputImageTexture, blurCoordinates[4]) * 0.093913;


		v2f_bloom vert_bloom(appdata_img v)
	{
		v2f_bloom o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = v.texcoord.xy;
		o.uv1.xy = o.uv.xy;
#if UNITY_UV_STARTS_AT_TOP  
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1 - o.uv.y;
		else
			o.uv.y = o.uv.y;
#endif    
		return o;
	}

	fixed4 frag_bloom(v2f_bloom i) : SV_Target
	{
		fixed4 ori = tex2D(_MainTex, i.uv1);
	fixed4 blur = tex2D(_BlurTex, i.uv);
	fixed4 final = ori + _bloomFactor * blur * _bloomColor;
	return final;
	}

		ENDCG

		SubShader
	{
		Pass
		{
			ZTest Off
			Cull Off
			ZWrite Off
			Fog{ Mode Off }

			CGPROGRAM
#pragma vertex vert_threshold  
#pragma fragment frag_threshold  
			ENDCG
		}

			Pass
		{
			ZTest Off
			Cull Off
			ZWrite Off
			Fog{ Mode Off }

			CGPROGRAM
#pragma vertex vert_blur2  
#pragma fragment frag_blur2  
			ENDCG
		}

			Pass
		{

			ZTest Off
			Cull Off
			ZWrite Off
			Fog{ Mode Off }

			CGPROGRAM
#pragma vertex vert_bloom  
#pragma fragment frag_bloom  
			ENDCG
		}


		Pass
		{
			ZTest Off
			Cull Off
			ZWrite Off
			Fog{ Mode Off }

			CGPROGRAM
#pragma vertex vert_blur3  
#pragma fragment frag_blur3  
			ENDCG
		}

	}
}