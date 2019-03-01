using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
//[ImageEffectAllowedInSceneView]
public class BloomSimple: MonoBehaviour
{
    public Material _Material;
    
    public int downSample = 1;

    public int samplerScale = 1;
    public LayerMask bloomMark;
    
 
    //Bloom泛光颜色
    public Color bloomColor = Color.white;
    //Bloom权值
    [Range(0.0f, 1.0f)]
    public float bloomFactor = 0.5f;

    public int TargetSize = 256;
    public RenderTexture bloomTarget;
    //public Camera cam;
    Camera myCamera;
    void Update()
    {
        if (null == myCamera)
            myCamera = GetComponent<Camera>();
        if (null == bloomTarget)
        {
            bloomTarget = new RenderTexture((int)(TargetSize * myCamera.aspect), TargetSize , 0, RenderTextureFormat.ARGB32);
        }
        if (bloomTarget.texelSize.y != TargetSize)
        {
            GameObject.DestroyImmediate(bloomTarget,true);
            bloomTarget = new RenderTexture((int)(TargetSize * myCamera.aspect), TargetSize, 0, RenderTextureFormat.ARGB32);
        }
        RenderTexture temp1 = RenderTexture.GetTemporary(TargetSize, TargetSize, 0,RenderTextureFormat.ARGB32);
        RenderTexture temp2 = RenderTexture.GetTemporary(TargetSize, TargetSize, 0, RenderTextureFormat.ARGB32);

        /*if (null == cam)
        {
            
            GameObject g = new GameObject("BloomCamera");
            cam = g.AddComponent<Camera>();
            cam.hideFlags = HideFlags.HideAndDontSave;
        }
        cam.CopyFrom(myCamera);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.enabled = false;
        cam.cullingMask = bloomMark;
        cam.targetTexture = bloomTarget;
        cam.transform.position = transform.position;
        cam.transform.rotation = transform.rotation;
        
        cam.Render();*/
        int oldMark = myCamera.cullingMask;
        Color oldColor = myCamera.backgroundColor;
        CameraClearFlags oldclearFlags = myCamera.clearFlags;
        myCamera.targetTexture = bloomTarget;
        myCamera.backgroundColor = Color.black;
        myCamera.clearFlags = CameraClearFlags.SolidColor;
        myCamera.cullingMask = bloomMark;
        myCamera.Render();
        myCamera.targetTexture = null;
        
       
        myCamera.cullingMask = oldMark;
        myCamera.backgroundColor = oldColor;
        myCamera.clearFlags = oldclearFlags;


        _Material.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
        Graphics.Blit(bloomTarget, temp1, _Material, 1);
        _Material.SetVector("_offsets", new Vector4(samplerScale, 0, 0, 0));
        Graphics.Blit(temp1, temp2, _Material, 1);
        Graphics.Blit(temp2, bloomTarget);
 
        RenderTexture.ReleaseTemporary(temp1);
        RenderTexture.ReleaseTemporary(temp2);




    }
    void OnDestory()
    {
        //if (null != cam)
        //    GameObject.DestroyImmediate(cam,true);
        if (null != bloomTarget)
            GameObject.DestroyImmediate(bloomTarget, true);
    }

     

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Bloom，将模糊后的图作为Material的Blur图参数
        _Material.SetTexture("_BlurTex", bloomTarget);
        _Material.SetVector("_bloomColor", bloomColor);
        _Material.SetFloat("_bloomFactor", bloomFactor);

        //使用pass2进行景深效果计算，清晰场景图直接从source输入到shader的_MainTex中
        Graphics.Blit(source, destination, _Material, 2);
    }
}