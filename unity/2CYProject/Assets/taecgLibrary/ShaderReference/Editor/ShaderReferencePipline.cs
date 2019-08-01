/**
 * @file         ShaderReferencePipline.cs
 * @author       Hongwei Li(taecg@qq.com)
 * @created      2018-11-17
 * @updated      2018-11-17
 *
 * @brief        渲染管线
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace taecg.tools.shaderReference
{
    public class ShaderReferencePipline : EditorWindow
    {
        #region 数据成员
        private Vector2 scrollPos;
        #endregion

        public void DrawMainGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            ShaderReferenceUtil.DrawTitle("应用程序阶段（ Application Stage）");
            ShaderReferenceUtil.DrawOneContent("0.Application Stage", "此阶段将需要在屏幕上绘制的几何体、摄像机位置、光照等输出到管线的几何阶段。");

            ShaderReferenceUtil.DrawTitle("几何阶段（ Geometry Stage）");
            ShaderReferenceUtil.DrawOneContent("1.模型和视图变换（Model and View Transform）", "模型和视图变换阶段分为模型变换和视图变换.\n模型变换的目的是将模型变换到适合渲染的空间当中，而视图变换的目的是将摄像机放置于坐标原点（以使裁剪和投影操作更简单高效），方便后续步骤的处理。");
            ShaderReferenceUtil.DrawOneContent("2.顶点着色（Vertex Shading）", "对应于ShaderLab中的vertex函数.\n顶点着色阶段的目的在于确定模型上顶点处的光照效果,其输出结果（颜色、向量、纹理坐标等）会被发送到光栅化阶段以进行插值操作。");
            ShaderReferenceUtil.DrawOneContent("3.投影（Projection）", "投影阶段分为正交投影与透视投影.\n投影其实就是矩阵变换，最终会使坐标位于归一化的设备坐标中，之所以叫投影就是因为最终Z轴坐标将被舍弃，也就是说此阶段主要的目的是将模型从三维空间投射到了二维的空间中的过程（但是坐标仍然是三维的，只是显示上看是二维的）。");
            ShaderReferenceUtil.DrawOneContent("4.裁剪（Clipping）", "裁剪根据图元在视体的位置分为三种情况：\n1.当图元完全位于视体内部，那么它可以直接进行下一个阶段。\n2.当图元完全位于视体外部，则不会进入下一个阶段，直接丢弃。\n3.当图元部分位于视体内部，则需要对位于视体内的图元进行裁剪处理。\n最终的目的就是对部分位于视体内部的图元进行裁剪操作，以使处于视体外部不需要渲染的图元进行裁剪丢弃。");
            ShaderReferenceUtil.DrawOneContent("5.屏幕映射（Screen Mapping）", "屏幕映射阶段的主要目的，是将之前步骤得到的坐标映射到对应的屏幕坐标系上。");

            ShaderReferenceUtil.DrawTitle("光栅化阶段（Rasterizer Stage）");
            ShaderReferenceUtil.DrawOneContent("6.三角形设定（Triangle Setup）", "主要用来计算三角形表面的差异和三角形表面的其他相关数据。该数据主要用于三角形遍历，以及由几何阶段处理的各种着色数据的插值操作所用。该过程在专门为其设计的硬件上执行。");
            ShaderReferenceUtil.DrawOneContent("7.三角形遍历（Triangle Traversal）", "进行逐像素遍历检查操作，检查该像素是否由三角形覆盖，找到哪些采样点或像素在三角形中的过程通常叫三角形遍历");
            ShaderReferenceUtil.DrawOneContent("8.像素着色(Pixel Shading)", "对应于ShaderLab中的frag函数.\n主要目的是计算所有需逐像素操作的过程,纹理贴图操作就是在这阶段进行的.");
            ShaderReferenceUtil.DrawOneContent("9.混合（Merging）", "主要任务是合成当前储存于缓冲器中的由之前的像素着色阶段产生的片段颜色。此阶段还负责可见性问题（Z 缓冲相关）的处理.");

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif