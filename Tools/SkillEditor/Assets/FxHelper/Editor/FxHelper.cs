using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
public class FxHelper : EditorWindow
{
    bool b = false;
    float time = 1.23f;

    // Add menu named "My Window" to the Window menu
    [MenuItem("TA/辅助/动作测试")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        FxHelper window = (FxHelper)EditorWindow.GetWindow(typeof(FxHelper));
        window.Show();
    }

    Animation anim;
    int sel = 0;
    void OnGUI()
    {

        time = EditorGUILayout.Slider(time, 0f, 10f);
        anim = EditorGUILayout.ObjectField("动作", anim, typeof(Animation)) as Animation;
        if (null != anim)
        {
            List<string> ls = new List<string>();
            foreach (AnimationState a in anim)
            {
                if(a!=null)
                    ls.Add(a.name);

            }

            if (ls.Count>0)
            {
                sel = EditorGUILayout.Popup("动作名:", sel, ls.ToArray());
                var a = anim[ls[sel]];
                a.time = time;
                a.speed = 1f;
                anim.Play(ls[sel]);
                anim.Sample();
                anim.Stop();
                a.speed = 0f;
                ParticleSystem[] ps = GameObject.FindObjectsOfType<ParticleSystem>();
                for (int i = 0, l = ps.Length; i < l; i++)
                {
                    
                    ParticleSystem p = ps[i];
                    
                    p.Simulate(time);
                }
                
            }
            
        }
    }
}
 