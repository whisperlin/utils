/*! \mainpage Introduction
 *
 * The Krabl Mesh Library is a C\# library to process meshes. 
 * It handles triangle- and quad-based meshes and can deal with non-manifold data.
 * The meshes are stored as a vertex list and a face list which references the vertex indices.<br>
 * <br>
 * For simple scripting needs, the KrablMeshUtility class should be enough. It provides shortcuts to
 * the most common mesh operations. To learn how to do more complex processing, a look at KrablMeshUtility.cs
 * is highly recommended.<br>
 * <br>
 * This Doxygen generated documentation does not include all library functions.
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KrablMesh {
	public class Vertex {
		public Vector3 coords;
		public int mark = 0;
		public bool valid = true;
		public IndexList linkedFaces = new IndexList(18);
		public Vector3 normal;	
		public Color color;
		public BoneWeight boneWeight;
		
		public Vertex() {
			coords = Vector3.zero;
		}
		
		public Vertex(Vector3 v) {
			coords = v;
		}
		
		public Vertex(float x, float y, float z) {
			coords = new Vector3(x, y, z);
		}
	}
		
	public class VertexPair {
		public int[] v = new int[2];
		
		public VertexPair(int a, int b) {
			v[0] = a;
			v[1] = b;
		}
		
		public int OtherVertex(int vertexIndex) {
			if (vertexIndex == v[0]) return v[1];
			else return v[0];
			/*else if (v == v2) return v1;
			else Debug.LogError("Invalid Edge");
			return -1;*/
		}
		
		public void ReplaceVertex(int vertexIndexOld, int vertexIndexNew) {
			if (v[0] == vertexIndexOld) v[0] = vertexIndexNew;
			if (v[1] == vertexIndexOld) v[1] = vertexIndexNew;
	//		if (v1 == v2) {	this means the pair is invalid!			
	//		}
		}
		
		public void Invalidate(int vertexIndex = 0) {
			v[0] = vertexIndex;
			v[1] = vertexIndex;
		}
		
		public bool ContainsVertex(int vertexIndex) {
			if (v[0] == vertexIndex) return true;
			return v[1] == vertexIndex;
		}
	}
	
	public class Edge : VertexPair {
		public float crease;
		public float angle;
		public IndexList linkedFaces = new IndexList(18);
		public int mark;
		public Edge(int a, int b): base(a,b) {
		}
	}
	
	public class Face {
		public int[] v = new int[4];
		public int mark = 0;
		public bool valid = true;
		public Vector3 normal;
		public Vector3[] vertexNormal = new Vector3[4];
		public int material;
		public Vector2[] uv1 = new Vector2[4];
		public Vector2[] uv2 = new Vector2[4];
		public int cornerCount = 3;
		
		public Face(int numVerts) {
			cornerCount = numVerts;
		}	
		
		public Face(int a, int b, int c) {
			v[0] = a;
			v[1] = b;
			v[2] = c;
			cornerCount = 3;
		}

		public Face(int a, int b, int c, int d) {
			v[0] = a;
			v[1] = b;
			v[2] = c;
			v[3] = d;
			cornerCount = 4;
		}
		
		// one less comparison, but only for triangles
		public int CornerIndexTriangle(int vertexIndex) {
			if (v[0] == vertexIndex) return 0;
			if (v[1] == vertexIndex) return 1;
			return 2;
		}
		
		public int CornerIndex(int vertexIndex) {
			// new unsafe but faster way
			if (v[0] == vertexIndex) return 0;
			if (v[1] == vertexIndex) return 1;
			if (cornerCount == 3) { // can only be 3 or 4
				return 2;
			} else {
				if (v[2] == vertexIndex) return 2;
				return 3;
			}
			
			// OLD WAY WITH CHECK
			/*
			for (int i = 0; i < cornerCount; ++i) if (v[i] == vertexIndex) return i;
			Debug.LogError("Couldn't find vertex in Face: " + vertexIndex);
			for (int i = 0; i < cornerCount; ++i) Debug.Log("face vert: " + v[i]);
			return 0;
			*/
		}
					
		public Vector3 VertexNormalForVertexIndex(int vertexIndex) {
			for (int i = 0; i < cornerCount; ++i) {
				if (v[i] == vertexIndex) return vertexNormal[i];
			}
			Debug.LogError("Couldn't find vertex in Face");
			return Vector3.zero;
		}
		
		public int ReplaceVertex(int a, int b) {
			int count = 0;
			for (int i = 0; i < cornerCount; ++i) {
				if (v[i] == a) {
					v[i] = b;
					count++;
				}
			}
			return count;
		}
					
		public void CopyVertexInfoFromFace(Face sFace, int sCorner, int tCorner) {
			v[tCorner] = sFace.v[sCorner];
			vertexNormal[tCorner] = sFace.vertexNormal[sCorner];
			uv1[tCorner] = sFace.uv1[sCorner];
			uv2[tCorner] = sFace.uv2[sCorner];
		}
		
		public bool VerticesInOrder(int a, int b) {
			int i;
			for (i = 0; i < cornerCount - 1; ++i) {
				if (a == v[i]) return (b == v[i + 1]);
			}
			if (a == v[i]) return (b == v[0]);
			else Debug.LogError("isInOrder? first vertex index not found");
			return false;
		}
	}
	
	public class CollapseInfo  {
		public VertexPair vp;
		public Vector3 targetPosition;
		public float positionCost = -1.0f; // Cost of just moving to targetPosition
		public float cost; // Total cose including penalties
		
		public float Ratio(BaseMesh m) {
			return KrablMesh.UnityUtils.ProjectedRatioOfPointOnVector(targetPosition, m.vertices[vp.v[0]].coords, m.vertices[vp.v[1]].coords);
		}
	}
	
	public class IndexList {
		public int[] array;
		int _capacity;
		public int Count;
		
		public IndexList(int capacity) {
			_capacity = capacity;
			array = new int[_capacity];
		}
		
		public void Add(int val) {
			if (Count >= _capacity) {
				GrowToCapacity(_capacity + _capacity);
			}
			array[Count++] = val;
		}
		
		public void AddUnsafe(int val) {
			array[Count++] = val;
		}
		
		public void Clear() {
			Count = 0;
		}
		
		public void GrowToCapacity(int newCapacity) {
			if (newCapacity > _capacity) {
				System.Array.Resize<int>(ref array, newCapacity);
				_capacity = newCapacity;
			}
		}
		
		public void Remove(int val) {
			for (int i = 0; i < Count; ++i) {
				if (array[i] == val) {
					System.Array.Copy(array, i + 1, array, i, Count - i - 1);
					Count--;
					return;
				}
			}
		}
		
		public int this[int key] {
		    get {
				return array[key];
		    }
		    set {
				array[key] = value;
		    }
		}

	}
	
	public delegate void ProgressDelegate(string text, float val);		
}