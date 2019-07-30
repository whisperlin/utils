// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum ViewSpace
	{
		Tangent,
		World
	}

	[Serializable]
	[NodeAttributes( "View Dir", "Camera And Screen", "View direction vector, you can select between <b>World</b> space or <b>Tangent</b> space" )]
	public sealed class ViewDirInputsCoordNode : SurfaceShaderINParentNode
	{
		private const string SpaceStr = "Space";
		private const string WorldDirVarStr = "worldViewDir";
		private const string SubLabelFormat = "Space( {0} )";

		[ SerializeField]
		private ViewSpace m_viewDirSpace = ViewSpace.World;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = AvailableSurfaceInputs.VIEW_DIR;
			InitialSetup();
			m_textLabelWidth = 75;
			m_autoWrapProperties = true;
			m_drawPreviewAsSphere = true;
			UpdateTitle();
			m_previewShaderGUID = "07b57d9823df4bd4d8fe6dcb29fca36a";
		}

		private void UpdateTitle()
		{
			m_additionalContent.text = string.Format( SubLabelFormat, m_viewDirSpace.ToString() );
			m_sizeIsDirty = true;
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
			m_viewDirSpace = (ViewSpace)m_upperLeftWidget.DrawWidget ( this, m_viewDirSpace );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateTitle();
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_viewDirSpace = ( ViewSpace ) EditorGUILayoutEnumPopup( SpaceStr, m_viewDirSpace );
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdateTitle();
			}
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_viewDirSpace == ViewSpace.World )
				m_previewMaterialPassId = 0;
			else if ( m_viewDirSpace == ViewSpace.Tangent )
				m_previewMaterialPassId = 1;
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if ( m_viewDirSpace == ViewSpace.Tangent )
				dataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( dataCollector.IsTemplate )
			{
				string varName = ( m_viewDirSpace == ViewSpace.World )?	dataCollector.TemplateDataCollectorInstance.GetNormalizedViewDir():
																			dataCollector.TemplateDataCollectorInstance.GetTangenViewDir();
				return GetOutputVectorItem( 0, outputId, varName );
			}


			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				if ( m_viewDirSpace == ViewSpace.World )
				{
					string precision = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT3 );
					string worldPos = GeneratorUtils.GenerateWorldPosition( ref dataCollector, UniqueId );

					dataCollector.AddLocalVariable( UniqueId, precision + " worldViewDir = normalize( UnityWorldSpaceViewDir( " + worldPos + " ) );" );
					return GetOutputVectorItem( 0, outputId, "worldViewDir" );
				}
				else
				{
					string precision = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT3 );
					string worldPos = GeneratorUtils.GenerateWorldPosition( ref dataCollector, UniqueId );
					string worldToTangent = GeneratorUtils.GenerateWorldToTangentMatrix( ref dataCollector, UniqueId, m_currentPrecisionType );

					dataCollector.AddLocalVariable( UniqueId, precision + " tangentViewDir = mul( " + worldToTangent + ", normalize( UnityWorldSpaceViewDir( " + worldPos + " ) ) );" );
					return GetOutputVectorItem( 0, outputId, "tangentViewDir" );
				}
			}
			else
			{
				if ( m_viewDirSpace == ViewSpace.World )
				{
					if ( dataCollector.DirtyNormal )
					{
						dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_POS ), true );
						dataCollector.AddToLocalVariables( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, WorldDirVarStr, "normalize( UnityWorldSpaceViewDir( " + Constants.InputVarStr + ".worldPos ) )" );
						return GetOutputVectorItem( 0, outputId, WorldDirVarStr );
					}
					else
					{
						return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
					}
				}
				else
				{
					dataCollector.ForceNormal = true;
					return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
				}
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 2402 )
				m_viewDirSpace = ( ViewSpace ) Enum.Parse( typeof( ViewSpace ), GetCurrentParam( ref nodeParams ) );

			UpdateTitle();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_viewDirSpace );
		}
	}
}
