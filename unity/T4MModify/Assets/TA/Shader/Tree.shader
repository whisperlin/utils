 

Shader "TA/Scene/Tree"
{
	Properties
	{
		_MainTex ("主贴图", 2D) = "white" {}
		_Color("颜色",Color) = (1,1,1,1)
		_AlphaCut("半透明剔除",Range(0,1))=0.2
		_Wind("风向",Vector) = (1,0.5,0,0)
		_Speed("速度",Range(0,5)) = 2
		_Ctrl("空间各向差异",Range(0,3.14)) = 0

		_Emission("自发光",Range(0,3)) = 0.5 
		_EmissionTex("自发光控制图",2D)  = "white" {}


		
		[KeywordEnum(Off,On)] _fadePhysics("是否开启碰撞交互", Float) = 0
		//[Toggle(_ENABLE_BILLBOARD_Y)] _ENABLE_BILLBOARD_Y("是否开启公告版", Float) = 0
	}

	SubShader
	{
		Cull  Off
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma   multi_compile  _  ENABLE_NEW_FOG
			#pragma   multi_compile  _  _POW_FOG_ON
			#pragma   multi_compile  _  _HEIGHT_FOG_ON
			#pragma   multi_compile  _  GLOBAL_ENV_SH9
			#pragma multi_compile _FADEPHY_OFF _FADEPHYSICS_ON

			#pragma multi_compile_instancing
			//#pragma shader_feature _ENABLE_BILLBOARD_Y

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
			#include "grass.cginc"
			
			 
			
			fixed4 frag (v2f i) : SV_Target
			{

				UNITY_SETUP_INSTANCE_ID(i);

				fixed4 c = tex2D(_MainTex, i.uv);
				c.rgb *= _Color.rgb;
#if !defined(LIGHTMAP_OFF) || defined(LIGHTMAP_ON)
				fixed4 e = tex2D(_EmissionTex, i.uv);
				fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2));
				c.rgb *= lm  + _Emission*e.b;
#else
				fixed4 e = tex2D(_EmissionTex, i.uv);
				half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				half nl = saturate(dot(i.normalWorld, lightDir)) + saturate(dot(i.normalWorld, -lightDir)) ;
				c.rgb = ( UNITY_LIGHTMODEL_AMBIENT + _LightColor0 * nl + _Emission*e.b) * c.rgb;
#endif
				//return i.color;
				clip(c.a - _AlphaCut);
				APPLY_HEIGHT_FOG(c,i.wpos,i.normalWorld,i.fogCoord);
				UNITY_APPLY_FOG_MOBILE(i.fogCoord, c);
				//return i.dis;
				return c;
			}
			ENDCG
		}

		Pass
		{
			Name "ShadowCaster"
			Tags{"LightMode" = "ShadowCaster"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing
			#include "UnityCG.cginc" 
			#include "grass.cginc"
			struct v2fsd
			{
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				V2F_SHADOW_CASTER;
			};
		 
			v2f vert(appdata_full  v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);


				float4 wpos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
		 
				grass_move(wpos, v.color);
				v.vertex =   mul(wpos, unity_WorldToObject);
				o.uv = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
	 
				return o;
			}
			float4  frag(v2f i) :SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				clip(c.a - _AlphaCut);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	Fallback "Mobile/Diffuse"
}