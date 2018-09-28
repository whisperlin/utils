using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class TurnOnDepthBuffer : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
    }
}