using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixer : MonoBehaviour {

    #region InEditorVariables
    public List<MassSpringCloth> cloths = new List<MassSpringCloth>();

    #endregion

    #region otherVariables
    Collider collider;
    #endregion
    // Use this for initialization
    void Start () {
        collider = GetComponent<Collider>();
        
        foreach (MassSpringCloth c in cloths)
        {
            foreach (Node n in c.getNodes())
            {
                if (collider.bounds.Contains(n.pos))
                {
                    n.isFixed = true;
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
    }
}
