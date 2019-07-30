// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum PropertyType
	{
		Constant = 0,
		Property,
		InstancedProperty,
		Global
	}

	[Serializable]
	public class PropertyAttributes
	{
		public string Name;
		public string Attribute;
		public PropertyAttributes( string name, string attribute )
		{
			Name = name;
			Attribute = attribute;
		}
	}

	[Serializable]
	public class PropertyNode : ParentNode
	{
		private const string IsPropertyStr = "Is Property";
		private const string PropertyNameStr = "Property Name";
		private const string PropertyInspectorStr = "Name";
		protected const string ParameterTypeStr = "Type";
		private const string PropertyTextfieldControlName = "PropertyName";
		private const string PropertyInspTextfieldControlName = "PropertyInspectorName";
		private const string OrderIndexStr = "Order Index";
		private const double MaxTimestamp = 2;
		private const double MaxPropertyTimestamp = 2;
		private readonly string[] LabelToolbarTitle = { "Material", "Default" };

		[SerializeField]
		protected PropertyType m_currentParameterType;

		[SerializeField]
		private PropertyType m_lastParameterType;

		[SerializeField]
		protected string m_propertyName;

		[SerializeField]
		protected string m_propertyInspectorName;

		[SerializeField]
		protected string m_precisionString;
		protected bool m_drawPrecisionUI = true;

		[SerializeField]
		private int m_orderIndex = -1;

		private int m_orderIndexOffset = 0;

		protected bool m_drawAttributes = true;

		protected bool m_underscoredGlobal = false;

		protected bool m_freeName;
		protected bool m_freeType;
		protected bool m_propertyNameIsDirty;

		protected bool m_propertyFromInspector;
		protected double m_propertyFromInspectorTimestamp;

		protected bool m_delayedDirtyProperty;
		protected double m_delayedDirtyPropertyTimestamp;

		protected string m_defaultPropertyName;
		protected string m_oldName = string.Empty;

		private bool m_reRegisterName = false;

		//protected bool m_useCustomPrefix = false;
		protected string m_customPrefix = null;

		private int m_propertyTab = 0;

		public PropertyNode() : base() { }
		public PropertyNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		[SerializeField]
		private string m_uniqueName;

		// Property Attributes
		private const float ButtonLayoutWidth = 15;

		private bool m_visibleAttribsFoldout;
		protected List<PropertyAttributes> m_availableAttribs = new List<PropertyAttributes>();
		private string[] m_availableAttribsArr;

		[SerializeField]
		protected List<int> m_selectedAttribs = new List<int>();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_textLabelWidth = 105;
			m_orderIndex = UIUtils.GetPropertyNodeAmount();
			m_currentParameterType = PropertyType.Constant;
			m_freeType = true;
			m_freeName = true;
			m_propertyNameIsDirty = true;
			m_availableAttribs.Add( new PropertyAttributes( "Hide in Inspector", "[HideInInspector]" ) );
			m_availableAttribs.Add( new PropertyAttributes( "HDR", "[HDR]" ) );
			m_availableAttribs.Add( new PropertyAttributes( "Gamma", "[Gamma]" ) );
			m_availableAttribs.Add( new PropertyAttributes( "Per Renderer Data", "[PerRendererData]" ) );
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();

			if ( PaddingTitleLeft == 0 && m_freeType )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if ( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}

		protected void BeginDelayedDirtyProperty()
		{
			m_delayedDirtyProperty = true;
			m_delayedDirtyPropertyTimestamp = EditorApplication.timeSinceStartup;
		}

		public void CheckDelayedDirtyProperty()
		{
			if ( m_delayedDirtyProperty )
			{
				if ( ( EditorApplication.timeSinceStartup - m_delayedDirtyPropertyTimestamp ) > MaxPropertyTimestamp )
				{
					m_delayedDirtyProperty = false;
					m_propertyNameIsDirty = true;
					m_sizeIsDirty = true;
				}
			}
		}

		public void BeginPropertyFromInspectorCheck()
		{
			m_propertyFromInspector = true;
			m_propertyFromInspectorTimestamp = EditorApplication.timeSinceStartup;
		}

		public void CheckPropertyFromInspector( bool forceUpdate = false )
		{
			if ( m_propertyFromInspector )
			{
				if ( forceUpdate || ( EditorApplication.timeSinceStartup - m_propertyFromInspectorTimestamp ) > MaxTimestamp )
				{
					m_propertyFromInspector = false;
					RegisterPropertyName( true, m_propertyInspectorName, m_underscoredGlobal );
					m_propertyNameIsDirty = true;
				}
			}
		}

		public override void ReleaseUniqueIdData()
		{
			UIUtils.ReleaseUniformName( UniqueId, m_oldName );
			RegisterFirstAvailablePropertyName( false );
		}

		protected override void OnUniqueIDAssigned()
		{
			RegisterFirstAvailablePropertyName( false );

			if ( m_nodeAttribs != null )
				m_uniqueName = m_nodeAttribs.Name + UniqueId;
		}

		public bool CheckLocalVariable( ref MasterNodeDataCollector dataCollector )
		{
			bool addToLocalValue = false;
			int count = 0;
			for ( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if ( m_outputPorts[ i ].IsConnected )
				{
					if ( m_outputPorts[ i ].ConnectionCount > 1 )
					{
						addToLocalValue = true;
						break;
					}
					count += 1;
					if ( count > 1 )
					{
						addToLocalValue = true;
						break;
					}
				}
			}

			if ( addToLocalValue )
			{
				ConfigureLocalVariable( ref dataCollector );
			}

			return addToLocalValue;
		}

		public virtual void ConfigureLocalVariable( ref MasterNodeDataCollector dataCollector ) { }
		public virtual void CopyDefaultsToMaterial() { }

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			RegisterPropertyName( true, obj.name, m_underscoredGlobal );
		}

		public void ChangeParameterType( PropertyType parameterType )
		{
			if ( m_currentParameterType == PropertyType.Constant )
			{
				CopyDefaultsToMaterial();
			}

			if ( parameterType == PropertyType.InstancedProperty )
			{
				UIUtils.AddInstancePropertyCount();
			}
			else if ( m_currentParameterType == PropertyType.InstancedProperty )
			{
				UIUtils.RemoveInstancePropertyCount();
			}

			if ( ( parameterType == PropertyType.Property || parameterType == PropertyType.InstancedProperty )
				&& m_currentParameterType != PropertyType.Property && m_currentParameterType != PropertyType.InstancedProperty )
			{
				UIUtils.RegisterPropertyNode( this );
			}

			if ( ( parameterType != PropertyType.Property && parameterType != PropertyType.InstancedProperty )
				&& ( m_currentParameterType == PropertyType.Property || m_currentParameterType == PropertyType.InstancedProperty ) )
			{
				UIUtils.UnregisterPropertyNode( this );
			}

			m_currentParameterType = parameterType;
		}

		void InitializeAttribsArray()
		{
			m_availableAttribsArr = new string[ m_availableAttribs.Count ];
			for ( int i = 0; i < m_availableAttribsArr.Length; i++ )
			{
				m_availableAttribsArr[ i ] = m_availableAttribs[ i ].Name;
			}
		}

		protected virtual void OnAtrributesChanged() { }
		void DrawAttributesAddRemoveButtons()
		{
			if ( m_availableAttribsArr == null )
			{
				InitializeAttribsArray();
			}

			EditorGUILayout.Separator();

			int attribCount = m_selectedAttribs.Count;
			if ( attribCount == 0 )
				m_visibleAttribsFoldout = false;
			// Add new port
			if ( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				m_selectedAttribs.Add( 0 );
				m_visibleAttribsFoldout = true;
				OnAtrributesChanged();
			}

			//Remove port
			if ( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				if ( attribCount > 0 )
				{
					m_selectedAttribs.RemoveAt( attribCount - 1 );
					OnAtrributesChanged();
				}
			}
		}

		public virtual void DrawAttributes()
		{
			int attribCount = m_selectedAttribs.Count;
			bool actionAllowed = true;
			int deleteItem = -1;
			if ( m_visibleAttribsFoldout )
			{
				for ( int i = 0; i < attribCount; i++ )
				{
					EditorGUI.BeginChangeCheck();
					{
						m_selectedAttribs[ i ] = EditorGUILayoutPopup( m_selectedAttribs[ i ], m_availableAttribsArr );
					}
					if ( EditorGUI.EndChangeCheck() )
					{
						OnAtrributesChanged();
					}

					EditorGUILayout.BeginHorizontal();
					GUILayout.Label( " " );
					// Add After
					if ( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
					{
						if ( actionAllowed )
						{
							m_selectedAttribs.Insert( i, m_selectedAttribs[ i ] );
							actionAllowed = false;
							OnAtrributesChanged();
						}
					}

					// Remove Current
					if ( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
					{
						if ( actionAllowed )
						{
							actionAllowed = false;
							deleteItem = i;
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				if ( deleteItem > -1 )
				{
					m_selectedAttribs.RemoveAt( deleteItem );
					OnAtrributesChanged();
				}
			}
		}
		public virtual void DrawMainPropertyBlock()
		{
			EditorGUILayout.BeginVertical();
			{
				if ( m_freeType )
				{
					PropertyType parameterType = ( PropertyType ) EditorGUILayoutEnumPopup( ParameterTypeStr, m_currentParameterType );
					if ( parameterType != m_currentParameterType )
					{
						ChangeParameterType( parameterType );
						BeginPropertyFromInspectorCheck();
					}
				}

				if ( m_freeName )
				{
					switch ( m_currentParameterType )
					{
						case PropertyType.Property:
						case PropertyType.InstancedProperty:
						{
							ShowPropertyInspectorNameGUI();
							ShowPropertyNameGUI( true );
							ShowPrecision();
							ShowToolbar();
						}
						break;
						case PropertyType.Global:
						{
							ShowPropertyInspectorNameGUI();
							ShowPropertyNameGUI( false );
							ShowPrecision();
							ShowDefaults();
						}
						break;
						case PropertyType.Constant:
						{
							ShowPropertyInspectorNameGUI();
							ShowPrecision();
							ShowDefaults();
						}
						break;
					}
				}
			}
			EditorGUILayout.EndVertical();
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			if ( m_freeType || m_freeName )
			{
				NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, DrawMainPropertyBlock );
				if ( m_drawAttributes )
					NodeUtils.DrawPropertyGroup( ref m_visibleAttribsFoldout, Constants.AttributesLaberStr, DrawAttributes, DrawAttributesAddRemoveButtons );
				CheckPropertyFromInspector();
			}
		}

		public void ShowPrecision()
		{
			if ( m_drawPrecisionUI )
			{
				EditorGUI.BeginChangeCheck();
				DrawPrecisionProperty();
				if ( EditorGUI.EndChangeCheck() )
					m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );

			}
		}

		public void ShowToolbar()
		{
			//if ( !CanDrawMaterial )
			//{
			//	ShowDefaults();
			//	return;
			//}

			m_propertyTab = GUILayout.Toolbar( m_propertyTab, LabelToolbarTitle );
			switch ( m_propertyTab )
			{
				default:
				case 0:
				EditorGUI.BeginChangeCheck();
				DrawMaterialProperties();
				if ( EditorGUI.EndChangeCheck() )
				{
					BeginDelayedDirtyProperty();
				}
				break;
				case 1:
				ShowDefaults();
				break;
			}
		}

		public void ShowDefaults()
		{
			EditorGUI.BeginChangeCheck();
			DrawSubProperties();
			if ( EditorGUI.EndChangeCheck() )
			{
				BeginDelayedDirtyProperty();
			}
		}

		public void ShowPropertyInspectorNameGUI()
		{
			EditorGUI.BeginChangeCheck();
			m_propertyInspectorName = EditorGUILayoutTextField( PropertyInspectorStr, m_propertyInspectorName );
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( m_propertyInspectorName.Length > 0 )
				{
					BeginPropertyFromInspectorCheck();
				}
			}
		}

		public void ShowPropertyNameGUI( bool isProperty )
		{
			bool guiEnabledBuffer = GUI.enabled;
			GUI.enabled = false;
			m_propertyName = EditorGUILayoutTextField( PropertyNameStr, m_propertyName );
			GUI.enabled = guiEnabledBuffer;
		}

		public virtual string GetPropertyValStr() { return string.Empty; }

		public override bool OnClick( Vector2 currentMousePos2D )
		{
			bool singleClick = base.OnClick( currentMousePos2D );
			m_propertyTab = 0;
			return singleClick;
		}

		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if ( currentMousePos2D.y - m_globalPosition.y > ( Constants.NODE_HEADER_HEIGHT + Constants.NODE_HEADER_EXTRA_HEIGHT ) * ContainerGraph.ParentWindow.CameraDrawInfo.InvertedZoom )
			{
				ContainerGraph.ParentWindow.ParametersWindow.IsMaximized = !ContainerGraph.ParentWindow.ParametersWindow.IsMaximized;
			}
		}

		public override void DrawTitle( Rect titlePos )
		{
			//base.DrawTitle( titlePos );
		}

		protected bool m_typeEditing;
		protected bool m_isEditing;
		protected bool m_stopEditing;
		protected bool m_startEditing;
		protected double m_clickTime;
		protected double m_doubleClickTime = 0.3;

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			// Custom Editable Title
			if ( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
			{
				if ( !m_isEditing && ( ( !ContainerGraph.ParentWindow.MouseInteracted && drawInfo.CurrentEventType == EventType.MouseDown && m_titleClickArea.Contains( drawInfo.MousePosition ) ) ) )
				{
					if ( ( EditorApplication.timeSinceStartup - m_clickTime ) < m_doubleClickTime )
						m_startEditing = true;
					else
						GUI.FocusControl( null );
					m_clickTime = EditorApplication.timeSinceStartup;
				}
				else if ( m_isEditing && ( ( drawInfo.CurrentEventType == EventType.MouseDown && !m_titleClickArea.Contains( drawInfo.MousePosition ) ) || !EditorGUIUtility.editingTextField ) )
				{
					m_stopEditing = true;
				}

				if ( m_isEditing || m_startEditing )
				{
					EditorGUI.BeginChangeCheck();
					GUI.SetNextControlName( m_uniqueName );
					m_propertyInspectorName = EditorGUITextField( m_titleClickArea, string.Empty, m_propertyInspectorName, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
					if ( EditorGUI.EndChangeCheck() )
					{
						SetTitleText( m_propertyInspectorName );
						m_sizeIsDirty = true;
						m_isDirty = true;
						if ( m_propertyInspectorName.Length > 0 )
						{
							BeginPropertyFromInspectorCheck();
						}
					}

					if( m_startEditing )
						EditorGUI.FocusTextInControl( m_uniqueName );
					//if( m_stopEditing )
					//	GUI.FocusControl( null );
				}

				if( drawInfo.CurrentEventType == EventType.Repaint )
				{
					if ( m_startEditing )
					{
						m_startEditing = false;
						m_isEditing = true;
					}

					if ( m_stopEditing )
					{
						m_stopEditing = false;
						m_isEditing = false;
						GUI.FocusControl( null );
					}
				}


				if ( m_freeType )
				{
					if( m_typeEditing )
					{
						PropertyType parameterType = ( PropertyType ) EditorGUIEnumPopup( m_typeRect, m_currentParameterType, UIUtils.PropertyPopUp );
						if ( parameterType != m_currentParameterType )
						{
							ChangeParameterType( parameterType );
							BeginPropertyFromInspectorCheck();
							m_typeEditing = false;
						}
					}
				}
			}

		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if ( drawInfo.CurrentEventType != EventType.MouseDown )
				return;

			if ( m_typeRect.Contains( drawInfo.MousePosition ) )
				m_typeEditing = true;
			else if ( m_typeEditing )
			{
				m_typeEditing = false;
			}
		}

		private Rect m_typeRect;
		private Rect m_titleClickArea;

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			if ( m_reRegisterName )
			{
				m_reRegisterName = false;
				UIUtils.RegisterUniformName( UniqueId, m_propertyName );
			}

			CheckDelayedDirtyProperty();

			if ( m_currentParameterType != m_lastParameterType || m_propertyNameIsDirty )
			{
				m_lastParameterType = m_currentParameterType;
				m_propertyNameIsDirty = false;
				if ( m_currentParameterType != PropertyType.Constant )
				{
					SetTitleText( m_propertyInspectorName );
					SetAdditonalTitleText( string.Concat( "Value( ", GetPropertyValStr(), " )" ) );
				}
				else
				{
					SetTitleText( m_propertyInspectorName );
					SetAdditonalTitleText( string.Concat( "Const( ", GetPropertyValStr(), " )" ) );
				}
				m_sizeIsDirty = true;
			}

			CheckPropertyFromInspector();

			if ( m_freeType )
			{
				m_typeRect = m_globalPosition;
				m_typeRect.x = m_typeRect.x + ( Constants.NodeButtonDeltaX - 1 ) * drawInfo.InvertedZoom + 1;
				m_typeRect.y = m_typeRect.y + Constants.NodeButtonDeltaY * drawInfo.InvertedZoom;
				m_typeRect.width = Constants.NodeButtonSizeX * drawInfo.InvertedZoom;
				m_typeRect.height = Constants.NodeButtonSizeY * drawInfo.InvertedZoom;
			}

			m_titleClickArea = m_titlePos;
			m_titleClickArea.height = Constants.NODE_HEADER_HEIGHT;
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );

			if ( !m_isVisible )
				return;

			// Type Popup
			if ( !m_typeEditing && m_freeType && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD4 )
				GUI.Label( m_typeRect, string.Empty, UIUtils.PropertyPopUp );

			// Fixed Title ( only renders when not editing )
			if ( !m_isEditing && !m_startEditing && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
			{
				GUI.Label( m_titleClickArea, m_content, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
			}
		}

		public void RegisterFirstAvailablePropertyName( bool releaseOldOne )
		{
			if ( releaseOldOne )
				UIUtils.ReleaseUniformName( UniqueId, m_oldName );

			UIUtils.GetFirstAvailableName( UniqueId, m_outputPorts[ 0 ].DataType, out m_propertyName, out m_propertyInspectorName, !string.IsNullOrEmpty( m_customPrefix ), m_customPrefix );
			m_oldName = m_propertyName;
			m_propertyNameIsDirty = true;
			m_reRegisterName = false;
			OnPropertyNameChanged();
		}

		public void RegisterPropertyName( bool releaseOldOne, string newName, bool forceUnderscore = false )
		{
			string propertyName = UIUtils.GeneratePropertyName( newName, m_currentParameterType, forceUnderscore );
			if ( m_propertyName.Equals( propertyName ) )
				return;

			if ( UIUtils.IsUniformNameAvailable( propertyName ) )
			{
				if ( releaseOldOne )
					UIUtils.ReleaseUniformName( UniqueId, m_oldName );

				m_oldName = propertyName;
				m_propertyName = propertyName;
				m_propertyInspectorName = newName;
				m_propertyNameIsDirty = true;
				m_reRegisterName = false;
				UIUtils.RegisterUniformName( UniqueId, propertyName );
				OnPropertyNameChanged();
			}
			else
			{

				GUI.FocusControl( string.Empty );
				RegisterFirstAvailablePropertyName( releaseOldOne );
				UIUtils.ShowMessage( string.Format( "Duplicate name found on edited node.\nAssigning first valid one {0}", m_propertyInspectorName ) );
			}
		}

		protected string CreateLocalVarDec( string value )
		{
			return string.Format( Constants.PropertyLocalVarDec, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ), m_propertyName, value );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			CheckPropertyFromInspector( true );
			if ( m_propertyName.Length == 0 )
			{
				RegisterFirstAvailablePropertyName( false );
			}
			switch ( CurrentParameterType )
			{
				case PropertyType.Property:
				{
					//Debug.Log( this.GetInstanceID()+" "+ OrderIndex+" "+GetPropertyValue() );
					dataCollector.AddToProperties( UniqueId, GetPropertyValue(), OrderIndex );
					string dataType = string.Empty;
					string dataName = string.Empty;
					if ( GetUniformData( out dataType, out dataName ) )
						dataCollector.AddToUniforms( UniqueId, dataType, dataName );
					//dataCollector.AddToUniforms( m_uniqueId, GetUniformValue() );
				}
				break;
				case PropertyType.InstancedProperty:
				{
					dataCollector.AddToPragmas( UniqueId, IOUtils.InstancedPropertiesHeader );
					dataCollector.AddToProperties( UniqueId, GetPropertyValue(), OrderIndex );
					dataCollector.AddToInstancedProperties( UniqueId, GetInstancedPropertyValue( dataCollector.IsTemplate ), OrderIndex );
				}
				break;
				case PropertyType.Global:
				{
					string dataType = string.Empty;
					string dataName = string.Empty;
					if ( GetUniformData( out dataType, out dataName ) )
						dataCollector.AddToUniforms( UniqueId, dataType, dataName );
					//dataCollector.AddToUniforms( m_uniqueId, GetUniformValue() );
				}
				break;
				case PropertyType.Constant: break;
			}
			dataCollector.AddPropertyNode( this );
			if ( m_currentParameterType == PropertyType.InstancedProperty && !m_outputPorts[ 0 ].IsLocalValue )
			{
				RegisterLocalVariable( 0, string.Format( IOUtils.InstancedPropertiesData, m_propertyName ), ref dataCollector, m_propertyName + "_Instance" );
			}
			return string.Empty;
		}

		public override void Destroy()
		{
			base.Destroy();
			if ( !string.IsNullOrEmpty( m_propertyName ) )
				UIUtils.ReleaseUniformName( UniqueId, m_propertyName );
			if ( m_currentParameterType == PropertyType.InstancedProperty )
			{
				UIUtils.RemoveInstancePropertyCount();
				UIUtils.UnregisterPropertyNode( this );
			}

			if ( m_currentParameterType == PropertyType.Property )
			{
				UIUtils.UnregisterPropertyNode( this );
			}

			m_availableAttribs.Clear();
			m_availableAttribs = null;
		}

		public string PropertyAttributes
		{
			get
			{
				int attribCount = m_selectedAttribs.Count;

				if ( m_selectedAttribs.Count == 0 )
					return string.Empty;

				string attribs = string.Empty;
				for ( int i = 0; i < attribCount; i++ )
				{
					attribs += m_availableAttribs[ m_selectedAttribs[ i ] ].Attribute;
				}
				return attribs;
			}
		}
		public virtual void OnPropertyNameChanged() { }
		public virtual void DrawSubProperties() { }
		public virtual void DrawMaterialProperties() { }

		public virtual string GetPropertyValue() { return string.Empty; }

		public string GetInstancedPropertyValue( bool isTemplate )
		{
			if ( isTemplate )
				return string.Format( IOUtils.InstancedPropertiesElement, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ), m_propertyName );
			else
				return string.Format( IOUtils.InstancedPropertiesElementTabs, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ), m_propertyName );
		}

		public virtual string GetUniformValue()
		{
			return string.Format( Constants.UniformDec, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ), m_propertyName );
		}

		public virtual bool GetUniformData( out string dataType, out string dataName )
		{
			dataType = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			dataName = m_propertyName;
			return true;
		}

		public PropertyType CurrentParameterType
		{
			get { return m_currentParameterType; }
			set { m_currentParameterType = value; }
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentParameterType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_propertyName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_propertyInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndex );
			int attribCount = m_selectedAttribs.Count;
			IOUtils.AddFieldValueToString( ref nodeInfo, attribCount );
			if ( attribCount > 0 )
			{
				for ( int i = 0; i < attribCount; i++ )
				{
					IOUtils.AddFieldValueToString( ref nodeInfo, m_availableAttribs[ m_selectedAttribs[ i ] ].Attribute );
				}
			}
			//IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndexOffset );
		}

		int IdForAttrib( string name )
		{
			int attribCount = m_availableAttribs.Count;
			for ( int i = 0; i < attribCount; i++ )
			{
				if ( m_availableAttribs[ i ].Attribute.Equals( name ) )
					return i;
			}
			return 0;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() < 2505 )
			{
				string property = GetCurrentParam( ref nodeParams );
				m_currentParameterType = property.Equals( "Uniform" ) ? PropertyType.Global : ( PropertyType ) Enum.Parse( typeof( PropertyType ), property );
			}
			else
			{
				m_currentParameterType = ( PropertyType ) Enum.Parse( typeof( PropertyType ), GetCurrentParam( ref nodeParams ) );
			}

			if ( m_currentParameterType == PropertyType.InstancedProperty )
			{
				UIUtils.AddInstancePropertyCount();
				UIUtils.RegisterPropertyNode( this );
			}

			if ( m_currentParameterType == PropertyType.Property )
			{
				UIUtils.RegisterPropertyNode( this );
			}

			m_propertyName = GetCurrentParam( ref nodeParams );
			m_propertyInspectorName = GetCurrentParam( ref nodeParams );

			if ( UIUtils.CurrentShaderVersion() > 13 )
			{
				m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}

			if ( UIUtils.CurrentShaderVersion() > 4102 )
			{
				int attribAmount = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				if ( attribAmount > 0 )
				{
					for ( int i = 0; i < attribAmount; i++ )
					{
						m_selectedAttribs.Add( IdForAttrib( GetCurrentParam( ref nodeParams ) ) );
					}
				}
				InitializeAttribsArray();
			}

			//if ( UIUtils.CurrentShaderVersion() > 7101 )
			//{
			//	m_orderIndexOffset = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			//}

			m_propertyNameIsDirty = true;
			m_reRegisterName = false;

			UIUtils.ReleaseUniformName( UniqueId, m_oldName );
			UIUtils.RegisterUniformName( UniqueId, m_propertyName );
			m_oldName = m_propertyName;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			m_reRegisterName = true;
		}

		public bool CanDrawMaterial { get { return m_materialMode && m_currentParameterType != PropertyType.Constant; } }
		public int RawOrderIndex
		{
			get { return m_orderIndex; }
		}

		public int OrderIndex
		{
			get { return m_orderIndex + m_orderIndexOffset; }
			set { m_orderIndex = value; }
		}

		public int OrderIndexOffset
		{
			get { return m_orderIndexOffset; }
			set { m_orderIndexOffset = value; }
		}
		public string PropertyData { get { return ( m_currentParameterType == PropertyType.InstancedProperty ) ? m_outputPorts[ 0 ].LocalValue : m_propertyName; } }
		public virtual string PropertyName { get { return m_propertyName; } }
		public string PropertyInspectorName { get { return m_propertyInspectorName; } }
		public bool FreeType { get { return m_freeType; } set { m_freeType = value; } }
		public bool ReRegisterName { get { return m_reRegisterName; } set { m_reRegisterName = value; } }
		public string CustomPrefix { get { return m_customPrefix; } set { m_customPrefix = value; } }
		public override void RefreshOnUndo()
		{
			base.RefreshOnUndo();
			BeginPropertyFromInspectorCheck();
		}
	}
}
