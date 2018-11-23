using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SetTarget : MonoBehaviour {
	public RenderTexture target;
	public Camera cam;
 

	static Material lineMaterial;
	static void CreateLineMaterial()
	{
		if (!lineMaterial)
		{
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			lineMaterial = new Material(shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			lineMaterial.SetInt("_ZWrite", 0);
		}
	}
	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
		 

	}

	void DrawRect(float x0,float y0,float x1,float y1 ,float z)
	{
		GL.Vertex3(x0, y0, z);
		GL.Vertex3(x1, y1, z);
		GL.Vertex3(x0, y1, z);
		GL.Vertex3(x0, y0, z);
		GL.Vertex3 (x1, y0, z);
		GL.Vertex3(x1, y1, z);
		 
	}
	void RenderBroder(RenderTexture t ,Color color)
	{
		CreateLineMaterial ();
		float hw = t.width*0.5f;
		float hh = t.height *0.5f;
		float broderWidth = 1.0f;
		float deltaX = 1.0f/t.width;
		float deltaY = 1.0f / t.height;
		Graphics.SetRenderTarget(t);
		lineMaterial.SetPass(0);
		GL.PushMatrix();
		GL.LoadOrtho ();
		GL.Begin(GL.TRIANGLES);
		GL.Color (color);
		DrawRect (0.0f-deltaY, 0.0f, 1.0f, 0.0f+deltaY,0.0f);
		DrawRect (0.0f, 1.0f-deltaY, 1.0f, 1.0f+deltaY,0.0f);
		DrawRect (0.0f-deltaX, 0.0f, 0.0f + deltaX,1.0f ,0.0f);
		DrawRect (1.0f-deltaX, 0.0f, 1.0f +deltaX,1.0f ,0.0f);
		GL.End();
		GL.PopMatrix ();
		Graphics.SetRenderTarget(null);
	}
	// Update is called once per frame
	void Update () {
 
		var old = cam.targetTexture;
		cam.targetTexture = target;
		cam.Render ();
		RenderBroder (target,Color.red);

		cam.targetTexture = null;
	}
}
