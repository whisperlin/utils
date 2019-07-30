// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public class PropertyDataCollector
	{
		public int NodeId;
		public int OrderIndex;
		public string PropertyName;

		public PropertyDataCollector( int nodeId, string propertyName, int orderIndex = -1 )
		{
			NodeId = nodeId;
			PropertyName = propertyName;
			OrderIndex = orderIndex;
		}
	}

	public class TextureDefaultsDataColector
	{
		private List<string> m_names = new List<string>();
		private List<Texture> m_values = new List<Texture>();
		public void AddValue( string newName, Texture newValue )
		{
			m_names.Add( newName );
			m_values.Add( newValue );
		}

		public void Destroy()
		{
			m_names.Clear();
			m_names = null;

			m_values.Clear();
			m_values = null;
		}

		public string[] NamesArr { get { return m_names.ToArray(); } }
		public Texture[] ValuesArr { get { return m_values.ToArray(); } }
	}

	public enum TextureChannelUsage
	{
		Not_Used,
		Used,
		Required
	}

	public class MasterNodeDataCollector
	{
		private bool m_showDebugMessages = false;
		private string m_input;
		private string m_customInput;
		private string m_properties;
		private string m_instancedProperties;
		private string m_uniforms;
		private string m_includes;
		private string m_pragmas;
		private string m_instructions;
		private string m_localVariables;
		private string m_vertexLocalVariables;
		private string m_specialLocalVariables;
		private string m_vertexData;
		private string m_customOutput;
		private string m_functions;
		private string m_grabPass;

		private List<PropertyDataCollector> m_inputList;
		private List<PropertyDataCollector> m_customInputList;
		private List<PropertyDataCollector> m_propertiesList;
		private List<PropertyDataCollector> m_instancedPropertiesList;
		private List<PropertyDataCollector> m_uniformsList;
		private List<PropertyDataCollector> m_includesList;
		private List<PropertyDataCollector> m_tagsList;
		private List<PropertyDataCollector> m_pragmasList;
		private List<PropertyDataCollector> m_instructionsList;
		private List<PropertyDataCollector> m_localVariablesList;
		private List<PropertyDataCollector> m_vertexLocalVariablesList;
		private List<PropertyDataCollector> m_specialLocalVariablesList;
		private List<PropertyDataCollector> m_vertexDataList;
		private List<PropertyDataCollector> m_customOutputList;
		private List<PropertyDataCollector> m_functionsList;
		private List<PropertyDataCollector> m_grabPassList;

		private Dictionary<string, PropertyDataCollector> m_inputDict;
		private Dictionary<string, PropertyDataCollector> m_customInputDict;
		private Dictionary<string, PropertyDataCollector> m_propertiesDict;
		private Dictionary<string, PropertyDataCollector> m_instancedPropertiesDict;
		private Dictionary<string, PropertyDataCollector> m_uniformsDict;
		private Dictionary<string, PropertyDataCollector> m_includesDict;
		private Dictionary<string, PropertyDataCollector> m_tagsDict;
		private Dictionary<string, PropertyDataCollector> m_pragmasDict;
		private Dictionary<string, int> m_virtualCoordinatesDict;
		private Dictionary<string, string> m_virtualVariablesDict;
		private Dictionary<string, PropertyDataCollector> m_localVariablesDict;
		private Dictionary<string, PropertyDataCollector> m_vertexLocalVariablesDict;
		private Dictionary<string, PropertyDataCollector> m_specialLocalVariablesDict;
		private Dictionary<string, PropertyDataCollector> m_vertexDataDict;
		private Dictionary<string, PropertyDataCollector> m_customOutputDict;
		private Dictionary<string, string> m_localFunctions;
		private TextureChannelUsage[] m_requireTextureProperty = { TextureChannelUsage.Not_Used, TextureChannelUsage.Not_Used, TextureChannelUsage.Not_Used, TextureChannelUsage.Not_Used };

		private bool m_dirtyInputs;
		private bool m_dirtyCustomInputs;
		private bool m_dirtyFunctions;
		private bool m_dirtyProperties;
		private bool m_dirtyInstancedProperties;
		private bool m_dirtyUniforms;
		private bool m_dirtyIncludes;
		private bool m_dirtyPragmas;
		private bool m_dirtyInstructions;
		private bool m_dirtyLocalVariables;
		private bool m_dirtyVertexLocalVariables;
		private bool m_dirtySpecialLocalVariables;
		private bool m_dirtyPerVertexData;
		private bool m_dirtyNormal;
		private bool m_forceNormal;

		private bool m_usingInternalData;
		private bool m_usingTexcoord0;
		private bool m_usingTexcoord1;
		private bool m_usingTexcoord2;
		private bool m_usingTexcoord3;
		private bool m_usingWorldPosition;
		private bool m_usingWorldNormal;
		private bool m_usingWorldReflection;
		private bool m_usingViewDirection;
		private bool m_usingLightAttenuation;
		private bool m_usingArrayDerivatives;

		private bool m_usingHigherSizeTexcoords;

		private bool m_usingCustomOutput;

		private bool m_forceNormalIsDirty;
		private bool m_grabPassIsDirty;
		private bool m_tesselationActive;

		private Dictionary<int, PropertyNode> m_propertyNodes;
		private MasterNode m_masterNode;

		private int m_availableUvInd = 0;
		private int m_availableVertexTempId = 0;
		private int m_availableFragTempId = 0;

		private MasterNodePortCategory m_portCategory;
		private RenderPath m_renderPath = RenderPath.All;

		//Templates specific data
		private AvailableShaderTypes m_masterNodeCategory;
		private List<string> m_vertexInputList;
		private Dictionary<string, string> m_vertexInputDict;
		private List<string> m_interpolatorsList;
		private Dictionary<string, string> m_interpolatorsDict;
		private List<string> m_vertexInterpDeclList;
		private Dictionary<string, string> m_vertexInterpDeclDict;
		private TemplateDataCollector m_templateDataCollector;

		public MasterNodeDataCollector( MasterNode masterNode ) : this()
		{
			m_masterNode = masterNode;
			m_masterNodeCategory = masterNode.CurrentMasterNodeCategory;
		}

		public MasterNodeDataCollector()
		{
			//m_masterNode = masterNode;
			m_input = "\t\tstruct Input\n\t\t{\n";
			m_customInput = "\t\tstruct SurfaceOutput{0}\n\t\t{\n";
			m_properties = IOUtils.PropertiesBegin;//"\tProperties\n\t{\n";
			m_uniforms = string.Empty;
			m_instructions = string.Empty;
			m_includes = string.Empty;
			m_pragmas = string.Empty;
			m_localVariables = string.Empty;
			m_specialLocalVariables = string.Empty;
			m_customOutput = string.Empty;

			m_inputList = new List<PropertyDataCollector>();
			m_customInputList = new List<PropertyDataCollector>();
			m_propertiesList = new List<PropertyDataCollector>();
			m_instancedPropertiesList = new List<PropertyDataCollector>();
			m_uniformsList = new List<PropertyDataCollector>();
			m_includesList = new List<PropertyDataCollector>();
			m_tagsList = new List<PropertyDataCollector>();
			m_pragmasList = new List<PropertyDataCollector>();
			m_instructionsList = new List<PropertyDataCollector>();
			m_localVariablesList = new List<PropertyDataCollector>();
			m_vertexLocalVariablesList = new List<PropertyDataCollector>();
			m_specialLocalVariablesList = new List<PropertyDataCollector>();
			m_vertexDataList = new List<PropertyDataCollector>();
			m_customOutputList = new List<PropertyDataCollector>();
			m_functionsList = new List<PropertyDataCollector>();
			m_grabPassList = new List<PropertyDataCollector>();


			m_inputDict = new Dictionary<string, PropertyDataCollector>();
			m_customInputDict = new Dictionary<string, PropertyDataCollector>();

			m_propertiesDict = new Dictionary<string, PropertyDataCollector>();
			m_instancedPropertiesDict = new Dictionary<string, PropertyDataCollector>();
			m_uniformsDict = new Dictionary<string, PropertyDataCollector>();
			m_includesDict = new Dictionary<string, PropertyDataCollector>();
			m_tagsDict = new Dictionary<string, PropertyDataCollector>();
			m_pragmasDict = new Dictionary<string, PropertyDataCollector>();
			m_virtualCoordinatesDict = new Dictionary<string, int>();
			m_localVariablesDict = new Dictionary<string, PropertyDataCollector>();
			m_virtualVariablesDict = new Dictionary<string, string>();
			m_specialLocalVariablesDict = new Dictionary<string, PropertyDataCollector>();
			m_vertexLocalVariablesDict = new Dictionary<string, PropertyDataCollector>();
			m_localFunctions = new Dictionary<string, string>();
			m_vertexDataDict = new Dictionary<string, PropertyDataCollector>();
			m_customOutputDict = new Dictionary<string, PropertyDataCollector>();

			m_dirtyInputs = false;
			m_dirtyCustomInputs = false;
			m_dirtyProperties = false;
			m_dirtyInstancedProperties = false;
			m_dirtyUniforms = false;
			m_dirtyInstructions = false;
			m_dirtyIncludes = false;
			m_dirtyPragmas = false;
			m_dirtyLocalVariables = false;
			m_dirtySpecialLocalVariables = false;
			m_grabPassIsDirty = false;

			m_portCategory = MasterNodePortCategory.Fragment;
			m_propertyNodes = new Dictionary<int, PropertyNode>();
			m_showDebugMessages = ( m_showDebugMessages && DebugConsoleWindow.DeveloperMode );

			//templates
			//m_masterNodeCategory = masterNode.CurrentMasterNodeCategory;

			m_vertexInputList = new List<string>();
			m_vertexInputDict = new Dictionary<string, string>();

			m_interpolatorsList = new List<string>();
			m_interpolatorsDict = new Dictionary<string, string>();

			m_vertexInterpDeclList = new List<string>();
			m_vertexInterpDeclDict = new Dictionary<string, string>();

			m_templateDataCollector = new TemplateDataCollector();
		}

		public void SetChannelUsage( int channelId, TextureChannelUsage usage )
		{
			if ( channelId > -1 && channelId < 4 )
				m_requireTextureProperty[ channelId ] = usage;
		}

		public TextureChannelUsage GetChannelUsage( int channelId )
		{
			if ( channelId > -1 && channelId < 4 )
				return m_requireTextureProperty[ channelId ];

			return TextureChannelUsage.Not_Used;
		}

		public void OpenPerVertexHeader( bool includeCustomData )
		{
			if ( m_dirtyPerVertexData )
				return;

			m_dirtyPerVertexData = true;
			if ( m_tesselationActive )
			{
				m_vertexData = "\t\tvoid " + Constants.VertexDataFunc + "( inout appdata " + Constants.VertexShaderInputStr + " )\n\t\t{\n";
			}
			else
			{
				m_vertexData = "\t\tvoid " + Constants.VertexDataFunc + "( inout appdata_full " + Constants.VertexShaderInputStr + ( includeCustomData ? ( string.Format( ", out Input {0}", Constants.VertexShaderOutputStr ) ) : string.Empty ) + " )\n\t\t{\n";
				if ( includeCustomData )
					m_vertexData += string.Format( "\t\t\tUNITY_INITIALIZE_OUTPUT( Input, {0} );\n", Constants.VertexShaderOutputStr );
			}
		}

		public void ClosePerVertexHeader()
		{
			if ( m_dirtyPerVertexData )
				m_vertexData += "\t\t}\n\n";
		}

		public void AddToVertexDisplacement( string value, VertexMode vertexMode )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_dirtyPerVertexData )
			{
				OpenPerVertexHeader( true );
			}

			switch ( vertexMode )
			{
				default:
				case VertexMode.Relative:
				{
					m_vertexData += "\t\t\t" + Constants.VertexShaderInputStr + ".vertex.xyz += " + value + ";\n";
				}
				break;
				case VertexMode.Absolute:
				{
					m_vertexData += "\t\t\t" + Constants.VertexShaderInputStr + ".vertex.xyz = " + value + ";\n";
				}
				break;
			}
		}


		public void AddToVertexNormal( string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_dirtyPerVertexData )
			{
				OpenPerVertexHeader( true );
			}

			m_vertexData += "\t\t\t" + Constants.VertexShaderInputStr + ".normal = " + value + ";\n";
		}


		public void AddVertexInstruction( string value, int nodeId = -1, bool addDelimiters = true )
		{
			if ( !m_dirtyPerVertexData )
			{
				OpenPerVertexHeader( true );
			}
			if ( !m_vertexDataDict.ContainsKey( value ) )
			{
				m_vertexDataDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_vertexDataList.Add( m_vertexDataDict[ value ] );
				m_vertexData += ( addDelimiters ? ( "\t\t\t" + value + ";\n" ) : value );
			}
		}

		public bool ContainsInput( string value )
		{
			return m_inputDict.ContainsKey( value );
		}

		public void AddToInput( int nodeId, string value, bool addSemiColon )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_inputDict.ContainsKey( value ) )
			{
				m_inputDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_inputList.Add( m_inputDict[ value ] );

				m_input += "\t\t\t" + value + ( ( addSemiColon ) ? ( ";\n" ) : "\n" );
				m_dirtyInputs = true;

				// TODO: move this elsewhere (waste of string calculations)
				if ( m_input.Contains( " worldNormal;" ) )
					UsingWorldNormal = true;

				if ( m_input.Contains( " worldRefl;" ) )
					UsingWorldReflection = true;

				if ( m_input.Contains( " worldPos;" ) )
					UsingWorldPosition = true;

				if ( m_input.Contains( " viewDir;" ) )
					UsingViewDirection = true;

				if ( m_input.Contains( "INTERNAL_DATA" ) )
					UsingInternalData = true;

				if ( m_input.Contains( "uv_texcoord;" ) )
					UsingTexcoord0 = true;

				if ( m_input.Contains( "uv2_texcoord2;" ) )
					UsingTexcoord1 = true;

				if ( m_input.Contains( "uv3_texcoord3;" ) )
					UsingTexcoord2 = true;

				if ( m_input.Contains( "uv4_texcoord4;" ) )
					UsingTexcoord3 = true;
			}
		}

		public void CloseInputs()
		{
			m_input += "\t\t};";
		}

		public void ChangeCustomInputHeader( string value )
		{
			m_customInput = m_customInput.Replace( "{0}", value );
		}

		public void AddToCustomInput( int nodeId, string value, bool addSemiColon )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_customInputDict.ContainsKey( value ) )
			{
				m_customInputDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_customInputList.Add( m_customInputDict[ value ] );
				m_customInput += "\t\t\t" + value + ( ( addSemiColon ) ? ( ";\n" ) : "\n" );
				m_dirtyCustomInputs = true;
			}
		}

		public void CloseCustomInputs()
		{
			m_customInput += "\t\t};";
		}



		// Used by Template Master Node to add tabs into variable declaration
		public void TabifyInstancedVars()
		{
			for ( int i = 0; i < m_instancedPropertiesList.Count; i++ )
			{
				m_instancedPropertiesList[ i ].PropertyName = '\t' + m_instancedPropertiesList[ i ].PropertyName;
			}
		}
		// Instanced properties
		public void SetupInstancePropertiesBlock( string blockName )
		{
			if ( IsTemplate )
			{
				Debug.LogWarning( "SetupInstancePropertiesBlock should not be used during template mode" );
			}


			if ( m_dirtyInstancedProperties )
			{
				m_instancedProperties = string.Format( IOUtils.InstancedPropertiesBeginTabs, blockName ) + m_instancedProperties + IOUtils.InstancedPropertiesEndTabs;
			}
		}

		public void AddToInstancedProperties( int nodeId, string value, int orderIndex )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_instancedPropertiesDict.ContainsKey( value ) )
			{
				m_instancedPropertiesDict.Add( value, new PropertyDataCollector( nodeId, value, orderIndex ) );
				m_instancedPropertiesList.Add( m_instancedPropertiesDict[ value ] );
				m_instancedProperties += value;
				m_dirtyInstancedProperties = true;
			}
		}

		public void CloseInstancedProperties()
		{
			if ( m_dirtyInstancedProperties )
			{
				m_instancedProperties += IOUtils.InstancedPropertiesEnd;
			}
		}

		// Properties
		public void AddToProperties( int nodeId, string value, int orderIndex )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_propertiesDict.ContainsKey( value ) )
			{
				//Debug.Log( UIUtils );
				m_propertiesDict.Add( value, new PropertyDataCollector( nodeId, value, orderIndex ) );
				m_propertiesList.Add( m_propertiesDict[ value ] );
				m_properties += string.Format( IOUtils.PropertiesElement, value );
				m_dirtyProperties = true;
			}
		}

		public string BuildPropertiesString()
		{
			List<PropertyDataCollector> list = new List<PropertyDataCollector>( m_propertiesDict.Values );
			//for ( int i = 0; i < list.Count; i++ )
			//{
			//	Debug.Log( list[ i ].OrderIndex + " " + list[ i ].PropertyName );
			//}

			list.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );
			m_properties = IOUtils.PropertiesBegin;
			for ( int i = 0; i < list.Count; i++ )
			{
				m_properties += string.Format( IOUtils.PropertiesElement, list[ i ].PropertyName );
				//Debug.Log()
			}
			m_properties += IOUtils.PropertiesEnd;
			return m_properties;
		}

		public void CloseProperties()
		{
			if ( m_dirtyProperties )
			{
				m_properties += IOUtils.PropertiesEnd;
			}
		}

		public void AddGrabPass( string value )
		{

			if ( string.IsNullOrEmpty( value ) )
			{
				if ( !m_grabPassIsDirty )
					m_grabPass += IOUtils.GrabPassEmpty;
			}
			else
			{
				m_grabPass += IOUtils.GrabPassBegin + value + IOUtils.GrabPassEnd;
			}
			m_grabPassList.Add( new PropertyDataCollector( -1, m_grabPass.Replace( "\t", string.Empty ).Replace( "\n", string.Empty ) ) );
			m_grabPassIsDirty = true;
		}

		// This is used by templates global variables to register already existing globals/properties
		public void SoftRegisterUniform( string dataName )
		{
			if ( !m_uniformsDict.ContainsKey( dataName ) )
			{
				m_uniformsDict.Add( dataName, new PropertyDataCollector( -1, dataName ) );
			}
		}
		public void AddToUniforms( int nodeId, string dataType, string dataName )
		{
			if ( string.IsNullOrEmpty( dataName ) || string.IsNullOrEmpty( dataType ) )
				return;

			if ( !m_uniformsDict.ContainsKey( dataName ) )
			{
				string value = UIUtils.GenerateUniformName( dataType, dataName );
				m_uniforms += "\t\t" + value + '\n';
				m_uniformsDict.Add( dataName, new PropertyDataCollector( nodeId, value ) );
				m_uniformsList.Add( m_uniformsDict[ dataName ] );
				m_dirtyUniforms = true;
			}
			else if ( m_uniformsDict[ dataName ].NodeId != nodeId )
			{
				if ( m_showDebugMessages ) UIUtils.ShowMessage( "AddToUniforms:Attempting to add duplicate " + dataName, MessageSeverity.Warning );
			}
		}

		public void AddToUniforms( int nodeId, string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_uniformsDict.ContainsKey( value ) )
			{
				m_uniforms += "\t\t" + value + '\n';
				m_uniformsDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_uniformsList.Add( m_uniformsDict[ value ] );
				m_dirtyUniforms = true;
			}
			else if ( m_uniformsDict[ value ].NodeId != nodeId )
			{
				if ( m_showDebugMessages ) UIUtils.ShowMessage( "AddToUniforms:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}
		}

		public void AddToIncludes( int nodeId, string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_includesDict.ContainsKey( value ) )
			{
				m_includesDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_includesList.Add( new PropertyDataCollector( nodeId, "#include \"" + value + "\"" ) );
				m_includes += "\t\t#include \"" + value + "\"\n";
				m_dirtyIncludes = true;
			}
			else
			{
				if ( m_showDebugMessages ) UIUtils.ShowMessage( "AddToIncludes:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}
		}

		public void AddToTags( int nodeId, string name, string value )
		{
			if ( string.IsNullOrEmpty( name ) || string.IsNullOrEmpty( value ) )
				return;

			if ( !m_tagsDict.ContainsKey( name ) )
			{
				string finalResult = string.Format( "\"{0}\"=\"{1}\"", name, value );
				m_tagsDict.Add( name, new PropertyDataCollector( nodeId, finalResult ) );
				m_tagsList.Add( new PropertyDataCollector( nodeId, finalResult ) );
			}
		}

		public void AddToPragmas( int nodeId, string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_pragmasDict.ContainsKey( value ) )
			{
				m_pragmasDict.Add( value, new PropertyDataCollector( nodeId, "#pragma " + value ) );
				m_pragmasList.Add( m_pragmasDict[ value ] );
				m_pragmas += "\t\t#pragma " + value + "\n";
				m_dirtyPragmas = true;
			}
			else
			{
				if ( m_showDebugMessages ) UIUtils.ShowMessage( "AddToPragmas:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}
		}

		public int GetVirtualCoordinatesId( int nodeId, string coord, string lodBias )
		{
			if ( !m_virtualCoordinatesDict.ContainsKey( coord ) )
			{
				m_virtualCoordinatesDict.Add( coord, nodeId );
				AddLocalVariable( nodeId, "VirtualCoord " + Constants.VirtualCoordNameStr + nodeId + " = VTComputeVirtualCoord" + lodBias + "(" + coord + ");" );
				return nodeId;
			}
			else
			{
				int fetchedId = 0;
				m_virtualCoordinatesDict.TryGetValue( coord, out fetchedId );
				return fetchedId;
			}
		}

		public bool AddToLocalVariables( MasterNodePortCategory category, int nodeId, PrecisionType precisionType, WirePortDataType type, string varName, string varValue )
		{
			if ( string.IsNullOrEmpty( varName ) || string.IsNullOrEmpty( varValue ) )
				return false;

			string value = UIUtils.PrecisionWirePortToCgType( precisionType, type ) + " " + varName + " = " + varValue + ";";
			return AddToLocalVariables( category, nodeId, value );
		}

		public bool AddToLocalVariables( int nodeId, PrecisionType precisionType, WirePortDataType type, string varName, string varValue )
		{
			if ( string.IsNullOrEmpty( varName ) || string.IsNullOrEmpty( varValue ) )
				return false;

			string value = UIUtils.PrecisionWirePortToCgType( precisionType, type ) + " " + varName + " = " + varValue + ";";
			return AddToLocalVariables( nodeId, value );
		}

		public bool AddToLocalVariables( MasterNodePortCategory category, int nodeId, string value, bool ignoreDuplicates = false )
		{
			if ( string.IsNullOrEmpty( value ) )
				return false;

			switch ( category )
			{
				case MasterNodePortCategory.Vertex:
				case MasterNodePortCategory.Tessellation:
				{
					return AddToVertexLocalVariables( nodeId, value, ignoreDuplicates );
				}
				case MasterNodePortCategory.Fragment:
				case MasterNodePortCategory.Debug:
				{
					return AddToLocalVariables( nodeId, value, ignoreDuplicates );
				}
			}

			return false;
		}

		public bool AddLocalVariable( int nodeId, PrecisionType precisionType, WirePortDataType type, string varName, string varValue )
		{
			if ( string.IsNullOrEmpty( varName ) || string.IsNullOrEmpty( varValue ) )
				return false;

			string value = UIUtils.PrecisionWirePortToCgType( precisionType, type ) + " " + varName + " = " + varValue + ";";
			return AddLocalVariable( nodeId, value );
		}

		public bool AddLocalVariable( int nodeId, string name, string value, bool ignoreDuplicates = false )
		{
			return AddLocalVariable( nodeId, name + " = " + value, ignoreDuplicates );
		}

		public bool AddLocalVariable( int nodeId, string value, bool ignoreDuplicates = false )
		{
			if ( string.IsNullOrEmpty( value ) )
				return false;

			switch ( m_portCategory )
			{
				case MasterNodePortCategory.Vertex:
				case MasterNodePortCategory.Tessellation:
				{
					return AddToVertexLocalVariables( nodeId, value, ignoreDuplicates );
				}
				case MasterNodePortCategory.Fragment:
				case MasterNodePortCategory.Debug:
				{
					return AddToLocalVariables( nodeId, value, ignoreDuplicates );
				}
			}

			return false;
		}

		public string AddVirtualLocalVariable( int nodeId, string variable, string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return string.Empty;

			string result = string.Empty;

			//switch ( m_portCategory )
			//{
			//case MasterNodePortCategory.Vertex:
			//case MasterNodePortCategory.Tessellation:
			//{
			//}
			//break;
			//case MasterNodePortCategory.Fragment:
			//case MasterNodePortCategory.Debug:
			//{
			if ( !m_virtualVariablesDict.ContainsKey( value ) )
			{
				m_virtualVariablesDict.Add( value, variable );
				result = variable;
			}
			else
			{
				m_virtualVariablesDict.TryGetValue( value, out result );
			}
			//}
			//break;
			//}

			return result;
		}

		public void AddCodeComments( bool forceForwardSlash, params string[] comments )
		{
			if ( m_portCategory == MasterNodePortCategory.Tessellation || m_portCategory == MasterNodePortCategory.Vertex )
			{
				AddToVertexLocalVariables( 0, IOUtils.CreateCodeComments( forceForwardSlash, comments ) );
			}
			else
			{
				AddToLocalVariables( 0, IOUtils.CreateCodeComments( forceForwardSlash, comments ) );
			}
		}

		public bool AddToLocalVariables( int nodeId, string value, bool ignoreDuplicates = false )
		{
			if ( string.IsNullOrEmpty( value ) )
				return false;

			if ( m_usingCustomOutput )
			{
				if ( !m_customOutputDict.ContainsKey( value ) || ignoreDuplicates )
				{
					if ( !m_customOutputDict.ContainsKey( value ) )
						m_customOutputDict.Add( value, new PropertyDataCollector( nodeId, value ) );

					m_customOutputList.Add( m_customOutputDict[ value ] );
					m_customOutput += "\t\t\t" + value + '\n';
					return true;
				}
				else
				{
					if ( m_showDebugMessages ) UIUtils.ShowMessage( "AddToLocalVariables:Attempting to add duplicate " + value, MessageSeverity.Warning );
				}
			}
			else
			{
				if ( !m_localVariablesDict.ContainsKey( value ) || ignoreDuplicates )
				{
					if ( !m_localVariablesDict.ContainsKey( value ) )
						m_localVariablesDict.Add( value, new PropertyDataCollector( nodeId, value ) );

					m_localVariablesList.Add( m_localVariablesDict[ value ] );
					AddToSpecialLocalVariables( nodeId, value, ignoreDuplicates );
					return true;
				}
				else
				{
					if ( m_showDebugMessages ) UIUtils.ShowMessage( "AddToLocalVariables:Attempting to add duplicate " + value, MessageSeverity.Warning );
				}
			}
			return false;
		}

		public void AddToSpecialLocalVariables( int nodeId, string value, bool ignoreDuplicates = false )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_specialLocalVariablesDict.ContainsKey( value ) || ignoreDuplicates )
			{
				if ( !m_specialLocalVariablesDict.ContainsKey( value ) )
					m_specialLocalVariablesDict.Add( value, new PropertyDataCollector( nodeId, value ) );

				m_specialLocalVariablesList.Add( m_specialLocalVariablesDict[ value ] );
				m_specialLocalVariables += "\t\t\t" + value + '\n';
				m_dirtySpecialLocalVariables = true;
			}
			else
			{
				if ( m_showDebugMessages ) UIUtils.ShowMessage( "AddToSpecialLocalVariables:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}
		}

		public void ClearSpecialLocalVariables()
		{
			//m_specialLocalVariablesDict.Clear();
			m_specialLocalVariables = string.Empty;
			m_dirtySpecialLocalVariables = false;
		}

		public bool AddToVertexLocalVariables( int nodeId, string varName, string varValue )
		{
			if ( string.IsNullOrEmpty( varName ) || string.IsNullOrEmpty( varValue ) )
				return false;

			string value = varName + " = " + varValue + ";";
			return AddToVertexLocalVariables( nodeId, value );
		}

		public bool AddToVertexLocalVariables( int nodeId, PrecisionType precisionType, WirePortDataType type, string varName, string varValue )
		{
			if ( string.IsNullOrEmpty( varName ) || string.IsNullOrEmpty( varValue ) )
				return false;

			string value = UIUtils.PrecisionWirePortToCgType( precisionType, type ) + " " + varName + " = " + varValue + ";";
			return AddToVertexLocalVariables( nodeId, value );
		}

		public bool AddToVertexLocalVariables( int nodeId, string value, bool ignoreDuplicates = false )
		{
			if ( string.IsNullOrEmpty( value ) )
				return false;

			if ( !m_vertexLocalVariablesDict.ContainsKey( value ) || ignoreDuplicates )
			{
				if ( !m_vertexLocalVariablesDict.ContainsKey( value ) )
					m_vertexLocalVariablesDict.Add( value, new PropertyDataCollector( nodeId, value ) );

				m_vertexLocalVariablesList.Add( m_vertexLocalVariablesDict[ value ] );
				m_vertexLocalVariables += "\t\t\t" + value + '\n';
				m_dirtyVertexLocalVariables = true;
				return true;
			}
			else
			{
				if ( m_showDebugMessages ) UIUtils.ShowMessage( "AddToVertexLocalVariables:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}

			return false;
		}

		public void ClearVertexLocalVariables()
		{
			//m_vertexLocalVariablesDict.Clear();
			m_vertexLocalVariables = string.Empty;
			m_dirtyVertexLocalVariables = false;
		}


		public bool CheckFunction( string header )
		{
			return m_localFunctions.ContainsKey( header );
		}

		public string AddFunctions( string header, string body, params object[] inParams )
		{
			if ( !m_localFunctions.ContainsKey( header ) )
			{
				m_localFunctions.Add( header, body );
				m_functionsList.Add( new PropertyDataCollector( -1, body ) );
				m_functions += "\n" + body + "\n";
				m_dirtyFunctions = true;
			}

			return String.Format( header, inParams );
		}

		public string AddFunctions( string header, string[] bodyLines, bool addNewLine, params object[] inParams )
		{
			if ( !m_localFunctions.ContainsKey( header ) )
			{
				string body = string.Empty;
				for ( int i = 0; i < bodyLines.Length; i++ )
				{
					body += ( m_masterNodeCategory == AvailableShaderTypes.Template ) ? bodyLines[ i ] : "\t\t" + bodyLines[ i ];
					if ( addNewLine )
						body += '\n';
				}

				m_localFunctions.Add( header, body );
				m_functionsList.Add( new PropertyDataCollector( -1, body ) );
				m_functions += "\n" + body + "\n";
				m_dirtyFunctions = true;
			}

			return String.Format( header, inParams );
		}

		public bool HasFunction( string functionId )
		{
			return m_localFunctions.ContainsKey( functionId );
		}

		public void AddFunction( string functionId, string body )
		{
			if ( !m_localFunctions.ContainsKey( functionId ) )
			{
				m_functionsList.Add( new PropertyDataCollector( -1, body ) );

				m_localFunctions.Add( functionId, body );
				m_functions += "\n" + body + "\n";
				m_dirtyFunctions = true;
			}
		}

		public void AddFunction( string functionId, string[] bodyLines, bool addNewline )
		{
			if ( !m_localFunctions.ContainsKey( functionId ) )
			{
				string body = string.Empty;
				for ( int i = 0; i < bodyLines.Length; i++ )
				{
					body += ( m_masterNodeCategory == AvailableShaderTypes.Template ) ? bodyLines[ i ] : "\t\t" + bodyLines[ i ];
					if ( addNewline )
						body += '\n';

				}
				m_functionsList.Add( new PropertyDataCollector( -1, body ) );

				m_localFunctions.Add( functionId, body );
				m_functions += "\n" + body + "\n";
				m_dirtyFunctions = true;
			}
		}

		public void AddInstructions( string value, bool addTabs = false, bool addLineEnding = false )
		{
			m_instructionsList.Add( new PropertyDataCollector( -1, value ) );
			m_instructions += addTabs ? "\t\t\t" + value : value;
			if ( addLineEnding )
			{
				m_instructions += '\n';
			}
			m_dirtyInstructions = true;
		}


		public void AddInstructions( bool addLineEnding, bool addTabs, params string[] values )
		{
			for ( int i = 0; i < values.Length; i++ )
			{
				m_instructionsList.Add( new PropertyDataCollector( -1, values[ i ] ) );
				m_instructions += addTabs ? "\t\t\t" + values[ i ] : values[ i ];
				if ( addLineEnding )
				{
					m_instructions += '\n';
				}
			}
			m_dirtyInstructions = true;
		}



		public void AddToStartInstructions( string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			m_instructions = value + m_instructions;
			m_dirtyInstructions = true;
		}

		public void ResetInstructions()
		{
			m_instructionsList.Clear();
			m_instructions = string.Empty;
			m_dirtyInstructions = false;
		}


		public void ResetVertexInstructions()
		{
			m_vertexDataList.Clear();
			m_vertexData = string.Empty;
			m_dirtyPerVertexData = false;
		}

		public void AddPropertyNode( PropertyNode node )
		{
			if ( !m_propertyNodes.ContainsKey( node.UniqueId ) )
			{
				m_propertyNodes.Add( node.UniqueId, node );
			}
		}

		public void UpdateMaterialOnPropertyNodes( Material material )
		{
			m_masterNode.UpdateMaterial( material );
			foreach ( KeyValuePair<int, PropertyNode> kvp in m_propertyNodes )
			{
				kvp.Value.UpdateMaterial( material );
			}
		}

		public void AddToVertexInput( string value )
		{
			if ( !m_vertexInputDict.ContainsKey( value ) )
			{
				m_vertexInputDict.Add( value, value );
				m_vertexInputList.Add( value );
			}
		}

		public void AddToInterpolators( string value )
		{
			if ( !m_interpolatorsDict.ContainsKey( value ) )
			{
				m_interpolatorsDict.Add( value, value );
				m_interpolatorsList.Add( value );
			}
		}

		public void AddToVertexInterpolatorsDecl( string value )
		{
			if ( !m_vertexInterpDeclDict.ContainsKey( value ) )
			{
				m_vertexInterpDeclDict.Add( value, value );
				m_vertexInterpDeclList.Add( value );
			}
		}

		public void UpdateShaderOnPropertyNodes( ref Shader shader )
		{
			if ( m_propertyNodes.Count == 0 )
				return;

			try
			{
				bool hasContents = false;
				//string metaNewcontents = IOUtils.LINE_TERMINATOR.ToString();
				TextureDefaultsDataColector defaultCol = new TextureDefaultsDataColector();
				foreach ( KeyValuePair<int, PropertyNode> kvp in m_propertyNodes )
				{
					hasContents = kvp.Value.UpdateShaderDefaults( ref shader, ref defaultCol ) || hasContents;
				}

				if ( hasContents )
				{
					ShaderImporter importer = ( ShaderImporter ) ShaderImporter.GetAtPath( AssetDatabase.GetAssetPath( shader ) );
					importer.SetDefaultTextures( defaultCol.NamesArr, defaultCol.ValuesArr );
					importer.SaveAndReimport();

					defaultCol.Destroy();
					defaultCol = null;
					//string metaFilepath = AssetDatabase.GetTextMetaFilePathFromAssetPath( AssetDatabase.GetAssetPath( shader ) );
					//string metaContents = IOUtils.LoadTextFileFromDisk( metaFilepath );

					//int startIndex = metaContents.IndexOf( IOUtils.MetaBegin );
					//int endIndex = metaContents.IndexOf( IOUtils.MetaEnd );

					//if ( startIndex > 0 && endIndex > 0 )
					//{
					//	startIndex += IOUtils.MetaBegin.Length;
					//	string replace = metaContents.Substring( startIndex, ( endIndex - startIndex ) );
					//	if ( hasContents )
					//	{
					//		metaContents = metaContents.Replace( replace, metaNewcontents );
					//	}
					//}
					//IOUtils.SaveTextfileToDisk( metaContents, metaFilepath, false );
				}
			}
			catch ( Exception e )
			{
				Debug.LogException( e );
			}
		}

		public void Destroy()
		{
			m_masterNode = null;

			m_inputList.Clear();
			m_inputList = null;

			m_customInputList.Clear();
			m_customInputList = null;

			m_propertiesList.Clear();
			m_propertiesList = null;

			m_instancedPropertiesList.Clear();
			m_instancedPropertiesList = null;

			m_uniformsList.Clear();
			m_uniformsList = null;

			m_includesList.Clear();
			m_includesList = null;

			m_tagsList.Clear();
			m_tagsList = null;

			m_pragmasList.Clear();
			m_pragmasList = null;

			m_instructionsList.Clear();
			m_instructionsList = null;

			m_localVariablesList.Clear();
			m_localVariablesList = null;

			m_vertexLocalVariablesList.Clear();
			m_vertexLocalVariablesList = null;

			m_specialLocalVariablesList.Clear();
			m_specialLocalVariablesList = null;

			m_vertexDataList.Clear();
			m_vertexDataList = null;

			m_customOutputList.Clear();
			m_customOutputList = null;

			m_functionsList.Clear();
			m_functionsList = null;

			m_grabPassList.Clear();
			m_grabPassList = null;

			m_propertyNodes.Clear();
			m_propertyNodes = null;

			m_inputDict.Clear();
			m_inputDict = null;

			m_customInputDict.Clear();
			m_customInputDict = null;

			m_propertiesDict.Clear();
			m_propertiesDict = null;

			m_instancedPropertiesDict.Clear();
			m_instancedPropertiesDict = null;

			m_uniformsDict.Clear();
			m_uniformsDict = null;

			m_includesDict.Clear();
			m_includesDict = null;

			m_tagsDict.Clear();
			m_tagsDict = null;

			m_pragmasDict.Clear();
			m_pragmasDict = null;

			m_virtualCoordinatesDict.Clear();
			m_virtualCoordinatesDict = null;

			m_virtualVariablesDict.Clear();
			m_virtualVariablesDict = null;

			m_localVariablesDict.Clear();
			m_localVariablesDict = null;

			m_specialLocalVariablesDict.Clear();
			m_specialLocalVariablesDict = null;

			m_vertexLocalVariablesDict.Clear();
			m_vertexLocalVariablesDict = null;

			m_localFunctions.Clear();
			m_localFunctions = null;

			m_vertexDataDict.Clear();
			m_vertexDataDict = null;

			m_customOutputDict.Clear();
			m_customOutputDict = null;

			//templates
			m_vertexInputList.Clear();
			m_vertexInputList = null;

			m_vertexInputDict.Clear();
			m_vertexInputDict = null;

			m_interpolatorsList.Clear();
			m_interpolatorsList = null;

			m_interpolatorsDict.Clear();
			m_interpolatorsDict = null;

			m_vertexInterpDeclList.Clear();
			m_vertexInterpDeclList = null;

			m_vertexInterpDeclDict.Clear();
			m_vertexInterpDeclDict = null;

			m_templateDataCollector.Destroy();
			m_templateDataCollector = null;
		}

		public string Inputs { get { return m_input; } }
		public string CustomInput { get { return m_customInput; } }
		public string Properties { get { return m_properties; } }
		public string InstancedProperties { get { return m_instancedProperties; } }
		public string Uniforms { get { return m_uniforms; } }
		public string Instructions { get { return m_instructions; } }
		public string Includes { get { return m_includes; } }
		public string Pragmas { get { return m_pragmas; } }
		public string LocalVariables { get { return m_localVariables; } }
		public string SpecialLocalVariables { get { return m_specialLocalVariables; } }
		public string VertexLocalVariables { get { return m_vertexLocalVariables; } }
		public string VertexData { get { return m_vertexData; } }
		public string CustomOutput { get { return m_customOutput; } }
		public string Functions { get { return m_functions; } }
		public string GrabPass { get { return m_grabPass; } }
		public bool DirtyInstructions { get { return m_dirtyInstructions; } }
		public bool DirtyUniforms { get { return m_dirtyUniforms; } }
		public bool DirtyProperties { get { return m_dirtyProperties; } }
		public bool DirtyInstancedProperties { get { return m_dirtyInstancedProperties; } }
		public bool DirtyInputs { get { return m_dirtyInputs; } }
		public bool DirtyCustomInput { get { return m_dirtyCustomInputs; } }
		public bool DirtyIncludes { get { return m_dirtyIncludes; } }
		public bool DirtyPragmas { get { return m_dirtyPragmas; } }
		public bool DirtyLocalVariables { get { return m_dirtyLocalVariables; } }
		public bool DirtyVertexVariables { get { return m_dirtyVertexLocalVariables; } }
		public bool DirtySpecialLocalVariables { get { return m_dirtySpecialLocalVariables; } }
		public bool DirtyPerVertexData { get { return m_dirtyPerVertexData; } }
		public bool DirtyFunctions { get { return m_dirtyFunctions; } }
		public bool DirtyGrabPass { get { return m_grabPassIsDirty; } }
		public int LocalVariablesAmount { get { return m_localVariablesDict.Count; } }
		public int SpecialLocalVariablesAmount { get { return m_specialLocalVariablesDict.Count; } }
		public int VertexLocalVariablesAmount { get { return m_vertexLocalVariablesDict.Count; } }
		public int AvailableUvIndex { get { return m_availableUvInd++; } }
		public bool TesselationActive { set { m_tesselationActive = value; } get { return m_tesselationActive; } }


		public int AvailableVertexTempId { get { return m_availableVertexTempId++; } }
		public int AvailableFragTempId { get { return m_availableFragTempId++; } }

		/// <summary>
		/// Returns true if Normal output is being written by something else
		/// </summary>
		public bool DirtyNormal
		{
			get { return m_dirtyNormal; }
			set { m_dirtyNormal = value; }
		}

		public bool IsFragmentCategory
		{
			get { return m_portCategory == MasterNodePortCategory.Fragment || m_portCategory == MasterNodePortCategory.Debug; }
		}

		public MasterNodePortCategory PortCategory
		{
			get { return m_portCategory; }
			set { m_portCategory = value; }
		}

		public bool IsTemplate { get { return m_masterNodeCategory == AvailableShaderTypes.Template; } }
		public AvailableShaderTypes MasterNodeCategory
		{
			get { return m_masterNodeCategory; }
			set { m_masterNodeCategory = value; }
		}

		/// <summary>
		/// Forces write to Normal output when the output is not connected
		/// </summary>
		public bool ForceNormal
		{
			get { return m_forceNormal; }
			set
			{
				if ( value )
				{
					if ( !m_forceNormalIsDirty )
					{
						m_forceNormal = value;
						m_forceNormalIsDirty = value;
					}
				}
				else
				{
					m_forceNormal = value;
				}
			}
		}

		public bool UsingInternalData
		{
			get { return m_usingInternalData; }
			set { m_usingInternalData = value; }
		}

		public bool UsingWorldNormal
		{
			get { return m_usingWorldNormal; }
			set { m_usingWorldNormal = value; }
		}

		public bool UsingWorldReflection
		{
			get { return m_usingWorldReflection; }
			set { m_usingWorldReflection = value; }
		}

		public bool UsingWorldPosition
		{
			get { return m_usingWorldPosition; }
			set { m_usingWorldPosition = value; }
		}

		public bool UsingViewDirection
		{
			get { return m_usingViewDirection; }
			set { m_usingViewDirection = value; }
		}

		public bool UsingTexcoord0
		{
			get { return m_usingTexcoord0; }
			set { m_usingTexcoord0 = value; }
		}

		public bool UsingTexcoord1
		{
			get { return m_usingTexcoord1; }
			set { m_usingTexcoord1 = value; }
		}

		public bool UsingTexcoord2
		{
			get { return m_usingTexcoord2; }
			set { m_usingTexcoord2 = value; }
		}

		public bool UsingTexcoord3
		{
			get { return m_usingTexcoord3; }
			set { m_usingTexcoord3 = value; }
		}

		public bool UsingCustomOutput
		{
			get { return m_usingCustomOutput; }
			set { m_usingCustomOutput = value; }
		}

		public bool UsingHigherSizeTexcoords
		{
			get { return m_usingHigherSizeTexcoords; }
			set { m_usingHigherSizeTexcoords = value; }
		}

		public bool UsingLightAttenuation
		{
			get { return m_usingLightAttenuation; }
			set { m_usingLightAttenuation = value; }
		}

		public bool UsingArrayDerivatives
		{
			get { return m_usingArrayDerivatives; }
			set { m_usingArrayDerivatives = value; }
		}

		public List<PropertyDataCollector> InputList { get { return m_inputList; } }
		public List<PropertyDataCollector> CustomInputList { get { return m_customInputList; } }
		public List<PropertyDataCollector> PropertiesList { get { return m_propertiesList; } }
		public List<PropertyDataCollector> InstancedPropertiesList { get { return m_instancedPropertiesList; } }
		public List<PropertyDataCollector> UniformsList { get { return m_uniformsList; } }
		public List<PropertyDataCollector> IncludesList { get { return m_includesList; } }
		public List<PropertyDataCollector> TagsList { get { return m_tagsList; } }
		public List<PropertyDataCollector> PragmasList { get { return m_pragmasList; } }
		public List<PropertyDataCollector> InstructionsList { get { return m_instructionsList; } }
		public List<PropertyDataCollector> LocalVariablesList { get { return m_localVariablesList; } }
		public List<PropertyDataCollector> VertexLocalVariablesList { get { return m_vertexLocalVariablesList; } }
		public List<PropertyDataCollector> SpecialLocalVariablesList { get { return m_specialLocalVariablesList; } }
		public List<PropertyDataCollector> VertexDataList { get { return m_vertexDataList; } }
		public List<PropertyDataCollector> CustomOutputList { get { return m_customOutputList; } }
		public List<PropertyDataCollector> FunctionsList { get { return m_functionsList; } }
		public List<PropertyDataCollector> GrabPassList { get { return m_grabPassList; } }
		//Templates
		public List<string> VertexInputList { get { return m_vertexInputList; } }
		public List<string> InterpolatorList { get { return m_interpolatorsList; } }
		public List<string> VertexInterpDeclList { get { return m_vertexInterpDeclList; } }
		public TemplateDataCollector TemplateDataCollectorInstance { get { return m_templateDataCollector; } }
		public RenderPath CurrentRenderPath
		{
			get { return m_renderPath; }
			set { m_renderPath = value; }
		}
	}
}
