using UnityEngine;
using System.Collections.Generic;

namespace KrablMesh {
	class UnityUtils {
		public static BoneWeight BoneWeightLerp(BoneWeight a, BoneWeight b, float fact) {
			if (fact <= 0.0f) return a;
			else if (fact >= 1.0f) return b;
		
			float afact = 1.0f - fact;

			Dictionary<int, float> resultWeightForBone = new Dictionary<int, float>(8);
			{
				int[] indexes = {	a.boneIndex0, a.boneIndex1, a.boneIndex2, a.boneIndex3,
								 	b.boneIndex0, b.boneIndex1, b.boneIndex2, b.boneIndex3};
				float[] weights = {	a.weight0*afact, a.weight1*afact, a.weight2*afact, a.weight3*afact,
									b.weight0*fact, b.weight1*fact, b.weight2*fact, b.weight3};
			
				for (int i = 0; i < 8; ++i) {
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
		
		public static BoneWeight BoneWeightBaricentricInterpolation(BoneWeight a, BoneWeight b, BoneWeight c, float facta, float factb, float factc) {
			if (facta >= 1.0f) return a;
			else if (factb >= 1.0f) return b;
			else if (factc >= 1.0f) return c;
		
			int[] indexes = {a.boneIndex0, a.boneIndex1, a.boneIndex2, a.boneIndex3,
							 b.boneIndex0, b.boneIndex1, b.boneIndex2, b.boneIndex3,
							 c.boneIndex0, c.boneIndex1, c.boneIndex2, c.boneIndex3};
			float[] weights = {	a.weight0*facta, a.weight1*facta, a.weight2*facta, a.weight3*facta,
								b.weight0*factb, b.weight1*factb, b.weight2*factb, b.weight3*factb,
								c.weight0*factc, c.weight1*factc, c.weight2*factc, c.weight3*factc};
			
			Dictionary<int, float> resultWeightForBone = new Dictionary<int, float>(12);
			for (int i = 0; i < 12; ++i) {
				if (resultWeightForBone.ContainsKey(indexes[i])) {
					resultWeightForBone[indexes[i]] += weights[i];
				} else {
					resultWeightForBone.Add(indexes[i], weights[i]);
				}
			}
			
			// Sort by weight
			List<KeyValuePair<int, float>> mList = new List<KeyValuePair<int, float>>(resultWeightForBone);
			
			mList.Sort((x,y) => y.Value.CompareTo(x.Value));
			
			// Make sure there are 4 entires
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

		// squared difference of two boneweights 0...4
		public static float BoneWeightDeltaSqr(BoneWeight a, BoneWeight b) {
			int[] indexes = {a.boneIndex0, a.boneIndex1, a.boneIndex2, a.boneIndex3,
							 b.boneIndex0, b.boneIndex1, b.boneIndex2, b.boneIndex3};
			float[] weights = {	a.weight0, a.weight1, a.weight2, a.weight3,
								-b.weight0,  -b.weight1,  -b.weight2,  -b.weight3};
			
			Dictionary<int, float> resultWeightForBone = new Dictionary<int, float>(8);
			for (int i = 0; i < 8; ++i) {
				if (resultWeightForBone.ContainsKey(indexes[i])) {
					resultWeightForBone[indexes[i]] += weights[i];
				} else {
					resultWeightForBone.Add(indexes[i], weights[i]);
				}
			}
			
			float result = 0.0f;
			foreach (KeyValuePair<int, float> pair in resultWeightForBone) {
				result += pair.Value*pair.Value;
			}			
			return result;
		}
		
		public static void NormalizeSmallVector(ref Vector3 v) {
			float mag = v.magnitude;
			if (mag != 0.0f) v *= 1.0f/mag;
			else v = Vector3.zero;
		}
		
		public static Vector3 BaricentricProjection(Vector3 point, Vector3 tr0, Vector3 tr1, Vector3 tr2) {
			Vector3 q = tr0;
			Vector3 u = tr1 - tr0;
			Vector3 v = tr2 - tr0;
			Vector3 n = Vector3.Cross(u, v);
			float sn = n.sqrMagnitude;
			
			if (sn == 0.0f) return new Vector3(0.333333f, 0.333333f, 0.333333f);
			
			float oneOverA4Squared = 1.0f/sn;
			Vector3 w = point - q;
			Vector3 res = new Vector3();
			res[2] = Vector3.Dot(Vector3.Cross(u, w), n)*oneOverA4Squared;
			res[1] = Vector3.Dot(Vector3.Cross(w, v), n)*oneOverA4Squared;
			res[0] = 1.0f - res[1] - res[2];
			return res;
		}
		
		public static bool AreBaricentricCoordsInsideTriangle(Vector3 bari) {
			if (bari.x < 0.0f || bari.x > 1.0f) return false;
			if (bari.y < 0.0f || bari.y > 1.0f) return false;
			if (bari.z < 0.0f || bari.z > 1.0f) return false;
			else return true;
		}
		
		// returns 1 for equilateral and 0 for triangle with a 180 degree angle
		// this is a squared metric for performance
		public static float TriangleCompactnessSqr(Vector3[] triangle) {
			Vector3 e0 = triangle[1] - triangle[0];
			Vector3 e1 = triangle[2] - triangle[1];
			Vector3 e2 = triangle[0] - triangle[2];
			float div = e0.sqrMagnitude + e1.sqrMagnitude + e2.sqrMagnitude;
			if (div == 0.0f) return 0.0f;
			float val = 12.0f*Vector3.Cross(e0, e1).sqrMagnitude; // 4*sqrt(3)*triangle area SQUARED
			return val/(div*div);
		}
		
		// Return the index of the axis with the largest absolute value (0 = x 1 = y 2 = z)
		public static int AxisWithLargestMagniture(Vector3 vec) {
			float v0 = Mathf.Abs(vec.x);
			float v1 = Mathf.Abs(vec.y);
			float v2 = Mathf.Abs(vec.z);
			if (v0 > v1) if (v0 > v2) return 0;
			else if (v1 > v2) return 1;
			return 2;
		}
						
		public static int Vector3CompareWithTolerance(Vector3 a, Vector3 b, float tolerance) {
			if (a.x < b.x - tolerance) return -1;
			if (a.x > b.x + tolerance) return 1;
			if (a.y < b.y - tolerance) return -1;
			if (a.y > b.y + tolerance) return 1;
			if (a.z < b.z - tolerance) return -1;
			if (a.z > b.z + tolerance) return 1;
			return 0;
		}

		public static int Vector3Compare(Vector3 a, Vector3 b) {
			if (a.x < b.x) return -1;
			if (a.x > b.x) return 1;
			if (a.y < b.y) return -1;
			if (a.y > b.y) return 1;
			if (a.z < b.z) return -1;
			if (a.z > b.z) return 1;
			return 0;
		}

		public static int Vector2CompareWithTolerance(Vector2 a, Vector2 b, float tolerance) {
			if (a.x < b.x - tolerance) return -1;
			if (a.x > b.x + tolerance) return 1;
			if (a.y < b.y - tolerance) return -1;
			if (a.y > b.y + tolerance) return 1;
			return 0;
		}

		public static int Vector2Compare(Vector2 a, Vector2 b) {
			if (a.x < b.x) return -1;
			if (a.x > b.x) return 1;
			if (a.y < b.y) return -1;
			if (a.y > b.y) return 1;
			return 0;
		}

		public static int BoneWeightCompare(BoneWeight bwa, BoneWeight bwb) {
			if (bwa.boneIndex0 < bwb.boneIndex0) return -1;
			if (bwa.boneIndex0 > bwb.boneIndex0) return 1;
			if (bwa.weight0 < bwb.weight0) return -1;
			if (bwa.weight0 > bwb.weight0) return 1;
			if (bwa.boneIndex1 < bwb.boneIndex1) return -1;
			if (bwa.boneIndex1 > bwb.boneIndex1) return 1;
			if (bwa.weight1 < bwb.weight1) return -1;
			if (bwa.weight1 > bwb.weight1) return 1;
			if (bwa.boneIndex2 < bwb.boneIndex2) return -1;
			if (bwa.boneIndex2 > bwb.boneIndex2) return 1;
			if (bwa.weight2 < bwb.weight2) return -1;
			if (bwa.weight2 > bwb.weight2) return 1;
			if (bwa.boneIndex3 < bwb.boneIndex3) return -1;
			if (bwa.boneIndex3 > bwb.boneIndex3) return 1;
			if (bwa.weight3 < bwb.weight3) return -1;
			if (bwa.weight3 > bwb.weight3) return 1;
			return 0;
		}
		
		public static float ProjectedRatioOfPointOnVector(Vector3 pt, Vector3 v0, Vector3 v1) {
			Vector3 vec = v1 - v0;
			float fact = vec.magnitude;
			if (fact == 0.0f) return 0.0f;
			fact = 1.0f/fact;
			vec *= fact;
			float dist = Vector3.Dot(pt - v0, vec);
			return Mathf.Clamp01(dist*fact);
		}
		
		// Weighted squared distance between colors. 0....1
		public static float ColorDeltaSqr(Color a, Color b) {
			float dr = (a.r - b.r)*0.30f;
			float dg = (a.g - b.g)*0.59f;
			float db = (a.b - b.b)*0.11f;
			   
			float result = dr*dr + dg*dg + db*db;
			return result*2.22123500666371f;
		}
	}
}
