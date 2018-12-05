using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderForge
{
    public class SFN_Enviment : SF_Node
    {
        public SFN_Enviment()
        {

        }

        protected virtual string GetNodeName()
        {
            return "Envirment node";
        }


        public override int GetEvaluatedComponentCount()
        {
            return 3;
        }

        static string GetDefaultString(string val, string defVal)
        {
            return string.IsNullOrEmpty(val) ? defVal : val;
        }


        public override void Initialize()
        {
            base.Initialize(GetNodeName());

            base.showColor = true;
            base.shaderGenMode = ShaderGenerationMode.CustomFunction;
            UseLowerReadonlyValues(true);

            connectors = new SF_NodeConnector[]{
                SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)};
        }

        public override string Evaluate(OutChannel channel = OutChannel.All)
        {
            return "GetEnvirmentColor(normalDirection)";
        }
    }

    public class SFN_GroundEnvirment : SFN_Enviment
    {
        protected override string GetNodeName()
        {
            return "Ground Envirment node";
        }

        public override string Evaluate(OutChannel channel = OutChannel.All)
        {
            return "GroundEnvirment";
        }
    }
}