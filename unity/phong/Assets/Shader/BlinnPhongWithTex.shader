Shader "Custom/BlinnPhongWithTex" {
	Properties{
		//材质的颜色
		_MaterialColor("MaterialColor",Color) = (1,1,1,1)
		//镜面反射光强系数
		_SpecularStrength("Specular",Range(0.0,5.0)) = 1.0
		//光滑程度(反光度)
		_Gloss("Gloss",Range(1.0,255)) = 20
		//主贴图
		_MainTex("Main Texture",2D) = "white"{}
	}
	SubShader{
		Pass{
			Tags{ "LightingType" = "ForwardBase" }
			LOD 200

			//******开始CG着色器语言编写模块******
			CGPROGRAM
			//引入头文件
			#include "Lighting.cginc"

			//定义vertexShader函数和fragmentShader函数
			#pragma vertex vertexShader
			#pragma fragment fragmentShader

			//定义Properties中的变量
			fixed4 _MaterialColor;
			float _SpecularStrength;
			float _Gloss;
			sampler2D _MainTex;

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
				float3 worldNormal : NORMAL;//世界坐标系中的法向量
				float3 worldPos : TEXCOORD0;//世界坐标系的坐标
				float2 uv : TECOORD1;//转换后（缩放+采样偏移）的纹理坐标
			};

			//定义顶点着色器
			vertexShaderOutput vertexShader(vertexShaderInput v) {
				vertexShaderOutput o;//顶点着色器的输出

				//把顶点从局部坐标系转到世界坐标系再转到视口坐标系
				o.pos = UnityObjectToClipPos(v.vertex);

				//把法线转换到世界空间
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);

				//把顶点从局部坐标系转到世界坐标系
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				//转换uv
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}

			//定义片段着色器
			fixed4 fragmentShader(vertexShaderOutput i) : SV_Target{
				//环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				/* 归一化法线，不能在顶点着色器中归一化后传进来，
				因为从顶点着色器到片段着色器有差值处理，
				传入的归一化法线并不是从顶点着色器直接传出的 */
				fixed3 worldNormal = normalize(i.worldNormal);

				//把光照方向归一化
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

				//计算漫反射光照强度
				/* 半兰伯特光照：将原来[-1,1]区间的光照条件转化到[0,1]区间，既保证
				了结果的正确，又整体提高了亮度，保证非受光面也能有光，而不是全黑 */
				fixed3 lambert = 0.5 *  dot(worldNormal, worldLightDir) + 0.5;
				//漫反射光照强度为lambert光强 * 光颜色
				fixed3 diffuse = lambert * _LightColor0.xyz;

				//计算镜面反射光照强度
				//计算该像素对应位置（顶点计算过后传给像素经过插值后）的观察向量v，相机坐标 - 像素位置
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				//计算半角向量（光线方向 + 视线方向，结果归一化）
				fixed3 halfDir = normalize(worldLightDir + viewDir);
				/*
				计算BlinnPhong镜面反射光照强度，其与半角向量方向和法线方向的夹角有关，
				夹角为dot(halfDir,worldNormal)，最后根据镜面反射光强系数计算反射值为
				_SpecularStrength * pow(dot(halfDir,worldNormal),Gloss)
				*/
				fixed3 specular = _LightColor0.rgb *_SpecularStrength * pow(max(0.0, dot(halfDir, worldNormal)), _Gloss);

				//纹理采样
				fixed4 tex = tex2D(_MainTex, i.uv);

				//纹理中rgb为正常颜色，a为一个高光的mask图，非高光部分a值为0，高光部分根据a的值控制高光强度
				fixed3 color = (ambient * _MaterialColor.rgb + diffuse * _MaterialColor.rgb + specular * _MaterialColor.rgb * tex.a) * tex.rgb;

				return fixed4(color, 1.0);

			}

			//*****结束CG着色器语言编写模块******
			ENDCG
		}
	}
	//前面的Shader失效的话，使用默认的Diffuse
	FallBack "Diffuse"
}
