using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Camera))]
[CanEditMultipleObjects]
public class CutCamera : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("截图"))
        {
            Camera camera = (Camera) target;

            if (camera.targetTexture != null)
            {
                RenderTexture rt = camera.targetTexture;
                camera.Render();
                RenderTexture.active = rt;

                Debug.Log(RenderTexture.active);

                Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

                texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                texture.Apply();

                byte[] bytes = texture.EncodeToPNG();
                Texture2D.DestroyImmediate(texture);

                string path = Application.dataPath + "/test.png";
                File.WriteAllBytes(path, bytes);
            }
        }

        base.OnInspectorGUI();
    }
}
