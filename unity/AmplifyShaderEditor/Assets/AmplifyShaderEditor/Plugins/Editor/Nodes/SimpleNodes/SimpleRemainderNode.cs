// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Remainder", "Math Operators", "Remainder between two variables" )]
	public sealed class SimpleRemainderNode : ParentNode
	{
		private string m_inputA = string.Empty;
		private string m_inputB = string.Empty;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_useInternalPortData = true;
			m_textLabelWidth = 35;
			AddInputPort( WirePortDataType.INT, false, "A" );
			AddInputPort( WirePortDataType.INT, false, "B" );
			AddOutputPort( WirePortDataType.INT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_previewShaderGUID = "8fdfc429d6b191c4985c9531364c1a95";
		}

		public override string GenerateShaderForOutput( int outputId,  ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			m_inputA = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.INT, ignoreLocalvar, true );
			m_inputB = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.INT, ignoreLocalvar, true );

			string result = string.Empty;
			switch ( m_outputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.INT:
				case WirePortDataType.COLOR:
				case WirePortDataType.OBJECT:
				{
					result =  "( " + m_inputA + " % " + m_inputB + " )";
				}break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					result = UIUtils.InvalidParameter( this );
				}
				break;
			}
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
