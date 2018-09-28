Shader "GPUSkinning/GPUSkinning_Unlit_Skin4_matcap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _huanjing ("huanjing", 2D) = "white" {}
        _qiehuan ("qiehuan", Range(0, 2)) = 0
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "GPUSkinningInclude.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float4 uv2 : TEXCOORD1;
		float4 uv3 : TEXCOORD2;
		float3 normal : NORMAL;

		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float3 normalDir : TEXCOORD1;
		float4 vertex : SV_POSITION;
	};

	sampler2D _MainTex;
	sampler2D _huanjing;
	float4 _MainTex_ST;
	float4 _huanjing_ST;
	float  _qiehuan;

	v2f vert(appdata v)
	{
		UNITY_SETUP_INSTANCE_ID(v);

		v2f o;
		
		float4 pos = skin4(v.vertex, v.uv2, v.uv3);
		float4 nor = skin4(float4(v.normal, 0), v.uv2, v.uv3);
		o.normalDir = UnityObjectToWorldNormal(nor.xyz);
		o.vertex = UnityObjectToClipPos(pos);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	float4 MatcapColor(v2f i)
	{
        float3 normalDirection = normalize(i.normalDir);
////// Lighting:
////// Emissive:
		float4 _yanse_var = tex2D(_MainTex,i.uv);
        float node_939_if_leA = step(1.0,_qiehuan);
        float node_939_if_leB = step(_qiehuan,1.0);
        float2 node_5350 = (mul( UNITY_MATRIX_V, float4(normalDirection,0) ).rg*0.5+0.5);
        float4 _huanjing_var = tex2D(_huanjing,TRANSFORM_TEX(node_5350, _huanjing));
        
        float3 emissive = ((_yanse_var.rgb+(_yanse_var.a*lerp((node_939_if_leA*float3(0.84,0,0.05))+(node_939_if_leB*float3(0,0.2,0.7)),_yanse_var.rgb,node_939_if_leA*node_939_if_leB)))*((_huanjing_var.rgb*4.0)+0.2));
        
        return float4(emissive,1);
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = MatcapColor(i);//tex2D(_MainTex, i.uv);
		return col;
	}
	ENDCG

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile ROOTON_BLENDOFF ROOTON_BLENDON_CROSSFADEROOTON ROOTON_BLENDON_CROSSFADEROOTOFF ROOTOFF_BLENDOFF ROOTOFF_BLENDON_CROSSFADEROOTON ROOTOFF_BLENDON_CROSSFADEROOTOFF
			ENDCG
		}
	}
}
