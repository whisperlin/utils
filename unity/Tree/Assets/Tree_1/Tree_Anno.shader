Shader "Unlit/Tree_Anno"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 NORMAL : NORMAL;
				float2 uv : TEXCOORD0;
				float4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 v_TEXCOORD0In : TEXCOORD1;
				float4 v_TEXCOORD1In : TEXCOORD2;
				float4 v_TEXCOORD2In : TEXCOORD3;
				float4 v_TEXCOORD14In : TEXCOORD4;
				float4 v_TEXCOORD5In : TEXCOORD5;
				float4 v_TEXCOORD6In : TEXCOORD6;
				float4 v_TEXCOORD7In : TEXCOORD7;
				float4 v_TEXCOORD8In : TEXCOORD8;
				float4 v_TEXCOORD9In : TEXCOORD9;
				float4 v_TEXCOORD11In : TEXCOORD10;
				float4 v_TEXCOORD12In : TEXCOORD11;
				float4 v_TEXCOORD13In : TEXCOORD12;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _MainTex;
			//float4 _MainTex_ST;

			//global constant
			float3 cDiffuseColor;
			float cAlphaRef;
			float cTrunkWindDeflection;
			float cLeafWindDeflection;

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);



				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				//fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return fixed(1).xxxx;
			}
			ENDCG
		}
	}
}
