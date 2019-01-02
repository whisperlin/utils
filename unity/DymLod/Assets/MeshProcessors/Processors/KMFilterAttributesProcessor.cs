using UnityEngine;

[AddComponentMenu("")]
public class KMFilterAttributesProcessor : KrablMesh.Processor {
	public enum MaterialOption {
		PassThrough,
		Merge,
	}
	
	public enum Option {
		PassThrough,
		Remove,
	}
	
	public MaterialOption materials;
	public Option vertexColors;
	public Option uv1Option;
	public Option uv2Option;
	public Option skin;
	
	override public string Name() { return "Filter Attributes"; }		
	
	override public void Calculate(ref KrablMesh.MeshEdges mesh, KMProcessorProgram parentProgram = null) {	
		base.Calculate(ref mesh);
		
		if (materials == MaterialOption.Merge) {
			mesh.numMaterials = 1;
			int numFaces = mesh.faceCount();
			for (int i = 0; i < numFaces; ++i) {
				mesh.faces[i].material = 0;
			}
		}
		
		if (vertexColors == Option.Remove) {
			mesh.hasVertexColors = false;
		}
		
		if (uv1Option == Option.Remove) {
			mesh.hasUV1 = false;
		}

		if (uv2Option == Option.Remove) {
			mesh.hasUV2 = false;
		}
		
		/*if (skin == Option.Remove) {
			mesh.hasBoneWeights = false;
			mesh.bindposes = null;
		}*/
	}

}
