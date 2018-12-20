 

using UnityEngine;
using System.Collections.Generic;

namespace ShaderForge {
	public class  SF_NodeConnectorStaticOutPut:SF_NodeConnector
	{
		public static SF_NodeConnectorStaticOutPut CreateC( SF_Node node, string strID, string label, ConType conType, ValueType valueType, bool outerLabel = false, string unconnectedEvaluationValue = null ) {
			return ScriptableObject.CreateInstance< SF_NodeConnectorStaticOutPut>().InitializeC(node, strID, label, conType,valueType, outerLabel, unconnectedEvaluationValue);
		}
		 
		public  SF_NodeConnectorStaticOutPut InitializeC( SF_Node node, string strID, string label, ConType conType, ValueType valueType, bool outerLabel = false, string unconnectedEvaluationValue = null ) {

			this.node = node;
			this.strID = strID;
			this.label = label;
			this.conType = conType;

			if(conType == ConType.cInput){
				conLine = ScriptableObject.CreateInstance<SF_NodeConnectionLine>().Initialize(node.editor, this);
			}

			this.valueType = this.valueTypeDefault = valueType;
			this.outerLabel = outerLabel;
			this.unconnectedEvaluationValue = unconnectedEvaluationValue;
			outputCons = new List<SF_NodeConnector>();
			return this;
		}

		public override void SetValueType(ValueType vt){
			if(conType == ConType.cOutput && this.valueType != vt){
				this.valueType = valueTypeDefault;
				foreach(SF_NodeConnector con in this.outputCons){
					if(con.valueTypeDefault == ValueType.VTvPending){
						con.valueType = this.valueType;
						con.node.OnUpdateNode();
					}
				}
			}
		}
		public override void SetValueTypeAndDefault( ValueType vt ) {
			valueType = valueTypeDefault;
		}
	}
	[System.Serializable]
	public class SFN_ColorChannelR : SF_Node_Arithmetic {

		public SFN_ColorChannelR() {
		}

		public override void Initialize() {
			base.Initialize( "channel R" );
			base.showColor = true;
			UseLowerReadonlyValues( true );
			connectors = new SF_NodeConnector[]{
				SF_NodeConnectorStaticOutPut.CreateC( this, "OUT", "", ConType.cOutput, ValueType.VTv1, false ).Outputting(OutChannel.R),
				SF_NodeConnector.Create( this, "IN", "", ConType.cInput, ValueType.VTv1v4, false ).SetRequired( true )};
			//connectors [0].lockDefaultType = true;
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1]);
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return  GetConnectorByStringID( "IN" ).TryEvaluate() ;
		}

		public override float EvalCPU( int c ) {
			return Mathf.Abs( GetInputData( "IN", 0 ) );
		}
		 
	}


	[System.Serializable]
	public class SFN_ColorChannelG : SF_Node_Arithmetic {

		public SFN_ColorChannelG() {
		}

		public override void Initialize() {
			base.Initialize( "channel G" );
			 

			base.showColor = true;
			UseLowerReadonlyValues( true );
			connectors = new SF_NodeConnector[]{
				SF_NodeConnectorStaticOutPut.CreateC( this, "OUT", "", ConType.cOutput, ValueType.VTv1, false ).Outputting(OutChannel.G),
				SF_NodeConnector.Create( this, "IN", "", ConType.cInput, ValueType.VTv1v4, false ).SetRequired( true )};
			//connectors [0].lockDefaultType = true;
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1]);

			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return  GetConnectorByStringID( "IN" ).TryEvaluate()  ;
		}

		public override float EvalCPU( int c ) {
			return Mathf.Abs( GetInputData( "IN", 1 ) );
		}
	}


	[System.Serializable]
	public class SFN_ColorChannelB : SF_Node_Arithmetic {

		public SFN_ColorChannelB() {
		}

		public override void Initialize() {
			base.Initialize( "channel B" );


			base.showColor = true;
			UseLowerReadonlyValues( true );
			connectors = new SF_NodeConnector[]{
				SF_NodeConnectorStaticOutPut.CreateC( this, "OUT", "", ConType.cOutput, ValueType.VTv1, false ).Outputting(OutChannel.B),
				SF_NodeConnector.Create( this, "IN", "", ConType.cInput, ValueType.VTv1v4, false ).SetRequired( true )};
			//connectors [0].lockDefaultType = true;
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1]);

			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return   GetConnectorByStringID( "IN" ).TryEvaluate()  ;
		}

		public override float EvalCPU( int c ) {
			return Mathf.Abs( GetInputData( "IN", 1 ) );
		}
	}



	[System.Serializable]
	public class SFN_ColorChannelA : SF_Node_Arithmetic {

		public SFN_ColorChannelA() {
		}

		public override void Initialize() {
			base.Initialize( "channel A" );
			base.showColor = true;
			UseLowerReadonlyValues( true );
			connectors = new SF_NodeConnector[]{
				SF_NodeConnectorStaticOutPut.CreateC( this, "OUT", "", ConType.cOutput, ValueType.VTv1, false ).Outputting(OutChannel.A),
				SF_NodeConnector.Create( this, "IN", "", ConType.cInput, ValueType.VTv1v4, false ).SetRequired( true )};
			//connectors [0].lockDefaultType = true;
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1]);
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return   GetConnectorByStringID( "IN" ).TryEvaluate()  ;
		}

		public override float EvalCPU( int c ) {
			return Mathf.Abs( GetInputData( "IN", 3 ) );
		}

	}
}