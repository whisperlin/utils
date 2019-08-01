/**
 * @file         ShaderReferenceAbout.cs
 * @author       Hongwei Li(taecg@qq.com)
 * @created      2018-11-10
 * @updated      2019-02-27
 *
 * @brief       制作名单
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace taecg.tools.shaderReference
{
    public class ShaderReferenceAbout : EditorWindow
    {
        #region 数据成员
        private Vector2 scrollPos;
        private Texture qq
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath (AssetDatabase.GUIDToAssetPath ("dad6600111a382542adff057f64137fe"), typeof (Texture)) as Texture;
            }
        }

        private Texture wechat
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath (AssetDatabase.GUIDToAssetPath ("08a00299070e2c345b3a1656b1965dfe"), typeof (Texture)) as Texture;
            }
        }
        #endregion

        public void DrawMainGUI ()
        {
            scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

            EditorGUILayout.Space ();

            GUIStyle labelStyle = new GUIStyle ("label");
            labelStyle.fontStyle = FontStyle.Italic;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField ("by taecg@qq.com  v0.06", labelStyle);

            GUIStyle style = new GUIStyle ();
            style.alignment = TextAnchor.MiddleCenter;

            GUILayout.Box (wechat, style);
            EditorGUILayout.TextArea ("Unity技术美术公众号 :  gh_8b69cca044dc", labelStyle);

            GUILayout.Box (qq, style);
            EditorGUILayout.TextArea ("Unity技术美术QQ交流群 :  19470667", labelStyle);

            EditorGUILayout.EndScrollView ();
        }
    }
}
#endif