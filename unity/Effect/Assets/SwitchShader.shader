Shader "Unlit/Switch"
{
	Properties
	{

		_MainTex("贴图1", 2D) = "white" {}
		_MainTex2("贴图2", 2D) = "white" {}
		_AnimMark("动画贴图", 2D) = "black" {}
		_BackgroundColor("背景色", Color) = (0,0,0,1)


		_Lan("Lan",Float) = 10
		_Progress("进度",Range(0.0,1.0)) = 1.0

		[KeywordEnum(BYC, NORMAL,BROKEN,ANIM,FY,FY2)] _SWITCH("切换类型", Float) = 0

		[KeywordEnum(TYPE1, TYPE2)] _DIR("切换方向", Float) = 0

	 
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

	 

		//Cull Off
		Lighting Off
  
		 

		Pass
		{
			Name "Default"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

 
			#pragma multi_compile _SWITCH_BYC _SWITCH_NORMAL _SWITCH_BROKEN _SWITCH_ANIM _SWITCH_FY _SWITCH_FY2
			#pragma multi_compile _DIR_TYPE1 _DIR_TYPE2
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata_t v)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = v.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = v.texcoord;

				OUT.color = v.color;
				return OUT;
			}

			sampler2D _MainTex; uniform float4 _MainTex_ST;
			sampler2D _MainTex2;; uniform float4 _MainTex2_ST;
			sampler2D _AnimMark; uniform float4 _AnimMark_ST;
			uniform float4 _BackgroundColor;

			float _Lan;
			float _Progress;

			float4 frag(v2f IN): SV_Target
			{

		#ifdef _SWITCH_BYC

			#ifdef _DIR_TYPE1
				 float2 uv = IN.texcoord;
				uv.x *= _Lan;
				uv.x = frac(uv.x);
				int needDiscard = step(_Progress,uv.x);

				float x1 = uv.x;

				uv = TRANSFORM_TEX(IN.texcoord, _MainTex2);
				uv += float2(-0.5,-0.5);
				uv.x -= x1;
				float2x2 qiebian = float2x2(1,0,(1.0 - _Progress),1);
				uv = mul(qiebian,uv);
				uv -= float2(-0.5,-0.5);
				uv.x += x1;

				float4 color1 = tex2D(_MainTex, TRANSFORM_TEX(IN.texcoord, _MainTex));
				float4 color2 = tex2D(_MainTex2, uv);
				color2 = lerp(color2, color1, needDiscard);
				return color2;
			#else
				float2 uv = IN.texcoord;
				uv.y *= _Lan;
				uv.y = frac(uv.y);
				int needDiscard = step(_Progress, uv.y);
				float y1 = uv.y;
				uv = TRANSFORM_TEX(IN.texcoord, _MainTex2);
				uv += float2(-0.5, -0.5);
				uv.y -= y1;
				float2x2 qiebian = float2x2(1, (1.0 - _Progress), 0.0, 1);
				uv = mul(qiebian, uv);
				uv -= float2(-0.5, -0.5);
				uv.y += y1;

				float4 color1 = tex2D(_MainTex, TRANSFORM_TEX(IN.texcoord, _MainTex));
				float4 color2 = tex2D(_MainTex2, uv);
				color2 = lerp(color2, color1, needDiscard);
				return color2;
			#endif

		#elif _SWITCH_NORMAL

			#ifdef _DIR_TYPE1
				float2 uv0 = IN.texcoord - float2(_Progress, 0);
				float t = step(uv0.x, 0);
			#else
				float2 uv0 = IN.texcoord - float2(0, _Progress);
				float t = step(uv0.y, 0);
			#endif
				float4 color1 = tex2D(_MainTex, TRANSFORM_TEX(uv0, _MainTex));
				float4 color2 = tex2D(_MainTex2, TRANSFORM_TEX(uv0, _MainTex2));
				return color2*t + (1 - t)*color1;

		#elif _SWITCH_BROKEN

			float2 uv0 = IN.texcoord;
			float4 _AnimMark_var = tex2D(_AnimMark, TRANSFORM_TEX(uv0, _AnimMark));
			_AnimMark_var.r = _AnimMark_var.r*0.9 + 0.1;
			float4 color1 = tex2D(_MainTex, TRANSFORM_TEX(uv0, _MainTex));
			float4 color2 = tex2D(_MainTex2, TRANSFORM_TEX(uv0, _MainTex2));
			float t = step(_AnimMark_var.r, _Progress);
			return color2*t + (1 - t)*color1;

		#elif _SWITCH_ANIM
			float2 uv0 = IN.texcoord;
			float4 _AnimMark_var = tex2D(_AnimMark, TRANSFORM_TEX(uv0, _AnimMark));
			float t = (_AnimMark_var.r + 1)*(1 - _Progress);
			float2 uv1 = uv0;
			uv1.x += t;
			float4 color1 = tex2D(_MainTex, TRANSFORM_TEX(uv0, _MainTex));
			float4 color2 = tex2D(_MainTex2, TRANSFORM_TEX(uv1, _MainTex2));

			if (uv1.x > 1)
				return color1;
			return color2;

		#elif _SWITCH_FY

			float2 uv1 = IN.texcoord;
			float2 uv0 = uv1;

			float4 color2 = tex2D(_MainTex2, TRANSFORM_TEX(uv1, _MainTex2));
			float a = IN.texcoord.x;
			float b = IN.texcoord.y;
			float s = _Progress * 2;
			float t = step(a + b, s);
			float t1 = step(a,s)*step(b,s);
			float y0 = s - a;
			float x0 = a - b + y0;
			uv0 = t1*float2(x0, y0) + (1 - t1)*uv0;
			float4 color1 = tex2D(_MainTex, TRANSFORM_TEX(uv0, _MainTex));
			return t * color2 + (1 - t)* color1;

		#elif _SWITCH_FY2
			 
			float f0 = 1 - _Progress;
			float2 uv0 = IN.texcoord;

			uv0.y /= f0 ;
			
			 
			uv0.x = (uv0.x -0.5)*    lerp(1, 2, uv0.y *_Progress*2)   + 0.5;
			float t0 = step(uv0.y, 1) *step(0,uv0.x)*step( uv0.x,1);
			float4 color1 = tex2D(_MainTex, TRANSFORM_TEX(uv0, _MainTex));
	
			uv0 = IN.texcoord;
			uv0.y -= f0;
			uv0.y /= _Progress ;
			uv0.x = (uv0.x - 0.5)* lerp(1/_Progress,1 , uv0.y) + 0.5;
			float t1 = step(0,uv0.y )*step(0, uv0.x)*step(uv0.x, 1);
			float4 color2 = tex2D(_MainTex2, TRANSFORM_TEX(uv0, _MainTex2));
			 
			float tf = step(_Progress , 0.5);
			float t = t0*tf + t1*(1-tf);
			float4 final = tf*color1 + (1 - tf)*color2;
			return _BackgroundColor *(1 - t) + final*t;
			
		#endif

				
				 
		
			}
			ENDCG
		}
	}
}
