using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Camera Depth Fade", "Camera And Screen", "Outputs a 0 - 1 gradient representing the distance between the surface of this object and camera near plane" )]
	public sealed class CameraDepthFade : ParentNode
	{
		//{0} - Eye Depth
		//{1} - Offset
		//{2} - Distance
		private const string CameraDepthFadeFormat = "(( {0} -_ProjectionParams.y - {1} ) / {2})";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "Length" );
			AddInputPort( WirePortDataType.FLOAT, false, "Offset" );
			m_inputPorts[ 0 ].FloatInternalData = 1;
			//m_inputPorts[ 0 ].InternalDataName = "Distance";
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_useInternalPortData = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string distance = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string offset = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );

			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			string value = string.Empty;

			if ( dataCollector.IsTemplate )
			{
				value = string.Format( CameraDepthFadeFormat, dataCollector.TemplateDataCollectorInstance.GetEyeDepth(), offset, distance );
				RegisterLocalVariable( 0, value, ref dataCollector, "cameraDepthFade" + OutputId );
				return m_outputPorts[ 0 ].LocalValue;
			}
			
			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				
				//dataCollector.AddVertexInstruction( "float cameraDepthFade" + UniqueId + " = (( -UnityObjectToViewPos( " + Constants.VertexShaderInputStr + ".vertex.xyz ).z -_ProjectionParams.y - " + offset + " ) / " + distance + ");", UniqueId );
				value = string.Format( CameraDepthFadeFormat,  "-UnityObjectToViewPos( " + Constants.VertexShaderInputStr + ".vertex.xyz ).z", offset, distance );
				RegisterLocalVariable( 0, value, ref dataCollector, "cameraDepthFade" + OutputId );
				return m_outputPorts[ 0 ].LocalValue;
			}

			dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );
			dataCollector.AddToInput( UniqueId, "float eyeDepth", true );

			string instruction = "-UnityObjectToViewPos( " + Constants.VertexShaderInputStr + ".vertex.xyz ).z";
			dataCollector.AddVertexInstruction( Constants.VertexShaderOutputStr + ".eyeDepth = " + instruction, UniqueId );

			value = string.Format( CameraDepthFadeFormat, Constants.InputVarStr + ".eyeDepth", offset, distance );
			RegisterLocalVariable( 0, value, ref dataCollector, "cameraDepthFade" + OutputId );
			//dataCollector.AddToLocalVariables( UniqueId, "float cameraDepthFade" + UniqueId + " = (( " + Constants.InputVarStr + ".eyeDepth -_ProjectionParams.y - "+ offset + " ) / " + distance + ");" );

			return m_outputPorts[0].LocalValue;
		}
	}
}
