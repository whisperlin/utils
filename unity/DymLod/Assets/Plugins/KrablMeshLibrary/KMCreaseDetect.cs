using UnityEngine;
using System.Collections;

namespace KrablMesh {
	/// <summary>
	/// Static methods to deal with marking edges as creases.
	/// </summary>
	public class CreaseDetect {
		/// <summary>
		/// Searches the mesh for edges that are connected to faces of different materials (submeshes). These are
		/// marked as creases.
		/// </summary>
		/// <param name='mesh'>
		/// The mesh to search.
		/// </param>
		/// <param name='creaseStrength'>
		/// The crease value to set the edges to. Currently only 1.0f has any effect.
		/// </param>
		public static void MarkCreasesFromMaterialSeams(MeshEdges mesh, float creaseStrength = 1.0f) {
			int numEdges = mesh.edgeCount();
			for (int edgeIndex = 0; edgeIndex < numEdges; ++edgeIndex) {
				if (mesh.isEdgeMaterialSeam(edgeIndex)) {
					mesh.edges[edgeIndex].crease = creaseStrength;
				}
			}
		}
		
		/// <summary>
		/// Searches through all edges of the mesh and compares the vertex normals on each face connected
		/// to an edge. If there is a difference in the vertex normals, the edge gets marked as crease.
		/// This method is automatically executed every time a Unity mesh is converted to a Krabl Mesh Library mesh.
		/// </summary>
		/// <param name='mesh'>
		/// The mesh to search.
		/// </param>
		/// <param name='creaseStrength'>
		/// The crease value to set the edges to. Currently only 1.0f has any effect.
		/// </param>
		public static void MarkCreasesFromFaceNormals(MeshEdges mesh, float creaseStrength = 1.0f) {
			mesh.CalculateEdgeLinkedFaces();
			int count = mesh.edgeCount();
			for (int edgeIndex = 0; edgeIndex < count; ++edgeIndex) {
				Edge edge = mesh.edges[edgeIndex];
				int v1 = edge.v[0];
				int v2 = edge.v[1];
				if (edge.linkedFaces.Count == 2) {
					Face fa = mesh.faces[edge.linkedFaces[0]];
					Face fb = mesh.faces[edge.linkedFaces[1]];
					Vector3 n1a = fa.VertexNormalForVertexIndex(v1);
					Vector3 n2a = fa.VertexNormalForVertexIndex(v2);
					Vector3 n1b = fb.VertexNormalForVertexIndex(v1);
					Vector3 n2b = fb.VertexNormalForVertexIndex(v2);
					if (KrablMesh.UnityUtils.Vector3CompareWithTolerance(n1a, n1b, mesh.equalityTolerance) != 0
						|| KrablMesh.UnityUtils.Vector3CompareWithTolerance(n2a, n2b, mesh.equalityTolerance) != 0) {
						edge.crease = creaseStrength;
					}
				} 
			}
		}
		
		/// <summary>
		/// Searches through all edges of the mesh and calculates the angle between the faces connected to the edge.
		/// The the angle is above the threshold, the edge is marked as a crease.
		/// </summary>
		/// <param name='mesh'>
		/// The mesh to search.
		/// </param>
		/// <param name='angleThreshold'>
		/// All edges with angles equal or larger than this are marked as creases.
		/// </param>
		/// <param name='creaseStrength'>
		/// The crease value to set the edges to. Currently only 1.0f has any effect.
		/// </param>
		public static void MarkCreasesFromEdgeAngles(MeshEdges mesh, float angleThreshold, float creaseStrength = 1.0f) {
			mesh.CalculateFaceNormals();
			int numEdges = mesh.edgeCount();
			for (int edgeIndex = 0; edgeIndex < numEdges; ++edgeIndex) {
				if (mesh.CalculateEdgeAngle(edgeIndex) >= angleThreshold) {
					mesh.edges[edgeIndex].crease = creaseStrength;
				}
			}
		}
		
		/// <summary>
		/// Sets the crease value of all edges in the mesh to 0.
		/// </summary>
		/// <param name='mesh'>
		/// The mesh to process.
		/// </param>
		public static void ClearAllCreases(MeshEdges mesh) {
			int numEdges = mesh.edgeCount();
			for (int edgeIndex = 0; edgeIndex < numEdges; ++edgeIndex) {
				mesh.edges[edgeIndex].crease = 0.0f;
			}
		}
	}
}
