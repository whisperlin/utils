using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Engine.IntUtil;
public class TestGC : MonoBehaviour {

	public Dictionary<int ,int> dic0 = new Dictionary<int, int>();
	public Dictionary<int ,int> dic1 = new Dictionary<int, int>(GCUtil.int_cmp);


	public Dictionary<string ,string> sdic0 = new Dictionary<string, string>();
	public Dictionary<string ,string> sdic1 = new Dictionary<string, string>(GCUtil.str_cmp);


	public Dictionary<IntVector3 ,IntVector2> v3dic0 = new Dictionary<IntVector3, IntVector2>();
	public Dictionary<IntVector3 ,IntVector2> v3dic1 = new Dictionary<IntVector3, IntVector2>(GCUtil.vec3_cmp);

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 100; i++) {
			dic0.Add (i, i % 10);
			dic1.Add (i, i % 10);
			sdic0.Add(i.ToString(),i.ToString());
			sdic1.Add(i.ToString(),i.ToString());
			v3dic0.Add (new IntVector3 (i, i, i), new IntVector2 (i,i));
			v3dic1.Add (new IntVector3 (i, i, i), new IntVector2 (i,i));
		}
	}
	
	// Update is called once per frame
	void Update () {
		//for (int i = 0; i < 100; i++) {
			int v;
			string s;
			IntVector2 v2;
			IntVector3 v3 = new IntVector3(1,1,1);
			Profiler.BeginSample("Has GC");
			Profiler.BeginSample("System Type");

			dic0.TryGetValue (1, out v);
			sdic0.TryGetValue ("1", out s);

			Profiler.EndSample ( );

			Profiler.BeginSample("User Type1");
			v3dic0.TryGetValue (v3, out v2);
			Profiler.EndSample ( );


			Profiler.EndSample ( );

			Profiler.BeginSample("No GC");
			
			Profiler.BeginSample("System Type2");
			dic1.TryGetValue (1, out v);
			sdic1.TryGetValue ("1", out s);
			Profiler.EndSample ( );

			Profiler.BeginSample("User Type");
			v3dic1.TryGetValue (v3, out v2);
			Profiler.EndSample ( );

			Profiler.EndSample ( );
		//}


		
	}
}
