using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderForge
{
    [System.Serializable]
    public class SFN_RichSplit : SF_Node
    {
        public SFN_RichSplit()
        {

        }

        public override void Initialize()
        {
            base.Initialize("Rich Split");

            connectors = new SF_NodeConnector[]{
                SF_NodeConnector.Create(this,"OUT","UV",ConType.cOutput,ValueType.VTv2,false),
                SF_NodeConnector.Create(this,"HorCount", "HorCount",ConType.cInput,ValueType.VTv1,false),
                                SF_NodeConnector.Create(this,"VerCount", "VerCount",ConType.cInput,ValueType.VTv1,false),
                                                SF_NodeConnector.Create(this,"UV", "UV",ConType.cInput,ValueType.VTv2,false),
                                                                SF_NodeConnector.Create(this,"Index", "Index",ConType.cInput,ValueType.VTv1,false),
            };
        }

        public override int GetEvaluatedComponentCount()
        {
            return 2;
        }


        public override string Evaluate(OutChannel channel = OutChannel.All)
        {
            string evalString = string.Format(@"TexUVSplit({0}, {1}, {2}, {3})", 
                GetConnectorByStringID("HorCount").TryEvaluate(),
                GetConnectorByStringID("VerCount").TryEvaluate(),
                GetConnectorByStringID("UV").TryEvaluate(),
                GetConnectorByStringID("Index").TryEvaluate());

            return evalString;
        }
    }
}