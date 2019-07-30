// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Custom Expression", "Miscellaneous", "Creates a custom expression or function if <b>return</b> is detected in the written code." )]
	public sealed class CustomExpressionNode : ParentNode
	{
		private const string CustomExpressionInfo = "Creates a custom expression or function according to how code is written on text area.\n\n" +
													" - If a return function is detected on Code text area then a function will be created.\n" +
													"Also in function mode a ; is expected on the end of each instruction line.\n\n" +
													"- If no return function is detected them an expression will be generated and used directly on the vertex/frag body.\n" +
													"On Expression mode a ; is not required on the end of an instruction line.";
		private const char LineFeedSeparator = '$';
		private const char SemiColonSeparator = '@';
		private const string ReturnHelper = "return";
		private const double MaxTimestamp = 1;
		private const string DefaultExpressionName = "My Custom Expression";
		private const string DefaultInputName = "In";
		private const string CodeTitleStr = "Code";
		private const string OutputTypeStr = "Output Type";
		private const string InputsStr = "Inputs";
		private const string InputNameStr = "Name";
		private const string InputTypeStr = "Type";
		private const string InputValueStr = "Value";
		private const string InputQualifierStr = "Qualifier";
		private const string ExpressionNameLabel = "Name";
		private const string FunctionCallMode = "Call Mode";

		private readonly string[] AvailableWireTypesStr =  
		{
		"int",
		"float",
		"float2",
		"float3",
		"float4",
		"float3x3",
		"float4x4",
		"sampler1D",
		"sampler2D",
		"sampler3D",
		"samplerCUBE"};

		private readonly string[] QualifiersStr =
		{
			"In",
			"Out",
			"InOut"
		};

		private readonly WirePortDataType[] AvailableWireTypes =
		{
			WirePortDataType.INT,
			WirePortDataType.FLOAT,
			WirePortDataType.FLOAT2,
			WirePortDataType.FLOAT3,
			WirePortDataType.FLOAT4,
			WirePortDataType.FLOAT3x3,
			WirePortDataType.FLOAT4x4,
			WirePortDataType.SAMPLER1D,
			WirePortDataType.SAMPLER2D,
			WirePortDataType.SAMPLER3D,
			WirePortDataType.SAMPLERCUBE
		};


		private readonly Dictionary<WirePortDataType, int> WireToIdx = new Dictionary<WirePortDataType, int>
		{
			{ WirePortDataType.INT,         0},
			{ WirePortDataType.FLOAT,       1},
			{ WirePortDataType.FLOAT2,      2},
			{ WirePortDataType.FLOAT3,      3},
			{ WirePortDataType.FLOAT4,      4},
			{ WirePortDataType.FLOAT3x3,    5},
			{ WirePortDataType.FLOAT4x4,    6},
			{ WirePortDataType.SAMPLER1D,   7},
			{ WirePortDataType.SAMPLER2D,   8},
			{ WirePortDataType.SAMPLER3D,   9},
			{ WirePortDataType.SAMPLERCUBE, 10}
		};

		[SerializeField]
		private string m_customExpressionName = string.Empty;

		[SerializeField]
		private List<bool> m_foldoutValuesFlags = new List<bool>();

		[SerializeField]
		private List<string> m_foldoutValuesLabels = new List<string>();

		[SerializeField]
		private List<VariableQualifiers> m_variableQualifiers = new List<VariableQualifiers>();

		[SerializeField]
		private string m_code = " ";

		[SerializeField]
		private int m_outputTypeIdx = 1;

		[SerializeField]
		private bool m_visibleInputsFoldout = true;

		[SerializeField]
		private bool m_callMode = false;

		[SerializeField]
		private bool m_functionMode = false;

		[SerializeField]
		private int m_firstAvailablePort = 0;

		[SerializeField]
		private string m_uniqueName;

		private int m_markedToDelete = -1;
		private const float ButtonLayoutWidth = 15;

		private bool m_repopulateNameDictionary = true;
		private Dictionary<string, int> m_usedNames = new Dictionary<string, int>();

		private double m_lastTimeNameModified = 0;
		private bool m_nameModified = false;

		private double m_lastTimeCodeModified = 0;
		private bool m_codeModified = false;

		private bool m_editPropertyNameMode = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "In0" );
			m_foldoutValuesFlags.Add( true );
			m_foldoutValuesLabels.Add( "[0]" );
			m_variableQualifiers.Add( VariableQualifiers.In );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_textLabelWidth = 95;
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();

			m_customExpressionName = DefaultExpressionName;
			SetTitleText( m_customExpressionName );

			if ( m_nodeAttribs != null )
				m_uniqueName = m_nodeAttribs.Name + OutputId;
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			CheckPortConnection( portId );
		}

		public override void OnConnectedOutputNodeChanges( int portId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( portId, otherNodeId, otherPortId, name, type );
			CheckPortConnection( portId );
		}

		void CheckPortConnection( int portId )
		{
			if ( portId == 0 && m_callMode )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			if ( m_nameModified )
			{
				if ( ( EditorApplication.timeSinceStartup - m_lastTimeNameModified ) > MaxTimestamp )
				{
					m_nameModified = false;
					m_repopulateNameDictionary = true;
				}
			}

			if ( m_repopulateNameDictionary )
			{
				m_repopulateNameDictionary = false;
				m_usedNames.Clear();
				for ( int i = 0; i < m_inputPorts.Count; i++ )
				{
					m_usedNames.Add( m_inputPorts[ i ].Name, i );
				}
			}

			if ( m_codeModified )
			{
				if ( ( EditorApplication.timeSinceStartup - m_lastTimeCodeModified ) > MaxTimestamp )
				{
					m_codeModified = false;
					m_functionMode = m_code.Contains( ReturnHelper );
				}
			}

			base.Draw( drawInfo );
		}

		public string GetFirstAvailableName()
		{
			string name = string.Empty;
			for ( int i = 0; i < m_inputPorts.Count + 1; i++ )
			{
				name = DefaultInputName + i;
				if ( !m_usedNames.ContainsKey( name ) )
				{
					return name;
				}
			}
			Debug.LogWarning( "Could not find valid name" );
			return string.Empty;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, DrawBaseProperties );
			NodeUtils.DrawPropertyGroup( ref m_visibleInputsFoldout, InputsStr, DrawInputs, DrawAddRemoveInputs );

			EditorGUILayout.HelpBox( CustomExpressionInfo, MessageType.Info );
		}

		string WrapCodeInFunction( bool isTemplate, string functionName )
		{
			//Hack to be used util indent is properly used
			int currIndent = UIUtils.ShaderIndentLevel;
			UIUtils.ShaderIndentLevel = isTemplate?0:1;

			if( !isTemplate ) UIUtils.ShaderIndentLevel++;

			//string functionName = UIUtils.RemoveInvalidCharacters( m_customExpressionName );
			string returnType = m_callMode ? "void" : UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			string functionBody = UIUtils.ShaderIndentTabs + returnType + " " + functionName + "( ";
			int count = m_inputPorts.Count - m_firstAvailablePort;
			for ( int i = 0; i < count; i++ )
			{
				int portIdx = i + m_firstAvailablePort;
				string qualifier = m_variableQualifiers[ i ] == VariableQualifiers.In?string.Empty:UIUtils.QualifierToCg( m_variableQualifiers[ i ] ) + " ";
				functionBody += qualifier + UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_inputPorts[ portIdx ].DataType ) + " " + m_inputPorts[ portIdx ].Name;
				if ( i < ( count - 1 ) )
				{
					functionBody += " , ";
				}
			}
			functionBody += " )\n" + UIUtils.ShaderIndentTabs + "{\n";
			UIUtils.ShaderIndentLevel++;
			{
				string[] codeLines = m_code.Split( IOUtils.LINE_TERMINATOR );
				for ( int i = 0; i < codeLines.Length; i++ )
				{
					if ( codeLines[ i ].Length > 0 )
					{
						functionBody += UIUtils.ShaderIndentTabs + codeLines[ i ] + '\n';
					}
				}
			}
			UIUtils.ShaderIndentLevel--;

			functionBody += UIUtils.ShaderIndentTabs + "}\n";
			UIUtils.ShaderIndentLevel = currIndent;
			return functionBody;
		}

		void DrawBaseProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_customExpressionName = EditorGUILayoutTextField( ExpressionNameLabel, m_customExpressionName );
			if ( EditorGUI.EndChangeCheck() )
			{
				SetTitleText( m_customExpressionName );
			}

			DrawPrecisionProperty();

			EditorGUI.BeginChangeCheck();
			m_callMode = EditorGUILayoutToggle( FunctionCallMode, m_callMode );
			if ( EditorGUI.EndChangeCheck() )
			{
				SetupCallMode();
			}


			EditorGUILayout.LabelField( CodeTitleStr );
			EditorGUI.BeginChangeCheck();
			{
				m_code = EditorGUILayoutTextArea( m_code, UIUtils.MainSkin.textArea );
			}
			if ( EditorGUI.EndChangeCheck() )
			{
				m_codeModified = true;
				m_lastTimeCodeModified = EditorApplication.timeSinceStartup;
			}

			if ( !m_callMode )
			{
				EditorGUI.BeginChangeCheck();
				m_outputTypeIdx = EditorGUILayoutPopup( OutputTypeStr, m_outputTypeIdx, AvailableWireTypesStr );
				if ( EditorGUI.EndChangeCheck() )
				{
					m_outputPorts[ 0 ].ChangeType( AvailableWireTypes[ m_outputTypeIdx ], false );
				}
			}
		}

		void SetupCallMode()
		{
			if ( m_callMode )
			{
				m_firstAvailablePort = 1;
				AddInputPortAt( 0, WirePortDataType.FLOAT, false, DefaultInputName );
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT, false );
			}
			else
			{
				m_firstAvailablePort = 0;
				if ( m_inputPorts[ 0 ].IsConnected )
				{
					m_containerGraph.DeleteConnection( true, UniqueId, m_inputPorts[ 0 ].PortId, false, true );
				}
				DeleteInputPortByArrayIdx( 0 );
				m_outputPorts[ 0 ].ChangeType( AvailableWireTypes[ m_outputTypeIdx ], false );
			}
		}

		void DrawAddRemoveInputs()
		{
			if ( m_inputPorts.Count == m_firstAvailablePort )
				m_visibleInputsFoldout = false;

			// Add new port
			if ( GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				AddPortAt( m_inputPorts.Count );
				m_visibleInputsFoldout = true;
			}

			//Remove port
			if ( GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				RemovePortAt( m_inputPorts.Count - 1 );
			}
		}

		void DrawInputs()
		{
			int count = m_inputPorts.Count - m_firstAvailablePort;
			for ( int i = 0; i < count; i++ )
			{
				int portIdx = i + m_firstAvailablePort;
				m_foldoutValuesFlags[ i ] = EditorGUILayoutFoldout( m_foldoutValuesFlags[ i ], m_foldoutValuesLabels[ i ] + " - " + m_inputPorts[ portIdx ].Name );

				if ( m_foldoutValuesFlags[ i ] )
				{
					EditorGUI.indentLevel += 1;

					//Qualifier
					bool guiEnabled = GUI.enabled;
					GUI.enabled = m_functionMode;
					m_variableQualifiers[ i ] = ( VariableQualifiers ) EditorGUILayoutPopup( InputQualifierStr, (int)m_variableQualifiers[ i ],QualifiersStr );
					GUI.enabled = guiEnabled;

					// Type
					int typeIdx = WireToIdx[ m_inputPorts[ portIdx ].DataType ];
					EditorGUI.BeginChangeCheck();
					{
						typeIdx = EditorGUILayoutPopup( InputTypeStr, typeIdx, AvailableWireTypesStr );
					}

					if ( EditorGUI.EndChangeCheck() )
					{
						m_inputPorts[ portIdx ].ChangeType( AvailableWireTypes[ typeIdx ], false );
					}

					//Name
					EditorGUI.BeginChangeCheck();
					{
						m_inputPorts[ portIdx ].Name = EditorGUILayoutTextField( InputNameStr, m_inputPorts[ portIdx ].Name );
					}
					if ( EditorGUI.EndChangeCheck() )
					{
						m_nameModified = true;
						m_lastTimeNameModified = EditorApplication.timeSinceStartup;
						m_inputPorts[ portIdx ].Name = UIUtils.RemoveInvalidCharacters( m_inputPorts[ portIdx ].Name );
						if ( string.IsNullOrEmpty( m_inputPorts[ portIdx ].Name ) )
						{
							m_inputPorts[ portIdx ].Name = DefaultInputName + i;
						}
					}

					// Port Data
					if ( !m_inputPorts[ portIdx ].IsConnected )
					{
						m_inputPorts[ portIdx ].ShowInternalData( this, true, InputValueStr );
					}

					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label( " " );
						// Add new port
						if ( GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
						{
							AddPortAt( portIdx );
						}

						//Remove port
						if ( GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
						{
							m_markedToDelete = portIdx;
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUI.indentLevel -= 1;
				}
			}

			if ( m_markedToDelete > -1 )
			{
				DeleteInputPortByArrayIdx( m_markedToDelete );
				m_markedToDelete = -1;
				m_repopulateNameDictionary = true;
			}
		}

		void AddPortAt( int idx )
		{
			AddInputPortAt( idx, WirePortDataType.FLOAT, false, GetFirstAvailableName() );

			m_foldoutValuesFlags.Add( true );
			m_foldoutValuesLabels.Add( "[" + idx + "]" );
			m_variableQualifiers.Add( VariableQualifiers.In );
			m_repopulateNameDictionary = true;
		}

		void RemovePortAt( int idx )
		{
			if ( m_inputPorts.Count > m_firstAvailablePort )
			{
				DeleteInputPortByArrayIdx( idx );
				m_foldoutValuesFlags.RemoveAt( idx );
				m_foldoutValuesLabels.RemoveAt( idx );
				m_variableQualifiers.RemoveAt( idx );
				m_repopulateNameDictionary = true;
			}
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			m_repopulateNameDictionary = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( string.IsNullOrEmpty( m_code ) )
			{
				UIUtils.ShowMessage( "Custom Expression need to have code associated", MessageSeverity.Warning );
				return "0";
			}

			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			string expressionName = UIUtils.RemoveInvalidCharacters( m_customExpressionName ) + OutputId;
			int count = m_inputPorts.Count;
			if ( count > 0 )
			{
				if ( m_callMode )
				{
					string mainData = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
					RegisterLocalVariable( 0, string.Format( Constants.CodeWrapper, mainData ), ref dataCollector, "local" + expressionName + OutputId );
				}


				if ( m_code.Contains( ReturnHelper ) )
				{

					string function = WrapCodeInFunction( dataCollector.IsTemplate, expressionName );
					dataCollector.AddFunction( expressionName, function );

					string functionCall = expressionName + "( ";
					for ( int i = m_firstAvailablePort; i < count; i++ )
					{
						string inputPortLocalVar = m_inputPorts[ i ].Name + OutputId;
						string result = m_inputPorts[ i ].GeneratePortInstructions( ref dataCollector );
						dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, inputPortLocalVar, result );

						functionCall += inputPortLocalVar;
						if ( i < ( count - 1 ) )
						{
							functionCall += " , ";
						}
					}
					functionCall += " )";

					if ( m_callMode )
					{
						dataCollector.AddLocalVariable( 0, functionCall + ";", true );
					}
					else
					{
						RegisterLocalVariable( 0, functionCall, ref dataCollector, "local" + expressionName + OutputId );
					}
				}
				else
				{
					string localCode = m_code;
					for ( int i = m_firstAvailablePort; i < count; i++ )
					{
						string inputPortLocalVar = m_inputPorts[ i ].Name + OutputId;
						localCode = localCode.Replace( m_inputPorts[ i ].Name, inputPortLocalVar );

						if ( m_inputPorts[ i ].IsConnected )
						{
							string result = m_inputPorts[ i ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ i ].DataType, true, true );
							dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, inputPortLocalVar, result );
						}
						else
						{
							dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, inputPortLocalVar, m_inputPorts[ i ].WrappedInternalData );
						}
					}
					if ( m_callMode )
					{
						string[] codeLines = localCode.Split( '\n' );
						for ( int codeIdx = 0; codeIdx < codeLines.Length; codeIdx++ )
						{
							dataCollector.AddLocalVariable( 0, codeLines[ codeIdx ], true );
						}
					}
					else
					{
						RegisterLocalVariable( 0, string.Format( Constants.CodeWrapper, localCode ), ref dataCollector, "local" + expressionName + OutputId );
					}
				}

				return m_outputPorts[ 0 ].LocalValue;
						}
			else
			{
				if ( m_code.Contains( ReturnHelper ) )
				{
					string function = WrapCodeInFunction( dataCollector.IsTemplate, expressionName );
					dataCollector.AddFunction( expressionName, function );
					string functionCall = expressionName + "()";
					RegisterLocalVariable( 0, functionCall, ref dataCollector, "local" + expressionName + OutputId );
					}
				else
				{
					RegisterLocalVariable( 0, string.Format( Constants.CodeWrapper, m_code ), ref dataCollector, "local" + expressionName + OutputId );
				}

				return m_outputPorts[ 0 ].LocalValue;
			}
		}

		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if ( currentMousePos2D.y - m_globalPosition.y > Constants.NODE_HEADER_HEIGHT + Constants.NODE_HEADER_EXTRA_HEIGHT )
			{
				ContainerGraph.ParentWindow.ParametersWindow.IsMaximized = !ContainerGraph.ParentWindow.ParametersWindow.IsMaximized;
			}
			else
			{
				m_editPropertyNameMode = true;
				GUI.FocusControl( m_uniqueName );
				TextEditor te = ( TextEditor ) GUIUtility.GetStateObject( typeof( TextEditor ), GUIUtility.keyboardControl );
				if ( te != null )
				{
					te.SelectAll();
				}
			}
		}

		public override void OnNodeSelected( bool value )
		{
			base.OnNodeSelected( value );
			if ( !value )
				m_editPropertyNameMode = false;
		}

		public override void DrawTitle( Rect titlePos )
		{
			if ( m_editPropertyNameMode )
			{
				titlePos.height = Constants.NODE_HEADER_HEIGHT;
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName( m_uniqueName );
				m_customExpressionName = GUITextField( titlePos, m_customExpressionName, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
				if ( EditorGUI.EndChangeCheck() )
				{
					SetTitleText( m_customExpressionName );
				}

				if ( Event.current.isKey && ( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter ) )
				{
					m_editPropertyNameMode = false;
					GUIUtility.keyboardControl = 0;
				}
			}
			else
			{
				base.DrawTitle( titlePos );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			// This node is, by default, created with one input port 
			base.ReadFromString( ref nodeParams );
			m_code = GetCurrentParam( ref nodeParams );
			m_code = m_code.Replace( LineFeedSeparator, '\n' );
			m_code = m_code.Replace( SemiColonSeparator, ';' );
			m_outputTypeIdx = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_outputPorts[ 0 ].ChangeType( AvailableWireTypes[ m_outputTypeIdx ], false );

			if ( UIUtils.CurrentShaderVersion() > 12001 )
			{
				m_callMode = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				if ( m_callMode )
				{
					m_firstAvailablePort = 1;
					AddInputPortAt( 0, WirePortDataType.FLOAT, false, DefaultInputName );
				}
			}

			int count = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if ( count == 0 )
			{
				DeleteInputPortByArrayIdx( 0 );
				m_foldoutValuesLabels.Clear();
				m_variableQualifiers.Clear();
			}
			else
			{
				for ( int i = 0; i < count; i++ )
				{
					bool foldoutValue = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					string name = GetCurrentParam( ref nodeParams );
					WirePortDataType type = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
					string internalData = GetCurrentParam( ref nodeParams );
					VariableQualifiers qualifier = VariableQualifiers.In;
					if ( UIUtils.CurrentShaderVersion() > 12001 )
					{
						qualifier = ( VariableQualifiers ) Enum.Parse( typeof( VariableQualifiers ), GetCurrentParam( ref nodeParams ) );
					}

					int portIdx = i + m_firstAvailablePort;
					if ( i == 0 )
					{
						m_inputPorts[ portIdx ].ChangeProperties( name, type, false );
						m_variableQualifiers[ 0 ] = qualifier;
						m_foldoutValuesFlags[ 0 ] = foldoutValue;
					}
					else
					{
						m_foldoutValuesLabels.Add( "[" + i + "]" );
						m_variableQualifiers.Add( qualifier );
						m_foldoutValuesFlags.Add( foldoutValue );
					   AddInputPort( type, false, name );
					}
					m_inputPorts[ i ].InternalData = internalData;
				}
			}

			if ( UIUtils.CurrentShaderVersion() > 7205 )
			{
				m_customExpressionName = GetCurrentParam( ref nodeParams );
				SetTitleText( m_customExpressionName );
			}
			
			m_repopulateNameDictionary = true;
			m_functionMode = m_code.Contains( ReturnHelper );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );

			string parsedCode = m_code.Replace( '\n', LineFeedSeparator );
			parsedCode = parsedCode.Replace( ';', SemiColonSeparator );

			IOUtils.AddFieldValueToString( ref nodeInfo, parsedCode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputTypeIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_callMode );

			int count = m_inputPorts.Count - m_firstAvailablePort;
			IOUtils.AddFieldValueToString( ref nodeInfo, count );
			for ( int i = 0; i < count; i++ )
			{
				int portIdx = m_firstAvailablePort + i;
				IOUtils.AddFieldValueToString( ref nodeInfo, m_foldoutValuesFlags[ i ] );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ portIdx ].Name );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ portIdx ].DataType );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ portIdx ].InternalData );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_variableQualifiers[ i ] );
			}
			IOUtils.AddFieldValueToString( ref nodeInfo, m_customExpressionName );
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			int portCount = m_inputPorts.Count;
			for ( int i = 0; i < portCount; i++ )
			{
				if ( m_inputPorts[i].DataType == WirePortDataType.COLOR )
				{
					m_inputPorts[i].ChangeType( WirePortDataType.FLOAT4, false ); ;
				}
			}
		}
	}
}
