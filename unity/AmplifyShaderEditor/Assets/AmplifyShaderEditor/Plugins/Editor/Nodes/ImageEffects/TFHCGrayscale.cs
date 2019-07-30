// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Grayscale
// Donated by The Four Headed Cat - @fourheadedcat

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Grayscale", "Image Effects", "Convert image colors to grayscale", null, KeyCode.None, true, false, null, null, true )]
	public sealed class TFHCGrayscale : ParentNode
	{
		private const string GrayscaleStyleStr = "Grayscale Style";

		[SerializeField]
		private int m_grayscaleStyle;

		[SerializeField]
		private readonly string[] m_GrayscaleStyleValues = { "Luminance", "Natural Classic", "Old School" };

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "In" );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_textLabelWidth = 120;
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_GrayscaleStyleValues[ m_grayscaleStyle ] ) );
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

		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
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
			m_grayscaleStyle = m_upperLeftWidget.DrawWidget( this, m_grayscaleStyle, m_GrayscaleStyleValues );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_GrayscaleStyleValues[ m_grayscaleStyle ] ) );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_grayscaleStyle = EditorGUILayoutPopup( GrayscaleStyleStr, m_grayscaleStyle, m_GrayscaleStyleValues );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_GrayscaleStyleValues[ m_grayscaleStyle ] ) );
			}
			EditorGUILayout.HelpBox( "Grayscale Old:\n\n - In: Image to convert.\n - Grayscale Style: Select the grayscale style.\n\n - Out: Grayscale version of the image.", MessageType.None );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_grayscaleStyle = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_GrayscaleStyleValues[ m_grayscaleStyle ] ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_grayscaleStyle );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string i = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string grayscale = string.Empty;
			switch ( m_grayscaleStyle )
			{
				case 1: { grayscale = "dot(" + i + ", float3(0.299,0.587,0.114))"; } break;
				case 2: { grayscale = "(" + i + ".r + " + i + ".g + " + i + ".b) / 3"; } break;
				default: { grayscale = "Luminance(" + i + ")"; } break;
			}
			return grayscale;
		}
	}
}
