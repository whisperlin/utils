using System;

namespace UnityEngine.PostProcessing
{
    [Serializable]
    public class DepthOfFieldModel : PostProcessingModel
    {
        

        [Serializable]
        public struct Settings
        {
            [Min(0.1f), Tooltip("Distance to the point of focus.")]
            public float focusDistance;

            [Range(0.05f, 32f), Tooltip("Ratio of aperture (known as f-stop or f-number). The smaller the value is, the shallower the depth of field is.")]
            public float aperture;

            [Range(1f, 300f), Tooltip("Distance between the lens and the film. The larger the value is, the shallower the depth of field is.")]
            public float focalLength;
 

 

            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {
                        focusDistance = 10f,
                        aperture = 5.6f,
                        focalLength = 50f,
 
 
                    };
                }
            }
        }

        [SerializeField]
        Settings m_Settings = Settings.defaultSettings;
        public Settings settings
        {
            get { return m_Settings; }
            set { m_Settings = value; }
        }

        public override void Reset()
        {
            m_Settings = Settings.defaultSettings;
        }
    }
}
