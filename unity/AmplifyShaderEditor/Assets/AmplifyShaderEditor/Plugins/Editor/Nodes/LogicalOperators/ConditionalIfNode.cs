// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "If", "Logical Operators", "Conditional comparison between A with B.", null, KeyCode.None, true, false, null, null, true )]
	public sealed class ConditionalIfNode : ParentNode
	{
		private const string  UseUnityBranchesStr = "Dynamic Branching";
		private const string UnityBranchStr = "UNITY_BRANCH ";

		private readonly string[] IfOps = { "if( {0} > {1} )",
											"if( {0} == {1} )",
											"if( {0} < {1} )" };

		private WirePortDataType m_inputMainDataType = WirePortDataType.FLOAT;
		private WirePortDataType m_outputMainDataType = WirePortDataType.FLOAT;
		private string[] m_results = { string.Empty, string.Empty, string.Empty };

		[SerializeField]
		private bool m_useUnityBranch = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, true, "A" );
			AddInputPort( WirePortDataType.FLOAT, true, "B" );
			AddInputPort( WirePortDataType.FLOAT, false, "A > B" );
			AddInputPort( WirePortDataType.FLOAT, false, "A == B" );
			AddInputPort( WirePortDataType.FLOAT, false, "A < B" );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_textLabelWidth = 131;
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnection( inputPortId );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnection( portId );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_useUnityBranch = EditorGUILayoutToggle( UseUnityBranchesStr, m_useUnityBranch );
		}


		public override void OnInputPortDisconnected( int portId )
		{
			UpdateConnection( portId );
		}

		//void TestMainInputDataType()
		//{
		//	WirePortDataType newType = WirePortDataType.FLOAT;
		//	if ( m_inputPorts[ 0 ].IsConnected && UIUtils.GetPriority( m_inputPorts[ 0 ].DataType ) > UIUtils.GetPriority( newType ) )
		//	{
		//		newType = m_inputPorts[ 0 ].DataType;
		//	}

		//	if ( m_inputPorts[ 1 ].IsConnected && ( UIUtils.GetPriority( m_inputPorts[ 1 ].DataType ) > UIUtils.GetPriority( newType ) ) )
		//	{
		//		newType = m_inputPorts[ 1 ].DataType;
		//	}

		//	m_inputMainDataType = newType;
		//}

		void TestMainOutputDataType()
		{
			WirePortDataType newType = WirePortDataType.FLOAT;
			for ( int i = 2; i < 5; i++ )
			{
				if ( m_inputPorts[ i ].IsConnected && ( UIUtils.GetPriority( m_inputPorts[ i ].DataType ) > UIUtils.GetPriority( newType ) ) )
				{
					newType = m_inputPorts[ i ].DataType;
				}
			}

			if ( newType != m_outputMainDataType )
			{
				m_outputMainDataType = newType;
				m_outputPorts[ 0 ].ChangeType( m_outputMainDataType, false );
			}
		}

		public void UpdateConnection( int portId )
		{
			m_inputPorts[ portId ].MatchPortToConnection();
			switch ( portId )
			{
				//case 0:
				//case 1:
				//{
				//	TestMainInputDataType();
				//}
				//break;
				case 2:
				case 3:
				case 4:
				{
					TestMainOutputDataType();
				}
				break;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			string AValue = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputMainDataType, ignoreLocalvar, true );
			string BValue = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_inputMainDataType, ignoreLocalvar, true );

			m_results[ 0 ] = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, m_outputMainDataType, ignoreLocalvar, true );
			m_results[ 1 ] = m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, m_outputMainDataType, ignoreLocalvar, true );
			m_results[ 2 ] = m_inputPorts[ 4 ].GenerateShaderForOutput( ref dataCollector, m_outputMainDataType, ignoreLocalvar, true );

			string localVarName = "ifLocalVar" + OutputId;
			string localVarDec = string.Format( "{0} {1} = 0;", UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ), localVarName );

			bool firstIf = true;
			List<string> instructions = new List<string>();
			instructions.Add( localVarDec );
			for ( int i = 2; i < 5; i++ )
			{
				if ( m_inputPorts[ i ].IsConnected )
				{
					int idx = i - 2;
					string ifOp = string.Format( IfOps[ idx ], AValue, BValue );
					if ( m_useUnityBranch )
					{
						ifOp = UnityBranchStr + ifOp;
					}
					instructions.Add( firstIf ? ifOp : "else " + ifOp );
					instructions.Add( string.Format( "\t{0} = {1};", localVarName, m_results[ idx ] ) );
					firstIf = false;
				}
			}

			if ( firstIf )
			{
				if( DebugConsoleWindow.DeveloperMode )
					UIUtils.ShowMessage( "No result inputs connectect on If Node. Using node internal data." );
				// no input nodes connected ... use port default values
				for ( int i = 2; i < 5; i++ )
				{
					int idx = i - 2;
					string ifOp = string.Format( IfOps[ idx ], AValue, BValue );
					if ( m_useUnityBranch )
					{
						ifOp = UnityBranchStr + ifOp;
					}
					instructions.Add( firstIf ? ifOp : "else " + ifOp );
					instructions.Add( string.Format( "\t{0} = {1};", localVarName, m_results[ idx ] ) );
					firstIf = false;
				}
			}

			for ( int i = 0; i < instructions.Count; i++ )
			{
				dataCollector.AddToLocalVariables( dataCollector.PortCategory, UniqueId, instructions[ i ] , true );
			}
			//dataCollector.AddInstructions( true, true, instructions.ToArray() );

			instructions.Clear();
			instructions = null;

			m_outputPorts[ 0 ].SetLocalValue( localVarName );
			return localVarName;
		}
		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 4103 )
			{
				m_useUnityBranch = Convert.ToBoolean( GetCurrentParam( ref nodeParams ));
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_useUnityBranch );
		}
	}
}
