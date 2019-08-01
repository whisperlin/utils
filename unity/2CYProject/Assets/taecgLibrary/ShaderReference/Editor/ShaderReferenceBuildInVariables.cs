/**
 * @file         ShaderReferenceBuildInVariables.cs
 * @author       Hongwei Li(taecg@qq.com)
 * @created      2019-02-26
 * @updated      2019-02-26
 *
 * @brief        内置变量相关
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace taecg.tools.shaderReference
{
    public class ShaderReferenceBuildInVariables : EditorWindow
    {
        #region 数据成员
        private Vector2 scrollPos;
        #endregion

        public void DrawMainGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            ShaderReferenceUtil.DrawTitle("Transformations");
            ShaderReferenceUtil.DrawOneContent("UNITY_MATRIX_MVP", "模型空间>>投影空间");
            ShaderReferenceUtil.DrawOneContent("UNITY_MATRIX_MV", "模型空间>>观察空间");
            //ShaderReferenceUtil.DrawOneContent("UNITY_MATRIX_V", "");
            //ShaderReferenceUtil.DrawOneContent("UNITY_MATRIX_P", "");
            //ShaderReferenceUtil.DrawOneContent("UNITY_MATRIX_VP", "");
            //ShaderReferenceUtil.DrawOneContent("UNITY_MATRIX_T_MV", "");
            //ShaderReferenceUtil.DrawOneContent("UNITY_MATRIX_IT_MV", "");
            //ShaderReferenceUtil.DrawOneContent("unity_ObjectToWorld", "");
            //ShaderReferenceUtil.DrawOneContent("unity_WorldToObject", "");

            ShaderReferenceUtil.DrawTitle("Camera and Screen"); 
            ShaderReferenceUtil.DrawOneContent("_WorldSpaceCameraPos", "主相机的世界坐标，类型：float3");
            //ShaderReferenceUtil.DrawOneContent("_ProjectionParams", "");
            ShaderReferenceUtil.DrawOneContent("_ScreenParams", "屏幕的相关参数，单位为像素。\nx表示屏幕的宽度\ny表示屏幕的高度\nz表示1+1/屏幕宽度\nw表示1+1/屏幕高度");
            //ShaderReferenceUtil.DrawOneContent("_ZBufferParams", "");
            //ShaderReferenceUtil.DrawOneContent("unity_OrthoParams", "");
            //ShaderReferenceUtil.DrawOneContent("unity_CameraProjection", "");
            //ShaderReferenceUtil.DrawOneContent("unity_CameraInvProjection", "");
            //ShaderReferenceUtil.DrawOneContent("unity_CameraWorldClipPlanes[6]", "");

            ShaderReferenceUtil.DrawTitle("Time");
            ShaderReferenceUtil.DrawOneContent("_Time", "时间，主要用于在Shader做动画,类型：float4\nx = t * 20\ny = t\nz = t *2\nw = t*3");
            ShaderReferenceUtil.DrawOneContent("_SinTime", "t是时间的正弦值，返回值(-1~1): \nx = t/8\ny = t/4\nz = t/2\nw = t");
            ShaderReferenceUtil.DrawOneContent("_CosTime", "t是时间的余弦值，返回值(-1~1):\nx = t/8\ny = t/4\nz = t/2\nw = t");
            ShaderReferenceUtil.DrawOneContent("unity_DeltaTime", "dt是时间增量,smoothDt是平滑时间\nx = dt\ny = 1/dt\nz = smoothDt\nz = 1/smoothDt");

            //ShaderReferenceUtil.DrawTitle("Lighting(Forward)");
            //ShaderReferenceUtil.DrawOneContent("_LightColor0", "");
            //ShaderReferenceUtil.DrawOneContent("_WorldSpaceLightPos0", "");
            //ShaderReferenceUtil.DrawOneContent("_LightMatrix0", "");
            //ShaderReferenceUtil.DrawOneContent("unity_4LightPosX0,unity_4LightPosY0,unity_4LightPosZ0", "");
            //ShaderReferenceUtil.DrawOneContent("unity_4LightAtten0", "");
            //ShaderReferenceUtil.DrawOneContent("unity_LightColor", "");
            //ShaderReferenceUtil.DrawOneContent("unity_WorldToShadow", ""); 

            //ShaderReferenceUtil.DrawTitle("Fog and Ambient");
            //ShaderReferenceUtil.DrawOneContent("unity_AmbientSky", "");
            //ShaderReferenceUtil.DrawOneContent("unity_AmbientEquator", "");
            //ShaderReferenceUtil.DrawOneContent("unity_AmbientGround", "");
            //ShaderReferenceUtil.DrawOneContent("UNITY_LIGHTMODEL_AMBIENT", "");
            //ShaderReferenceUtil.DrawOneContent("unity_FogColor", "");
            //ShaderReferenceUtil.DrawOneContent("unity_FogParams", "");

            //ShaderReferenceUtil.DrawTitle("Various");
            //ShaderReferenceUtil.DrawOneContent("unity_LODFade", "");
            //ShaderReferenceUtil.DrawOneContent("_TextureSampleAdd", "");
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif