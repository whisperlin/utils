using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class ShaderFinder
{
    [MenuItem("Tools/Find_Nature_Terrain_Diffuse %Q")]
    static void Find_Nature_Terrain_Diffuse()
    {
        Find("Nature/Terrain/Diffuse");
    }

    static void Find(string name)
    {
        Renderer[] renders = GameObject.FindObjectsOfType<Renderer>();
        foreach (var render in renders)
        {
            if (render.sharedMaterials != null)
            {
                foreach (var mat in render.sharedMaterials)
                {
                    if (mat && mat.shader)
                    {
                        string shaderName = mat.shader.name;
                        if (shaderName.Equals(name))
                        {
                            Debug.Log(GetPath(render.transform));
                        }
                    }
                }
            }
        }
    }

    static string GetPath(Transform transform)
    {
        if (transform.parent)
        {
            return GetPath(transform.parent) + "/" + transform.name;
        }
        return transform.name;
    }
}
