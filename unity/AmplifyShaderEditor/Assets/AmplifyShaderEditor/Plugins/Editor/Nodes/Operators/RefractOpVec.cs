// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Refract", "Vector Operators", "Computes a refraction vector" )]
	public sealed class RefractOpVec : DynamicTypeNode
	{
		override protected void AddPorts()
		{
			base.AddPorts();
			m_inputPorts[ 0 ].Name = "Incident";
			m_inputPorts[ 1 ].Name = "Normal";
			AddInputPort( WirePortDataType.FLOAT, true, "Eta" );
			m_textLabelWidth = 67;
			m_previewShaderGUID = "5ab44ca484bed8b4884b03b1c00fdc3d";
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;
			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			string interp = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 2 ].DataType, ignoreLocalvar );
			string result = "refract( " + m_inputA + " , " + m_inputB + " , " + interp + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
