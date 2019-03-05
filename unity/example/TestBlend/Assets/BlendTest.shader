Shader "Unlit/BlendTest"
{
	Properties
	{
		_COLOR0("_COLOR", COLOR) = (1,1,1,1)
		_COLOR1("_COLOR1", COLOR) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			float4 _COLOR0;	
			struct appdata
			{
				float4 vertex : POSITION;
 
			};

			struct v2f
			{
 
				float4 vertex : SV_POSITION;
			};

		 
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
 
 
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return _COLOR0;
			}
			ENDCG
		}

		
		Pass
			{
				Blend DstColor Zero
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
								// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"
				//Blend DstColor Zero
				float4 _COLOR1;
				struct appdata
				{
					float4 vertex : POSITION;

				};

				struct v2f
				{

					float4 vertex : SV_POSITION;
				};



				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					return o;
				}

				fixed4 frag(v2f i): SV_Target
				{
					return _COLOR1;
				}
				ENDCG
			}
	}
}
