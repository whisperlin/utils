// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

//https://docs.unity3d.com/Manual/SL-BuiltinFunctions.html

using System;
using UnityEngine;
namespace AmplifyShaderEditor
{
	[Serializable]
	public class HelperParentNode : ParentNode
	{
		[SerializeField]
		protected string m_funcType = string.Empty;

		protected string m_localVarName = null;
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
		}

		public override string GetIncludes()
		{
			return Constants.UnityCgLibFuncs;
		}
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[0].IsLocalValue )
				return m_outputPorts[0].LocalValue;

			dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );
			string concatResults = string.Empty;
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				string result = string.Empty;
				if ( m_inputPorts[ i ].IsConnected )
				{
					result = m_inputPorts[ i ].GeneratePortInstructions( ref dataCollector );
				}
				else
				{
					result = m_inputPorts[ i ].WrappedInternalData;
				}

				concatResults += result;
				if ( i != ( m_inputPorts.Count - 1 ) )
					concatResults += " , ";
			}
			string finalResult = m_funcType + "( " + concatResults + " )";

			RegisterLocalVariable( 0, finalResult, ref dataCollector , m_localVarName );

			return m_outputPorts[ 0 ].LocalValue;
		}
	}
}
