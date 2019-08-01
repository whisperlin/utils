/**
 * @file         ShaderReferenceRenderState.cs
 * @author       Hongwei Li(taecg@qq.com)
 * @created      2018-12-29
 * @updated      2018-12-29
 *
 * @brief        SubShader中渲染设置
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace taecg.tools.shaderReference
{
    public class ShaderReferenceRenderState : EditorWindow
    {
        #region 数据成员
        private Vector2 scrollPos;
        #endregion

        public void DrawMainGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            ShaderReferenceUtil.DrawTitle("Cull");
            ShaderReferenceUtil.DrawOneContent("Cull Back | Front | Off", "背面剔除,默认值为Back。\nBack：表示剔除背面，也就是显示正面，这也是最常用的设置。\nFront：表示剔除前面，只显示背面。\nOff:表示关闭剔除，也就是正反面都渲染。");

            ShaderReferenceUtil.DrawTitle("深度缓冲");
            ShaderReferenceUtil.DrawOneContent("ZTest (Less | Greater | LEqual | GEqual | Equal | NotEqual | Always)", "深度测试，拿当前像素的深度值与深度缓冲中的深度值进行比较，默认值为LEqual。"
            + "\nLess：小于，表示如果当前像素的深度值小于深度缓冲中的深度值，则通过，以下类同。"
            + "\nGreater：大于。"
            + "\nLequal：小于等于。"
            + "\nGequal：大于等于。"
            + "\nEqual：等于。"
            + "\nNotEqual：不等于。"
            + "\nAlways：永远通过。");
            ShaderReferenceUtil.DrawOneContent("ZWrite On | Off", "深度写入，默认值为On。\nOn：向深度缓冲中写入深度值。\nOff：关闭深度写入。");
            ShaderReferenceUtil.DrawOneContent("Offset Factor, Units", "深度偏移，offset = (m * factor) + (r * units)，默认值为0,0"
            + "\nm：指多边形的深度斜率（在光栅化阶段计算得出）中的最大值,多边形越是与近裁剪面平行，m值就越接近0。"
            + "\nr：表示能产生在窗口坐标系的深度值中可分辨的差异的最小值，r是由具体实现OpenGL的平台指定的一个常量。"
            + "\n结论：一个大于0的offset会把模型推远，一个小于0的offset会把模型拉近。");

            ShaderReferenceUtil.DrawTitle("颜色遮罩");
            ShaderReferenceUtil.DrawOneContent("ColorMask RGB | A | 0 | R、G、B、A的任意组合", "颜色遮罩，默认值为：RGBA，表示写入RGBA四个通道。");

            ShaderReferenceUtil.DrawTitle("混合");
            ShaderReferenceUtil.DrawOneContent("说明", "颜色混合，源颜色与目标颜色以给定的公式进行混合出最终的新颜色。\n源颜色：当前Shader计算出的颜色。\n目标颜色：已经存在颜色缓存中的颜色。默认值为Blend Off,即表示关闭混合。\n在混合时可以针对某个RT做混合，比如Blend 3 One One,就是对RenderTarget3做混合。");
            ShaderReferenceUtil.DrawOneContent("Blend SrcFactor DstFactor", "SrcFactor为源颜色，DstFactor为目标颜色，将两者按Op中指定的操作进行混合。");
            ShaderReferenceUtil.DrawOneContent("Blend SrcFactor DstFactor, SrcFactorA DstFactorA", "对RGB和A通道分别做混合操作。");
            ShaderReferenceUtil.DrawOneContent("BlendOp Op", "混合时的操作运算符，默认值为Add（加法操作）。");
            ShaderReferenceUtil.DrawOneContent("BlendOp OpColor, OpAlpha", "对RGB和A通道分别指定混合运算符。");
            ShaderReferenceUtil.DrawOneContent("AlphaToMask On | Off", "当值为On时，在使用MSAA时，会根据像素结果将alpha值进行修改多重采样覆盖率，对植被和其他经过alpha测试的着色器非常有用。");
            ShaderReferenceUtil.DrawOneContent("Blend factors", "混合因子"
            + "\nOne：源或目标的完整值"
            + "\nZero：0"
            + "\nSrcColor：源的颜色值"
            + "\nSrcAlpha：源的Alpha值"
            + "\nDstColor：目标的颜色值"
            + "\nDstAlpha：目标的Alpha值"
            + "\nOneMinusSrcColor：1-源颜色得到的值"
            + "\nOneMinusSrcAlpha：1-源Alpha得到的值"
            + "\nOneMinusDstColor：1-目标颜色得到的值"
            + "\nOneMinusDstAlpha：1-目标Alpha得到的值");
            ShaderReferenceUtil.DrawOneContent("Blend Types", "常用的几种混合类型"
            + "\nBlend SrcAlpha OneMinusSrcAlpha// Traditional transparency"
            + "\nBlend One OneMinusSrcAlpha// Premultiplied transparency"
            + "\nBlend One One"
            + "\nBlend OneMinusDstColor One // Soft Additive"
            + "\nBlend DstColor Zero // Multiplicative"
            + "\nBlend DstColor SrcColor // 2x Multiplicative");
            ShaderReferenceUtil.DrawOneContent("Blend operations", "混合操作的具体运算符"
            + "\nAdd：源+目标"
            + "\nSub：源-目标"
            + "\nRevSub：目标-源"
            + "\nMin：源与目标中最小值"
            + "\nMax：源与目标中最大值"
            + "\n以下仅用于DX11.1中"
            + "\nLogicalClear"
            + "\nLogicalSet"
            + "\nLogicalCopy"
            + "\nLogicalCopyInverted"
            + "\nLogicalNoop"
            + "\nLogicalInvert"
            + "\nLogicalAnd"
            + "\nLogicalNand"
            + "\nLogicalOr"
            + "\nLogicalNor"
            + "\nLogicalXor"
            + "\nLogicalEquiv"
            + "\nLogicalAndReverse"
            + "\nLogicalAndInverted"
            + "\nLogicalOrReverse"
            + "\nLogicalOrInverted");
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif