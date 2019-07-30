// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Space Camera Pos", "Camera And Screen", "World Space Camera position" )]
	public sealed class WorldSpaceCameraPos : ConstantShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "XYZ", WirePortDataType.FLOAT3 );
			m_value = "_WorldSpaceCameraPos";
			m_previewShaderGUID = "6b0c78411043dd24dac1152c84bb63ba";
		}
	}
}
