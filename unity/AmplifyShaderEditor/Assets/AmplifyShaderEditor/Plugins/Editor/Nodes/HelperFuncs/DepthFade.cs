using UnityEngine;
using UnityEditor;

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Depth Fade", "Surface Data", "Outputs a 0 - 1 gradient representing the distance between the surface of this object and geometry behind" )]
	public sealed class DepthFade : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "Dist" );
			m_inputPorts[ 0 ].FloatInternalData = 1;
			m_inputPorts[ 0 ].InternalDataName = "Distance";
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_useInternalPortData = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				UIUtils.ShowNoVertexModeNodeMessage( this );
				return "0";
			}

			dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );
			dataCollector.AddToUniforms( UniqueId, "uniform sampler2D _CameraDepthTexture;" );

			string screenPos = string.Empty;
			if ( dataCollector.IsTemplate )
			{
				screenPos = dataCollector.TemplateDataCollectorInstance.GetScreenPos();
			}
			else
			{
                screenPos = GeneratorUtils.GenerateScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, true );
			}
			
			string screenDepth = "LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(" + screenPos + "))))";
			string distance = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );

			dataCollector.AddToLocalVariables( UniqueId, "float screenDepth" + OutputId + " = " + screenDepth + ";" );
			dataCollector.AddToLocalVariables( UniqueId, "float distanceDepth" + OutputId + " = abs( ( screenDepth" + OutputId + " - LinearEyeDepth( " + screenPos + ".z/ " + screenPos + ".w ) ) / ( " + distance + " ) );" );
			return "distanceDepth" + OutputId;
		}
	}
}
