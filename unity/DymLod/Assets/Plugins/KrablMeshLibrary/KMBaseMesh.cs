using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace KrablMesh {
	public enum MeshTopology {
		Mixed, // Tris & Quads
		Triangles,
		Quads
	}
	
	public class BaseMesh {
		public List<Vertex> vertices = new List<Vertex>();
		public List<Face> faces = new List<Face>();
		public int numValidVerts = 0;
		public int numValidFaces = 0;
		public int numMaterials = 1;
		public bool hasVertexColors = false;
		public bool hasBoneWeights = false;
	 	public MeshTopology topology = MeshTopology.Mixed;
		public bool hasUV1 = false;
		public bool hasUV2 = false;
		public float equalityTolerance = 0.0f;

		public Matrix4x4[] bindposes = null;
		public bool calculateTangents = true;
		protected int uniqueTag = 1 << 24;		
		
		public int GetUniqueTag() {
			return uniqueTag++;
		}
		
		// VERTICES //		
		
		public int vertCount() { return vertices.Count; }
													
		public Vertex corner(int faceIndex, int cornerIndex)
		{
			return vertices[faces[faceIndex].v[cornerIndex]];
		}
		
		public int AddVertex(Vector3 coords) {
			int vid = vertices.Count;
			vertices.Add(new Vertex(coords));
			numValidVerts++;
			return vid;
		}
				
		public bool IsVertexValid(int vertexIndex) {
			return vertices[vertexIndex].valid;
		}
					
		public int CompareVertices(int vertexIndexA, int vertexIndexB, float positionTolerance) {
			Vertex va = vertices[vertexIndexA];
			Vertex vb = vertices[vertexIndexB];
			
			int res = KrablMesh.UnityUtils.Vector3CompareWithTolerance(va.coords, vb.coords, positionTolerance); 
			if (res != 0) return res;
			if (hasBoneWeights) {
				res = KrablMesh.UnityUtils.BoneWeightCompare(va.boneWeight, vb.boneWeight);
				if (res != 0) return res;
			}
			return 0;
		}
		
		public void ReplaceVertex(int vertexIndexOld, int vertexIndexNew) {
			IndexList linkedFaces = vertices[vertexIndexOld].linkedFaces;
			int num = linkedFaces.Count;
			for (int i = 0; i < num; ++i) {
				int faceIndex = linkedFaces[i];
				int n2 = faces[faceIndex].ReplaceVertex(vertexIndexOld, vertexIndexNew);
				if (n2 != 1) {
					Debug.LogError("Weird error vertex found " + n2 + " times in face.");
				}
				vertices[vertexIndexNew].linkedFaces.Add(faceIndex);
			}
			vertices[vertexIndexOld].linkedFaces.Clear();
		}
		
		public Vector3 CalculateVertexNormal(int vertexIndex) {
			Vertex vert = vertices[vertexIndex];
			Vector3 n = Vector3.zero;
			IndexList linkedFaces = vert.linkedFaces;
			int num = linkedFaces.Count;
			for (int i = 0; i < num; ++i) {
				int faceIndex = linkedFaces[i];
				n += faces[faceIndex].normal;
			}
			UnityUtils.NormalizeSmallVector(ref n);
			vert.normal = n;
			return n;
		}
		
		public void CalculateVertexNormals() {
			int numVerts = vertCount();
			for (int i = 0; i < numVerts; ++i) {
				CalculateVertexNormal(i);
			}
		}
		
		// VERTEXPAIRS //
		
		public bool isVertexPairValid(VertexPair vp)
		{
			if (vp.v[0] == vp.v[1]) return false;
			if (IsVertexValid(vp.v[0]) == false) return false;
			if (IsVertexValid(vp.v[1]) == false) return false;
			return true;
		}
		
		public Vector3 CalculateVertexPairCenter(VertexPair vp) 
		{
			return 0.5f*(vertices[vp.v[0]].coords + vertices[vp.v[1]].coords);
		}

		// FACES //

		public int faceCount() { return faces.Count; }
							
		public int AddFace(Face f) {
			int faceIndex = faces.Count;
			faces.Add(f);
			
			topology = MeshTopology.Mixed;
	
			for (int i = 0; i < f.cornerCount; ++i) {
				vertices[f.v[i]].linkedFaces.Add(faceIndex);
			}
			numValidFaces++;
			return faceIndex;
		}
					
		public void UnlinkFace(int faceIndex) {
			Face f = faces[faceIndex];
			if (!f.valid) {
//				Debug.LogError("Unlinking invalid face!");
			} else {
				numValidFaces--;
				f.valid = false;
			
				for (int i = 0; i < f.cornerCount; ++i) {
					vertices[f.v[i]].linkedFaces.Remove(faceIndex);
				}
			}
		}
											
		public Vector3 CalculateFaceNormal(int faceIndex) {
			int i;
			Face face = faces[faceIndex];
			int vertCount = face.cornerCount;
			int[] vindex = face.v;
			Vector3[] vec = new Vector3[vertCount + 1];
			
			for (i = 0; i < vertCount - 1; ++i) vec[i] = vertices[vindex[i + 1]].coords - vertices[vindex[i]].coords;
			vec[i] = vertices[vindex[0]].coords - vertices[vindex[i]].coords;
			Vector3 normal = Vector3.zero;
			if (vertCount == 3) {   
				normal = Vector3.Cross(vec[0], vec[1]); 
			} else {
				// Sum normals based on consequtive pairs of edges
				vec[vertCount] = vec[0];
				for (i = 0; i < vertCount; ++i) {
					normal += Vector3.Cross(vec[i], vec[i + 1]);
				}
			}
			UnityUtils.NormalizeSmallVector(ref normal);								
			face.normal = normal;
			return face.normal;
		}
		
		public void CalculateFaceNormals() {
			int numFaces = faces.Count;
			for (int i = 0; i < numFaces; ++i) {
				CalculateFaceNormal(i);
			}
		}
		
		public Vector3 CalculateFaceCenter(int faceIndex) {
			Face f = faces[faceIndex];
			Vector3 result = Vector3.zero;
			for (int cornerIndex = 0; cornerIndex < f.cornerCount; ++cornerIndex) {
				result += vertices[f.v[cornerIndex]].coords;
			}
			return result*(1.0f/((float)f.cornerCount));
		}
		
		public Vector2 CalculateFaceCenterUV1(int faceIndex) {
			Face f = faces[faceIndex];
			Vector2 result = Vector2.zero;
			for (int cornerIndex = 0; cornerIndex < f.cornerCount; ++cornerIndex) {
				result += f.uv1[cornerIndex];
			}
			return result*(1.0f/((float)f.cornerCount));
		}

		public Vector2 CalculateFaceCenterUV2(int faceIndex) {
			Face f = faces[faceIndex];
			Vector2 result = Vector2.zero;
			for (int cornerIndex = 0; cornerIndex < f.cornerCount; ++cornerIndex) {
				result += f.uv2[cornerIndex];
			}
			return result*(1.0f/((float)f.cornerCount));
		}
		
		public Vector3 CalculateFaceCenterVertexNormal(int faceIndex) {
			Face f = faces[faceIndex];
			Vector3 result = Vector3.zero;
			for (int cornerIndex = 0; cornerIndex < f.cornerCount; ++cornerIndex) {
				result += f.vertexNormal[cornerIndex];
			}
			return result*(1.0f/((float)f.cornerCount));
		}

		public BoneWeight CalculateFaceCenterBoneWeight(int faceIndex) {
			Face f = faces[faceIndex];
			Dictionary<int, float> resultWeightForBone = new Dictionary<int, float>(4*f.cornerCount);
	
			for (int cornerIndex = 0; cornerIndex < f.cornerCount; ++cornerIndex) {
				Vertex v = vertices[f.v[cornerIndex]];
				BoneWeight bw = v.boneWeight;
	
				int[] indexes = {bw.boneIndex0, bw.boneIndex1, bw.boneIndex2, bw.boneIndex3};
				float[] weights = {bw.weight0, bw.weight1, bw.weight2, bw.weight3};
			
				for (int i = 0; i < 4; ++i) {
					if (resultWeightForBone.ContainsKey(indexes[i])) {
						resultWeightForBone[indexes[i]] += weights[i];
					} else {
						resultWeightForBone.Add(indexes[i], weights[i]);
					}
				}
			} 
			
			// Sort by weight
			List<KeyValuePair<int, float>> mList = new List<KeyValuePair<int, float>>(resultWeightForBone);

			mList.Sort((x,y) => y.Value.CompareTo(x.Value));
			// Make sure there are at least 4 entries
			while (mList.Count < 4) mList.Add(new KeyValuePair<int, float>(0, 0.0f));
			
			float weightSum = mList[0].Value + mList[1].Value + mList[2].Value + mList[3].Value;			
			float weightFact = 1.0f;
			if (weightSum != 0.0f) weightFact = 1.0f/weightSum;
			
			BoneWeight result = new BoneWeight();
			result.boneIndex0 = mList[0].Key;
			result.weight0 = mList[0].Value*weightFact;
			result.boneIndex1 = mList[1].Key;
			result.weight1 = mList[1].Value*weightFact;
			result.boneIndex2 = mList[2].Key;
			result.weight2 = mList[2].Value*weightFact;
			result.boneIndex3 = mList[3].Key;
			result.weight3 = mList[3].Value*weightFact;
			
			return result;
		}
		
		public Color CalculateFaceCenterColor(int faceIndex) {
			Face f = faces[faceIndex];
			Color result = Color.clear;
			for (int cornerIndex = 0; cornerIndex < f.cornerCount; ++cornerIndex) {
				result += vertices[f.v[cornerIndex]].color;
			}
			return result*(1.0f/((float)f.cornerCount));
		}

		public float CalculateFaceArea(int faceIndex) {
			Face f = faces[faceIndex];
			if (f.cornerCount != 3) {
				Debug.LogError("Currently only the area of triangles can be calculated.");
			}
			
			Vector3 a = vertices[f.v[1]].coords - vertices[f.v[0]].coords;
			Vector3 b = vertices[f.v[2]].coords - vertices[f.v[0]].coords;
			float area = Vector3.Cross(a, b).magnitude;
			area = Mathf.Abs(0.5f*area);
			return area;
		}
		
		public float CornerAngle(int faceIndex, int cornerIndex) {
			Face f = faces[faceIndex];
			int[] fv = f.v;
			int vertCount = f.cornerCount;
			int prev = cornerIndex - 1; 
			int next = cornerIndex + 1;
			if (prev < 0) prev = vertCount - 1;
			else if (next >= vertCount) next = 0;
			
			Vector3 p = vertices[fv[cornerIndex]].coords;
			Vector3 a = vertices[fv[prev]].coords - p;
			Vector3 b = vertices[fv[next]].coords - p;
			return Vector3.Angle(a, b);
		}
								
		// ALGORITHMS
		
		public virtual void Clear() {
			vertices.Clear();
			faces.Clear();
			numValidVerts = 0;
			numValidFaces = 0;
			numMaterials = 1;
			hasVertexColors = false;
			hasBoneWeights = false;
		 	topology = MeshTopology.Mixed;
			hasUV1 = false;
			hasUV2 = false;
			equalityTolerance = 0.0f;
	
			bindposes = null;
			calculateTangents = true;
		}
		
		public MeshTopology DetermineMeshTopology() {
			if (faces.Count == 0) {
				topology = MeshTopology.Mixed;
			} else {
				Face f0 = faces[0];
				int f0CornerCount = f0.cornerCount;
				int faceCount = faces.Count;
				int faceIndex = 1;
				for (; faceIndex < faceCount; ++faceIndex) {
					if (faces[faceIndex].cornerCount != f0CornerCount) break;
				}
				if (faceIndex == faceCount) {
					if (f0CornerCount == 4) topology = MeshTopology.Quads;
					else topology = MeshTopology.Triangles;
				} else {
					topology = MeshTopology.Mixed;
				}
			}
			return topology;
		}
		
		// collects the vertices that are connected to vertexIndex
		// TODO: optimize .. store Face array and get neighbours via switch for quads
		public void CollectVerticesAroundVertex(int vertexIndex, ref List<int> list) {
			int mark = GetUniqueTag();
			IndexList surroundingFaces = vertices[vertexIndex].linkedFaces; // surrounding faces
			int numFaces = surroundingFaces.Count;

			if (topology == MeshTopology.Triangles) {
				vertices[vertexIndex].mark = mark;
				for (int i = 0; i < numFaces; ++i) {	
					Face face = faces[surroundingFaces[i]];
					for (int cornerIndex = 0; cornerIndex < 3; ++cornerIndex) { // three points!
						int vertIndex = face.v[cornerIndex];
						if (vertices[vertIndex].mark != mark) {
							list.Add(vertIndex);
							vertices[vertIndex].mark = mark;
						}
					}
				}
			} else {
				// For n-gons collect prev and next corner vertices
				for (int i = 0; i < numFaces; ++i) {	
					Face face = faces[surroundingFaces[i]];
					int next = 0;
					int corner = face.cornerCount - 1;
					int prev = face.cornerCount - 2;
					for (;next < face.cornerCount; ++next) {
						if (face.v[corner] == vertexIndex) {
							int vert = face.v[next];
							if (vertices[vert].mark != mark) {
								list.Add(vert);
								vertices[vert].mark = mark;
							}
							vert = face.v[prev];
							if (vertices[vert].mark != mark) {
								list.Add(vert);
								vertices[vert].mark = mark;
							}
							break;
						}
						prev = corner;
						corner = next;
					}
				}
			}
		}	
		
		// What this basically does is search for common faces in the two linkedFaces lists
		// It uses & destroys face marks
		public void CollectVertexPairFaces(VertexPair pair, IndexList commonFaces) {
			IndexList il0 = vertices[pair.v[0]].linkedFaces; // surrounding faces
			IndexList il1 = vertices[pair.v[1]].linkedFaces; // surrounding faces		
			int[] v0Faces = il0.array;
			int[] v1Faces = il1.array;
			int v0Count = il0.Count;
			int v1Count = il1.Count;
			int tag = GetUniqueTag();
			
			commonFaces.GrowToCapacity(v1Count);
			for (int i = 0; i < v0Count; ++i) faces[v0Faces[i]].mark = tag;
			for (int i = 0; i < v1Count; ++i) {
				int faceIndex = v1Faces[i];
				if (faces[faceIndex].mark == tag) {
					commonFaces.AddUnsafe(faceIndex);
				}
			}
		}
				
		public void CollectCollapseFacesForVertexPair(VertexPair pair, IndexList changeFaces0, IndexList changeFaces1, IndexList commonFaces) {
			IndexList il0 = vertices[pair.v[0]].linkedFaces;
			IndexList il1 = vertices[pair.v[1]].linkedFaces;
			int[] v0Faces = il0.array;
			int[] v1Faces = il1.array;
			int v0Count = il0.Count;
			int v1Count = il1.Count;
			int tag = GetUniqueTag();
			
			// Grow target lists to save on checks later
			changeFaces0.GrowToCapacity(v0Count);
			changeFaces1.GrowToCapacity(v1Count);
			commonFaces.GrowToCapacity(v0Count); // could be min(v0count, v1count), but that's probably slower
			
			for (int i = 0; i < v1Count; ++i) faces[v1Faces[i]].mark = tag;
			for (int i = 0; i < v0Count; ++i) {
				int faceIndex = v0Faces[i];
				Face f = faces[faceIndex];
			//	if (f.valid) {
					if (f.mark == tag) {
						commonFaces.AddUnsafe(faceIndex);
						f.mark = 0;
					} else {
						changeFaces0.AddUnsafe(faceIndex);
					}
			//	}
			}
			for (int i = 0; i < v1Count; ++i) {
				int faceIndex = v1Faces[i];
				Face f = faces[faceIndex];
				if (/*f.valid &&*/ f.mark == tag) {
					changeFaces1.AddUnsafe(faceIndex);
				}
			}
		}
		
		IndexList changeFaces0 = new IndexList(32);
		IndexList changeFaces1 = new IndexList(32);
		IndexList removeFaces  = new IndexList(32);
		
		// REQUIRES triangulated mesh!
		public int CollapseVertexPair(CollapseInfo info) {
			if (topology != MeshTopology.Triangles) {
				Debug.LogError("KrablMesh: Collapsing a vertex pair requires a triangle mesh");
				return 0;
			}
			
			VertexPair pair = info.vp;
			int vindex0 = pair.v[0];
			int vindex1 = pair.v[1];
			Vertex vertex0 = vertices[vindex0];
			Vertex vertex1 = vertices[vindex1];
			int i, j;
			
			changeFaces0.Clear();
			changeFaces1.Clear();
			removeFaces.Clear();
			CollectCollapseFacesForVertexPair(pair, changeFaces0, changeFaces1, removeFaces);			
							
			// Adjust parameters of vertex0 to the new position
			float ratio1 = info.Ratio(this);
			
			// try baricentric projection on all the removeFaces (usually 2)
			int projFaceIndex = -1;
			Face projFace = null;
			int projCorner0 = 0, projCorner1 = 0;
			Vector3 bari = Vector3.zero;
			int[] v = null;
			for (i = 0; i < removeFaces.Count; ++i) {
				Face f = faces[removeFaces[i]];
				v = f.v;
				bari = UnityUtils.BaricentricProjection(info.targetPosition, vertices[v[0]].coords, vertices[v[1]].coords, vertices[v[2]].coords);		
				if (UnityUtils.AreBaricentricCoordsInsideTriangle(bari)) {
					projFaceIndex = removeFaces[i];
					projFace = f;
					projCorner0 = projFace.CornerIndexTriangle(vindex0);
					projCorner1 = projFace.CornerIndexTriangle(vindex1);
					break;
				} 
			}
			// There must not be invalid faces in changeFaces0 or changeFaces1 !!!
		/*	for (i = 0; i < changeFaces0.Count; ++i) if (faces[changeFaces0[i]].valid == false) Debug.LogError("NOOO!");
			for (i = 0; i < changeFaces1.Count; ++i) if (faces[changeFaces1[i]].valid == false) Debug.LogError("NOOO!");
			for (i = 0; i < removeFaces.Count; ++i) if (faces[removeFaces[i]].valid == false) Debug.LogError("NOOO!");
			*/				
			// Deal with vertex colors and boneweights. these are per vertex.
			if (projFace != null) {
				if (hasVertexColors) vertex0.color = bari.x*vertices[v[0]].color + bari.y*vertices[v[1]].color + bari.z*vertices[v[2]].color;
				if (hasBoneWeights) vertex0.boneWeight = UnityUtils.BoneWeightBaricentricInterpolation(vertices[v[0]].boneWeight, vertices[v[1]].boneWeight, vertices[v[2]].boneWeight, bari.x, bari.y, bari.z);
			} else {
				if (hasVertexColors) vertex0.color = Color.Lerp(vertex0.color, vertex1.color, ratio1);
				if (hasBoneWeights) vertex0.boneWeight = UnityUtils.BoneWeightLerp(vertex0.boneWeight, vertex1.boneWeight, ratio1);
			}
			
			// Determine corner numbers for v0 in changefaces0 and v1 in changefaces1
			IndexList corners0 = new IndexList(changeFaces0.Count);
			for (i = 0; i < changeFaces0.Count; ++i) corners0[i] = faces[changeFaces0[i]].CornerIndexTriangle(vindex0);
			IndexList corners1 = new IndexList(changeFaces1.Count);
			for (i = 0; i < changeFaces1.Count; ++i) corners1[i] = faces[changeFaces1[i]].CornerIndexTriangle(vindex1);
			
			#region Face-Dependent Attributes (Vertex normals, uv1, uv2)
			
			// NORMALS
			int count = 0, filterTag = GetUniqueTag();
			Vector3 projNormalNew = Vector3.zero;
			if (projFace != null) {
				projNormalNew = bari.x*projFace.vertexNormal[0] + bari.y*projFace.vertexNormal[1] + bari.z*projFace.vertexNormal[2];					
				count = _replaceCornerNormalInFaceGroup(projFace.vertexNormal[projCorner0], projNormalNew, changeFaces0, corners0, filterTag);						
			}
			if (count < changeFaces0.Count) {
				// there are faces which cannot use baricentric projection
				for (j = 0; j < removeFaces.Count; ++j) {
					if (removeFaces[j] != projFaceIndex) {
						Face f2 = faces[removeFaces[j]]; int c0 = f2.CornerIndexTriangle(vindex0), c1 = f2.CornerIndexTriangle(vindex1);
						Vector3 oldNormal = f2.vertexNormal[c0];
						_replaceCornerNormalInFaceGroup(oldNormal, Vector3.Lerp(oldNormal, f2.vertexNormal[c1], ratio1), changeFaces0, corners0, filterTag);
					}
				}
			}
			
			count = 0; filterTag = GetUniqueTag();
			if (projFace != null) {
				count = _replaceCornerNormalInFaceGroup(projFace.vertexNormal[projCorner1], projNormalNew, changeFaces1, corners1, filterTag);			
			}
			if (count < changeFaces1.Count) {
				// there are faces which cannot use baricentric projection
				for (j = 0; j < removeFaces.Count; ++j) {
					if (removeFaces[j] != projFaceIndex) {
						Face f2 = faces[removeFaces[j]]; int c0 = f2.CornerIndexTriangle(vindex0), c1 = f2.CornerIndexTriangle(vindex1);
						Vector3 oldNormal = f2.vertexNormal[c1];
						_replaceCornerNormalInFaceGroup(oldNormal, Vector3.Lerp(f2.vertexNormal[c0], oldNormal, ratio1), changeFaces1, corners1, filterTag);
					}
				}
			}
			
			if (hasUV1) {
				count = 0; filterTag = GetUniqueTag();
				Vector2	projUV1New = Vector2.zero;
				if (projFace != null) {
					projUV1New = bari.x*projFace.uv1[0] + bari.y*projFace.uv1[1] + bari.z*projFace.uv1[2];					
					count = _replaceCornerUV1InFaceGroup(projFace.uv1[projCorner0], projUV1New, changeFaces0, corners0, filterTag);						
				}
				if (count < changeFaces0.Count) {
					// there are faces which cannot use baricentric projection
					for (j = 0; j < removeFaces.Count; ++j) {
						if (removeFaces[j] != projFaceIndex) {
							Face f2 = faces[removeFaces[j]]; int c0 = f2.CornerIndexTriangle(vindex0), c1 = f2.CornerIndexTriangle(vindex1);
							Vector2 oldUV1 = f2.uv1[c0];
							_replaceCornerUV1InFaceGroup(oldUV1, Vector2.Lerp(oldUV1, f2.uv1[c1], ratio1), changeFaces0, corners0, filterTag);
						}
					}
				}
			
				count = 0; filterTag = GetUniqueTag();
				if (projFace != null) {
					count = _replaceCornerUV1InFaceGroup(projFace.uv1[projCorner1], projUV1New, changeFaces1, corners1, filterTag);			
				}
				if (count < changeFaces1.Count) {
					// there are faces which cannot use baricentric projection
					for (j = 0; j < removeFaces.Count; ++j) {
						if (removeFaces[j] != projFaceIndex) {
							Face f2 = faces[removeFaces[j]]; int c0 = f2.CornerIndexTriangle(vindex0), c1 = f2.CornerIndexTriangle(vindex1);
							Vector2 oldUV1 = f2.uv1[c1];
							_replaceCornerUV1InFaceGroup(oldUV1, Vector2.Lerp(f2.uv1[c0], oldUV1, ratio1), changeFaces1, corners1, filterTag);
						}
					}
				}
			}

			if (hasUV2) {
				count = 0; filterTag = GetUniqueTag();
				Vector2	projUV2New = Vector2.zero;
				if (projFace != null) {
					projUV2New = bari.x*projFace.uv2[0] + bari.y*projFace.uv2[1] + bari.z*projFace.uv2[2];					
					count = _replaceCornerUV2InFaceGroup(projFace.uv2[projCorner0], projUV2New, changeFaces0, corners0, filterTag);						
				}
				if (count < changeFaces0.Count) {
					// there are faces which cannot use baricentric projection
					for (j = 0; j < removeFaces.Count; ++j) {
						if (removeFaces[j] != projFaceIndex) {
							Face f2 = faces[removeFaces[j]]; int c0 = f2.CornerIndexTriangle(vindex0), c1 = f2.CornerIndexTriangle(vindex1);
							Vector2 oldUV2 = f2.uv2[c0];
							_replaceCornerUV2InFaceGroup(oldUV2, Vector2.Lerp(oldUV2, f2.uv2[c1], ratio1), changeFaces0, corners0, filterTag);
						}
					}
				}
			
				count = 0; filterTag = GetUniqueTag();
				if (projFace != null) {
					count = _replaceCornerUV2InFaceGroup(projFace.uv2[projCorner1], projUV2New, changeFaces1, corners1, filterTag);			
				}
				if (count < changeFaces1.Count) {
					// there are faces which cannot use baricentric projection
					for (j = 0; j < removeFaces.Count; ++j) {
						if (removeFaces[j] != projFaceIndex) {
							Face f2 = faces[removeFaces[j]]; int c0 = f2.CornerIndexTriangle(vindex0), c1 = f2.CornerIndexTriangle(vindex1);
							Vector2 oldUV2 = f2.uv2[c1];
							_replaceCornerUV2InFaceGroup(oldUV2, Vector2.Lerp(f2.uv2[c0], oldUV2, ratio1), changeFaces1, corners1, filterTag);
						}
					}
				}
			}
			#endregion
			// Move vertex to goal position
			vertex0.coords = info.targetPosition;
			
			// remove faces
		//	Debug.Log("Change faces 1 num: " + changeFaces0.Count);
		//	Debug.Log("Change faces 2 num: " + changeFaces1.Count);
		//	Debug.Log("Remove faces num: " + removeFaces.Count);
			for (i = 0; i < removeFaces.Count; ++i) {
				UnlinkFace(removeFaces[i]);
			}
			
			// change vertex on vindex1 faces, update surrounding faces on vindex0
			for (i = 0; i < changeFaces1.Count; ++i) {
				int faceIndex = changeFaces1[i];
				Face f = faces[faceIndex];
				if (f.valid) {
					f.ReplaceVertex(vindex1, vindex0);
					vertex0.linkedFaces.Add(faceIndex);
				}
			}

			// mark vindex1 as invalid
			vertex1.linkedFaces.Clear();
			if (vertex1.valid == true) {
				numValidVerts--;
				vertex1.valid = false;
			} else {
				Debug.LogError("vindex1 was already invalid");
			}
			return vindex0;
		}
		
		int _replaceCornerNormalInFaceGroup(Vector3 oldNormal, Vector3 newNormal, IndexList faceList, IndexList corners, int filterTag) {
			int count = 0;
			for (int i = 0; i < faceList.Count; ++i) {
				Face f = faces[faceList[i]];
				if (f.mark == filterTag) continue;
				if (UnityUtils.Vector3CompareWithTolerance(f.vertexNormal[corners[i]], oldNormal, equalityTolerance) == 0) {
					f.vertexNormal[corners[i]] = newNormal;
					f.mark = filterTag;
					count++;
				}
			}
			return count;
		}

		int _replaceCornerUV1InFaceGroup(Vector2 oldUV1, Vector2 newUV1, IndexList faceList, IndexList corners, int filterTag) {
			int count = 0;
			for (int i = 0; i < faceList.Count; ++i) {
				Face f = faces[faceList[i]];
				if (f.mark == filterTag) continue;
				if (UnityUtils.Vector2CompareWithTolerance(f.uv1[corners[i]], oldUV1, equalityTolerance) == 0) {
					f.uv1[corners[i]] = newUV1;
					f.mark = filterTag;
					count++;
				}
			}
			return count;
		}

		int _replaceCornerUV2InFaceGroup(Vector2 oldUV2, Vector2 newUV2, IndexList faceList, IndexList corners, int filterTag) {
			int count = 0;
			for (int i = 0; i < faceList.Count; ++i) {
				Face f = faces[faceList[i]];
				if (f.mark == filterTag) continue;
				if (UnityUtils.Vector2CompareWithTolerance(f.uv2[corners[i]], oldUV2, equalityTolerance) == 0) {
					f.uv2[corners[i]] = newUV2;
					f.mark = filterTag;
					count++;
				}
			}
			return count;
		}

		public int InvalidateUnconnectedVertices() {
			int vertCount = vertices.Count;
			int count = 0;
			for (int i = 0; i < vertCount; ++i) {
				Vertex v = vertices[i];
				if (v.valid) {
					if (v.linkedFaces.Count == 0) {
						//Debug.Log("Removed vertex without faces");
						v.valid = false;
						numValidVerts--;
						count++;
					}
				}
			}
			return count;
		}
		
		public int InvalidateDegenerateFaces() {
			int faceCount = faces.Count;
			int count = 0;
			for (int faceIndex = 0; faceIndex < faceCount; ++faceIndex) {
				Face f = faces[faceIndex];
				if (f.valid) {
					int[] v = f.v;
					int numCorners = f.cornerCount;
					for (int j = 0; j < numCorners; ++j) {
						if (IsVertexValid(v[j]) == false) {
							UnlinkFace(faceIndex);
							count++;
							break; // j!
						}
						for (int k = j + 1; k < numCorners; ++k) {
							if (v[j] == v[k]) {
								UnlinkFace(faceIndex);
								count++;
								j = numCorners; break;
							}
						}
					}
					// todo? maybe get rid of faces with area 0?
				}
			}
			return count;
		}
		
		public void RebuildMesh(ref int[] vertTable, ref int[] faceTable, bool verbose = false) {
			InvalidateUnconnectedVertices();
			InvalidateDegenerateFaces();
			int numVerts = vertCount();
			int numFaces = faceCount();
			
			vertTable = new int[numVerts];
			faceTable = new int[numFaces];
			List<Vertex> newVerts = new List<Vertex>();
			List<Face> newFaces = new List<Face>();
			
			for (int i = 0; i < numVerts; ++i) {
				if (vertices[i].valid) {
					vertTable[i] = newVerts.Count;
					newVerts.Add(vertices[i]);
				} else {
					vertTable[i] = -1;
				}
			}
			for (int i = 0; i < numFaces; ++i) {
				if (faces[i].valid) {
					faceTable[i] = newFaces.Count;
					newFaces.Add(faces[i]);
				} else {
					faceTable[i] = -1;
				}
			}
			vertices = newVerts;
			faces = newFaces;
			// Update index
			if (verbose) {
				Debug.Log("Rebuild Vertex Count " + numVerts + " -> " + vertCount());
				Debug.Log("Rebuild Face Count " + numFaces + " -> " + faceCount());
			}
			numVerts = vertCount();
			numFaces = faceCount();
			for (int i = 0; i < numVerts; ++i) {
				IndexList lFaces = vertices[i].linkedFaces;
				IndexList lFacesNew = new IndexList(18);
				for (int j = 0; j < lFaces.Count; ++j) {
					int l = faceTable[lFaces[j]];
					if (l != -1) lFacesNew.Add(l);
					else Debug.LogError("!!!!!");
				}
				vertices[i].linkedFaces = lFacesNew;
			}
			for (int i = 0; i < numFaces; ++i) {
				int[] v = faces[i].v;
				for (int j = 0; j < faces[i].cornerCount; ++j) {
					v[j] = vertTable[v[j]];
				}
			}	
			numValidVerts = numVerts;
			numValidFaces = numFaces;
		}
		
		public void RebuildVertexLinkedFaces()
		{
			int numVerts = vertCount();
			int numFaces = faceCount();
			
			for (int vertexIndex = 0; vertexIndex < numVerts; ++vertexIndex) {
				vertices[vertexIndex].linkedFaces.Clear();
			}
			for (int faceIndex = 0; faceIndex < numFaces; ++faceIndex) {
				Face f = faces[faceIndex];
				for (int cornerIndex = 0; cornerIndex < f.cornerCount; ++cornerIndex) {
					vertices[f.v[cornerIndex]].linkedFaces.Add(faceIndex);
				}
			}
		}
		
	}
}
