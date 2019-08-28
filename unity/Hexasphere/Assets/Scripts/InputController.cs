using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private Camera mCamera;

    private Dictionary<Collider, IInputElement> mInputElements = new Dictionary<Collider, IInputElement>();

    //Drag fields
    private List<IInputElement> mDraggedElements = new List<IInputElement>();
    private Vector3 lastScreenPosition;
    private bool isDragging;
    //-----------
    //MouseOver fields
    private HashSet<IInputElement> blockingElements = new HashSet<IInputElement>();
    private HashSet<IInputElement> mMouseOverElements = new HashSet<IInputElement>();

    private static InputController mInstance;

    public static InputController Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<InputController>();
                if (mInstance == null)
                {
                    mInstance = new GameObject().AddComponent<InputController>();
                    mInstance.mCamera = Camera.main;
                }
            }
            return mInstance;
        }
    }

    public void AddTrackingElement(IInputElement inputElement)
    {
        mInputElements[inputElement.MainCollider] = inputElement;
    }

    public void RemoveTrackingElement(IInputElement inputElement)
    {
        if (mInputElements.ContainsKey(inputElement.MainCollider))
            mInputElements.Remove(inputElement.MainCollider);
    }

    private void Awake()
    {
        mInstance = this;
    }

    private void Update()
    {
        ProcessMouse();
    }

    private void ProcessMouse()
    {
        ProcessMouseOver();
        ProcessMouseClicks();
        ProcessMouseDrags(1);
    }

    private void ProcessMouseDrags(int mouseIndex)
    {
        if (Input.GetMouseButtonDown(mouseIndex))
        {
            mDraggedElements.Clear();
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitInfos = Physics.RaycastAll(ray);
            hitInfos = hitInfos.OrderBy((c) => c.distance).ToArray();

            foreach (var hitInfo in hitInfos)
            {
                IInputElement element;
                if (mInputElements.TryGetValue(hitInfo.collider, out element))
                {
                    mDraggedElements.Add(element);
                    lastScreenPosition = Input.mousePosition;
                    isDragging = true;
                }
            }

            return;
        }

        if (Input.GetMouseButtonUp(mouseIndex))
        {
            mDraggedElements.Clear();
            isDragging = false;
        }

        if (Input.GetMouseButton(mouseIndex) && isDragging)
        {
            var newScreenPosition = Input.mousePosition;
            if (newScreenPosition != lastScreenPosition)
            {
                foreach (var dragElement in mDraggedElements)
                {
                    if (dragElement.ProcessDrag(lastScreenPosition - newScreenPosition))
                        break;
                }
            }
            lastScreenPosition = newScreenPosition;
        }
    }

    private void ProcessMouseClicks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
            var hitInfos = Physics.RaycastAll(ray);

            foreach (var hitInfo in hitInfos)
            {
                IInputElement element;
                if (mInputElements.TryGetValue(hitInfo.collider, out element))
                {
                    if (element.ProcessClick(0))
                        return;
                }
            }
        }
    }

    private void ProcessMouseOver()
    {
        Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, 100000f);
        List<IInputElement> elements = new List<IInputElement>();

        if (hitInfos.Length != 0)
        {
            IInputElement element;
            hitInfos = hitInfos.OrderBy((c) => c.distance).ToArray();
            foreach (var hitInfo in hitInfos)
            {
                mInputElements.TryGetValue(hitInfo.collider, out element);

                if (element != null)
                {
                    elements.Add(element);
                    if (blockingElements.Contains(element))
                        break;

                    if (!mMouseOverElements.Contains(element))
                    {
                        mMouseOverElements.Add(element);
                        if (element.ProcessMouseOver())
                        {
                            blockingElements.Add(element);
                        }
                    }
                }
            }
        }

        var theyLostMouse = mMouseOverElements.Except(elements).ToList();
        var theyAreNotBlockingAnymode = blockingElements.Except(elements).ToList();

        foreach (var looser in theyAreNotBlockingAnymode)
        {
            blockingElements.Remove(looser);
        }

        foreach (var looser in theyLostMouse)
        {
            looser.ProcessMouseLost();
            mMouseOverElements.Remove(looser);
        }
    }
}
