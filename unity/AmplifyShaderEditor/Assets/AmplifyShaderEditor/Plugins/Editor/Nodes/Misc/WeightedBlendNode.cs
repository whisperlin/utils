// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;
using UnityEditor;
	
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Weighted Blend", "Miscellaneous", "Mix all channels through weighted average sum", null, KeyCode.None, true )]
	public sealed class WeightedBlendNode : WeightedAvgNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputData = new string[ 6 ];
		}
		
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			GetInputData( ref dataCollector, ignoreLocalvar );

			string result = string.Empty;
			string avgSum = string.Empty;

			string localVarName = "weightedBlendVar" + OutputId;
			dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ 0 ].DataType, localVarName, m_inputData[ 0 ] );
			
			if ( m_activeCount  < 2 )
			{
				return CreateOutputLocalVariable( 0, m_inputData[ 0 ], ref dataCollector );
			}
			else
			{
				for ( int i = 0; i < m_activeCount; i++ )
				{
					result += localVarName + Constants.VectorSuffixes[ i ] + "*" + m_inputData[ i + 1 ];
					avgSum += localVarName + Constants.VectorSuffixes[ i ];
					if ( i != ( m_activeCount - 1 ) )
					{
						result += " + ";
						avgSum += " + ";
					}
				}
			}

			result = UIUtils.AddBrackets( result ) + "/" + UIUtils.AddBrackets( avgSum );
			result = UIUtils.AddBrackets( result );
			RegisterLocalVariable( 0, result, ref dataCollector, "weightedAvg" + OutputId );
			return m_outputPorts[ 0 ].LocalValue;
		}
	}
}
