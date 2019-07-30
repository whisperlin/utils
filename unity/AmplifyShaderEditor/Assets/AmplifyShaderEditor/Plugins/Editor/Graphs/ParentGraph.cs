// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ParentGraph : ISerializationCallbackReceiver
	{
		public enum NodeLOD
		{
			LOD0,
			LOD1,
			LOD2,
			LOD3,
			LOD4,
			LOD5
		}

		private NodeLOD m_lodLevel = NodeLOD.LOD0;
		private GUIStyle nodeStyleOff;
		private GUIStyle nodeStyleOn;
		private GUIStyle nodeTitle;
		private GUIStyle commentaryBackground;
		//private GUIStyle m_overlayThumbnail;

		//[SerializeField]
		//private AmplifyShaderFunction m_currentShaderFunction = null;

		public delegate void EmptyGraphDetected( ParentGraph graph );
		public event EmptyGraphDetected OnEmptyGraphDetectedEvt;

		public delegate void NodeEvent( ParentNode node );
		public event NodeEvent OnNodeEvent = null;
		public event NodeEvent OnNodeRemovedEvent;

		public delegate void DuplicateEvent();
		public event DuplicateEvent OnDuplicateEvent;

		public event MasterNode.OnMaterialUpdated OnMaterialUpdatedEvent;
		public event MasterNode.OnMaterialUpdated OnShaderUpdatedEvent;

		private bool m_afterDeserializeFlag = true;

		//[SerializeField]
		private AmplifyShaderEditorWindow m_parentWindow = null;

		[SerializeField]
		private int m_validNodeId;

		[SerializeField]
		private List<ParentNode> m_nodes;

		// Sampler Nodes registry
		[SerializeField]
		private NodeUsageRegister m_samplerNodes;

		[SerializeField]
		private NodeUsageRegister m_texturePropertyNodes;

		[SerializeField]
		private NodeUsageRegister m_textureArrayNodes;

		// Screen Color Nodes registry
		[SerializeField]
		private NodeUsageRegister m_screenColorNodes;

		[SerializeField]
		private NodeUsageRegister m_localVarNodes;

		[SerializeField]
		private NodeUsageRegister m_propertyNodes;

		[SerializeField]
		private NodeUsageRegister m_functionInputNodes;

		[SerializeField]
		private NodeUsageRegister m_functionNodes;

		[SerializeField]
		private NodeUsageRegister m_functionOutputNodes;

		[SerializeField]
		private int m_masterNodeId = Constants.INVALID_NODE_ID;

		[SerializeField]
		private bool m_isDirty;

		[SerializeField]
		private bool m_saveIsDirty = false;

		[SerializeField]
		private int m_nodeClicked;

		[SerializeField]
		private int m_loadedShaderVersion;

		[SerializeField]
		private int m_instancePropertyCount = 0;

		[SerializeField]
		private int m_virtualTextureCount = 0;

		[SerializeField]
		private int m_graphId = 0;

		[SerializeField]
		private PrecisionType m_currentPrecision = PrecisionType.Float;

		[SerializeField]
		public NodeAvailability m_currentCanvasMode = NodeAvailability.SurfaceShader;

		private List<ParentNode> m_visibleNodes = new List<ParentNode>();

		private List<ParentNode> m_nodePreviewList = new List<ParentNode>();

		private Dictionary<int, ParentNode> m_nodesDict;
		private List<ParentNode> m_selectedNodes;
		private List<ParentNode> m_markedForDeletion;
		private List<WireReference> m_highlightedWires;
		private System.Type m_masterNodeDefaultType;

		private NodeGrid m_nodeGrid;

		private bool m_markedToDeSelect = false;
		private int m_markToSelect = -1;
		private bool m_markToReOrder = false;

		private bool m_hasUnConnectedNodes = false;

		private bool m_checkSelectedWireHighlights = false;

		//private Rect m_auxRect = new Rect();

		//private GUIStyle m_titleOverlay;
		//private GUIStyle m_buttonOverlay;
		//int m_tittleOverlayIndex = -1;
		//int m_buttonOverlayIndex = -1;

		// Bezier info
		private List<WireBezierReference> m_bezierReferences;
		private const int MaxBezierReferences = 50;
		private int m_wireBezierCount = 0;

		protected int m_normalDependentCount = 0;
		private bool m_forceCategoryRefresh = false;

		public ParentGraph()
		{
			m_normalDependentCount = 0;
			m_nodeGrid = new NodeGrid();
			m_nodes = new List<ParentNode>();
			m_samplerNodes = new NodeUsageRegister();
			m_propertyNodes = new NodeUsageRegister();
			m_functionInputNodes = new NodeUsageRegister();
			m_functionNodes = new NodeUsageRegister();
			m_functionOutputNodes = new NodeUsageRegister();
			m_texturePropertyNodes = new NodeUsageRegister();
			m_textureArrayNodes = new NodeUsageRegister();
			m_screenColorNodes = new NodeUsageRegister();
			m_localVarNodes = new NodeUsageRegister();

			m_selectedNodes = new List<ParentNode>();
			m_markedForDeletion = new List<ParentNode>();
			m_highlightedWires = new List<WireReference>();
			m_nodesDict = new Dictionary<int, ParentNode>();
			m_validNodeId = 0;
			IsDirty = false;
			SaveIsDirty = false;
			m_masterNodeDefaultType = typeof( StandardSurfaceOutputNode );

			m_bezierReferences = new List<WireBezierReference>( MaxBezierReferences );
			for( int i = 0; i < MaxBezierReferences; i++ )
			{
				m_bezierReferences.Add( new WireBezierReference() );
			}

			nodeStyleOff = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOff );
			nodeStyleOn = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOn );
			nodeTitle = UIUtils.GetCustomStyle( CustomStyle.NodeHeader );
			commentaryBackground = UIUtils.GetCustomStyle( CustomStyle.CommentaryBackground );
			//m_overlayThumbnail = GUI.skin.FindStyle( "ObjectFieldThumbOverlay2" );
		}

		public void UpdateRegisters()
		{
			m_samplerNodes.UpdateNodeArr();
			m_propertyNodes.UpdateNodeArr();
			m_functionInputNodes.UpdateNodeArr();
			m_functionNodes.UpdateNodeArr();
			m_functionOutputNodes.UpdateNodeArr();
			m_texturePropertyNodes.UpdateNodeArr();
			m_textureArrayNodes.UpdateNodeArr();
			m_screenColorNodes.UpdateNodeArr();
			m_localVarNodes.UpdateNodeArr();
		}

		public int GetValidId()
		{
			return m_validNodeId++;
		}

		void UpdateIdFromNode( ParentNode node )
		{
			if( node.UniqueId >= m_validNodeId )
			{
				m_validNodeId = node.UniqueId + 1;
			}
		}

		public void ResetNodeConnStatus()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Connected )
				{
					m_nodes[ i ].ConnStatus = NodeConnectionStatus.Not_Connected;
				}
			}
		}

		public void CleanUnusedNodes()
		{
			List<ParentNode> unusedNodes = new List<ParentNode>();
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Not_Connected )
				{
					unusedNodes.Add( m_nodes[ i ] );
				}
			}

			for( int i = 0; i < unusedNodes.Count; i++ )
			{
				DestroyNode( unusedNodes[ i ] );
			}
			unusedNodes.Clear();
			unusedNodes = null;

			IsDirty = true;
		}

		// Destroy all nodes excluding Master Node
		public void ClearGraph()
		{
			List<ParentNode> list = new List<ParentNode>();
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_nodes[ i ].UniqueId != m_masterNodeId )
				{
					list.Add( m_nodes[ i ] );
				}
			}

			while( list.Count > 0 )
			{
				DestroyNode( list[ 0 ] );
				list.RemoveAt( 0 );
			}
		}

		public void CleanNodes()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( m_nodes[ i ] != null )
				{
					Undo.ClearUndo( m_nodes[ i ] );
					m_nodes[ i ].Destroy();
					GameObject.DestroyImmediate( m_nodes[ i ] );
				}
			}

			m_masterNodeId = Constants.INVALID_NODE_ID;
			m_validNodeId = 0;
			m_instancePropertyCount = 0;
			m_virtualTextureCount = 0;

			m_nodes.Clear();
			m_samplerNodes.Clear();
			m_propertyNodes.Clear();
			m_functionInputNodes.Clear();
			m_functionNodes.Clear();
			m_functionOutputNodes.Clear();
			m_texturePropertyNodes.Clear();
			m_textureArrayNodes.Clear();
			m_screenColorNodes.Clear();
			m_localVarNodes.Clear();
			m_selectedNodes.Clear();
			m_markedForDeletion.Clear();
		}

		public void ResetHighlightedWires()
		{
			for( int i = 0; i < m_highlightedWires.Count; i++ )
			{
				m_highlightedWires[ i ].WireStatus = WireStatus.Default;
			}
			m_highlightedWires.Clear();
		}

		public void HighlightWiresStartingNode( ParentNode node )
		{
			for( int outputIdx = 0; outputIdx < node.OutputPorts.Count; outputIdx++ )
			{
				for( int extIdx = 0; extIdx < node.OutputPorts[ outputIdx ].ExternalReferences.Count; extIdx++ )
				{
					WireReference wireRef = node.OutputPorts[ outputIdx ].ExternalReferences[ extIdx ];
					ParentNode nextNode = GetNode( wireRef.NodeId );
					if( nextNode && nextNode.ConnStatus == NodeConnectionStatus.Connected )
					{
						InputPort port = nextNode.GetInputPortByUniqueId( wireRef.PortId );
						if( port.ExternalReferences.Count == 0 || port.ExternalReferences[ 0 ].WireStatus == WireStatus.Highlighted )
						{
							// if even one wire is already highlighted then this tells us that this node was already been analysed
							return;
						}

						port.ExternalReferences[ 0 ].WireStatus = WireStatus.Highlighted;
						m_highlightedWires.Add( port.ExternalReferences[ 0 ] );
						HighlightWiresStartingNode( nextNode );
					}
				}
			}
		}

		void PropagateHighlightDeselection( ParentNode node, int portId = -1 )
		{
			if( portId > -1 )
			{
				InputPort port = node.GetInputPortByUniqueId( portId );
				port.ExternalReferences[ 0 ].WireStatus = WireStatus.Default;
			}

			if( node.Selected )
				return;

			for( int i = 0; i < node.InputPorts.Count; i++ )
			{
				if( node.InputPorts[ i ].ExternalReferences.Count > 0 && node.InputPorts[ i ].ExternalReferences[ 0 ].WireStatus == WireStatus.Highlighted )
				{
					// even though node is deselected, it receives wire highlight from a previous one 
					return;
				}
			}

			for( int outputIdx = 0; outputIdx < node.OutputPorts.Count; outputIdx++ )
			{
				for( int extIdx = 0; extIdx < node.OutputPorts[ outputIdx ].ExternalReferences.Count; extIdx++ )
				{
					WireReference wireRef = node.OutputPorts[ outputIdx ].ExternalReferences[ extIdx ];
					ParentNode nextNode = GetNode( wireRef.NodeId );
					PropagateHighlightDeselection( nextNode, wireRef.PortId );
				}
			}
		}

		public void ResetNodesData()
		{
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				m_nodes[ i ].ResetNodeData();
			}
		}

		public void FullCleanUndoStack()
		{
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_nodes[ i ] != null )
				{
					Undo.ClearUndo( m_nodes[ i ] );
				}
			}
		}

		public void FullRegisterOnUndoStack()
		{
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_nodes[ i ] != null )
				{
					Undo.RegisterCompleteObjectUndo( m_nodes[ i ], Constants.UndoRegisterFullGrapId );
				}
			}
		}

		public void Destroy()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( m_nodes[ i ] != null )
				{
					Undo.ClearUndo( m_nodes[ i ] );
					m_nodes[ i ].Destroy();
					GameObject.DestroyImmediate( m_nodes[ i ] );
				}
			}

			OnNodeRemovedEvent = null;

			m_masterNodeId = Constants.INVALID_NODE_ID;
			m_validNodeId = 0;
			m_instancePropertyCount = 0;

			m_nodeGrid.Destroy();
			m_nodeGrid = null;

			m_nodes.Clear();
			m_nodes = null;

			m_samplerNodes.Destroy();
			m_samplerNodes = null;

			m_propertyNodes.Destroy();
			m_propertyNodes = null;

			m_functionInputNodes.Destroy();
			m_functionInputNodes = null;

			m_functionNodes.Destroy();
			m_functionNodes = null;

			m_functionOutputNodes.Destroy();
			m_functionOutputNodes = null;

			m_texturePropertyNodes.Destroy();
			m_texturePropertyNodes = null;

			m_textureArrayNodes.Destroy();
			m_textureArrayNodes = null;

			m_screenColorNodes.Destroy();
			m_screenColorNodes = null;

			m_localVarNodes.Destroy();
			m_localVarNodes = null;

			m_selectedNodes.Clear();
			m_selectedNodes = null;

			m_markedForDeletion.Clear();
			m_markedForDeletion = null;

			m_nodesDict.Clear();
			m_nodesDict = null;

			m_nodePreviewList.Clear();
			m_nodePreviewList = null;

			IsDirty = true;

			OnNodeEvent = null;
			OnDuplicateEvent = null;
			//m_currentShaderFunction = null;

			OnMaterialUpdatedEvent = null;
			OnShaderUpdatedEvent = null;
			OnEmptyGraphDetectedEvt = null;

			nodeStyleOff = null;
			nodeStyleOn = null;
			nodeTitle = null;
			commentaryBackground = null;
		}

		void OnNodeChangeSizeEvent( ParentNode node )
		{
			m_nodeGrid.RemoveNodeFromGrid( node, true );
			m_nodeGrid.AddNodeToGrid( node );
		}

		public void OnNodeFinishMoving( ParentNode node, bool testOnlySelected, InteractionMode interactionMode )
		{
			if( OnNodeEvent != null )
				OnNodeEvent( node );

			m_nodeGrid.RemoveNodeFromGrid( node, true );
			m_nodeGrid.AddNodeToGrid( node );

			if( testOnlySelected )
			{
				for( int i = m_visibleNodes.Count - 1; i > -1; i-- )
				{
					if( node.UniqueId != m_visibleNodes[ i ].UniqueId )
					{
						switch( interactionMode )
						{
							case InteractionMode.Target:
							{
								node.OnNodeInteraction( m_visibleNodes[ i ] );
							}
							break;
							case InteractionMode.Other:
							{
								m_visibleNodes[ i ].OnNodeInteraction( node );
							}
							break;
							case InteractionMode.Both:
							{
								node.OnNodeInteraction( m_visibleNodes[ i ] );
								m_visibleNodes[ i ].OnNodeInteraction( node );
							}
							break;
						}
					}
				}
			}
			else
			{
				for( int i = m_nodes.Count - 1; i > -1; i-- )
				{
					if( node.UniqueId != m_nodes[ i ].UniqueId )
					{
						switch( interactionMode )
						{
							case InteractionMode.Target:
							{
								node.OnNodeInteraction( m_nodes[ i ] );
							}
							break;
							case InteractionMode.Other:
							{
								m_nodes[ i ].OnNodeInteraction( node );
							}
							break;
							case InteractionMode.Both:
							{
								node.OnNodeInteraction( m_nodes[ i ] );
								m_nodes[ i ].OnNodeInteraction( node );
							}
							break;
						}
					}
				}
			}
		}


		public void OnNodeReOrderEvent( ParentNode node, int index )
		{
			if( node.Depth < index )
			{
				Debug.LogWarning( "Reorder canceled: This is a specific method for when reordering needs to be done and a its original index is higher than the new one" );
			}
			else
			{
				m_nodes.Remove( node );
				m_nodes.Insert( index, node );
				m_markToReOrder = true;
			}
		}

		public void AddNode( ParentNode node, bool updateId = false, bool addLast = true, bool registerUndo = true, bool fetchMaterialValues = true )
		{
			if( registerUndo )
			{
				UIUtils.MarkUndoAction();
				Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoCreateNodeId );
				Undo.RegisterCreatedObjectUndo( node, Constants.UndoCreateNodeId );
			}

			if( OnNodeEvent != null )
			{
				OnNodeEvent( node );
			}
			if( updateId )
			{
				node.UniqueId = GetValidId();
			}
			else
			{
				UpdateIdFromNode( node );
			}

			node.SetMaterialMode( CurrentMaterial, fetchMaterialValues );

			if( addLast )
			{
				m_nodes.Add( node );
				node.Depth = m_nodes.Count;
			}
			else
			{
				m_nodes.Insert( 0, node );
				node.Depth = 0;
			}

			if( m_nodesDict.ContainsKey( node.UniqueId ) )
				m_nodesDict[ node.UniqueId ] = node;
			else
				m_nodesDict.Add( node.UniqueId, node );

			m_nodeGrid.AddNodeToGrid( node );
			node.OnNodeChangeSizeEvent += OnNodeChangeSizeEvent;
			node.OnNodeReOrderEvent += OnNodeReOrderEvent;
			IsDirty = true;
		}


		public ParentNode GetClickedNode()
		{
			if( m_nodeClicked < 0 )
				return null;
			return GetNode( m_nodeClicked );
		}

		public ParentNode GetNode( int nodeId )
		{
			if( m_nodesDict.Count != m_nodes.Count )
			{
				m_nodesDict.Clear();
				for( int i = 0; i < m_nodes.Count; i++ )
				{
					if( m_nodes[ i ] != null )
						m_nodesDict.Add( m_nodes[ i ].UniqueId, m_nodes[ i ] );
				}
			}

			if( m_nodesDict.ContainsKey( nodeId ) )
				return m_nodesDict[ nodeId ];

			return null;
		}

		public void ForceReOrder()
		{
			m_nodes.Sort( ( x, y ) => x.Depth.CompareTo( y.Depth ) );
		}

		public bool Draw( DrawInfo drawInfo )
		{
			MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
			if( m_forceCategoryRefresh && masterNode != null )
			{
				masterNode.RefreshAvailableCategories();
				m_forceCategoryRefresh = false;
			}

			SaveIsDirty = false;
			if( m_afterDeserializeFlag )
			{
				m_afterDeserializeFlag = false;
				CleanCorruptedNodes();
				if( m_nodes.Count == 0 )
				{
					ParentWindow.CreateNewGraph( "Empty" );
					SaveIsDirty = true;
					if( OnEmptyGraphDetectedEvt != null )
						OnEmptyGraphDetectedEvt( this );
				}

				for( int i = 0; i < m_nodes.Count; i++ )
				{
					m_nodes[ i ].SetContainerGraph( this );
				}
			}

			if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				if( m_markedToDeSelect )
					DeSelectAll();

				if( m_markToSelect > -1 )
				{
					AddToSelectedNodes( GetNode( m_markToSelect ) );
					m_markToSelect = -1;
				}

				if( m_markToReOrder )
				{
					m_markToReOrder = false;
					int nodesCount = m_nodes.Count;
					for( int i = 0; i < nodesCount; i++ )
					{
						m_nodes[ i ].Depth = i;
					}
				}
			}

			if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				// Resizing Nods per LOD level
				NodeLOD newLevel = NodeLOD.LOD0;
				float referenceValue;
				if( drawInfo.InvertedZoom > 0.5f )
				{
					newLevel = NodeLOD.LOD0;
					referenceValue = 4;
				}
				else if( drawInfo.InvertedZoom > 0.25f )
				{
					newLevel = NodeLOD.LOD1;
					referenceValue = 2;
				}
				else if( drawInfo.InvertedZoom > 0.15f )
				{
					newLevel = NodeLOD.LOD2;
					referenceValue = 1;
				}
				else if( drawInfo.InvertedZoom > 0.1f )
				{
					newLevel = NodeLOD.LOD3;
					referenceValue = 0;
				}
				else if( drawInfo.InvertedZoom > 0.07f )
				{
					newLevel = NodeLOD.LOD4;
					referenceValue = 0;
				}
				else
				{
					newLevel = NodeLOD.LOD5;
					referenceValue = 0;
				}

				// Just a sanity check
				nodeStyleOff = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOff );
				nodeStyleOn = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOn );
				nodeTitle = UIUtils.GetCustomStyle( CustomStyle.NodeHeader );
				commentaryBackground = UIUtils.GetCustomStyle( CustomStyle.CommentaryBackground );

				if( newLevel != m_lodLevel || ( UIUtils.MainSkin != null && UIUtils.MainSkin.textField.border.left != referenceValue ) )
				{
					m_lodLevel = newLevel;
					switch( m_lodLevel )
					{
						default:
						case NodeLOD.LOD0:
						{
							UIUtils.MainSkin.textField.border.left = 4;
							UIUtils.MainSkin.textField.border.right = 4;
							UIUtils.MainSkin.textField.border.top = 4;
							UIUtils.MainSkin.textField.border.bottom = 4;

							nodeStyleOff.border.left = 6;
							nodeStyleOff.border.right = 6;
							nodeStyleOff.border.top = 6;
							nodeStyleOff.border.bottom = 6;

							nodeStyleOn.border.left = 6;
							nodeStyleOn.border.right = 6;
							nodeStyleOn.border.top = 6;
							nodeStyleOn.border.bottom = 6;

							nodeTitle.border.left = 6;
							nodeTitle.border.right = 6;
							nodeTitle.border.top = 6;
							nodeTitle.border.bottom = 4;

							commentaryBackground.border.left = 6;
							commentaryBackground.border.right = 6;
							commentaryBackground.border.top = 6;
							commentaryBackground.border.bottom = 6;
						}
						break;
						case NodeLOD.LOD1:
						{
							UIUtils.MainSkin.textField.border.left = 2;
							UIUtils.MainSkin.textField.border.right = 2;
							UIUtils.MainSkin.textField.border.top = 2;
							UIUtils.MainSkin.textField.border.bottom = 2;

							nodeStyleOff.border.left = 5;
							nodeStyleOff.border.right = 5;
							nodeStyleOff.border.top = 5;
							nodeStyleOff.border.bottom = 5;

							nodeStyleOn.border.left = 5;
							nodeStyleOn.border.right = 5;
							nodeStyleOn.border.top = 5;
							nodeStyleOn.border.bottom = 5;

							nodeTitle.border.left = 5;
							nodeTitle.border.right = 5;
							nodeTitle.border.top = 5;
							nodeTitle.border.bottom = 2;

							commentaryBackground.border.left = 5;
							commentaryBackground.border.right = 5;
							commentaryBackground.border.top = 5;
							commentaryBackground.border.bottom = 5;
						}
						break;
						case NodeLOD.LOD2:
						{
							UIUtils.MainSkin.textField.border.left = 1;
							UIUtils.MainSkin.textField.border.right = 1;
							UIUtils.MainSkin.textField.border.top = 1;
							UIUtils.MainSkin.textField.border.bottom = 1;

							nodeStyleOff.border.left = 2;
							nodeStyleOff.border.right = 2;
							nodeStyleOff.border.top = 2;
							nodeStyleOff.border.bottom = 3;

							nodeStyleOn.border.left = 4;
							nodeStyleOn.border.right = 4;
							nodeStyleOn.border.top = 4;
							nodeStyleOn.border.bottom = 3;

							nodeTitle.border.left = 2;
							nodeTitle.border.right = 2;
							nodeTitle.border.top = 2;
							nodeTitle.border.bottom = 2;

							commentaryBackground.border.left = 2;
							commentaryBackground.border.right = 2;
							commentaryBackground.border.top = 2;
							commentaryBackground.border.bottom = 3;
						}
						break;
						case NodeLOD.LOD3:
						case NodeLOD.LOD4:
						case NodeLOD.LOD5:
						{
							UIUtils.MainSkin.textField.border.left = 0;
							UIUtils.MainSkin.textField.border.right = 0;
							UIUtils.MainSkin.textField.border.top = 0;
							UIUtils.MainSkin.textField.border.bottom = 0;

							nodeStyleOff.border.left = 1;
							nodeStyleOff.border.right = 1;
							nodeStyleOff.border.top = 1;
							nodeStyleOff.border.bottom = 2;

							nodeStyleOn.border.left = 2;
							nodeStyleOn.border.right = 2;
							nodeStyleOn.border.top = 2;
							nodeStyleOn.border.bottom = 2;

							nodeTitle.border.left = 1;
							nodeTitle.border.right = 1;
							nodeTitle.border.top = 1;
							nodeTitle.border.bottom = 1;

							commentaryBackground.border.left = 1;
							commentaryBackground.border.right = 1;
							commentaryBackground.border.top = 1;
							commentaryBackground.border.bottom = 2;
						}
						break;
					}
				}
			}

			m_visibleNodes.Clear();
			//int nullCount = 0;
			m_hasUnConnectedNodes = false;
			bool repaint = false;
			Material currentMaterial = masterNode != null ? masterNode.CurrentMaterial : null;
			EditorGUI.BeginChangeCheck();
			bool repaintMaterialInspector = false;

			// Dont use nodeCount variable because node count can change in this loop???
			int nodeCount = m_nodes.Count;
			ParentNode node = null;
			for( int i = 0; i < nodeCount; i++ )
			{
				node = m_nodes[ i ];
				if( !node.IsOnGrid )
				{
					m_nodeGrid.AddNodeToGrid( node );
				}

				node.MovingInFrame = false;

				if( drawInfo.CurrentEventType == EventType.Repaint )
					node.OnNodeLayout( drawInfo );

				m_hasUnConnectedNodes = m_hasUnConnectedNodes ||
										( node.ConnStatus != NodeConnectionStatus.Connected && node.ConnStatus != NodeConnectionStatus.Island );

				if( node.RequireMaterialUpdate && currentMaterial != null )
				{
					node.UpdateMaterial( currentMaterial );
					repaintMaterialInspector = true;
				}

				if( node.IsVisible )
					m_visibleNodes.Add( node );

				IsDirty = ( m_isDirty || node.IsDirty );
				SaveIsDirty = ( m_saveIsDirty || node.SaveIsDirty );
			}

			// Handles GUI controls
			nodeCount = m_nodes.Count;
			for( int i = nodeCount - 1; i >= 0; i-- )
			//for ( int i = 0; i < nodeCount; i++ )
			{
				node = m_nodes[ i ];
				bool restoreMouse = false;
				if( drawInfo.CurrentEventType == EventType.mouseDown && m_nodeClicked > -1 && node.UniqueId != m_nodeClicked )
				{
					restoreMouse = true;
					drawInfo.CurrentEventType = EventType.ignore;
				}

				node.DrawGUIControls( drawInfo );

				if( restoreMouse )
				{
					drawInfo.CurrentEventType = EventType.mouseDown;
				}
			}

			// Draw connection wires
			if( drawInfo.CurrentEventType == EventType.Repaint )
				DrawWires( ParentWindow.WireTexture, drawInfo, ParentWindow.WindowContextPallete.IsActive, ParentWindow.WindowContextPallete.CurrentPosition );

			// Master Draw
			nodeCount = m_nodes.Count;
			for( int i = 0; i < nodeCount; i++ )
			{
				node = m_nodes[ i ];
				bool restoreMouse = false;
				if( drawInfo.CurrentEventType == EventType.mouseDown && m_nodeClicked > -1 && node.UniqueId != m_nodeClicked )
				{
					restoreMouse = true;
					drawInfo.CurrentEventType = EventType.ignore;
				}

				node.Draw( drawInfo );

				if( restoreMouse )
				{
					drawInfo.CurrentEventType = EventType.mouseDown;
				}
			}

			// Draw Tooltip
			if( drawInfo.CurrentEventType == EventType.Repaint || drawInfo.CurrentEventType == EventType.mouseDown )
			{
				int visibleCount = m_visibleNodes.Count;
				for( int i = visibleCount - 1; i >= 0; i-- )
				{
					if( !m_visibleNodes[ i ].IsMoving )
					{
						bool showing = m_visibleNodes[ i ].ShowTooltip( drawInfo );
						if( showing )
							break;
					}
				}
			}

			if( repaintMaterialInspector )
			{
				if( ASEMaterialInspector.Instance != null )
				{
					ASEMaterialInspector.Instance.Repaint();
				}
			}

			if( m_checkSelectedWireHighlights )
			{
				m_checkSelectedWireHighlights = false;
				ResetHighlightedWires();
				for( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					HighlightWiresStartingNode( m_selectedNodes[ i ] );
				}
			}

			if( EditorGUI.EndChangeCheck() )
			{
				repaint = true;
				SaveIsDirty = true;
			}

			if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				// Revert LOD changes to LOD0 (only if it's different)
				if( UIUtils.MainSkin.textField.border.left != 4 )
				{
					UIUtils.MainSkin.textField.border.left = 4;
					UIUtils.MainSkin.textField.border.right = 4;
					UIUtils.MainSkin.textField.border.top = 4;
					UIUtils.MainSkin.textField.border.bottom = 4;

					nodeStyleOff.border.left = 6;
					nodeStyleOff.border.right = 6;
					nodeStyleOff.border.top = 6;
					nodeStyleOff.border.bottom = 6;

					nodeStyleOn.border.left = 6;
					nodeStyleOn.border.right = 6;
					nodeStyleOn.border.top = 6;
					nodeStyleOn.border.bottom = 6;

					nodeTitle.border.left = 6;
					nodeTitle.border.right = 6;
					nodeTitle.border.top = 6;
					nodeTitle.border.bottom = 4;

					commentaryBackground.border.left = 6;
					commentaryBackground.border.right = 6;
					commentaryBackground.border.top = 6;
					commentaryBackground.border.bottom = 6;
				}
			}

			//if ( nullCount == m_nodes.Count )
			//	m_nodes.Clear();

			ChangedLightingModel = false;

			return repaint;
		}

		public bool UpdateMarkForDeletion()
		{
			if( m_markedForDeletion.Count != 0 )
			{
				DeleteMarkedForDeletionNodes();
				return true;
			}
			return false;
		}

		public void DrawWires( Texture2D wireTex, DrawInfo drawInfo, bool contextPaletteActive, Vector3 contextPalettePos )
		{
			//Handles.BeginGUI();
			//Debug.Log(GUI.depth);
			// Draw connected node wires
			m_wireBezierCount = 0;
			for( int nodeIdx = 0; nodeIdx < m_nodes.Count; nodeIdx++ )
			{
				ParentNode node = m_nodes[ nodeIdx ];
				if( ( object ) node == null )
					return;

				for( int inputPortIdx = 0; inputPortIdx < node.InputPorts.Count; inputPortIdx++ )
				{
					InputPort inputPort = node.InputPorts[ inputPortIdx ];
					if( inputPort.ExternalReferences.Count > 0 )
					{
						bool cleanInvalidConnections = false;
						for( int wireIdx = 0; wireIdx < inputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference reference = inputPort.ExternalReferences[ wireIdx ];
							if( reference.NodeId != -1 && reference.PortId != -1 )
							{
								ParentNode outputNode = GetNode( reference.NodeId );
								if( outputNode != null )
								{
									OutputPort outputPort = outputNode.GetOutputPortByUniqueId( reference.PortId );
									Vector3 endPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
									Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );
									float x = ( startPos.x < endPos.x ) ? startPos.x : endPos.x;
									float y = ( startPos.y < endPos.y ) ? startPos.y : endPos.y;
									float width = Mathf.Abs( startPos.x - endPos.x ) + outputPort.Position.width;
									float height = Mathf.Abs( startPos.y - endPos.y ) + outputPort.Position.height;
									Rect portsBoundingBox = new Rect( x, y, width, height );

									bool isVisible = node.IsVisible || outputNode.IsVisible;
									if( !isVisible )
									{
										isVisible = drawInfo.TransformedCameraArea.Overlaps( portsBoundingBox );
									}

									if( isVisible )
									{

										Rect bezierBB = DrawBezier( drawInfo.InvertedZoom, startPos, endPos, inputPort.DataType, outputPort.DataType, reference.WireStatus, wireTex, node, outputNode );
										bezierBB.x -= Constants.OUTSIDE_WIRE_MARGIN;
										bezierBB.y -= Constants.OUTSIDE_WIRE_MARGIN;

										bezierBB.width += Constants.OUTSIDE_WIRE_MARGIN * 2;
										bezierBB.height += Constants.OUTSIDE_WIRE_MARGIN * 2;

										if( m_wireBezierCount < m_bezierReferences.Count )
										{
											m_bezierReferences[ m_wireBezierCount ].UpdateInfo( ref bezierBB, inputPort.NodeId, inputPort.PortId, outputPort.NodeId, outputPort.PortId );
										}
										else
										{
											m_bezierReferences.Add( new WireBezierReference( ref bezierBB, inputPort.NodeId, inputPort.PortId, outputPort.NodeId, outputPort.PortId ) );
										}
										m_wireBezierCount++;

									}
								}
								else
								{
									if( DebugConsoleWindow.DeveloperMode )
										UIUtils.ShowMessage( "Detected Invalid connection from node " + node.UniqueId + " port " + inputPortIdx + " to Node " + reference.NodeId + " port " + reference.PortId, MessageSeverity.Error );
									cleanInvalidConnections = true;
									inputPort.ExternalReferences[ wireIdx ].Invalidate();
								}
							}
						}

						if( cleanInvalidConnections )
						{
							inputPort.RemoveInvalidConnections();
						}
					}
				}
			}

			//Draw selected wire
			if( m_parentWindow.WireReferenceUtils.ValidReferences() )
			{
				if( m_parentWindow.WireReferenceUtils.InputPortReference.IsValid )
				{
					InputPort inputPort = GetNode( m_parentWindow.WireReferenceUtils.InputPortReference.NodeId ).GetInputPortByUniqueId( m_parentWindow.WireReferenceUtils.InputPortReference.PortId );
					Vector3 endPos = Vector3.zero;
					if( m_parentWindow.WireReferenceUtils.SnapEnabled )
					{
						Vector2 pos = ( m_parentWindow.WireReferenceUtils.SnapPosition + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
						endPos = new Vector3( pos.x, pos.y ) + UIUtils.ScaledPortsDelta;
					}
					else
					{
						endPos = contextPaletteActive ? contextPalettePos : new Vector3( Event.current.mousePosition.x, Event.current.mousePosition.y );
					}

					Vector3 startPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
					DrawBezier( drawInfo.InvertedZoom, endPos, startPos, inputPort.DataType, inputPort.DataType, WireStatus.Default, wireTex );
				}

				if( m_parentWindow.WireReferenceUtils.OutputPortReference.IsValid )
				{
					OutputPort outputPort = GetNode( m_parentWindow.WireReferenceUtils.OutputPortReference.NodeId ).GetOutputPortByUniqueId( m_parentWindow.WireReferenceUtils.OutputPortReference.PortId );
					Vector3 endPos = Vector3.zero;
					if( m_parentWindow.WireReferenceUtils.SnapEnabled )
					{
						Vector2 pos = ( m_parentWindow.WireReferenceUtils.SnapPosition + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
						endPos = new Vector3( pos.x, pos.y ) + UIUtils.ScaledPortsDelta;
					}
					else
					{
						endPos = contextPaletteActive ? contextPalettePos : new Vector3( Event.current.mousePosition.x, Event.current.mousePosition.y );
					}
					Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );
					DrawBezier( drawInfo.InvertedZoom, startPos, endPos, outputPort.DataType, outputPort.DataType, WireStatus.Default, wireTex );
				}
			}
			//Handles.EndGUI();
		}

		Rect DrawBezier( float invertedZoom, Vector3 startPos, Vector3 endPos, WirePortDataType inputDataType, WirePortDataType outputDataType, WireStatus wireStatus, Texture2D wireTex, ParentNode inputNode = null, ParentNode outputNode = null )
		{
			startPos += UIUtils.ScaledPortsDelta;
			endPos += UIUtils.ScaledPortsDelta;

			float wiresThickness =/* drawInfo.InvertedZoom * */Constants.WIRE_WIDTH;

			// Calculate the 4 points for bezier taking into account wire nodes and their automatic tangents
			float mag = ( endPos - startPos ).magnitude;
			float resizedMag = Mathf.Min( mag, Constants.HORIZONTAL_TANGENT_SIZE * invertedZoom );

			Vector3 startTangent = new Vector3( startPos.x + resizedMag, startPos.y );
			Vector3 endTangent = new Vector3( endPos.x - resizedMag, endPos.y );

			if( ( object ) inputNode != null && inputNode.GetType() == typeof( WireNode ) )
				endTangent = endPos + ( ( inputNode as WireNode ).TangentDirection ) * mag * 0.33f;

			if( ( object ) outputNode != null && outputNode.GetType() == typeof( WireNode ) )
				startTangent = startPos - ( ( outputNode as WireNode ).TangentDirection ) * mag * 0.33f;

			///////////////Draw tangents
			//Rect box1 = new Rect( new Vector2( startTangent.x, startTangent.y ), new Vector2( 10, 10 ) );
			//box1.x -= box1.width * 0.5f;
			//box1.y -= box1.height * 0.5f;
			//GUI.Box( box1, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );

			//Rect box2 = new Rect( new Vector2( endTangent.x, endTangent.y ), new Vector2( 10, 10 ) );
			//box2.x -= box2.width * 0.5f;
			//box2.y -= box2.height * 0.5f;
			//GUI.Box( box2, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );

			//m_auxRect.Set( 0, 0, UIUtils.CurrentWindow.position.width, UIUtils.CurrentWindow.position.height );
			//GLDraw.BeginGroup( m_auxRect );

			int ty = 1;
			float wireThickness = 0;


			if( ParentWindow.Options.MultiLinePorts )
			{
				GLDraw.MultiLine = true;
				Shader.SetGlobalFloat( "_InvertedZoom", invertedZoom );
				switch( outputDataType )
				{
					case WirePortDataType.FLOAT2: ty = 2; break;
					case WirePortDataType.FLOAT3: ty = 3; break;
					case WirePortDataType.FLOAT4:
					case WirePortDataType.COLOR:
					{
						ty = 4;
					}
					break;
					default: ty = 1; break;
				}
				wireThickness = Mathf.Lerp( wiresThickness * ( ty * invertedZoom * -0.05f + 0.15f ), wiresThickness * ( ty * invertedZoom * 0.175f + 0.3f ), invertedZoom + 0.4f );
			}
			else
			{
				GLDraw.MultiLine = false;
				wireThickness = Mathf.Lerp( wiresThickness * ( invertedZoom * -0.05f + 0.15f ), wiresThickness * ( invertedZoom * 0.175f + 0.3f ), invertedZoom + 0.4f );
			}

			Rect boundBox = new Rect();
			int segments = 11;
			if( LodLevel <= ParentGraph.NodeLOD.LOD4 )
				segments = Mathf.Clamp( Mathf.FloorToInt( mag * 0.2f * invertedZoom ), 11, 35 );
			else
				segments = ( int ) ( invertedZoom * 14.28f * 11 );

			if( ParentWindow.Options.ColoredPorts && wireStatus != WireStatus.Highlighted )
				boundBox = GLDraw.DrawBezier( startPos, startTangent, endPos, endTangent, UIUtils.GetColorForDataType( outputDataType, false, false ), UIUtils.GetColorForDataType( inputDataType, false, false ), wireThickness, segments, ty );
			else
				boundBox = GLDraw.DrawBezier( startPos, startTangent, endPos, endTangent, UIUtils.GetColorFromWireStatus( wireStatus ), wireThickness, segments, ty );
			//GLDraw.EndGroup();

			//GUI.Box( m_auxRect, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );
			//GUI.Box( boundBox, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );
			//if ( UIUtils.CurrentWindow.Options.ColoredPorts && wireStatus != WireStatus.Highlighted )
			//	Handles.DrawBezier( startPos, endPos, startTangent, endTangent, UIUtils.GetColorForDataType( outputDataType, false, false ), wireTex, wiresTickness );
			//else
			//	Handles.DrawBezier( startPos, endPos, startTangent, endTangent, UIUtils.GetColorFromWireStatus( wireStatus ), wireTex, wiresTickness );

			//Handles.DrawLine( startPos, startTangent );
			//Handles.DrawLine( endPos, endTangent );

			float extraBound = 30 * invertedZoom;
			boundBox.xMin -= extraBound;
			boundBox.xMax += extraBound;
			boundBox.yMin -= extraBound;
			boundBox.yMax += extraBound;

			return boundBox;
		}

		public void DrawBezierBoundingBox()
		{
			for( int i = 0; i < m_wireBezierCount; i++ )
			{
				m_bezierReferences[ i ].DebugDraw();
			}
		}

		public WireBezierReference GetWireBezierInPos( Vector2 position )
		{
			for( int i = 0; i < m_wireBezierCount; i++ )
			{
				if( m_bezierReferences[ i ].Contains( position ) )
					return m_bezierReferences[ i ];
			}
			return null;
		}


		public List<WireBezierReference> GetWireBezierListInPos( Vector2 position )
		{
			List<WireBezierReference> list = new List<WireBezierReference>();
			for( int i = 0; i < m_wireBezierCount; i++ )
			{
				if( m_bezierReferences[ i ].Contains( position ) )
					list.Add( m_bezierReferences[ i ] );
			}

			return list;
		}


		public void MoveSelectedNodes( Vector2 delta, bool snap = false )
		{
			//bool validMovement = delta.magnitude > 0.001f;
			//if ( validMovement )
			//{
			//	Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoMoveNodesId );
			//	for ( int i = 0; i < m_selectedNodes.Count; i++ )
			//	{
			//		if ( !m_selectedNodes[ i ].MovingInFrame )
			//		{
			//			Undo.RecordObject( m_selectedNodes[ i ], Constants.UndoMoveNodesId );
			//			m_selectedNodes[ i ].Move( delta, snap );
			//		}
			//	}
			//	IsDirty = true;
			//}

			bool performUndo = delta.magnitude > 0.01f;
			if( performUndo )
				Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoMoveNodesId );
			for( int i = 0; i < m_selectedNodes.Count; i++ )
			{
				if( !m_selectedNodes[ i ].MovingInFrame )
				{
					if( performUndo )
						Undo.RecordObject( m_selectedNodes[ i ], Constants.UndoMoveNodesId );
					m_selectedNodes[ i ].Move( delta, snap );
				}
			}
			IsDirty = true;
		}

		public void SetConnection( int InNodeId, int InPortId, int OutNodeId, int OutPortId )
		{
			ParentNode inNode = GetNode( InNodeId );
			ParentNode outNode = GetNode( OutNodeId );
			InputPort inputPort = null;
			OutputPort outputPort = null;
			if( inNode != null && outNode != null )
			{
				inputPort = inNode.GetInputPortByUniqueId( InPortId );
				outputPort = outNode.GetOutputPortByUniqueId( OutPortId );
				if( inputPort != null && outputPort != null )
				{
					if( inputPort.IsConnectedTo( OutNodeId, OutPortId ) || outputPort.IsConnectedTo( InNodeId, InPortId ) )
					{
						if( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowMessage( "Node/Port already connected " + InNodeId, MessageSeverity.Error );
						return;
					}

					if( !inputPort.CheckValidType( outputPort.DataType ) )
					{
						if( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowIncompatiblePortMessage( true, inNode, inputPort, outNode, outputPort );
						return;
					}

					if( !outputPort.CheckValidType( inputPort.DataType ) )
					{

						if( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowIncompatiblePortMessage( false, outNode, outputPort, inNode, inputPort );
						return;
					}
					if( !inputPort.Available || !outputPort.Available )
					{
						if( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowMessage( "Ports not available to connection", MessageSeverity.Warning );

						return;
					}

					if( inputPort.ConnectTo( OutNodeId, OutPortId, outputPort.DataType, false ) )
					{
						inNode.OnInputPortConnected( InPortId, OutNodeId, OutPortId );
					}


					if( outputPort.ConnectTo( InNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked ) )
					{
						outNode.OnOutputPortConnected( OutPortId, InNodeId, InPortId );
					}
				}
				else if( ( object ) inputPort == null )
				{
					if( DebugConsoleWindow.DeveloperMode )
						UIUtils.ShowMessage( "Input Port " + InPortId + " doesn't exist on node " + InNodeId, MessageSeverity.Error );
				}
				else
				{
					if( DebugConsoleWindow.DeveloperMode )
						UIUtils.ShowMessage( "Output Port " + OutPortId + " doesn't exist on node " + OutNodeId, MessageSeverity.Error );
				}
			}
			else if( ( object ) inNode == null )
			{
				if( DebugConsoleWindow.DeveloperMode )
					UIUtils.ShowMessage( "Input node " + InNodeId + " doesn't exist", MessageSeverity.Error );
			}
			else
			{
				if( DebugConsoleWindow.DeveloperMode )
					UIUtils.ShowMessage( "Output node " + OutNodeId + " doesn't exist", MessageSeverity.Error );
			}
		}

		public void CreateConnection( int inNodeId, int inPortId, int outNodeId, int outPortId, bool registerUndo = true )
		{
			ParentNode outputNode = GetNode( outNodeId );
			if( outputNode != null )
			{
				OutputPort outputPort = outputNode.OutputPorts[ outPortId ];
				if( outputPort != null )
				{
					ParentNode inputNode = GetNode( inNodeId );
					InputPort inputPort = inputNode.GetInputPortByUniqueId( inPortId );

					if( !inputPort.CheckValidType( outputPort.DataType ) )
					{
						UIUtils.ShowIncompatiblePortMessage( true, inputNode, inputPort, outputNode, outputPort );
						return;
					}

					if( !outputPort.CheckValidType( inputPort.DataType ) )
					{
						UIUtils.ShowIncompatiblePortMessage( false, outputNode, outputPort, inputNode, inputPort );
						return;
					}

					inputPort.DummyAdd( outputPort.NodeId, outputPort.PortId );
					outputPort.DummyAdd( inNodeId, inPortId );

					if( UIUtils.DetectNodeLoopsFrom( inputNode, new Dictionary<int, int>() ) )
					{
						inputPort.DummyRemove();
						outputPort.DummyRemove();
						m_parentWindow.WireReferenceUtils.InvalidateReferences();
						UIUtils.ShowMessage( "Infinite Loop detected" );
						Event.current.Use();
						return;
					}

					inputPort.DummyRemove();
					outputPort.DummyRemove();

					if( inputPort.IsConnected )
					{
						DeleteConnection( true, inNodeId, inPortId, true, false, registerUndo );
					}

					//link output to input
					if( outputPort.ConnectTo( inNodeId, inPortId, inputPort.DataType, inputPort.TypeLocked ) )
						outputNode.OnOutputPortConnected( outputPort.PortId, inNodeId, inPortId );

					//link input to output
					if( inputPort.ConnectTo( outputPort.NodeId, outputPort.PortId, outputPort.DataType, inputPort.TypeLocked ) )
						inputNode.OnInputPortConnected( inPortId, outputNode.UniqueId, outputPort.PortId );

					MarkWireHighlights();
				}
				ParentWindow.ShaderIsModified = true;
			}
		}

		public void DeleteInvalidConnections()
		{
			int count = m_nodes.Count;
			for( int nodeIdx = 0; nodeIdx < count; nodeIdx++ )
			{
				{
					int inputCount = m_nodes[ nodeIdx ].InputPorts.Count;
					for( int inputIdx = 0; inputIdx < inputCount; inputIdx++ )
					{
						if( !m_nodes[ nodeIdx ].InputPorts[ inputIdx ].Visible && m_nodes[ nodeIdx ].InputPorts[ inputIdx ].IsConnected )
						{
							DeleteConnection( true, m_nodes[ nodeIdx ].UniqueId, m_nodes[ nodeIdx ].InputPorts[ inputIdx ].PortId, true, true );
						}
					}
				}
				{
					int outputCount = m_nodes[ nodeIdx ].OutputPorts.Count;
					for( int outputIdx = 0; outputIdx < outputCount; outputIdx++ )
					{
						if( !m_nodes[ nodeIdx ].OutputPorts[ outputIdx ].Visible && m_nodes[ nodeIdx ].OutputPorts[ outputIdx ].IsConnected )
						{
							DeleteConnection( false, m_nodes[ nodeIdx ].UniqueId, m_nodes[ nodeIdx ].OutputPorts[ outputIdx ].PortId, true, true );
						}
					}
				}
			}
		}

		public void DeleteAllConnectionFromNode( int nodeId, bool registerOnLog, bool propagateCallback )
		{
			ParentNode node = GetNode( nodeId );
			if( ( object ) node == null )
				return;
			DeleteAllConnectionFromNode( node, registerOnLog, propagateCallback );
		}

		public void DeleteAllConnectionFromNode( ParentNode node, bool registerOnLog, bool propagateCallback )
		{

			for( int i = 0; i < node.InputPorts.Count; i++ )
			{
				if( node.InputPorts[ i ].IsConnected )
					DeleteConnection( true, node.UniqueId, node.InputPorts[ i ].PortId, registerOnLog, propagateCallback );
			}

			for( int i = 0; i < node.OutputPorts.Count; i++ )
			{
				if( node.OutputPorts[ i ].IsConnected )
					DeleteConnection( false, node.UniqueId, node.OutputPorts[ i ].PortId, registerOnLog, propagateCallback );
			}
		}

		public void DeleteConnection( bool isInput, int nodeId, int portId, bool registerOnLog, bool propagateCallback, bool registerUndo = true )
		{
			ParentNode node = GetNode( nodeId );
			if( ( object ) node == null )
				return;

			if( registerUndo )
			{
				UIUtils.MarkUndoAction();
				Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoDeleteConnectionId );
				Undo.RecordObject( node, Constants.UndoDeleteConnectionId );
			}

			if( isInput )
			{
				InputPort inputPort = node.GetInputPortByUniqueId( portId );
				if( inputPort != null && inputPort.IsConnected )
				{

					if( node.ConnStatus == NodeConnectionStatus.Connected )
					{
						inputPort.GetOutputNode().DeactivateNode( portId, false );
						m_checkSelectedWireHighlights = true;
					}

					for( int i = 0; i < inputPort.ExternalReferences.Count; i++ )
					{
						WireReference inputReference = inputPort.ExternalReferences[ i ];
						ParentNode outputNode = GetNode( inputReference.NodeId );
						if( registerUndo )
							Undo.RecordObject( outputNode, Constants.UndoDeleteConnectionId );
						outputNode.GetOutputPortByUniqueId( inputReference.PortId ).InvalidateConnection( inputPort.NodeId, inputPort.PortId );
						if( propagateCallback )
							outputNode.OnOutputPortDisconnected( inputReference.PortId );
					}
					inputPort.InvalidateAllConnections();
					if( propagateCallback )
						node.OnInputPortDisconnected( portId );
				}
			}
			else
			{
				OutputPort outputPort = node.GetOutputPortByUniqueId( portId );
				if( outputPort != null && outputPort.IsConnected )
				{
					if( propagateCallback )
						node.OnOutputPortDisconnected( portId );

					for( int i = 0; i < outputPort.ExternalReferences.Count; i++ )
					{
						WireReference outputReference = outputPort.ExternalReferences[ i ];
						ParentNode inputNode = GetNode( outputReference.NodeId );
						if( registerUndo )
							Undo.RecordObject( inputNode, Constants.UndoDeleteConnectionId );
						if( inputNode.ConnStatus == NodeConnectionStatus.Connected )
						{
							node.DeactivateNode( portId, false );
							m_checkSelectedWireHighlights = true;
						}
						inputNode.GetInputPortByUniqueId( outputReference.PortId ).InvalidateConnection( outputPort.NodeId, outputPort.PortId );
						if( propagateCallback )
							inputNode.OnInputPortDisconnected( outputReference.PortId );
					}
					outputPort.InvalidateAllConnections();
				}
			}
			IsDirty = true;
		}

		public void DeleteSelectedNodes()
		{
			bool invalidateMasterNode = false;
			int count = m_selectedNodes.Count;
			for( int nodeIdx = 0; nodeIdx < count; nodeIdx++ )
			{
				ParentNode node = m_selectedNodes[ nodeIdx ];
				if( node.UniqueId == m_masterNodeId )
				{
					invalidateMasterNode = true;
				}
				else
				{
					DestroyNode( node );
				}
			}

			if( invalidateMasterNode )
			{
				CurrentOutputNode.Selected = false;
			}
			//Clear all references
			m_selectedNodes.Clear();
			IsDirty = true;
		}

		public void DeleteNodesOnArray( ref ParentNode[] nodeArray )
		{
			bool invalidateMasterNode = false;
			for( int nodeIdx = 0; nodeIdx < nodeArray.Length; nodeIdx++ )
			{
				ParentNode node = nodeArray[ nodeIdx ];
				if( node.UniqueId == m_masterNodeId )
				{
					FunctionOutput fout = node as FunctionOutput;
					if( fout != null )
					{
						for( int i = 0; i < m_nodes.Count; i++ )
						{
							FunctionOutput secondfout = m_nodes[ i ] as FunctionOutput;
							if( secondfout != null && secondfout != fout )
							{
								secondfout.Function = fout.Function;
								AssignMasterNode( secondfout, false );

								DeselectNode( fout );
								DestroyNode( fout );
								break;
							}
						}
					}
					invalidateMasterNode = true;
				}
				else
				{
					DeselectNode( node );
					DestroyNode( node );
				}
				nodeArray[ nodeIdx ] = null;
			}

			if( invalidateMasterNode && CurrentMasterNode != null )
			{
				CurrentMasterNode.Selected = false;
			}

			//Clear all references
			nodeArray = null;
			IsDirty = true;
		}

		public void MarkWireNodeSequence( WireNode node, bool isInput )
		{
			if( node == null )
			{
				return;
			}

			if( m_markedForDeletion.Contains( node ) )
				return;

			m_markedForDeletion.Add( node );

			if( isInput && node.InputPorts[ 0 ].IsConnected )
			{
				MarkWireNodeSequence( GetNode( node.InputPorts[ 0 ].ExternalReferences[ 0 ].NodeId ) as WireNode, isInput );
			}
			else if( !isInput && node.OutputPorts[ 0 ].IsConnected )
			{
				MarkWireNodeSequence( GetNode( node.OutputPorts[ 0 ].ExternalReferences[ 0 ].NodeId ) as WireNode, isInput );
			}
		}

		public void UndoableDeleteSelectedNodes( List<ParentNode> nodeList )
		{
			if( nodeList.Count == 0 )
				return;

			List<ParentNode> validNode = new List<ParentNode>();

			for( int i = 0; i < nodeList.Count; i++ )
			{
				if( nodeList[ i ] != null && nodeList[ i ].UniqueId != m_masterNodeId )
				{
					validNode.Add( nodeList[ i ] );
				}
			}
			UIUtils.ClearUndoHelper();
			ParentNode[] selectedNodes = new ParentNode[ validNode.Count ];
			for( int i = 0; i < selectedNodes.Length; i++ )
			{
				if( validNode[ i ] != null )
				{
					selectedNodes[ i ] = validNode[ i ];
					UIUtils.CheckUndoNode( selectedNodes[ i ] );
				}
			}

			//Check nodes connected to deleted nodes to preserve connections on undo
			List<ParentNode> extraNodes = new List<ParentNode>();
			for( int selectedNodeIdx = 0; selectedNodeIdx < selectedNodes.Length; selectedNodeIdx++ )
			{
				// Check inputs
				if( selectedNodes[ selectedNodeIdx ] != null )
				{
					int inputIdxCount = selectedNodes[ selectedNodeIdx ].InputPorts.Count;
					if( inputIdxCount > 0 )
					{
						for( int inputIdx = 0; inputIdx < inputIdxCount; inputIdx++ )
						{
							if( selectedNodes[ selectedNodeIdx ].InputPorts[ inputIdx ].IsConnected )
							{
								int nodeIdx = selectedNodes[ selectedNodeIdx ].InputPorts[ inputIdx ].ExternalReferences[ 0 ].NodeId;
								if( nodeIdx > -1 )
								{
									ParentNode node = GetNode( nodeIdx );
									if( node != null && UIUtils.CheckUndoNode( node ) )
									{
										extraNodes.Add( node );
									}
								}
							}
						}
					}
				}

				// Check outputs
				if( selectedNodes[ selectedNodeIdx ] != null )
				{
					int outputIdxCount = selectedNodes[ selectedNodeIdx ].OutputPorts.Count;
					if( outputIdxCount > 0 )
					{
						for( int outputIdx = 0; outputIdx < outputIdxCount; outputIdx++ )
						{
							int inputIdxCount = selectedNodes[ selectedNodeIdx ].OutputPorts[ outputIdx ].ExternalReferences.Count;
							if( inputIdxCount > 0 )
							{
								for( int inputIdx = 0; inputIdx < inputIdxCount; inputIdx++ )
								{
									int nodeIdx = selectedNodes[ selectedNodeIdx ].OutputPorts[ outputIdx ].ExternalReferences[ inputIdx ].NodeId;
									if( nodeIdx > -1 )
									{
										ParentNode node = GetNode( nodeIdx );
										if( UIUtils.CheckUndoNode( node ) )
										{
											extraNodes.Add( node );
										}
									}
								}
							}
						}
					}

				}
			}

			UIUtils.ClearUndoHelper();

			//Record deleted nodes
			UIUtils.MarkUndoAction();
			Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoDeleteNodeId );
			Undo.RecordObjects( selectedNodes, Constants.UndoDeleteNodeId );
			Undo.RecordObjects( extraNodes.ToArray(), Constants.UndoDeleteNodeId );
			//Record deleting connections
			for( int i = 0; i < selectedNodes.Length; i++ )
			{
				CurrentOutputNode.Selected = false;
				DeleteAllConnectionFromNode( selectedNodes[ i ], false, true );
			}
			//Delete
			DeleteNodesOnArray( ref selectedNodes );

			extraNodes.Clear();
			extraNodes = null;

			EditorUtility.SetDirty( ParentWindow );

			ParentWindow.ForceRepaint();
		}


		public void DeleteMarkedForDeletionNodes()
		{
			UndoableDeleteSelectedNodes( m_markedForDeletion );
			m_markedForDeletion.Clear();
			IsDirty = true;

			//bool invalidateMasterNode = false;
			//int count = m_markedForDeletion.Count;
			//for ( int nodeIdx = 0; nodeIdx < count; nodeIdx++ )
			//{
			//	ParentNode node = m_markedForDeletion[ nodeIdx ];
			//	if ( node.UniqueId == m_masterNodeId )
			//	{
			//		invalidateMasterNode = true;
			//	}
			//	else
			//	{
			//		if ( node.Selected )
			//		{
			//			m_selectedNodes.Remove( node );
			//			node.Selected = false;
			//		}
			//		DestroyNode( node );
			//	}
			//}

			//if ( invalidateMasterNode )
			//{
			//	CurrentMasterNode.Selected = false;
			//}
			////Clear all references
			//m_markedForDeletion.Clear();
			//IsDirty = true;
		}

		public void DestroyNode( int nodeId )
		{
			ParentNode node = GetNode( nodeId );
			DestroyNode( node );
		}

		public void DestroyNode( ParentNode node, bool registerUndo = true )
		{
			if( node == null )
			{
				UIUtils.ShowMessage( "Attempting to destroying a inexistant node ", MessageSeverity.Warning );
				return;
			}

			if( node.ConnStatus == NodeConnectionStatus.Connected && !m_checkSelectedWireHighlights )
			{
				ResetHighlightedWires();
				m_checkSelectedWireHighlights = true;
			}

			//TODO: check better placement of this code (reconnects wires from wire nodes)
			//if ( node.GetType() == typeof( WireNode ) )
			//{
			//	if ( node.InputPorts[ 0 ].ExternalReferences != null && node.InputPorts[ 0 ].ExternalReferences.Count > 0 )
			//	{
			//		WireReference backPort = node.InputPorts[ 0 ].ExternalReferences[ 0 ];
			//		for ( int i = 0; i < node.OutputPorts[ 0 ].ExternalReferences.Count; i++ )
			//		{
			//			UIUtils.CurrentWindow.ConnectInputToOutput( node.OutputPorts[ 0 ].ExternalReferences[ i ].NodeId, node.OutputPorts[ 0 ].ExternalReferences[ i ].PortId, backPort.NodeId, backPort.PortId );
			//		}
			//	}
			//}

			if( node.UniqueId != m_masterNodeId )
			{
				m_nodeGrid.RemoveNodeFromGrid( node, false );
				//Send Deactivation signal if active
				if( node.ConnStatus == NodeConnectionStatus.Connected )
				{
					node.DeactivateNode( -1, true );
				}

				//Invalidate references
				//Invalidate input references
				for( int inputPortIdx = 0; inputPortIdx < node.InputPorts.Count; inputPortIdx++ )
				{
					InputPort inputPort = node.InputPorts[ inputPortIdx ];
					if( inputPort.IsConnected )
					{
						for( int wireIdx = 0; wireIdx < inputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference inputReference = inputPort.ExternalReferences[ wireIdx ];
							ParentNode outputNode = GetNode( inputReference.NodeId );
							outputNode.GetOutputPortByUniqueId( inputReference.PortId ).InvalidateConnection( inputPort.NodeId, inputPort.PortId );
							outputNode.OnOutputPortDisconnected( inputReference.PortId );
						}
						inputPort.InvalidateAllConnections();
					}
				}

				//Invalidate output reference
				for( int outputPortIdx = 0; outputPortIdx < node.OutputPorts.Count; outputPortIdx++ )
				{
					OutputPort outputPort = node.OutputPorts[ outputPortIdx ];
					if( outputPort.IsConnected )
					{
						for( int wireIdx = 0; wireIdx < outputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference outputReference = outputPort.ExternalReferences[ wireIdx ];
							ParentNode outnode = GetNode( outputReference.NodeId );
							if( outnode != null )
							{
								outnode.GetInputPortByUniqueId( outputReference.PortId ).InvalidateConnection( outputPort.NodeId, outputPort.PortId );
								outnode.OnInputPortDisconnected( outputReference.PortId );
							}
						}
						outputPort.InvalidateAllConnections();
					}
				}

				//Remove node from main list
				//Undo.RecordObject( node, "Destroying node " + ( node.Attributes != null? node.Attributes.Name: node.GetType().ToString() ) );
				if( registerUndo )
				{
					UIUtils.MarkUndoAction();
					Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoDeleteNodeId );
					Undo.RecordObject( node, Constants.UndoDeleteNodeId );
				}

				if( OnNodeRemovedEvent != null )
					OnNodeRemovedEvent( node );

				m_nodes.Remove( node );
				m_nodesDict.Remove( node.UniqueId );
				node.Destroy();
				if( registerUndo )
					Undo.DestroyObjectImmediate( node );
				else
					ScriptableObject.DestroyImmediate( node );
				IsDirty = true;
				m_markToReOrder = true;
			}
			//else if( node.UniqueId == m_masterNodeId && node.GetType() == typeof(FunctionOutput) )
			//{
			//	Debug.Log( "Attempting to destroy a output node" );
			//	DeselectNode( node );
			//	UIUtils.ShowMessage( "Attempting to destroy a output node" );
			//}
			else
			{
				DeselectNode( node );
				UIUtils.ShowMessage( "Attempting to destroy a master node" );
			}
		}

		void AddToSelectedNodes( ParentNode node )
		{
			node.Selected = true;
			m_selectedNodes.Add( node );
			node.OnNodeStoppedMovingEvent += OnNodeFinishMoving;
			if( node.ConnStatus == NodeConnectionStatus.Connected )
			{
				HighlightWiresStartingNode( node );
			}
		}

		void RemoveFromSelectedNodes( ParentNode node )
		{
			node.Selected = false;
			m_selectedNodes.Remove( node );
			node.OnNodeStoppedMovingEvent -= OnNodeFinishMoving;
		}

		public void SelectNode( ParentNode node, bool append, bool reorder )
		{
			if( node == null )
				return;

			if( append )
			{
				if( !m_selectedNodes.Contains( node ) )
				{
					AddToSelectedNodes( node );
				}
			}
			else
			{
				DeSelectAll();
				AddToSelectedNodes( node );
			}
			if( reorder && !node.ReorderLocked )
			{
				m_nodes.Remove( node );
				m_nodes.Add( node );
				m_markToReOrder = true;
			}
		}

		public void MultipleSelection( Rect selectionArea, bool append, bool reorder )
		{
			if( !append )
				DeSelectAll();

			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( !m_nodes[ i ].Selected && selectionArea.Overlaps( m_nodes[ i ].Position, true ) )
				//if ( !m_nodes[ i ].Selected && selectionArea.Includes( m_nodes[ i ].Position ) )
				{
					m_nodes[ i ].Selected = true;
					AddToSelectedNodes( m_nodes[ i ] );
				}
			}
			if( reorder )
			{
				for( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					if( !m_selectedNodes[ i ].ReorderLocked )
					{
						m_nodes.Remove( m_selectedNodes[ i ] );
						m_nodes.Add( m_selectedNodes[ i ] );
						m_markToReOrder = true;
					}
				}
			}
		}

		public void SelectAll()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( !m_nodes[ i ].Selected )
					AddToSelectedNodes( m_nodes[ i ] );
			}
		}

		public void SelectMasterNode()
		{
			if( m_masterNodeId != Constants.INVALID_NODE_ID )
			{
				SelectNode( CurrentMasterNode, false, false );
			}
		}

		public void SelectOutputNode()
		{
			if( m_masterNodeId != Constants.INVALID_NODE_ID )
			{
				SelectNode( CurrentOutputNode, false, false );
			}
		}

		public void DeselectNode( int nodeId )
		{
			ParentNode node = GetNode( nodeId );
			if( node )
			{
				m_selectedNodes.Remove( node );
				node.Selected = false;
			}
		}

		public void DeselectNode( ParentNode node )
		{
			m_selectedNodes.Remove( node );
			node.Selected = false;
			PropagateHighlightDeselection( node );
		}



		public void DeSelectAll()
		{
			m_markedToDeSelect = false;
			for( int i = 0; i < m_selectedNodes.Count; i++ )
			{
				m_selectedNodes[ i ].Selected = false;
				m_selectedNodes[ i ].OnNodeStoppedMovingEvent -= OnNodeFinishMoving;
			}
			m_selectedNodes.Clear();
			ResetHighlightedWires();
		}

		public void AssignMasterNode()
		{
			if( m_selectedNodes.Count == 1 )
			{
				OutputNode newOutputNode = m_selectedNodes[ 0 ] as OutputNode;
				MasterNode newMasterNode = newOutputNode as MasterNode;
				if( newOutputNode != null )
				{
					if( m_masterNodeId != Constants.INVALID_NODE_ID && m_masterNodeId != newOutputNode.UniqueId )
					{
						OutputNode oldOutputNode = GetNode( m_masterNodeId ) as OutputNode;
						MasterNode oldMasterNode = oldOutputNode as MasterNode;
						if( oldOutputNode != null )
						{
							oldOutputNode.IsMainOutputNode = false;
							if( oldMasterNode != null )
							{
								oldMasterNode.ClearUpdateEvents();
							}
						}
					}
					m_masterNodeId = newOutputNode.UniqueId;
					newOutputNode.IsMainOutputNode = true;
					if( newMasterNode != null )
					{
						newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
						newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
					}
				}
			}

			IsDirty = true;
		}

		public void AssignMasterNode( OutputNode node, bool onlyUpdateGraphId )
		{
			AssignMasterNode( node.UniqueId, onlyUpdateGraphId );
			MasterNode masterNode = node as MasterNode;
			if( masterNode != null )
			{
				masterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
				masterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
			}
		}

		public void AssignMasterNode( int nodeId, bool onlyUpdateGraphId )
		{
			if( nodeId < 0 || m_masterNodeId == nodeId )
				return;

			if( m_masterNodeId > Constants.INVALID_NODE_ID )
			{
				OutputNode oldOutputNode = ( GetNode( nodeId ) as OutputNode );
				MasterNode oldMasterNode = oldOutputNode as MasterNode;
				if( oldOutputNode != null )
				{
					oldOutputNode.IsMainOutputNode = false;
					if( oldMasterNode != null )
					{
						oldMasterNode.ClearUpdateEvents();
					}
				}
			}

			if( onlyUpdateGraphId )
			{
				m_masterNodeId = nodeId;
			}
			else
			{
				OutputNode outputNode = ( GetNode( nodeId ) as OutputNode );
				if( outputNode != null )
				{
					outputNode.IsMainOutputNode = true;
					m_masterNodeId = nodeId;
				}
			}

			IsDirty = true;
		}

		public void RefreshOnUndo()
		{
			if( m_nodes != null )
			{
				int count = m_nodes.Count;
				for( int i = 0; i < count; i++ )
				{
					if( m_nodes[ i ] != null )
					{
						m_nodes[ i ].RefreshOnUndo();
					}
				}
			}
		}

		public void DrawGrid( DrawInfo drawInfo )
		{
			m_nodeGrid.DrawGrid( drawInfo );
		}

		public float MaxNodeDist
		{
			get { return m_nodeGrid.MaxNodeDist; }
		}

		public List<ParentNode> GetNodesInGrid( Vector2 transformedMousePos )
		{
			return m_nodeGrid.GetNodesOn( transformedMousePos );
		}

		public void FireMasterNode( Shader selectedShader )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).Execute( selectedShader );
		}

		public Shader FireMasterNode( string pathname, bool isFullPath )
		{
			return ( GetNode( m_masterNodeId ) as MasterNode ).Execute( pathname, isFullPath );
		}

		public void ForceSignalPropagationOnMasterNode()
		{
			if( CurrentOutputNode != null )
				CurrentOutputNode.GenerateSignalPropagation();
			List<ParentNode> localVarNodes = m_localVarNodes.NodesList;
			int count = localVarNodes.Count;
			for( int i = 0; i < count; i++ )
			{
				SignalGeneratorNode node = localVarNodes[ i ] as SignalGeneratorNode;
				if( node != null )
				{
					node.GenerateSignalPropagation();
				}
			}
		}

		public void UpdateShaderOnMasterNode( Shader newShader )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).UpdateFromShader( newShader );
		}

		public void CopyValuesFromMaterial( Material material )
		{
			Material currMaterial = CurrentMaterial;
			if( currMaterial == material )
			{
				for( int i = 0; i < m_nodes.Count; i++ )
				{
					m_nodes[ i ].ForceUpdateFromMaterial( material );
				}
			}
		}

		public void UpdateMaterialOnMasterNode( Material material )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).UpdateMasterNodeMaterial( material );
		}

		public void SetMaterialModeOnGraph( Material mat, bool fetchMaterialValues = true )
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].SetMaterialMode( mat, fetchMaterialValues );
			}
		}

		public ParentNode CheckNodeAt( Vector3 pos, bool checkForRMBIgnore = false )
		{
			ParentNode selectedNode = null;

			// this is checked on the inverse order to give priority to nodes that are drawn on top  ( last on the list )
			for( int i = m_nodes.Count - 1; i > -1; i-- )
			{
				if( m_nodes[ i ].Contains( pos ) )
				{
					if( checkForRMBIgnore )
					{
						if( !m_nodes[ i ].RMBIgnore )
						{
							selectedNode = m_nodes[ i ];
							break;
						}
					}
					else
					{
						selectedNode = m_nodes[ i ];
						break;
					}
				}
			}
			return selectedNode;
		}

		public void ResetNodesLocalVariables()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].Reset();
				m_nodes[ i ].ResetOutputLocals();

				FunctionNode fnode = m_nodes[ i ] as FunctionNode;
				if( fnode != null )
				{
					if( fnode.Function != null )
						fnode.FunctionGraph.ResetNodesLocalVariables();
				}
			}
		}

		public void ResetNodesLocalVariablesIfNot( MasterNodePortCategory category )
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].Reset();
				m_nodes[ i ].ResetOutputLocalsIfNot( category );

				FunctionNode fnode = m_nodes[ i ] as FunctionNode;
				if( fnode != null )
				{
					if( fnode.Function != null )
						fnode.FunctionGraph.ResetNodesLocalVariablesIfNot( category );
				}
			}
		}

		public void ResetNodesLocalVariables( ParentNode node )
		{
			node.Reset();
			node.ResetOutputLocals();
			int count = node.InputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( node.InputPorts[ i ].IsConnected )
				{
					ResetNodesLocalVariables( m_nodesDict[ node.InputPorts[ i ].GetConnection().NodeId ] );
				}
			}
		}

		public void ResetNodesLocalVariablesIfNot( ParentNode node, MasterNodePortCategory category )
		{
			node.Reset();
			node.ResetOutputLocalsIfNot( category );
			int count = node.InputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( node.InputPorts[ i ].IsConnected )
				{
					ResetNodesLocalVariablesIfNot( m_nodesDict[ node.InputPorts[ i ].GetConnection().NodeId ], category );
				}
			}
		}


		public override string ToString()
		{
			string dump = ( "Parent Graph \n" );
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				dump += ( m_nodes[ i ] + "\n" );
			}
			return dump;
		}

		public void OrderNodesByGraphDepth()
		{
			if( CurrentMasterNode != null )
			{
				CurrentMasterNode.SetupNodeCategories();
				int count = m_nodes.Count;
				for( int i = 0; i < count; i++ )
				{
					if( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Island )
					{
						m_nodes[ i ].CalculateCustomGraphDepth();
					}
				}
			}
			else
			{
				//TODO: remove this dynamic list
				List<FunctionOutput> allOutputs = new List<FunctionOutput>();
				for( int i = 0; i < AllNodes.Count; i++ )
				{
					FunctionOutput temp = AllNodes[ i ] as FunctionOutput;
					if( temp != null )
						allOutputs.Add( temp );
				}
				MasterNodeDataCollector dummy = new MasterNodeDataCollector();
				for( int j = 0; j < allOutputs.Count; j++ )
				{
					allOutputs[ j ].SetupNodeCategories( ref dummy );
					int count = m_nodes.Count;
					for( int i = 0; i < count; i++ )
					{
						if( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Island )
						{
							m_nodes[ i ].CalculateCustomGraphDepth();
						}
					}
				}
				dummy.Destroy();
				dummy = null;
			}

			m_nodes.Sort( ( x, y ) => { return y.GraphDepth.CompareTo( x.GraphDepth ); } );
		}

		public void WriteToString( ref string nodesInfo, ref string connectionsInfo )
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].FullWriteToString( ref nodesInfo, ref connectionsInfo );
				IOUtils.AddLineTerminator( ref nodesInfo );
			}
		}

		public void Reset()
		{
			SaveIsDirty = false;
			IsDirty = false;
		}

		public void OnBeforeSerialize()
		{
			//DeSelectAll();
		}

		public void OnAfterDeserialize()
		{
			m_afterDeserializeFlag = true;
		}

		public void CleanCorruptedNodes()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( ( object ) m_nodes[ i ] == null )
				{
					m_nodes.RemoveAt( i );
					CleanCorruptedNodes();
				}
			}
		}

		public void OnDuplicateEventWrapper()
		{
			if( OnDuplicateEvent != null )
			{
				AmplifyShaderEditorWindow temp = UIUtils.CurrentWindow;
				UIUtils.CurrentWindow = ParentWindow;
				OnDuplicateEvent();
				UIUtils.CurrentWindow = temp;
			}
		}

		public ParentNode CreateNode( AmplifyShaderFunction shaderFunction, bool registerUndo, int nodeId = -1, bool addLast = true )
		{
			FunctionNode newNode = ScriptableObject.CreateInstance<FunctionNode>();
			if( newNode )
			{
				newNode.ContainerGraph = this;
				newNode.CommonInit( shaderFunction );
				newNode.UniqueId = nodeId;
				AddNode( newNode, nodeId < 0, addLast, registerUndo );
			}
			return newNode;
		}

		public ParentNode CreateNode( AmplifyShaderFunction shaderFunction, bool registerUndo, Vector2 pos, int nodeId = -1, bool addLast = true )
		{
			ParentNode newNode = CreateNode( shaderFunction, registerUndo, nodeId, addLast );
			if( newNode )
			{
				newNode.Vec2Position = pos;
			}
			return newNode;
		}

		public ParentNode CreateNode( System.Type type, bool registerUndo, int nodeId = -1, bool addLast = true )
		{
			ParentNode newNode = ScriptableObject.CreateInstance( type ) as ParentNode;
			if( newNode )
			{
				newNode.ContainerGraph = this;
				newNode.UniqueId = nodeId;
				AddNode( newNode, nodeId < 0, addLast, registerUndo );
			}
			return newNode;
		}

		public ParentNode CreateNode( System.Type type, bool registerUndo, Vector2 pos, int nodeId = -1, bool addLast = true )
		{
			ParentNode newNode = CreateNode( type, registerUndo, nodeId, addLast );
			if( newNode )
			{
				newNode.Vec2Position = pos;
			}
			return newNode;
		}

		public void FireMasterNodeReplacedEvent()
		{
			MasterNode masterNode = CurrentMasterNode;
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_nodes[ i ].UniqueId != m_masterNodeId )
				{
					m_nodes[ i ].OnMasterNodeReplaced( masterNode );
				}
			}
		}

		public MasterNode ReplaceMasterNode( AvailableShaderTypes newType )
		{
			DeSelectAll();
			ResetNodeConnStatus();
			MasterNode newMasterNode = null;
			switch( newType )
			{
				default:
				case AvailableShaderTypes.SurfaceShader:
				{
					m_currentCanvasMode = NodeAvailability.SurfaceShader;
					newMasterNode = CreateNode( typeof( StandardSurfaceOutputNode ), false ) as MasterNode;
				}
				break;
				case AvailableShaderTypes.Template:
				{
					m_currentCanvasMode = NodeAvailability.TemplateShader;
					newMasterNode = CreateNode( typeof( TemplateMasterNode ), false ) as MasterNode;
				}
				break;
			}

			MasterNode currMasterNode = GetNode( m_masterNodeId ) as MasterNode;
			if( currMasterNode != null )
			{
				currMasterNode.ReleaseResources();
				newMasterNode.CopyFrom( currMasterNode );
				m_masterNodeId = -1;
				DestroyNode( currMasterNode );
			}

			m_masterNodeId = newMasterNode.UniqueId;
			newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
			newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
			newMasterNode.IsMainOutputNode = true;
			return newMasterNode;
		}

		public void CreateNewEmpty( string name )
		{
			CleanNodes();
			MasterNode newMasterNode = CreateNode( m_masterNodeDefaultType, false ) as MasterNode;
			newMasterNode.SetName( name );
			m_masterNodeId = newMasterNode.UniqueId;

			ParentWindow.IsShaderFunctionWindow = false;
			m_currentCanvasMode = NodeAvailability.SurfaceShader;

			newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
			newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
			newMasterNode.IsMainOutputNode = true;
			LoadedShaderVersion = UIUtils.CurrentVersion();
		}

		public void CreateNewEmptyTemplate( string templateGUID )
		{
			CleanNodes();
			TemplateMasterNode newMasterNode = CreateNode( typeof( TemplateMasterNode ), false ) as TemplateMasterNode;
			m_masterNodeId = newMasterNode.UniqueId;

			ParentWindow.IsShaderFunctionWindow = false;
			m_currentCanvasMode = NodeAvailability.TemplateShader;

			newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
			newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
			newMasterNode.IsMainOutputNode = true;

			newMasterNode.SetTemplate( TemplatesManager.GetTemplate( templateGUID ), true, true );

			LoadedShaderVersion = UIUtils.CurrentVersion();
		}

		public void CreateNewEmptyFunction( AmplifyShaderFunction shaderFunction )
		{
			CleanNodes();
			FunctionOutput newOutputNode = CreateNode( typeof( FunctionOutput ), false ) as FunctionOutput;
			m_masterNodeId = newOutputNode.UniqueId;

			ParentWindow.IsShaderFunctionWindow = true;
			m_currentCanvasMode = NodeAvailability.ShaderFunction;

			newOutputNode.IsMainOutputNode = true;
		}

		public void ForceCategoryRefresh() { m_forceCategoryRefresh = true; }
		public void RefreshExternalReferences()
		{
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				m_nodes[ i ].RefreshExternalReferences();
			}
		}

		public Vector2 SelectedNodesCentroid
		{
			get
			{
				if( m_selectedNodes.Count == 0 )
					return Vector2.zero;
				Vector2 pos = new Vector2( 0, 0 );
				for( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					pos += m_selectedNodes[ i ].Vec2Position;
				}

				pos /= m_selectedNodes.Count;
				return pos;
			}
		}

		public void AddVirtualTextureCount()
		{
			m_virtualTextureCount += 1;
		}

		public void RemoveVirtualTextureCount()
		{
			m_virtualTextureCount -= 1;
			if( m_virtualTextureCount < 0 )
			{
				Debug.LogWarning( "Invalid virtual texture count" );
			}
		}

		public bool HasVirtualTexture { get { return m_virtualTextureCount > 0; } }

		public void AddInstancePropertyCount()
		{
			m_instancePropertyCount += 1;
		}

		public void RemoveInstancePropertyCount()
		{
			m_instancePropertyCount -= 1;
			if( m_instancePropertyCount < 0 )
			{
				Debug.LogWarning( "Invalid property instance count" );
			}
		}

		public bool IsInstancedShader { get { return m_instancePropertyCount > 0; } }

		public void AddNormalDependentCount() { m_normalDependentCount += 1; }

		public void RemoveNormalDependentCount()
		{
			m_normalDependentCount -= 1;
			if( m_normalDependentCount < 0 )
			{
				Debug.LogWarning( "Invalid normal dependentCount count" );
			}
		}

		public void SetModeFromMasterNode()
		{
			MasterNode masterNode = CurrentMasterNode;
			if( masterNode != null )
			{
				switch( masterNode.CurrentMasterNodeCategory )
				{
					default:
					case AvailableShaderTypes.SurfaceShader:
					{
						CurrentCanvasMode = NodeAvailability.SurfaceShader;
					}
					break;
					case AvailableShaderTypes.Template:
					{
						CurrentCanvasMode = NodeAvailability.TemplateShader;
					}
					break;
				}
			}
			else
			{
				CurrentCanvasMode = NodeAvailability.SurfaceShader;
			}
		}
		public bool IsNormalDependent { get { return m_normalDependentCount > 0; } }

		public void MarkToDeselect() { m_markedToDeSelect = true; }
		public void MarkToSelect( int nodeId ) { m_markToSelect = nodeId; }
		public void MarkWireHighlights() { m_checkSelectedWireHighlights = true; }
		public List<ParentNode> SelectedNodes { get { return m_selectedNodes; } }
		public List<ParentNode> MarkedForDeletionNodes { get { return m_markedForDeletion; } }
		public int CurrentMasterNodeId { get { return m_masterNodeId; } }

		public Shader CurrentShader
		{
			get
			{
				MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
				if( masterNode != null )
					return masterNode.CurrentShader;
				return null;
			}
		}

		public Material CurrentMaterial
		{
			get
			{
				MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
				if( masterNode != null )
					return masterNode.CurrentMaterial;
				return null;
			}
		}

		public NodeAvailability CurrentCanvasMode { get { return m_currentCanvasMode; } set { m_currentCanvasMode = value; } }
		public OutputNode CurrentOutputNode { get { return GetNode( m_masterNodeId ) as OutputNode; } }
		public FunctionOutput CurrentFunctionOutput { get { return GetNode( m_masterNodeId ) as FunctionOutput; } }
		public MasterNode CurrentMasterNode { get { return GetNode( m_masterNodeId ) as MasterNode; } }
		public StandardSurfaceOutputNode CurrentStandardSurface { get { return GetNode( m_masterNodeId ) as StandardSurfaceOutputNode; } }
		public List<ParentNode> AllNodes { get { return m_nodes; } }
		public int NodeCount { get { return m_nodes.Count; } }
		public List<ParentNode> VisibleNodes { get { return m_visibleNodes; } }

		public int NodeClicked
		{
			set { m_nodeClicked = value; }
			get { return m_nodeClicked; }
		}

		public bool IsDirty
		{
			set { m_isDirty = value && UIUtils.DirtyMask; }
			get
			{
				bool value = m_isDirty;
				m_isDirty = false;
				return value;
			}
		}

		public bool SaveIsDirty
		{
			set { m_saveIsDirty = value && UIUtils.DirtyMask; }
			get { return m_saveIsDirty; }
		}
		public int LoadedShaderVersion
		{
			get { return m_loadedShaderVersion; }
			set { m_loadedShaderVersion = value; }
		}

		public AmplifyShaderFunction CurrentShaderFunction
		{
			get { if( CurrentFunctionOutput != null ) return CurrentFunctionOutput.Function; else return null; }
			set { if( CurrentFunctionOutput != null ) CurrentFunctionOutput.Function = value; }
		}

		public bool HasUnConnectedNodes { get { return m_hasUnConnectedNodes; } }
		public NodeUsageRegister SamplerNodes { get { return m_samplerNodes; } }
		public NodeUsageRegister TexturePropertyNodes { get { return m_texturePropertyNodes; } }
		public NodeUsageRegister TextureArrayNodes { get { return m_textureArrayNodes; } }
		public NodeUsageRegister PropertyNodes { get { return m_propertyNodes; } }
		public NodeUsageRegister FunctionInputNodes { get { return m_functionInputNodes; } }
		public NodeUsageRegister FunctionNodes { get { return m_functionNodes; } }
		public NodeUsageRegister FunctionOutputNodes { get { return m_functionOutputNodes; } }
		public NodeUsageRegister ScreenColorNodes { get { return m_screenColorNodes; } }
		public NodeUsageRegister LocalVarNodes { get { return m_localVarNodes; } }
		public PrecisionType CurrentPrecision
		{
			get { return m_currentPrecision; }
			set { m_currentPrecision = value; }
		}

		public NodeLOD LodLevel
		{
			get { return m_lodLevel; }
		}

		public List<ParentNode> NodePreviewList { get { return m_nodePreviewList; } set { m_nodePreviewList = value; } }

		public void SetGraphId( int id )
		{
			m_graphId = id;
		}

		public int GraphId
		{
			get { return m_graphId; }
		}

		public AmplifyShaderEditorWindow ParentWindow
		{
			get { return m_parentWindow; }
			set { m_parentWindow = value; }
		}

		private bool m_changedLightingModel = false;
		public bool ChangedLightingModel
		{
			get { return m_changedLightingModel; }
			set { m_changedLightingModel = value; }
		}
	}
}
