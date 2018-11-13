using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderParameters
{
    public static readonly int BiasParameters = Shader.PropertyToID("BiasParameters");
    public static readonly string _FakeValue = ("_FakeValue");

    public static readonly string _MainTex_ST = ("_MainTex_ST");
    
    public static readonly string _RoadLine_Color = ("_RoadLine_Color");
    public static readonly string _RoadLine_Segmen_Space = ("_RoadLine_Segmen_Space");
    public static readonly string _Select_id = "_Select_id";
    public static readonly string _BuildingADProp = "_BuildingADProp";

    public static readonly string LeftVector_0 = "LeftVector_0";
    public static readonly string LeftVector_1 = "LeftVector_1";
    public static readonly string LeftVector_2 = "LeftVector_2";

    public static readonly string RightVector_0 = "RightVector_0";
    public static readonly string RightVector_1 = "RightVector_1";
    public static readonly string RightVector_2 = "RightVector_2";

    public static readonly string TerrainLocalPosition = "_TerrainLocalPosition";
    public static readonly string DirectionSelectMask1 = "_DirectionSelectMask1";
    public static readonly string DirectionSelectMask2 = "_DirectionSelectMask2";

    public static readonly string shaderPorpID_GPUSkinning_FrameIndex_PixelSegmentation = "_GPUSkinning_FrameIndex_PixelSegmentation";
    public static readonly string shaderPropID_GPUSkinning_RootMotion = "_GPUSkinning_RootMotion";
    public static readonly string shaderPropID_GPUSkinning_RootMotion_CrossFade = "_GPUSkinning_RootMotion_CrossFade";
    public static readonly string shaderPorpID_GPUSkinning_FrameIndex_PixelSegmentation_Blend_CrossFade = "_GPUSkinning_FrameIndex_PixelSegmentation_Blend_CrossFade";

    public static readonly string ChannelsVector4_0 = "ChannelsVector4_0";
    public static readonly string ChannelsVector4_1 = "ChannelsVector4_1";
    public static readonly string ChannelsVector4_2 = "ChannelsVector4_2";
}
