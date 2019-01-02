using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KrablMesh {
	public class EditorDrawing {	
		public static void DrawMesh(KrablMesh.BaseMesh mesh, Transform transform) {
			int numVerts = mesh.vertCount();
			int numFaces = mesh.faceCount();
		
			Vector3[] pts = new Vector3[numVerts];
			for (int i = 0 ; i < numVerts; ++i) {
				Vertex v = mesh.vertices[i];
				pts[i] = transform.TransformPoint(v.coords);
			}
			
			Gizmos.color = Color.red;
			for (int i = 0; i < numFaces; ++i) {
				Face f = mesh.faces[i];
				if (f.valid) {
					int[] vi = f.v;
					int j;
					for (j = 0; j < f.cornerCount - 1; ++j) {
						Gizmos.DrawLine(pts[vi[j]], pts[vi[j + 1]]);
					}
					Gizmos.DrawLine(pts[vi[j]], pts[vi[0]]);
				}
			}
		}
				
		public static void DrawMeshEdges(KrablMesh.MeshEdges mesh, Transform transform, float normalLength = 0.0f, SkinnedMeshRenderer smr = null) {
			int numVerts = mesh.vertCount();
			int numEdges = mesh.edgeCount();
			Vector3[] pts = new Vector3[numVerts];
			Matrix4x4[] boneMatrices = null;
			Matrix4x4 mat = new Matrix4x4();
			
			bool useBones = (smr != null) && (mesh.hasBoneWeights);
			
			if (useBones) {
				boneMatrices = BoneMatricesFromBonesAndBindposes(mesh.bindposes, smr.bones);
				for (int i = 0 ; i < numVerts; ++i) {
					Vertex v = mesh.vertices[i];
					DeformationMatrixForBoneWeight(v.boneWeight, boneMatrices, ref mat);
					pts[i] = mat.MultiplyPoint3x4(v.coords);
				}
			} else {			
				for (int i = 0 ; i < numVerts; ++i) {
					Vertex v = mesh.vertices[i];
					pts[i] = transform.TransformPoint(v.coords);
				}
			}
			
			for (int i = 0; i < numEdges; ++i) {
				if (mesh.IsEdgeValid(i)) {
					Edge e = mesh.edges[i];
					Color col = Color.clear;
					if (e.linkedFaces.Count <= 1) {
						col = Color.blue;
					}
					if (mesh.hasUV1 && mesh.IsEdgeUV1Seam(i)) {
						col = Color.magenta;
					}
					if (e.crease > 0.0f) {
						col = Color.red;
					}
					if (col.a != 0.0f) {
						Gizmos.color = col;						
						Gizmos.DrawLine(pts[e.v[0]], pts[e.v[1]]);
					}
				}
			}
			
			if (normalLength > 0.0f) {
				Gizmos.color = Color.white;
				int numFaces = mesh.faceCount();
				if (useBones == false) {
					for (int i = 0; i < numFaces; ++i) {
						Face f = mesh.faces[i];
						if (f.valid) {
							int[] vi = f.v;
							for (int j = 0; j < f.cornerCount; ++j) {
								Vector3 n = f.vertexNormal[j];
								n = transform.TransformDirection(n);
								Vector3 p = pts[vi[j]];
								Gizmos.DrawLine(p, p + n*normalLength);
							}
						}
					}
				} else {
					for (int i = 0; i < numFaces; ++i) {
						Face f = mesh.faces[i];
						if (f.valid) {
							int[] vi = f.v;
							for (int j = 0; j < f.cornerCount; ++j) {
								int vindex = vi[j];
								DeformationMatrixForBoneWeight(mesh.vertices[vindex].boneWeight, boneMatrices, ref mat);	
								Vector3 n = f.vertexNormal[j];
								n = mat.MultiplyVector(n);
								Vector3 p = pts[vindex];
								Gizmos.DrawLine(p, p + n.normalized*normalLength);
							}
						}
					}
				}
			}			
		}
		
		public static Matrix4x4[] BoneMatricesFromBonesAndBindposes(Matrix4x4[] bindposes, Transform[] bones) {
			Matrix4x4[] boneMat = new Matrix4x4[bones.Length];

       		for (int i = 0; i < bones.Length; ++i) {
            	boneMat[i] = bones[i].localToWorldMatrix*bindposes[i];
			}
			return boneMat;
		}
		
		public static void DeformationMatrixForBoneWeight(BoneWeight bw, Matrix4x4[] boneMatrices, ref Matrix4x4 matrix) {
			int j;
			
			Matrix4x4 bm = boneMatrices[bw.boneIndex0];
 			float w = bw.weight0;
         	for (j = 0; j < 16; ++j) matrix[j] = bm[j]*w;

			w = bw.weight1;
 			if (w != 0.0f) {
				bm = boneMatrices[bw.boneIndex1];
       			for (j = 0; j < 16; ++j) matrix[j] += bm[j]*w;
			}

			w = bw.weight2;
 			if (w != 0.0f) {
				bm = boneMatrices[bw.boneIndex2];
       			for (j = 0; j < 16; ++j) matrix[j] += bm[j]*w;
			}

			w = bw.weight3;
 			if (w != 0.0f) {
				bm = boneMatrices[bw.boneIndex3];
       			for (j = 0; j < 16; ++j) matrix[j] += bm[j]*w;
			}
        }	

	}
}
