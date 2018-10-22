using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureHandle
{
	#region private params and methods

	protected string name;
	protected Sprite  [] texs = null;
	protected UITextureManager.LoadState state = UITextureManager.LoadState.NONE;

	#endregion

	public string Name {
		get{ return name; }
	}

	public bool isDone {
		get{ return state == UITextureManager.LoadState.LOADED; }
	}

	public Sprite[] textures {
		get{ return texs; }
	}
}
[ExecuteInEditMode]
public class UITextureManager : MonoBehaviour
{

	public float deleteTime = 3f;

	public enum LoadState
	{
		NONE,
		LOADING,
		LOADED,
		FAILED,
		DEAD}

	;

	#region private params and methods

	class _TextureHandle :TextureHandle
	{
		public float deleteTime;
		public int count = 0;

		public LoadState State {
			get {
				return state;
			}
			set {
				state = value;
			}
		}

		public void SetPath (string n)
		{
			name = n;
		}

		public void SetTexture (Sprite[] t)
		{
			texs = t;
		}
	}

	static UITextureManager _instance = null;
	Dictionary<string,_TextureHandle> textures = new Dictionary<string, _TextureHandle> ();
	List<_TextureHandle> deads = new List<_TextureHandle> ();

	IEnumerator LoadTexture (_TextureHandle h)
	{
		#if UNITY_EDITOR
		string path = "file:" + Application.streamingAssetsPath + "/Windows/" + h.Name;
		#elif UNITY_IPHONE
		string path = Application.dataPath +"/Raw"+"/UIAtlas/"+h.Name+".t2d";
		#elif UNITY_ANDROID
		string path =Application.streamingAssetsPath+"/Android/"+h.Name;
		#endif
		SimpleLog.Log ("loading " + path);
		float beging = Time.realtimeSinceStartup;
		WWW www = new WWW (path);
		while (!www.isDone)
			yield return null;
		string[] list = www.assetBundle.GetAllAssetNames ();


		AssetBundleRequest request =   www.assetBundle.LoadAssetWithSubAssetsAsync<Sprite> (list [0]);
		//AssetBundleRequest request = www.assetBundle.LoadAssetAsync (list [0],typeof(Sprite));
		while (!request.isDone)
			yield return null;

 
		//foreach (Object s in request.allAssets) {
		//Debug.LogError (s.name);
		//}
		//Sprite _spr = request.allAssets[0] as Sprite;
		Sprite [] _sprs = new Sprite[request.allAssets.Length];
		for (int i = 0, len = request.allAssets.Length; i < len; i++) {
			_sprs[i] = (Sprite) request.allAssets[i];
		}
		h.SetTexture (_sprs);


		www.assetBundle.Unload (false);
		h.State = LoadState.LOADED;
		h.deleteTime = Time.realtimeSinceStartup;
		float end = Time.realtimeSinceStartup;
		SimpleLog.Log ("loading finish " + path + "used " + (end - beging).ToString () + "s");
		if (h.count == 0)
			ReleaseTexture (h);
	}

	void ReleaseTexture (_TextureHandle h)
	{
		if (h.State == LoadState.LOADING) {
			return;
		}
		h.deleteTime = Time.realtimeSinceStartup;
		h.State = LoadState.DEAD;
		deads.Add (h);

	}

	#endregion

	
	public static UITextureManager Instance ()
	{
		if (null == _instance) {
			GameObject g = new GameObject ("UITextureManager");
			g.hideFlags = HideFlags.HideAndDontSave;
			GameObject.DontDestroyOnLoad (g);
			_instance = g.AddComponent<UITextureManager> ();
		}
		return _instance;
	}
	
 
	public TextureHandle LoadAtlasTexture (string path)
	{
		_TextureHandle h = null;
		if (!textures.TryGetValue (path, out h)) {
			h = new _TextureHandle ();
			h.SetPath (path);
			h.State = LoadState.NONE;
			textures [path] = h;
			StartCoroutine (LoadTexture (h));
		}
		if (h.State == LoadState.DEAD)
			h.State = LoadState.LOADED;
		h.count++;
		return h;
	}

	public void ReleaseAtlasTexture (string name)
	{
		_TextureHandle h = null;
		if (!textures.TryGetValue (name, out h))
			return;
		if (h.count > 0)
			h.count--;
		if (h.count == 0) {
			ReleaseTexture (h);
		}
	}
	void OnDestroy()
	{

		foreach(_TextureHandle  h in textures.Values )
		{
			for(int j= 0 ; j<h.textures.Length ;j++)
			{
			GameObject.DestroyImmediate (h.textures [j].texture,true);
			GameObject.DestroyImmediate (h.textures[j], true);
			}
		}
		textures.Clear ();
		deads.Clear ();
	}
	void Update ()
	{
		#if UNITY_EDITOR
		//Debug.Log(Application.isPlaying.ToString());
		//Debug.Log(UnityEditor.EditorApplication.isPlaying.ToString() + "  "+UnityEditor.EditorApplication.isPaused.ToString());
		if(!Application.isPlaying)
		{
			GameObject.DestroyImmediate(this.gameObject,true);
			return;
		}
			
		#endif

		int _len = deads.Count;
		for (int i = 0; i < _len ;) {
			_TextureHandle h = deads [i];
			if (h.State == LoadState.DEAD) {
				if (Time.realtimeSinceStartup - h.deleteTime > deleteTime) {
					SimpleLog.Log ("Auto Release " + h.Name);
					deads.Remove (h);
					textures.Remove (h.Name);
					
					for(int j= 0 ; j<h.textures.Length ;j++)
					{
						GameObject.DestroyImmediate (h.textures [j].texture,true);
						GameObject.DestroyImmediate (h.textures[j], true);
					}
					
					_len--;
				} else {
					i++;
				}
			} else {
				deads.RemoveAt (i);
				_len--;
			}
		}
	}


}
