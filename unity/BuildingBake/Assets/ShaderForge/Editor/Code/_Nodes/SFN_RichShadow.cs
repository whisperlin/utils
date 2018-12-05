using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderForge
{
    [System.Serializable]
    public class SFN_RichShadow : SF_Node
    {
        public SFN_RichShadow()
        {

        }

        public override void Initialize()
        {
            base.Initialize("Rich shadow");

            connectors = new SF_NodeConnector[]{
                SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv4,false),
                SF_NodeConnector.Create(this,"IN","",ConType.cInput,ValueType.VTv3,false),
            };
        }

        public override string Evaluate(OutChannel channel = OutChannel.All)
        {
            string evalString = string.Format(@"FINAL_SHADOW_COLOR_SINGLE({0}, i, normalDirection)", GetConnectorByStringID("IN").TryEvaluate());

            return evalString;
        }
    }
}