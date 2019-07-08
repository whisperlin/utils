using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityEditor
{
    internal class CharBRDFShaderEditor : ShaderGUI
    {
        public enum BlendMode
        {
            Opaque,
            Transparent,
        }
        private static class Styles
        {
            public static GUIStyle optionsButton = "PaneOptions";
            public static GUIContent uvSetLabel = new GUIContent("UV Set");
            public static GUIContent[] uvSetOptions = new GUIContent[] { new GUIContent("UV channel 0"), new GUIContent("UV channel 1") };

            public static string emptyTootip = "";

            public static string whiteSpaceString = " ";
            public static string primaryMapsText = "Main Maps";
            public static string lightContrlText = "Lighting";
            public static string materializeText = "Materialize";
            public static string renderingMode = "渲染方式";
            public static string lightingMode = "光源方式";
            public static readonly string[] blendNames = { "不透明", "透明" };
        }

        MaterialProperty blendMode = null;
        MaterialProperty mainTexture = null;
        MaterialProperty environmentReflectTexture = null;
        MaterialProperty normalTexture = null;
        MaterialProperty controlTexture = null;
        MaterialProperty other0Texture = null;

        MaterialProperty ccLAPE = null;
        MaterialProperty metallicColor = null;
        MaterialProperty ccNMRB = null;
        MaterialProperty sssScatterColor0 = null;
        MaterialProperty ccWSAX = null;

        MaterialProperty devMode = null;
        MaterialProperty dev_NormalCtrlTexture = null;
        MaterialProperty dev_MetallicCtrlTexture = null;
        MaterialProperty dev_3SCtrlTexture = null;
        MaterialProperty dev_srCtrlTexture = null;

        MaterialEditor m_MaterialEditor;

        bool m_FirstTimeApply = true;

        public void FindProperties(MaterialProperty[] props)
        {
            blendMode = FindProperty("_Mode", props);

            mainTexture = FindProperty("sam_diffuse", props);
            environmentReflectTexture = FindProperty("sam_environment_reflect", props);
            normalTexture = FindProperty("sam_normal", props);
            controlTexture = FindProperty("sam_control", props);
            other0Texture = FindProperty("sam_other0", props, false);

            ccLAPE = FindProperty("_CC_LAPE", props);
            metallicColor = FindProperty("metallic_color", props);
            ccNMRB = FindProperty("_CC_NMRB", props);
            sssScatterColor0 = FindProperty("sss_scatter_color0", props);
            ccWSAX = FindProperty("_CC_WSAX", props);

            devMode = FindProperty("_DevMode", props, false);
            dev_NormalCtrlTexture = FindProperty("sam_normalCtrl", props, false);
            dev_MetallicCtrlTexture = FindProperty("sam_metallicCtrl", props, false);
            dev_3SCtrlTexture = FindProperty("sam_3sCtrl", props, false);
            dev_srCtrlTexture = FindProperty("sam_srCtrl", props, false);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
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
            materialEditor.RenderQueueField();
        }

        public void ShaderPropertiesGUI(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            {
                DoModesPopup(material);
                // Primary properties
                GUILayout.Label(Styles.primaryMapsText, EditorStyles.boldLabel);
                DoTexturesArea(material);

                EditorGUILayout.Space();
                GUILayout.Label(Styles.lightContrlText, EditorStyles.boldLabel);
                DoLightArea(material);

                EditorGUILayout.Space();
                GUILayout.Label(Styles.materializeText, EditorStyles.boldLabel);
                DoMaterializeArea(material);
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendMode.targets)
                    MaterialChanged((Material)obj);
            }
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            base.AssignNewShaderToMaterial(material, oldShader, newShader);
            material.SetFloat("_Mode", 0.0f);
            MaterialChanged(material);
        }

        void DoModesPopup(Material material)
        {
            EditorGUI.showMixedValue = blendMode.hasMixedValue;
            var bldMode = (BlendMode)blendMode.floatValue;

            EditorGUI.BeginChangeCheck();
            bldMode = (BlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)bldMode, Styles.blendNames);
            bool isDev = EditorGUILayout.Toggle("开发模式", devMode != null && devMode.floatValue != 0);
            if (EditorGUI.EndChangeCheck())
            {
                blendMode.floatValue = (float)bldMode;
                if (devMode != null) devMode.floatValue = isDev ? 1.0f : 0.0f;
                MaterialChanged(material);
                PropertyChanged(material);
            }

            EditorGUI.showMixedValue = false;
        }
        void DoTexturesArea(Material material)
        {
            m_MaterialEditor.TextureProperty(mainTexture, "颜色贴图", false);
            m_MaterialEditor.TextureProperty(normalTexture, "法线贴图", false);
            m_MaterialEditor.TextureProperty(environmentReflectTexture, "反射贴图", false);
            if (other0Texture != null)
                m_MaterialEditor.TextureProperty(other0Texture, "other0", false);
            if (devMode == null || devMode.floatValue == 0)
            {
                m_MaterialEditor.TextureProperty(controlTexture, "控制贴图", false);
            }
            else
            {
                m_MaterialEditor.TextureProperty(dev_NormalCtrlTexture, "粗糙控制贴图", false);
                m_MaterialEditor.TextureProperty(dev_MetallicCtrlTexture, "金属控制贴图", false);
                m_MaterialEditor.TextureProperty(dev_3SCtrlTexture, "3S皮肤贴图", false);
                m_MaterialEditor.TextureProperty(dev_srCtrlTexture, "高光反射控制贴图", false);
            }
        }

        void DoLightArea(Material material)
        {
            EditorGUI.BeginChangeCheck();
            float env_exposure = EditorGUILayout.Slider("环境曝光度", ccLAPE.vectorValue.w, 0.0f, 3.0f);
            float character_light_factor = EditorGUILayout.Slider("亮度系数", ccLAPE.vectorValue.x, 0.0f, 1.0f);
            float change_color_bright_add = EditorGUILayout.Slider("亮度叠加", ccLAPE.vectorValue.y, 0.0f, 1.0f);
            float point_light_scale = EditorGUILayout.Slider("点光强度", ccLAPE.vectorValue.z, -1.0f, 1.0f);
            if (EditorGUI.EndChangeCheck())
            {
                ccLAPE.vectorValue = new Vector4(character_light_factor, change_color_bright_add, point_light_scale, env_exposure);
                PropertyChanged(material);
            }
            m_MaterialEditor.ShaderProperty(metallicColor, "金属颜色");
            m_MaterialEditor.ShaderProperty(sssScatterColor0, "皮肤消散色");
            EditorGUI.BeginChangeCheck();
            float sss_warp0 = EditorGUILayout.Slider("皮肤包裹", ccWSAX.vectorValue.x, 0.0f, 1.0f);
            float sss_scatter0 = EditorGUILayout.Slider("皮肤消散", ccWSAX.vectorValue.y, 0.0f, 1.0f);
            float alphaRef = ccWSAX.vectorValue.z;
            if ((BlendMode)blendMode.floatValue == BlendMode.Transparent)
                alphaRef = EditorGUILayout.Slider("透明裁剪", alphaRef, 0.0f, 1.0f);
            if (EditorGUI.EndChangeCheck())
            {
                ccWSAX.vectorValue = new Vector4(sss_warp0, sss_scatter0, alphaRef, 0);
                PropertyChanged(material);
            }
        }

        void DoMaterializeArea(Material material)
        {
            EditorGUI.BeginChangeCheck();
            float normalmap_scale = EditorGUILayout.Slider("法线强度", ccNMRB.vectorValue.x, 0.0f, 5.0f);
            float metallic_offset = EditorGUILayout.Slider("金属度调节", ccNMRB.vectorValue.y, -1.0f, 1.0f);
            float roughness_offset = EditorGUILayout.Slider("粗糙度调节", ccNMRB.vectorValue.z, -1.0f, 1.0f);
            bool bloom_switch = EditorGUILayout.Toggle("泛光开启", ccNMRB.vectorValue.w != 0);
            if (EditorGUI.EndChangeCheck())
            {
                ccNMRB.vectorValue = new Vector4(normalmap_scale, metallic_offset, roughness_offset, bloom_switch ? 1 : 0);
                PropertyChanged(material);
            }
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
            SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
            SetKeyword(material, "DEV_MODE_ON", material.GetFloat("_DevMode") != 0);
            /*Shader.SetGlobalVectorArray("PointLightAttrs", new Vector4[20] {
                new Vector4(0, 0, 0, 0),
                new Vector4(1, 0.929412f, 0.882353f, 1),
                new Vector4(10f, 0, 0, 0),
                new Vector4(7053.23f, 65.644f, 1579.67f, 190f),
                new Vector4(2, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(135.103f, 0, 0, 0.001f),
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(135.103f, 0, 0, 0.001f),
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(135.103f, 0, 0, 0.001f),
                new Vector4(0, 0, 0, 0),
            });
            Shader.SetGlobalVector("weather_intensity", new Vector4(0.74f, 0.8f, 0, 0));*/
        }
        public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest + 2;
                    material.DisableKeyword("_TRANSPARENT_MODE");
                    break;
                case BlendMode.Transparent:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest + 4;
                    material.EnableKeyword("_TRANSPARENT_MODE");
                    break;
            }
        }
        static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }
        static Matrix4x4 FA2M4x4(float[] data)
        {
            Matrix4x4 ret = new Matrix4x4();
            ret[0] = 1;
            ret[5] = 1;
            ret[10] = 1;
            ret[15] = 1;
            for (int i=0; i<16; ++i)
            {
                ret[i] = data[i];
            }
            return ret;
        }
    }
}
