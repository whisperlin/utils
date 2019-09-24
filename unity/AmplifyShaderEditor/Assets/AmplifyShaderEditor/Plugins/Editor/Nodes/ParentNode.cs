// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum PreviewLocation
	{
		Auto,
		TopCenter,
		BottomCenter,
		Left,
		Right
	}

	public enum NodeMessageType
	{
		Error,
		Warning,
		Info
	}

	[Serializable]
	public class ParentNode : UndoParentNode, ISerializationCallbackReceiver
	{
		private const double NodeClickTime = 0.2;
		protected GUIContent PrecisionContante = new GUIContent( "Precision", "Changes the precision of internal calculations, using lower types saves some performance\nDefault: Float" );
		private const int MoveCountBuffer = 3;// When testing for stopped movement we need to take Layout and Repaint into account for them not to interfere with tests
		private const float MinInsideBoxWidth = 20;
		private const float MinInsideBoxHeight = 10;

		public delegate void OnNodeEvent( ParentNode node, bool testOnlySelected, InteractionMode interactionMode );
		public delegate void OnNodeGenericEvent( ParentNode node );
		public delegate void OnNodeReOrder( ParentNode node, int index );
		public delegate void DrawPropertySection();

		//[SerializeField]
		protected ParentGraph m_containerGraph = null;

		[SerializeField]
		protected PrecisionType m_currentPrecisionType = PrecisionType.Float;

		[SerializeField]
		protected InteractionMode m_defaultInteractionMode = InteractionMode.Other;

		[SerializeField]
		public event OnNodeEvent OnNodeStoppedMovingEvent;

		[SerializeField]
		public OnNodeGenericEvent OnNodeChangeSizeEvent;

		[SerializeField]
		public OnNodeGenericEvent OnNodeDestroyedEvent;

		[SerializeField]
		public event OnNodeReOrder OnNodeReOrderEvent;

		[SerializeField]
		private int m_uniqueId;

		[SerializeField]
		protected Rect m_position;

		[SerializeField]
		protected Rect m_unpreviewedPosition;

		[SerializeField]
		protected GUIContent m_content;

		[SerializeField]
		protected GUIContent m_additionalContent;

		[SerializeField]
		protected bool m_initialized;

		[SerializeField]
		protected NodeConnectionStatus m_connStatus;
		protected bool m_selfPowered = false;

		[SerializeField]
		private int m_activeConnections;

		[SerializeField]
		protected System.Type m_activeType;

		[SerializeField]
		protected int m_activePort;

		[SerializeField]
		protected int m_activeNode;

		protected NodeRestrictions m_restrictions;

		[SerializeField]
		protected Color m_statusColor;

		[SerializeField]
		protected Rect m_propertyDrawPos;

		// Ports
		[SerializeField]
		protected List<InputPort> m_inputPorts = new List<InputPort>();

		protected Dictionary<int, InputPort> m_inputPortsDict;

		[SerializeField]
		protected List<OutputPort> m_outputPorts = new List<OutputPort>();

		protected Dictionary<int, OutputPort> m_outputPortsDict;

		[SerializeField]
		protected Rect m_globalPosition;

		[SerializeField]
		protected Rect m_headerPosition;

		//private Vector2 m_tooltipOffset;

		[SerializeField]
		protected bool m_sizeIsDirty = false;

		[SerializeField]
		protected Vector2 m_extraSize;

		[SerializeField]
		protected Vector2 m_insideSize;

		[SerializeField]
		protected float m_fontHeight;

		// Editor State save on Play Button
		[SerializeField]
		protected bool m_isDirty;

		[SerializeField]
		private int m_isMoving = 0;
		[SerializeField]
		private Rect m_lastPosition;

		// Live Shader Gen
		[SerializeField]
		private bool m_saveIsDirty;

		[SerializeField]
		protected bool m_requireMaterialUpdate = false;

		[SerializeField]
		protected int m_commentaryParent = -1;

		[SerializeField]
		protected int m_depth = -1;

		[SerializeField]
		protected bool m_materialMode = false;

		[SerializeField]
		protected bool m_showPreview = false;

		[SerializeField]
		protected int m_previewMaterialPassId = -1;

		// Error Box Messages
		private Rect m_errorBox;
		private bool m_previousErrorMessage = false;
		protected bool m_showErrorMessage = false;
		protected NodeMessageType m_errorMessageTypeIsError = NodeMessageType.Error;
		protected string m_errorMessageTooltip = string.Empty;

		private GUIContent m_errorContent = new GUIContent( "ERROR" );
		private GUIContent m_warningContent = new GUIContent( "WARNING" );
		private GUIContent m_infoContent = new GUIContent( "INFO" );

		// Styles
		protected GUIStyle m_buttonStyle = null;
		protected GUIStyle m_labelStyle = null;
		protected GUIStyle m_toggleStyle = null;
		protected GUIStyle m_textfieldStyle = null;
		protected GUIStyle m_empty = new GUIStyle();

		// Drawing Node
		protected PreviewLocation m_selectedLocation = PreviewLocation.Auto;
		private int m_extraHeaderHeight = 0;
		protected bool m_isVisible;
		protected bool m_selected = false;
		protected bool m_rmbIgnore;
		protected GUIContent m_sizeContentAux;

		protected uint m_currentReadParamIdx = 1;
		protected bool m_reorderLocked = false;

		protected Rect m_cachedPos;
		protected Vector2 m_accumDelta = Vector2.zero;

		private bool m_isOnGrid = false;
		protected bool m_useInternalPortData = false;
		protected bool m_autoDrawInternalPortData = true;
		protected DrawOrder m_drawOrder = DrawOrder.Default;

		protected bool m_movingInFrame = false;
		protected float m_anchorAdjust = -1;

		protected Color m_headerColor;

		protected Color m_headerColorModifier = Color.white;

		protected bool m_infiniteLoopDetected = false;
		protected int m_textLabelWidth = -1;

		private bool m_linkVisibility = false;

		protected int m_category = 0;

		protected double m_lastTimeSelected;
		private double m_tooltipTimestamp;
		private string m_tooltipText;

		protected Rect m_unscaledRemainingBox;
		protected Rect m_remainingBox;

		private int m_visibleInputs = 0;
		private int m_visibleOutputs = 0;

		private double m_doubleClickTimestamp;
		private const double DoubleClickTime = 0.25;

		protected bool m_canExpand = true;

		protected bool m_firstDraw = true;

		protected int m_matrixId = -1;

		private float m_paddingTitleLeft = 0;
		private float m_paddingTitleRight = 0;

		// Preview Fields
		private Material m_previewMaterial = null;
		private Shader m_previewShader = null;
		protected string m_previewShaderGUID = string.Empty;
		protected float m_marginPreviewLeft = 0;
		protected bool m_globalShowPreview = false;
		protected Rect m_unscaledPreviewRect;
		protected Rect m_previewRect;
		protected bool m_drawPreviewMaskButtons = true;
		private int m_channelNumber = 0;
		protected bool m_firstPreviewDraw = true;
		protected bool m_drawPreview = true;
		protected bool m_drawPreviewExpander = true;
		private bool m_spherePreview = false;
		protected bool m_drawPreviewAsSphere = false;
		protected bool m_forceDrawPreviewAsPlane = false;

		private int m_cachedMainTexId = -1;
		private int m_cachedMaskTexId = -1;
		private int m_cachedPortsId = -1;
		private int m_cachedPortId = -1;

		private int m_cachedDrawSphereId = -1;
		private int m_cachedInvertedZoomId = -1;
		//private int m_cachedIsLinearId = -1;

		private bool[] m_previewChannels = { true, true, true, false };

		protected bool m_autoWrapProperties = false;
		private bool m_internalDataFoldout = true;
		protected bool m_propertiesFoldout = true;
		protected bool m_repopulateDictionaries = true;

		protected Vector2 m_lastInputBottomRight = Vector2.zero;
		protected Vector2 m_lastOutputBottomLeft = Vector2.zero;

		private Vector4 m_portMask = Vector4.zero;

		private Vector2 m_auxVector2 = Vector4.zero;
		protected Rect m_auxRect;

		protected PreviewLocation m_autoLocation;
		protected Rect m_titlePos;
		protected Rect m_addTitlePos;
		protected Rect m_expandRect;
		protected Rect m_currInputPortPos;
		protected Rect m_currOutputPortPos;
		protected Color m_colorBuffer;

		protected int m_graphDepth = 0;
		public ParentNode()
		{
			m_position = new Rect( 0, 0, 0, 0 );
			m_content = new GUIContent( GUIContent.none );
			m_additionalContent = new GUIContent( GUIContent.none );
			CommonInit( -1 );
		}

		public ParentNode( int uniqueId, float x, float y, float width, float height )
		{
			m_position = new Rect( x, y, width, height );
			m_content = new GUIContent( GUIContent.none );
			m_additionalContent = new GUIContent( GUIContent.none );
			CommonInit( uniqueId );
		}

		public virtual void OnEnable()
		{
			m_tooltipTimestamp = Time.realtimeSinceStartup;

			hideFlags = HideFlags.DontSave;

			if( m_containerGraph == null )
				m_containerGraph = UIUtils.CurrentWindow.CurrentGraph;

			if( m_containerGraph.ParentWindow == null )
				m_containerGraph.ParentWindow = UIUtils.CurrentWindow;
		}

		protected virtual void CommonInit( int uniqueId )
		{
			m_uniqueId = uniqueId;

			m_isOnGrid = false;
			ConnStatus = NodeConnectionStatus.Not_Connected;
			m_inputPorts = new List<InputPort>();
			m_inputPortsDict = new Dictionary<int, InputPort>();

			m_outputPorts = new List<OutputPort>();
			m_outputPortsDict = new Dictionary<int, OutputPort>();

			System.Reflection.MemberInfo info = this.GetType();
			m_nodeAttribs = info.GetCustomAttributes( true )[ 0 ] as NodeAttributes;
			if( m_nodeAttribs != null )
			{
				m_content.text = m_nodeAttribs.Name;
				//m_content.tooltip = m_nodeAttribs.Description;
				m_tooltipText = m_nodeAttribs.Description;
				m_selected = false;
				m_headerColor = UIUtils.GetColorFromCategory( m_nodeAttribs.Category );
			}

			m_sizeContentAux = new GUIContent();
			m_extraSize = new Vector2( 0, 0 );
			m_insideSize = new Vector2( 0, 0 );
			m_sizeIsDirty = true;
			m_initialized = true;
			m_restrictions = new NodeRestrictions();

			m_propertyDrawPos = new Rect();
		}

		public virtual void AfterCommonInit()
		{
			if( PreviewShader && !HasPreviewShader )
			{
				m_drawPreview = false;
				m_drawPreviewExpander = false;
				m_canExpand = false;
			}

			if( m_drawPreviewExpander )
			{
				m_paddingTitleRight += Constants.PreviewExpanderWidth + Constants.IconsLeftRightMargin;
				m_paddingTitleLeft = Constants.PreviewExpanderWidth + Constants.IconsLeftRightMargin;
			}
		}

		public virtual void Destroy()
		{
			if( OnNodeDestroyedEvent != null )
			{
				OnNodeDestroyedEvent( this );
				OnNodeDestroyedEvent = null;
			}

			OnNodeStoppedMovingEvent = null;
			OnNodeChangeSizeEvent = null;
			OnNodeReOrderEvent = null;
			if( m_restrictions != null )
				m_restrictions.Destroy();
			m_restrictions = null;

			int inputCount = m_inputPorts.Count;
			for( int i = 0; i < inputCount; i++ )
			{
				m_inputPorts[ i ].Destroy();
			}
			m_inputPorts.Clear();
			m_inputPorts = null;

			int outputCount = m_outputPorts.Count;
			for( int i = 0; i < outputCount; i++ )
			{
				m_outputPorts[ i ].Destroy();
			}
			m_outputPorts.Clear();
			m_outputPorts = null;

			m_inputPortsDict.Clear();
			m_inputPortsDict = null;

			m_outputPortsDict.Clear();
			m_outputPortsDict = null;

			m_buttonStyle = null;
			m_labelStyle = null;
			m_toggleStyle = null;
			m_textfieldStyle = null;
			m_empty = null;

			if( m_previewMaterial != null )
				DestroyImmediate( m_previewMaterial );
			m_previewMaterial = null;

			m_previewShader = null;
		}

		public virtual void Move( Vector2 delta )
		{
			Move( delta, false );
		}

		public virtual void Move( Vector2 delta, bool snap )
		{
			if( m_isMoving == 0 )
			{
				m_cachedPos = m_position;
				m_accumDelta = Vector2.zero;
			}

			m_isMoving = MoveCountBuffer;
			m_accumDelta += delta;

			if( snap )
			{
				m_position.x = Mathf.Round( ( m_cachedPos.x + m_accumDelta.x ) / 16 ) * 16;
				m_position.y = Mathf.Round( ( m_cachedPos.y + m_accumDelta.y ) / 16 ) * 16;
			}
			else
			{
				m_position.x += delta.x;
				m_position.y += delta.y;
			}
			//if(Event.current.type == EventType.Layout)
			m_movingInFrame = true;
		}

		public virtual void UpdateMaterial( Material mat )
		{
			m_requireMaterialUpdate = false;
		}

		public virtual void SetMaterialMode( Material mat, bool fetchMaterialValues )
		{
			m_materialMode = ( mat != null );
		}

		public virtual bool UpdateShaderDefaults( ref Shader shader, ref TextureDefaultsDataColector defaultCol ) { return false; }
		public virtual void ForceUpdateFromMaterial( Material material ) { }
		public void SetSaveIsDirty()
		{
			if( m_connStatus == NodeConnectionStatus.Connected )
			{
				SaveIsDirty = true;
			}
		}

		public void ActivateNodeReordering( int index )
		{
			if( OnNodeReOrderEvent != null )
				OnNodeReOrderEvent( this, index );
		}

		void RecalculateInputPortId()
		{
			m_inputPortsDict.Clear();
			int count = m_inputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_inputPorts[ i ].IsConnected )
				{
					int nodeId = m_inputPorts[ i ].ExternalReferences[ 0 ].NodeId;
					int portId = m_inputPorts[ i ].ExternalReferences[ 0 ].PortId;
					ParentNode node = UIUtils.GetNode( nodeId );
					if( node != null )
					{
						int outputCount = node.OutputPorts[ portId ].ExternalReferences.Count;
						for( int j = 0; j < outputCount; j++ )
						{
							if( node.OutputPorts[ portId ].ExternalReferences[ j ].NodeId == m_uniqueId &&
								node.OutputPorts[ portId ].ExternalReferences[ j ].PortId == m_inputPorts[ i ].PortId )
							{
								node.OutputPorts[ portId ].ExternalReferences[ j ].PortId = i;
							}
						}
					}
				}
				m_inputPorts[ i ].PortId = i;
				m_inputPortsDict.Add( i, m_inputPorts[ i ] );
			}
		}

		public void RemoveInputPort( int idx )
		{
			if( idx < m_inputPorts.Count )
			{
				m_inputPortsDict.Remove( m_inputPorts[ idx ].PortId );
				m_inputPorts.RemoveAt( idx );
				SetSaveIsDirty();
				m_sizeIsDirty = true;
			}
		}

		public void RemoveOutputPort( int idx )
		{
			if( idx < m_outputPorts.Count )
			{
				m_outputPortsDict.Remove( m_outputPorts[ idx ].PortId );
				m_outputPorts.RemoveAt( idx );
				SetSaveIsDirty();
				m_sizeIsDirty = true;
			}
		}

		// Manually add Ports 
		public void AddInputPort( WirePortDataType type, bool typeLocked, string name, int orderId = -1, MasterNodePortCategory category = MasterNodePortCategory.Fragment, int uniquePortId = -1 )
		{
			InputPort port = new InputPort( m_uniqueId, ( uniquePortId < 0 ? m_inputPorts.Count : uniquePortId ), type, name, typeLocked, ( orderId >= 0 ? orderId : m_inputPorts.Count ), category );
			m_inputPorts.Add( port );
			m_inputPortsDict.Add( port.PortId, port );
			SetSaveIsDirty();
			m_sizeIsDirty = true;
		}

		public void AddInputPort( WirePortDataType type, bool typeLocked, string name, string dataName, int orderId = -1, MasterNodePortCategory category = MasterNodePortCategory.Fragment, int uniquePortId = -1 )
		{
			InputPort port = new InputPort( m_uniqueId, ( uniquePortId < 0 ? m_inputPorts.Count : uniquePortId ), type, name, dataName, typeLocked, ( orderId >= 0 ? orderId : m_inputPorts.Count ), category );
			m_inputPorts.Add( port );
			m_inputPortsDict.Add( port.PortId, port );
			SetSaveIsDirty();
			m_sizeIsDirty = true;
		}

		public void AddInputPortAt( int idx, WirePortDataType type, bool typeLocked, string name, int orderId = -1, MasterNodePortCategory category = MasterNodePortCategory.Fragment, int uniquePortId = -1 )
		{
			InputPort port = new InputPort( m_uniqueId, ( uniquePortId < 0 ? m_inputPorts.Count : uniquePortId ), type, name, typeLocked, ( orderId >= 0 ? orderId : m_inputPorts.Count ), category );
			m_inputPorts.Insert( idx, port );
			m_inputPortsDict.Add( port.PortId, port );
			SetSaveIsDirty();
			m_sizeIsDirty = true;
			RecalculateInputPortId();
		}

		public void AddOutputPort( WirePortDataType type, string name, int uniquePortId = -1 )
		{
			m_outputPorts.Add( new OutputPort( m_uniqueId, ( uniquePortId < 0 ? m_outputPorts.Count : uniquePortId ), type, name ) );
			m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
			SetSaveIsDirty();
			m_sizeIsDirty = true;
		}

		public void AddOutputVectorPorts( WirePortDataType type, string name )
		{
			m_sizeIsDirty = true;
			m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, type, name ) );
			m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );

			switch( type )
			{
				case WirePortDataType.FLOAT2:
				{
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "X" ) );
					m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "Y" ) );
					m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "X" ) );
					m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "Y" ) );
					m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "Z" ) );
					m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "X" ) );
					m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "Y" ) );
					m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "Z" ) );
					m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "W" ) );
					m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
				}
				break;
			}
			SetSaveIsDirty();
		}

		public string GetOutputVectorItem( int vectorPortId, int currentPortId, string result )
		{
			if( m_outputPorts[ 0 ].DataType == WirePortDataType.COLOR )
			{
				switch( currentPortId - vectorPortId )
				{
					case 1: result += ".r"; break;
					case 2: result += ".g"; break;
					case 3: result += ".b"; break;
					case 4: result += ".a"; break;
				}
			}
			else
			{
				switch( currentPortId - vectorPortId )
				{
					case 1: result += ".x"; break;
					case 2: result += ".y"; break;
					case 3: result += ".z"; break;
					case 4: result += ".w"; break;
				}
			}
			return result;
		}

		public void AddOutputColorPorts( string name, bool addAlpha = true )
		{
			m_sizeIsDirty = true;
			//Main port
			m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, addAlpha ? WirePortDataType.COLOR : WirePortDataType.FLOAT3, name ) );
			m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );

			//Color components port
			m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "R" ) );
			m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
			m_outputPorts[ m_outputPorts.Count - 1 ].CustomColor = Color.red;

			m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "G" ) );
			m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
			m_outputPorts[ m_outputPorts.Count - 1 ].CustomColor = Color.green;

			m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "B" ) );
			m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
			m_outputPorts[ m_outputPorts.Count - 1 ].CustomColor = Color.blue;

			if( addAlpha )
			{
				m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "A" ) );
				m_outputPortsDict.Add( m_outputPorts[ m_outputPorts.Count - 1 ].PortId, m_outputPorts[ m_outputPorts.Count - 1 ] );
				m_outputPorts[ m_outputPorts.Count - 1 ].CustomColor = Color.white;
			}
		}

		public void ConvertFromVectorToColorPorts()
		{
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.COLOR, false );

			m_outputPorts[ 1 ].Name = "R";
			m_outputPorts[ 1 ].CustomColor = Color.red;

			m_outputPorts[ 2 ].Name = "G";
			m_outputPorts[ 2 ].CustomColor = Color.green;

			m_outputPorts[ 3 ].Name = "B";
			m_outputPorts[ 3 ].CustomColor = Color.blue;

			m_outputPorts[ 4 ].Name = "A";
			m_outputPorts[ 4 ].CustomColor = Color.white;
		}


		public string GetOutputColorItem( int vectorPortId, int currentPortId, string result )
		{
			switch( currentPortId - vectorPortId )
			{
				case 1: result += ".r"; break;
				case 2: result += ".g"; break;
				case 3: result += ".b"; break;
				case 4: result += ".a"; break;
			}
			return result;
		}

		public void ChangeOutputType( WirePortDataType type, bool invalidateConnections )
		{
			int outputCount = m_outputPorts.Count;
			for( int i = 0; i < outputCount; i++ )
			{
				m_outputPorts[ i ].ChangeType( type, invalidateConnections );
			}
		}

		public void ChangeInputType( WirePortDataType type, bool invalidateConnections )
		{
			int inputCount = m_inputPorts.Count;
			for( int i = 0; i < inputCount; i++ )
			{
				m_inputPorts[ i ].ChangeType( type, invalidateConnections );
			}
		}

		public void ChangeOutputProperties( int outputID, string newName, WirePortDataType newType, bool invalidateConnections = true )
		{
			if( outputID < m_outputPorts.Count )
			{
				m_outputPorts[ outputID ].ChangeProperties( newName, newType, invalidateConnections );
				IsDirty = true;
				m_sizeIsDirty = true;
				SetSaveIsDirty();
			}
		}

		public void ChangeOutputName( int outputID, string newName )
		{
			if( outputID < m_outputPorts.Count )
			{
				m_outputPorts[ outputID ].Name = newName;
				IsDirty = true;
				m_sizeIsDirty = true;
			}
		}

		public InputPort CheckInputPortAt( Vector3 pos )
		{
			int count = m_inputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_inputPorts[ i ].InsideActiveArea( pos ) )
					return m_inputPorts[ i ];
			}
			return null;
		}

		public InputPort GetFirstInputPortOfType( WirePortDataType dataType, bool countObjectTypeAsValid )
		{
			int count = m_inputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( ( m_inputPorts[ i ].DataType == dataType ) || ( countObjectTypeAsValid && m_inputPorts[ i ].DataType == WirePortDataType.OBJECT ) )
					return m_inputPorts[ i ];
			}
			return null;
		}

		public OutputPort CheckOutputPortAt( Vector3 pos )
		{
			int count = m_outputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_outputPorts[ i ].InsideActiveArea( pos ) )
					return m_outputPorts[ i ];
			}
			return null;
		}

		public OutputPort GetFirstOutputPortOfType( WirePortDataType dataType, bool checkForCasts )
		{
			int count = m_outputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( ( m_outputPorts[ i ].DataType == dataType ) || ( checkForCasts && UIUtils.CanCast( dataType, m_outputPorts[ i ].DataType ) ) )
					return m_outputPorts[ i ];
			}
			return null;
		}

		virtual protected void ChangeSizeFinished() { m_firstPreviewDraw = true; /*MarkForPreviewUpdate();*/ }
		protected void ChangeSize()
		{
			m_cachedPos = m_position;
			//UIUtils.ResetMainSkin();

			string maxInString = string.Empty;
			string maxOutString = string.Empty;
			int inputCount = 0;
			int inputSize = m_inputPorts.Count;
			for( int i = 0; i < inputSize; i++ )
			{
				if( m_inputPorts[ i ].Visible )
				{
					if( m_inputPorts[ i ].Name.Length > maxInString.Length )
					{
						maxInString = m_inputPorts[ i ].Name;
					}

					inputCount += 1;
				}
			}

			int outputCount = 0;
			int outputSize = m_outputPorts.Count;
			for( int o = 0; o < outputSize; o++ )
			{
				if( m_outputPorts[ o ].Visible )
				{
					if( m_outputPorts[ o ].Name.Length > maxOutString.Length )
					{
						maxOutString = m_outputPorts[ o ].Name;
					}
					outputCount += 1;
				}
			}

			Vector2 inSize = Vector2.zero;
			Vector2 outSize = Vector2.zero;
			if( maxInString.Length > 0 )
			{
				m_sizeContentAux.text = maxInString;
				inSize = UIUtils.UnZoomedInputPortStyle.CalcSize( m_sizeContentAux );
				inSize.x += UIUtils.PortsSize.x + Constants.PORT_TO_LABEL_SPACE_X * 2;

				if( UIUtils.PortsSize.y > inSize.y )
					inSize.y = UIUtils.PortsSize.y;
			}

			inSize.x += m_marginPreviewLeft;

			if( maxOutString.Length > 0 )
			{
				m_sizeContentAux.text = maxOutString;
				outSize = UIUtils.UnZoomedOutputPortPortStyle.CalcSize( m_sizeContentAux );
				outSize.x += UIUtils.PortsSize.x + Constants.PORT_TO_LABEL_SPACE_X * 2;

				if( UIUtils.PortsSize.y > outSize.y )
					outSize.y = UIUtils.PortsSize.y;
			}

			if( m_additionalContent.text.Length > 0 )
				m_extraHeaderHeight = ( int ) Constants.NODE_HEADER_EXTRA_HEIGHT;
			else
				m_extraHeaderHeight = 0;

			float headerWidth = Mathf.Max( UIUtils.UnZoomedNodeTitleStyle.CalcSize( m_content ).x + m_paddingTitleLeft + m_paddingTitleRight, UIUtils.UnZoomedPropertyValuesTitleStyle.CalcSize( m_additionalContent ).x + m_paddingTitleLeft + m_paddingTitleRight );
			m_position.width = Mathf.Max( headerWidth, Mathf.Max( MinInsideBoxWidth, m_insideSize.x ) + inSize.x + outSize.x ) + Constants.NODE_HEADER_LEFTRIGHT_MARGIN * 2;

			//m_position.width += m_extraSize.x;

			m_fontHeight = Mathf.Max( inSize.y, outSize.y );
			m_position.height = Mathf.Max( inputCount, outputCount ) * ( m_fontHeight + Constants.INPUT_PORT_DELTA_Y );// + Constants.INPUT_PORT_DELTA_Y;
			m_position.height = Mathf.Max( m_position.height, Mathf.Max( MinInsideBoxHeight, m_insideSize.y ) );
			m_position.height += UIUtils.HeaderMaxHeight + m_extraHeaderHeight + Constants.INPUT_PORT_DELTA_Y;// + m_extraSize.y;
			if( m_showErrorMessage )
				m_position.height += 24;

			m_unpreviewedPosition = m_position;
			//UIUtils.CurrentWindow.CameraDrawInfo.InvertedZoom = cachedZoom;
			if( OnNodeChangeSizeEvent != null )
			{
				OnNodeChangeSizeEvent( this );
			}
			ChangeSizeFinished();
		}
		public virtual void Reset() { }
		public virtual void OnOutputPortConnected( int portId, int otherNodeId, int otherPortId ) { }

		public virtual void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			InputPort port = GetInputPortByUniqueId( portId );
			if( activateNode && m_connStatus == NodeConnectionStatus.Connected )
			{
				port.GetOutputNode().ActivateNode( m_activeNode, m_activePort, m_activeType );
			}

			OnNodeChange();
			SetSaveIsDirty();
		}

		public virtual void OnInputPortDisconnected( int portId ) { OnNodeChange(); }
		public virtual void OnOutputPortDisconnected( int portId ) { }

		public virtual void OnNodeChange()
		{
			CheckSpherePreview();
			int count = m_outputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_outputPorts[ i ].IsConnected )
				{
					for( int f = 0; f < m_outputPorts[ i ].ExternalReferences.Count; f++ )
					{
						m_outputPorts[ i ].GetInputNode( f ).OnNodeChange();
					}
				}
			}
		}

		public void ActivateNode( int signalGenNodeId, int signalGenPortId, System.Type signalGenNodeType )
		{
			if( m_selfPowered )
				return;

			ConnStatus = m_restrictions.GetRestiction( signalGenNodeType, signalGenPortId ) ? NodeConnectionStatus.Error : NodeConnectionStatus.Connected;
			m_activeConnections += 1;

			m_activeType = signalGenNodeType;
			m_activeNode = signalGenNodeId;
			m_activePort = signalGenPortId;

			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				if( m_inputPorts[ i ].IsConnected )
				{
					m_inputPorts[ i ].GetOutputNode().ActivateNode( signalGenNodeId, signalGenPortId, signalGenNodeType );
				}
			}
			SetSaveIsDirty();
		}

		public void DeactivateNode( int deactivatedPort, bool forceComplete )
		{
			if( m_selfPowered )
				return;

			SetSaveIsDirty();
			m_activeConnections -= 1;
			if( forceComplete || m_activeConnections <= 0 )
			{
				m_activeConnections = 0;
				ConnStatus = NodeConnectionStatus.Not_Connected;
				for( int i = 0; i < m_inputPorts.Count; i++ )
				{
					if( m_inputPorts[ i ].IsConnected )
					{
						m_inputPorts[ i ].GetOutputNode().DeactivateNode( deactivatedPort == -1 ? m_inputPorts[ i ].PortId : deactivatedPort, false );
					}
				}
			}
		}

		public Rect GlobalToLocalPosition( DrawInfo drawInfo )
		{
			float width = m_globalPosition.width / drawInfo.InvertedZoom;
			float height = m_globalPosition.height / drawInfo.InvertedZoom;

			float x = m_globalPosition.x / drawInfo.InvertedZoom - drawInfo.CameraOffset.x;
			float y = m_globalPosition.y / drawInfo.InvertedZoom - drawInfo.CameraOffset.y;
			return new Rect( x, y, width, height );
		}

		protected void CalculatePositionAndVisibility( DrawInfo drawInfo )
		{
			//m_movingInFrame = false;
			m_globalPosition = m_position;
			m_globalPosition.x = drawInfo.InvertedZoom * ( m_globalPosition.x + drawInfo.CameraOffset.x );
			m_globalPosition.y = drawInfo.InvertedZoom * ( m_globalPosition.y + drawInfo.CameraOffset.y );
			m_globalPosition.width *= drawInfo.InvertedZoom;
			m_globalPosition.height *= drawInfo.InvertedZoom;

			m_isVisible = ( m_globalPosition.x + m_globalPosition.width > 0 ) &&
							( m_globalPosition.x < drawInfo.CameraArea.width ) &&
							( m_globalPosition.y + m_globalPosition.height > 0 ) &&
							( m_globalPosition.y < drawInfo.CameraArea.height );

			if( m_isMoving > 0 && drawInfo.CurrentEventType != EventType.MouseDrag )
			{
				float deltaX = Mathf.Abs( m_lastPosition.x - m_position.x );
				float deltaY = Mathf.Abs( m_lastPosition.y - m_position.y );
				if( deltaX < 0.01f && deltaY < 0.01f )
				{
					m_isMoving -= 1;
					if( m_isMoving == 0 )
					{
						OnSelfStoppedMovingEvent();
					}
				}
				else
				{
					m_isMoving = MoveCountBuffer;
				}
				m_lastPosition = m_position;
			}
		}

		public void FireStoppedMovingEvent( bool testOnlySelected, InteractionMode interactionMode )
		{
			if( OnNodeStoppedMovingEvent != null )
				OnNodeStoppedMovingEvent( this, testOnlySelected, interactionMode );
		}

		public virtual void OnSelfStoppedMovingEvent()
		{
			FireStoppedMovingEvent( true, m_defaultInteractionMode );
		}

		protected void DrawPrecisionProperty() { m_currentPrecisionType = ( PrecisionType ) EditorGUILayoutEnumPopup( PrecisionContante, m_currentPrecisionType ); }

		public virtual void DrawTitle( Rect titlePos )
		{
			if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
			{
				GUI.Label( titlePos, m_content, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
			}
		}

		public virtual void DrawPreview( DrawInfo drawInfo, Rect rect )
		{
			//if ( !m_drawPreview )
			//	return;

			if( m_cachedDrawSphereId == -1 )
				m_cachedDrawSphereId = Shader.PropertyToID( "_DrawSphere" );

			if( m_cachedInvertedZoomId == -1 )
				m_cachedInvertedZoomId = Shader.PropertyToID( "_InvertedZoom" );

			m_channelNumber = 0;
			Vector4 mask = Vector4.one;
			if( m_outputPorts.Count > 0 )
			{
				switch( m_outputPorts[ 0 ].DataType )
				{
					case WirePortDataType.FLOAT:
					m_channelNumber = 1;
					mask.Set( 1, 1, 1, 0 );
					break;
					case WirePortDataType.FLOAT2:
					m_channelNumber = 2;
					mask.Set( m_previewChannels[ 0 ] ? 1 : 0, m_previewChannels[ 1 ] ? 1 : 0, 1, 0 );
					break;
					case WirePortDataType.COLOR:
					case WirePortDataType.FLOAT4:
					case WirePortDataType.SAMPLER1D:
					case WirePortDataType.SAMPLER2D:
					case WirePortDataType.SAMPLER3D:
					case WirePortDataType.SAMPLERCUBE:
					m_channelNumber = 4;
					mask.Set( m_previewChannels[ 0 ] ? 1 : 0, m_previewChannels[ 1 ] ? 1 : 0, m_previewChannels[ 2 ] ? 1 : 0, m_previewChannels[ 3 ] ? 1 : 0 );
					break;
					default:
					m_channelNumber = 3;
					mask.Set( m_previewChannels[ 0 ] ? 1 : 0, m_previewChannels[ 1 ] ? 1 : 0, m_previewChannels[ 2 ] ? 1 : 0, 0 );
					break;
				}
			}

			UIUtils.LinearMaterial.SetFloat( m_cachedDrawSphereId, ( SpherePreview ? 1 : 0 ) );
			UIUtils.LinearMaterial.SetFloat( m_cachedInvertedZoomId, drawInfo.InvertedZoom );
			UIUtils.LinearMaterial.SetVector( "_Mask", mask );
			EditorGUI.DrawPreviewTexture( rect, PreviewTexture, UIUtils.LinearMaterial );

			//Preview buttons
			if( m_drawPreviewMaskButtons )
				DrawPreviewMaskButtonsRepaint( drawInfo, rect );
		}

		protected void DrawPreviewMaskButtonsLayout( DrawInfo drawInfo, Rect rect )
		{
			if( rect.Contains( drawInfo.MousePosition ) && m_channelNumber > 1 && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
			{
				Rect buttonRect = rect;
				buttonRect.height = 14 * drawInfo.InvertedZoom;
				buttonRect.y = rect.yMax - buttonRect.height;
				buttonRect.width = 14 * drawInfo.InvertedZoom;

				if( m_channelNumber == 2 )
				{
					m_previewChannels[ 0 ] = GUI.Toggle( buttonRect, m_previewChannels[ 0 ], string.Empty, GUIStyle.none );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					m_previewChannels[ 1 ] = GUI.Toggle( buttonRect, m_previewChannels[ 1 ], string.Empty, GUIStyle.none );
				}
				else if( m_channelNumber == 3 )
				{
					m_previewChannels[ 0 ] = GUI.Toggle( buttonRect, m_previewChannels[ 0 ], string.Empty, GUIStyle.none );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					m_previewChannels[ 1 ] = GUI.Toggle( buttonRect, m_previewChannels[ 1 ], string.Empty, GUIStyle.none );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					m_previewChannels[ 2 ] = GUI.Toggle( buttonRect, m_previewChannels[ 2 ], string.Empty, GUIStyle.none );
				}
				else if( m_channelNumber == 4 )
				{
					m_previewChannels[ 0 ] = GUI.Toggle( buttonRect, m_previewChannels[ 0 ], string.Empty, GUIStyle.none );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					m_previewChannels[ 1 ] = GUI.Toggle( buttonRect, m_previewChannels[ 1 ], string.Empty, GUIStyle.none );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					m_previewChannels[ 2 ] = GUI.Toggle( buttonRect, m_previewChannels[ 2 ], string.Empty, GUIStyle.none );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					m_previewChannels[ 3 ] = GUI.Toggle( buttonRect, m_previewChannels[ 3 ], string.Empty, GUIStyle.none );
				}
			}
		}

		protected void DrawPreviewMaskButtonsRepaint( DrawInfo drawInfo, Rect rect )
		{
			if( drawInfo.CurrentEventType == EventType.Repaint && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 && rect.Contains( drawInfo.MousePosition ) && m_channelNumber > 1 )
			{
				Rect buttonRect = rect;
				buttonRect.height = 14 * drawInfo.InvertedZoom;
				buttonRect.y = rect.yMax - buttonRect.height;
				buttonRect.width = 14 * drawInfo.InvertedZoom;

				if( m_channelNumber == 2 )
				{
					UIUtils.MiniButtonTopMid.Draw( buttonRect, "R", false, false, m_previewChannels[ 0 ], false );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					UIUtils.MiniButtonTopRight.Draw( buttonRect, "G", false, false, m_previewChannels[ 1 ], false );
				}
				else if( m_channelNumber == 3 )
				{
					UIUtils.MiniButtonTopMid.Draw( buttonRect, "R", false, false, m_previewChannels[ 0 ], false );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					UIUtils.MiniButtonTopMid.Draw( buttonRect, "G", false, false, m_previewChannels[ 1 ], false );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					UIUtils.MiniButtonTopRight.Draw( buttonRect, "B", false, false, m_previewChannels[ 2 ], false );
				}
				else if( m_channelNumber == 4 )
				{
					UIUtils.MiniButtonTopMid.Draw( buttonRect, "R", false, false, m_previewChannels[ 0 ], false );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					UIUtils.MiniButtonTopMid.Draw( buttonRect, "G", false, false, m_previewChannels[ 1 ], false );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					UIUtils.MiniButtonTopMid.Draw( buttonRect, "B", false, false, m_previewChannels[ 2 ], false );
					buttonRect.x += 14 * drawInfo.InvertedZoom;
					UIUtils.MiniButtonTopRight.Draw( buttonRect, "A", false, false, m_previewChannels[ 3 ], false );
				}
			}
		}

		/// <summary>
		/// This method should only be called to calculate layouts of elements to be draw later, only runs once per frame and before wires are drawn
		/// </summary>
		/// <param name="drawInfo"></param>
		public virtual void OnNodeLayout( DrawInfo drawInfo )
		{
			if( m_repopulateDictionaries )
			{
				m_repopulateDictionaries = false;

				m_inputPortsDict.Clear();
				int inputCount = m_inputPorts.Count;
				for( int i = 0; i < inputCount; i++ )
				{
					m_inputPortsDict.Add( m_inputPorts[ i ].PortId, m_inputPorts[ i ] );
				}

				m_outputPortsDict.Clear();
				int outputCount = m_outputPorts.Count;
				for( int i = 0; i < outputCount; i++ )
				{
					m_outputPortsDict.Add( m_outputPorts[ i ].PortId, m_outputPorts[ i ] );
				}
			}

			if( ContainerGraph.ChangedLightingModel )
			{
				m_sizeIsDirty = true;
				m_firstPreviewDraw = true;
			}

			if( m_firstDraw )
			{
				m_firstDraw = false;
				AfterCommonInit();
				OnNodeChange();
			}

			if( m_previousErrorMessage != m_showErrorMessage )
			{
				m_sizeIsDirty = true;
			}

			if( m_sizeIsDirty )
			{
				m_sizeIsDirty = false;
				ChangeSize();
			}

			CalculatePositionAndVisibility( drawInfo );

			m_unscaledRemainingBox = m_position;
			m_remainingBox = m_globalPosition;

			m_lastInputBottomRight = m_position.position;
			m_lastOutputBottomLeft = m_position.position;
			m_lastOutputBottomLeft.x += m_position.width;

			m_visibleInputs = 0;
			m_visibleOutputs = 0;

			if( m_additionalContent.text.Length > 0 )
				m_extraHeaderHeight = ( int ) Constants.NODE_HEADER_EXTRA_HEIGHT;
			else
				m_extraHeaderHeight = 0;

			m_lastInputBottomRight.y += UIUtils.HeaderMaxHeight + m_extraHeaderHeight;
			m_lastOutputBottomLeft.y += UIUtils.HeaderMaxHeight + m_extraHeaderHeight;
			m_unscaledRemainingBox.y += UIUtils.HeaderMaxHeight + m_extraHeaderHeight;

			if( m_isVisible )
			{
				// Header
				m_headerPosition = m_globalPosition;
				m_headerPosition.height = UIUtils.CurrentHeaderHeight + m_extraHeaderHeight * drawInfo.InvertedZoom;

				// Title
				m_titlePos = m_globalPosition;
				m_titlePos.height = m_headerPosition.height;
				if( m_additionalContent.text.Length > 0 )
					m_titlePos.yMin += ( 4 * drawInfo.InvertedZoom );
				else
					m_titlePos.yMin += ( 8 * drawInfo.InvertedZoom );
				m_titlePos.width -= ( m_paddingTitleLeft + m_paddingTitleRight ) * drawInfo.InvertedZoom;
				m_titlePos.x += m_paddingTitleLeft * drawInfo.InvertedZoom;

				// Additional Title
				if( m_additionalContent.text.Length > 0 )
				{
					m_addTitlePos = m_titlePos;
					m_addTitlePos.y = m_globalPosition.y;
					m_addTitlePos.yMin += ( 19 * drawInfo.InvertedZoom );
				}

				// Expander
				if( m_drawPreviewExpander && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD4 )
				{
					m_expandRect = m_globalPosition;
					m_expandRect.width = Constants.PreviewExpanderWidth * drawInfo.InvertedZoom;
					m_expandRect.x = m_globalPosition.x + m_globalPosition.width - ( Constants.IconsLeftRightMargin + Constants.PreviewExpanderWidth ) * drawInfo.InvertedZoom; //titlePos.x + titlePos.width;
					m_expandRect.height = Constants.PreviewExpanderHeight * drawInfo.InvertedZoom;
					m_expandRect.y = m_globalPosition.y + m_headerPosition.height * 0.5f - Constants.PreviewExpanderHeight * drawInfo.InvertedZoom * 0.5f;
				}
			}

			if( m_anchorAdjust < 0 )
			{
				m_anchorAdjust = UIUtils.GetCustomStyle( CustomStyle.PortEmptyIcon ).normal.background.width;
			}

			m_unscaledRemainingBox.y += Constants.INPUT_PORT_DELTA_Y;
			m_lastOutputBottomLeft.y += Constants.INPUT_PORT_DELTA_Y;
			m_lastInputBottomRight.y += Constants.INPUT_PORT_DELTA_Y;

			// Input Ports
			{
				m_currInputPortPos = m_globalPosition;
				m_currInputPortPos.width = drawInfo.InvertedZoom * UIUtils.PortsSize.x;
				m_currInputPortPos.height = drawInfo.InvertedZoom * UIUtils.PortsSize.y;

				m_currInputPortPos.x += drawInfo.InvertedZoom * Constants.PORT_INITIAL_X;
				m_currInputPortPos.y += drawInfo.InvertedZoom * Constants.PORT_INITIAL_Y + m_extraHeaderHeight * drawInfo.InvertedZoom;
				int inputCount = m_inputPorts.Count;

				float initialX = m_lastInputBottomRight.x;

				for( int i = 0; i < inputCount; i++ )
				{
					if( m_inputPorts[ i ].Visible )
					{
						m_visibleInputs++;
						// Button
						m_inputPorts[ i ].Position = m_currInputPortPos;

						// Label
						m_inputPorts[ i ].LabelPosition = m_currInputPortPos;
						float deltaX = 1f * drawInfo.InvertedZoom * ( UIUtils.PortsSize.x + Constants.PORT_TO_LABEL_SPACE_X );
						m_auxRect = m_inputPorts[ i ].LabelPosition;
						m_auxRect.x += deltaX;
						m_inputPorts[ i ].LabelPosition = m_auxRect;

						if( m_inputPorts[ i ].DirtyLabelSize || m_inputPorts[ i ].LabelSize == Vector2.zero )
						{
							m_inputPorts[ i ].DirtyLabelSize = false;
							m_sizeContentAux.text = m_inputPorts[ i ].Name;
							m_inputPorts[ i ].UnscaledLabelSize = UIUtils.UnZoomedInputPortStyle.CalcSize( m_sizeContentAux );
						}

						m_inputPorts[ i ].LabelSize = m_inputPorts[ i ].UnscaledLabelSize * drawInfo.InvertedZoom;

						m_lastInputBottomRight.x = Mathf.Max( m_lastInputBottomRight.x, initialX + m_inputPorts[ i ].UnscaledLabelSize.x + Constants.PORT_INITIAL_X + Constants.PORT_TO_LABEL_SPACE_X + UIUtils.PortsSize.x );

						if( !m_inputPorts[ i ].Locked )
						{
							float overflow = 2;
							float scaledOverflow = 3 * drawInfo.InvertedZoom;
							m_auxRect = m_currInputPortPos;
							m_auxRect.yMin -= scaledOverflow + overflow;
							m_auxRect.yMax += scaledOverflow + overflow;
							m_auxRect.xMin -= Constants.PORT_INITIAL_X * drawInfo.InvertedZoom + scaledOverflow + overflow;
							m_auxRect.xMax += m_inputPorts[ i ].LabelSize.x + Constants.PORT_TO_LABEL_SPACE_X * drawInfo.InvertedZoom + scaledOverflow + overflow;
							m_inputPorts[ i ].ActivePortArea = m_auxRect;
						}
						m_currInputPortPos.y += drawInfo.InvertedZoom * ( m_fontHeight + Constants.INPUT_PORT_DELTA_Y );
					}
				}
				if( m_visibleInputs > 0 )
					m_lastInputBottomRight.y += m_fontHeight * m_visibleInputs + Constants.INPUT_PORT_DELTA_Y * ( m_visibleInputs - 1 );
			}

			// Output Ports
			{
				m_currOutputPortPos = m_globalPosition;
				m_currOutputPortPos.width = drawInfo.InvertedZoom * UIUtils.PortsSize.x;
				m_currOutputPortPos.height = drawInfo.InvertedZoom * UIUtils.PortsSize.y;

				m_currOutputPortPos.x += ( m_globalPosition.width - drawInfo.InvertedZoom * ( Constants.PORT_INITIAL_X + m_anchorAdjust ) );
				m_currOutputPortPos.y += drawInfo.InvertedZoom * Constants.PORT_INITIAL_Y + m_extraHeaderHeight * drawInfo.InvertedZoom;
				int outputCount = m_outputPorts.Count;

				float initialX = m_lastOutputBottomLeft.x;

				for( int i = 0; i < outputCount; i++ )
				{
					if( m_outputPorts[ i ].Visible )
					{
						m_visibleOutputs++;
						//Button
						m_outputPorts[ i ].Position = m_currOutputPortPos;

						// Label
						m_outputPorts[ i ].LabelPosition = m_currOutputPortPos;
						float deltaX = 1f * drawInfo.InvertedZoom * ( UIUtils.PortsSize.x + Constants.PORT_TO_LABEL_SPACE_X );
						m_auxRect = m_outputPorts[ i ].LabelPosition;
						m_auxRect.x -= deltaX;
						m_outputPorts[ i ].LabelPosition = m_auxRect;


						//GUIStyle outputPortStyle = UIUtils.GetCustomStyle( CustomStyle.OutputPortLabel );
						if( m_outputPorts[ i ].DirtyLabelSize || m_outputPorts[ i ].LabelSize == Vector2.zero )
						{
							m_outputPorts[ i ].DirtyLabelSize = false;
							m_sizeContentAux.text = m_outputPorts[ i ].Name;
							m_outputPorts[ i ].UnscaledLabelSize = UIUtils.UnZoomedOutputPortPortStyle.CalcSize( m_sizeContentAux );
						}

						m_outputPorts[ i ].LabelSize = m_outputPorts[ i ].UnscaledLabelSize * drawInfo.InvertedZoom;

						m_lastOutputBottomLeft.x = Mathf.Min( m_lastOutputBottomLeft.x, initialX - m_outputPorts[ i ].UnscaledLabelSize.x - Constants.PORT_INITIAL_X - Constants.PORT_TO_LABEL_SPACE_X - UIUtils.PortsSize.x );

						if( !m_outputPorts[ i ].Locked )
						{
							float overflow = 2;
							float scaledOverflow = 3 * drawInfo.InvertedZoom;
							m_auxRect = m_currOutputPortPos;
							m_auxRect.yMin -= scaledOverflow + overflow;
							m_auxRect.yMax += scaledOverflow + overflow;
							m_auxRect.xMin -= m_outputPorts[ i ].LabelSize.x + Constants.PORT_TO_LABEL_SPACE_X * drawInfo.InvertedZoom + scaledOverflow + overflow;
							m_auxRect.xMax += Constants.PORT_INITIAL_X * drawInfo.InvertedZoom + scaledOverflow + overflow;
							m_outputPorts[ i ].ActivePortArea = m_auxRect;
						}
						m_currOutputPortPos.y += drawInfo.InvertedZoom * ( m_fontHeight + Constants.INPUT_PORT_DELTA_Y );
					}
				}
				if( m_visibleOutputs > 0 )
					m_lastOutputBottomLeft.y += m_fontHeight * m_visibleOutputs + Constants.INPUT_PORT_DELTA_Y * ( m_visibleOutputs - 1 );
			}

			m_lastInputBottomRight.x += m_marginPreviewLeft;

			//Vector2 scaledLastOutputBottomLeft = ( m_lastOutputBottomLeft + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
			//GUI.Label( new Rect( scaledLastOutputBottomLeft, Vector2.one * 2 ), string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );

			m_unscaledRemainingBox.xMin = m_lastInputBottomRight.x;
			//m_unscaledRemainingBox.yMin = m_lastInputBottomRight.y;
			m_unscaledRemainingBox.xMax = m_lastOutputBottomLeft.x;
			m_unscaledRemainingBox.yMax = Mathf.Max( m_lastOutputBottomLeft.y, m_lastInputBottomRight.y );

			m_remainingBox.position = ( m_unscaledRemainingBox.position + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
			m_remainingBox.size = m_unscaledRemainingBox.size * drawInfo.InvertedZoom;

			//GUI.Label( m_remainingBox, string.Empty, UIUtils.Box );

			if( m_visibleInputs == 0 )
			{
				m_remainingBox.x += Constants.PORT_INITIAL_X * drawInfo.InvertedZoom;
				m_remainingBox.width -= Constants.PORT_INITIAL_X * drawInfo.InvertedZoom;
			}

			if( m_visibleOutputs == 0 )
			{
				m_remainingBox.width -= Constants.PORT_INITIAL_X * drawInfo.InvertedZoom;
			}

			if( ContainerGraph.ParentWindow.GlobalPreview != m_globalShowPreview )
			{
				m_globalShowPreview = ContainerGraph.ParentWindow.GlobalPreview;
				m_sizeIsDirty = true;
			}

			// Generate Proper Preview Rect
			float marginAround = 10;
			float scaledMarginAround = marginAround * drawInfo.InvertedZoom;
			float previewSize = 128;
			PreviewLocation m_autoLocation = m_selectedLocation;
			if( m_selectedLocation == PreviewLocation.Auto )
			{
				if( m_visibleOutputs > m_visibleInputs )
				{
					m_autoLocation = PreviewLocation.Left;
				}
				else if( m_visibleOutputs < m_visibleInputs )
				{
					m_autoLocation = PreviewLocation.Right;
				}
				else if( m_unscaledRemainingBox.width > previewSize )
				{
					m_autoLocation = PreviewLocation.TopCenter;
				}
				else
				{
					m_autoLocation = PreviewLocation.BottomCenter;
				}
			}

			if( m_canExpand && ( m_showPreview || m_globalShowPreview ) )
			{
				if( m_autoLocation == PreviewLocation.TopCenter )
				{
					m_unscaledPreviewRect.y = m_unscaledRemainingBox.y;
					m_unscaledPreviewRect.x = m_unscaledRemainingBox.center.x - 0.5f * ( previewSize + 2 * marginAround );
				}
				else if( m_autoLocation == PreviewLocation.BottomCenter )
				{
					m_unscaledPreviewRect.y = Mathf.Max( m_lastOutputBottomLeft.y, m_lastInputBottomRight.y );
					m_unscaledPreviewRect.x = m_position.x + 0.5f * m_position.width - 0.5f * ( previewSize + 2 * marginAround );
				}
				else if( m_autoLocation == PreviewLocation.Left )
				{
					m_unscaledPreviewRect.y = m_lastInputBottomRight.y;
					m_unscaledPreviewRect.x = m_position.x;
				}
				else if( m_autoLocation == PreviewLocation.Right )
				{
					m_unscaledPreviewRect.y = m_lastOutputBottomLeft.y;
					m_unscaledPreviewRect.x = m_lastInputBottomRight.x;
				}
				if( m_autoLocation == PreviewLocation.BottomCenter )
					m_unscaledPreviewRect.height = previewSize + 2 * marginAround;
				else if( m_autoLocation == PreviewLocation.TopCenter )
					m_unscaledPreviewRect.height = previewSize + marginAround;
				else
					m_unscaledPreviewRect.height = previewSize + ( m_visibleInputs > 0 && m_visibleOutputs > 0 ? 2 * marginAround : marginAround );
				m_unscaledPreviewRect.width = previewSize + 2 * marginAround;

				m_previewRect = m_unscaledPreviewRect;
				m_previewRect.position = ( m_previewRect.position + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
				m_auxVector2.Set( previewSize * drawInfo.InvertedZoom, previewSize * drawInfo.InvertedZoom );
				m_previewRect.size = m_auxVector2;

				if( m_autoLocation == PreviewLocation.BottomCenter )
				{
					m_auxVector2.Set( m_previewRect.position.x + scaledMarginAround, m_previewRect.position.y + scaledMarginAround );
					m_previewRect.position = m_auxVector2;
				}
				else if( m_autoLocation == PreviewLocation.TopCenter )
				{
					m_auxVector2.Set( m_previewRect.position.x + scaledMarginAround, m_previewRect.position.y );
					m_previewRect.position = m_auxVector2;
				}
				else
				{
					m_previewRect.position += new Vector2( scaledMarginAround, ( m_visibleInputs > 0 && m_visibleOutputs > 0 ? scaledMarginAround : 0 ) );
				}
			}

			// Adjust node rect after preview
			if( m_firstPreviewDraw )
			{
				m_firstPreviewDraw = false;
				if( m_canExpand && ( m_showPreview || m_globalShowPreview ) )
				{
					if( m_autoLocation == PreviewLocation.TopCenter )
					{
						float fillWidth = m_unscaledRemainingBox.width - m_unscaledPreviewRect.width;
						m_extraSize.x = Mathf.Max( -fillWidth, 0 );
						float fillHeight = m_position.yMax - m_unscaledPreviewRect.yMax;
						m_extraSize.y = Mathf.Max( -fillHeight, 0 );
					}
					if( m_autoLocation == PreviewLocation.BottomCenter )
					{
						float fillWidth = m_position.width - m_unscaledPreviewRect.width;
						m_extraSize.x = Mathf.Max( -fillWidth, 0 );
						float fillHeight = m_position.yMax - m_unscaledPreviewRect.yMax;
						m_extraSize.y = Mathf.Max( -fillHeight, 0 );
					}
					else if( m_autoLocation == PreviewLocation.Left )
					{
						float fillWidth = m_lastOutputBottomLeft.x - m_unscaledPreviewRect.xMax;
						m_extraSize.x = Mathf.Max( -fillWidth, 0 );
						float fillHeight = m_position.yMax - m_unscaledPreviewRect.yMax;
						m_extraSize.y = Mathf.Max( -fillHeight, 0 );
					}
					else if( m_autoLocation == PreviewLocation.Right )
					{
						float fillWidth = m_position.xMax - m_unscaledPreviewRect.xMax;
						m_extraSize.x = Mathf.Max( -fillWidth, 0 );
						float fillHeight = m_position.yMax - m_unscaledPreviewRect.yMax;
						m_extraSize.y = Mathf.Max( -fillHeight, 0 );
					}

					if( m_showErrorMessage )
						m_extraSize.y += 24;
				}
				else if( m_canExpand )
				{
					m_extraSize.y = 0;
					m_extraSize.x = 0;
				}

				m_position.width = m_unpreviewedPosition.width + m_extraSize.x;
				m_position.height = m_unpreviewedPosition.height + m_extraSize.y;
			}


			if( m_showErrorMessage )
			{
				m_errorBox = m_globalPosition;
				m_errorBox.y = ( m_globalPosition.yMax - 28 * drawInfo.InvertedZoom ) + 3 * drawInfo.InvertedZoom;
				m_errorBox.height = 25 * drawInfo.InvertedZoom;
			}

			m_previousErrorMessage = m_showErrorMessage;
		}

		/// <summary>
		/// This method should only be called to draw elements, runs once per frame and after wires are drawn
		/// </summary>
		/// <param name="drawInfo"></param>
		public virtual void OnNodeRepaint( DrawInfo drawInfo )
		{
			if( !m_isVisible )
				return;

			m_colorBuffer = GUI.color;
			// Background
			GUI.color = m_infiniteLoopDetected ? Constants.InfiniteLoopColor : Constants.NodeBodyColor;
			GUI.Label( m_globalPosition, string.Empty, UIUtils.GetCustomStyle( CustomStyle.NodeWindowOff ) );

			// Header
			//GUI
			GUI.color = m_headerColor * m_headerColorModifier;
			GUI.Label( m_headerPosition, string.Empty, UIUtils.GetCustomStyle( CustomStyle.NodeHeader ) );
			GUI.color = m_colorBuffer;

			// Title
			DrawTitle( m_titlePos );

			// Additional Tile
			if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
				GUI.Label( m_addTitlePos, m_additionalContent, UIUtils.GetCustomStyle( CustomStyle.PropertyValuesTitle ) );

			// Expander
			if( m_drawPreviewExpander && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD4 )
				GUI.Label( m_expandRect, string.Empty, ( m_showPreview ? UIUtils.PreviewCollapser : UIUtils.PreviewExpander ) );

			// Input Ports
			int inputCount = m_inputPorts.Count;
			GUIStyle inputPortStyle = UIUtils.GetCustomStyle( CustomStyle.InputPortlabel );

			for( int i = 0; i < inputCount; i++ )
			{
				if( m_inputPorts[ i ].Visible )
				{
					// Input Port Icon
					if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD4 )
					{
						if( m_inputPorts[ i ].Locked )
							GUI.color = Constants.LockedPortColor;
						else if( ContainerGraph.ParentWindow.Options.ColoredPorts )
							GUI.color = UIUtils.GetColorForDataType( m_inputPorts[ i ].DataType, false, true );
						else
							GUI.color = m_inputPorts[ i ].HasCustomColor ? m_inputPorts[ i ].CustomColor : UIUtils.GetColorForDataType( m_inputPorts[ i ].DataType, true, true );

						GUIStyle style = m_inputPorts[ i ].IsConnected ? UIUtils.GetCustomStyle( CustomStyle.PortFullIcon ) : UIUtils.GetCustomStyle( CustomStyle.PortEmptyIcon );
						GUI.Label( m_inputPorts[ i ].Position, string.Empty, style );

						GUI.color = m_colorBuffer;
					}

					// Input Port Label
					if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
					{
						if( m_inputPorts[ i ].Locked )
						{
							GUI.color = Constants.PortLockedTextColor;
							GUI.Label( m_inputPorts[ i ].LabelPosition, m_inputPorts[ i ].Name, inputPortStyle );
							GUI.color = m_colorBuffer;
						}
						else
						{
							GUI.Label( m_inputPorts[ i ].LabelPosition, m_inputPorts[ i ].Name, inputPortStyle );
						}
					}
				}
			}

			// Output Ports
			int outputCount = m_outputPorts.Count;
			GUIStyle outputPortStyle = UIUtils.GetCustomStyle( CustomStyle.OutputPortLabel );

			for( int i = 0; i < outputCount; i++ )
			{
				if( m_outputPorts[ i ].Visible )
				{
					// Output Port Icon
					if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD4 )
					{
						if( m_outputPorts[ i ].Locked )
							GUI.color = Constants.LockedPortColor;
						else if( ContainerGraph.ParentWindow.Options.ColoredPorts )
							GUI.color = UIUtils.GetColorForDataType( m_outputPorts[ i ].DataType, false, false );
						else
							GUI.color = m_outputPorts[ i ].HasCustomColor ? m_outputPorts[ i ].CustomColor : UIUtils.GetColorForDataType( m_outputPorts[ i ].DataType, true, false );

						GUIStyle style = m_outputPorts[ i ].IsConnected ? UIUtils.GetCustomStyle( CustomStyle.PortFullIcon ) : UIUtils.GetCustomStyle( CustomStyle.PortEmptyIcon );
						GUI.Label( m_outputPorts[ i ].Position, string.Empty, style );

						GUI.color = m_colorBuffer;
					}

					// Output Port Label
					if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
					{
						if( m_outputPorts[ i ].Locked )
						{
							GUI.color = Constants.PortLockedTextColor;
							GUI.Label( m_outputPorts[ i ].LabelPosition, m_outputPorts[ i ].Name, outputPortStyle );
							GUI.color = m_colorBuffer;
						}
						else
						{
							GUI.Label( m_outputPorts[ i ].LabelPosition, m_outputPorts[ i ].Name, outputPortStyle );
						}
					}
				}
			}

			// Preview
			if( ( m_showPreview || m_globalShowPreview ) && m_drawPreview )
				DrawPreview( drawInfo, m_previewRect );

			// Error and Warning bottom message
			if( m_showErrorMessage )
			{
				GUI.color = new Color( 0.0f, 0.0f, 0.0f, 0.5f );
				GUI.Label( m_errorBox, string.Empty, UIUtils.Separator );
				GUI.color = m_colorBuffer;

				GUIContent icon = new GUIContent();
				GUIContent message;
				GUIStyle currentStyle;

				switch( m_errorMessageTypeIsError )
				{
					default:
					case NodeMessageType.Error:
					{
						message = m_errorContent;
						icon.image = UIUtils.SmallErrorIcon;
						currentStyle = UIUtils.BoldErrorStyle;
					}
					break;
					case NodeMessageType.Warning:
					{
						message = m_warningContent;
						icon.image = UIUtils.SmallWarningIcon;
						currentStyle = UIUtils.BoldWarningStyle;
					}
					break;
					case NodeMessageType.Info:
					{
						message = m_infoContent;
						icon.image = UIUtils.SmallInfoIcon;
						currentStyle = UIUtils.BoldInfoStyle;
					}
					break;
				}

				Rect textBox = m_errorBox;
				textBox.y += 1 * drawInfo.InvertedZoom;
				textBox.height = 24 * drawInfo.InvertedZoom;

				float textWidth = currentStyle.CalcSize( message ).x;

				GUI.Label( textBox, message, currentStyle );
				textBox.x -= textWidth * 0.5f + 12 * drawInfo.InvertedZoom;
				GUI.Label( textBox, icon, currentStyle );
				textBox.x += textWidth + 24 * drawInfo.InvertedZoom;
				GUI.Label( textBox, icon, currentStyle );
			}

			// Selection Box
			if( m_selected )
			{
				GUI.color = m_infiniteLoopDetected ? Constants.InfiniteLoopColor : m_colorBuffer;
				GUI.Label( m_globalPosition, string.Empty, UIUtils.GetCustomStyle( CustomStyle.NodeWindowOn ) );
				GUI.color = m_colorBuffer;
			}

			// Debug Visualizers
			//GUI.Label( m_remainingBox, string.Empty, UIUtils.Box );
		}

		/// <summary>
		/// Handles gui controls, runs before node layout
		/// </summary>
		/// <param name="drawInfo"></param>
		public virtual void DrawGUIControls( DrawInfo drawInfo )
		{
			if( !m_initialized )
				return;

			if( !m_isVisible )
				return;

			if( drawInfo.CurrentEventType == EventType.MouseDown && drawInfo.LeftMouseButtonPressed/*&& !ContainerGraph.ParentWindow.MouseInteracted */&& m_expandRect.Contains( drawInfo.MousePosition ) )
			{
				m_showPreview = !m_showPreview;
				m_sizeIsDirty = true;
				ContainerGraph.ParentWindow.MouseInteracted = true;
			}
			//else
			//{
			//	EditorGUIUtility.AddCursorRect( m_expandRect, MouseCursor.SlideArrow );
			//}

			DrawGuiPorts( drawInfo );
		}

		public static bool MyRepeatButton( DrawInfo drawInfo, Rect position, string text, GUIStyle style )
		{
			if(/* drawInfo.CurrentEventType == EventType.MouseDown &&*/ position.Contains( drawInfo.MousePosition ) )
			{
				UIUtils.CurrentWindow.MouseInteracted = true;
				return true;
			}
			return false;
		}

		public void DrawGuiPorts( DrawInfo drawInfo )
		{
			if( !m_initialized )
				return;

			if( !m_isVisible )
				return;

			if( drawInfo.CurrentEventType == EventType.MouseDown )
			{
				int inputCount = m_inputPorts.Count;
				int outputCount = m_outputPorts.Count;

				for( int i = 0; i < inputCount; i++ )
				{
					if( m_inputPorts[ i ].Visible && !m_inputPorts[ i ].Locked && m_isVisible && MyRepeatButton( drawInfo, m_inputPorts[ i ].ActivePortArea, string.Empty, GUIStyle.none ) && drawInfo.LeftMouseButtonPressed )
					{
						m_inputPorts[ i ].Click();
						// need to put the mouse button on a hot state so it will detect the Mouse Up event correctly on the Editor Window
						int controlID = GUIUtility.GetControlID( FocusType.Passive );
						//int controlID = GUIUtility.GetControlID( "repeatButton".GetHashCode(), FocusType.Passive, m_inputPorts[ i ].ActivePortArea );
						GUIUtility.hotControl = controlID;

						bool saveReference = true;
						if( m_inputPorts[ i ].IsConnected )
						{
							double doubleTapTime = EditorApplication.timeSinceStartup;
							bool doubleTap = ( doubleTapTime - m_doubleClickTimestamp ) < DoubleClickTime;
							m_doubleClickTimestamp = doubleTapTime;

							if( doubleTap )
							{
								m_containerGraph.DeleteConnection( true, UniqueId, m_inputPorts[ i ].PortId, true, true );
								Event.current.Use();
							}
							else
							//if ( AppyModifierToPort( _inputPorts[ i ], true ) )
							//{
							//saveReference = false;
							//}
							if( !AppyModifierToPort( m_inputPorts[ i ], true ) )
							{
								UIUtils.ShowContextOnPick = false;
								PickInput( m_inputPorts[ i ] );
							}
							saveReference = false;
						}

						if( saveReference && !m_containerGraph.ParentWindow.WireReferenceUtils.InputPortReference.IsValid )
						//if ( !modifierApplied && !UIUtils.InputPortReference.IsValid )
						{
							m_containerGraph.ParentWindow.WireReferenceUtils.InputPortReference.SetReference( m_uniqueId, m_inputPorts[ i ].PortId, m_inputPorts[ i ].DataType, m_inputPorts[ i ].TypeLocked );
						}

						IsDirty = true;
						inputCount = m_inputPorts.Count;
					}
				}

				for( int i = 0; i < outputCount; i++ )
				{
					if( m_outputPorts[ i ].Visible && MyRepeatButton( drawInfo, m_outputPorts[ i ].ActivePortArea, string.Empty, GUIStyle.none ) && drawInfo.LeftMouseButtonPressed )
					{
						m_outputPorts[ i ].Click();
						// need to put the mouse button on a hot state so it will detect the Mouse Up event correctly on the Editor Window
						int controlID = GUIUtility.GetControlID( FocusType.Passive );
						//int controlID = GUIUtility.GetControlID( "aseRepeatButton".GetHashCode(), FocusType.Passive, m_outputPorts[ i ].ActivePortArea );
						GUIUtility.hotControl = controlID;

						bool saveReference = true;
						if( m_outputPorts[ i ].IsConnected )
						{
							if( AppyModifierToPort( m_outputPorts[ i ], false ) )
							{
								saveReference = false;
							}
						}

						if( saveReference && !m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.IsValid )
						{
							m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.SetReference( m_uniqueId, m_outputPorts[ i ].PortId, m_outputPorts[ i ].DataType, false );
						}

						IsDirty = true;
						outputCount = m_outputPorts.Count;
					}
				}
			}

			//Preview buttons
			if( m_drawPreviewMaskButtons && ( drawInfo.CurrentEventType == EventType.MouseDown || drawInfo.CurrentEventType == EventType.MouseUp ) )
				DrawPreviewMaskButtonsLayout( drawInfo, m_previewRect );
		}

		/// <summary>
		/// Can be used to draw an entire node, runs after wires
		/// </summary>
		/// <param name="drawInfo"></param>
		public virtual void Draw( DrawInfo drawInfo )
		{
			if( !m_initialized )
				return;

			if( drawInfo.CurrentEventType == EventType.Repaint )
				OnNodeRepaint( drawInfo );
		}

		public virtual void SetPreviewInputs()
		{
			if( !HasPreviewShader || !m_initialized )
				return;

			int count = m_inputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_inputPorts[ i ].IsConnected && m_inputPorts[ i ].InputNodeHasPreview() )
				{
					m_inputPorts[ i ].SetPreviewInputTexture();
				}
				else
				{
					m_inputPorts[ i ].SetPreviewInputValue();
				}
			}
		}

		public virtual void AfterPreviewRefresh() { }

		public bool SafeDraw( DrawInfo drawInfo )
		{
			EditorGUI.BeginChangeCheck();
			Draw( drawInfo );
			if( EditorGUI.EndChangeCheck() )
			{
				SaveIsDirty = true;
				return true;
			}
			return false;
		}

		public bool ShowTooltip( DrawInfo drawInfo )
		{
			if( string.IsNullOrEmpty( m_tooltipText ) )
				return false;

			if( m_globalPosition.Contains( drawInfo.MousePosition ) || m_linkVisibility )
			{
				if( m_tooltipTimestamp + 0.6f < Time.realtimeSinceStartup || m_linkVisibility )
				{
					bool errorTooltip = false;
					if( m_showErrorMessage && m_errorBox.Contains( drawInfo.MousePosition ) && !string.IsNullOrEmpty( m_errorMessageTooltip ) )
						errorTooltip = true;

					Rect globalTooltipPos = m_globalPosition;
					GUIContent temp = new GUIContent( errorTooltip ? m_errorMessageTooltip : m_tooltipText );
					UIUtils.TooltipBox.wordWrap = false;
					Vector2 optimal = UIUtils.TooltipBox.CalcSize( temp );
					if( optimal.x > 300f )
					{
						UIUtils.TooltipBox.wordWrap = true;
						optimal.x = 300f;
						optimal.y = UIUtils.TooltipBox.CalcHeight( temp, 300f );
					}

					globalTooltipPos.width = Mathf.Max( 120, optimal.x );
					globalTooltipPos.height = optimal.y;
					globalTooltipPos.center = m_globalPosition.center;

					if( !errorTooltip )
						globalTooltipPos.height += 16;

					if( errorTooltip )
						globalTooltipPos.y = 10 + m_globalPosition.yMax;
					else
						globalTooltipPos.y = m_globalPosition.yMin - 10 - globalTooltipPos.height;

					if( globalTooltipPos.x < 10 )
						globalTooltipPos.x = 10;

					if( globalTooltipPos.x + globalTooltipPos.width > Screen.width - 10 )
						globalTooltipPos.x = Screen.width - globalTooltipPos.width - 10;

					if( globalTooltipPos.y < 40 )
						globalTooltipPos.y = 40;

					if( errorTooltip && globalTooltipPos.y + globalTooltipPos.height > Screen.height - 32 )
						globalTooltipPos.y = Screen.height - 32 - globalTooltipPos.height;

					GUI.Label( globalTooltipPos, temp, UIUtils.TooltipBox );

					if( !errorTooltip )
					{
						Rect link = globalTooltipPos;
						link.y = globalTooltipPos.yMax - 16;
						link.height = 16;
						link.width = 86;
						link.x = globalTooltipPos.center.x - 43;
						Rect hover = globalTooltipPos;
						hover.yMax += 15;// m_globalPosition.yMax;
						m_linkVisibility = hover.Contains( drawInfo.MousePosition );
						if( link.Contains( drawInfo.MousePosition ) )
						{
							if( drawInfo.CurrentEventType == EventType.MouseDown )
							{
								if( m_tooltipTimestamp + 1f < Time.realtimeSinceStartup )
								{
									Application.OpenURL( Attributes.NodeUrl );
								}
							}
							else
							{
								UIUtils.MainSkin.customStyles[ 52 ].Draw( link, "online reference", true, false, false, false );
							}
						}
						else
						{
							GUI.Label( link, "online reference", UIUtils.MainSkin.customStyles[ 52 ] );
						}
					}
					return true;
				}
			}
			else
			{
				if( !m_linkVisibility )
					m_tooltipTimestamp = Time.realtimeSinceStartup;
			}

			return false;
		}

		public virtual bool SafeDrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			PreDrawProperties();
			if( m_autoWrapProperties )
			{
				NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, DrawProperties );
			}
			else
			{
				DrawProperties();
			}
			if( EditorGUI.EndChangeCheck() )
			{
				//UIUtils.RecordObject(this);
				//MarkForPreviewUpdate();
				return true;
			}
			return false;
		}


		public void PreDrawProperties()
		{
			if( ( object ) m_buttonStyle == null )
			{
				m_buttonStyle = UIUtils.Button;
			}

			if( ( object ) m_labelStyle == null )
			{
				m_labelStyle = UIUtils.Label;
			}

			if( ( object ) m_toggleStyle == null )
			{
				m_toggleStyle = UIUtils.Toggle;
			}

			if( ( object ) m_textfieldStyle == null )
			{
				m_textfieldStyle = UIUtils.Textfield;
			}

			if( m_useInternalPortData && m_autoDrawInternalPortData )
			{
				DrawInternalDataGroup();
			}
		}

		virtual public void DrawProperties() { }

		protected void DrawInternalDataGroup()
		{
			bool drawInternalDataUI = false;
			int inputCount = m_inputPorts.Count;
			if( inputCount > 0 )
			{
				for( int i = 0; i < inputCount; i++ )
				{
					if( m_inputPorts[ i ].Available && m_inputPorts[ i ].ValidInternalData && !m_inputPorts[ i ].IsConnected )
					{
						drawInternalDataUI = true;
						break;
					}
				}
			}

			if( drawInternalDataUI )
				NodeUtils.DrawPropertyGroup( ref m_internalDataFoldout, Constants.InternalDataLabelStr, () =>
				{
					for( int i = 0; i < m_inputPorts.Count; i++ )
					{
						if( m_inputPorts[ i ].ValidInternalData && !m_inputPorts[ i ].IsConnected && m_inputPorts[ i ].Visible )
						{
							m_inputPorts[ i ].ShowInternalData( this );
						}
					}
				} );
		}

		protected void PickInput( WirePort port )
		{
			WireReference connection = port.GetConnection( 0 );
			m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.SetReference( connection.NodeId, connection.PortId, connection.DataType, connection.TypeLocked );
			m_containerGraph.DeleteConnection( true, UniqueId, port.PortId, true, true );
			//TODO: check if not necessary
			Event.current.Use();
			IsDirty = true;
			SetSaveIsDirty();
		}

		protected bool AppyModifierToPort( WirePort port, bool isInput )
		{
			bool modifierApplied = false;
			switch( Event.current.modifiers )
			{
				case EventModifiers.Alt:
				{
					m_containerGraph.DeleteConnection( isInput, UniqueId, port.PortId, true, true );
					modifierApplied = true;
					m_containerGraph.ParentWindow.InvalidateAlt();
				}
				break;
				case EventModifiers.Control:
				{
					//WireReference connection = port.GetConnection( 0 );
					//if ( isInput )
					//{
					//	UIUtils.OutputPortReference.SetReference( connection.NodeId, connection.PortId, connection.DataType, connection.TypeLocked );
					//}
					//else
					//{
					//	UIUtils.InputPortReference.SetReference( connection.NodeId, connection.PortId, connection.DataType, connection.TypeLocked );
					//}

					//UIUtils.DeleteConnection( isInput, UniqueId, port.PortId, true );
					//modifierApplied = true;

					if( !isInput )
					{
						WireReference connection = port.GetConnection( 0 );
						m_containerGraph.ParentWindow.WireReferenceUtils.InputPortReference.SetReference( connection.NodeId, connection.PortId, connection.DataType, connection.TypeLocked );
						m_containerGraph.DeleteConnection( isInput, UniqueId, port.PortId, true, true );
						modifierApplied = true;
					}
				}
				break;
			}

			if( isInput )
				m_containerGraph.ParentWindow.WireReferenceUtils.SwitchPortReference.SetReference( port.NodeId, port.PortId, port.DataType, false ); //always save last connection
			else
				m_containerGraph.ParentWindow.WireReferenceUtils.SwitchPortReference.SetReference( -1, -1, WirePortDataType.OBJECT, false ); //invalidate connection

			if( modifierApplied )
			{
				Event.current.Use();
				IsDirty = true;
				SetSaveIsDirty();
			}
			return modifierApplied;
		}

		public void DeleteAllInputConnections( bool alsoDeletePorts )
		{
			int count = m_inputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_inputPorts[ i ].IsConnected )
					m_containerGraph.DeleteConnection( true, UniqueId, m_inputPorts[ i ].PortId, false, true );
			}
			if( alsoDeletePorts )
			{
				m_inputPorts.Clear();
				m_inputPortsDict.Clear();
			}
			SetSaveIsDirty();
		}

		public void DeleteAllOutputConnections( bool alsoDeletePorts )
		{
			int count = m_outputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_outputPorts[ i ].IsConnected )
					m_containerGraph.DeleteConnection( false, UniqueId, m_outputPorts[ i ].PortId, false, true );
			}

			if( alsoDeletePorts )
			{
				m_outputPorts.Clear();
				m_outputPortsDict.Clear();
			}
			SetSaveIsDirty();
		}

		public void DeleteInputPortByArrayIdx( int arrayIdx )
		{
			if( arrayIdx >= m_inputPorts.Count )
				return;

			m_containerGraph.DeleteConnection( true, UniqueId, arrayIdx, false, true );
			m_inputPortsDict.Remove( m_inputPorts[ arrayIdx ].PortId );
			m_inputPorts.RemoveAt( arrayIdx );

			m_sizeIsDirty = true;
			SetSaveIsDirty();
			RecalculateInputPortId();
		}

		public void DeleteOutputPortByArrayIdx( int portIdx )
		{
			if( portIdx >= m_outputPorts.Count )
				return;

			m_containerGraph.DeleteConnection( false, UniqueId, portIdx, false, true );
			m_outputPortsDict.Remove( m_outputPorts[ portIdx ].PortId );
			m_outputPorts.RemoveAt( portIdx );
			m_sizeIsDirty = true;
		}

		public InputPort GetInputPortByArrayId( int id )
		{
			if( id < m_inputPorts.Count )
				return m_inputPorts[ id ];

			return null;
		}

		public OutputPort GetOutputPortByArrayId( int id )
		{
			if( id < m_outputPorts.Count )
				return m_outputPorts[ id ];

			return null;
		}

		public InputPort GetInputPortByUniqueId( int id )
		{
			if( m_inputPortsDict.ContainsKey( id ) )
				return m_inputPortsDict[ id ];

			if( m_inputPortsDict.Count != m_inputPorts.Count )
				m_repopulateDictionaries = true;

			int inputCount = m_inputPorts.Count;
			for( int i = 0; i < inputCount; i++ )
			{
				if( m_inputPorts[ i ].PortId == id )
				{
					return m_inputPorts[ i ];
				}
			}
			return null;
		}

		public OutputPort GetOutputPortByUniqueId( int id )
		{
			if( m_outputPortsDict.ContainsKey( id ) )
				return m_outputPortsDict[ id ];

			if( m_outputPortsDict.Count != m_outputPorts.Count )
				m_repopulateDictionaries = true;

			int outputCount = m_outputPorts.Count;
			for( int i = 0; i < outputCount; i++ )
			{
				if( m_outputPorts[ i ].PortId == id )
					return m_outputPorts[ i ];
			}
			return null;
		}

		public override string ToString()
		{
			string dump = "";
			dump += ( "Type: " + GetType() );
			dump += ( " Unique Id: " + UniqueId + "\n" );
			dump += ( " Inputs: \n" );

			int inputCount = m_inputPorts.Count;
			int outputCount = m_outputPorts.Count;

			for( int inputIdx = 0; inputIdx < inputCount; inputIdx++ )
			{
				dump += ( m_inputPorts[ inputIdx ] + "\n" );
			}
			dump += ( "Outputs: \n" );
			for( int outputIdx = 0; outputIdx < outputCount; outputIdx++ )
			{
				dump += ( m_outputPorts[ outputIdx ] + "\n" );
			}
			return dump;
		}

		public string GetValueFromOutputStr( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( ignoreLocalvar )
			{
				return GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			}

			if( m_outputPorts[ outputId ].IsLocalValue )
			{
				if( m_outputPorts[ outputId ].DataType != WirePortDataType.OBJECT && m_outputPorts[ outputId ].DataType != inputPortType )
				{
					return UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), null, m_outputPorts[ outputId ].DataType, inputPortType, m_outputPorts[ outputId ].LocalValue );
				}
				else
				{
					return m_outputPorts[ outputId ].LocalValue;
				}
			}

			string result = GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			result = CreateOutputLocalVariable( outputId, result, ref dataCollector );

			if( m_outputPorts[ outputId ].DataType != WirePortDataType.OBJECT && m_outputPorts[ outputId ].DataType != inputPortType )
			{
				result = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), null, m_outputPorts[ outputId ].DataType, inputPortType, result );
			}
			return result;
		}

		public virtual string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			return string.Empty;
		}

		protected virtual void OnUniqueIDAssigned() { }
		public virtual void ReleaseUniqueIdData() { }

		public string CreateOutputLocalVariable( int outputId, string value, ref MasterNodeDataCollector dataCollector )
		{
			//if ( dataCollector.DirtyNormal && ContainerGraph.IsNormalDependent )
			//	return value;

			if( m_outputPorts[ outputId ].IsLocalValue )
				return m_outputPorts[ outputId ].LocalValue;

			if( m_outputPorts[ outputId ].ConnectionCount > 1 )
			{
				RegisterLocalVariable( outputId, value, ref dataCollector );
				//bool vertexMode = dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation;
				//string localVar = m_outputPorts[ outputId ].ConfigOutputLocalValue( m_currentPrecisionType, value );
				//if ( !vertexMode && UIUtils.GetCategoryInBitArray( Category, MasterNodePortCategory.Fragment ) )
				//	dataCollector.AddToLocalVariables( m_uniqueId, localVar );

				//if ( vertexMode && UIUtils.GetCategoryInBitArray( Category, MasterNodePortCategory.Vertex ) || UIUtils.GetCategoryInBitArray( Category, MasterNodePortCategory.Tessellation ) )
				//	dataCollector.AddToVertexLocalVariables( m_uniqueId, localVar );

				return m_outputPorts[ outputId ].LocalValue;
			}

			return value;
		}

		public void RegisterLocalVariable( int outputId, string value, ref MasterNodeDataCollector dataCollector, string customName = null )
		{
			bool vertexMode = dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation;
			string localVar = m_outputPorts[ outputId ].ConfigOutputLocalValue( m_currentPrecisionType, value, customName, dataCollector.PortCategory );

			//if ( !vertexMode && ( UIUtils.GetCategoryInBitArray( Category, MasterNodePortCategory.Fragment ) || UIUtils.GetCategoryInBitArray( Category, MasterNodePortCategory.Debug ) ) )
			//	dataCollector.AddToLocalVariables( m_uniqueId, localVar );

			//if ( vertexMode && ( UIUtils.GetCategoryInBitArray( Category, MasterNodePortCategory.Vertex ) || UIUtils.GetCategoryInBitArray( Category, MasterNodePortCategory.Tessellation )) )
			//	dataCollector.AddToVertexLocalVariables( m_uniqueId, localVar );

			if( vertexMode )
			{
				dataCollector.AddToVertexLocalVariables( m_uniqueId, localVar );
			}
			else
			{
				dataCollector.AddToLocalVariables( m_uniqueId, localVar );
			}
		}

		public void InvalidateConnections()
		{
			int inputCount = m_inputPorts.Count;
			int outputCount = m_outputPorts.Count;

			for( int i = 0; i < inputCount; i++ )
			{
				m_inputPorts[ i ].InvalidateAllConnections();
			}

			for( int i = 0; i < outputCount; i++ )
			{
				m_outputPorts[ i ].InvalidateAllConnections();
			}
		}

		public virtual bool OnClick( Vector2 currentMousePos2D )
		{
			bool singleClick = true;
			if( ( EditorApplication.timeSinceStartup - m_lastTimeSelected ) < NodeClickTime )
			{
				OnNodeDoubleClicked( currentMousePos2D );
				singleClick = false;
			}

			m_lastTimeSelected = EditorApplication.timeSinceStartup;
			return singleClick;
		}

		public virtual void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			ContainerGraph.ParentWindow.ParametersWindow.IsMaximized = !ContainerGraph.ParentWindow.ParametersWindow.IsMaximized;
		}

		public virtual void OnNodeSelected( bool value )
		{
			if( !value )
			{
				if( m_inputPorts != null )
				{
					int count = m_inputPorts.Count;
					for( int i = 0; i < count; i++ )
					{
						m_inputPorts[ i ].ResetEditing();
					}
				}

				if( m_outputPorts != null )
				{
					int count = m_outputPorts.Count;
					for( int i = 0; i < count; i++ )
					{
						m_outputPorts[ i ].ResetEditing();
					}
				}
			}
		}

		public virtual void ResetOutputLocals()
		{
			int outputCount = m_outputPorts.Count;
			for( int i = 0; i < outputCount; i++ )
			{
				m_outputPorts[ i ].ResetLocalValue();
			}
		}


		public virtual void ResetOutputLocalsIfNot( MasterNodePortCategory category )
		{
			int outputCount = m_outputPorts.Count;
			for( int i = 0; i < outputCount; i++ )
			{
				if( !m_outputPorts[ i ].IsLocalOnCategory( category ) )
					m_outputPorts[ i ].ResetLocalValue();
			}
		}

		public virtual void Rewire()
		{
		}

		public int UniqueId
		{
			get { return m_uniqueId; }

			set
			{
				m_uniqueId = value;

				int inputCount = m_inputPorts.Count;
				int outputCount = m_outputPorts.Count;

				for( int inputIdx = 0; inputIdx < inputCount; inputIdx++ )
				{
					m_inputPorts[ inputIdx ].NodeId = value;
				}

				for( int outputIdx = 0; outputIdx < outputCount; outputIdx++ )
				{
					m_outputPorts[ outputIdx ].NodeId = value;
				}
				OnUniqueIDAssigned();
			}
		}

		public string OutputId
		{
			get
			{
				if( ContainerGraph.GraphId > 0 )
					return UniqueId + "_g" + ContainerGraph.GraphId;
				else
					return UniqueId.ToString();
			}
		}


		public virtual Rect Position { get { return m_position; } }
		public Rect TruePosition { get { return m_position; } }

		public Vector2 CenterPosition { get { return new Vector2( m_position.x + m_position.width * 0.5f, m_position.y + m_position.height * 0.5f ); ; } }

		public Rect GlobalPosition { get { return m_globalPosition; } }

		public Vector2 Corner { get { return new Vector2( m_position.x + m_position.width, m_position.y + m_position.height ); } }
		public Vector2 Vec2Position
		{
			get { return new Vector2( m_position.x, m_position.y ); }

			set
			{
				m_position.x = value.x;
				m_position.y = value.y;
			}
		}

		public Vector3 Vec3Position
		{
			get { return new Vector3( m_position.x, m_position.y, 0f ); }

			set
			{
				m_position.x = value.x;
				m_position.y = value.y;
			}
		}


		public bool Selected
		{
			get { return m_selected; }
			set
			{
				m_infiniteLoopDetected = false;
				m_selected = value;
				OnNodeSelected( value );
			}
		}

		public List<InputPort> InputPorts { get { return m_inputPorts; } }

		public List<OutputPort> OutputPorts
		{
			get { return m_outputPorts; }
		}

		public bool IsConnected { get { return m_connStatus == NodeConnectionStatus.Connected; } }
		public NodeConnectionStatus ConnStatus
		{
			get { return m_connStatus; }
			set
			{
				if( m_selfPowered )
				{
					m_connStatus = NodeConnectionStatus.Connected;
				}
				else
				{
					m_connStatus = value;
				}

				switch( m_connStatus )
				{
					case NodeConnectionStatus.Island:
					case NodeConnectionStatus.Not_Connected: m_statusColor = Constants.NodeDefaultColor; break;
					case NodeConnectionStatus.Connected: m_statusColor = Constants.NodeConnectedColor; break;
					case NodeConnectionStatus.Error: m_statusColor = Constants.NodeErrorColor; break;
				}

			}
		}

		public bool SelfPowered
		{
			set
			{
				m_selfPowered = value;
				if( value )
				{
					ConnStatus = NodeConnectionStatus.Connected;
				}
			}
		}

		// This is also called when recording on Undo
		public virtual void OnBeforeSerialize() { }
		public virtual void OnAfterDeserialize()
		{
			m_selected = false;
			m_isOnGrid = false;
			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].ResetWireReferenceStatus();
			}
			m_repopulateDictionaries = true;
		}

		//Inherited classes must call this base method in order to setup id and position
		public virtual void ReadFromString( ref string[] nodeParams )
		{
			ParentReadFromString( ref nodeParams );
		}

		public void ParentReadFromString( ref string[] nodeParams )
		{
			m_currentReadParamIdx = IOUtils.NodeTypeId + 1;

			UniqueId = Convert.ToInt32( nodeParams[ m_currentReadParamIdx++ ] );

			string[] posCoordinates = nodeParams[ m_currentReadParamIdx++ ].Split( IOUtils.VECTOR_SEPARATOR );

			m_position.x = Convert.ToSingle( posCoordinates[ 0 ] );
			m_position.y = Convert.ToSingle( posCoordinates[ 1 ] );

			if( UIUtils.CurrentShaderVersion() > 22 )
				m_currentPrecisionType = ( PrecisionType ) Enum.Parse( typeof( PrecisionType ), GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 5004 )
				m_showPreview = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

		}

		//should be called after ReadFromString
		public virtual void ReadInputDataFromString( ref string[] nodeParams )
		{
			int count = 0;
			if( UIUtils.CurrentShaderVersion() > 7003 )
			{
				count = Convert.ToInt32( nodeParams[ m_currentReadParamIdx++ ] );
			}
			else
			{
				count = m_inputPorts.Count;
			}

			for( int i = 0; i < count && i < nodeParams.Length && m_currentReadParamIdx < nodeParams.Length; i++ )
			{
				if( UIUtils.CurrentShaderVersion() < 5003 )
				{
					int newId = VersionConvertInputPortId( i );
					if( UIUtils.CurrentShaderVersion() > 23 )
					{
						m_inputPorts[ newId ].DataType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), nodeParams[ m_currentReadParamIdx++ ] );
					}

					m_inputPorts[ newId ].InternalData = nodeParams[ m_currentReadParamIdx++ ];
					if( m_inputPorts[ newId ].IsEditable && UIUtils.CurrentShaderVersion() >= 3100 && m_currentReadParamIdx < nodeParams.Length )
					{
						m_inputPorts[ newId ].Name = nodeParams[ m_currentReadParamIdx++ ];
					}
				}
				else
				{
					int portId = Convert.ToInt32( nodeParams[ m_currentReadParamIdx++ ] );
					WirePortDataType DataType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), nodeParams[ m_currentReadParamIdx++ ] );
					string InternalData = nodeParams[ m_currentReadParamIdx++ ];
					bool isEditable = Convert.ToBoolean( nodeParams[ m_currentReadParamIdx++ ] );
					string Name = string.Empty;
					if( isEditable && m_currentReadParamIdx < nodeParams.Length )
					{
						Name = nodeParams[ m_currentReadParamIdx++ ];
					}

					InputPort inputPort = GetInputPortByUniqueId( portId );
					if( inputPort != null )
					{
						inputPort.DataType = DataType;
						inputPort.InternalData = InternalData;
						if( !string.IsNullOrEmpty( Name ) )
						{
							inputPort.Name = Name;
						}
					}
				}
			}
		}

		public virtual void ReadOutputDataFromString( ref string[] nodeParams )
		{
			int count = 0;
			if( UIUtils.CurrentShaderVersion() > 7003 )
			{
				count = Convert.ToInt32( nodeParams[ m_currentReadParamIdx++ ] );
			}
			else
			{
				count = m_outputPorts.Count;
			}

			for( int i = 0; i < count && i < nodeParams.Length && m_currentReadParamIdx < nodeParams.Length; i++ )
			{
				m_outputPorts[ i ].DataType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), nodeParams[ m_currentReadParamIdx++ ] );
			}
		}

		public virtual void ReadAdditionalClipboardData( ref string[] nodeParams ) { }

		protected string GetCurrentParam( ref string[] nodeParams )
		{
			if( m_currentReadParamIdx < nodeParams.Length )
			{
				return nodeParams[ m_currentReadParamIdx++ ];
			}

			UIUtils.ShowMessage( "Invalid params number in node " + m_uniqueId + " of type " + GetType(), MessageSeverity.Error );
			return string.Empty;
		}

		protected string GetCurrentParam( int index, ref string[] nodeParams )
		{
			if( m_currentReadParamIdx < nodeParams.Length )
			{
				return nodeParams[ index ];
			}

			UIUtils.ShowMessage( "Invalid params number in node " + m_uniqueId + " of type " + GetType(), MessageSeverity.Error );
			return string.Empty;
		}


		public virtual void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			IOUtils.AddTypeToString( ref nodeInfo, IOUtils.NodeParam );
			IOUtils.AddFieldValueToString( ref nodeInfo, GetType() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_uniqueId );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_position.x.ToString() + IOUtils.VECTOR_SEPARATOR + m_position.y.ToString() ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentPrecisionType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_showPreview );
			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].WriteToString( ref connectionsInfo );
			}
		}

		public virtual void WriteInputDataToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts.Count );
			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].PortId );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].DataType );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].InternalData );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].IsEditable );
				if( m_inputPorts[ i ].IsEditable )
				{
					IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].Name );
				}
			}
		}

		public void WriteOutputDataToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputPorts.Count );
			for( int i = 0; i < m_outputPorts.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_outputPorts[ i ].DataType );
			}
		}

		public virtual void WriteAdditionalClipboardData( ref string nodeInfo ) { }

		public virtual string GetIncludes() { return string.Empty; }
		public virtual void OnObjectDropped( UnityEngine.Object obj ) { }
		public virtual void SetupFromCastObject( UnityEngine.Object obj ) { }
		public virtual bool OnNodeInteraction( ParentNode node ) { return false; }
		public virtual void OnConnectedOutputNodeChanges( int portId, int otherNodeId, int otherPortId, string name, WirePortDataType type ) { }
		public virtual void OnConnectedInputNodeChanges( int portId, int otherNodeId, int otherPortId, string name, WirePortDataType type ) { }

		public Rect CachedPos { get { return m_cachedPos; } }

		public bool IsOnGrid
		{
			set { m_isOnGrid = value; }
			get { return m_isOnGrid; }
		}

		public uint CurrentReadParamIdx
		{
			get { return m_currentReadParamIdx++; }
			set { m_currentReadParamIdx = value; }
		}

		public Dictionary<string, InputPort> InputPortsDict
		{
			get
			{
				Dictionary<string, InputPort> dict = new Dictionary<string, InputPort>();
				for( int i = 0; i < m_inputPorts.Count; i++ )
				{
					dict.Add( m_inputPorts[ i ].Name, m_inputPorts[ i ] );
				}
				return dict;
			}
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

		public virtual void ResetNodeData()
		{
			m_category = 0;
			m_graphDepth = 0;
		}

		public virtual void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			UIUtils.SetCategoryInBitArray( ref m_category, nodeData.Category );
			nodeData.GraphDepth += 1;
			if( nodeData.GraphDepth > m_graphDepth )
			{
				m_graphDepth = nodeData.GraphDepth;
			}
			int count = m_inputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_inputPorts[ i ].IsConnected )
				{
					m_inputPorts[ i ].GetOutputNode().PropagateNodeData( nodeData, ref dataCollector );
				}
			}
		}

		protected string m_previousTitle = string.Empty;
		public void SetTitleTextOnCallback( string compareTitle, Action<ParentNode, string> callback )
		{
			if( !m_previousTitle.Equals( compareTitle ) )
			{
				m_previousTitle = compareTitle;
				callback( this, compareTitle );
			}
		}

		protected string m_previousAdditonalTitle = string.Empty;
		public void SetAdditonalTitleTextOnCallback( string compareTitle, Action<ParentNode, string> callback )
		{
			if( !m_previousAdditonalTitle.Equals( compareTitle ) )
			{
				m_previousAdditonalTitle = compareTitle;
				callback( this, compareTitle );
			}
		}

		public void SetTitleText( string newText )
		{
			if( !newText.Equals( m_content.text ) )
			{
				m_content.text = newText;
				//m_firstPreviewDraw = true;
				m_sizeIsDirty = true;
			}
		}

		public void SetAdditonalTitleValue( object value )
		{
			SetAdditonalTitleText( string.Format( Constants.SubTitleValueFormatStr, value ) );
		}

		public void SetAdditonalTitleText( string newText )
		{
			if( !newText.Equals( m_additionalContent.text ) )
			{
				m_additionalContent.text = newText;
				//m_firstPreviewDraw = true;
				m_sizeIsDirty = true;
			}
		}


		//Methods created to take into account new ports added on nodes newer versions
		//This way we can convert connections from previous versions to newer ones and not brake shader graph
		public virtual int VersionConvertInputPortId( int portId ) { return portId; }
		public virtual int VersionConvertOutputPortId( int portId ) { return portId; }

		public virtual string DataToArray { get { return string.Empty; } }

		public bool SaveIsDirty
		{
			set { m_saveIsDirty = value && UIUtils.DirtyMask; }
			get
			{
				bool value = m_saveIsDirty;
				m_saveIsDirty = false;
				return value;
			}
		}

		public GUIContent TitleContent { get { return m_content; } }
		public GUIContent AdditonalTitleContent { get { return m_additionalContent; } }
		public bool IsVisible { get { return m_isVisible; } }
		public NodeAttributes Attributes { get { return m_nodeAttribs; } }
		public bool ReorderLocked { get { return m_reorderLocked; } }
		public bool RequireMaterialUpdate { get { return m_requireMaterialUpdate; } }
		public bool RMBIgnore { get { return m_rmbIgnore; } }
		public float TextLabelWidth { get { return m_textLabelWidth; } }
		public bool IsMoving { get { return m_isMoving > 0; } }
		public bool MovingInFrame { get { return m_movingInFrame; } set { m_movingInFrame = value; } }
		public bool SizeIsDirty { get { return m_sizeIsDirty; } }
		public int Category { get { return m_category; } }
		public int CommentaryParent
		{
			get { return m_commentaryParent; }
			set { m_commentaryParent = value; }
		}

		public int Depth
		{
			get { return m_depth; }
			set { m_depth = value; }
		}

		public int MatrixId
		{
			get { return m_matrixId; }
			set { m_matrixId = value; }
		}

		public float PaddingTitleRight
		{
			get { return m_paddingTitleRight; }
			set { m_paddingTitleRight += value; }
		}

		public float PaddingTitleLeft
		{
			get { return m_paddingTitleLeft; }
			set { m_paddingTitleLeft += value; }
		}

		public void RenderNodePreview()
		{
			//Runs at least one time
			if( !HasPreviewShader || !m_initialized )
				return;

			SetPreviewInputs();

			if( m_cachedMainTexId == -1 )
				m_cachedMainTexId = Shader.PropertyToID( "_MainTex" );

			if( m_cachedMaskTexId == -1 )
				m_cachedMaskTexId = Shader.PropertyToID( "_MaskTex" );

			if( m_cachedPortsId == -1 )
				m_cachedPortsId = Shader.PropertyToID( "_Ports" );

			if( m_cachedPortId == -1 )
				m_cachedPortId = Shader.PropertyToID( "_Port" );

			int count = m_outputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( i + m_outputPorts[ i ].IndexPreviewOffset == 0 )
				{
					RenderTexture temp = RenderTexture.active;
					RenderTexture beforeMask = RenderTexture.GetTemporary( 128, 128, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear );
					RenderTexture.active = beforeMask;
					Graphics.Blit( null, beforeMask, PreviewMaterial, m_previewMaterialPassId );

					m_portMask.Set( 0, 0, 0, 0 );

					switch( m_outputPorts[ i ].DataType )
					{
						case WirePortDataType.INT:
						case WirePortDataType.FLOAT:
						m_portMask.Set( 1, 1, 1, 1 );
						break;
						case WirePortDataType.FLOAT2:
						m_portMask.Set( 1, 1, 0, 0 );
						break;
						case WirePortDataType.FLOAT3:
						m_portMask.Set( 1, 1, 1, 0 );
						break;
						case WirePortDataType.COLOR:
						case WirePortDataType.FLOAT4:
						m_portMask.Set( 1, 1, 1, 1 );
						break;
						default:
						m_portMask.Set( 1, 1, 1, 1 );
						break;
					}

					if( m_outputPorts[ i ].DataType == WirePortDataType.FLOAT3x3 || m_outputPorts[ i ].DataType == WirePortDataType.FLOAT4x4 )
					{
						m_outputPorts[ i ].MaskingMaterial.SetTexture( m_cachedMainTexId, EditorGUIUtility.whiteTexture );
					}
					else
					{
						m_outputPorts[ i ].MaskingMaterial.SetTexture( m_cachedMainTexId, beforeMask );
					}
					m_outputPorts[ i ].MaskingMaterial.SetVector( m_cachedPortsId, m_portMask );
					RenderTexture.active = m_outputPorts[ i ].OutputPreviewTexture;
					Graphics.Blit( null, m_outputPorts[ i ].OutputPreviewTexture, m_outputPorts[ i ].MaskingMaterial, 0 );

					RenderTexture.ReleaseTemporary( beforeMask );
					RenderTexture.active = temp;
				}
				else
				{
					RenderTexture temp = RenderTexture.active;
					if( m_outputPorts[ i ].IndexPreviewOffset > 0 )
						m_outputPorts[ i ].MaskingMaterial.SetTexture( m_cachedMaskTexId, InputPorts[ 0 ].InputPreviewTexture );
					else
						m_outputPorts[ i ].MaskingMaterial.SetTexture( m_cachedMaskTexId, PreviewTexture );
					m_outputPorts[ i ].MaskingMaterial.SetFloat( m_cachedPortId, i + m_outputPorts[ i ].IndexPreviewOffset );

					RenderTexture.active = m_outputPorts[ i ].OutputPreviewTexture;
					Graphics.Blit( null, m_outputPorts[ i ].OutputPreviewTexture, m_outputPorts[ i ].MaskingMaterial, 1 );
					RenderTexture.active = temp;
				}
			}
		}

		protected void ShowTab( NodeMessageType type, string tooltip )
		{
			m_showErrorMessage = true;
			m_errorMessageTypeIsError = type;
			m_errorMessageTooltip = tooltip;
		}

		protected void ShowTab()
		{
			m_showErrorMessage = true;
		}

		protected void HideTab()
		{
			m_showErrorMessage = false;
		}

		public RenderTexture PreviewTexture
		{
			get
			{
				if( m_outputPorts.Count > 0 )
					return m_outputPorts[ 0 ].OutputPreviewTexture;
				else
					return null;
			}
		}

		public void FullWriteToString( ref string nodesInfo, ref string connectionsInfo )
		{
			WriteToString( ref nodesInfo, ref connectionsInfo );
			WriteInputDataToString( ref nodesInfo );
			WriteOutputDataToString( ref nodesInfo );
		}

		public void ClipboardFullWriteToString( ref string nodesInfo, ref string connectionsInfo )
		{
			FullWriteToString( ref nodesInfo, ref connectionsInfo );
			WriteAdditionalClipboardData( ref nodesInfo );
		}

		public void FullReadFromString( ref string[] parameters )
		{
			try
			{
				ReadFromString( ref parameters );
				ReadInputDataFromString( ref parameters );
				ReadOutputDataFromString( ref parameters );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
		}

		public void ClipboardFullReadFromString( ref string[] parameters )
		{
			try
			{
				FullReadFromString( ref parameters );
				ReadAdditionalClipboardData( ref parameters );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
		}

		public string GenerateClippedTitle( string original )
		{
			if( UIUtils.UnZoomedNodeTitleStyle == null )
				return original;

			GUIContent content = new GUIContent( original );

			string finalTitle = string.Empty;
			bool addEllipsis = false;
			for( int i = 1; i <= original.Length; i++ )
			{
				content.text = original.Substring( 0, i );
				Vector2 titleSize = UIUtils.UnZoomedNodeTitleStyle.CalcSize( content );
				int maxSize = 170;
				if( titleSize.x > maxSize )
				{
					addEllipsis = true;
					break;
				}
				else
				{
					finalTitle = content.text;
				}
			}
			if( addEllipsis )
				finalTitle += "...";
			return finalTitle;
		}

		public virtual void RefreshOnUndo() { }
		public virtual void CalculateCustomGraphDepth() { }
		public int GraphDepth { get { return m_graphDepth; } }

		public PrecisionType CurrentPrecisionType { get { return m_currentPrecisionType; } }

		public Material PreviewMaterial
		{
			get
			{
				if( m_previewMaterial == null )
				{
					m_previewMaterial = new Material( PreviewShader );
				}
				return m_previewMaterial;
			}
		}

		public Shader PreviewShader
		{
			get
			{
				if( m_previewShader == null )
				{
					m_previewShader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( m_previewShaderGUID ) );
				}

				if( m_previewShader == null )
				{
					m_previewShader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "d9ca47581ac157145bff6f72ac5dd73e" ) ); //ranged float guid
				}

				if( m_previewShader == null )
					m_previewShader = Shader.Find( "Unlit/Colored Transparent" );

				return m_previewShader;
			}
		}

		public bool HasPreviewShader
		{
			get { return !string.IsNullOrEmpty( m_previewShaderGUID ); }
		}

		public void CheckSpherePreview()
		{
			bool oneIsSphere = false;

			if( m_drawPreviewAsSphere )
				oneIsSphere = true;
			int count = m_inputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				ParentNode node = m_inputPorts[ i ].GetOutputNode( 0 );
				if( node != null && node.SpherePreview )
					oneIsSphere = true;
			}

			if( m_forceDrawPreviewAsPlane )
				oneIsSphere = false;

			SpherePreview = oneIsSphere;
		}

		public bool SpherePreview
		{
			get { return m_spherePreview; }
			set { m_spherePreview = value; }
		}

		public virtual void SetContainerGraph( ParentGraph newgraph )
		{
			m_containerGraph = newgraph;
		}
		public virtual void OnMasterNodeReplaced( MasterNode newMasterNode ) { }
		public virtual void RefreshExternalReferences() { }

		/// <summary>
		/// It's the graph the node exists in, this is set after node creation and it's not available on CommonInit
		/// </summary>
		public ParentGraph ContainerGraph
		{
			get { return m_containerGraph; }
			set { m_containerGraph = value; }
		}
		public virtual bool Contains( Vector2 pos ) { return m_globalPosition.Contains( pos ); }
		public virtual bool Contains( Vector3 pos ) { return m_globalPosition.Contains( pos ); }
	}
}
