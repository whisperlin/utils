/**
 * @file         ShaderReferenceMath.cs
 * @author       Hongwei Li(taecg@qq.com)
 * @created      2018-12-08
 * @updated      2018-12-08
 *
 * @brief        GPU相关
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace taecg.tools.shaderReference
{
    public class ShaderReferenceGPU : EditorWindow
    {
        #region 数据成员
        private Vector2 scrollPos;
        #endregion

        public void DrawMainGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            ShaderReferenceUtil.DrawTitle("部件");
            ShaderReferenceUtil.DrawOneContent("GPU", "Graphic Processing Unit,简称GPU，中文翻译为图形处理器。");
            ShaderReferenceUtil.DrawOneContent("BIOS", "BIOS是Basic Input Output System的简称，也就是基本输入输出系统。\n显卡BIOS主要用于存放显示芯片与驱动程序之间的控制程序，另外还保存显卡的型号、规格、生产厂家以及出厂时间等信息。");
            ShaderReferenceUtil.DrawOneContent("PCB", "Printed Circuit Board,显卡PCB就是印制电路板，除了用于固定各种小零件外，PCB的主要功能是提供其上各项零件的相互电气连接。");
            ShaderReferenceUtil.DrawOneContent("晶振", "Crystal，石英振荡器的简称，是时钟电路中最重要的部分。\n显卡晶振为27MHz，其作用是向显卡各部分提供基准频率。晶振就像个标尽尺，工作频率不稳定会造成相关设备工作频率不稳定。");
            ShaderReferenceUtil.DrawOneContent("电容", "在显卡上是两个直立的零件，电容对显卡主要起滤波和稳定电流的作用，只有在保证电流稳定的情况下，显卡才能正常的工作。");

            ShaderReferenceUtil.DrawTitle("性能参数");
            ShaderReferenceUtil.DrawOneContent("显卡架构", "就是指显示芯片各种处理单元的组成和工作模式，在参数相同的情况下，架构越先进，效率就越高，性能也就越强。\n而同架构的显卡 最重要的指标由处理器数量、核芯频率、显存、位宽来决定。");
            ShaderReferenceUtil.DrawOneContent("核心频率", "显卡的核心频率是指显示核心的工作频率。");
            ShaderReferenceUtil.DrawOneContent("显存", "显存也称为帧缓存，它的作用是用来存储显示芯片处理过的或者即将提取的渲染数据。我们在显示屏上看到的画面都是由一个个的像素点构成的，而每个像素点都是以4~64bit的数据来控制亮度和色彩的，这些数据必须通过显存来保存，并交由显示芯片和CPU调配，才能把运算结果转化为图像输出到显示器上。");
            ShaderReferenceUtil.DrawOneContent("显存位宽", "显存位宽是显存在一个时钟周期内所能传送数据的位数，位数越大则瞬间能传输的数据量就越大。\n显存带宽=显存频率X显存位宽÷8\n举例：500MHzx256bit÷8=16GB/s");
            ShaderReferenceUtil.DrawOneContent("显存速度", "显存速度一般以ns（纳秒）为单位。常用的显存速度有7ns、5ns、1.1ns等，越小速度越快，性能越好。");
            ShaderReferenceUtil.DrawOneContent("显存频率", "显存频率在一定程度上反应着显存的速度，以MHz（兆赫兹）为单位。显存频率与显存时钟周期是相关的，二者成倒数关系：\n显存频率=1÷显存时钟周期");

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif