// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Grab Screen Position", "Camera And Screen", "Screen position correctly transformed to be used with Grab Screen Color" )]
	public sealed class GrabScreenPosition : ParentNode
	{
		private const string ProjectStr = "Project";
		private const string ScreenPosStr = "screenPos";
		private readonly string ProjectionInstruction = "{0}.xyzw /= {0}.w;";
		private readonly string[] HackInstruction = {   "#if UNITY_UV_STARTS_AT_TOP",
														"float scale{0} = -1.0;",
														"#else",
														"float scale{0} = 1.0;",
														"#endif",
														"float halfPosW{1} = {0}.w * 0.5;",
														"{0}.y = ( {0}.y - halfPosW{1} ) * _ProjectionParams.x* scale{1} + halfPosW{1};",
														"#ifdef UNITY_SINGLE_PASS_STEREO",
														"{0}.xy = TransformStereoScreenSpaceTex({0}.xy, {0}.w);",
														"#endif"};


		private readonly string ScreenPosOnVert00Str = "{0} = ComputeScreenPos( mul( UNITY_MATRIX_MVP, {1}.vertex));";
	
		private readonly string[] m_outputTypeStr = { "Normalized", "Screen" };
	
		[SerializeField]
		private int m_outputTypeInt = 0;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT4, "XYZW" );
			m_autoWrapProperties = true;
			m_textLabelWidth = 65;
			ConfigureHeader();
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
			m_outputTypeInt = m_upperLeftWidget.DrawWidget( this, m_outputTypeInt, m_outputTypeStr );
			if( EditorGUI.EndChangeCheck() )
			{
				ConfigureHeader();
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			
			EditorGUI.BeginChangeCheck();
			m_outputTypeInt = EditorGUILayoutPopup( "Type", m_outputTypeInt, m_outputTypeStr );
			if ( EditorGUI.EndChangeCheck() )
			{
				ConfigureHeader();
			}
		}

		void ConfigureHeader()
		{
			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_outputTypeStr[ m_outputTypeInt ] ) );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return GetOutputColorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );

			//string localVarName = ScreenPosStr + m_uniqueId;
			string localVarName = string.Empty;

			bool isFragment = dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug;

			if ( isFragment )
			{
				string screenPos = dataCollector.IsTemplate? dataCollector.TemplateDataCollectorInstance.GetScreenPos():GeneratorUtils.GenerateScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, true );
				localVarName = screenPos + OutputId;
				//dataCollector.AddToInput( m_uniqueId, "float4 " + ScreenPosStr, true );
				string value = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ) + " " + localVarName + " = " + screenPos + ";";
				dataCollector.AddLocalVariable( UniqueId, value );
			}
			else
			{
				string screenPos = dataCollector.IsTemplate ? dataCollector.TemplateDataCollectorInstance.GetScreenPos() : GeneratorUtils.GenerateVertexScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, false );
				localVarName = screenPos + OutputId;
				string localVarDecl = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ) + " " + localVarName;
				string value = string.Format( ScreenPosOnVert00Str, localVarDecl, dataCollector.IsTemplate ? dataCollector.TemplateDataCollectorInstance.CurrentTemplateData.VertexFunctionData.InVarName : Constants.VertexShaderInputStr );
				dataCollector.AddLocalVariable( UniqueId, value );
				//dataCollector.AddLocalVariable( m_uniqueId, string.Format( ScreenPosOnVert01Str, localVarName ) );
			}
			
			dataCollector.AddLocalVariable( UniqueId, HackInstruction[ 0 ], true );
			dataCollector.AddLocalVariable( UniqueId, string.Format( HackInstruction[ 1 ], OutputId ), true );
			dataCollector.AddLocalVariable( UniqueId, HackInstruction[ 2 ], true );
			dataCollector.AddLocalVariable( UniqueId, string.Format( HackInstruction[ 3 ], OutputId ), true );
			dataCollector.AddLocalVariable( UniqueId, HackInstruction[ 4 ], true );
			dataCollector.AddLocalVariable( UniqueId, string.Format( HackInstruction[ 5 ], localVarName, OutputId ), true );
			dataCollector.AddLocalVariable( UniqueId, string.Format( HackInstruction[ 6 ], localVarName, OutputId ), true );
			dataCollector.AddLocalVariable( UniqueId, HackInstruction[ 7 ], true );
			dataCollector.AddLocalVariable( UniqueId, string.Format( HackInstruction[ 8 ], localVarName, OutputId ), true );
			dataCollector.AddLocalVariable( UniqueId, HackInstruction[ 9 ], true );
			if ( m_outputTypeInt == 0 )
			{
				dataCollector.AddLocalVariable( UniqueId, string.Format( ProjectionInstruction, localVarName ) );
			}

			m_outputPorts[ 0 ].SetLocalValue( localVarName );
			//RegisterLocalVariable(outputId, localVarName ,ref dataCollector)
			return GetOutputColorItem( 0, outputId, localVarName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 3108 )
			{
				if ( UIUtils.CurrentShaderVersion() < 6102 )
				{
					bool project = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					m_outputTypeInt = project ? 0 : 1;
				}
				else
				{
					m_outputTypeInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
			}

			ConfigureHeader();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputTypeInt );
		}
	}
}
