using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KrablMesh {	
	class PlaneDistanceError {
		double[] cf = new double[10];
		double _fact;
			
		public PlaneDistanceError() {
		}

		public PlaneDistanceError(PlaneDistanceError b) {
			_fact = b._fact;
			for (int i = 0; i < 10; ++i) cf[i] = b.cf[i];
		}

		public PlaneDistanceError(double a, double b, double c, double d, double fact) {
			Set(a, b, c, d, fact);
		}
		
		public void Set(double a, double b, double c, double d, double fact) {
			cf[0] = a*a; cf[1] = a*b; cf[2] = a*c; cf[3] = a*d;				
			cf[4] = b*b; cf[5] = b*c; cf[6] = b*d; 
			cf[7] = c*c; cf[8] = c*d;
			cf[9] = d*d;
			
			_fact = fact;
		}
		
		public void Clear() {
			_fact = 0.0;
			for (int i = 0; i < 10; ++i) cf[i] = 0.0;	
		}
		
		public void OpAdd(PlaneDistanceError b) {
			_fact += b._fact;
			for (int i = 0; i < 10; ++i) cf[i] += b.cf[i];
		}
		
		public void OpMul(double val) {
			for (int i = 0; i < 10; ++i) cf[i] *= val;
		}
		
		public void SetFactor(double fact) {
			_fact = fact;
		}
		
		public double Factor() {
			return _fact;
		}
		
		public double CalculateError(Vector3 pt) {
			// Evaluate Av^2 + 2bv + c
			return pt.x*(pt.x*cf[0] + 2.0*(pt.y*cf[1] + pt.z*cf[2] + cf[3]))
				+ pt.y*(pt.y*cf[4] + 2.0*(pt.z*cf[5] + cf[6]))
				+ pt.z*(pt.z*cf[7] + cf[8] + cf[8])
				+ cf[9];
		}
					
		public bool OptimalVertex(ref Vector3 result) {
			double t00 = cf[4]*cf[7] - cf[5]*cf[5];
			double t01 = cf[5]*cf[2] - cf[7]*cf[1];
			double t02 = cf[1]*cf[5] - cf[2]*cf[4];
			
			double det = cf[0]*t00 + cf[1]*t01 + cf[2]*t02;

			if (det > -1e-12 && det < 1e-12) return false;
			det = -1.0/det;
			 
		//	double t10 = t01; //(cf[5]*cf[2] - cf[1]*cf[7]);
			double t11 = cf[7]*cf[0] - cf[2]*cf[2];
			double t12 = cf[2]*cf[1] - cf[0]*cf[5];
		//	double t20 = t02; // cf[1]*cf[5] - cf[4]*cf[2];
		//	double t21 = t12; // cf[2]*cf[1] - cf[5]*cf[0];
			double t22 = cf[0]*cf[4] - cf[1]*cf[1];
											
			result.x = (float)(det*(t00*cf[3] + t01*cf[6] + t02*cf[8]));
			result.y = (float)(det*(t01*cf[3] + t11*cf[6] + t12*cf[8]));
			result.z = (float)(det*(t02*cf[3] + t12*cf[6] + t22*cf[8]));
			
			return true;
		}
		
		public bool OptimalVertexLinear(ref Vector3 result, Vector3 p, Vector3 q) {
			Vector3 v = p - q;
			
			double av0 = cf[0]*v.x + cf[1]*v.y + cf[2]*v.z;
			double av1 = cf[1]*v.x + cf[4]*v.y + cf[5]*v.z;
			double av2 = cf[2]*v.x + cf[5]*v.y + cf[7]*v.z;
				
			double denom = v.x*av0 + v.y*av1 + v.z*av2;
			if (denom > -1e-12 && denom < 1e-12) return false;
			denom = 1.0/(denom + denom);
				
			double aq0 = cf[0]*q.x + cf[1]*q.y + cf[2]*q.z;
			double aq1 = cf[1]*q.x + cf[4]*q.y + cf[5]*q.z;
			double aq2 = cf[2]*q.x + cf[5]*q.y + cf[7]*q.z;
			
			double param = (-2.0*(cf[3]*v.x + cf[6]*v.y + cf[8]*v.z)
				- (v.x*aq0 + v.y*aq1 + v.z*aq2)
				- (q.x*av0 + q.y*av1 + q.z*av2))*denom;
			result = q + Mathf.Clamp01((float)param)*v;
			return true;
		}
		
		public static PlaneDistanceError operator + (PlaneDistanceError a, PlaneDistanceError b) {
			PlaneDistanceError res = new PlaneDistanceError(a);
			res.OpAdd(b);
			return res;
		}
	}
}