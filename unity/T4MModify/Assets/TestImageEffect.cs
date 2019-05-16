using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestImageEffect : MonoBehaviour {

    public Material _Material;
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material)
        {
            Graphics.Blit(source, destination, _Material);
        }
    }

 
}
