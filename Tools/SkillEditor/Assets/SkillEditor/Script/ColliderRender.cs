using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ColliderRender : MonoBehaviour {
    private static Mesh _unityCapsuleMesh = null;
    private static Mesh _unityCubeMesh = null;
    private static Mesh _unityCylinderMesh = null;
    private static Mesh _unityPlaneMesh = null;
    private static Mesh _unitySphereMesh = null;
    private static Mesh _unityQuadMesh = null;
    public static ColliderRender colliderRender = null;

    public float attackRange = 1f;
    public static Mesh GetUnityPrimitiveMesh(PrimitiveType primitiveType)
    {
        switch (primitiveType)
        {
            case PrimitiveType.Sphere:
                return GetCachedPrimitiveMesh(ref _unitySphereMesh, primitiveType);
                break;
            case PrimitiveType.Capsule:
                return GetCachedPrimitiveMesh(ref _unityCapsuleMesh, primitiveType);
                break;
            case PrimitiveType.Cylinder:
                return GetCachedPrimitiveMesh(ref _unityCylinderMesh, primitiveType);
                break;
            case PrimitiveType.Cube:
                return GetCachedPrimitiveMesh(ref _unityCubeMesh, primitiveType);
                break;
            case PrimitiveType.Plane:
                return GetCachedPrimitiveMesh(ref _unityPlaneMesh, primitiveType);
                break;
            case PrimitiveType.Quad:
                return GetCachedPrimitiveMesh(ref _unityQuadMesh, primitiveType);
                break;
            default:
                return null;
        }
    }

    private static Mesh GetCachedPrimitiveMesh(ref Mesh primMesh, PrimitiveType primitiveType)
    {
        if (primMesh == null)
        {
            //Debug.Log("Getting Unity Primitive Mesh: " + primitiveType);
            primMesh = Resources.GetBuiltinResource<Mesh>(GetPrimitiveMeshPath(primitiveType));

            if (primMesh == null)
            {
               // Debug.LogError("Couldn't load Unity Primitive Mesh: " + primitiveType);
            }
        }

        return primMesh;
    }

    private static string GetPrimitiveMeshPath(PrimitiveType primitiveType)
    {
        switch (primitiveType)
        {
            case PrimitiveType.Sphere:
                return "Sphere.fbx";
                break;
            case PrimitiveType.Capsule:
                return "Capsule.fbx";
                break;
            case PrimitiveType.Cylinder:
                return "Cylinder.fbx";
                break;
            case PrimitiveType.Cube:
                return "Cube.fbx";
                break;
            case PrimitiveType.Plane:
                return "Plane.fbx";//"New-Plane.fbx"
                break;
            case PrimitiveType.Quad:
                return "Quad.fbx";
                break;
            default:
                return "";
        }
    }

    public Collider [] colliders = new Collider[0];
    static Color gizmosColor = new Color(0.5f, 0.5f, 1.0f);
    void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        for (int i = 0; i < colliders.Length; i++)
        {
            var c = colliders[i];
            if (null == c)
                continue;
            if (c is MeshCollider)
            {
                MeshCollider c0 = (MeshCollider)c;
                Gizmos.DrawWireMesh(c0.sharedMesh, c.transform.position, c.transform.rotation, c.transform.lossyScale);
            }
            else if (c is BoxCollider)
            {
                BoxCollider c0 = (BoxCollider)c;
                Gizmos.DrawWireMesh(GetUnityPrimitiveMesh(PrimitiveType.Cube), c.transform.localToWorldMatrix.MultiplyPoint(c0.center), c.transform.rotation, new Vector3(c.transform.lossyScale.x* c0.size.x, c.transform.lossyScale.y* c0.size.y, c.transform.lossyScale.z* c0.size.z) );
            }
            else if (c is CapsuleCollider)
            {
                CapsuleCollider c0 = (CapsuleCollider)c;
                Gizmos.DrawWireMesh(GetUnityPrimitiveMesh(PrimitiveType.Capsule), c.transform.localToWorldMatrix.MultiplyPoint(c0.center), c.transform.rotation, new Vector3(c.transform.lossyScale.x * c0.radius , c.transform.lossyScale.y * c0.height*0.25f, c.transform.lossyScale.z * c0.radius ));

            }
            else if(c is SphereCollider)
            {
                SphereCollider c0 = (SphereCollider)c;

                Gizmos.DrawWireMesh(GetUnityPrimitiveMesh(PrimitiveType.Sphere), c.transform.localToWorldMatrix.MultiplyPoint(c0.center), c.transform.rotation, new Vector3(c.transform.lossyScale.x * c0.radius , c.transform.lossyScale.y * c0.radius , c.transform.lossyScale.z * c0.radius  ));
                 

            }
        }
        
        {
            //Gizmos.color  = new Color(1.0f,0.5f, 0.5f);
            //Gizmos.DrawWireMesh(GetUnityPrimitiveMesh(PrimitiveType.Cylinder), Vector3.zero,Quaternion.identity,  attackRange);
#if UNITY_EDITOR
            Handles.color = new Color(1.0f, 0.5f, 0.5f); 
            Handles.DrawWireDisc(Vector3.zero, Vector3.up, attackRange);
            
#endif


        }
            
    }

    private void Update()
    {
        if (colliderRender != this )
            GameObject.DestroyImmediate(this.gameObject);
    }
}
