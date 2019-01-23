#ifndef  _______________BPR_COGIC_______________________
#define  _______________BPR_COGIC_______________________




		 //#include "UnityCG.cginc"
#include "RolantinCG_REMOVE_REDEFINE.cginc"

 

// uniform float4 _Color;
//uniform sampler2D _MainTex;
//uniform float4 _MainTex_ST;
//uniform sampler2D _BumpMap;
uniform float4 _BumpMap_ST;

uniform sampler2D _MatCap;;
uniform float4 _MatCap_ST;
uniform float _MatCapFactor;


uniform samplerCUBE _IBL_Diffuse;
uniform float4 _IBL_Color;

uniform sampler2D _MetallicTex;
uniform float4 _MetallicTex_ST;
//uniform samplerCUBE _SBL;
uniform sampler2D _EmssionMap;
uniform float4 _EmssionMap_ST;
uniform float _EmissionIntensity;

//float _Cutoff;
float4 _emissioncolor;
float4 _SkinColor;
float GlobeLight;
float _emission;

uniform sampler2D _ColorMark;
uniform float4 _SubColor;

uniform float _BlinnPhongSP;
uniform float powersp;

struct VertexOutput {
        float4 pos: SV_POSITION;
        float3 normal: NORMAL;
        float4 tangent: TANGENT;
        float2 uv0: TEXCOORD0;
        float4 posWorld: TEXCOORD1;
        float3 normalDir: TEXCOORD2;
        float3 tangentDir: TEXCOORD3;
        float3 bitangentDir: TEXCOORD4;
		 #if _SHADER_LEVEL_LOW
		float4 twoUv : TEXCOORD5;//主纹理uv存xy,matCapUv存在zw,这算一种优化
		#endif
};
// vertex shader
VertexOutput vert_surf (appdata_vert v) {
	  UNITY_SETUP_INSTANCE_ID(v);
	  VertexOutput o;
	   float2 uv;
	  myvert (v, o.uv0);
	  //o.pos = UnityObjectToClipPos(v.vertex);


	   o.normalDir = UnityObjectToWorldNormal(v.normal);
        o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
        o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
        o.posWorld = mul(unity_ObjectToWorld, v.vertex);
        o.pos = UnityObjectToClipPos(v.vertex);
        o.normal =  v.normal;
        o.tangent =  v.tangent;
		
		 #if _SHADER_LEVEL_LOW
		o.twoUv.xy = TRANSFORM_TEX(v.uv0, _MainTex);
        //matcap存的其实是：法线中xy分量作为uv，对应的光照颜色信息，即用xy去采样就好，注：xy必须是归一化法线中的分量，故z才没必要
        o.twoUv.zw = UnityObjectToWorldNormal(v.normal).xy;
        o.twoUv.zw = o.twoUv.zw * 0.5 + 0.5;//(-1,1)->(0,1)
       #endif

	   
  return o;
}




void SBLAndIBL0(samplerCUBE CubeTexIBL , float3 normalDirection , float3 MetallicTex,out float3 sblColor , out float3 iblColor){	 
		  float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld);
		  float3 viewReflectDirection = reflect( - viewDirection, normalDirection);
		  //float3 CubeTex = texCUBElod(CubeTexIBL,float4(normalDirection,IBL_Blur)).rgb;
		  float4 dir = normalize(float4(normalDirection,IBL_Blur));
		   
		  float3 CubeTex = texCUBElod(CubeTexIBL ,dir).rgb;   
		  //float3 CubeTex = texCUBElod(CubeTexIBL,float4(1,0,0,1)).rgb;   
		 //float3 CubeTex =float4(1,0,0,1);
		  //final color
		  iblColor = _AmbientLight* CubeTex * IBL_Intensity;
		  sblColor = CubeTex*0.5 * SBL_Intensity * MetallicTex ;    
}

float4 frag_surf(VertexOutput i) : COLOR {

    posWorld = i.posWorld.xyz;

    float3x3 tangentTransform = float3x3(i.tangentDir, i.bitangentDir, i.normalDir);
    float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
///////Mapping                
    float3 _BumpMap_var = UnpackNormal (tex2D(_BumpMap, TRANSFORM_TEX(i.uv0, _BumpMap)));
    float3 _Emssion_var =tex2D(_EmssionMap, TRANSFORM_TEX(i.uv0, _EmssionMap));
    float3 Emission = _Emssion_var* _EmissionIntensity;
    float3 normalLocal = NormalIntensity(_BumpMap_var);
    float3 normalDirection = normalize(mul(normalLocal, tangentTransform)); // Perturbed normals
    float3 BacknormalDirection = -normalDirection; // Perturbed normals
    float3 viewReflectDirection = reflect( - viewDirection, normalDirection);
    float4 _MainTex_var = ColorSpace (tex2D(_MainTex, TRANSFORM_TEX(i.uv0, _MainTex)));

    #if S_SUB_COLOR
    	float4 _mark_color_var = tex2D(_ColorMark, TRANSFORM_TEX(i.uv0, _MainTex));
    	float3 grey = dot(_MainTex_var.rgb, float3(0.299, 0.587, 0.114));
    	_MainTex_var.rgb = lerp(grey*_SubColor.rgb,   _MainTex_var.rgb,  _mark_color_var.r);
    	//_SubColor
    	//if _mark_color_var.r >0.1 

    #endif

    #if _ENABLE_CUT
    clip(_MainTex_var.a - 0.5 *_Cutoff );
    #endif
    //_ENABLE_CUT


////// Lighting:

  
    //float3 pointlight = PointLight(normalDirection,PointLightPosition) * LightColor2 * LightIntensity2;
  
    float3 lightDirection = normalize(LightDir0);
    float3 halfDirection = normalize(viewDirection + lightDirection);
    float4x4 _DirectionalLight = DirectionalLight(normalDirection);
    float3 _DirectionalLight1 = DirectionalLight1(BacknormalDirection);

     float3 FullLight = _DirectionalLight[0];
    //float3 FullLight = _DirectionalLight[0] + _DirectionalLight[1]  +  _DirectionalLight[2];

 //   float3 SkinLight = (_DirectionalLight1 ) + PointLight(BacknormalDirection ,PointLightPosition);

    ///////// Gloss:
    float4 _Metallic_var = tex2D(_MetallicTex, TRANSFORM_TEX(i.uv0, _MetallicTex)); 
    ////// Specular:
    float3 specularColor = _Metallic_var;
    float3 diffuseColor = (_MainTex_var.rgb * _Color.rgb); // Need this for specular when using metallic
    float3 MetallicDiff;
  
//HightLigtht
	float3 indirectSpecular;
   	float3 IBLColor;
	#if defined(_DISABLE_IBL_DIFFUSE)
    	SBLAndIBLSample(_IBL_Color, normalDirection , _Metallic_var.r,indirectSpecular,IBLColor);
   #else
   		  
    	SBLAndIBL(_IBL_Diffuse, normalDirection , _Metallic_var.r,indirectSpecular,IBLColor); 
    #endif
   //return fixed4(indirectSpecular+IBLColor, 1);  
    //float3 indirectSpecular = SBL(_IBL_Diffuse, normalDirection , _Metallic_var.r);
   
  




   #if _SHADER_LEVEL_TOP
   		 float3 PbrSpecular = PBR_SP(normalDirection, normalize(-LightDir0), _DirectionalLight[0], _Metallic_var, diffuseColor, MetallicDiff) * powersp ;
   		 float3 FinalSP = (PbrSpecular + indirectSpecular );
    
   	 	
   #endif
   #if _SHADER_LEVEL_MID
   		 float3 HighLight = (Phong(LightDir2, normalDirection,_Metallic_var.r,_Metallic_var.a) )  * diffuseColor *  _BlinnPhongSP;
   		 float3 FinalSP = HighLight + indirectSpecular ;

   		 float3 specularColor0 =  _Metallic_var.r * _MetallicPower;
   		 float specularMonochrome;
		 MetallicDiff = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor0, specularColor0, specularMonochrome );
   #endif
   #if _SHADER_LEVEL_LOW
   		//_MatCap
		fixed4 mapCapCol = tex2D(_MatCap, i.twoUv.zw);
		float3 HighLight =  mapCapCol.xyz * _MatCapFactor+indirectSpecular;
   		float3 FinalSP = HighLight + indirectSpecular ;

   		 float3 specularColor0 =  _Metallic_var.r * _MetallicPower;
   		 float specularMonochrome;
		 MetallicDiff = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor0, specularColor0, specularMonochrome );
   #endif

 


    /////// Diffuse:
    //   NdotL = dot( normalDirection, LightDir1 );
//         float4 _SkinMap_var = tex2D(_SkinMap, TRANSFORM_TEX(i.uv0, _SkinMap));

  //  float3 Skin = _SkinMap_var.rgb * ( (_DirectionalLight1 ) +PointLight(BacknormalDirection ,LightDir2))  * _SkinIntensity * _SkinColor;
//       float3 Skin = _SkinMap_var.rgb *  (_DirectionalLight1 ) * _SkinIntensity * _SkinColor;
    // Light wrapping
    //  float3 NdotLWrap = NdotL * ( 1.0 - Skin );
    //  float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap +Skin );
    //  NdotL = max(0.0,dot( normalDirection, lightDirection ));
    //  half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
    //  float nlPow5 = Pow5(1-NdotLWrap);
    //  float nvPow5 = Pow5(1-NdotV);

     #if _SHADER_LEVEL_TOP
     	float3 directDiffuse = ( FullLight + IBLColor+ Emission )* MetallicDiff;
     #else
     	float3 directDiffuse = ( FullLight + IBLColor+ Emission )* MetallicDiff;
     #endif
    
    //     float3 directDiffuse = (Skin + FullLight + IBLColor+ Emission)* MetallicDiff;

    float3 pbl = (directDiffuse + FinalSP ) * GlobeLight;
    //return _MainTex_var;
    return fixed4(pbl, 1);
}

#endif