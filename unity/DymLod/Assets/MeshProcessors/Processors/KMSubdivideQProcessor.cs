using UnityEngine;
using System.Collections;
using KrablMesh;

[AddComponentMenu("")]
public class KMSubdivideQProcessor : KrablMesh.Processor {
	public enum NormalMode { Interpolate, Recalculate }

	public bool trisToQuads = true;
	public float trisToQuadsMaxEdgeAngle = 30.0f;
	public NormalMode normalMode = NormalMode.Recalculate;
	public int iterations = 1;
	public string[] iterationsOverridePlatform = new string[0];
	public int[] iterationsOverride = new int[0];
	
	override public string Name() { return "Subdivide (Quads)"; }		
	
	override public void Calculate(ref KrablMesh.MeshEdges mesh, KMProcessorProgram parentProgram = null) {			
		base.Calculate(ref mesh);

		SubdivideQ sub = new SubdivideQ();
		SubdivideQParameters par = new SubdivideQParameters();
		sub.progressDelegate = delegate(string text, float val) {
			ReportProgress(text, val);
		};
		
		par.trisToQuads = trisToQuads;
		par.trisToQuadsMaxAngle = trisToQuadsMaxEdgeAngle;
		par.iterations = iterations;
		par.recalculateNormals = (normalMode == NormalMode.Recalculate);
		if (platformID != null) {
			for (int i = 0; i < iterationsOverridePlatform.Length; ++i) {
				if (platformID.Equals(iterationsOverridePlatform[i])) {
					par.iterations = iterationsOverride[i];
					break;
				}
			}
		}
		
		sub.Execute(ref mesh, par);
	}
	
	override public bool IsPlatformDependent() { 
		return (iterationsOverridePlatform.Length > 0);
	}

}
