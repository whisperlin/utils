/**
 * @file         ShaderReferenceMath.cs
 * @author       Hongwei Li(taecg@qq.com)
 * @created      2018-11-16
 * @updated      2019-02-26
 *
 * @brief        数学运算相关
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace taecg.tools.shaderReference
{
    public class ShaderReferenceMath : EditorWindow
    {
        #region 数据成员
        private Vector2 scrollPos;
        #endregion

        public void DrawMainGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            ShaderReferenceUtil.DrawTitle("Math");
            ShaderReferenceUtil.DrawOneContent("abs (x)", "取绝对值，即正值返回正值，负值返回的还是正值\nx值可以为向量");
            ShaderReferenceUtil.DrawOneContent("ceil (x)", "对x进行向上取整，即x=0.1返回1，x=1.5返回2，x=-0.3返回0");
            ShaderReferenceUtil.DrawOneContent("clamp (x)", "返回不小于等于x的整数，即x=0.1返回1，x=1.5返回2，x=-0.3返回0");

            ShaderReferenceUtil.DrawOneContent("dot (a,b)", "点乘，a和b必须为三维向量或者四维向量,其计算结果是两个向量夹角的余弦值，相当于a.x*b.x+a.y*b.y+a.z*b.z\na和b的位置无所谓前后，结果都是一样的\n");
            ShaderReferenceUtil.DrawOneContent("exp (x)", "计算x的指数");
            ShaderReferenceUtil.DrawOneContent("exp2 (x)", "计算2的x次方");

            ShaderReferenceUtil.DrawOneContent("floor (x)", "对x值进行向下取整\n比如:floor (0.6) = 0.0,floor (-0.6) = -1.0");
            ShaderReferenceUtil.DrawOneContent("fmod (x,y)", "返回x/y的余数。如果y为0，结果不可预料");
            ShaderReferenceUtil.DrawOneContent("frac (xy)", "返回x的小数部分");

            ShaderReferenceUtil.DrawOneContent("length (v)", "返回一个向量的模，即 sqrt(dot(v,v))");
            ShaderReferenceUtil.DrawOneContent("lerp (A,B,alpha)", "线性插值.\n如果alpha=0，则返回A;\n如果alpha=1，则返回B;\n否则返回A与B的混合值");
            ShaderReferenceUtil.DrawOneContent("normalize (v)", "归一化向量");
            ShaderReferenceUtil.DrawOneContent("reflect(I, N)", "根据入射光方向向量 I ，和顶点法向量 N ，计算反射光方向向量。其中 I 和 N 必须被归一化，需要非常注意的是，这个 I 是指向顶点的；函数只对三元向量有效。");
            ShaderReferenceUtil.DrawOneContent("refract(I,N,eta)", "计算折射向量， I 为入射光线， N 为法向量， eta 为折射系数；其中 I 和 N 必须被归一化，如果 I 和 N 之间的夹角太大，则返回（ 0 ， 0 ， 0 ），也就是没有折射光线； I 是指向顶点的；函数只对三元向量有效。");
            ShaderReferenceUtil.DrawOneContent("smoothstep (min,max,x)", "如果 x 比min 小，返回 0\n如果 x 比max 大，返回 1\n如果 x 处于范围 [min，max]中，则返回 0 和 1 之间的值(按值在min和max间的比例).");
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif