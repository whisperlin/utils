// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Standard Surface Light", "Light", "Provides a way to create a standard surface light model in custom lighting mode", NodeAvailabilityFlags = ( int ) NodeAvailability.CustomLighting )]
	public sealed class CustomStandardSurface : ParentNode
	{
		private bool m_specular = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Albedo" );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			m_inputPorts[ 1 ].Vector3InternalData = Vector3.forward;
			AddInputPort( WirePortDataType.FLOAT3, false, "Emission" );
			AddInputPort( WirePortDataType.FLOAT, false, "Metallic" );
			AddInputPort( WirePortDataType.FLOAT, false, "Smoothness" );
			AddInputPort( WirePortDataType.FLOAT, false, "Occlusion" );
			m_inputPorts[ 5 ].FloatInternalData = 1;
			AddOutputPort( WirePortDataType.FLOAT3, "RGB" );
			m_autoWrapProperties = true;
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_errorMessageTooltip = "This node only returns correct information using a custom light model, otherwise returns 0";
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( m_inputPorts[ 0 ].IsConnected )
				dataCollector.DirtyNormal = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_specular = EditorGUILayoutToggle( "Specular", m_specular );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_specular )
					m_inputPorts[ 3 ].ChangeProperties( "Specular", WirePortDataType.FLOAT3, false );
				else
					m_inputPorts[ 3 ].ChangeProperties( "Metallic", WirePortDataType.FLOAT, false );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( ContainerGraph.CurrentCanvasMode == NodeAvailability.TemplateShader || ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting )
				return "float3(0,0,0)";

			string specularMode = string.Empty;
			if( m_specular )
				specularMode = "Specular";

			dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
			dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
			dataCollector.ForceNormal = true;

			dataCollector.AddLocalVariable( UniqueId, "SurfaceOutputStandard" + specularMode + " s" + OutputId + " = (SurfaceOutputStandard" + specularMode + " ) 0;" );
			dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Albedo = " + m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) + ";" );
			dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Normal = WorldNormalVector( i, " + m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector ) + ");" );
			dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Emission = " + m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector ) + ";" );
			if( m_specular )
				dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Specular = " + m_inputPorts[ 3 ].GeneratePortInstructions( ref dataCollector ) + ";" );
			else
				dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Metallic = " + m_inputPorts[ 3 ].GeneratePortInstructions( ref dataCollector ) + ";" );
			dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Smoothness = " + m_inputPorts[ 4 ].GeneratePortInstructions( ref dataCollector ) + ";" );
			dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Occlusion = " + m_inputPorts[ 5 ].GeneratePortInstructions( ref dataCollector ) + ";\n" );

			dataCollector.AddLocalVariable( UniqueId, "gi.light.ndotl = LambertTerm( s" + OutputId + ".Normal, gi.light.dir );" );
			dataCollector.AddLocalVariable( UniqueId, "data.light = gi.light;\n", true );

			dataCollector.AddLocalVariable( UniqueId, "UnityGI gi" + OutputId + " = gi;" );
			dataCollector.AddLocalVariable( UniqueId, "#ifdef UNITY_PASS_FORWARDBASE", true );
			dataCollector.AddLocalVariable( UniqueId, "Unity_GlossyEnvironmentData g" + OutputId + ";" );
			dataCollector.AddLocalVariable( UniqueId, "g" + OutputId + ".roughness = 1 - s" + OutputId + ".Smoothness;" );
			dataCollector.AddLocalVariable( UniqueId, "g" + OutputId + ".reflUVW = reflect( -data.worldViewDir, s" + OutputId + ".Normal );" );
			dataCollector.AddLocalVariable( UniqueId, "gi" + OutputId + " = UnityGlobalIllumination( data, s" + OutputId + ".Occlusion, s" + OutputId + ".Normal, g" + OutputId + " );" );
			dataCollector.AddLocalVariable( UniqueId, "#endif\n", true );
			dataCollector.AddLocalVariable( UniqueId, "float3 surfResult" + OutputId + " = LightingStandard" + specularMode + " ( s" + OutputId + ", viewDir, gi" + OutputId + " ).rgb;" );
			dataCollector.AddLocalVariable( UniqueId, "surfResult" + OutputId + " += s" + OutputId + ".Emission;\n" );
			return "surfResult" + OutputId;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if( ContainerGraph.CurrentCanvasMode == NodeAvailability.TemplateShader || ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting ) )
				m_showErrorMessage = true;
			else
				m_showErrorMessage = false;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_specular = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_specular );
		}
	}
}
