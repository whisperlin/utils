// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Static Switch", "Logical Operators", "Creates a shader keyword toggle", Available = true )]
	public sealed class StaticSwitch : PropertyNode
	{
		[SerializeField]
		private bool m_defaultValue = false;

		[SerializeField]
		private int m_multiCompile = 0;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			AddInputPort( WirePortDataType.FLOAT, false, "True" );
			AddInputPort( WirePortDataType.FLOAT, false, "False" );
			m_headerColor = new Color( 0.0f, 0.55f, 0.45f, 1f );
			m_customPrefix = "Keyword ";
			m_autoWrapProperties = false;
			m_freeType = false;
			m_currentParameterType = PropertyType.Property;
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			UIUtils.RegisterPropertyNode( this );
		}

		public override void Destroy()
		{
			base.Destroy();
			UIUtils.UnregisterPropertyNode( this );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnections();
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnections();
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			UpdateConnections();
		}

		private void UpdateConnections()
		{
			WirePortDataType mainType = WirePortDataType.FLOAT;
			WirePortDataType portOneType = m_inputPorts[ 0 ].IsConnected ? m_inputPorts[ 0 ].GetOutputConnection().DataType : WirePortDataType.FLOAT;
			WirePortDataType portTwoType = m_inputPorts[ 1 ].IsConnected ? m_inputPorts[ 1 ].GetOutputConnection().DataType : WirePortDataType.FLOAT;

			int highest = UIUtils.GetPriority( mainType );
			if ( UIUtils.GetPriority( portOneType ) > highest )
			{
				mainType = portOneType;
				highest = UIUtils.GetPriority( portOneType );
			}

			if ( UIUtils.GetPriority( portTwoType ) > highest )
			{
				mainType = portTwoType;
				highest = UIUtils.GetPriority( portTwoType );
			}

			m_inputPorts[ 0 ].ChangeType( mainType, false );
			m_inputPorts[ 1 ].ChangeType( mainType, false );
			m_outputPorts[ 0 ].ChangeType( mainType, false );
		}

		public override string GetPropertyValue()
		{
			return "[Toggle]" + m_propertyName + "(\"" + m_propertyInspectorName + "\", Int) = " + ( m_defaultValue ? 1 : 0 );
		}

		public override string PropertyName
		{
			get
			{
				return base.PropertyName.ToUpper();
			}
		}

		public override string GetPropertyValStr()
		{
			return PropertyName + "_ON";
		}

		public override string GetUniformValue()
		{
			return string.Empty;
		}

		public override void DrawProperties()
		{
			//base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, PropertyGroup );
			CheckPropertyFromInspector();
		}
		public readonly static string[] KeywordTypeStr = { "Shader Feature", "Multi Compile" };
		public readonly static int[] KeywordTypeInt = { 0, 1 };
		void PropertyGroup()
		{
			m_multiCompile = EditorGUILayoutIntPopup( "Keyword Type", m_multiCompile, KeywordTypeStr, KeywordTypeInt );
			ShowPropertyInspectorNameGUI();
			ShowPropertyNameGUI( true );
			bool guiEnabledBuffer = GUI.enabled;
			GUI.enabled = false;
			EditorGUILayoutTextField( "Keyword Name", PropertyName + "_ON" );
			GUI.enabled = guiEnabledBuffer;
			m_defaultValue = EditorGUILayoutToggle( "Default Value", m_defaultValue );
			EditorGUILayout.HelpBox("Keyword Type:\n" +
				"The difference is that unused variants of \"Shader Feature\" shaders will not be included into game build while \"Multi Compile\" variants are included regardless of their usage.\n\n" +
				"So \"Shader Feature\" makes most sense for keywords that will be set on the materials, while \"Multi Compile\" for keywords that will be set from code globally.\n\n" +
				"You can set keywords using the material property using the \"Property Name\" or you can set the keyword directly using the \"Keyword Name\".", MessageType.None);
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			string trueCode = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string falseCode = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			if ( m_multiCompile == 1)
				dataCollector.AddToPragmas( UniqueId, "multi_compile __ " + PropertyName + "_ON" );
			else
				dataCollector.AddToPragmas( UniqueId, "shader_feature " + PropertyName + "_ON" );

			string outType = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			dataCollector.AddLocalVariable( UniqueId, "#ifdef " + PropertyName + "_ON", true );
			dataCollector.AddLocalVariable( UniqueId, outType + " staticSwitch" + OutputId + " = " + trueCode + ";", true );
			dataCollector.AddLocalVariable( UniqueId, "#else", true );
			dataCollector.AddLocalVariable( UniqueId, outType + " staticSwitch" + OutputId + " = " + falseCode + ";", true );
			dataCollector.AddLocalVariable( UniqueId, "#endif", true );
			return "staticSwitch" + OutputId;
		}

		public override bool GetUniformData( out string dataType, out string dataName )
		{
			dataType = string.Empty;
			dataName = string.Empty;
			return false;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_multiCompile = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_defaultValue = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_multiCompile );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValue );
		}
	}
}
