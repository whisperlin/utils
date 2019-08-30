using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    private GameObject _obj;
    private GameObject _canvas;
    private GameObject _instance;

    // Use this for initialization
    void Start () {
		_obj = ResourceManager.Instance.LoadAsset<GameObject> ("Prefabs/Login");
//        _obj = (GameObject)Resources.Load("Prefabs/Login");
        if (null == _obj)
        {
            Debug.LogFormat("{0} is null", "Login.prefab");
        }
        else
        {
            Debug.LogFormat("{0} is not null", "Login.prefab");
        }
        _canvas = GameObject.Find("Canvas");
        if (null == _canvas)
        {
            Debug.LogFormat("{0} is null", "canvas");
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (null == _instance)
        {
            _instance = Instantiate(_obj);
            _instance.transform.parent = _canvas.transform;
            _instance.transform.localScale = new Vector3(1, 1, 1);
        }
        //Instantiate(_obj);

	}
}
