// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Reflect", "Vector Operators", "Reflection vector given an incidence vector and a normal vector" )]
	public sealed class ReflectOpNode : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].Name = "Incident";
			m_inputPorts[ 1 ].Name = "Normal";
			m_textLabelWidth = 67;
			m_previewShaderGUID = "fb520f2145c0fa0409320a9e6d720758";
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			string result = "reflect( " + m_inputA + " , " + m_inputB + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
