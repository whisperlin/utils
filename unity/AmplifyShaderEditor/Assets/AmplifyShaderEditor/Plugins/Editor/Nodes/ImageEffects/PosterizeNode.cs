// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

//https://www.shadertoy.com/view/ldX3D4
using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Posterize", "Image Effects", "Converts a continuous gradation of tones to multiple regions of fewer tones" )]
	public sealed class PosterizeNode : ParentNode
	{
		private const string PosterizationPowerStr = "Power";
		[SerializeField]
		private int m_posterizationPower = 1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.INT, false, "Posterization Power [1-256]" );
			AddInputPort( WirePortDataType.COLOR, false, "Color" );
			AddOutputPort( WirePortDataType.COLOR, Constants.EmptyPortValue );
			m_textLabelWidth = 60;
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			{
				m_posterizationPower = EditorGUILayoutIntSlider( PosterizationPowerStr, m_posterizationPower, 1, 256 );
			}
			EditorGUILayout.EndVertical();
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			string posterizationPower = "1";
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				posterizationPower = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			}
			else
			{
				posterizationPower = m_posterizationPower.ToString();
			}

			string colorTarget = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			
			string divVar = "div" + OutputId;
			dataCollector.AddLocalVariable( UniqueId, "float " + divVar + "=256.0/float(" + posterizationPower + ");" );
			string result = "( floor( " + colorTarget + " * " + divVar + " ) / " + divVar + " )";

			RegisterLocalVariable( 0, result, ref dataCollector, "posterize" + OutputId );

			return m_outputPorts[ 0 ].LocalValue;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_posterizationPower );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_posterizationPower = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
		}
	}
}
