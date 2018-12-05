using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderForge
{
    public class SFN_PBRLod2 : SF_Node
    {
		public SFN_PBRLod2()
        {

        }

        public override void Initialize()
        {
            base.Initialize("PBR  Lod no spe");

            base.showColor = true;
            base.shaderGenMode = ShaderGenerationMode.CustomFunction;
            UseLowerReadonlyValues(true);
            /*
            float4 _Metallic_var,
            float4 _MainTex_var, 
            float4 _DiffCubemap, 
            float4 _SpecCubemap
            */
            connectors = new SF_NodeConnector[]{
                SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false),
                SF_NodeConnector.Create(this,"MainTex","MainTex",ConType.cInput,ValueType.VTv3,false).SetRequired(false),
                SF_NodeConnector.Create(this,"Metallic","Metallic",ConType.cInput,ValueType.VTv3,false).SetRequired(false),
                SF_NodeConnector.Create(this,"Metallic_alpha","G",ConType.cInput,ValueType.VTv1,false).SetRequired(false),
                SF_NodeConnector.Create(this,"Metallic_red","mr",ConType.cInput,ValueType.VTv1,false).SetRequired(false),
                SF_NodeConnector.Create(this,"DiffCubemap","DiffCubemap",ConType.cInput,ValueType.VTv3,false).SetRequired(false),
            };
        }


        public override int GetEvaluatedComponentCount()
        {
            return 3;
        }

        static string GetDefaultString(string val, string defVal)
        {
            return string.IsNullOrEmpty(val) ? defVal : val;
        }

        public override string Evaluate(OutChannel channel = OutChannel.All)
        {


            //             string evalStr = "";
            // 
            //             evalStr += GetConnectorByStringID("A").TryEvaluate() + "*" + GetConnectorByStringID("B").TryEvaluate();
            // 
            //             ChainAppendIfConnected(ref evalStr, "*", "C", "D", "E");
            // 
            //             return "(" + evalStr + ")";

            string MetallicVal = GetConnectorByStringID("Metallic").TryEvaluate();
            string Metallic_a = GetConnectorByStringID("Metallic_alpha").TryEvaluate();
            string Metallic_r = GetConnectorByStringID("Metallic_red").TryEvaluate();
            string MainTex = GetConnectorByStringID("MainTex").TryEvaluate();
            string Diffcubemap = GetConnectorByStringID("DiffCubemap").TryEvaluate();

			return string.Format(@"PBRLow2(normalDirection, 
                    viewDirection,{0},{1},{2},{3},{4})", 
				GetDefaultString(MetallicVal, "float3(0,0,0)"),
				GetDefaultString(Metallic_a, "0"),
				GetDefaultString(Metallic_r, "0"),
				GetDefaultString(MainTex, "float3(0,0,0)"),
				GetDefaultString(Diffcubemap, "float3(0,0,0)"));
        }
    }
}