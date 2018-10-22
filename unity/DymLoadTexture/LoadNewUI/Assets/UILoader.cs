using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UILoader : MonoBehaviour
{

	public string uiName;
	//public Transform parent;
	//public static List<GameObject> ctrlList =new List<GameObject>();
	IEnumerator Load ()
	{



		GameObject root;
		//string _path = "file:"+Application.streamingAssetsPath+"/UI/"+uiName+".ui";

		#if UNITY_EDITOR
		string _path = "file:" + Application.dataPath + "/StreamingAssets" + "/Windows/" + uiName;

		#elif UNITY_IPHONE
		string _path = Application.dataPath +"/Raw"+"/UI/"+uiName+".ui";;

		#elif UNITY_ANDROID
		string _path =Application.streamingAssetsPath+"/Android/"+uiName;

		#endif

		Debug.Log ("Loading UI " + _path);
		WWW www = new WWW (_path);
		while (!www.isDone)
			yield return null;
		string[] names = www.assetBundle.GetAllAssetNames ();
		foreach (string name in names) {
			Debug.Log (name);
		}
		string[] list = www.assetBundle.GetAllAssetNames ();
		AssetBundleRequest request = www.assetBundle.LoadAssetAsync (list[0]);
		while (!request.isDone)
			yield return null;

		root = (GameObject)GameObject.Instantiate (request.asset);
		//root.transform.parent = parent;
		www.assetBundle.Unload (false);


		ImageData data = root.GetComponent<ImageData> ();
		if (null != data) {
			UISpriteCtrl ls = root.AddComponent<UISpriteCtrl> ();
			ls.AddData (data);
			GameObject.Destroy (data);
		}
		//Vector3 pos =  root.transform.position;
		root.transform.parent = transform;
		root.transform.position = new Vector3(Camera.main.pixelWidth/2,Camera.main.pixelHeight/2,0);

		//ctrlList.Add (root);

		Debug.Log ("Loading UI finish " + _path);

	}
	// Use this for initialization
	void Start ()
	{
		StartCoroutine (Load ());
	}
	// Update is called once per frame
	void Update ()
	{

	}
}
