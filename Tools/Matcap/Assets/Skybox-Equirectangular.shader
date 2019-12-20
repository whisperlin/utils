Shader "Skybox/Equirectangular 360" {
Properties {
	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
	[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
	_Rotation ("Rotation", Range(0, 360)) = 0
	_MainTex("Texture", 2D) = "white" {}
	//[NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Off ZWrite Off

	Pass {
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0

		#include "UnityCG.cginc"

		//samplerCUBE _Tex;
		sampler2D _MainTex;
		half4 _MainTex_HDR;
		half4 _Tint;
		half _Exposure;
		float _Rotation;


		inline float2 ToRadialCoords(float3 coords)
		{
			float3 normalizedCoords = normalize(coords);
			float latitude = acos(normalizedCoords.y);
			float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
			float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
			return float2(0.5, 1.0) - sphereCoords;
		}
		float3 RotateAroundYInDegrees (float3 vertex, float degrees)
		{
			float alpha = degrees * UNITY_PI / 180.0;
			float sina, cosa;
			sincos(alpha, sina, cosa);
			float2x2 m = float2x2(cosa, -sina, sina, cosa);
			return float3(mul(m, vertex.xz), vertex.y).xzy;
		}
		
		struct appdata_t {
			float4 vertex : POSITION;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float3 texcoord : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		v2f vert (appdata_t v)
		{
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
			o.vertex = UnityObjectToClipPos(rotated);
			o.texcoord = v.vertex.xyz;
			return o;
		}

		fixed4 frag (v2f i) : SV_Target
		{
			float2 uv = ToRadialCoords(i.texcoord);
			half4 tex = tex2D(_MainTex, uv);
			//half4 tex = texCUBE (_Tex, i.texcoord);
			half3 c = DecodeHDR (tex, _MainTex_HDR);
			c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
			c *= _Exposure;
			return half4(c, 1);
		}
		ENDCG 
	}
} 	


Fallback Off

}
