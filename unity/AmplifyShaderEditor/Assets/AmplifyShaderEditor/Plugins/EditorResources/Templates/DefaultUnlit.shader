Shader /*ase_name*/ "ASETemplateShaders/DefaultUnlit" /*end*/
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		/*ase_props*/
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase" /*ase_tags*/}
		LOD 100
		Cull Off
		/*ase_pass*/

		Pass
		{
			CGPROGRAM
			#pragma target 3.0 
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			/*ase_pragma*/

			struct appdata
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				/*ase_vdata:p=p;uv0=tc0.xy;uv1=tc1.xy*/
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 texcoord : TEXCOORD0;
				/*ase_interp(1,7):sp=sp.xyzw;uv0=tc0.xy;uv1=tc0.zw*/
			};

			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			/*ase_globals*/
			
			v2f vert ( appdata v /*ase_vert_input*/)
			{
				v2f o;
				o.texcoord.xy = v.texcoord.xy;
				o.texcoord.zw = v.texcoord1.xy;
				
				// ase common template code
				/*ase_vert_code:v=appdata;o=v2f*/
				
				o.vertex.xyz += /*ase_vert_out:Local Vertex;Float3*/ float3(0,0,0) /*end*/;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i /*ase_frag_input*/) : SV_Target
			{
				fixed4 myColorVar;
				// ase common template code
				/*ase_frag_code:i=v2f*/
				
				myColorVar = /*ase_frag_out:Frag Color;Float4*/fixed4(1,0,0,1)/*end*/;
				return myColorVar;
			}
			ENDCG
		}
	}
}
