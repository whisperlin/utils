using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderForge
{
    [System.Serializable]
    public class SFN_RichMainST : SF_Node
    {
        public SFN_RichMainST()
        {

        }

        public override void Initialize()
        {
            base.Initialize("Rich _Main_ST");

            //base.property = ScriptableObject.CreateInstance<SFP_MainSTProperty>().Initialize(this);
            
            connectors = new SF_NodeConnector[]{
                SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv4,false),
                SF_NodeConnector.Create(this,"xy","xy",ConType.cOutput,   ValueType.VTv2).Outputting(OutChannel.RG),
                SF_NodeConnector.Create(this,"z","z",ConType.cOutput,ValueType.VTv2).Outputting(OutChannel.B),
                SF_NodeConnector.Create(this,"w","w",ConType.cOutput,ValueType.VTv2).Outputting(OutChannel.A),
            };
        }

        public override int GetEvaluatedComponentCount()
        {
            return 4;
        }

        public override string Evaluate(OutChannel channel = OutChannel.All)
        {
            string evalString = @"UNITY_ACCESS_INSTANCED_PROP(INSTANC_BUF, _MainTex_ST)";

            return evalString;
        }
        
    }
}