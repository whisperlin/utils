using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubelet : MonoBehaviour
{
    public List<GameObject> Planes = new List<GameObject>();

    public void SetColor(int x, int y, int z)
    {
        if (y == 1)
            Planes[0].SetActive(true);
        else if (y == -1)
            Planes[1].SetActive(true);
            
        if (x == 1)
            Planes[2].SetActive(true);
        else if (x == -1)
            Planes[3].SetActive(true);
            
        if (z == -1)
            Planes[4].SetActive(true);
        else if (z == 1)
            Planes[5].SetActive(true);  
    }
}
