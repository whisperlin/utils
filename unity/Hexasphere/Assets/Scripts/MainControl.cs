using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainControl : MonoBehaviour, IInputElement
{
    [SerializeField]
    private Icosahedron mIcoSphere;

    [SerializeField]
    private Hexagon mHexPrefab;

    [SerializeField]
    private SphereCollider mSphereCollider;

    [SerializeField]
    private int mIterationCount;

    [SerializeField]
    private Slider mSlider;

    [SerializeField]
    private float mRotationSpeedMultiplyer = 1f;

    private List<Hexagon> mTiles;
    private Vector2 rotationSpeed = Vector2.zero;

    public int IterationCount
    {
        get
        {
            return mIterationCount;
        }
        set
        {
            if (mIterationCount != value)
            {
                mIterationCount = value;
                CreateSphere();
            }
        }
    }

    public Collider MainCollider
    {
        get
        {
            return mSphereCollider;
        }
    }

    public bool ProcessClick(int mouseIndex)
    {
        return false;
    }

    public bool ProcessDrag(Vector3 delta)
    {
        delta *= mRotationSpeedMultiplyer;
        rotationSpeed.Set(delta.x, delta.y);
        return true;
    }

    public void ProcessMouseLost()
    {
    }

    public bool ProcessMouseOver()
    {
        return false;
    }

    public void SetNumSubdivisions(float val)
    {
        IterationCount = (int)val;
    }

    private void CreateSphere()
    {
        if (mIterationCount < 1)
            mIterationCount = 1;
        transform.localRotation = Quaternion.identity;
        foreach (var hex in mTiles)
        {
            Destroy(hex.gameObject);
        }
        mTiles.Clear();

        mSphereCollider.radius = 10.1f;
        mIcoSphere.GenerateIcosahedron(10f, mIterationCount);
        InputController.Instance.AddTrackingElement(this);

        foreach (var tile in mIcoSphere.Tiles)
        {
            var hexagon = Instantiate(mHexPrefab) as Hexagon;
            hexagon.Init(tile, transform);
            mTiles.Add(hexagon);
        }
    }

    private void Update()
    {
        if (Mathf.Abs(rotationSpeed.magnitude) > 0.1)
        {
            transform.RotateAround(transform.position, Vector3.up, rotationSpeed.x);
            transform.RotateAround(transform.position, Vector3.left, rotationSpeed.y);
            rotationSpeed = Vector2.Lerp(rotationSpeed, Vector2.zero, 0.05f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private void Start()
    {
        mSlider.onValueChanged.AddListener(SetNumSubdivisions);

        mTiles = new List<Hexagon>();
        CreateSphere();
    }
}
