using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityEditor
{
    internal class BumpedSpecularShaderEditor : ShaderGUI
    {
        public static string CtlTexture = "RGBA控制贴图";
        MaterialProperty devMode = null;
        MaterialProperty dev_RedCtrlTexture = null;
        MaterialProperty dev_GreenCtrlTexture = null;
        MaterialProperty dev_BlueCtrlTexture = null;
        MaterialProperty dev_AlphaCtrlTexture = null;
        MaterialEditor m_MaterialEditor;

        bool m_FirstTimeApply = true;

        public void FindProperties(MaterialProperty[] props)
        {
            devMode = FindProperty("_DevMode", props, false);
            dev_RedCtrlTexture = FindProperty("_RedCtrl", props, false);
            dev_GreenCtrlTexture = FindProperty("_GreenCtrl", props, false);
            dev_BlueCtrlTexture = FindProperty("_BlueCtrl", props, false);
            dev_AlphaCtrlTexture = FindProperty("_AlphaCtrl", props, false);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            base.OnGUI(materialEditor, props);
            FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
            m_MaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;

            // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
            // material to a standard shader.
            // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
            if (m_FirstTimeApply)
            {
                m_FirstTimeApply = false;
            }

            ShaderPropertiesGUI(material);
        }

        public void ShaderPropertiesGUI(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;
            DoModesPopup(material);
            GUILayout.Label(CtlTexture, EditorStyles.boldLabel);
            DoTexturesArea(material);
        }

        void DoModesPopup(Material material)
        {
            EditorGUI.BeginChangeCheck();
            bool isDev = EditorGUILayout.Toggle("开发模式", devMode != null && devMode.floatValue != 0);
            if (EditorGUI.EndChangeCheck())
            {
                if (devMode != null) devMode.floatValue = isDev ? 1.0f : 0.0f;
                MaterialChanged(material);
                PropertyChanged(material);
            }
        }
        void DoTexturesArea(Material material)
        {
            if (devMode == null || devMode.floatValue == 0)
            {
                dev_RedCtrlTexture.textureValue = null;
                dev_GreenCtrlTexture.textureValue = null;
                dev_BlueCtrlTexture.textureValue = null;
                dev_AlphaCtrlTexture.textureValue = null;
            }
            else
            {
                m_MaterialEditor.TextureProperty(dev_RedCtrlTexture, "红色通道控制贴图", false);
                m_MaterialEditor.TextureProperty(dev_GreenCtrlTexture, "绿色通道控制贴图", false);
                m_MaterialEditor.TextureProperty(dev_BlueCtrlTexture, "蓝色通道控制贴图", false);
                m_MaterialEditor.TextureProperty(dev_AlphaCtrlTexture, "透明通道控制贴图", false);
            }
            PropertyChanged(material);
        }

        void PropertyChanged(Material material)
        {
            EditorUtility.SetDirty(material);
            MarkSceneDirty();
        }

        static void MarkSceneDirty()
        {
            var scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
        }
        static void MaterialChanged(Material material)
        {
            SetKeyword(material, "DEV_MODE_ON", material.GetFloat("_DevMode") != 0);
        }

        static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }
    }
}
