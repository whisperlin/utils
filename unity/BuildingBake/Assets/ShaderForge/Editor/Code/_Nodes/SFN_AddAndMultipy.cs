using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_AddAndMultipy : SF_Node {

		public SFN_AddAndMultipy() {

		}

		public override void Initialize() {
			base.Initialize("Add and multiply");

			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			UseLowerReadonlyValues( true );

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false),
				SF_NodeConnector.Create(this,"A","A",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"B","B",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"C","C",ConType.cInput,ValueType.VTvPending,false).SetRequired(false),
				SF_NodeConnector.Create(this,"D","D",ConType.cInput,ValueType.VTvPending,false).SetRequired(false),
				SF_NodeConnector.Create(this,"E","E",ConType.cInput,ValueType.VTvPending,false).SetRequired(false)
			};


			//SetExtensionConnectorChain("B", "C", "D", "E");

			//base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2], connectors[3], connectors[4], connectors[5] );

		}

		public override void GetModularShaderFixes( out string prefix, out string infix, out string suffix ) {
			prefix = "";
			infix  = " + ";
			suffix = "";
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {


			string evalStr = "";

			evalStr += GetConnectorByStringID( "A" ).TryEvaluate() + "*" + GetConnectorByStringID( "B" ).TryEvaluate();

			ChainAppendIfConnected(ref evalStr, "*", "C", "D", "E");

			return "(" + evalStr + ")";
		}


		public override float EvalCPU( int c ) {

			float result = GetInputData( "A", c ) + GetInputData( "B", c );

			if(GetInputIsConnected("C")){
				result += GetInputData( "C", c );
			}
			if(GetInputIsConnected("D")){
				result += GetInputData( "D", c );
			}
			if(GetInputIsConnected("E")){
				result += GetInputData( "E", c );
			}

			return result;
		}

	}
}