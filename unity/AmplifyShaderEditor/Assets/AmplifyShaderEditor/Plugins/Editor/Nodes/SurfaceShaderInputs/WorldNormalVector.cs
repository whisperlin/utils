// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Normal", "Surface Data", "Per pixel world normal vector, accepts a <b>Normal</b> vector in tangent space (ie: normalmap)" )]
	public sealed class WorldNormalVector : ParentNode
	{
		private const string NormalVecValStr = "newWorldNormal";
		private const string NormalVecDecStr = "float3 {0} = {1};";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			//m_inputPorts[ 0 ].InternalData Vector3InternalData = UnityEngine.Vector3.forward;
			//UIUtils.AddNormalDependentCount();
			m_previewShaderGUID = "5f55f4841abb61e45967957788593a9d";
			m_drawPreviewAsSphere = true;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_inputPorts[ 0 ].IsConnected )
				m_previewMaterialPassId = 1;
			else
				m_previewMaterialPassId = 0;
		}

		//public override void Destroy()
		//{
		//	ContainerGraph.RemoveNormalDependentCount();
		//	base.Destroy();
		//}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData , ref dataCollector );
			if ( m_inputPorts[ 0 ].IsConnected )
				dataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.IsTemplate )
			{
				if ( m_inputPorts[ 0 ].IsConnected )
				{
					if ( m_outputPorts[ 0 ].IsLocalValue )
						return m_outputPorts[ 0 ].LocalValue;


					string value = dataCollector.TemplateDataCollectorInstance.GetWorldNormal( m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) );
					RegisterLocalVariable( 0, value, ref dataCollector, "worldNormal" + OutputId );
					return m_outputPorts[ 0 ].LocalValue;
				}
				else
				{
					return GetOutputVectorItem( 0, outputId, dataCollector.TemplateDataCollectorInstance.GetWorldNormal());
				}
			}

			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
				//dataCollector.AddToInput( m_uniqueId, Constants.InternalData, false );
				string result = string.Empty;
				if ( m_inputPorts[ 0 ].IsConnected )
				{
					dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
					dataCollector.ForceNormal = true;

					result = "WorldNormalVector( " + Constants.InputVarStr + " , " + m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar ) + " )";

					if ( m_outputPorts[ 0 ].ConnectionCount > 1 )
					{
						dataCollector.AddToLocalVariables( UniqueId, string.Format( NormalVecDecStr, NormalVecValStr + UniqueId, result ) );
						return GetOutputVectorItem( 0, outputId, NormalVecValStr + UniqueId );
					}
				}
				else
				{
					if ( !dataCollector.DirtyNormal )
					{
						result = Constants.InputVarStr+".worldNormal";
					}
					else
					{
						dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
						result = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
					}
				}

				return GetOutputVectorItem( 0, outputId, result );
			}
			else
			{
				if ( m_inputPorts[ 0 ].IsConnected )
				{
					string inputTangent = m_inputPorts[ 0 ].GeneratePortInstructions(ref dataCollector);

					dataCollector.AddToVertexLocalVariables(UniqueId, "float3 normalWorld = UnityObjectToWorldNormal( "+ Constants.VertexShaderInputStr + ".normal );" );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float4 tangentWorld = float4( UnityObjectToWorldDir( " + Constants.VertexShaderInputStr + ".tangent.xyz ), " + Constants.VertexShaderInputStr + ".tangent.w );" );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3x3 tangentToWorld = CreateTangentToWorldPerVertex( normalWorld, tangentWorld.xyz, tangentWorld.w );" );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3 tangentNormal" + OutputId + " = " + inputTangent+";" );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3 modWorldtangent" + OutputId + " = (tangentToWorld[0] * tangentNormal" + OutputId + ".x + tangentToWorld[1] * tangentNormal" + OutputId + ".y + tangentToWorld[2] * tangentNormal" + OutputId + ".z);" );
					return GetOutputVectorItem( 0, outputId, "modWorldtangent" + OutputId );
				}
				else
				{
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3 normalWorld = UnityObjectToWorldNormal( " + Constants.VertexShaderInputStr + ".normal );" );
					return GetOutputVectorItem( 0, outputId, "normalWorld" );
					//if ( m_outputPorts[ 0 ].IsLocalValue )
					//	return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );

					//RegisterLocalVariable( 0, string.Format( "UnityObjectToWorldNormal( {0}.normal )", Constants.VertexShaderInputStr ), ref dataCollector, "normalWorld" );

					//return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
				}
				//half3 worldNormal = UnityObjectToWorldNormal( v.normal );
			}
		}
	}
}
