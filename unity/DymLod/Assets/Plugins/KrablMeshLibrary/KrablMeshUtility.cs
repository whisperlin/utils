using UnityEngine;

/// <summary>
/// A collection of static calls to easily use the Krabl Mesh Library in Unity3D scripts.
/// </summary>

public class KrablMeshUtility  {
	
	/// <summary>
	/// Simplify a mesh by collapsing edges until the target face count is reached.
	/// </summary>
	/// <param name='unityMesh'>
	/// The mesh to simplify. 
	/// </param>
	/// <param name='targetFaceCount'>
	/// The number of triangles to reduce the mesh to. If this is higher than the initial
	/// number of triangles, no processing will occur.
	/// </param>
	/// <param name='highQuality'>
	/// Use slower, but more precise calculations.
	/// </param> 

	public static void SimplifyMesh(Mesh unityMesh, int targetFaceCount, bool highQuality = true) {
		KrablMesh.MeshEdges kmesh = new KrablMesh.MeshEdges();
		KrablMesh.Simplify sim = new KrablMesh.Simplify();
		KrablMesh.SimplifyParameters simpars = new KrablMesh.SimplifyParameters();
			
		KrablMesh.ImportExport.UnityMeshToMeshEdges(unityMesh, kmesh);	
		simpars.targetFaceCount = targetFaceCount;
		simpars.recalculateVertexPositions = highQuality;
		simpars.checkTopology = !highQuality;
		simpars.maxEdgesPerVertex = highQuality ? 18 : 0;
		if (highQuality == false) {
			simpars.preventNonManifoldEdges = false;
			simpars.boneWeightProtection = 0.0f;
			simpars.vertexColorProtection = 0.0f;
		}
		
		sim.Execute(ref kmesh, simpars);		
		KrablMesh.ImportExport.MeshEdgesToUnityMesh(kmesh, unityMesh);
	}
	
	/// <summary>
	/// Subdivides a mesh by applying a quads-based subdivision algorithm. Optionally, triangles are first merged to quads if possible to produce a better topology.
	/// For every iteration the face count of the mesh quadruples. The result mesh has quad topology.
	/// </summary>
	/// <param name='unityMesh'>
	/// The mesh to subdivide.
	/// </param>
	/// <param name='iterations'>
	/// The number of iterations the algorithm should perform. High numbers (>3) quickly lead to producing more triangles than
	/// unity can handle! (>65k).
	/// </param>
	/// <param name='trisToQuads'>
	/// Attempt to convert triangles to quads before the subdivision (highly recommended).
	/// </param>
	
	public static void SubdivideQuadsMesh(Mesh unityMesh, int iterations, bool trisToQuads = true) {
		KrablMesh.MeshEdges kmesh = new KrablMesh.MeshEdges();
		KrablMesh.SubdivideQ sub = new KrablMesh.SubdivideQ();
		KrablMesh.SubdivideQParameters subpars = new KrablMesh.SubdivideQParameters();
		
		KrablMesh.ImportExport.UnityMeshToMeshEdges(unityMesh, kmesh);
		subpars.trisToQuads = trisToQuads;
		subpars.iterations = iterations;
		
		sub.Execute(ref kmesh, subpars);		
		KrablMesh.ImportExport.MeshEdgesToUnityMesh(kmesh, unityMesh);
	}
	
	/// <summary>
	/// Change the normals of a mesh to produce a flat-shaded look. This will increase the vertex count as all vertices need to be split.
	/// </summary>
	/// <param name='unityMesh'>
	/// The mesh to flat shade.
	/// </param>

	public static void FlatShadeMesh(Mesh unityMesh) {
		KrablMesh.MeshEdges kmesh = new KrablMesh.MeshEdges();
		
		KrablMesh.ImportExport.UnityMeshToMeshEdges(unityMesh, kmesh);
		
		KrablMesh.CreaseDetect.MarkCreasesFromEdgeAngles(kmesh, 0.0f);
		kmesh.CalculateFaceVertexNormalsFromEdgeCreases();
		
		KrablMesh.ImportExport.MeshEdgesToUnityMesh(kmesh, unityMesh);
	}
	
	/// <summary>
	/// Change to normals of a mesh to produce a smooth-shaded look. This will reduce the vertex count and all mesh corners will just have one normal.
	/// </summary>
	/// <param name='unityMesh'>
	/// The mesh to smooth shade.
	/// </param>
	
	public static void SmoothShadeMesh(Mesh unityMesh) {
		KrablMesh.MeshEdges kmesh = new KrablMesh.MeshEdges();
		
		KrablMesh.ImportExport.UnityMeshToMeshEdges(unityMesh, kmesh);
		
		KrablMesh.CreaseDetect.ClearAllCreases(kmesh);
		kmesh.CalculateFaceVertexNormalsFromEdgeCreases();
		
		KrablMesh.ImportExport.MeshEdgesToUnityMesh(kmesh, unityMesh);
	}

}
