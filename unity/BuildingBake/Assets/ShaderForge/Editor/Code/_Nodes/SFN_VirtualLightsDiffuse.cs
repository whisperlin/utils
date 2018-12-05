using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderForge
{
    [System.Serializable]
    public class SFN_VirtualLightsDiffuse : SF_Node
    {
        public SFN_VirtualLightsDiffuse()
        {

        }

        public override void Initialize()
        {
            base.Initialize("virtual lights diffuse");

            connectors = new SF_NodeConnector[]{
                SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false),
            };
        }

        public override string Evaluate(OutChannel channel = OutChannel.All)
        {
            string evalString = @"(max(0.0, dot(normalDirection, DirectionLightDir0)) * DirectionLightColor0.xyz * DirectionLightIntensity0
                                  + max(0.0, dot(normalDirection, DirectionLightDir1)) * DirectionLightColor1.xyz * DirectionLightIntensity1
                                  + max(0.0, dot(normalDirection, DirectionLightDir2)) * DirectionLightColor2.xyz * DirectionLightIntensity2)";

            return evalString;
        }
    }
}
