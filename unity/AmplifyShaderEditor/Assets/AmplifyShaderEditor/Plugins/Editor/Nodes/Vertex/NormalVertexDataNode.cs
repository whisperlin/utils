// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Vertex Normal", "Vertex Data", "Vertex normal vector in object space, can be used in both local vertex offset and fragment outputs" )]
	public sealed class NormalVertexDataNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "normal";
			ChangeOutputProperties( 0, "XYZ", WirePortDataType.FLOAT3 );
			m_outputPorts[ 4 ].Visible = false;
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "6b24b06c33f9fe84c8a2393f13ab5406";
		}
		
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			string vertexNormal = string.Empty;

			if ( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
			{
				vertexNormal = dataCollector.TemplateDataCollectorInstance.GetVertexNormal();
				return GetOutputVectorItem( 0, outputId, vertexNormal );
			}

			if ( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
				if( dataCollector.DirtyNormal )
				{
					dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
					dataCollector.ForceNormal = true;
				}
			}

			vertexNormal = GeneratorUtils.GenerateVertexNormal( ref dataCollector, UniqueId, m_currentPrecisionType );
			return GetOutputVectorItem( 0, outputId, vertexNormal );
		}
	}
}
