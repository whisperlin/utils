using UnityEngine;
using System.Collections;
/*created by Jason 20160805
 * 浮生若梦出品
 * 作者QQ:541211225
 * Mobile:15821684699
 * v2.0
*/
public class JasonReplay : MonoBehaviour {
	[UnityEngine.Header("作者公众号“特效基地” v2.0")]
	[UnityEngine.Header("Press space key to replay")]

	public bool AutoReplay = true;
	public float replayTimer = 1.5f;
	public float playSpeed = 1.0f;

	private float myTime;

	// Use this for initialization
	void Start () {
		
		myTime = replayTimer;

	}
	void Update () {
		
		myTime -= Time.deltaTime;
		if(myTime <= 0 && AutoReplay == true){
			Jason ();
			myTime = replayTimer;
		}

		Time.timeScale = playSpeed;
		if(Input.GetKey(KeyCode.Space)){
			Jason ();
		}
	}

	void Jason(){

		Application.LoadLevel(Application.loadedLevel);

	}
}
