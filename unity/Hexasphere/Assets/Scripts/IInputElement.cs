using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputElement
{
    Collider MainCollider
    {
        get;
    }
    // Input elements returns true if it blocks such input behind the element;

    bool ProcessMouseOver();
    void ProcessMouseLost();
    bool ProcessClick(int mouseIndex);
    bool ProcessDrag(Vector3 delta);
}
