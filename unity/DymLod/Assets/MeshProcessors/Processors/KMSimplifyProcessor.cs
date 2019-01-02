using UnityEngine;

[AddComponentMenu("")]
public class KMSimplifyProcessor : KrablMesh.Processor {	 
//	public float maximumError = 0.0f;
	public int targetTriangleCount = 0;
	public string[] ttcOverridePlatform = new string[0];
	public int[] ttcOverrideTargetTriangleCount = new int[0];
	
	public bool allowVertexReposition = true;
	public bool preventNonManifoldEdges = false;
	public bool recalculateNormals = false;
	
	public float borders = 1.0f;
public float creases = 0.0f;
	public float uvSeams = 1.0f;
	public float uv2Seams = 0.0f;
	public float materialSeams = 1.0f;
	
//	public int maxEdgesPerVertex = 18;
//	public float minTriangleShape = 0.12f;
	
	// Delta protections
	public float boneWeightProtection = 1.0f;
	public float vertexColorProtection = 1.0f;
//	public float vertexNormalProtection = 0.0f;

	public bool advancedSettingsVisible = false;

	override public string Name() { return "Simplify"; }		
		
	override public void Calculate(ref KrablMesh.MeshEdges mesh, KMProcessorProgram parentProgram = null) {
		base.Calculate(ref mesh);
		
		KrablMesh.Simplify sim = new KrablMesh.Simplify();
		KrablMesh.SimplifyParameters par = new KrablMesh.SimplifyParameters();
		
		sim.progressDelegate = delegate(string text, float val) {
			ReportProgress(text, val);
		};
		
	//	par.maximumError = maximumError;
		par.targetFaceCount = targetTriangleCount;
		if (platformID != null) {
			for (int i = 0; i < ttcOverridePlatform.Length; ++i) {
				if (platformID.Equals(ttcOverridePlatform[i])) {
					par.targetFaceCount = ttcOverrideTargetTriangleCount[i];
					break;
				}
			}
		}
		
		par.recalculateVertexPositions = allowVertexReposition;
		par.preventNonManifoldEdges = preventNonManifoldEdges;
		
		par.borderWeight = borders; 
		par.creaseWeight = creases; 
		par.uvSeamWeight = uvSeams; 
		par.uv2SeamWeight = uv2Seams;
		par.materialSeamWeight = materialSeams; 
		
		// par.minTriangleShape = minTriangleShape;
		
		par.boneWeightProtection 	= boneWeightProtection;
		par.vertexColorProtection 	= vertexColorProtection;
	//	par.vertexNormalProtection 	= vertexNormalProtection;
		
		sim.Execute(ref mesh, par);
	}
	
	override public bool IsPlatformDependent() { 
		return (ttcOverridePlatform.Length > 0);
	}
}
