// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Color", "Constants And Properties", "Color property", null, KeyCode.Alpha5 )]
	public sealed class ColorNode : PropertyNode
	{
		[SerializeField]
		[ColorUsage( true, true, float.MinValue, float.MinValue, float.MinValue, float.MaxValue )]
		private Color m_defaultValue = new Color( 0, 0, 0, 0 );

		[SerializeField]
		[ColorUsage( true, true, float.MinValue, float.MinValue, float.MinValue, float.MaxValue )]
		private Color m_materialValue = new Color( 0, 0, 0, 0 );

		[SerializeField]
		private bool m_isHDR = false;

		private ColorPickerHDRConfig m_hdrConfig = new ColorPickerHDRConfig( 0, float.MaxValue, 0, float.MaxValue );
		private GUIContent m_dummyContent;

		private int m_cachedPropertyId = -1;

		public ColorNode() : base() { }
		public ColorNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_insideSize.Set( 100, 50 );

			m_dummyContent = new GUIContent();
			AddOutputColorPorts( "RGBA" );
			m_drawPreview = false;
			m_drawPreviewExpander = false;
			m_canExpand = false;
			m_selectedLocation = PreviewLocation.BottomCenter;
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			m_previewShaderGUID = "6cf365ccc7ae776488ae8960d6d134c3";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_cachedPropertyId == -1 )
				m_cachedPropertyId = Shader.PropertyToID( "_InputColor" );

			if ( m_materialMode && m_currentParameterType != PropertyType.Constant )
				PreviewMaterial.SetColor( m_cachedPropertyId, m_materialValue );
			else
				PreviewMaterial.SetColor( m_cachedPropertyId, m_defaultValue );
		}

		public override void CopyDefaultsToMaterial()
		{
			m_materialValue = m_defaultValue;
		}

		public override void DrawSubProperties()
		{
			m_defaultValue = EditorGUILayoutColorField( Constants.DefaultValueLabelContent, m_defaultValue, false, true, m_isHDR, m_hdrConfig );
		}

		public override void DrawMaterialProperties()
		{
			if ( m_materialMode )
				EditorGUI.BeginChangeCheck();

			m_materialValue = EditorGUILayoutColorField( Constants.MaterialValueLabelContent, m_materialValue, false, true, m_isHDR, m_hdrConfig );

			if ( m_materialMode && EditorGUI.EndChangeCheck() )
				m_requireMaterialUpdate = true;
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			m_propertyDrawPos = m_globalPosition;
			m_propertyDrawPos.x = m_remainingBox.x;
			m_propertyDrawPos.y = m_remainingBox.y;
			m_propertyDrawPos.width = 80 * drawInfo.InvertedZoom;
			m_propertyDrawPos.height = m_remainingBox.height;
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if ( drawInfo.CurrentEventType != EventType.MouseDown )
				return;

			Rect hitBox = m_remainingBox;
			//hitBox.xMin -= LabelWidth * drawInfo.InvertedZoom;
			bool insideBox = hitBox.Contains( drawInfo.MousePosition );

			if ( insideBox )
			{
				m_isEditingFields = true;
			}
			else if ( m_isEditingFields && !insideBox )
			{
				GUI.FocusControl( null );
				m_isEditingFields = false;
			}
		}

		private bool m_isEditingFields;

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if ( !m_isVisible )
				return;

			if ( m_isEditingFields )
			{
				if ( m_materialMode && m_currentParameterType != PropertyType.Constant )
				{
					EditorGUI.BeginChangeCheck();
					m_materialValue = EditorGUIColorField( m_propertyDrawPos, m_dummyContent, m_materialValue, false, true, m_isHDR, m_hdrConfig );
					if ( EditorGUI.EndChangeCheck() )
					{
						m_requireMaterialUpdate = true;
						if ( m_currentParameterType != PropertyType.Constant )
						{
							BeginDelayedDirtyProperty();
						}
					}
				}
				else
				{
					EditorGUI.BeginChangeCheck();

					m_defaultValue = EditorGUIColorField( m_propertyDrawPos, m_dummyContent, m_defaultValue, false, true, m_isHDR, m_hdrConfig );
					if ( EditorGUI.EndChangeCheck() )
					{
						BeginDelayedDirtyProperty();
					}
				}
			}
			else if ( drawInfo.CurrentEventType == EventType.Repaint )
			{
				if ( m_materialMode && m_currentParameterType != PropertyType.Constant )
					EditorGUIUtility.DrawColorSwatch( m_propertyDrawPos, m_materialValue );
				else
					EditorGUIUtility.DrawColorSwatch( m_propertyDrawPos, m_defaultValue );

				GUI.Label( m_propertyDrawPos, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
			}
		}

		public override void ConfigureLocalVariable( ref MasterNodeDataCollector dataCollector )
		{
			Color color = m_defaultValue;
			dataCollector.AddLocalVariable( UniqueId, CreateLocalVarDec( color.r + "," + color.g + "," + color.b + "," + color.a ) );

			m_outputPorts[ 0 ].SetLocalValue( m_propertyName );
			m_outputPorts[ 1 ].SetLocalValue( m_propertyName + ".r" );
			m_outputPorts[ 2 ].SetLocalValue( m_propertyName + ".g" );
			m_outputPorts[ 3 ].SetLocalValue( m_propertyName + ".b" );
			m_outputPorts[ 4 ].SetLocalValue( m_propertyName + ".a" );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );

			if ( m_currentParameterType != PropertyType.Constant )
				return GetOutputVectorItem( 0, outputId, PropertyData );

			if ( m_outputPorts[ outputId ].IsLocalValue )
			{
				return m_outputPorts[ outputId ].LocalValue;
			}

			if ( CheckLocalVariable( ref dataCollector ) )
			{
				return m_outputPorts[ outputId ].LocalValue;
			}

			Color color = m_defaultValue;
			string result = string.Empty;

			switch ( outputId )
			{
				case 0:
				{
					result = m_precisionString + "(" + color.r + "," + color.g + "," + color.b + "," + color.a + ")";
				}
				break;

				case 1:
				{
					result = color.r.ToString();
				}
				break;
				case 2:
				{
					result = color.g.ToString();
				}
				break;
				case 3:
				{
					result = color.b.ToString();
				}
				break;
				case 4:
				{
					result = color.a.ToString();
				}
				break;
			}
			return result;
		}

		protected override void OnAtrributesChanged()
		{
			CheckIfHDR();
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			CheckIfHDR();
		}

		void CheckIfHDR()
		{
			int count = m_selectedAttribs.Count;
			bool hdrBuffer = m_isHDR;
			m_isHDR = false;
			for ( int i = 0; i < count; i++ )
			{
				if ( m_selectedAttribs[ i ] == 1 /*HDR Property ID*/)
				{
					m_isHDR = true;
					break;
				}
			}

			if ( hdrBuffer && !m_isHDR )
			{
				bool fireDirtyProperty = false;

				if ( m_defaultValue.r > 1 || m_defaultValue.g > 1 || m_defaultValue.b > 1 )
				{
					float defaultColorLength = Mathf.Sqrt( m_defaultValue.r * m_defaultValue.r + m_defaultValue.g * m_defaultValue.g + m_defaultValue.b * m_defaultValue.b );
					m_defaultValue.r /= defaultColorLength;
					m_defaultValue.g /= defaultColorLength;
					m_defaultValue.b /= defaultColorLength;
					fireDirtyProperty = true;
				}

				if ( m_materialValue.r > 1 || m_materialValue.g > 1 || m_materialValue.b > 1 )
				{
					float materialColorLength = Mathf.Sqrt( m_materialValue.r * m_materialValue.r + m_materialValue.g * m_materialValue.g + m_materialValue.b * m_materialValue.b );
					m_materialValue.r /= materialColorLength;
					m_materialValue.g /= materialColorLength;
					m_materialValue.b /= materialColorLength;
					fireDirtyProperty = true;
				}

				if ( fireDirtyProperty )
					BeginDelayedDirtyProperty();
			}
		}

		public override string GetPropertyValue()
		{
			return PropertyAttributes + m_propertyName + "(\"" + m_propertyInspectorName + "\", Color) = (" + m_defaultValue.r + "," + m_defaultValue.g + "," + m_defaultValue.b + "," + m_defaultValue.a + ")";
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );

			if ( UIUtils.IsProperty( m_currentParameterType ) )
			{
				mat.SetColor( m_propertyName, m_materialValue );
			}
		}

		public override void SetMaterialMode( Material mat, bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat, fetchMaterialValues );
			if ( m_materialMode && fetchMaterialValues )
			{
				MaterialValue = ( UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) ) ?
																					mat.GetColor( m_propertyName ) :
																					m_defaultValue;
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
			{
				MaterialValue = material.GetColor( m_propertyName );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			string[] colorChannels = GetCurrentParam( ref nodeParams ).Split( IOUtils.VECTOR_SEPARATOR );
			if ( colorChannels.Length == 4 )
			{
				m_defaultValue.r = Convert.ToSingle( colorChannels[ 0 ] );
				m_defaultValue.g = Convert.ToSingle( colorChannels[ 1 ] );
				m_defaultValue.b = Convert.ToSingle( colorChannels[ 2 ] );
				m_defaultValue.a = Convert.ToSingle( colorChannels[ 3 ] );
			}
			else
			{
				UIUtils.ShowMessage( "Incorrect number of color values", MessageSeverity.Error );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValue.r.ToString() + IOUtils.VECTOR_SEPARATOR + m_defaultValue.g.ToString() + IOUtils.VECTOR_SEPARATOR + m_defaultValue.b.ToString() + IOUtils.VECTOR_SEPARATOR + m_defaultValue.a.ToString() );
		}

		public override void ReadAdditionalClipboardData( ref string[] nodeParams )
		{
			base.ReadAdditionalClipboardData( ref nodeParams );
			m_materialValue = IOUtils.StringToColor( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteAdditionalClipboardData( ref string nodeInfo )
		{
			base.WriteAdditionalClipboardData( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, IOUtils.ColorToString( m_materialValue ) );
		}

		public override string GetPropertyValStr()
		{
			return ( m_materialMode && m_currentParameterType != PropertyType.Constant ) ? m_materialValue.r.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_materialValue.g.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_materialValue.b.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_materialValue.a.ToString( Constants.PropertyVectorFormatLabel ) :
																						m_defaultValue.r.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_defaultValue.g.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_defaultValue.b.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_defaultValue.a.ToString( Constants.PropertyVectorFormatLabel );
		}

		private Color MaterialValue
		{
			set
			{
				if ( !m_isHDR && ( value.r > 1 || value.g > 1 || value.r > 1 ) )
				{
					float materialColorLength = Mathf.Sqrt( value.r * value.r + value.g * value.g + value.b * value.b );
					m_materialValue.r = value.r / materialColorLength;
					m_materialValue.g = value.g / materialColorLength;
					m_materialValue.b = value.b / materialColorLength;
					m_materialValue.a = value.a;
				}
				else
				{
					m_materialValue = value;
				}
			}
		}

		public Color Value
		{
			get { return m_defaultValue; }
			set { m_defaultValue = value; }
		}
	}
}
