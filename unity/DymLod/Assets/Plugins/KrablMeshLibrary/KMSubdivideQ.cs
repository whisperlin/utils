using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KrablMesh;

namespace KrablMesh {
	[System.Serializable]
	/// <summary>
	/// The parameters used for quad-based mesh subdivision.
	/// </summary>
	public class SubdivideQParameters {
		/// <summary>
		/// Perform a triangles to quads operation before starting the subdivision iterations.
		/// Quads lead to much better subdivision results. 
		/// </summary>
		public bool trisToQuads = true;
		/// <summary>
		/// The max edge angle allowed for an edge which is dissolved during the triangles to quads pre-processing step.
		/// </summary>
		public float trisToQuadsMaxAngle = 40.0f;
		/// <summary>
		/// The number of subdivision iterations to perform. Every iteration produces four quads for every face of the input, so
		/// the face count quadruples. Higher iteration numbers quickly lead to excessive face and vertex numbers Unity3D currently cannot 
		/// handle. There's a limit of 65k vertices and triangles.
		/// </summary>
		public int iterations = 1;
		//public bool subdivideUV = false;
		/// <summary>
		/// If smooth is false, vertices are not interpolated, but only linearly split and the mesh shape does not change.
		/// </summary>
		public bool smooth = true;
		/// <summary>
		/// Recalculate normals after all subdivision steps. The alternative is linear interpolation of the original normals. Recalculating
		/// usually leads to better results that conform better to the new mesh shape.
		/// </summary>
		public bool recalculateNormals = true;
	}
	
	/// <summary>
	/// An mesh subdivision algorithm based on quads.
	/// </summary>
	public class SubdivideQ {
		public Vector3[] vertPoints = null;
		public Vector3[] facePoints = null;
		public Vector3[] edgePoints = null;
	
		public bool smooth = true;
		public bool recalculateNormals = true;
		
		public ProgressDelegate progressDelegate = null;

		/// <summary>
		/// Perform quad-based mesh subdivision
		/// </summary>
		/// <param name='mesh'>
		/// A mesh (KrablMesh.MeshEdges) to subdivide. The mesh will be replaced by a new mesh structure.
		/// </param>
		/// <param name='parameters'>
		/// The parameters to use during the subdivison.
		/// </param>
		public void Execute(ref MeshEdges mesh, SubdivideQParameters parameters) {
			float progSteps = (float)parameters.iterations;
			if (parameters.trisToQuads) progSteps += 1.0f;
			if (parameters.recalculateNormals) progSteps += 1.0f;
			float progStep = (progSteps > 0.0f) ? (1.0f/progSteps) : 1.0f;
			float progress = 0.0f;
			
			smooth = parameters.smooth;
			recalculateNormals = parameters.recalculateNormals;
			if (parameters.trisToQuads) {
				if (progressDelegate != null) {
					progressDelegate("TrisToQuads", progress);
					progress += progStep;
				}
				Ops.TrisToQuads(mesh, parameters.trisToQuadsMaxAngle);
			}
			for (int i = 0; i < parameters.iterations; ++i) {
				if (progressDelegate != null) {
					progressDelegate("Iteration " + (i + 1), progress);
					progress += progStep;
				}
				Subdivide(ref mesh);
			}
			if (recalculateNormals) {
				if (progressDelegate != null) {
					progressDelegate("Recalculate Normals", progress);
					progress += progStep;
				}
				mesh.CalculateFaceVertexNormalsFromEdgeCreases();
			}
		}

			
		public void Subdivide(ref MeshEdges mesh) {
			int i, j;
			int numVerts = mesh.vertCount();
			int numFaces = mesh.faceCount();
			int numEdges = mesh.edgeCount();
			mesh.CalculateEdgeLinkedFaces();
			
			_calculateVertexPositions(mesh);
			
			// TODO:don't generate a new mesh... just add the verts to the old mesh = faster and uses less memory
			MeshEdges newMesh = new MeshEdges();
			int faceOffset = numVerts;
			int edgeOffset = numVerts + numFaces;
			float[] edgeRatio = new float[numEdges];
			
			// Add vertices to new mesh, precalculate stuff
			for (i = 0; i < numVerts; ++i) {
				newMesh.AddVertex(vertPoints[i]);
			}
			for (i = 0; i < numFaces; ++i) {
				newMesh.AddVertex(facePoints[i]);
			}
			for (i = 0; i < numEdges; ++i) {
				int vertexIndex = newMesh.AddVertex(edgePoints[i]);
				Vertex nv = newMesh.vertices[vertexIndex];
				Vertex ov0 = mesh.vertices[mesh.edges[i].v[0]];
				Vertex ov1 = mesh.vertices[mesh.edges[i].v[1]];
				edgeRatio[i] = KrablMesh.UnityUtils.ProjectedRatioOfPointOnVector(nv.coords, ov0.coords, ov1.coords);
			}
						
			if (mesh.hasBoneWeights) {
				for (i = 0; i < numVerts; ++i) {
					newMesh.vertices[i].boneWeight = mesh.vertices[i].boneWeight;
				}
				for (i = 0; i < numEdges; ++i) {
					int[] vi = mesh.edges[i].v;
					newMesh.vertices[edgeOffset + i].boneWeight = KrablMesh.UnityUtils.BoneWeightLerp(
						mesh.vertices[vi[0]].boneWeight, 
						mesh.vertices[vi[1]].boneWeight, 
						edgeRatio[i]);
				}
				for (i = 0; i < numFaces; ++i) {
					newMesh.vertices[faceOffset + i].boneWeight = mesh.CalculateFaceCenterBoneWeight(i);
				}
			}			
			
			if (mesh.hasVertexColors) {
				for (i = 0; i < numVerts; ++i) {
					newMesh.vertices[i].color = mesh.vertices[i].color;
				}
				for (i = 0; i < numEdges; ++i) {
					int[] vi = mesh.edges[i].v;
					newMesh.vertices[edgeOffset + i].color = Color.Lerp(
						mesh.vertices[vi[0]].color, 
						mesh.vertices[vi[1]].color, 
						edgeRatio[i]);
				}
				for (i = 0; i < numFaces; ++i) {
					newMesh.vertices[faceOffset + i].color = mesh.CalculateFaceCenterColor(i);
				}
			}
			
			// Create the faces
			Face nf, of;
			int g, h;
			int a, b, c, d;
			for (int faceIndex = 0; faceIndex < numFaces; ++faceIndex) {
				of = mesh.faces[faceIndex];
				if (of.valid) {
					h = of.cornerCount - 1; // scan through with indices g, h, i
					g = of.cornerCount - 2;
					Vector2 centerUV1 = mesh.CalculateFaceCenterUV1(faceIndex);
					Vector2 centerUV2 = mesh.CalculateFaceCenterUV2(faceIndex);
					Vector3 centerVertexNormal = mesh.CalculateFaceCenterVertexNormal(faceIndex);
					for (i = 0; i < of.cornerCount; ++i) {
						int edgeIndex0 = mesh.EdgeIndexForVertices(of.v[h], of.v[i]);
						if (edgeIndex0 < 0) continue;
						float ratio0 = edgeRatio[edgeIndex0];
						if (mesh.edges[edgeIndex0].v[0] == of.v[h]) {
							a = h; b = i; // interpolation indexes. 1- ratio gives inaccurate results (floating point problems)
						} else {
							a = i; b = h;
						}						
						
						int edgeIndex1 = mesh.EdgeIndexForVertices(of.v[g], of.v[h]);
						if (edgeIndex1 < 0) continue;
						float ratio1 = edgeRatio[edgeIndex1];
						if (mesh.edges[edgeIndex1].v[0] == of.v[g]) {
							c = g; d = h;
						} else {
							c = h; d = g;
						}
						
						nf = new Face(
							of.v[h], // corner point
							edgeOffset + edgeIndex0, 
							faceOffset + faceIndex, // center point
							edgeOffset + edgeIndex1 
						);
						nf.uv1[0] = of.uv1[h];
						nf.uv1[1] = Vector2.Lerp(of.uv1[a], of.uv1[b], ratio0); 
						nf.uv1[2] = centerUV1;
						nf.uv1[3] = Vector2.Lerp(of.uv1[c], of.uv1[d], ratio1);
						
						if (mesh.hasUV2) {
							nf.uv2[0] = of.uv2[h];
							nf.uv2[1] = Vector2.Lerp(of.uv2[a], of.uv2[b], ratio0);
							nf.uv2[2] = centerUV2;
							nf.uv2[3] = Vector2.Lerp(of.uv2[c], of.uv2[d], ratio1);
						}	
						
						if (!recalculateNormals) {
							nf.vertexNormal[0] = of.vertexNormal[h];
							nf.vertexNormal[1] = Vector3.Lerp(of.vertexNormal[a], of.vertexNormal[b], ratio0);
							nf.vertexNormal[2] = centerVertexNormal;
							nf.vertexNormal[3] = Vector3.Lerp(of.vertexNormal[c], of.vertexNormal[d], ratio1);
						}

						nf.material = of.material;
						
						newMesh.AddFace(nf);
						g = h;
						h = i;
					}	
				}
			}
			
			newMesh.hasUV1 = mesh.hasUV1;
			newMesh.hasUV2 = mesh.hasUV2;
			newMesh.hasBoneWeights = mesh.hasBoneWeights;
			newMesh.bindposes = mesh.bindposes;
			newMesh.numMaterials = mesh.numMaterials;
			newMesh.hasVertexColors = mesh.hasVertexColors;
			
			newMesh.topology = MeshTopology.Quads;
			newMesh.GenerateEdgeList();
			newMesh.GenerateEdgeTopology();
			// Need to copy edge information from the original mesh to the new mesh				
			// Every edge of the original mesh now has two parts
			
			int edgePointIndex;
			for (i = 0; i < numEdges; ++i) {
				Edge e = mesh.edges[i];
				edgePointIndex = edgeOffset + i; // as newmesh was just constructed, we know the correct vertex index of the edge center point!
				for (j = 0; j < 2; ++j) {
					int eindex = newMesh.EdgeIndexForVertices(e.v[j], edgePointIndex);
					
						// Copy attributes (just crease for now)
					newMesh.edges[eindex].crease = e.crease;
				}
			}
			mesh = newMesh;
		}
		
		// Simple implementation based on linkedEdges. It would be possible without edges, but this algo needs edges anyways
		bool _isVertexBorder(MeshEdges mesh, int vertIndex) {
			List<int> linkedEdges = mesh.linkedEdgesForVert(vertIndex);
			for (int i = 0; i < linkedEdges.Count; ++i) {
				int edgeIndex = linkedEdges[i];
				if (mesh.IsEdgeValid(edgeIndex) && mesh.IsEdgeBorder(edgeIndex)) return true;
			}
			return false;
		}
		
		void _calculateVertexPositions(MeshEdges mesh) {
			int numVerts = mesh.vertCount();
			int numFaces = mesh.faceCount();
			int numEdges = mesh.edgeCount();
			
			vertPoints = new Vector3[numVerts];
			facePoints = new Vector3[numFaces];
			edgePoints = new Vector3[numEdges];
			
			// In any case the new face points are the center of the faces
			for (int i = 0; i < numFaces; ++i) facePoints[i] = mesh.CalculateFaceCenter(i);
	
			if (smooth) {
				// First count the number of creases connected to each vertex
				int[] vertexNumCreases = new int[numVerts];
				for (int i = 0; i < numVerts; ++i) {
					List<int> linkedEdges = mesh.linkedEdgesForVert(i);
					for (int j = 0; j < linkedEdges.Count; ++j) {
						int edgeIndex = linkedEdges[j];
						if (mesh.IsEdgeValid(edgeIndex) && mesh.edges[edgeIndex].crease > 0.0) vertexNumCreases[i]++;
					}
				}
				
				// Edge is the average of the two ends and the connected face centers
				for (int i = 0; i < numEdges; ++i) {
					Edge e = mesh.edges[i];
					IndexList linkedFaces = e.linkedFaces;
					edgePoints[i] = mesh.CalculateVertexPairCenter(e);
					
					// If an edge connects two verts that do not move, use the center to prevent overlaps
					//bool betweenNonMovableVertices = (vertexNumCreases[e.v[0]] > 2 && vertexNumCreases[e.v[1]] > 2);
					//if (betweenNonMovableVertices) Debug.Log("Between NONmovable verts");
					if (/*betweenNonMovableVertices == false &&*/ e.crease == 0.0f && linkedFaces.Count >= 2) { 
						// creases and borders stay at edge centers
						// other edges use the edge center + the center of all attached face centers
						Vector3 faceCenterCenter = facePoints[linkedFaces[0]];
						int numLinked = e.linkedFaces.Count;
						for (int j = 1; j < numLinked; ++j) faceCenterCenter += facePoints[linkedFaces[j]];
						edgePoints[i] = 0.5f*(edgePoints[i] + faceCenterCenter*(1.0f/((float)numLinked)));
					}
				}	
				// Vert
				for (int i = 0; i < numVerts; ++i) {
					Vector3 oldPosition = mesh.vertices[i].coords;
					List<int> linkedEdges = mesh.linkedEdgesForVert(i);
					if (_isVertexBorder(mesh, i) == false) {
						// First deal with edges. the number of creases needs to be known!
						
						
						Vector3 edgesCenter = Vector3.zero;
						float numCreases = 0.0f;
						Vector3 creaseCenter = Vector3.zero;
						float nEdges = 0.0f;
						for (int j = 0; j < linkedEdges.Count; ++j) {
							int edgeIndex = linkedEdges[j];
							if (mesh.IsEdgeValid(edgeIndex)) {
								Edge e = mesh.edges[edgeIndex];
								Vector3 center = mesh.CalculateVertexPairCenter(e);
								if (e.crease > 0.0f) {
									numCreases += 1.0f;
									creaseCenter += center;
								}
								nEdges += 1.0f;
								edgesCenter += center; 
							}
						}
						edgesCenter *= 1.0f/nEdges;
												
						// For points without crease edges or just one -> do nothing
						if (numCreases == 2.0f) {
							// like a border connection
							vertPoints[i] = 0.5f*oldPosition + creaseCenter*(0.5f/numCreases);
						} else if (numCreases > 2.0f) {
							// A sharp corner
							vertPoints[i] = oldPosition;
						} else {
							// Full formula including faces center which needs to be calculated
							IndexList linkedFaces = mesh.vertices[i].linkedFaces;
							Vector3 facesCenter = Vector3.zero;
							float n = 0.0f;
							for (int j = 0; j < linkedFaces.Count; ++j) { 
								int faceIndex = linkedFaces[j];
								if (mesh.faces[faceIndex].valid) {
									facesCenter += facePoints[faceIndex];
									n += 1.0f;
								}
							}
							float invN = 1.0f/n;
							vertPoints[i] = (facesCenter*invN + 2.0f*edgesCenter + (n - 3.0f)*oldPosition)*invN; // useless for 2 creases!!!
						}
					} else {
						// Border Vertex. get center of all connected border centers.
						if (vertexNumCreases[i] > 2) {
							vertPoints[i] = oldPosition;
						} else {
							float nBorders = 0.0f;
							Vector3 borderCenter = Vector3.zero;
							for (int j = 0; j < linkedEdges.Count; ++j) {
								int edgeIndex = linkedEdges[j];
								if (mesh.IsEdgeValid(edgeIndex)) {
									Edge e = mesh.edges[edgeIndex];
									if (mesh.IsEdgeBorder(edgeIndex)) {
										borderCenter += mesh.CalculateVertexPairCenter(e); 
										nBorders += 1.0f;
									}
								}
							}
							vertPoints[i] = 0.5f*oldPosition + borderCenter*(0.5f/nBorders);
						}
					}
				}
			} else {
				// Subdivision without smoothing
				for (int i = 0; i < numVerts; ++i) vertPoints[i] = mesh.vertices[i].coords;
				for (int i = 0; i < numEdges; ++i) edgePoints[i] = mesh.CalculateVertexPairCenter(mesh.edges[i]);
			}
		}
	}
}
