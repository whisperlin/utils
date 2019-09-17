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
            p.Simulate(time*2f);//编辑器接口慢一倍？
        }

        Animation[] ans = g.GetComponentsInChildren<Animation>();
        for (int i = 0, l = ans.Length; i < l; i++)
        {
            Animation animaction = ans[i];
            animaction[animaction.clip.name].time = time;
             
         
            animaction.Play();
            animaction.Sample();
            animaction.Stop();
        }

         

    }
	
	 
}
