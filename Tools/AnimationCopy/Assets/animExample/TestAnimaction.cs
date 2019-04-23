using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimaction : MonoBehaviour {

    public void OnAnimFinish(string test)
    {
        Debug.Log("On Finish "+test);
    }
}
