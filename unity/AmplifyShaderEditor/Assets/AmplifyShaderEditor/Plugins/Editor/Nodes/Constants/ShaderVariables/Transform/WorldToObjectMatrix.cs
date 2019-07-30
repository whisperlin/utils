// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "World To Object Matrix", "Matrix Transform", "Inverse of current world matrix" )]
	public sealed class WorldToObjectMatrix : ConstantShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "Out", WirePortDataType.FLOAT4x4 );
#if UNITY_5_4_OR_NEWER
            m_value = "unity_WorldToObject";
#else
            m_value = "_World2Object";
#endif
			m_drawPreview = false;
		}
    }
}
