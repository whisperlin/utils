// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Light Attenuation", "Light", "Contains light attenuation for all types of light", NodeAvailabilityFlags = ( int ) NodeAvailability.CustomLighting )]
	public sealed class LightAttenuation : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_errorMessageTooltip = "This node only returns correct information using a custom light model, otherwise returns 1";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
            if ( ContainerGraph.CurrentCanvasMode == NodeAvailability.TemplateShader || ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting )
                return "1";

			dataCollector.UsingLightAttenuation = true;
			return "ase_lightAtten";
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
            if ( ContainerGraph.CurrentCanvasMode == NodeAvailability.TemplateShader || ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting ) )
                m_showErrorMessage = true;
			else
				m_showErrorMessage = false;
		}
	}
}
