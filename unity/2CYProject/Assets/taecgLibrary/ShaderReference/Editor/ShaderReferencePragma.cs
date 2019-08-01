/**
 * @file         ShaderReferencePragma.cs
 * @author       Hongwei Li(taecg@qq.com)
 * @created      2019-01-17
 * @updated      2019-01-17
 *
 * @brief        Pass中的Pragma
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace taecg.tools.shaderReference
{
    public class ShaderReferencePragma : EditorWindow
    {
        #region 数据成员
        private Vector2 scrollPos;
        #endregion

        public void DrawMainGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            ShaderReferenceUtil.DrawTitle("Pragma");
            ShaderReferenceUtil.DrawOneContent("#pragma enable_d3d11_debug_symbols", "开启d3d11调试，加此命令后相关的名称与代码不会被剔除，便于在调试工具中进行查看分析");
            //ShaderReferenceUtil.DrawOneContent("ceil (x)", "返回不小于等于x的整数，即x=0.1返回1，x=1.5返回2，x=-0.3返回0");
            //ShaderReferenceUtil.DrawOneContent("clamp (x)", "返回不小于等于x的整数，即x=0.1返回1，x=1.5返回2，x=-0.3返回0");
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif