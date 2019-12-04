using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class TestScene
{
    public static void Create(string roleId)
    {
        EditorApplication.isPlaying = true;
        GameObject root = GameObject.Find("test scene");
        if (null != root)
        {
            GameObject.DestroyImmediate(root);
        }
        root = new GameObject();
        root.name = "test scene";
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "ground";
        cube.isStatic = true;
        cube.transform.localScale = new Vector3(10f, 1f, 10f);
        cube.transform.parent = root.transform;
        cube.transform.localPosition = new Vector3(0, -1, 0);
        cube.layer = 8;
        GlobalCamp _GlobalCamp = root.AddComponent<GlobalCamp>();
        _GlobalCamp.ground = 1<<cube.layer;
        LCHRoleData data = SkillEditorData.Instance.skillsData.GetRole(roleId);

#if UNITY_EDITOR
        GameObject role = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath<GameObject>(data.mod));
        role.name = "Player";
        role.transform.parent = root.transform;
        role.transform.localPosition = Vector3.zero;

        LCharacter c = role.AddComponent<LCharacter>();
        c.camp = 0;
        c.animCtrl = role.GetComponent<Animation>();

        SimpleVirtualInput vi = role.AddComponent<SimpleVirtualInput>();
        ActionIdle idle = role.AddComponent<ActionIdle>();
        ActionWalk walk = role.AddComponent<ActionWalk>();
        ActionFall fail =  role.AddComponent<ActionFall>();
        {
            int cc = c.animCtrl.GetClipCount();
            foreach (AnimationState states in c.animCtrl)
            {
                string name = states.name;
                if (name.IndexOf("idle", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    idle.animName = name;
                }
                if (name.IndexOf("walk", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    walk.animName = name;
                }
                if (name.IndexOf("run", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    walk.animName = name;
                }
                if (name.IndexOf("down", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fail.animName = name;
                }
                if (name.IndexOf("fail", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fail.animName = name;
                }
                if (name.IndexOf("walk", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    walk.animName = name;
                }
            }
        }
        

#endif

        //GameObject role = 


    }
}
