using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCharacterColliderData
{
    
    public string type;
    public object data;

    public T getData<T>()
    {
        return (T)data;
    }
}