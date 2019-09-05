using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemHelper  {

	// Use this for initialization
	public static void Simulate( GameObject g,float time) {
        ParticleSystem[] ps = g.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0, l = ps.Length; i < l; i++)
        {
            ParticleSystem p = ps[i];
            p.Simulate(time);
        }
		
	}
	
	 
}
