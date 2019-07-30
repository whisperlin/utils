using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Template Parameter", "Constants And Properties", "Select and use one of the pre-existing properties given by the template" )]
	public class TemplateShaderPropertyNode : ParentNode
	{
		private const string ErrorMessageStr = "This node can only be used inside a Template category!";
		private const string WarningStr = "Preview only works for properties";
		private const string PropertyLabelStr = "Parameter";
		private const string TypeLabelStr = "Type: ";
		
		private int IntPropertyId;
		private int FloatPropertyId;
		private int VectorPropertyId;
		private int Sampler2DPropertyId;
		private int Sampler3DPropertyId;
		private int SamplerCubePropertyId;

		[SerializeField]
		private int m_currentPropertyIdx = -1;

		[SerializeField]
		private string m_propertyName = string.Empty;
		[SerializeField]
		private int m_propertyNameId = 0;

		[SerializeField]
		private string m_typeName = string.Empty;

		private bool m_fetchPropertyId = false;
		private List<TemplateShaderPropertyData> m_shaderProperties = null;
		private string[] m_propertyLabels = null;

		private UpperLeftWidgetHelper m_upperLeftWidgetHelper = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_textLabelWidth = 67;
			m_previewShaderGUID = "4feb2016be0ece148b8bf234508f6aa4";
		}

		public override void OnEnable()
		{
			base.OnEnable();
			IntPropertyId = Shader.PropertyToID( "_IntData" );
			FloatPropertyId = Shader.PropertyToID( "_FloatData" );
			VectorPropertyId = Shader.PropertyToID( "_VectorData" );
			Sampler2DPropertyId = Shader.PropertyToID( "_Sampler2DData" );
			Sampler3DPropertyId = Shader.PropertyToID( "_Sampler3DData" );
			SamplerCubePropertyId = Shader.PropertyToID( "_SamplerCubeData" );
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();

			if ( PaddingTitleLeft == 0 )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if ( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			if ( m_currentPropertyIdx > -1 )
			{
				EditorGUI.BeginChangeCheck();
				m_currentPropertyIdx = EditorGUILayoutPopup( PropertyLabelStr, m_currentPropertyIdx, m_propertyLabels );
				if ( EditorGUI.EndChangeCheck() )
				{
					UpdateFromId();
				}
				EditorGUILayout.LabelField( m_typeName );
			}
		}

		void CheckWarningState()
		{
			if ( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
			{
				ShowTab( NodeMessageType.Error, ErrorMessageStr );
			}
			else
			{
				if ( m_shaderProperties != null &&
					m_shaderProperties.Count > m_currentPropertyIdx &&
					m_shaderProperties[ m_currentPropertyIdx ].PropertyType == PropertyType.Global )
				{
					ShowTab( NodeMessageType.Info, WarningStr );
				}
				else
				{
					m_showErrorMessage = false;
				}
			}
		}

		public override void SetPreviewInputs()
		{
			if ( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
				return;

			if ( m_shaderProperties == null || m_currentPropertyIdx >= m_shaderProperties.Count )
				return;

			if ( m_shaderProperties[ m_currentPropertyIdx ].PropertyType == PropertyType.Global )
			{
				m_additionalContent.text = string.Empty;
				PreviewMaterial.SetInt( IntPropertyId, 0 );
				return;
			}

			Material currMat = m_containerGraph.CurrentMaterial;
			if ( currMat != null && currMat.HasProperty( m_propertyNameId ) )
			{
				switch ( m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType )
				{
					case WirePortDataType.INT:
					{
						int value = currMat.GetInt( m_propertyNameId );
						m_additionalContent.text = GenerateTitle( value );
						PreviewMaterial.SetInt( IntPropertyId, value );
					}
					break;
					case WirePortDataType.FLOAT:
					{
						float value = currMat.GetFloat( m_propertyNameId );
						m_additionalContent.text = GenerateTitle( value );
						PreviewMaterial.SetFloat( FloatPropertyId, value );
					}
					break;
					case WirePortDataType.FLOAT4:
					{
						Vector4 value = currMat.GetVector( m_propertyNameId );
						m_additionalContent.text = GenerateTitle( value.x, value.y, value.z, value.w );
						PreviewMaterial.SetVector( VectorPropertyId, value );
					}
					break;
					case WirePortDataType.COLOR:
					{
						Color value = currMat.GetColor( m_propertyNameId );
						m_additionalContent.text = GenerateTitle( value.r, value.g, value.b, value.a );
						PreviewMaterial.SetColor( VectorPropertyId, value );
					}
					break;
					case WirePortDataType.SAMPLER2D:
					{
						m_additionalContent.text = string.Empty;
						PreviewMaterial.SetTexture( Sampler2DPropertyId, currMat.GetTexture( m_propertyNameId ) );
					}
					break;
					case WirePortDataType.SAMPLER3D:
					{
						m_additionalContent.text = string.Empty;
						PreviewMaterial.SetTexture( Sampler3DPropertyId, currMat.GetTexture( m_propertyNameId ) );
					}
					break;
					case WirePortDataType.SAMPLERCUBE:
					{
						m_additionalContent.text = string.Empty;
						PreviewMaterial.SetTexture( SamplerCubePropertyId, currMat.GetTexture( m_propertyNameId ) );
					}
					break;
				}
			}
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );
			m_upperLeftWidgetHelper.OnNodeLayout( m_globalPosition, drawInfo );
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );
			m_upperLeftWidgetHelper.DrawGUIControls( drawInfo );
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );
			if( !m_isVisible )
				return;
			m_upperLeftWidgetHelper.OnNodeRepaint( ContainerGraph.LodLevel );
		}

		public override void Draw( DrawInfo drawInfo )
		{
			if ( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
			{
				if ( !m_showErrorMessage || m_errorMessageTypeIsError == NodeMessageType.Info )
				{
					ShowTab( NodeMessageType.Error, ErrorMessageStr );
				}
			}
			else if ( m_showErrorMessage )
			{
				if ( m_errorMessageTypeIsError == NodeMessageType.Error )
					HideTab();
			}

			base.Draw( drawInfo );
			if ( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
				return;


			if ( m_shaderProperties == null )
			{
				MasterNode masterNode = m_containerGraph.CurrentMasterNode;
				if ( masterNode.CurrentMasterNodeCategory == AvailableShaderTypes.Template )
				{
					TemplateData currentTemplate = ( masterNode as TemplateMasterNode ).CurrentTemplate;
					if ( currentTemplate != null )
					{
						m_shaderProperties = currentTemplate.AvailableShaderProperties;
						m_fetchPropertyId = true;
					}
				}
			}

			if ( m_fetchPropertyId )
			{
				m_fetchPropertyId = false;
				FetchPropertyId();
			}

			if ( m_currentPropertyIdx > -1 )
			{
				EditorGUI.BeginChangeCheck();
				m_currentPropertyIdx = m_upperLeftWidgetHelper.DrawWidget( this,m_currentPropertyIdx, m_propertyLabels );
				if ( EditorGUI.EndChangeCheck() )
				{
					UpdateFromId();
				}
			}
		}

		void FetchPropertyId()
		{
			if ( m_shaderProperties != null )
			{
				m_currentPropertyIdx = 0;
				m_propertyLabels = new string[ m_shaderProperties.Count ];
				for ( int i = 0; i < m_shaderProperties.Count; i++ )
				{
					if ( m_shaderProperties[ i ].PropertyName.Equals( m_propertyName ) )
					{
						m_currentPropertyIdx = i;
					}
					m_propertyLabels[ i ] = m_shaderProperties[ i ].PropertyInspectorName;
				}
				UpdateFromId();
			}
			else
			{
				m_currentPropertyIdx = -1;
			}
		}

		void UpdateFromId()
		{
			if ( m_shaderProperties != null )
			{
				bool areCompatible = TemplateHelperFunctions.CheckIfCompatibles( m_outputPorts[ 0 ].DataType, m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType );
				m_outputPorts[ 0 ].ChangeType( m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType, false );
				if ( !areCompatible )
				{
					m_containerGraph.DeleteConnection( false, UniqueId, 0, false, true );
				}

				m_propertyName = m_shaderProperties[ m_currentPropertyIdx ].PropertyName;
				m_content.text = m_shaderProperties[ m_currentPropertyIdx ].PropertyInspectorName;
				m_propertyNameId = Shader.PropertyToID( m_propertyName );
				m_typeName = TypeLabelStr + m_shaderProperties[ m_currentPropertyIdx ].PropertyType.ToString();
				m_sizeIsDirty = true;
				Material currMat = m_containerGraph.CurrentMaterial;
				if ( currMat != null )
				{
					if ( m_shaderProperties[ m_currentPropertyIdx ].PropertyType == PropertyType.Global )
					{
						m_previewMaterialPassId = 0;
						if ( !m_showErrorMessage )
						{
							ShowTab( NodeMessageType.Info, WarningStr );
						}
					}
					else
					{
						if ( m_showErrorMessage && m_errorMessageTypeIsError != NodeMessageType.Error)
						{
							HideTab();
						}
						switch ( m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType )
						{
							case WirePortDataType.INT: m_previewMaterialPassId = 0; break;
							case WirePortDataType.FLOAT: m_previewMaterialPassId = 1; break;
							case WirePortDataType.FLOAT4:
							case WirePortDataType.COLOR: m_previewMaterialPassId = 2; break;
							case WirePortDataType.SAMPLER2D: m_previewMaterialPassId = 3; break;
							case WirePortDataType.SAMPLER3D: m_previewMaterialPassId = 4; break;
							case WirePortDataType.SAMPLERCUBE: m_previewMaterialPassId = 5; break;
							default: PreviewMaterial.SetPass( 0 ); break;
						}
					}
				}
				CheckWarningState();
			}
		}

		string GenerateTitle( params float[] values )
		{
			string finalResult = "( ";
			if ( values.Length == 1 )
			{
				finalResult += values[ 0 ].ToString( Mathf.Abs( values[ 0 ] ) > 1000 ? Constants.PropertyBigFloatFormatLabel : Constants.PropertyFloatFormatLabel );
			}
			else
			{
				for ( int i = 0; i < values.Length; i++ )
				{
					finalResult += values[ i ].ToString( Mathf.Abs( values[ i ] ) > 1000 ? Constants.PropertyBigVectorFormatLabel : Constants.PropertyVectorFormatLabel );
					if ( i < ( values.Length - 1 ) )
						finalResult += ",";
				}
			}
			finalResult += " )";
			return finalResult;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.MasterNodeCategory != AvailableShaderTypes.Template )
			{
				UIUtils.ShowMessage( "This node is only intended for templates use only" );
				return "0";
			}
			return m_propertyName;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_propertyName = GetCurrentParam( ref nodeParams );
			m_propertyNameId = Shader.PropertyToID( m_propertyName );
			m_fetchPropertyId = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_propertyName );
		}

		public override void OnMasterNodeReplaced( MasterNode newMasterNode )
		{
			base.OnMasterNodeReplaced( newMasterNode );
			if ( newMasterNode.CurrentMasterNodeCategory == AvailableShaderTypes.Template )
			{
				m_shaderProperties = ( newMasterNode as TemplateMasterNode ).CurrentTemplate.AvailableShaderProperties;
				FetchPropertyId();
				//m_containerGraph.DeleteConnection( false, UniqueId, 0, false, true );
			}
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			CheckWarningState();
		}

		public override void Destroy()
		{
			base.Destroy();
			m_propertyLabels = null;
			m_shaderProperties = null;
			m_upperLeftWidgetHelper = null;
		}
	}
}
