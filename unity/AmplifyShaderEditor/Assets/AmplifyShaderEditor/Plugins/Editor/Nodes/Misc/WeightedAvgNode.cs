// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;
using UnityEditor;
namespace AmplifyShaderEditor
{
	[Serializable]

	public class WeightedAvgNode : ParentNode
	{
		protected string[] AmountsStr = { "Layer 1", "Layer 2", "Layer 3", "Layer 4" };

		[SerializeField]
		protected int m_minimumSize = 1;

		[SerializeField]
		protected WirePortDataType m_mainDataType = WirePortDataType.FLOAT;

		[SerializeField]
		protected string[] m_inputData;
		[SerializeField]
		protected int m_activeCount = 0;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "Weights" );
			AddInputPort( WirePortDataType.FLOAT, false, AmountsStr[ 0 ] );
			AddInputPort( WirePortDataType.FLOAT, false, AmountsStr[ 1 ] );
			AddInputPort( WirePortDataType.FLOAT, false, AmountsStr[ 2 ] );
			AddInputPort( WirePortDataType.FLOAT, false, AmountsStr[ 3 ] );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );

			UpdateConnection( 0 );
			m_useInternalPortData = true;
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

		void UpdateInputPorts( int activePorts )
		{
			int idx = 1;
			for ( ; idx < m_minimumSize + activePorts; idx++ )
			{
				m_inputPorts[ idx ].Visible = true;
			}

			m_activeCount = idx - 1;
			
			for ( ; idx < m_inputPorts.Count; idx++ )
			{
				m_inputPorts[ idx ].Visible = false;
			}
		}

		protected void UpdateConnection( int portId )
		{
			if( m_inputPorts[ portId ].IsConnected )
				m_inputPorts[ portId ].MatchPortToConnection();

			if ( portId == 0 )
			{
				switch ( m_inputPorts[ 0 ].DataType )
				{
					case WirePortDataType.INT:
					case WirePortDataType.FLOAT:
					{
						UpdateInputPorts( 1 );
					}
					break;
					case WirePortDataType.FLOAT2:
					{
						UpdateInputPorts( 2 );
					}
					break;
					case WirePortDataType.FLOAT3:
					{
						UpdateInputPorts( 3 );
					}
					break;
					case WirePortDataType.COLOR:
					case WirePortDataType.FLOAT4:
					{
						UpdateInputPorts( 4 );
					}
					break;
					case WirePortDataType.OBJECT:
					case WirePortDataType.FLOAT3x3:
					case WirePortDataType.FLOAT4x4:
					{
						for ( int i = 1; i < m_inputPorts.Count; i++ )
						{
							m_inputPorts[ i ].Visible = false;
						}
						m_activeCount = 0;
					}
					break;
				}
			}
			else
			{
				SetMainOutputType();
			}
			m_sizeIsDirty = true;
		}

		protected void SetMainOutputType()
		{
			m_mainDataType = WirePortDataType.OBJECT;
			int count = m_inputPorts.Count;
			for ( int i = 1; i < count; i++ )
			{
				if ( m_inputPorts[ i ].Visible )
				{
					if ( m_mainDataType != m_inputPorts[ i ].DataType &&
							UIUtils.GetPriority( m_inputPorts[ i ].DataType ) > UIUtils.GetPriority( m_mainDataType ) )
					{
						m_mainDataType = m_inputPorts[ i ].DataType;
					}
				}
			}

			m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
		}

		protected void GetInputData( ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			m_inputData[ 0 ] = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			for ( int i = 1; i < m_inputPorts.Count; i++ )
			{
				if ( m_inputPorts[ i ].Visible )
				{
					m_inputData[ i ] = m_inputPorts[ i ].GenerateShaderForOutput( ref dataCollector, m_mainDataType, ignoreLocalvar, true );
				}
			}
		}

		public override void ReadInputDataFromString( ref string[] nodeParams )
		{
			base.ReadInputDataFromString( ref nodeParams );
			UpdateConnection( 0 );
		}
	}
}
