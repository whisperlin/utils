using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {

    static BulletManager _instance;
    public static BulletManager Instance()
    {
        return _instance;
    }

    public List<LCHBullet> bullets = new List<LCHBullet>();
    // Use this for initialization
    void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
