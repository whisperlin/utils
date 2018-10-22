using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UISpriteCtrl : MonoBehaviour {

	public Image []images = null;
	public string [] paths = null;
	public string [] names = null;
	TextureHandle [] handles = null;	
	bool finish = false;
	public void AddData(ImageData data)
	{
		this.images = data.images.ToArray();
		this.names = data.names.ToArray();
		this.paths = data.paths.ToArray ();
		LoadAtlas();
	}

	void LoadAtlas ()
	{
		if(null != images)
		{
			handles = new TextureHandle[images.Length];
			for(int i = 0 ; i < images.Length ; i++)
			{	images[i].enabled  = false;
				handles[i] =   UITextureManager.Instance().LoadAtlasTexture(paths[i]);
			}
		}
	}
	public void SwitchTexture(Image img , string name)
	{
		for (int i = 0, len = images.Length; i < len; i++) {
			if(images[i] == img)
			{
				if (names [i] == name)
					return;
				 
				names [i] = name;
				//如果已经加载完成了，便切换贴图..
				if (handles [i].isDone) {
					for (int j = 0; j < handles [i].textures.Length; j++) {
						if (name.CompareTo (handles [i].textures [j].name) == 0) {
							images [i].sprite = handles [i].textures [j];
							return;
						}
					}
				}
			}
		}
	}
	void OnEnable ()
	{
		LoadAtlas();
		finish = false;
	}
	void OnDisable() {
		for(int i = 0 ; i < handles.Length ; i++)
		{
			images[i].enabled = false;
			UITextureManager.Instance().ReleaseAtlasTexture(paths[i]);
		}
		handles = null;	
	}

	// Update is called once per frame
	void Update () {
		if (finish)
			return;
		for (int i = 0, len = images.Length; i < len; i++) {
			if (null != images [i].sprite) {
				names [i] = images [i].sprite.name;
			}
		}
		for(int i = 0 ; i < handles.Length ; i++)
		{
			if(! handles[i].isDone )
			{
				return;
			}
		}
		for(int i = 0 ; i < handles.Length ; i++)
		{
			string name = names [i];
			bool found = false;
			for (int j = 0; j < handles [i].textures.Length; j++) {
				
				if (name.CompareTo (handles [i].textures [j].name) == 0) {
					images [i].sprite = handles [i].textures [j];
					found = true;
				}
			}
			if (!found) {
				Debug.LogError ("No Found " + name);
				for (int j = 0; j < handles [i].textures.Length; j++) {
					Debug.LogError (":"+handles [i].textures [j].name);
				}
			}
			images[i].enabled  = true;
		}
		finish = true;
	}
}
