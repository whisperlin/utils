using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Function Node", "Functions", "Function Node", KeyCode.None, false, 0, int.MaxValue, typeof( AmplifyShaderFunction ) )]
	public class FunctionNode : ParentNode
	{
		[SerializeField]
		private AmplifyShaderFunction m_function;

		//[SerializeField]
		private ParentGraph m_functionGraph;

		[SerializeField]
		private int m_functionGraphId = -1;

		[SerializeField]
		private List<FunctionInput> m_allFunctionInputs;

		[SerializeField]
		private List<FunctionOutput> m_allFunctionOutputs;

		[SerializeField]
		private ReordenatorNode m_reordenator;

		[SerializeField]
		private string m_filename;

		[SerializeField]
		private string m_headerTitle = string.Empty;

		[SerializeField]
		private int m_orderIndex;

		[SerializeField]
		private string m_functionCheckSum;

		private bool m_parametersFoldout = true;
		private ParentGraph m_outsideGraph = null;

		string LastLine( string text )
		{
			string[] lines = text.Replace( "\r", "" ).Split( '\n' );
			return lines[ lines.Length - 1 ];
		}

		public void CommonInit( AmplifyShaderFunction function )
		{
			if ( function == null )
				return;

			m_function = function;
			SetTitleText( Function.FunctionName );

			if ( m_functionGraph == null )
			{
				m_functionGraph = new ParentGraph();
				m_functionGraph.ParentWindow = ContainerGraph.ParentWindow;
			}

			m_functionGraphId = Mathf.Max( m_functionGraphId, ContainerGraph.ParentWindow.GraphCount );
			ContainerGraph.ParentWindow.GraphCount = m_functionGraphId + 1;
			m_functionGraph.SetGraphId( m_functionGraphId );

			ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
			ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;

			AmplifyShaderEditorWindow.LoadFromMeta( ref m_functionGraph, ContainerGraph.ParentWindow.ContextMenuInstance, ContainerGraph.ParentWindow.CurrentVersionInfo, Function.FunctionInfo );
			m_functionCheckSum = LastLine( m_function.FunctionInfo );
			List<ParentNode> propertyList = UIUtils.PropertyNodesList();

			ContainerGraph.ParentWindow.CustomGraph = cachedGraph;

			m_allFunctionInputs = new List<FunctionInput>();
			m_allFunctionOutputs = new List<FunctionOutput>();

			for ( int i = 0; i < m_functionGraph.AllNodes.Count; i++ )
			{
				//Debug.Log( m_functionGraph.AllNodes[ i ].GetType()+" "+ m_functionGraph.AllNodes[ i ].IsConnected );
				if ( m_functionGraph.AllNodes[ i ].GetType() == typeof( FunctionInput ) )
				{
					m_allFunctionInputs.Add( m_functionGraph.AllNodes[ i ] as FunctionInput );
				}
				else if ( m_functionGraph.AllNodes[ i ].GetType() == typeof( FunctionOutput ) )
				{
					m_allFunctionOutputs.Add( m_functionGraph.AllNodes[ i ] as FunctionOutput );
				}
			}

			m_allFunctionInputs.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );
			m_allFunctionOutputs.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

			int inputCount = m_allFunctionInputs.Count;
			for ( int i = 0; i < inputCount; i++ )
			{
				AddInputPort( m_allFunctionInputs[ i ].SelectedInputType, false, m_allFunctionInputs[ i ].InputName );
				PortSwitchRestriction( m_inputPorts[ i ] );
			}

			int outputCount = m_allFunctionOutputs.Count;
			for ( int i = 0; i < outputCount; i++ )
			{
				AddOutputPort( m_allFunctionOutputs[ i ].AutoOutputType, m_allFunctionOutputs[ i ].OutputName );
				PortSwitchRestriction( m_outputPorts[ i ] );
			}

			//create reordenator to main graph
			bool inside = false;
			if ( ContainerGraph.ParentWindow.CustomGraph != null )
				inside = true;

			if ( /*hasConnectedProperties*/propertyList.Count > 0 )
			{
				m_reordenator = ScriptableObject.CreateInstance<ReordenatorNode>();
				m_reordenator.Init( "_" + Function.FunctionName, Function.FunctionName, propertyList );
				m_reordenator.OrderIndex = m_orderIndex;
				m_reordenator.HeaderTitle = Function.FunctionName;
				m_reordenator.IsInside = inside;
			}

			if ( m_reordenator != null )
			{
				cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
				ContainerGraph.ParentWindow.CustomGraph = null;
				UIUtils.RegisterPropertyNode( m_reordenator );
				ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			}

			m_textLabelWidth = 100;
		}

		public void PortSwitchRestriction( WirePort port )
		{
			switch ( port.DataType )
			{
				case WirePortDataType.OBJECT:
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				{
					port.CreatePortRestrictions( WirePortDataType.FLOAT, WirePortDataType.FLOAT2, WirePortDataType.FLOAT3, WirePortDataType.FLOAT4, WirePortDataType.COLOR, WirePortDataType.INT, WirePortDataType.OBJECT );
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					port.CreatePortRestrictions( WirePortDataType.FLOAT3x3, WirePortDataType.FLOAT4x4, WirePortDataType.OBJECT );
				}
				break;
				case WirePortDataType.SAMPLER1D:
				case WirePortDataType.SAMPLER2D:
				case WirePortDataType.SAMPLER3D:
				case WirePortDataType.SAMPLERCUBE:
				{
					port.CreatePortRestrictions( WirePortDataType.SAMPLER1D, WirePortDataType.SAMPLER2D, WirePortDataType.SAMPLER3D, WirePortDataType.SAMPLERCUBE, WirePortDataType.OBJECT );
				}
				break;
				default:
				break;
			}
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			UIUtils.RegisterFunctionNode( this );
		}

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			base.SetupFromCastObject( obj );
			AmplifyShaderFunction function = obj as AmplifyShaderFunction;
			CommonInit( function );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			if ( m_allFunctionInputs[ portId ].AutoCast )
			{
				m_inputPorts[ portId ].MatchPortToConnection();

				ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
				ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;
				m_allFunctionInputs[ portId ].ChangeOutputType( m_inputPorts[ portId ].DataType, false );
				ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			}

			for ( int i = 0; i < m_allFunctionOutputs.Count; i++ )
			{
				m_outputPorts[ i ].ChangeType( m_allFunctionOutputs[ i ].InputPorts[ 0 ].DataType, false );
			}
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			if ( m_allFunctionInputs[ inputPortId ].AutoCast )
			{
				m_inputPorts[ inputPortId ].MatchPortToConnection();

				ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
				ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;
				m_allFunctionInputs[ inputPortId ].ChangeOutputType( m_inputPorts[ inputPortId ].DataType, false );
				ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			}

			for ( int i = 0; i < m_allFunctionOutputs.Count; i++ )
			{
				m_outputPorts[ i ].ChangeType( m_allFunctionOutputs[ i ].InputPorts[ 0 ].DataType, false );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			if ( Function == null )
				return;

			if ( Function.Description.Length > 0 )
				NodeUtils.DrawPropertyGroup( ref m_parametersFoldout, "Parameters", DrawDescription );
		}

		private void DrawDescription()
		{
			EditorGUILayout.HelpBox( "Description:\n" + Function.Description, MessageType.None );
		}

		public override void Destroy()
		{
			base.Destroy();

			if ( m_reordenator != null )
			{
				m_reordenator.Destroy();
				m_reordenator = null;
				ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
				ContainerGraph.ParentWindow.CustomGraph = null;
				UIUtils.UnregisterPropertyNode( m_reordenator );
				ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			}

			UIUtils.UnregisterFunctionNode( this );

			ParentGraph cachedGraph2 = ContainerGraph.ParentWindow.CustomGraph;
			ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;

			if ( m_allFunctionInputs != null )
				m_allFunctionInputs.Clear();
			m_allFunctionInputs = null;

			if ( m_allFunctionOutputs != null )
				m_allFunctionOutputs.Clear();
			m_allFunctionOutputs = null;

			if ( m_functionGraph != null )
				m_functionGraph.Destroy();
			m_functionGraph = null;

			ContainerGraph.ParentWindow.CustomGraph = cachedGraph2;
			m_function = null;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if ( ContainerGraph.ParentWindow.CheckFunctions && m_function != null )
			{
				string newCheckSum = LastLine( m_function.FunctionInfo );
				if ( !m_functionCheckSum.Equals( newCheckSum ) )
				{
					m_functionCheckSum = newCheckSum;
					ContainerGraph.OnDuplicateEvent += DuplicateMe;
				}
			}
		}

		public void DuplicateMe()
		{
			ParentNode newNode = ContainerGraph.CreateNode( m_function, false, Vec2Position );
			for ( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if ( m_outputPorts[ i ].IsConnected )
				{
					if ( newNode.OutputPorts != null && newNode.OutputPorts[ i ] != null )
					{
						for ( int j = m_outputPorts[ i ].ExternalReferences.Count - 1; j >= 0; j-- )
						{
							ContainerGraph.CreateConnection( m_outputPorts[ i ].ExternalReferences[ j ].NodeId, m_outputPorts[ i ].ExternalReferences[ j ].PortId, newNode.OutputPorts[ i ].NodeId, newNode.OutputPorts[ i ].PortId );
						}
					}
				}
				else
				{
					if ( newNode.OutputPorts != null && newNode.OutputPorts[ i ] != null )
					{
						ContainerGraph.DeleteConnection( false, newNode.UniqueId, i, false, false, false );
					}
				}
			}

			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				if ( m_inputPorts[ i ].IsConnected )
				{
					if ( newNode.InputPorts != null && i < newNode.InputPorts.Count && newNode.InputPorts[ i ] != null )
					{
						ContainerGraph.CreateConnection( newNode.InputPorts[ i ].NodeId, newNode.InputPorts[ i ].PortId, m_inputPorts[ i ].ExternalReferences[ 0 ].NodeId, m_inputPorts[ i ].ExternalReferences[ 0 ].PortId );
					}
				}
			}

			ContainerGraph.OnDuplicateEvent -= DuplicateMe;

			ContainerGraph.DestroyNode( this, false );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
			m_outsideGraph = cachedGraph;
			ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;

			if ( m_reordenator != null && m_reordenator.RecursiveCount() > 0 && m_reordenator.HasTitle )
			{
				dataCollector.AddToProperties( UniqueId, "[Header(" + m_reordenator.HeaderTitle + ")]", m_reordenator.OrderIndex );
			}
			string result = string.Empty;
			for ( int i = 0; i < m_allFunctionInputs.Count; i++ )
			{
				if ( m_inputPorts[ i ].IsConnected )
					m_allFunctionInputs[ i ].OnPortGeneration += FunctionNodeOnPortGeneration;
			}

			result += m_allFunctionOutputs[ outputId ].GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );

			for ( int i = 0; i < m_allFunctionInputs.Count; i++ )
			{
				if ( m_inputPorts[ i ].IsConnected )
					m_allFunctionInputs[ i ].OnPortGeneration -= FunctionNodeOnPortGeneration;
			}

			ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			return result;
		}

		private string FunctionNodeOnPortGeneration( ref MasterNodeDataCollector dataCollector, int index, ParentGraph graph )
		{
			ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
			ContainerGraph.ParentWindow.CustomGraph = m_outsideGraph;
			string result = m_inputPorts[ index ].GeneratePortInstructions( ref dataCollector );
			ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			return result;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );

			if ( Function != null )
				IOUtils.AddFieldValueToString( ref nodeInfo, m_function.name );
			else
				IOUtils.AddFieldValueToString( ref nodeInfo, m_filename );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_reordenator != null ? m_reordenator.RawOrderIndex : -1 );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_headerTitle );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_functionGraphId );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_filename = GetCurrentParam( ref nodeParams );
			m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_headerTitle = GetCurrentParam( ref nodeParams );

			if ( UIUtils.CurrentShaderVersion() > 7203 )
			{
				m_functionGraphId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}
			string[] guids = AssetDatabase.FindAssets( "t:AmplifyShaderFunction " + m_filename );
			if ( guids.Length > 0 )
			{
				AmplifyShaderFunction loaded = AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( AssetDatabase.GUIDToAssetPath( guids[ 0 ] ) );
				if ( loaded != null )
				{
					CommonInit( loaded );
				}
				else
				{
					SetTitleText( "ERROR" );
				}
			}
			else
			{
				SetTitleText( "Missing Function" );
			}
		}

		public override void ReadOutputDataFromString( ref string[] nodeParams )
		{
			if ( Function == null )
				return;

			base.ReadOutputDataFromString( ref nodeParams );

			ConfigureInputportsAfterRead();
		}

		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if ( Function == null )
				return;

			ContainerGraph.DeSelectAll();
			this.Selected = true;

			ContainerGraph.ParentWindow.OnLeftMouseUp();
			AmplifyShaderEditorWindow.LoadShaderFunctionToASE( Function, true );
			this.Selected = false;
		}

		private void ConfigureInputportsAfterRead()
		{
			if ( InputPorts != null )
			{
				int inputCount = InputPorts.Count;
				for ( int i = 0; i < inputCount; i++ )
				{
					InputPorts[ i ].ChangeProperties( m_allFunctionInputs[ i ].InputName, m_allFunctionInputs[ i ].SelectedInputType, false );
				}
			}

			if ( OutputPorts != null )
			{
				int outputCount = OutputPorts.Count;
				for ( int i = 0; i < outputCount; i++ )
				{
					OutputPorts[ i ].ChangeProperties( m_allFunctionOutputs[ i ].OutputName, m_allFunctionOutputs[ i ].AutoOutputType, false );
				}
			}
		}

		public bool HasProperties { get { return m_reordenator != null; } }

		public ParentGraph FunctionGraph
		{
			get
			{
				return m_functionGraph;
			}

			set
			{
				m_functionGraph = value;
			}
		}

		public AmplifyShaderFunction Function
		{
			get { return m_function; }
			set { m_function = value; }
		}

		public override void SetContainerGraph( ParentGraph newgraph )
		{
			base.SetContainerGraph( newgraph );
			if ( m_functionGraph == null )
				return;
			for ( int i = 0; i < m_functionGraph.AllNodes.Count; i++ )
			{
				m_functionGraph.AllNodes[ i ].SetContainerGraph( m_functionGraph );
			}
		}
	}
}
