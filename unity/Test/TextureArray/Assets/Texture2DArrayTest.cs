using UnityEngine;

public class Texture2DArrayTest : MonoBehaviour
{
	public Material material;
	Texture2DArray mTexture;


	void Start()
	{
		mTexture = new Texture2DArray(256, 256, 2, TextureFormat.RGBA32, false, true);

		var temp = new Texture2D(256, 256, TextureFormat.RGBA32, false);
		for (int x = 0; x < temp.width; x++)
			for (int y = 0; y < temp.height; y++)
				temp.SetPixel(x, y, Color.red);

		mTexture.SetPixels(temp.GetPixels(), 0);

		for (int x = 0; x < temp.width; x++)
			for (int y = 0; y < temp.height; y++)
				temp.SetPixel(x, y, Color.blue);

		mTexture.SetPixels(temp.GetPixels(), 1);

		mTexture.Apply();

		material.SetTexture("_TextureArray", mTexture);
	}
}