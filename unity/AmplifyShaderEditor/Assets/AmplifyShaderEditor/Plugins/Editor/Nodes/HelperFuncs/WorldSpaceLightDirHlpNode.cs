// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Space Light Dir", "Light", "Computes normalized world space light direction" )]
	public sealed class WorldSpaceLightDirHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "UnityWorldSpaceLightDir";
			//m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_inputPorts[ 0 ].Visible = false;
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_outputPorts[ 0 ].Name = "XYZ";
			m_useInternalPortData = false;
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "2e8dc46eb6fb2124d9f0007caf9567e3";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetWorldSpaceLightDir();

			//if ( m_outputPorts[ 0 ].IsLocalValue )
			//	return m_outputPorts[ 0 ].LocalValue;

			dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );
			dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_POS ), true );

			string worldPos = GeneratorUtils.GenerateWorldPosition( ref dataCollector, UniqueId );
			return GeneratorUtils.GenerateWorldLightDirection( ref dataCollector, UniqueId, m_currentPrecisionType, worldPos );

			//dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );

			//string result = string.Empty;
			////if ( m_inputPorts[ 0 ].IsConnected )
			////{
			////	result = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, ignoreLocalvar, 0, true );
			////}
			////else
			////{
			//	string input = UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_POS );
			//	dataCollector.AddToInput( UniqueId, input, true );
			//	result = Constants.InputVarStr + "." + UIUtils.GetInputValueFromType( AvailableSurfaceInputs.WORLD_POS );
			////}
			//result = m_funcType + "( " + result + " )";
			//RegisterLocalVariable( 0, result, ref dataCollector, "worldSpaceLightDir" + OutputId );
			//return m_outputPorts[ 0 ].LocalValue;
		}
	}
}
