using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Function Output", "Functions", "Function output", NodeAvailabilityFlags = (int)NodeAvailability.ShaderFunction )]
	public sealed class FunctionOutput : OutputNode
	{
		public FunctionOutput() : base() { CommonInit(); }
		public FunctionOutput( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { CommonInit(); }

		[SerializeField]
		private string m_outputName = "Output";

		[SerializeField]
		private int m_orderIndex = -1;

		[SerializeField]
		private AmplifyShaderFunction m_function;
		public AmplifyShaderFunction Function
		{
			get { return m_function; }
			set { m_function = value; }
		}

		void CommonInit()
		{
			m_isMainOutputNode = false;
			m_connStatus = NodeConnectionStatus.Connected;
			m_activeType = GetType();
			m_currentPrecisionType = PrecisionType.Float;
			m_textLabelWidth = 100;
			m_autoWrapProperties = true;
			AddInputPort( WirePortDataType.FLOAT, false, "  " );
			SetTitleText( m_outputName );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			UIUtils.RegisterFunctionOutputNode( this );
		}


		public override void Destroy()
		{
			base.Destroy();
			UIUtils.UnregisterFunctionOutputNode( this );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ 0 ].MatchPortToConnection();
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ 0 ].MatchPortToConnection();
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			m_inputPorts[ 0 ].UpdateInternalData();
			return m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_outputName = EditorGUILayout.TextField( "Name", m_outputName );
			if ( EditorGUI.EndChangeCheck() )
			{
				SetTitleText( m_outputName );
				UIUtils.UpdateFunctionOutputData( UniqueId, m_outputName );
			}
		}

		public WirePortDataType AutoOutputType
		{
			get { return m_inputPorts[ 0 ].DataType; }
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndex );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_outputName = GetCurrentParam( ref nodeParams );
			m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );

			if ( m_function == null )
				m_function = UIUtils.CurrentWindow.OpenedShaderFunction;
			SetTitleText( m_outputName );
			UIUtils.UpdateFunctionOutputData( UniqueId, m_outputName );
		}

		public string OutputName
		{
			get { return m_outputName; }
		}

		public int OrderIndex
		{
			get { return m_orderIndex; }
			set { m_orderIndex = value; }
		}
	}
}
