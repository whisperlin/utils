using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderForge
{
    [System.Serializable]
    public class SFN_Specular : SFN_Vector1
    {
        public SFN_Specular()
        {

        }

        public override void Initialize()
        {
            node_height = 24;
            //node_width = (int)(NODE_WIDTH*1.25f);
            base.Initialize("Specular");
            lowerRect.y -= 8;
            lowerRect.height = 28;
            base.showColor = false;
            base.UseLowerPropertyBox(true);
            base.texture.uniform = true;
            base.texture.CompCount = 1;
            base.shaderGenMode = ShaderGenerationMode.OffUniform;

            connectors = new SF_NodeConnector[]{
                SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
            };

        }

        public override string Evaluate(OutChannel channel = OutChannel.All)
        {
            string str = texture.dataUniform[0].ToString("0.0###########"); // At least one decimal

//             if (texture.dataUniform[0] < 0f)
//                 return "(" + str + ")";
            return string.Format("GetSpec(DirectionLightDir0.xyz, viewDirection, normalDirection, {0}) * DirectionLightColor0 * DirectionLightIntensity0", str);
        }
    }
}
