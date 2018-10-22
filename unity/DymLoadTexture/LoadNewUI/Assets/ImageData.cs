using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*public class ImageInformation
{
	public Image img;
	public string path;
	public string name;
}*/

public class ImageData : MonoBehaviour {
	//这样好方便调试.
	public List<Image> images = new List<Image>();
	public List<string> paths = new List<string>();
	public List<string> names = new List<string>();
}
