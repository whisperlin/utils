// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "LOD Fade", "Miscellaneous", "LODFadeNode" )]
	public sealed class LODFadeNode : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "Fade[0...1]" );
			ChangeOutputName( 2, "Fade[16Lvl]" );
			ChangeOutputName( 3, "Unused" );
			ChangeOutputName( 4, "Unused" );
			m_value = "unity_LODFade";
		}
	}
}
