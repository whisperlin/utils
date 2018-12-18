Shader "MyTest/TestMalloc"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        // 声明需要的控件
        [Toggle(S_BOOL)] _S_BOOL("S_BOOL", Int) = 0
        [Toggle] _MyToggle1("MyToggle1", Float) = 0
        [Toggle(MyToggle2)] _MyToggle2("MyToggle2", Float) = 0
        [KeywordEnum(One, Two, Three)] _MyEnum("MyEnum", Float) = 0
    }
    SubShader
    {
        Tags{ "RenderType" = "Opaque" }
        LOD 200
		CGINCLUDE
		//some code
		END
        CGPROGRAM

        #pragma surface surf Lambert addshadow

        // 创建变量，用来接收控件的值
        #pragma shader_feature S_BOOL
        #pragma shader_feature S_OUTSIDE_TEST
        #pragma shader_feature _MYTOGGLE1_ON
        #pragma shader_feature MyToggle2
        #pragma multi_compile _MYENUM_ONE _MYENUM_TWO _MYENUM_THREE

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            #if S_BOOL
            o.Albedo.gb *= 0.5;
            #endif

            //#if _MYTOGGLE1_ON
            //o.Albedo.gb *= 0.5;
            //#endif

            //#if MyToggle2
            //o.Albedo.gb *= 0.5;
            //#endif


            #if S_OUTSIDE_TEST
            o.Albedo.gb = fixed3(1,0,0);
            #endif



           
        }

        ENDCG
    }
}