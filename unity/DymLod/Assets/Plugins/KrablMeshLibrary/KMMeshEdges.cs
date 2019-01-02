using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace KrablMesh {	
	/// <summary>
	/// A polygonal mesh with edge information.
	/// </summary>
	public class MeshEdges : KrablMesh.BaseMesh {
		public List<Edge> edges = null;
		protected List<int>[] linkedEdges = null; // List of edge indexes linked to each vertex
		
		public int edgeCount() { return (edges != null) ? edges.Count : -1; }
		public List<int> linkedEdgesForVert(int vertexIndex) { return linkedEdges[vertexIndex]; }
		
		public void AddEdge(Edge e) {
			edges.Add(e);
		}
		
		public bool IsEdgeValid(int edgeIndex) {
			Edge e = edges[edgeIndex];
			if (e.linkedFaces.Count == 0) return false;
			return isVertexPairValid(e); 
		}

		public float EdgeLength(int edgeIndex) 
		{
			int[] v = edges[edgeIndex].v;				
			return Vector3.Distance(vertices[v[0]].coords, vertices[v[1]].coords);
		}

		public float EdgeLengthSqr(int edgeIndex) 
		{
			int[] v = edges[edgeIndex].v;				
			return (vertices[v[0]].coords - vertices[v[1]].coords).sqrMagnitude;
		}

		public void UnlinkEdge(int edgeIndex) {
			if (IsEdgeValid(edgeIndex)) {
			} else {
				Edge e = edges[edgeIndex];
				for (int i = 0; i < 2; ++i) {
					linkedEdges[e.v[i]].Remove(edgeIndex);
				}
				e.Invalidate();			
			}
		}
		
		public void ClearEdges() 
		{
			edges = null;
			linkedEdges = null;
		}
		
		public bool IsEdgeBorder(int edgeIndex) 
		{
			return (edges[edgeIndex].linkedFaces.Count == 1);
		}
		
		// TODO: do this better with one call to determine various edge flags
		// Requires edges.linkedfaces to be valid!
		public bool IsEdgeUV1Seam(int edgeIndex) 
		{
			Edge e = edges[edgeIndex];
			IndexList linkedFaces = e.linkedFaces;
			switch (linkedFaces.Count) {
			case 0:
			case 1:
				return false;
			case 2:
				// Check if the verts have the same uv2 coords on both faces
				Face f0 = faces[linkedFaces[0]];
				Face f1 = faces[linkedFaces[1]];
				int vi0 = e.v[0];
			//	Debug.Log("Edge " + e.v[0] + "->" + e.v[1]);
			//	Debug.Log("Face0 n " + f0.cornerCount + " " + f0.v[0] + " " + f0.v[1] + " " + f0.v[2] + " " + f0.v[3]);
				if (f0.uv1[f0.CornerIndex(vi0)] != f1.uv1[f1.CornerIndex(vi0)]) return true;
				int vi1 = e.v[1];
			//	Debug.Log("Face1 n + " + f1.cornerCount + " " + f1.v[0] + " " + f1.v[1] + " " + f1.v[2] + " " + f1.v[3]);
				return (f0.uv1[f0.CornerIndex(vi1)] != f1.uv1[f1.CornerIndex(vi1)]); 
			default: // > 2
				return true; 
			}
		}

		// requires edges.linkedfaces to be valid
		public bool IsEdgeUV2Seam(int edgeIndex) 
		{
			Edge e = edges[edgeIndex];
			IndexList linkedFaces = e.linkedFaces;
			switch (linkedFaces.Count) {
			case 0:
			case 1:
				return false; 
			case 2:
				// Check if the verts have the same uv2 coords on both faces
				Face f0 = faces[linkedFaces[0]];
				Face f1 = faces[linkedFaces[1]];
				int vi0 = e.v[0];
				if (f0.uv2[f0.CornerIndex(vi0)] != f1.uv2[f1.CornerIndex(vi0)]) return true;
				int vi1 = e.v[1];
				return (f0.uv2[f0.CornerIndex(vi1)] != f1.uv2[f1.CornerIndex(vi1)]); 
			default:
				return true;
			}
		}
		
		// requires edges.linkedfaces to be valid
		public bool isEdgeMaterialSeam(int edgeIndex)
		{
			IndexList linkedFaces = edges[edgeIndex].linkedFaces;
			if (linkedFaces.Count < 2) return false;
			
			int mat = faces[linkedFaces[0]].material;
			for (int i = 1; i < linkedFaces.Count; ++i) {
				if (mat != faces[linkedFaces[i]].material) return true;
			}
			return false;
		}

		// ALGORITHMS //
		
		public int EdgeIndexForVertices(int vindex0, int vindex1) {
			int edgeIndex;
			List<int> linkedEdgeIndex = linkedEdges[vindex0];
			int count = linkedEdgeIndex.Count;
			
			for (int i = 0; i < count; ++i) {
				edgeIndex = linkedEdgeIndex[i];
				if (edges[edgeIndex].OtherVertex(vindex0) == vindex1) return edgeIndex;
			}
		//	Debug.LogError("Bad bad bad. Linked edge that doesn't contain the linked vertex v0:" + vindex0 + " v1: " + vindex1 + " linkedEdgeCountV0: " + linkedEdgeIndex.Count);
			return -1;
		}
		
		public void GenerateEdgeTopology() {
			CalculateEdgeLinkedFaces();
		}
		
		public void GenerateEdgeList() {
			int numVerts = vertCount();
			
			// alloc new arrays, let gc do the cleanup
			edges = new List<Edge>();
			
			linkedEdges = new List<int>[numVerts];
			for (int i = 0; i < numVerts; ++i) {
				linkedEdges[i] = new List<int>();
			}
			
			List<int> surroundingVerts = new List<int>();
			for (int i = 0; i < numVerts; ++i) {
				surroundingVerts.Clear();
				CollectVerticesAroundVertex(i, ref surroundingVerts);
				int count = surroundingVerts.Count;
				for (int j = 0; j < count; ++j) {
					int vertexIndex = surroundingVerts[j];
					if (i < vertexIndex) {
						Edge edge = new Edge(i, vertexIndex);
						int edgeIndex = edges.Count;
						linkedEdges[i].Add(edgeIndex);
						linkedEdges[vertexIndex].Add(edgeIndex);
						edges.Add(edge);
					}
				}
			}				
		}
		
		public void RegenerateVertexLinkedEdges() {
			int numVerts = vertCount();
			int numEdges = edges.Count;
			linkedEdges = new List<int>[numVerts];
			for (int vertIndex = 0; vertIndex < numVerts; ++vertIndex) {
				linkedEdges[vertIndex] = new List<int>();
			}
		
			for (int edgeIndex = 0; edgeIndex < numEdges; ++edgeIndex) {
				int[] vi = edges[edgeIndex].v;
				linkedEdges[vi[0]].Add(edgeIndex);
				linkedEdges[vi[1]].Add(edgeIndex);
			}
		}
				
		public void CalculateEdgeLinkedFaces() {
			int numEdges = edgeCount();
			for (int i = 0; i < numEdges; ++i) {
				Edge edge = edges[i];
				edge.linkedFaces.Clear();
				CollectVertexPairFaces(edge, edge.linkedFaces);
			/*	if (edge.linkedFaces.Count > 2) {
					Debug.Log("WARNING! An edge has " + edge.linkedFaces.Count + " faces!");
				}*/
			}
		}
		
		public List<int>[] CalculateFaceLinkedEdges() {
			// Needs linkedFaces on edges to be valid !!!!
			int numFaces = faceCount();
			List<int>[] result = new List<int>[numFaces];
			for (int i = 0; i < numFaces; ++i) {
				result[i] = new List<int>();
			}
			
			int numEdges = edgeCount();
			for (int i = 0; i < numEdges; ++i) {
				IndexList edgeFaces = edges[i].linkedFaces;
				for (int j = 0; j < edgeFaces.Count; ++j) {
					result[edgeFaces[j]].Add(i);
				}
			}
			return result;
		}
		
		void _markGroupFaceNeightbours(int vertexIndex, int faceIndex, ref List<int>[] faceLinkedEdges, int groupIndex) {
			Stack<int> stack = new Stack<int>();
			stack.Push(faceIndex);
			do {
				faceIndex = stack.Pop();
				Face f = faces[faceIndex];
				if (f.mark == -1) {
					f.mark = groupIndex; // mark face group. -1 = available
					// locate neighbours connected by non-creases
					List<int> faceEdges = faceLinkedEdges[faceIndex];
					// Search for other faces connected with non-crease edges
					for (int e = 0; e < faceEdges.Count; ++e) {
						Edge edge = edges[faceEdges[e]];
						if (IsEdgeValid(faceEdges[e]) && edge.crease < 1.0f && edge.ContainsVertex(vertexIndex)) {
							IndexList edgeFaces = edge.linkedFaces;
							for (int k = 0; k < edgeFaces.Count; ++k) {
								int edgeFaceIndex = edgeFaces[k];
								if (faces[edgeFaceIndex].mark == -1) {
									stack.Push(edgeFaceIndex);
								}
							}
						}
					}
				}
			} while (stack.Count > 0);
		}
		
		public void CalculateFaceVertexNormalsFromEdgeCreasesForVertex(int vertexIndex, ref List<int>[] faceLinkedEdges)
		{ 
			int i,j, grp;
			List<int> grpCornerIndex = new List<int>();
			
			Vertex v = vertices[vertexIndex];
			IndexList vertexFaces = v.linkedFaces;
			int vertexFaceCount = vertexFaces.Count;
			//	List<int> vertexEdges = linkedEdges[vertexIndex];
			// Clear face marks around vertex
			for (j = 0; j < vertexFaceCount; ++j) {
				faces[vertexFaces[j]].mark = -1; // TODO: could be faster with uniqueTag
			}
			// This will add each facemark to a groupIndex
			int groupIndex = 0;
			for (j = 0; j < vertexFaceCount; ++j) {
				int faceIndex = vertexFaces[j];
				if (faces[faceIndex].mark == -1) { // face still available
					_markGroupFaceNeightbours(vertexIndex, vertexFaces[j], ref faceLinkedEdges, groupIndex);
					groupIndex++;
				}
			}
			// Build group arrays
			List<int>[] groups = new List<int>[groupIndex]; // are these too many allocations?
			for (i = 0; i < groupIndex; ++i) groups[i] = new List<int>();
			for (i = 0; i < vertexFaceCount; ++i) {
				int faceIndex = vertexFaces[i];
				int mark = faces[faceIndex].mark;
				groups[mark].Add(faceIndex);
			}
					
			// Calculate and set normal for each face on the vertex based on the groups
			Vector3 normal;
			for (grp = 0; grp < groupIndex; ++grp) {
				normal = Vector3.zero;
				List<int> grpFaces = groups[grp];
				int cnt = grpFaces.Count;
				grpCornerIndex.Clear();
				for (i = 0; i < cnt; ++i) {								
					Face f = faces[grpFaces[i]];
					if (f.normal == Vector3.zero) {
					//	Debug.Log("face has zero normal .. valid " + f.valid);
					}
					// Multiply with corner angle (=SLOW?)
					int corner = f.CornerIndex(vertexIndex);
					float fact = CornerAngle(grpFaces[i], corner); 
					normal += f.normal*fact; 
					grpCornerIndex.Add(corner);
				}
				UnityUtils.NormalizeSmallVector(ref normal);
				if (normal == Vector3.zero) {
				//	Debug.Log("NORMAL == ZERO facecount " + cnt);
				}
				// Now set the normal to all group faces
				for (i = 0; i < cnt; ++i) {
					faces[grpFaces[i]].vertexNormal[grpCornerIndex[i]] = normal;
				}
			}			
		}
		
		/// <summary>
		/// Calculates the face vertex normals based on the edge crease information. Edges marked as
		/// creases will get split normals.
		/// </summary>
		public void CalculateFaceVertexNormalsFromEdgeCreases() {
			CalculateFaceNormals(); // maybe not needed
			CalculateEdgeLinkedFaces(); // maybe not needed
			List<int>[] faceLinkedEdges = CalculateFaceLinkedEdges();
			int numVerts = vertCount();
			for (int vertexIndex = 0; vertexIndex < numVerts; ++vertexIndex) {
				if (IsVertexValid(vertexIndex)) {
					CalculateFaceVertexNormalsFromEdgeCreasesForVertex(vertexIndex, ref faceLinkedEdges);
				}
			}
		}
					
		public new int CollapseVertexPair(CollapseInfo info) {
			VertexPair pair = info.vp;
			int v0 = pair.v[0];
			int v1 = pair.v[1];
			List<int> v0Edges = linkedEdges[v0];
			List<int> v1Edges = linkedEdges[v1];
			int i;
			// Update edges 
			// Mark the vertices that are connected by edges
			for (i = 0; i < v1Edges.Count; ++i) {
				int edgeIndex = v1Edges[i];
				int other = edges[edgeIndex].OtherVertex(v1);
				vertices[other].mark = -1;
			}
			for (i = 0; i < v0Edges.Count; ++i) {
				int edgeIndex = v0Edges[i];
				int other = edges[edgeIndex].OtherVertex(v0);
				vertices[other].mark = edgeIndex;
			}
			// now v1 verts that are only connected to v1 have value -1, double edge-connected verts have the edgeindex as mark				
			for (i = 0; i < v1Edges.Count; ++i) {
				int edgeIndex = v1Edges[i];
				if (vertices[edges[edgeIndex].OtherVertex(v1)].mark == -1) {
					edges[edgeIndex].ReplaceVertex(v1, v0);
					if (IsEdgeValid(edgeIndex)) {
						v0Edges.Add(edgeIndex);
					}
				} else {
					Edge e1 = edges[edgeIndex];
					int vindex = e1.OtherVertex(v1);
					Edge e0 = edges[vertices[vindex].mark]; // vertex mark is edge index!
					// There has to be another edge connecting v0 to vertex vindex
					e0.crease = Mathf.Max(e1.crease, e0.crease); // keep the max crease value
					UnlinkEdge(edgeIndex); // no more need for this!
				}
			}
			// Remove invalid edges from mesh
			for (i = v0Edges.Count - 1; i >= 0; --i) { // backwards should be faster and i stays valid!
				int edgeIndex = v0Edges[i];
				if (IsEdgeValid(edgeIndex) == false) {
					UnlinkEdge(edgeIndex);
					//v0Edges.Remove(edgeIndex);
				} 
			}

			// Deal with vertices and faces in baseclass
			info.vp = new VertexPair(v0, v1); // the original might have been invalidated THIS IS BAD
			base.CollapseVertexPair(info);
			v1Edges.Clear();
			
			// rebuild linkedfaces for the remaining edges
			for (i = 0; i < v0Edges.Count; ++i) {
				Edge edge = edges[v0Edges[i]];
				edge.linkedFaces.Clear();
				CollectVertexPairFaces(edge, edge.linkedFaces);
			}
			
			return v0;
		}
		
		public void RebuildMesh(bool verbose = false)
		{
	/*		for (int i = 0; i < edgeCount; ++i) {
				if (IsEdgeValid(i)) {
					edgeTable[i] = newEdges.Count;
					newEdges.Add(edges[i]);
				}
			} */
			
			int[] vertexTable = null;
			int[] faceTable = null; // not really used for now
			base.RebuildMesh(ref vertexTable, ref faceTable, verbose);
			
			int edgeCount = edges.Count;
			List<Edge> newEdges = new List<Edge>();
			//edges = newEdges;
			//edgeCount = edges.Count;
			for (int i = 0; i < edgeCount; ++i) {
				Edge e = edges[i];
				if (e.v[0] != e.v[1]) {
					e.v[0] = vertexTable[e.v[0]];
					e.v[1] = vertexTable[e.v[1]];
					if (e.linkedFaces.Count > 0 && e.v[0] != -1 && e.v[1] != -1 && e.v[0] != e.v[1]) { // -1 is for invalid verts
						newEdges.Add(e);
					}
				}
				e.linkedFaces.Clear();
			}
			edges = newEdges;
			if (verbose) {
				Debug.Log("Rebuild Edge Count " + edgeCount + " -> " + newEdges.Count);
			}
			
			RegenerateVertexLinkedEdges();
			CalculateEdgeLinkedFaces();
		}
		
		public float CalculateEdgeAngle(int edgeIndex) {
			IndexList edgefaces = edges[edgeIndex].linkedFaces;
			if (edgefaces.Count != 2) return 180.0f;
			Face f1 = faces[edgefaces[0]];
			Face f2 = faces[edgefaces[1]];
			return Vector3.Angle(f1.normal, f2.normal);
		}
		
		public bool CanEdgeBeDissolved(int edgeIndex) {
			// TODO!
			Edge e = edges[edgeIndex];
			// Don't dissolve creases
			if (e.crease == 1.0f) return false;
			// Only dissolve edges with two linked faces
			if (e.linkedFaces.Count != 2) return false;
			
			int f1index = e.linkedFaces[0];
			int f2index = e.linkedFaces[1];
			Face f1 = faces[f1index];
			Face f2 = faces[f2index];
			// Don't dissolve material borders
			if (f1.material != f2.material) return false;
				
			int f1corner1 = f1.CornerIndex(e.v[0]);
			int f1corner2 = f1.CornerIndex(e.v[1]);
			int f2corner1 = f2.CornerIndex(e.v[0]);
			int f2corner2 = f2.CornerIndex(e.v[1]);
			if (hasUV1) {
				// Don't dissolve uv1 borders
				if (f1.uv1[f1corner1] != f2.uv1[f2corner1]) return false;
				if (f1.uv1[f1corner2] != f2.uv1[f2corner2]) return false;
			}
			if (hasUV2) {
				if (f1.uv2[f1corner1] != f2.uv2[f2corner1]) return false;
				if (f1.uv2[f1corner2] != f2.uv2[f2corner2]) return false;
			}
			return true;
		}
		
		// Dissolve an edge between two triangles.
		// the first is a quad afterwards and the second triangle is destroyed
		// expects valid input, no checks!
		public void DissolveEdgeTriangles(int edgeIndex) {
			int i;
			
			Edge e = edges[edgeIndex];
			// e needs to have two linked faces
			int f1index = e.linkedFaces[0];
			int f2index = e.linkedFaces[1];
			Face f1 = faces[f1index];
			Face f2 = faces[f2index];
			// f1 and f2 have two common vertices, need to be triangles
			int v2uniqueCorner = -1;
			for (i = 0; i < 3; ++i) {
				if (e.ContainsVertex(f2.v[i]) == false) {
					v2uniqueCorner = i;
					break;
				}
			}
			for (i = 0; i < 3; ++i) {
				if (e.ContainsVertex(f1.v[i]) == false) {
					int insertPos = i + 2;
					if (insertPos >= 3) insertPos -= 3;
					
					f1.cornerCount = 4; // f1 is now a quad
					UnlinkFace(f2index);
					vertices[f2.v[v2uniqueCorner]].linkedFaces.Add(f1index);
					// Move verts backwards
					for (int j = 3; j > insertPos; --j) {
						f1.CopyVertexInfoFromFace(f1, j - 1, j);
					}
					f1.CopyVertexInfoFromFace(f2, v2uniqueCorner, insertPos);
					e.linkedFaces.Clear();
					break;
				}
			}
		}
		
		public override void Clear() {
			ClearEdges();
			base.Clear();
		}
	}
}