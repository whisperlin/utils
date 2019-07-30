// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Blend Normals", "Textures", "Blend Normals" )]
	public class BlendNormalsNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal A" );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal B" );
			AddOutputPort( WirePortDataType.FLOAT3, "XYZ" );
			m_useInternalPortData = true;
			m_previewShaderGUID = "bcdf750ff5f70444f98b8a3efa50dc6f";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			dataCollector.AddToIncludes( UniqueId, Constants.UnityStandardUtilsLibFuncs );
			string _inputA = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar, true );
			string _inputB = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar, true );
			string result = "BlendNormals( " + _inputA + " , " + _inputB + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
