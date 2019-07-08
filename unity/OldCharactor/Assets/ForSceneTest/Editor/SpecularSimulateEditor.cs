using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class SpecularSimulateEditor {
	[MenuItem("Easy/Scene/Specular Simulate")]
	public static void UpdateLightDir()
	{
		var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		var objs = scene.GetRootGameObjects();
		
		Light light = null;
		Vector3 lightDir = Vector3.down;

		var select = Selection.activeGameObject;
		if(select)
		{
			light = select.GetComponent<Light>();
		}

		List<MeshRenderer> renderers = new List<MeshRenderer>();

		foreach(var i in objs)
		{
			if(light == null)
			{
				if(i.name.Equals("Directional light"))
				{
					light = i.GetComponent<Light>();
				}
			}

			var rs = i.GetComponentsInChildren<MeshRenderer>();
			renderers.AddRange(rs);
		}

		if(light)
		{
			var lightRot = light.transform.rotation.eulerAngles;
			lightRot.x = 15;
			lightDir = Quaternion.Euler(lightRot) * Vector3.forward;
		}

		foreach(var i in renderers)
		{
			var mats = i.sharedMaterials;
			foreach(var j in mats)
			{
				if(j.shader.name.StartsWith("YuLongZhi/SpecularSimulate"))
				{
					j.SetVector("_ShaderLightDir", lightDir);
				}
			}
		}
	}
}
