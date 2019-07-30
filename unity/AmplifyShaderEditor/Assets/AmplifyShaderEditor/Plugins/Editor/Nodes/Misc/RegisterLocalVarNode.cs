// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Register Local Var", "Miscellaneous", "Forces a local variable to be written with the given name. Can then be fetched at any place with a <b>Get Local Var</b> node." )]
	public sealed class RegisterLocalVarNode : SignalGeneratorNode
	{
		private const string LocalDefaultNameStr = "myVarName";
		private const string LocalVarNameStr = "Var Name";
		private const string OrderIndexStr = "Order Index";
		private const string AutoOrderIndexStr = "Auto Order";

		private string m_oldName = string.Empty;
		private bool m_reRegisterName = false;
		private int m_autoOrderIndex = int.MaxValue;
		private bool m_forceUpdate = true;

		[SerializeField]
		private string m_variableName = LocalDefaultNameStr;

		[SerializeField]
		private int m_orderIndex = -1;

		[SerializeField]
		private bool m_autoIndexActive = true;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_textLabelWidth = 85;
			m_variableName += UIUtils.LocalVarNodeAmount();
			m_oldName = m_variableName;
			m_autoWrapProperties = true;
			UIUtils.RegisterLocalVarNode( this );
			UpdateTitle();
			m_previewShaderGUID = "5aaa1d3ea9e1fa64781647e035a82334";
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override void Destroy()
		{
			base.Destroy();

			UIUtils.UnregisterLocalVarNode( this );
			UIUtils.ReleaseLocalVariableName( UniqueId, m_variableName );
		}

		void UpdateTitle()
		{
			SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, m_variableName ) );
		}

		public override void DrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_variableName = EditorGUILayoutTextField( LocalVarNameStr, m_variableName );
			if ( EditorGUI.EndChangeCheck() )
			{

				m_variableName = UIUtils.RemoveInvalidCharacters( m_variableName );
				if ( string.IsNullOrEmpty( m_variableName ) )
				{
					m_variableName = LocalDefaultNameStr + OutputId;
				}

				if ( UIUtils.IsLocalvariableNameAvailable( m_variableName ) )
				{
					UIUtils.ReleaseLocalVariableName( UniqueId, m_oldName );
					UIUtils.RegisterLocalVariableName( UniqueId, m_variableName );
					m_oldName = m_variableName;
					UIUtils.UpdateLocalVarDataNode( UniqueId, m_variableName );
					UpdateTitle();
					m_forceUpdate = true;
				}
				else
				{
					m_variableName = m_oldName;
				}
			}

			DrawPrecisionProperty();
		}


		public override void OnEnable()
		{
			base.OnEnable();
			m_reRegisterName = true;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if ( m_reRegisterName )
			{
				m_reRegisterName = false;
				UIUtils.RegisterLocalVariableName( UniqueId, m_variableName );
			}

			if ( m_forceUpdate )
			{
				m_forceUpdate = false;
				UpdateTitle();
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
			{
				return m_outputPorts[ 0 ].LocalValue;
			}
			string result = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			RegisterLocalVariable( 0, result, ref dataCollector, m_variableName + OutputId );
			return m_outputPorts[ 0 ].LocalValue;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_variableName = GetCurrentParam( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 14 )
				m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );

			if ( UIUtils.CurrentShaderVersion() > 3106 )
			{
				m_autoIndexActive = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
			else
			{
				m_autoIndexActive = false;
			}

			UIUtils.UpdateLocalVarDataNode( UniqueId, m_variableName );

			UIUtils.ReleaseLocalVariableName( UniqueId, m_oldName );
			UIUtils.RegisterLocalVariableName( UniqueId, m_variableName );

			m_forceUpdate = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_variableName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndex );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoIndexActive );
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			if ( m_autoOrderIndex < nodeData.OrderIndex )
			{
				nodeData.OrderIndex = m_autoOrderIndex - 1;
			}
			else
			{
				m_autoOrderIndex = nodeData.OrderIndex;
				nodeData.OrderIndex -= 1;
			}

			base.PropagateNodeData( nodeData, ref dataCollector );
		}

		public override void ResetNodeData()
		{
			base.ResetNodeData();
			m_autoOrderIndex = int.MaxValue;
		}

		public override string DataToArray { get { return m_variableName; } }
	}
}
