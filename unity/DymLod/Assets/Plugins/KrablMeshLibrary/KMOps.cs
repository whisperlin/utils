using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KrablMesh;

namespace KrablMesh {
	/// <summary>
	/// Some static methods to do operations on MeshEdges.
	/// </summary>
	public class Ops {	
		/// <summary>
		/// Joins vertices in a mesh that have the same geometric location based on the mesh's
		/// equalityTolerance parameter which is set when a mesh is create (it's usually zero.)
		/// </summary>
		/// <param name='mesh'>
		/// The mesh to search for duplicate vertices.
		/// </param>
		public static void RemoveDoubleVertices(BaseMesh mesh) {
			int numVerts = mesh.vertCount();
			
			List<int> vertexOrder = new List<int>(numVerts);
			for (int i = 0; i < numVerts; ++i) vertexOrder.Add(i);
			
			vertexOrder.Sort(delegate(int a, int b) {
				return mesh.CompareVertices(a, b, mesh.equalityTolerance);
			});
			
			int[] uniqueVertex = new int[numVerts];
			int unique = vertexOrder[0];
			uniqueVertex[unique] = unique;
			for (int i = 1; i < numVerts; ++i) {
				int vertexIndex = vertexOrder[i];
				if (mesh.CompareVertices(unique, vertexIndex, mesh.equalityTolerance) != 0) {
					unique = vertexIndex;						
				}
				uniqueVertex[vertexIndex] = unique;
			}
			// TODO: maybe use the center of all the almost equal vertices .. probably does not matter
			for (int i = 0; i < numVerts; ++i) {
				if (i != uniqueVertex[i]) {
					mesh.ReplaceVertex(i, uniqueVertex[i]);
				}
			}
			/* int num = */ mesh.InvalidateUnconnectedVertices();
			//Debug.Log("RemoveDoubleVertices Invalidated " + num + " unconnected vertices.");
			/*num = */ mesh.InvalidateDegenerateFaces();
			// Debug.Log("RemoveDoubleVertices Invalidated " + num + " degenerate faces.");
		}
		
		// TrisToQuads. Needs mesh with edgelist
		/// <summary>
		/// Joins neighbour triangles to quads in a mesh by dissolving selected edges. 
		/// This method needs the mesh to have its edges calculated. The edges are sorted 
		/// by their angles and dissolved in order until the maximum edge angle is reached.
		/// Special edges such as uv borders or material seams are not dissolved and concave quads
		/// are avoided. This method is used by the quad-based subdivision algorithm as it works 
		/// much better with quads.
		/// </summary>
		/// <param name='mesh'>
		/// The mesh to process.
		/// </param>
		/// <param name='maximumEdgeAngle'>
		/// The maximum angle between two triangles to be joined to a (non-planar) quad.
		/// </param>
		public static void TrisToQuads(MeshEdges mesh, float maximumEdgeAngle) {
			if (mesh.topology != MeshTopology.Triangles) {
				return;
			}
			int i, j;

			mesh.topology = MeshTopology.Mixed; // Most likely we'll end up with a mixed topology as some triangles will be left.
			mesh.CalculateFaceNormals();
			mesh.CalculateEdgeLinkedFaces();
			// Calculate the edge angles and compile list of 2-face edges
			int numEdges = mesh.edgeCount();
			List<Edge> inneredges = new List<Edge>();
			for (i = 0; i < numEdges; ++i) {
				Edge e = mesh.edges[i]
				;
				if (e.linkedFaces.Count == 2 && mesh.CanEdgeBeDissolved(i)) {
					e.mark = i; // Save the index!
					e.angle = mesh.CalculateEdgeAngle(i);
					if (e.angle < maximumEdgeAngle) {
						inneredges.Add(e);
					}
				}
			}
			// Sort by angle
			inneredges.Sort(delegate(Edge a, Edge b) { return a.angle.CompareTo(b.angle); });
		
			int iecount = inneredges.Count;
	//		Debug.Log("Number of inneredges " + iecount);
			Vector3[] qVec = new Vector3[5];
			for (i = 0; i < iecount; ++i) {
				Edge e = inneredges[i];
							
				Face f1 = mesh.faces[e.linkedFaces[0]];
				Face f2 = mesh.faces[e.linkedFaces[1]];
							
				// check if edge is between two triangles				
				if (f1.valid && f2.valid && (f1.cornerCount == 3) && (f2.cornerCount == 3)) {				
					// Make sure this doesn't lead to a concave quad
					Vector3 newNormal = f1.normal + f2.normal;
					//newNormal.Normalize();
					// Collect all vertex coordinates
					qVec[0] = mesh.vertices[e.v[0]].coords;
					qVec[2] = mesh.vertices[e.v[1]].coords;
					for (j = 0; j < 3; ++j) {
						if (e.ContainsVertex(f1.v[j]) == false) {
							qVec[1] = mesh.vertices[f1.v[j]].coords;
							break;
						}
					}
					for (j = 0; j < 3; ++j) {
						if (e.ContainsVertex(f2.v[j]) == false) {
							qVec[3] = mesh.vertices[f2.v[j]].coords;
							break;
						}
					}
					// Flip order if it doesn't conform to f1
					if (f1.VerticesInOrder(e.v[0], e.v[1]) == true) {
						Vector3 temp = qVec[1]; qVec[1] = qVec[3]; qVec[3] = temp;
					}
					// calculate edge vectors
					qVec[4] = qVec[0];
					for (j = 0; j < 4; ++j) {
						qVec[j] -= qVec[j + 1];
					}
					qVec[4] = qVec[0];
					
					bool convex = true;
					for (j = 0; j < 4; ++j) {
						Vector3 localN = Vector3.Cross(qVec[j], qVec[j + 1]);
						//localN.Normalize();
						float nAngleCos = Vector3.Dot(newNormal, localN);
						//Debug.Log("Angle " + nAngleCod);
						if (nAngleCos <= 0.0f) { 
							convex = false; 
							break; 
						}
					}
					
					if (convex) {
		//				Debug.Log("Dissolving edge");
						mesh.DissolveEdgeTriangles(e.mark);
					} else {
				//		Debug.Log("not dissolving edge -> concave quad");
					}
				}
			}
			
			// There are now invalid faces and edges
			mesh.RebuildMesh();
			mesh.GenerateEdgeTopology();
		}
		
		#region -- Triangulation --
		
		public static void TriangulateWithEdges(MeshEdges mesh) {
			if (mesh.topology == MeshTopology.Triangles) return;
			
			int oldNumFaces = mesh.faceCount();
			Triangulate((BaseMesh)mesh);
			int newNumFaces = mesh.faceCount();
			
			// Iterate through the new faces
			for (int faceIndex = oldNumFaces; faceIndex < newNumFaces; ++faceIndex) {
				Face f = mesh.faces[faceIndex];
				Edge e = new Edge(f.v[0], f.v[1]); // the new edge is always 0->1 because triangulate is constructed to do that.
				mesh.AddEdge(e);
			}
			mesh.RegenerateVertexLinkedEdges();
			mesh.GenerateEdgeTopology();
			//mesh.GenerateEdgeList(); // not good! Looses edge information!
			// What was really needed is looking for the new edges created.. leave the old ones alone!
			//mesh.CalculateEdgeLinkedFaces();
		}

		public static void Triangulate(BaseMesh mesh) {
			if (mesh.topology == MeshTopology.Triangles) return;
			
			int faceIndex, cornerIndex;
			
			int numFaces = mesh.faceCount();
			Vector3[] coords = new Vector3[4];
			for (faceIndex = 0; faceIndex < numFaces; ++faceIndex) {
				Face f = mesh.faces[faceIndex];
				if (f.valid && f.cornerCount == 4) {
					Face fnew = new Face(3);
					// copy face parameters
					fnew.material = f.material;
					for (cornerIndex = 0; cornerIndex < 4; ++cornerIndex) {
						coords[cornerIndex] = mesh.vertices[f.v[cornerIndex]].coords;
					}
					
					Vector3 diag02 = coords[0] - coords[2];
					Vector3 diag13 = coords[1] - coords[3];
					if (diag02.sqrMagnitude < diag13.sqrMagnitude) {
						// Create triangles 201 and 023 .. use diag as first edge as a hint
						fnew.CopyVertexInfoFromFace(f, 0, 0);
						fnew.CopyVertexInfoFromFace(f, 2, 1);
						fnew.CopyVertexInfoFromFace(f, 3, 2);
					} else {		
						// Create triangles 312 and 013 .. use diag as first edge as a hint
						fnew.CopyVertexInfoFromFace(f, 3, 0);
						fnew.CopyVertexInfoFromFace(f, 1, 1);
						fnew.CopyVertexInfoFromFace(f, 2, 2);
						f.CopyVertexInfoFromFace(f, 3, 2);
					}
					mesh.AddFace(fnew);
					// First face needs to become a triangle
					f.cornerCount = 3;
				}
			}
			// Rebuild linkedFaces for Verts
			int numVerts = mesh.vertCount();
			for (int vertIndex = 0; vertIndex < numVerts; ++vertIndex) {
				mesh.vertices[vertIndex].linkedFaces = new IndexList(18);
			}
			numFaces = mesh.faceCount();
			
			for (faceIndex = 0; faceIndex < numFaces; ++faceIndex) {
				Face f = mesh.faces[faceIndex];
				for (cornerIndex = 0; cornerIndex < f.cornerCount; ++cornerIndex) {
					mesh.vertices[f.v[cornerIndex]].linkedFaces.Add(faceIndex);
				}
			}
			mesh.topology = MeshTopology.Triangles;
		}
		
		#endregion
	}
}