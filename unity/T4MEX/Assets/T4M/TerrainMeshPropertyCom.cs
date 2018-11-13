using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MapData;
using System;

[ExecuteInEditMode]
public class TerrainMeshPropertyCom : MonoBehaviour
{
    //正在使用的混合图的RGBA通道索引
    public Vector4[] UsingChannels;
    //使用的索引数量 1、2、3、4、5、6、7、8、12
    public int usingChannelsNumber;

    public Vector4 LocalPosition = Vector4.zero;

    public Bounds mSourceBounds;


    public void SetUseMaterial(Material mat)
    {
        MeshRenderer md = GetComponent<MeshRenderer>();
        if (md != null)
        {
            md.sharedMaterial = mat;
        }
        else
        {
            Debug.LogError("SetUseMaterial error ");
        }
    }


   

    private void Start()
    {
#if UNITY_EDITOR
        SetMaterialProperty();
#endif
    }
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SetMaterialProperty();
        }
#endif
    }
    private void SetMaterialProperty()
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            return;
        }

        MaterialPropertyBlock matPb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(matPb);
        matPb.SetVector(ShaderParameters.TerrainLocalPosition, LocalPosition);
        if (UsingChannels.Length == 1)
        {
            matPb.SetVector(ShaderParameters.ChannelsVector4_0, (UsingChannels[0] - Vector4.one));
        }
        else if (UsingChannels.Length == 2)
        {
            matPb.SetVector(ShaderParameters.ChannelsVector4_0, UsingChannels[0] - Vector4.one);
            matPb.SetVector(ShaderParameters.ChannelsVector4_1, UsingChannels[1] - Vector4.one);
        }
        else if (UsingChannels.Length == 3)
        {
            matPb.SetVector(ShaderParameters.ChannelsVector4_0, UsingChannels[0] - Vector4.one);
            matPb.SetVector(ShaderParameters.ChannelsVector4_1, UsingChannels[1] - Vector4.one);
            matPb.SetVector(ShaderParameters.ChannelsVector4_2, UsingChannels[2] - Vector4.one);
        }


        renderer.SetPropertyBlock(matPb);
    }
    


}