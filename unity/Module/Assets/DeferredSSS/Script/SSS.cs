using UnityEngine;
using System.Collections;

public class SSS: MonoBehaviour
{
    public Material mat;
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        mat.SetFloat("_InvWidth",1.0f/ src.width);
        mat.SetFloat("_InvHeight", 1.0f / src.height);
        Graphics.Blit(src, dest);
        Graphics.Blit(src, dest, mat);
    }
}