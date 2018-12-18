using UnityEngine;

public class Texture3DTest : MonoBehaviour
{
	public Renderer target;
	public int size = 16;


	void Start()
	{
		var tex = new Texture3D(size, size, size, TextureFormat.RGBA32, false);
		var colors = new Color[size * size * size];
		var k = 0;

		for (int z = 0; z < size; z++)
		{
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++, k++)
				{
					if (z == 0)
						colors[k] = Color.blue;
					else
						colors[k] = Color.red;
				}
			}
		}
		tex.wrapMode = TextureWrapMode.Repeat;
		tex.SetPixels(colors);
		tex.Apply();
		target.material.SetTexture("_MainTexture", tex);
	}
}