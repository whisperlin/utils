using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KrablMesh {
	public class Debugging  {
		static bool _areIntListsTheSame(List<int> a, List<int>b) {
			bool res = true;
			for (int i = 0; res && i < a.Count; ++i)
				if (b.Contains(a[i]) == false) res = false;
			for (int i = 0; res && i < b.Count; ++i)
				if (a.Contains(b[i]) == false) res = false;
			if (a.Count != b.Count) res = false;
			
			if (!res) {
				for (int j = 0; j < a.Count; ++j) Debug.Log("a:" + a[j]);
				for (int j = 0; j < b.Count; ++j) Debug.Log("b:" + b[j]);
			}
			return res;
		}

		public static bool CheckMeshIntegrity(MeshEdges mesh) {
			// Vertex linked faces
			int numVerts = mesh.vertCount();
			int numFaces = mesh.faceCount();
			int numEdges = mesh.edgeCount();
			List<int>[] mVertexLinkedFaces = new List<int>[numVerts];
			for (int i = 0; i < numVerts; ++i) mVertexLinkedFaces[i] = new List<int>();
			for (int i = 0; i < numFaces; ++i) {
				Face f = mesh.faces[i];
				if (f.valid) {
					for (int j = 0; j < f.cornerCount; ++j) {
						int vertIndex = f.v[j];
						if (mesh.IsVertexValid(vertIndex)) {
							mVertexLinkedFaces[vertIndex].Add(i);
						}
					}
				}
			}
			for (int i = 0; i < numVerts; ++i) {
				if (mesh.IsVertexValid(i)) {
					IndexList test = new IndexList(4);
					for (int j = 0; j < mesh.vertices[i].linkedFaces.Count; ++j) {
						if (mesh.faces[mesh.vertices[i].linkedFaces[j]].valid) 
							test.Add(mesh.vertices[i].linkedFaces[j]);
					}
					/*if (_areIntListsTheSame(mesh.vertices[i].linkedFaces, mVertexLinkedFaces[i]) == false) {
						Debug.LogError("INVALID MESH vertexLinkedFaces not correct for vertex " + i);
						return false;
					}*/
				}
			}
			
			// Vertex linked edges			
			for (int i = 0; i < numVerts; ++i) {
				if (mesh.IsVertexValid(i)) {
					List<int> li1 = new List<int>();
					mesh.CollectVerticesAroundVertex(i, ref li1);
					List<int> li2 = new List<int>();
					List<int> le = mesh.linkedEdgesForVert(i);
					for (int j = 0; j < le.Count; ++j) {
						if (mesh.IsEdgeValid(le[j])) {
							li2.Add(mesh.edges[le[j]].OtherVertex(i));
						}
					}
					if (_areIntListsTheSame(li1, li2) == false) {
						Debug.LogError("INVALID MESH vertexLinkedEdges not correct for vertex" + i);
						return false;
					}
				}
			}
			
			// Edge linked faces
			for (int i = 0; i < numEdges; ++i) {
				if (mesh.IsEdgeValid(i)) {
					IndexList test = new IndexList(18);
					mesh.CollectVertexPairFaces(mesh.edges[i], test);
				/*	if (_areIntListsTheSame(test, mesh.edges[i).linkedFaces) == false) {
						Debug.LogError("INVALID MESH edgeLinkedFaces not correct for edge" + i);
						return false;
					}*/
				}
			}				
			
			Debug.Log("Mesh integrity is good.");
			return true;
		}
		
		public static float CalculateMinTriangleShape(BaseMesh mesh) {
			int faceCount = mesh.faceCount();
			Vector3[] tr = new Vector3[3];
			float minShape = 10.0f;
			for (int i = 0; i < faceCount; ++i) {
				Face f = mesh.faces[i];
				if (f.valid) {
					for (int j = 0; j < 3; ++j) {
						tr[j] = mesh.vertices[f.v[j]].coords;
					}
					float val = KrablMesh.UnityUtils.TriangleCompactnessSqr(tr);
					if (val < minShape) minShape = val;
				}
			}
			return minShape;
		}
	}
}
