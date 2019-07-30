namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Color Space Double", "Miscellaneous", "Color Space Double" )]
	public class ColorSpaceDouble : ParentNode
	{
		private const string ColorSpaceDoubleStr = "unity_ColorSpaceDouble";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputColorPorts( "RGBA" );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			return GetOutputVectorItem( 0, outputId, ColorSpaceDoubleStr ); ;
		}
	}
}
