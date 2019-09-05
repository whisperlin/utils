using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLoadLevel : MonoBehaviour {

    public int index = 1;
    public string sceneName = "demo3";
    // Use this for initialization
    void Start () {
        SceneManager.LoadScene(sceneName);
        //Application.LoadLevel(index);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
