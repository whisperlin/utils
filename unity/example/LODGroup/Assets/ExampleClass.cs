using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LODGroup))] 
public class ExampleClass : MonoBehaviour
{
	public LODGroup group;

	void Start()
	{
		// Programmatically create a LOD group and add LOD levels.
		// Create a GUI that allows for forcing a specific LOD level.
		group = gameObject.GetComponent<LODGroup>();
 
		// Add 4 LOD levels
		LOD[] lods = new LOD[3];
		for (int i = 0; i < group.lodCount; i++)
		{
			PrimitiveType primType = PrimitiveType.Cube;
			switch (i)
			{
			case 1:
				primType = PrimitiveType.Capsule;
				break;
			case 2:
				primType = PrimitiveType.Sphere;
				break;
			case 3:
				primType = PrimitiveType.Cylinder;
				break;
			}
			GameObject go = GameObject.CreatePrimitive(primType);
			go.transform.parent = gameObject.transform;
			Renderer[] renderers = new Renderer[1];
			renderers[0] = go.GetComponent<Renderer>();
			float screenRelativeTransitionHeight = 1f;

			switch (i)
			{
			case 1:
				screenRelativeTransitionHeight = 0.25f;
				break;
			case 2:
				screenRelativeTransitionHeight = 0.05f;
				break;
			case 3:
				screenRelativeTransitionHeight = 0.02f;
				break;
			}
			lods[i] = new LOD(screenRelativeTransitionHeight, renderers);

		}
		group.SetLODs(lods);
		group.RecalculateBounds();
	}

	void OnGUI()
	{
		if (GUILayout.Button("Enable / Disable"))
			group.enabled = !group.enabled;

		if (GUILayout.Button("Default"))
			group.ForceLOD(-1);

		if (GUILayout.Button("Force 0"))
			group.ForceLOD(0);

		if (GUILayout.Button("Force 1"))
			group.ForceLOD(1);

		if (GUILayout.Button("Force 2"))
			group.ForceLOD(2);

		if (GUILayout.Button("Force 3"))
			group.ForceLOD(3);

		if (GUILayout.Button("Force 4"))
			group.ForceLOD(4);

		if (GUILayout.Button("Force 5"))
			group.ForceLOD(5);

		if (GUILayout.Button("Force 6"))
			group.ForceLOD(6);
	}
}