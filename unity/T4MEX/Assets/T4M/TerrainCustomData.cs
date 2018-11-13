using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCore;

[ExecuteInEditMode]
public class TerrainCustomData : MonoBehaviour
{

    public Matrix4x4 LocalToWorldMatrix;

    public Material[] MaterialArray;

    private string ChannelCountName = "UseChannelCount";

    private void OnEnable()
    {
        Shader.SetGlobalMatrix("_TerrainLToWMatrix", LocalToWorldMatrix);

        if (MaterialArray != null)
        {
            for (int i = 0; i < MaterialArray.Length; i++)
            {
                string keyName = ChannelCountName + (i + 1);
                //Shader.DisableKeyword(ChannelCountName + (i + 1));
                for (int j = 0; j < MaterialArray.Length; j++)
                {
                    if (j <= i)
                    {
                        MaterialArray[i].EnableKeyword(ChannelCountName + (j + 1));
                    }
                    else
                    {
                        MaterialArray[i].DisableKeyword(ChannelCountName + (j + 1));
                    }
                }
            }
        }

        Shader.DisableKeyword("UseChannelCount3");
    }

    [Button("SetMatrix"), Click("SetMatrix")]
    public string testMatrix;

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SetMatrix();
        }
#endif
    }

    public void SetMatrix()
    {
        LocalToWorldMatrix = transform.localToWorldMatrix;
        Shader.SetGlobalMatrix("_TerrainLToWMatrix", LocalToWorldMatrix);

        Shader.SetGlobalVector("_TerrainBeginPos", transform.position);
    }

    public void SetLocalToWorldMtr(Matrix4x4 mtr)
    {
        LocalToWorldMatrix = mtr;
    }


}
