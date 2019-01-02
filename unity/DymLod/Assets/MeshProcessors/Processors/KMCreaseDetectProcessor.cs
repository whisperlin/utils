using UnityEngine;

[AddComponentMenu("")]
public class KMCreaseDetectProcessor : KrablMesh.Processor {
	public bool creasesFromNormals = true;
	public bool creasesFromEdgeAngles = false;
	public float minEdgeAngle = 50.0f;
	public bool creasesFromMaterialSeams = false;
//	public float creaseStrength = 1.0f;
	
	override public string Name() { return "Detect Creases"; }		
	
	override public void Calculate(ref KrablMesh.MeshEdges mesh, KMProcessorProgram parentProgram = null)  {			
		base.Calculate(ref mesh);
		
		bool recalcNormals = false;
		
		KrablMesh.CreaseDetect.ClearAllCreases(mesh);
		
		if (creasesFromNormals) {
			KrablMesh.CreaseDetect.MarkCreasesFromFaceNormals(mesh, 1.0f);
	//		if (creaseStrength < 1.0f) recalcNormals = true;
		} else {
			recalcNormals = true;
		}
		
		if (creasesFromEdgeAngles) {
			KrablMesh.CreaseDetect.MarkCreasesFromEdgeAngles(mesh, minEdgeAngle, 1.0f);
			recalcNormals = true;
		}
		if (creasesFromMaterialSeams) {
			KrablMesh.CreaseDetect.MarkCreasesFromMaterialSeams(mesh, 1.0f);
			recalcNormals = true;
		}
		
		if (recalcNormals) {
			mesh.CalculateFaceVertexNormalsFromEdgeCreases();
		}
	}
}
