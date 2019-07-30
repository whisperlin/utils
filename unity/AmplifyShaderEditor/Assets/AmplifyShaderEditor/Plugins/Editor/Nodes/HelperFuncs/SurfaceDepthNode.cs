using UnityEngine;
using UnityEditor;

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Surface Depth", "Surface Data", "Returns the surface view depth" )]
	public sealed class SurfaceDepthNode : ParentNode
	{
		[SerializeField]
		private int m_viewSpaceInt = 0;

		private readonly string[] m_viewSpaceStr = { "Eye Space", "0-1 Space" };
		private readonly string[] m_vertexNameStr = { "eyeDepth", "clampDepth" };

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, "Depth" );
			m_autoWrapProperties = true;
			SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
		}

		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();
			if( PaddingTitleLeft == 0 )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );
			m_upperLeftWidget.OnNodeLayout( m_globalPosition, drawInfo );
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );
			m_upperLeftWidget.DrawGUIControls( drawInfo );
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );
			if( !m_isVisible )
				return;
			m_upperLeftWidget.OnNodeRepaint( ContainerGraph.LodLevel );
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			EditorGUI.BeginChangeCheck();
			m_viewSpaceInt = m_upperLeftWidget.DrawWidget( this, m_viewSpaceInt, m_viewSpaceStr );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_viewSpaceInt = EditorGUILayoutPopup( "View Space", m_viewSpaceInt, m_viewSpaceStr );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate )
			{
				return dataCollector.TemplateDataCollectorInstance.GetEyeDepth();
			}

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				string vertexSpace = m_viewSpaceInt == 1 ? " * _ProjectionParams.w" : "";
				string vertexInstruction = "-UnityObjectToViewPos( " + Constants.VertexShaderInputStr + ".vertex.xyz ).z" + vertexSpace;
				dataCollector.AddVertexInstruction( "float " + m_vertexNameStr[ m_viewSpaceInt ] + " = " + vertexInstruction, UniqueId );

				return m_vertexNameStr[ m_viewSpaceInt ];
			}

			dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );


			if( dataCollector.TesselationActive )
			{
				string eyeDepth = GeneratorUtils.GenerateScreenDepthOnFrag( ref dataCollector, UniqueId, m_currentPrecisionType );
				if( m_viewSpaceInt == 1 )
				{
					dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT, m_vertexNameStr[ 1 ], eyeDepth + " * _ProjectionParams.w" );
					return m_vertexNameStr[ 1 ];
				}
				else
				{
					return eyeDepth;
				}
			}
			else
			{
				string space = string.Empty;
				if( m_viewSpaceInt == 1 )
					space = " * _ProjectionParams.w";

				dataCollector.AddToInput( UniqueId, "float " + m_vertexNameStr[ m_viewSpaceInt ], true );
				string instruction = "-UnityObjectToViewPos( " + Constants.VertexShaderInputStr + ".vertex.xyz ).z" + space;
				dataCollector.AddVertexInstruction( Constants.VertexShaderOutputStr + "." + m_vertexNameStr[ m_viewSpaceInt ] + " = " + instruction, UniqueId );

				return Constants.InputVarStr + "." + m_vertexNameStr[ m_viewSpaceInt ];
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_viewSpaceInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_viewSpaceInt );
		}
	}

}
