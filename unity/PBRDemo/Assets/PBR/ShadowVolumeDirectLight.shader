Shader "Rolan/Effect/Shadow"
{
	Properties
	{
		_Color("Color Tint", color) = (1, 1, 1, 1)
		_LightDirect("Light Direct(XYZ), Floor(W)", Vector) = (6, 27, 14, 0)
		_Weakness("Weakness", Range(0.1, 10)) = 0.5
	}
	SubShader{   
		Tags{ "Queue" = "Transparent" }
		Pass{
		Stencil
		{
		  Ref 1
		  Comp Greater
		  Pass Replace
		  }
		  Blend SrcAlpha OneMinusSrcAlpha
		  CULL Back
		 
		  CGPROGRAM
		  #pragma vertex vert
          #pragma fragment frag
          #include "UnityCG.cginc"
          #pragma target 3.0 

          struct VertexInput
				{
				float4 vertex : POSITION;
				};

				struct VertexOutput
				{
					float4 vertex : SV_POSITION;
					float4 dis : TEXCOORD1;
				};

				float4 _Color;
				float4 _LightDirect;
				float _Weakness;
				VertexOutput vert(VertexInput v)
				{
		            VertexOutput o = (VertexOutput) 0;
					float4 vertex = mul(unity_ObjectToWorld, v.vertex);
					float3 lightDir = _LightDirect.xyz;

					float k = max(0, ((vertex.y - _LightDirect.w) / lightDir.y));
					o.dis.xyz = lightDir * k;
					o.dis.w = sign(k) * _Weakness / (length(o.dis.xyz) + _Weakness);
					vertex.xyz -= o.dis.xyz;
					vertex.xyz -= lightDir * (max(0, vertex.y - _LightDirect.w) / lightDir.y);
					o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, vertex));
					return o;
					}

				float4 frag(VertexOutput i) : SV_Target
				{
					_Color.a *= i.dis.w;
					return _Color;
				}
					ENDCG
			}
		}
	FallBack "Diffuse"
}
