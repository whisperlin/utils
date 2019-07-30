// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Fog Params", "Light", "Parameters for fog calculation" )]
	public sealed class FogParamsNode : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "Exp2" );
			ChangeOutputName( 2, "Exp" );
			ChangeOutputName( 3, "Linear mode" );
			ChangeOutputName( 4, "Linear mode" );
			m_value = "unity_FogParams";
		}
	}
}
