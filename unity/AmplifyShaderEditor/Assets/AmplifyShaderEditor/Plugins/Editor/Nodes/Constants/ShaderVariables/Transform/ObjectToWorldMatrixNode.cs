// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Object To World Matrix", "Matrix Transform", "Current model matrix" )]
	public sealed class ObjectToWorldMatrixNode : ConstantShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "Out", WirePortDataType.FLOAT4x4 );
#if UNITY_5_4_OR_NEWER
            m_value = "unity_ObjectToWorld";
#else
            m_value = "_Object2World";
#endif
		}
    }
}
