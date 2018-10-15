Shader "Custom/DiffuseWithTex_HLP" {
	Properties {
		//材质的颜色
		_Diffuse("Diffuse",Color) = (1,1,1,1)
		//主贴图
		_MainTex("Texture",2D) = "white"{}
	}
	SubShader{
		Pass{
			Tags{ "RenderType" = "Opaque" }
			LOD 200

			//******开始CG着色器语言编写模块******
			CGPROGRAM
			//引入头文件
			#include "Lighting.cginc"

			//定义Properties中的变量
			fixed4 _Diffuse;//材质颜色
			sampler2D _MainTex;//主贴图

			//使用TRANSFROM_TEX宏就需要定义XXX_ST
			float4 _MainTex_ST;

			//定义结构体：顶点着色器阶段输入的数据
			struct vertexShaderInput {
				float4 vertex : POSITION;//顶点坐标
				float3 normal : NORMAL;//法向量
				float4 texcoord : TEXCOORD0;//纹理坐标
			};

			//定义结构体: 顶点着色器阶段输出的内容
			struct vertexShaderOutput {
				float4 pos : SV_POSITION;//顶点视口坐标系的坐标
				float3 worldNormal : TEXCOORD0;//世界坐标系中的法向量
				float2 uv : TEXCOORD1;//转换后的纹理坐标
			};

			//定义顶点着色器
			vertexShaderOutput vertexShader(vertexShaderInput v) {
				vertexShaderOutput o;//顶点着色器的输出

				//把顶点从局部坐标系转到世界坐标系再转到视口坐标系
				o.pos = UnityObjectToClipPos(v.vertex);

				//把法线转换到世界空间
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);

				/*
					通过TRANSFORM_TEX宏转化纹理坐标，主要处理了Offset和Tiling的改变，默认
					时等同于o.uv = v.texcoord.xy
				*/
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}

			//定义片段着色器
			fixed4 fragmentShader(vertexShaderOutput i) : SV_Target{
				//Unity自身的diffuse也带了环境光，在这里我们增强一下环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Diffuse.xyz;//增强后的环境光

				/* 归一化法线，不能在顶点着色器中归一化后传进来，
				因为从顶点着色器到片段着色器有差值处理，
				传入的归一化法线并不是从顶点着色器直接传出的 */
				fixed3 worldNormal = normalize(i.worldNormal);

				//把光照方向归一化
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

				/* 半兰伯特光照：将原来[-1,1]区间的光照条件转化到[0,1]区间，既保证
				了结果的正确，又整体提高了亮度，保证非受光面也能有光，而不是全黑 */
				fixed3 lambert = 0.5 *  dot(worldNormal, worldLightDir) + 0.5;

				//最终输出颜色为lambert光强 * 材质Diffuse颜色 * 光颜色 + 环境光
				fixed3 diffuse = lambert * _Diffuse.xyz * _LightColor0.xyz + ambient;

				//进行纹理采样
				fixed4 color = tex2D(_MainTex, i.uv);

				return fixed4(diffuse * color.rgb, 1.0);

			}

			//使用vertexShader函数和fragmentShader函数
			#pragma vertex vertexShader
			#pragma fragment fragmentShader

			//*****结束CG着色器语言编写模块******
			ENDCG
		}
	}
	//前面的Shader失效的话，使用默认的Diffuse
	FallBack "Diffuse"
}
