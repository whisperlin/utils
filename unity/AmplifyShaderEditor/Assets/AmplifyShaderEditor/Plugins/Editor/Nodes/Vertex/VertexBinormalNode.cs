// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Vertex Binormal World
// Donated by Community Member Kebrus

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Bitangent", "Surface Data", "Per pixel world bitangent vector" )]
	public sealed class VertexBinormalNode : ParentNode
	{
		//private const string WorldBiTangentDefFrag = "WorldNormalVector( {0}, float3(0,1,0) )";
		//private const string WorldBiTangentDefVert = "UnityObjectToWorldDir( {0}.tangent.xyz )";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "76873532ab67d2947beaf07151383cbe";
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			dataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.IsTemplate )
				return GetOutputVectorItem( 0, outputId, dataCollector.TemplateDataCollectorInstance.GetWorldBinormal() );

			dataCollector.ForceNormal = true;

			dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
			dataCollector.AddToInput( UniqueId, Constants.InternalData, false );

			string worldBitangent = GeneratorUtils.GenerateWorldBitangent( ref dataCollector, UniqueId );

			return GetOutputVectorItem( 0, outputId, worldBitangent );
		}
	}
}
