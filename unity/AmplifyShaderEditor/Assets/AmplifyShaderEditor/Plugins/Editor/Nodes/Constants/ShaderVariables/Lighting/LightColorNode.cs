// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Light Color", "Light", "Light Color, RGB value already contains light intensity while A only contains light intensity" )]
	public sealed class LightColorNode : ShaderVariablesNode
	{
		private const string m_lightColorValue = "_LightColor0";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "RGBA", WirePortDataType.COLOR );
			m_previewShaderGUID = "43f5d3c033eb5044e9aeb40241358349";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.IsTemplate )
				dataCollector.AddToIncludes( -1, Constants.UnityLightingLib );

			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			return m_lightColorValue;
		}
	}
}
