using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathfHelper  {

    public static Vector3 Vector3Mul(Vector3 v1,Vector3 v2)
    {
        return new Vector3(v1.x*v2.x,v1.y*v2.y,v1.z*v2.z);
    }
}
