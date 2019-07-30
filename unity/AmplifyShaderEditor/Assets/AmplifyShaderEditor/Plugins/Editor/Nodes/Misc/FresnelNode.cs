// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
// http://kylehalladay.com/blog/tutorial/2014/02/18/Fresnel-Shaders-From-The-Ground-Up.html
// http://http.developer.nvidia.com/CgTutorial/cg_tutorial_chapter07.html

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Fresnel", "Surface Data", "Simple Fresnel effect" )]
	public sealed class FresnelNode : ParentNode
	{
		private const string WorldDirVarStr = "worldViewDir";
		private const string WorldDirFuncStr = "normalize( UnityWorldSpaceViewDir( {0} ) )";

		private const string FresnedDotVar = "fresnelDotVal";
		private const string FresnedFinalVar = "fresnelFinalVal";

		private const string FresnesDotOp = "float {0} = dot( {1},{2} );";
		private const string FresnesFinalOp = "float {0} = {1};";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "World Normal" );
			AddInputPort( WirePortDataType.FLOAT, false, "Bias" );
			AddInputPort( WirePortDataType.FLOAT, false, "Scale" );
			AddInputPort( WirePortDataType.FLOAT, false, "Power" );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_useInternalPortData = true;
			m_drawPreviewAsSphere = true;
			m_inputPorts[ 2 ].FloatInternalData = 1;
			m_inputPorts[ 3 ].FloatInternalData = 5;
			m_previewShaderGUID = "240145eb70cf79f428015012559f4e7d";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_inputPorts[ 0 ].IsConnected )
				m_previewMaterialPassId = 1;
			else
				m_previewMaterialPassId = 0;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_POS ), true );
			if ( dataCollector.IsFragmentCategory )
			{
				string worldPosVar = dataCollector.IsTemplate ? dataCollector.TemplateDataCollectorInstance.GetWorldPos() : Constants.InputVarStr + ".worldPos";
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, WorldDirVarStr, string.Format( WorldDirFuncStr, worldPosVar ) );

			}
			else
			{
				string worldPosVar = dataCollector.IsTemplate ? dataCollector.TemplateDataCollectorInstance.GetWorldPos() : GeneratorUtils.GenerateWorldPosition( ref dataCollector, UniqueId ); 
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, WorldDirVarStr, string.Format( WorldDirFuncStr, worldPosVar ) );
			}

			string normal = string.Empty;
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				normal = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar, true );
			}
			else
			{
				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
				dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
				normal = dataCollector.IsTemplate ? dataCollector.TemplateDataCollectorInstance.GetWorldNormal() : GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
				//string normalWorld = "WorldNormalVector( " + Constants.InputVarStr + ", float3( 0, 0, 1 ) );";
				//dataCollector.AddToLocalVariables( m_uniqueId, "float3 worldNormal = "+ normalWorld );
				//normal = "worldNormal";
				//dataCollector.ForceNormal = true;
			}

			string bias = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalvar, true );
			string scale = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalvar, true );
			string power = m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalvar, true );

			string ndotl = "dot( " + normal + ", " + WorldDirVarStr + " )";

			string fresnelFinalVar = FresnedFinalVar + OutputId;
			string result = string.Format( "({0} + {1}*pow( 1.0 - {2} , {3}))", bias, scale, ndotl, power );

			RegisterLocalVariable( 0, result, ref dataCollector, fresnelFinalVar );
			return m_outputPorts[ 0 ].LocalValue;
		}
	}
}
