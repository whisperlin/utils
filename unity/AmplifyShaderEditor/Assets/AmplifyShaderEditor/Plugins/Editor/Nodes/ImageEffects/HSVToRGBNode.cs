using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "HSV to RGB", "Image Effects", "Converts from HSV to RGB color space" )]
	public class HSVToRGBNode : ParentNode
	{
		private const string HSVToRGBHeader = "HSVToRGB( {0}3({1},{2},{3}) )";
		private readonly string[] HSVToRGBFunction = { "{0}3 HSVToRGB( {0}3 c )\n",
														"{\n",
														"\t{0}4 K = {0}4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );\n",
														"\t{0}3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );\n",
														"\treturn c.z * lerp( K.xxx, clamp( p - K.xxx, 0.0, 1.0 ), c.y );\n",
														"}\n"};
		private readonly bool[] HSVToRGBFlags = {   true,
													false,
													true,
													true,
													false,
													false};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "Hue" );
			AddInputPort( WirePortDataType.FLOAT, false, "Saturation" );
			AddInputPort( WirePortDataType.FLOAT, false, "Value" );
			AddOutputColorPorts( Constants.EmptyPortValue, false );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			DrawPrecisionProperty();
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );

			string precisionString = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT );
			if ( !dataCollector.HasFunction( HSVToRGBHeader ) )
			{
				//Hack to be used util indent is properly used
				int currIndent = UIUtils.ShaderIndentLevel;
				if ( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
				{
					UIUtils.ShaderIndentLevel = 0;
				}
				else
				{
					UIUtils.ShaderIndentLevel = 1;
					UIUtils.ShaderIndentLevel++;
				}
				
				string finalFunction = string.Empty;
				for ( int i = 0; i < HSVToRGBFunction.Length; i++ )
				{
					finalFunction += UIUtils.ShaderIndentTabs + ( HSVToRGBFlags[ i ] ? string.Format( HSVToRGBFunction[ i ], precisionString ) : HSVToRGBFunction[ i ] );
				}
				
				UIUtils.ShaderIndentLevel = currIndent;

				dataCollector.AddFunction( HSVToRGBHeader, finalFunction );
			}

			string hue = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string saturation = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			string value = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );

			RegisterLocalVariable( 0, string.Format( HSVToRGBHeader, precisionString, hue, saturation, value ), ref dataCollector, "hsvTorgb" + OutputId );
			return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
		}
	}
}
