using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KrablMesh;


namespace KrablMesh {	
	/// <summary>
	/// The parameters used for a edge-collapse simplification operation.
	/// </summary>
	[System.Serializable]
	public class SimplifyParameters {
		/// <summary>
		/// The number of edges to collapse. -1: don't use this as completion condition.
		/// </summary>
		public int edgesToCollapse = -1;
		
		/// <summary>
		/// The number of triangles the mesh should have after collapsing. -1: don't use this as completion condition.
		/// It's possible the result mesh will have 1 or 2 tris less that this parameters as each edge collapse affect multiple triangles.
		/// </summary>
		public int targetFaceCount = -1;
		
		/// <summary>
		/// The maximum allowed error introduced by a collapse. This can be used to keep a constant mesh quality, a constant minimum curvature.
		/// It has a range of 0..1000. A value of x means, a change of x/1000 of the mesh size is acceptable.
		/// -1: don't use this as completion condition.
		/// </summary>
		public float maximumError = -1;
		
		/// <summary>
		/// Allow the algorithm to move vertices. This leads to more calculations, but also to a more accurate mesh shape.
		/// </summary>
		public bool recalculateVertexPositions = true;
		
		/// <summary>
		/// Disallow the algorithm from producing non manifold edges. This does also affect intermediate steps and leads to independent volumes
		/// never being completely dissolved. For small target triangle counts, always set this to false.
		/// </summary>
		public bool preventNonManifoldEdges = false;
		
		/// <summary>
		/// How much to protect border edges during simplification. 1 is a good value. Very high values (~10000) lead to border edges not being collapsed at all.
		/// </summary>
		public float borderWeight = 1.0f;
		
		/// <summary>
		/// How much to protect edges with different materials/submeshes on it sides. 0: treat like normal edges, 1: protect reasonably, 10000: do not collapse any of these edges.
		/// </summary>
		public float materialSeamWeight = 1.0f;
		
		/// <summary>
		/// How much to protect edges at uv island borders/uv seams. 0: treat like normal edges, 1: protect reasonably, 10000: do not collapse any of these edges.
		/// </summary>
		public float uvSeamWeight = 1.0f;
		
		/// <summary>
		/// How much to protect edges at uv island borders/uv seams. 0: treat like normal edges, 1: protect reasonably, 10000: do not collapse any of these edges.
		/// </summary>
		public float uv2SeamWeight = 0.0f;
		
		/// <summary>
		/// How much to additionally protect edges that are marked as creases. Creases usually are already protected by the mesh shape as collapsing a geometrical crease 
		/// produces a very high error value. 0: treat like normal edges, 1: protect reasonably, 10000: do not collapse any of these edges.
		/// </summary>
		public float creaseWeight = 0.0f;
		
		/// <summary>
		/// How much to additionally protect edges that have vertices with different boneweights, weighted by the calculated difference of these boneweights. 
		/// Use this to end up with more vertices in flexible areas of skinned meshes. 0: treat like normal edges, 1: protect reasonably, 10000: do not collapse any of these edges.
		/// </summary>
		public float boneWeightProtection = 0.1f;
		
		/// <summary>
		/// How much to additionally protect edges that have vertices with different vertex colors, weighted by the calculated difference of these colors. 
		/// 0: treat like normal edges, 1: protect reasonably, 10000: do not collapse any of these edges.
		/// </summary>
		public float vertexColorProtection = 0.0f;
		
		/// <summary>
		/// The maximum allowed number of edges/faces connected to one vertex. Decrease this to get a nicer mesh topology with less 'stars', 
		/// but more triangles for the same shape. The default value of 18 is usually never reached. 
		/// 0 means do not execute this check.
		/// </summary>
		public int maxEdgesPerVertex = 18; 
		
		/// <summary>
		/// Check the result topology of collapses. This will prevent overlapping triangles and generally lead to a better result mesh topology.
		/// </summary>
		public bool checkTopology = true;
	//	public float minTriangleShape = 0.12f;
	}
	
	/// <summary>
	/// An edge-collapsed based simplification algorithm MeshEdges.
	/// </summary>
	public class Simplify {
		SimplifyParameters _parameters;
			
		public ProgressDelegate progressDelegate = null;

		const bool localizeErrors = true;

		// Penalties
		const float kMeshPenaltyNonManifold = 1e7f;
		const float kMeshPenaltyBadTopology = 1e6f;
		const float kMeshPenaltyMaxEdgesPerVertex = 1e5f;
//		const float kMeshPenaltySharp = 1e4f;
		
		//public float sharpnessLimitSqr = 0.0f; // 1 == equilateral 0 = sliver
		
		MinHeap<CollapseInfo> heap = null;
		HeapNode<CollapseInfo>[] heapNodes = null;
		PlaneDistanceError[] pdePerVertex = null;
		MeshEdges mesh = null;
		
		const int kProgressGroups = 100;
		const float kProgressInterval = 0.2f;
		
		float meshSize = 1.0f; // The width on the largest axis
		bool noPenalties = false;
		
		/// <summary>
		/// Perform mesh simplification.
		/// </summary>
		/// <param name='mesh'>
		/// A mesh (KrablMesh.MeshEdges) to simplify. The mesh will be changed and might even be replaced by a new mesh structure.
		/// </param>
		/// <param name='parameters'>
		/// The parameters to use during the simplification.
		/// </param>
		public void Execute(ref MeshEdges mesh, SimplifyParameters parameters) {			
			if (mesh.vertCount() == 0) return;
			
			if (mesh.topology != MeshTopology.Triangles) {
				Ops.TriangulateWithEdges(mesh);
			}
			
			if (progressDelegate != null) progressDelegate("Initialize", 0.0f);
			InitializeCollapsing(mesh, parameters);
			if (parameters.edgesToCollapse > 0) {
				Collapse(parameters.edgesToCollapse);
			} else if (parameters.targetFaceCount > 0) {
				int totalFacesToRemove = mesh.numValidFaces - parameters.targetFaceCount;
				int progressCounter = 0;
				float t = Time.realtimeSinceStartup + kProgressInterval;
				while (mesh.numValidFaces > parameters.targetFaceCount) {
					Collapse(1);
					progressCounter--;
					if (progressCounter <= 0) {
						progressCounter = kProgressGroups;
						if (Time.realtimeSinceStartup - t > kProgressInterval && progressDelegate != null) {
							t = Time.realtimeSinceStartup;
							int facesRemoved = totalFacesToRemove - (mesh.numValidFaces - parameters.targetFaceCount);
							progressDelegate("Mesh Faces " + mesh.numValidFaces + "->" + parameters.targetFaceCount, 0.1f + 0.9f*((float)facesRemoved)/((float)totalFacesToRemove));
						}
					}
				}
			} else if (parameters.maximumError > 0) {
				// Determine size of mesh to scale errors
				meshSize = _determineMeshSize(mesh);
				int totalFacesToRemove = mesh.numValidFaces;
				float t = Time.realtimeSinceStartup + kProgressInterval;
				float costThreshold = meshSize*parameters.maximumError*0.001f;
				costThreshold *= costThreshold;
				int progressCounter = 0;
				while (true) {
					int num = Collapse(1, costThreshold);
					if (num == 0) break;
					progressCounter--;
					if (progressCounter <= 0) {
						progressCounter = kProgressGroups;
						if (Time.realtimeSinceStartup - t > kProgressInterval && progressDelegate != null) {
							t = Time.realtimeSinceStartup;
							int facesRemoved = totalFacesToRemove - (mesh.numValidFaces - parameters.targetFaceCount);
							progressDelegate("Mesh Faces " + mesh.numValidFaces + "->" + parameters.targetFaceCount, 0.1f + 0.9f*((float)facesRemoved)/((float)totalFacesToRemove)); // TODO: do this better
						}
					}
				} 
			}
			//Debugging.CheckMeshIntegrity(mesh);
			mesh.RebuildMesh();
			
			/*float minShape = Debugging.CalculateMinTriangleShape(mesh);
			Debug.LogError("MIN TRIANGLE SHAPE" + Mathf.Sqrt(minShape) + " param " + Mathf.Sqrt(sharpnessLimitSqr));
			*/
			Cleanup();
		}
		
		// the mesh needs to have its edge list and creases calculated
		public void InitializeCollapsing(MeshEdges _mesh, SimplifyParameters parameters) {
			mesh = _mesh;
			_parameters = parameters;
			// Gain some time by avoiding unecessary calculations
			if (_mesh.hasBoneWeights == false) parameters.boneWeightProtection = 0.0f;
			if (_mesh.hasVertexColors == false) parameters.vertexColorProtection = 0.0f;

			_calculatePdePerVertex();
			// construct heap
			int numEdges = mesh.edgeCount();
			heapNodes = new HeapNode<CollapseInfo>[numEdges];
			heap = new MinHeap<CollapseInfo>();				
			int progressCounter = kProgressGroups;
			float t = Time.realtimeSinceStartup;
			for (int i = 0; i < numEdges; ++i) {
				CollapseInfo pc = new CollapseInfo();
				_calculateEdgeCost(i, pc);
				heapNodes[i] = heap.Insert(new HeapNode<CollapseInfo>(pc.cost, pc));
				progressCounter--;
				if (progressCounter <= 0) {
					progressCounter = kProgressGroups;
					if (Time.realtimeSinceStartup - t > kProgressInterval && progressDelegate != null) {
						t = Time.realtimeSinceStartup;
						progressDelegate("Initialize Edge " + i + "/" + numEdges, 0.1f*((float)i)/((float)numEdges));
					}
				}
			}
			
			// shortcut for fastest calc
			noPenalties = (parameters.checkTopology == false 
				&& parameters.maxEdgesPerVertex == 0 
				&& parameters.preventNonManifoldEdges == false
				&& parameters.boneWeightProtection <= 0.0f
				&& parameters.vertexColorProtection <= 0.0f);
			//sharpnessLimitSqr = parameters.minTriangleShape*parameters.minTriangleShape;
			//if (parameters.minTriangleShape < 0.0f) sharpnessLimitSqr = 0.0f;
		}
		
		public void Cleanup() {
			_parameters = null;
			progressDelegate = null;
			pdePerVertex = null;
			heapNodes = null;
			heap = null;
			mesh = null;
		}
		
		List<int> _edgesToUpdate = new List<int>(32);
		List<int> _surroundingVerts = new List<int>(32);
				
		public int Collapse(int numEdgesToCollapse = 1, float maxCost = 1e6f) {
			int collapsesDone = 0;
			while (numEdgesToCollapse > 0) {
				HeapNode<CollapseInfo> node;
				do {
					node = heap.Extract();
				} while (node != null && mesh.isVertexPairValid(node.obj.vp) == false);
				if (node == null) break;
				
				if (node.heapValue > maxCost) break;
				
				CollapseInfo cinfo = node.obj;
				VertexPair vp = cinfo.vp;
				// Add the error terms .. v[0] is the vertex that survives the collapse
				pdePerVertex[vp.v[0]].OpAdd(pdePerVertex[vp.v[1]]);
				
				// DO THE COLLAPSE
				int vindex = mesh.CollapseVertexPair(cinfo); // vindex is the surviving vertex index
											
				// Need to update the cost of all edges around all linked faces of vindex
				// these are the edges connected to vindex + all the edges connected to vindex neighbours
				_edgesToUpdate.Clear();
				_surroundingVerts.Clear();
			
				mesh.CollectVerticesAroundVertex(vindex, ref _surroundingVerts);
								
				// TODO: do this better -> why?
				int mark = mesh.GetUniqueTag();
				for (int i = 0; i < _surroundingVerts.Count; ++i) {
					List<int> lEdges = mesh.linkedEdgesForVert(_surroundingVerts[i]);
					for (int j = 0; j < lEdges.Count; ++j) {
						int edgeIndex = lEdges[j];
						Edge e = mesh.edges[edgeIndex];
						if (e.mark != mark) {
							e.mark = mark;
							if (mesh.IsEdgeValid(edgeIndex)) {
								_edgesToUpdate.Add(edgeIndex);
							}
						} 
					}
				}
							
				// DO the update
				for (int i = 0; i < _edgesToUpdate.Count; ++i) {
					int edgeIndex = _edgesToUpdate[i];
					HeapNode<CollapseInfo> hnode = heapNodes[edgeIndex];
					if (mesh.edges[edgeIndex].ContainsVertex(vindex)) {
						_calculateEdgeCost(edgeIndex, hnode.obj);
					} else {
						_updateEdgePenalties(edgeIndex, hnode.obj, vindex);
					}
					heap.Update(hnode, hnode.obj.cost);
				}
				
				numEdgesToCollapse--;
				collapsesDone++;
			}
			return collapsesDone;
		}
		
		PlaneDistanceError pde = new PlaneDistanceError();

		void _recalculateVertexPde(int vertexIndex) {
			IndexList faceIndexes = mesh.vertices[vertexIndex].linkedFaces;
			pdePerVertex[vertexIndex].Clear();
			for (int i = 0; i < faceIndexes.Count; ++i) {
				Face f = mesh.faces[faceIndexes[i]];
				if (f.valid) {
					_determinePdeForFace(faceIndexes[i], ref pde);
					pdePerVertex[vertexIndex].OpAdd(pde);
				}
			}
		}
		
		void _calculatePdePerVertex() {
			// This constructs a error term for every vertex based on the surrounding faces
			// The value of this term is a sum of the squared distances to all the planes
			if (mesh == null) {
				Debug.LogError("Mesh is not initialized. Could not calculate plane distance errors."); return;
			}

			int numVerts = mesh.vertCount();
			int numFaces = mesh.faceCount();
			// Initialize pdes
			pdePerVertex = new PlaneDistanceError[numVerts];
			for (int i = 0; i < numVerts; ++i) pdePerVertex[i] = new PlaneDistanceError();
			
			PlaneDistanceError pde = new PlaneDistanceError();
			for (int faceIndex = 0; faceIndex < numFaces; ++faceIndex) {
				_determinePdeForFace(faceIndex, ref pde);
				// Add to all point errors of the face
				KrablMesh.Face face = mesh.faces[faceIndex];
				for (int i = 0; i < face.cornerCount; ++i) {
					pdePerVertex[face.v[i]].OpAdd(pde);
				}
			}	

			// Mesh constrains
			int numEdges = mesh.edgeCount();
			// Look for border vertices and mesh discontinuities 
			for (int edgeIndex = 0; edgeIndex < numEdges; ++edgeIndex) {
				if (_determineEdgeContraintForEdge(edgeIndex, ref pde)) {
					Edge edge = mesh.edges[edgeIndex];
					pdePerVertex[edge.v[0]].OpAdd(pde);
					pdePerVertex[edge.v[1]].OpAdd(pde);
				}				
			}
		}
		
		void _determinePdeForFace(int faceIndex, ref PlaneDistanceError pde) {
			KrablMesh.Face face = mesh.faces[faceIndex];
			Vector3 n = mesh.CalculateFaceNormal(faceIndex);
				
			// Create error struct from the plane of the face (using face normal)
			float offset = -Vector3.Dot(n, mesh.vertices[face.v[0]].coords);
			pde.Set(n.x, n.y, n.z, offset, mesh.CalculateFaceArea(faceIndex));			
			// Multiply by face area (factor) for weighting
			pde.OpMul(pde.Factor());
		}
		
		bool _determineEdgeContraintForEdge(int edgeIndex, ref PlaneDistanceError pde) {
			float weight = 0.0f;
			if (mesh.IsEdgeBorder(edgeIndex)) {
				weight = _parameters.borderWeight;
			} else {
				if (mesh.edges[edgeIndex].crease == 1.0f) weight = Mathf.Max(weight, _parameters.creaseWeight);
				if (mesh.numMaterials > 1 && mesh.isEdgeMaterialSeam(edgeIndex)) weight = Mathf.Max(weight, _parameters.materialSeamWeight);
				if (mesh.hasUV1 && mesh.IsEdgeUV1Seam(edgeIndex)) weight = Mathf.Max(weight, _parameters.uvSeamWeight);
				if (mesh.hasUV2 && mesh.IsEdgeUV2Seam(edgeIndex)) weight = Mathf.Max(weight, _parameters.uv2SeamWeight);
			}
			if (weight > 0.0f) {
				_determinePdeToConstrainEdge(mesh.edges[edgeIndex], weight, ref pde);
				return true;
			}
			return false;
		}
		 		
		PlaneDistanceError pde2 = new PlaneDistanceError();

		void _determinePdeToConstrainEdge(Edge edge, float weight, ref PlaneDistanceError pde) {
			int vi0 = edge.v[0];
			int vi1 = edge.v[1];
			
			Vector3 v0 = mesh.vertices[vi0].coords;
			Vector3 vEdge = mesh.vertices[vi1].coords - v0;
			Vector3 edgeNormal;
			pde.Clear();
			float mag = vEdge.sqrMagnitude;
			for (int i = 0; i < edge.linkedFaces.Count; ++i) {
				edgeNormal = mesh.faces[edge.linkedFaces[i]].normal;
		
				Vector3 n = Vector3.Cross(vEdge, edgeNormal); // normal to edge and face
				UnityUtils.NormalizeSmallVector(ref n);
			
				float d = -Vector3.Dot(n, v0);
				pde2.Set(n.x, n.y, n.z, d, mag);
			// Multiply by face area (factor) for weighting
				pde2.OpMul(pde2.Factor()*weight*0.5f);
				pde.OpAdd(pde2);
			}
		}
		
		IndexList faces0 = new IndexList(32);
		IndexList faces1 = new IndexList(32);
		IndexList commonFaces = new IndexList(32);
				
		void _calculateEdgeCost(int edgeIndex, CollapseInfo cinfo)
		{
			Edge edge = mesh.edges[edgeIndex];
			int vindex0 = edge.v[0];
			int vindex1 = edge.v[1];
			Vertex v0 = mesh.vertices[vindex0];
			Vertex v1 = mesh.vertices[vindex1];
		
			cinfo.vp = edge;
			
			PlaneDistanceError pde = pdePerVertex[edge.v[0]] + pdePerVertex[edge.v[1]];
			
			if (_parameters.recalculateVertexPositions) {
				if (mesh.IsEdgeBorder(edgeIndex) == false && pde.OptimalVertex(ref cinfo.targetPosition)) {
					cinfo.cost = (float)pde.CalculateError(cinfo.targetPosition);	
				//	Debug.Log(">optimal placement");
				} else if (pde.OptimalVertexLinear(ref cinfo.targetPosition, v0.coords, v1.coords)) {
					// the error term is not solvable
					// Try to find a vert on the line going from v0 to v1
					cinfo.cost = (float)pde.CalculateError(cinfo.targetPosition);	
				//	Debug.Log(">line placement");
				} else {
					// Choose vert from the two endpoints and the midpoint
					Vector3 tp = 0.5f*(v0.coords + v1.coords);
					double error0 = pde.CalculateError(v0.coords);
					double error1 = pde.CalculateError(v1.coords);
					double error2 = pde.CalculateError(tp);
					if (error0 < error1) {
						if (error0 < error2) {
							cinfo.targetPosition = v0.coords; cinfo.cost = (float)error0;
						} else {
							cinfo.targetPosition = tp; cinfo.cost = (float)error2;
						}
					} else {
						if (error1 < error2) {
							cinfo.targetPosition = v1.coords; cinfo.cost = (float)error1;
						} else {
							cinfo.targetPosition = tp; cinfo.cost = (float)error2;
						}
					}
				}
			} else {
				double error0 = pde.CalculateError(v0.coords);
				double error1 = pde.CalculateError(v1.coords);
				if (error0 < error1) {
					cinfo.targetPosition = v0.coords; 
					cinfo.cost = (float)error0;
				} else {
					cinfo.targetPosition = v1.coords;
					cinfo.cost = (float)error1;
				}
			}
			
			// Choose minimal error point -> bad for border edges which are underdefined
			if (localizeErrors) cinfo.cost *= 1.0f/((float)pde.Factor());
						
			cinfo.positionCost = cinfo.cost;
			
			if (noPenalties == false) {
				_updateEdgePenalties(edgeIndex, cinfo, -1);
			}
		}
		
		void _updateEdgePenalties(int edgeIndex, CollapseInfo cinfo, int movedVertexIndex = -1) {	
			Edge edge = mesh.edges[edgeIndex];
			int vindex0 = edge.v[0];
			int vindex1 = edge.v[1];
			Vertex v0 = mesh.vertices[vindex0];
			Vertex v1 = mesh.vertices[vindex1];
			
			faces0.Clear();
			faces1.Clear();
			commonFaces.Clear();
			
			bool hadPenalty = (cinfo.cost >= kMeshPenaltyMaxEdgesPerVertex);
			
			cinfo.cost = cinfo.positionCost; // reset cost
			// determine the faces involved in the collapse .. 
			mesh.CollectCollapseFacesForVertexPair(mesh.edges[edgeIndex], faces0, faces1, commonFaces);
				
			// Penalties
							
			int filterTag = 0;
			
			if (_parameters.preventNonManifoldEdges && _producesNonManifold(mesh, cinfo)) {
				cinfo.cost += kMeshPenaltyNonManifold; // largest penalty first
			} else {
				if (movedVertexIndex != -1 && hadPenalty == false) {
					// For cinfos that are not new and had no penalty before, all faces besides the onces connected
					// to the moved vertex can be skipped.
					filterTag = mesh.GetUniqueTag();
					IndexList linkedFaces = mesh.vertices[movedVertexIndex].linkedFaces;
					int count = linkedFaces.Count;
					for (int i = 0; i < count; ++i) mesh.faces[linkedFaces[i]].mark = filterTag;
				}
				if (_parameters.checkTopology && _producesBadTopology(mesh, cinfo, filterTag)) {
					// Apply penalties for bad collapses
					cinfo.cost += kMeshPenaltyBadTopology; 
				} else if (_parameters.maxEdgesPerVertex > 0 && _vertexDegreeAfterCollapse(mesh, cinfo) > _parameters.maxEdgesPerVertex) { // Hard coded at 18 for now.. rarely reached, but always check!
					// Avoid collapses leading to excessive stars (many verts connected to one)
					cinfo.cost += kMeshPenaltyMaxEdgesPerVertex; 
				}
			}
			
			// Additional penalties:			
			float val = 0.0f;
			if (_parameters.boneWeightProtection > 0.0f) {
				val += UnityUtils.BoneWeightDeltaSqr(v0.boneWeight, v1.boneWeight)*_parameters.boneWeightProtection;
			}
			if (_parameters.vertexColorProtection > 0.0f) {
				val += UnityUtils.ColorDeltaSqr(v0.color, v1.color)*_parameters.vertexColorProtection;
			}
			if (val != 0.0f) {
				cinfo.cost += 0.1f*val*mesh.EdgeLengthSqr(edgeIndex); 
			}
		}
		
		// Count number of surrounding faces after the collapse
		int _vertexDegreeAfterCollapse(BaseMesh mesh, CollapseInfo ci) {
			return faces0.Count + faces1.Count;
		}
						
		bool _producesNonManifold(BaseMesh mesh, CollapseInfo ci) {
			// ... If v0 and v1 don't have nr of commonfaces common neighbours
			List<Vertex> vertices = mesh.vertices;
			List<Face> faces = mesh.faces;
			int common = commonFaces.Count;
			int tag = mesh.GetUniqueTag();
			
			int count = faces0.Count;
			int[] faceArray = faces0.array;
			for (int i = 0; i < count; ++i) {
				int[] v = faces[faceArray[i]].v;
				 // known to be a valid triangle
				vertices[v[0]].mark = tag;
				vertices[v[1]].mark = tag;
				vertices[v[2]].mark = tag;
			}
			count = faces1.Count;
			faceArray = faces1.array;
			for (int i = 0; i < count; ++i) {
				int[] v = faces[faceArray[i]].v;
				if (vertices[v[0]].mark == tag) { common--; if (common < 0) return true; vertices[v[0]].mark = 0; }
				if (vertices[v[1]].mark == tag) { common--; if (common < 0) return true; vertices[v[1]].mark = 0; }
				if (vertices[v[2]].mark == tag) { common--; if (common < 0) return true; vertices[v[2]].mark = 0; }				
			}
			
			return common != 0;
		}
		
		//Vector3[] tr = new Vector3[3];
		
		/*
		// rchecks result triangles against limit
		bool _producesSharpTriangles(Mesh mesh, CollapseInfo ci, float limitSqr) {
			for (int k = 0; k < 2; ++k) {
				int vindex = ci.vp.v[k];
				List<int> linkedFaces = (k == 0) ? faces0 : faces1;
				for (int i = 0; i < linkedFaces.Count; ++i) {
					Face f = mesh.faces[linkedFaces[i]];					
					if (f.v[0] == vindex) 		{ tr[0] = ci.targetPosition; 			tr[1] = mesh.vertex(f.v[1]).coords; tr[2] = mesh.vertex(f.v[2]).coords; }
					else if (f.v[1] == vindex) 	{ tr[0] = mesh.vertex(f.v[0]).coords; 	tr[1] = ci.targetPosition; 			tr[2] = mesh.vertex(f.v[2]).coords; }
					else 						{ tr[0] = mesh.vertex(f.v[0]).coords; 	tr[1] = mesh.vertex(f.v[1]).coords;	tr[2] = ci.targetPosition; 			}
					float val = UnityUtils.TriangleCompactnessSqr(tr);
					if (val < limitSqr) return true;
				}
			}
			return false;
		}
		*/
		
		// Check topology to avoid flipping the mesh by collapsing
		bool _producesBadTopology(BaseMesh mesh, CollapseInfo ci, int filterTag = 0) {
			Vector3 p1, origPos, vOpp, faceNormal, normal;
			List<Vertex> vertices = mesh.vertices;
			List<Face> faces = mesh.faces;
			
			for (int k = 0; k < 2; ++k) {
				int vindex = ci.vp.v[k];
				origPos = vertices[vindex].coords;
				IndexList linkedFacesList = (k == 0) ? faces0 : faces1;
				int[] linkedFaces = linkedFacesList.array;
				for (int i = 0; i < linkedFacesList.Count; ++i) {
					Face f = faces[linkedFaces[i]];
					if (f.mark >= filterTag) {
						// Construct a plane from opposing sides (p1->p2) && the face normal -> todo make this better (faster)
						if (f.v[0] == vindex) {
							p1 = vertices[f.v[1]].coords;
							vOpp = vertices[f.v[2]].coords - p1;
						} else if (f.v[1] == vindex) {
							p1 = vertices[f.v[2]].coords;
							vOpp = vertices[f.v[0]].coords - p1;
						} else {
							p1 = vertices[f.v[0]].coords;
							vOpp = vertices[f.v[1]].coords - p1;
						}
						
						faceNormal = Vector3.Cross(vOpp, origPos - p1); // the face normal
						normal = Vector3.Cross(faceNormal, vOpp); // normal of constructed plane
						if (Vector3.Dot(ci.targetPosition - p1, normal) < 0.0f) return true; 
					}
				}
			}
			return false;
		}
		
		/*
		 * NOT USED. the badtopology test is faster & better
		 
		  bool _producesInvertedFaces(BaseMesh mesh, CollapseInfo ci) {
			Vector3[] tr = new Vector3[3];
			for (int k = 0; k < 2; ++k) {
				int vindex = ci.vp.v[k];
				List<int> linkedFaces = (k == 0) ? faces0 : faces1;
				for (int i = 0; i < linkedFaces.Count; ++i) {
					Face f = mesh.faces[linkedFaces[i]];
					Vector3 oldN = mesh.CalculateFaceNormal(linkedFaces[i]);
					if (f.v[0] == vindex) 		{ tr[0] = ci.targetPosition; 			tr[1] = mesh.vertex(f.v[1]).coords; tr[2] = mesh.vertex(f.v[2]).coords; }
					else if (f.v[1] == vindex) 	{ tr[0] = mesh.vertex(f.v[0]).coords; 	tr[1] = ci.targetPosition; 			tr[2] = mesh.vertex(f.v[2]).coords; }
					else 						{ tr[0] = mesh.vertex(f.v[0]).coords; 	tr[1] = mesh.vertex(f.v[1]).coords;	tr[2] = ci.targetPosition; 			}
					Vector3 newN = Vector3.Cross(tr[1] - tr[0], tr[2] - tr[1]);
					if (Vector3.Dot(oldN, newN) < 0.0f) {
						return true;
					}
				}
			}
			return false;
		}*/
		
		float _determineMeshSize(BaseMesh mesh) {
			Vector3 min = mesh.vertices[0].coords;
			Vector3 max = min;
			int numVerts = mesh.vertCount();
			for (int i = 0; i < numVerts; ++i) {
				Vector3 c = mesh.vertices[i].coords;
				if 		(c.x < min.x) min.x = c.x;
				else if (c.x > max.x) max.x = c.x;
				if 		(c.y < min.y) min.y = c.y;
				else if (c.y > max.y) max.y = c.y;
				if 		(c.z < min.z) min.z = c.z;
				else if (c.z > max.z) max.z = c.z;
			}
			max -= min;
			float result = max.x;
			if (max.y > meshSize) meshSize = max.y;
			if (max.z > meshSize) meshSize = max.z;
			return result;
		}
	}
}
