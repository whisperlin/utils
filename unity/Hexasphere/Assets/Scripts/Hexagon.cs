using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour, IInputElement
{
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float mRotationAlpha;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float mPositionAlpha;

    [SerializeField]
    private MeshFilter mFilter;
    [SerializeField]
    private MeshRenderer mRenderer;
    [SerializeField]
    private MeshCollider mCollider;
    [SerializeField]
    private Transform mTransform;
    [SerializeField]
    private float mSelectedOutDistance;
    [SerializeField]
    [Range(0f, 1f)]
    private float mAnimationSpeed;

    private Vector3 mCenterDirection;       //direction to sphere center
    private Vector3 mRotationDirection;     //the vector in local coords to rotate around
    private float mTargetRotationAlpha;
    private float mTargetPositionAlpha;

    private Mesh mHexMesh;
    private Transform mParent;

    private Vector3 mMouseOverPosition;
    private Vector3 mNoMousePosition;
    private Quaternion mNoRotation;
    private Quaternion mFlippedRotation;
    private bool mIsAnimating;
    private bool mIsFlipped;
    private bool mIsPushed;

    public Collider MainCollider
    {
        get
        {
            return mCollider;
        }
    }

    public bool IsFlipped
    {
        get
        {
            return mIsFlipped;
        }
        set
        {
            if (value != mIsFlipped)
            {
                switch (value)
                {
                    case true:
                        mTargetRotationAlpha = 1;
                        mIsAnimating = true;
                        break;
                    case false:
                        mTargetRotationAlpha = 0;
                        mIsAnimating = true;
                        break;
                }
                mIsFlipped = value;
            }
        }
    }

    public bool IsPushed
    {
        get
        {
            return mIsPushed;
        }
        set
        {
            if(value !=mIsPushed)
            {
                switch(value)
                {
                    case true:
                        mTargetPositionAlpha = 1;
                        mIsAnimating = true;
                        break;
                    case false:
                        mTargetPositionAlpha = 0;
                        mIsAnimating = true;
                        break;
                }
                mIsPushed = value;
            }
        }
    }

    public void Init(Tile tile, Transform parent = null)
    {
        mParent = parent;
        if (parent != null)
        {
            transform.SetParent(mParent);
        }

        var pointsList = new List<Vector3>(tile.Boundary.ToUnityVectorsArray());
        var center = new Vector3();
        foreach (var point in pointsList)
        {
            center += point;
        }

        center /= pointsList.Count;
        mCenterDirection = -center;
        mCenterDirection.Normalize();
        mRotationDirection = pointsList.Count == 6 ? pointsList[0] - pointsList[3] : pointsList[0] - (pointsList[1] + pointsList[4]) / 2;
        mRotationDirection.Normalize();

        //move them back
        for (int i = 0; i < pointsList.Count; i++)
        {
            pointsList[i] -= center;
        }

        var indices = new List<int>();
        for (int i = 0; i < pointsList.Count - 2; i++)
        {
            indices.Add(0);
            indices.Add(i + 1);
            indices.Add(i + 2);
        }

        transform.localPosition = center;

        mHexMesh.Clear();
        mHexMesh.SetVertices(pointsList);
        mHexMesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        mHexMesh.RecalculateNormals();

        mFilter.sharedMesh = mHexMesh;
        mCollider.sharedMesh = mHexMesh;

        InputController.Instance.AddTrackingElement(this);

        BakeTransformations();
    }

    public void Update()
    {
        var rot = Quaternion.Lerp(mNoRotation, mFlippedRotation, mRotationAlpha);
        var pos = Vector3.Lerp(mNoMousePosition, mMouseOverPosition, mPositionAlpha);
        mTransform.localRotation = rot;
        mTransform.localPosition = pos;

        ProcessAnimations();
    }

    public void Awake()
    {
        mHexMesh = new Mesh();
    }

    public void OnDestroy()
    {
        Destroy(mHexMesh);
        InputController.Instance.RemoveTrackingElement(this);
    }

    public bool ProcessMouseOver()
    {
        IsPushed = true;
        return true;
    }

    public bool ProcessClick(int mouseIndex)
    {
        IsFlipped = !IsFlipped;
        return false;
    }

    public bool ProcessDrag(Vector3 delta)
    {
        return false;
    }

    public void ProcessMouseLost()
    {
        IsPushed = false;
    }

    private void BakeTransformations()
    {
        mNoRotation = mTransform.localRotation;
        mFlippedRotation = Quaternion.AngleAxis(180f, mRotationDirection);
        mNoMousePosition = mTransform.localPosition;
        mMouseOverPosition = mTransform.localPosition - mCenterDirection * mSelectedOutDistance;
    }

    private void ProcessAnimations()
    {
        if (!mIsAnimating)
            return;
        //check if we should stop animating
        if (Mathf.Abs(mRotationAlpha - mTargetRotationAlpha) < 0.01)
            mRotationAlpha = mTargetRotationAlpha;
        if (Mathf.Abs(mPositionAlpha - mTargetPositionAlpha) < 0.01)
            mPositionAlpha = mTargetPositionAlpha;
        if (mRotationAlpha == mTargetRotationAlpha && mPositionAlpha == mTargetPositionAlpha)
            mIsAnimating = false;
        //----------------------------------
        //Animation
        mPositionAlpha = Mathf.Lerp(mPositionAlpha, mTargetPositionAlpha, 0.05f);
        mRotationAlpha = Mathf.Lerp(mRotationAlpha, mTargetRotationAlpha, 0.05f);
    }
}
