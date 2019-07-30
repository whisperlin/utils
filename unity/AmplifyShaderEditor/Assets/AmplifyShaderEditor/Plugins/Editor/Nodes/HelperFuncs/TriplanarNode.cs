using UnityEngine;
using UnityEditor;

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Triplanar Sampler", "Textures", "Triplanar Mapping" )]
	public sealed class TriplanarNode : ParentNode
	{
		[SerializeField]
		private string m_uniqueName;

		private bool m_editPropertyNameMode = false;
		[SerializeField]
		private string m_propertyInspectorName = "Triplanar Sampler";

		private enum TriplanarType { Spherical, Cylindrical }

		[SerializeField]
		private TriplanarType m_selectedTriplanarType = TriplanarType.Spherical;

		private enum TriplanarSpace { Object, World }

		[SerializeField]
		private TriplanarSpace m_selectedTriplanarSpace = TriplanarSpace.World;

		[SerializeField]
		private bool m_normalCorrection = false;

		[SerializeField]
		private TexturePropertyNode m_topTexture;
		[SerializeField]
		private TexturePropertyNode m_midTexture;
		[SerializeField]
		private TexturePropertyNode m_botTexture;

		[SerializeField]
		private string m_tempTopInspectorName = string.Empty;
		[SerializeField]
		private string m_tempTopName = string.Empty;
		private TexturePropertyValues m_tempTopDefaultValue = TexturePropertyValues.white;
		private int m_tempTopOrderIndex = -1;
		private Texture2D m_tempTopDefaultTexture = null;

		private string m_tempMidInspectorName = string.Empty;
		private string m_tempMidName = string.Empty;
		private TexturePropertyValues m_tempMidDefaultValue = TexturePropertyValues.white;
		private int m_tempMidOrderIndex = -1;
		private Texture2D m_tempMidDefaultTexture = null;

		private string m_tempBotInspectorName = string.Empty;
		private string m_tempBotName = string.Empty;
		private TexturePropertyValues m_tempBotDefaultValue = TexturePropertyValues.white;
		private int m_tempBotOrderIndex = -1;
		private Texture2D m_tempBotDefaultTexture = null;

		private bool m_topTextureFoldout = true;
		private bool m_midTextureFoldout = true;
		private bool m_botTextureFoldout = true;

		private string m_functionNormalCall = "TriplanarNormal( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7} )";
		private string m_functionNormalHeader = "inline float3 TriplanarNormal( sampler2D topBumpMap, sampler2D midBumpMap, sampler2D botBumpMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float vertex )";
		private string[] m_functionNormalBody = {
			"float3 projNormal = ( pow( abs( worldNormal ), falloff ) );",
			"projNormal /= projNormal.x + projNormal.y + projNormal.z;",
			"float3 nsign = sign(worldNormal);",
			"half3 xNorm; half3 yNorm; half3 zNorm;",
			"if(vertex == 1){",
			"xNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );",
			"yNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.zx).xy,0,0) ) );",
			"zNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );",
			"} else {",
			"xNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );",
			"yNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.zx ) );",
			"zNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );",
			"}",
			"xNorm = normalize( half3( xNorm.xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ) );",
			"yNorm = normalize( half3( yNorm.xy + worldNormal.zx, worldNormal.y));",
			"zNorm = normalize( half3( zNorm.xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ) );",
			"xNorm = xNorm.zyx;",
			"yNorm = yNorm.yzx;",
			"zNorm = zNorm.xyz;",
			"return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;"
		};
		private string[] m_functionNormalBodyTMB = {
			"float3 projNormal = ( pow( abs( worldNormal ), falloff ) );",
			"projNormal /= projNormal.x + projNormal.y + projNormal.z;",
			"float3 nsign = sign(worldNormal);",
			"float negProjNormalY = max( 0, projNormal.y * -nsign.y );",
			"projNormal.y = max( 0, projNormal.y * nsign.y );",
			"half3 xNorm; half3 yNorm; half3 yNormN; half3 zNorm;",
			"if(vertex == 1){",
			"xNorm = UnpackNormal( tex2Dlod( midBumpMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );",
			"yNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.zx).xy,0,0) ) );",
			"yNormN = UnpackNormal( tex2Dlod( botBumpMap, float4((tilling * worldPos.zx).xy,0,0) ) );",
			"zNorm = UnpackNormal( tex2Dlod( midBumpMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );",
			"} else {",
			"xNorm = UnpackNormal( tex2D( midBumpMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );",
			"yNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.zx ) );",
			"yNormN = UnpackNormal( tex2D( botBumpMap, tilling * worldPos.zx ) );",
			"zNorm = UnpackNormal( tex2D( midBumpMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );",
			"}",
			"xNorm = normalize( half3( xNorm.xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ) );",
			"yNorm = normalize( half3( yNorm.xy + worldNormal.zx, worldNormal.y));",
			"yNormN = normalize( half3( yNormN.xy + worldNormal.zx, worldNormal.y));",
			"zNorm = normalize( half3( zNorm.xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ) );",
			"xNorm = xNorm.zyx;",
			"yNorm = yNorm.yzx;",
			"yNormN = yNormN.yzx;",
			"zNorm = zNorm.xyz;",
			"return xNorm * projNormal.x + yNorm * projNormal.y + yNormN * negProjNormalY + zNorm * projNormal.z;"
		};


		private string m_functionSamplingCall = "TriplanarSampling( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7} )";
		private string m_functionSamplingHeader = "inline float4 TriplanarSampling( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float vertex )";
		private string[] m_functionSamplingBody = {
			"float3 projNormal = ( pow( abs( worldNormal ), falloff ) );",
			"projNormal /= projNormal.x + projNormal.y + projNormal.z;",
			"float3 nsign = sign( worldNormal );",
			"half4 xNorm; half4 yNorm; half4 zNorm;",
			"if(vertex == 1){",
			"xNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );",
			"yNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.zx).xy,0,0) ) );",
			"zNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );",
			"} else {",
			"xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );",
			"yNorm = ( tex2D( topTexMap, tilling * worldPos.zx ) );",
			"zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );",
			"}",
			"return xNorm* projNormal.x + yNorm* projNormal.y + zNorm* projNormal.z;"
		};

		private string[] m_functionSamplingBodyTMB = {
			"float3 projNormal = ( pow( abs( worldNormal ), falloff ) );",
			"projNormal /= projNormal.x + projNormal.y + projNormal.z;",
			"float3 nsign = sign( worldNormal );",
			"float negProjNormalY = max( 0, projNormal.y * -nsign.y );",
			"projNormal.y = max( 0, projNormal.y * nsign.y );",
			"half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;",
			"if(vertex == 1){",
			"xNorm = ( tex2Dlod( midTexMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );",
			"yNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.zx).xy,0,0) ) );",
			"yNormN = ( tex2Dlod( botTexMap, float4((tilling * worldPos.zx).xy,0,0) ) );",
			"zNorm = ( tex2Dlod( midTexMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );",
			"} else {",
			"xNorm = ( tex2D( midTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );",
			"yNorm = ( tex2D( topTexMap, tilling * worldPos.zx ) );",
			"yNormN = ( tex2D( botTexMap, tilling * worldPos.zx ) );",
			"zNorm = ( tex2D( midTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );",
			"}",
			"return xNorm* projNormal.x + yNorm* projNormal.y + yNormN * negProjNormalY + zNorm* projNormal.z;"
		};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.SAMPLER2D, true, "Top" );
			AddInputPort( WirePortDataType.SAMPLER2D, true, "Middle" );
			AddInputPort( WirePortDataType.SAMPLER2D, true, "Bottom" );
			AddInputPort( WirePortDataType.FLOAT, true, "Tiling" );
			AddInputPort( WirePortDataType.FLOAT, true, "Falloff" );
			AddOutputColorPorts( "RGBA" );
			m_useInternalPortData = true;
			InputPorts[ 3 ].FloatInternalData = 1;
			InputPorts[ 4 ].FloatInternalData = 1;
			m_selectedLocation = PreviewLocation.TopCenter;
			m_marginPreviewLeft = 43;
			m_drawPreviewAsSphere = true;
			m_drawPreviewExpander = false;
			m_drawPreview = true;
			m_showPreview = true;
			m_autoDrawInternalPortData = false;
			m_textLabelWidth = 120;
			//m_propertyInspectorName = "Triplanar Sampler";
			m_previewShaderGUID = "8723015ec59743143aadfbe480e34391";
		}

		public void ReadPropertiesData()
		{
			// Top
			if ( UIUtils.IsUniformNameAvailable( m_tempTopName ) )
			{
				UIUtils.ReleaseUniformName( UniqueId, m_topTexture.PropertyName );
				if ( !string.IsNullOrEmpty( m_tempTopInspectorName ) )
				{
					m_topTexture.SetInspectorName( m_tempTopInspectorName );
				}
				if ( !string.IsNullOrEmpty( m_tempTopName ) )
					m_topTexture.SetPropertyName( m_tempTopName );
				UIUtils.RegisterUniformName( UniqueId, m_topTexture.PropertyName );
			}
			m_topTexture.DefaultTextureValue = m_tempTopDefaultValue;
			m_topTexture.OrderIndex = m_tempTopOrderIndex;
			m_topTexture.DefaultValue = m_tempTopDefaultTexture;
			//m_topTexture.SetMaterialMode( UIUtils.CurrentWindow.CurrentGraph.CurrentMaterial, true );

			// Mid
			if ( UIUtils.IsUniformNameAvailable( m_tempMidName ) )
			{
				UIUtils.ReleaseUniformName( UniqueId, m_midTexture.PropertyName );
				if ( !string.IsNullOrEmpty( m_tempMidInspectorName ) )
					m_midTexture.SetInspectorName( m_tempMidInspectorName );
				if ( !string.IsNullOrEmpty( m_tempMidName ) )
					m_midTexture.SetPropertyName( m_tempMidName );
				UIUtils.RegisterUniformName( UniqueId, m_midTexture.PropertyName );
			}
			m_midTexture.DefaultTextureValue = m_tempMidDefaultValue;
			m_midTexture.OrderIndex = m_tempMidOrderIndex;
			m_midTexture.DefaultValue = m_tempMidDefaultTexture;

			// Bot
			if ( UIUtils.IsUniformNameAvailable( m_tempBotName ) )
			{
				UIUtils.ReleaseUniformName( UniqueId, m_botTexture.PropertyName );
				if ( !string.IsNullOrEmpty( m_tempBotInspectorName ) )
					m_botTexture.SetInspectorName( m_tempBotInspectorName );
				if ( !string.IsNullOrEmpty( m_tempBotName ) )
					m_botTexture.SetPropertyName( m_tempBotName );
				UIUtils.RegisterUniformName( UniqueId, m_botTexture.PropertyName );
			}
			m_botTexture.DefaultTextureValue = m_tempBotDefaultValue;
			m_botTexture.OrderIndex = m_tempBotOrderIndex;
			m_botTexture.DefaultValue = m_tempBotDefaultTexture;
		}

		public override void SetMaterialMode( Material mat, bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat, fetchMaterialValues );
			m_topTexture.SetMaterialMode( mat, fetchMaterialValues );
			m_midTexture.SetMaterialMode( mat, fetchMaterialValues );
			m_botTexture.SetMaterialMode( mat, fetchMaterialValues );
		}

		public void Init()
		{
			// Top
			m_topTexture = ScriptableObject.CreateInstance<TexturePropertyNode>();
			m_topTexture.ContainerGraph = ContainerGraph;
			m_topTexture.CustomPrefix = "Top Texture ";
			m_topTexture.UniqueId = UniqueId;
			m_topTexture.DrawAutocast = false;
			m_topTexture.CurrentParameterType = PropertyType.Property;

			// Mid
			m_midTexture = ScriptableObject.CreateInstance<TexturePropertyNode>();
			m_midTexture.ContainerGraph = ContainerGraph;
			m_midTexture.CustomPrefix = "Mid Texture ";
			m_midTexture.UniqueId = UniqueId;
			m_midTexture.DrawAutocast = false;
			m_midTexture.CurrentParameterType = PropertyType.Property;
			
			// Bot
			m_botTexture = ScriptableObject.CreateInstance<TexturePropertyNode>();
			m_botTexture.ContainerGraph = ContainerGraph;
			m_botTexture.CustomPrefix = "Bot Texture ";
			m_botTexture.UniqueId = UniqueId;
			m_botTexture.DrawAutocast = false;
			m_botTexture.CurrentParameterType = PropertyType.Property;
			
			if (m_materialMode)
				SetDelayedMaterialMode( ContainerGraph.CurrentMaterial);

			if ( m_nodeAttribs != null )
				m_uniqueName = m_nodeAttribs.Name + OutputId;

			ConfigurePorts();

			ReRegisterPorts();
		}

		public override void Destroy()
		{
			base.Destroy();
			
			//UIUtils.UnregisterPropertyNode( m_topTexture );
			//UIUtils.UnregisterTexturePropertyNode( m_topTexture );

			//UIUtils.UnregisterPropertyNode( m_midTexture );
			//UIUtils.UnregisterTexturePropertyNode( m_midTexture );

			//UIUtils.UnregisterPropertyNode( m_botTexture );
			//UIUtils.UnregisterTexturePropertyNode( m_botTexture );
			if( m_topTexture != null )
				m_topTexture.Destroy();
			m_topTexture = null;
			if ( m_midTexture != null )
				m_midTexture.Destroy();
			m_midTexture = null;
			if ( m_botTexture != null )
				m_botTexture.Destroy();
			m_botTexture = null;

			m_tempTopDefaultTexture = null;
			m_tempMidDefaultTexture = null;
			m_tempBotDefaultTexture = null;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();
			if ( m_topTexture == null )
				return;

			
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				PreviewMaterial.SetTexture( "_A", m_inputPorts[ 0 ].InputPreviewTexture );
			} else
			{
				PreviewMaterial.SetTexture( "_A", m_topTexture.Value );
			}
			if ( m_selectedTriplanarType == TriplanarType.Cylindrical && m_midTexture != null )
			{
				if ( m_inputPorts[ 1 ].IsConnected )
					PreviewMaterial.SetTexture( "_B", m_inputPorts[ 1 ].InputPreviewTexture );
				else
					PreviewMaterial.SetTexture( "_B", m_midTexture.Value );
				if ( m_inputPorts[ 2 ].IsConnected )
					PreviewMaterial.SetTexture( "_C", m_inputPorts[ 2 ].InputPreviewTexture );
				else
					PreviewMaterial.SetTexture( "_C", m_botTexture.Value );
			}

			PreviewMaterial.SetFloat( "_IsNormal", (m_normalCorrection ? 1 : 0));
			PreviewMaterial.SetFloat( "_IsSpherical", ( m_selectedTriplanarType == TriplanarType.Spherical ? 1 : 0 ) );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			ReRegisterPorts();
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			ReRegisterPorts();
		}

		public void ReRegisterPorts()
		{
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				UIUtils.UnregisterPropertyNode( m_topTexture );
				UIUtils.UnregisterTexturePropertyNode( m_topTexture );
			}
			else if ( m_inputPorts[ 0 ].Visible )
			{
				UIUtils.RegisterPropertyNode( m_topTexture );
				UIUtils.RegisterTexturePropertyNode( m_topTexture );
			}

			if ( m_inputPorts[ 1 ].IsConnected || m_selectedTriplanarType == TriplanarType.Spherical )
			{
				UIUtils.UnregisterPropertyNode( m_midTexture );
				UIUtils.UnregisterTexturePropertyNode( m_midTexture );
			}
			else if( m_inputPorts[ 1 ].Visible && m_selectedTriplanarType == TriplanarType.Cylindrical )
			{
				UIUtils.RegisterPropertyNode( m_midTexture );
				UIUtils.RegisterTexturePropertyNode( m_midTexture );
			}

			if ( m_inputPorts[ 2 ].IsConnected || m_selectedTriplanarType == TriplanarType.Spherical )
			{
				UIUtils.UnregisterPropertyNode( m_botTexture );
				UIUtils.UnregisterTexturePropertyNode( m_botTexture );
			}
			else if ( m_inputPorts[ 2 ].Visible && m_selectedTriplanarType == TriplanarType.Cylindrical )
			{
				UIUtils.RegisterPropertyNode( m_botTexture );
				UIUtils.RegisterTexturePropertyNode( m_botTexture );
			}
		}

		public void ConfigurePorts()
		{
			switch ( m_selectedTriplanarType )
			{
				case TriplanarType.Spherical:
				InputPorts[ 0 ].Name = "Tex";
				InputPorts[ 1 ].Visible = false;
				InputPorts[ 2 ].Visible = false;
				break;
				case TriplanarType.Cylindrical:
				InputPorts[ 0 ].Name = "Top";
				InputPorts[ 1 ].Visible = true;
				InputPorts[ 2 ].Visible = true;
				break;
			}

			if ( m_normalCorrection )
			{
				m_outputPorts[ 0 ].ChangeProperties( "XYZ", WirePortDataType.FLOAT3, false );
				m_outputPorts[ 1 ].ChangeProperties( "X", WirePortDataType.FLOAT, false );
				m_outputPorts[ 2 ].ChangeProperties( "Y", WirePortDataType.FLOAT, false );
				m_outputPorts[ 3 ].ChangeProperties( "Z", WirePortDataType.FLOAT, false );

				m_outputPorts[ 4 ].Visible = false;
			} else
			{
				m_outputPorts[ 0 ].ChangeProperties( "RGBA", WirePortDataType.FLOAT4, false );
				m_outputPorts[ 1 ].ChangeProperties( "R", WirePortDataType.FLOAT, false );
				m_outputPorts[ 2 ].ChangeProperties( "G", WirePortDataType.FLOAT, false );
				m_outputPorts[ 3 ].ChangeProperties( "B", WirePortDataType.FLOAT, false );
				m_outputPorts[ 4 ].ChangeProperties( "A", WirePortDataType.FLOAT, false );
				
				m_outputPorts[ 4 ].Visible = true;
			}
			m_outputPorts[ 0 ].DirtyLabelSize = true;
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData , ref dataCollector );
			dataCollector.DirtyNormal = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, "Parameters", DrawMainOptions);
			DrawInternalDataGroup();
			if ( m_selectedTriplanarType == TriplanarType.Spherical )
				NodeUtils.DrawPropertyGroup( ref m_topTextureFoldout, "Texture", DrawTopTextureOptions );
			else
				NodeUtils.DrawPropertyGroup( ref m_topTextureFoldout, "Top Texture", DrawTopTextureOptions );

			if ( m_selectedTriplanarType == TriplanarType.Cylindrical )
			{
				NodeUtils.DrawPropertyGroup( ref m_midTextureFoldout, "Middle Texture", DrawMidTextureOptions );
				NodeUtils.DrawPropertyGroup( ref m_botTextureFoldout, "Bottom Texture", DrawBotTextureOptions );
			}
		}

		void DrawMainOptions()
		{
			EditorGUI.BeginChangeCheck();
			m_propertyInspectorName = EditorGUILayoutTextField( "Name", m_propertyInspectorName );

			m_selectedTriplanarType = ( TriplanarType ) EditorGUILayoutEnumPopup( "Mapping", m_selectedTriplanarType );

			m_selectedTriplanarSpace = ( TriplanarSpace ) EditorGUILayoutEnumPopup( "Space", m_selectedTriplanarSpace );

			m_normalCorrection = EditorGUILayoutToggle( "Normal Map", m_normalCorrection );
			if ( EditorGUI.EndChangeCheck() )
			{
				SetTitleText( m_propertyInspectorName );
				ConfigurePorts();
				ReRegisterPorts();
			}
		}

		void DrawTopTextureOptions()
		{
			EditorGUI.BeginChangeCheck();
			m_topTexture.ShowPropertyInspectorNameGUI();
			m_topTexture.ShowPropertyNameGUI( true );
			m_topTexture.ShowToolbar();
			if ( EditorGUI.EndChangeCheck() )
			{
				m_topTexture.BeginPropertyFromInspectorCheck();
				if ( m_materialMode )
					m_requireMaterialUpdate = true;
			}

			m_topTexture.CheckPropertyFromInspector();
		}

		void DrawMidTextureOptions()
		{
			if ( m_midTexture == null )
				return;
			EditorGUI.BeginChangeCheck();
			m_midTexture.ShowPropertyInspectorNameGUI();
			m_midTexture.ShowPropertyNameGUI( true );
			m_midTexture.ShowToolbar();
			if ( EditorGUI.EndChangeCheck() )
			{
				m_midTexture.BeginPropertyFromInspectorCheck();
				if ( m_materialMode )
					m_requireMaterialUpdate = true;
			}

			m_midTexture.CheckPropertyFromInspector();
		}

		void DrawBotTextureOptions()
		{
			if ( m_botTexture == null )
				return;

			EditorGUI.BeginChangeCheck();
			m_botTexture.ShowPropertyInspectorNameGUI();
			m_botTexture.ShowPropertyNameGUI( true );
			m_botTexture.ShowToolbar();
			if ( EditorGUI.EndChangeCheck() )
			{
				m_botTexture.BeginPropertyFromInspectorCheck();
				if ( m_materialMode )
					m_requireMaterialUpdate = true;
			}

			m_botTexture.CheckPropertyFromInspector();
		}

		public override void OnEnable()
		{
			base.OnEnable();
			if ( !m_afterDeserialize )
				Init(); //Generate texture properties
			else
				m_afterDeserialize = false;

			//if ( m_topTexture != null )
			//	m_topTexture.ReRegisterName = true;

			//if(m_selectedTriplanarType == TriplanarType.Cylindrical )
			//{
			//	if ( m_midTexture != null )
			//		m_midTexture.ReRegisterName = true;

			//	if ( m_botTexture != null )
			//		m_botTexture.ReRegisterName = true;
			//}
		}

		bool m_afterDeserialize = false;

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			m_afterDeserialize = true;
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			if ( m_topTexture.ReRegisterName )
			{
				m_topTexture.ReRegisterName = false;
				UIUtils.RegisterUniformName( UniqueId, m_topTexture.PropertyName );
			}

			m_topTexture.CheckDelayedDirtyProperty();
			m_topTexture.CheckPropertyFromInspector();

			if ( m_selectedTriplanarType == TriplanarType.Cylindrical )
			{
				if ( m_midTexture.ReRegisterName )
				{
					m_midTexture.ReRegisterName = false;
					UIUtils.RegisterUniformName( UniqueId, m_midTexture.PropertyName );
				}

				m_midTexture.CheckDelayedDirtyProperty();
				m_midTexture.CheckPropertyFromInspector();

				if ( m_botTexture.ReRegisterName )
				{
					m_botTexture.ReRegisterName = false;
					UIUtils.RegisterUniformName( UniqueId, m_botTexture.PropertyName );
				}

				m_botTexture.CheckDelayedDirtyProperty();
				m_botTexture.CheckPropertyFromInspector();
			}

			m_allPicker = m_previewRect;
			m_allPicker.x -= 43 * drawInfo.InvertedZoom;
			m_allPicker.width = 43 * drawInfo.InvertedZoom;

			m_startPicker = m_previewRect;
			m_startPicker.x -= 43 * drawInfo.InvertedZoom;
			m_startPicker.width = 43 * drawInfo.InvertedZoom;
			m_startPicker.height = 43 * drawInfo.InvertedZoom;

			m_pickerButton = m_startPicker;
			m_pickerButton.width = 30 * drawInfo.InvertedZoom;
			m_pickerButton.x = m_startPicker.xMax - m_pickerButton.width - 2;
			m_pickerButton.height = 10 * drawInfo.InvertedZoom;
			m_pickerButton.y = m_startPicker.yMax - m_pickerButton.height - 2;
		}

		private Rect m_allPicker;
		private Rect m_startPicker;
		private Rect m_pickerButton;
		private bool m_editing;

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if ( !( drawInfo.CurrentEventType == EventType.MouseDown || drawInfo.CurrentEventType == EventType.MouseUp || drawInfo.CurrentEventType == EventType.ExecuteCommand || drawInfo.CurrentEventType == EventType.DragPerform ) )
				return;

			bool insideBox = m_allPicker.Contains( drawInfo.MousePosition );

			if ( insideBox )
			{
				m_editing = true;
			}
			else if ( m_editing && !insideBox && drawInfo.CurrentEventType != EventType.ExecuteCommand )
			{
				GUI.FocusControl( null );
				m_editing = false;
			}
		}
		private int m_pickId = 0;
		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			Rect pickerButtonClone = m_pickerButton;
			Rect startPickerClone = m_startPicker;

			if ( m_editing )
			{
				if ( GUI.Button( pickerButtonClone, string.Empty, GUIStyle.none ) )
				{
					int controlID = EditorGUIUtility.GetControlID( FocusType.Passive );
					EditorGUIUtility.ShowObjectPicker<Texture2D>( m_topTexture.Value, false, "", controlID );
					m_pickId = 0;
				}

				if( m_selectedTriplanarType == TriplanarType.Cylindrical )
				{
					pickerButtonClone.y += startPickerClone.height;
					if ( GUI.Button( pickerButtonClone, string.Empty, GUIStyle.none ) )
					{
						int controlID = EditorGUIUtility.GetControlID( FocusType.Passive );
						EditorGUIUtility.ShowObjectPicker<Texture2D>( m_midTexture.Value, false, "", controlID );
						m_pickId = 1;
					}

					pickerButtonClone.y += startPickerClone.height;
					if ( GUI.Button( pickerButtonClone, string.Empty, GUIStyle.none ) )
					{
						int controlID = EditorGUIUtility.GetControlID( FocusType.Passive );
						EditorGUIUtility.ShowObjectPicker<Texture2D>( m_botTexture.Value, false, "", controlID );
						m_pickId = 2;
					}
				}

				string commandName = Event.current.commandName;
				UnityEngine.Object newValue = null;
				if ( commandName.Equals( "ObjectSelectorUpdated" ) || commandName.Equals( "ObjectSelectorClosed" ) )
				{
					newValue = EditorGUIUtility.GetObjectPickerObject();
					if ( m_pickId == 2 )
					{
						if ( newValue != ( UnityEngine.Object ) m_botTexture.Value )
						{
							UndoRecordObject( this, "Changing value EditorGUIObjectField on node Triplanar Node" );
							m_botTexture.Value = newValue != null ? ( Texture2D ) newValue : null;

							if ( m_materialMode )
								m_requireMaterialUpdate = true;
						}
					} else if ( m_pickId == 1 )
					{
						if ( newValue != ( UnityEngine.Object ) m_midTexture.Value )
						{
							UndoRecordObject( this, "Changing value EditorGUIObjectField on node Triplanar Node" );
							m_midTexture.Value = newValue != null ? ( Texture2D ) newValue : null;

							if ( m_materialMode )
								m_requireMaterialUpdate = true;
						}
					} else
					{
						if ( newValue != ( UnityEngine.Object ) m_topTexture.Value )
						{
							UndoRecordObject( this, "Changing value EditorGUIObjectField on node Triplanar Node" );
							m_topTexture.Value = newValue != null ? ( Texture2D ) newValue : null;

							if ( m_materialMode )
								m_requireMaterialUpdate = true;
						}
					}

					if ( commandName.Equals( "ObjectSelectorClosed" ) )
						m_editing = false;
				}

				if ( GUI.Button( startPickerClone, string.Empty, GUIStyle.none ) )
				{
					if ( m_inputPorts[ 0 ].IsConnected )
					{
						UIUtils.FocusOnNode( m_inputPorts[ 0 ].GetOutputNode( 0 ), 1, true );
					}
					else
					{
						if ( m_topTexture.Value != null )
						{
							Selection.activeObject = m_topTexture.Value;
							EditorGUIUtility.PingObject( Selection.activeObject );
						}
					}
					m_editing = false;
				}

				if ( m_selectedTriplanarType == TriplanarType.Cylindrical )
				{
					startPickerClone.y += startPickerClone.height;
					if ( GUI.Button( startPickerClone, string.Empty, GUIStyle.none ) )
					{
						if ( m_inputPorts[ 1 ].IsConnected )
						{
							UIUtils.FocusOnNode( m_inputPorts[ 1 ].GetOutputNode( 0 ), 1, true );
						}
						else
						{
							if ( m_midTexture.Value != null )
							{
								Selection.activeObject = m_midTexture.Value;
								EditorGUIUtility.PingObject( Selection.activeObject );
							}
						}
						m_editing = false;
					}

					startPickerClone.y += startPickerClone.height;
					if ( GUI.Button( startPickerClone, string.Empty, GUIStyle.none ) )
					{
						if ( m_inputPorts[ 2 ].IsConnected )
						{
							UIUtils.FocusOnNode( m_inputPorts[ 2 ].GetOutputNode( 0 ), 1, true );
						}
						else
						{
							if ( m_botTexture.Value != null )
							{
								Selection.activeObject = m_botTexture.Value;
								EditorGUIUtility.PingObject( Selection.activeObject );
							}
						}
						m_editing = false;
					}
				}
			}

			pickerButtonClone = m_pickerButton;
			startPickerClone = m_startPicker;

			if ( drawInfo.CurrentEventType == EventType.Repaint )
			{
				// Top
				if ( m_inputPorts[ 0 ].IsConnected )
				{
					EditorGUI.DrawPreviewTexture( startPickerClone, m_inputPorts[ 0 ].GetOutputConnection( 0 ).OutputPreviewTexture, null, ScaleMode.ScaleAndCrop );
				}
				else if ( m_topTexture.Value != null )
				{
					EditorGUI.DrawPreviewTexture( startPickerClone, m_topTexture.Value, null, ScaleMode.ScaleAndCrop );
					if ( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
						GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
				}
				else
				{
					GUI.Label( startPickerClone, string.Empty, UIUtils.ObjectFieldThumb );
					if ( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
					{
						GUI.Label( startPickerClone, "None (Texture2D)", UIUtils.MiniObjectFieldThumbOverlay );
						GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
					}
				}
				GUI.Label( startPickerClone, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );

				if ( m_selectedTriplanarType == TriplanarType.Cylindrical )
				{
					// Mid
					startPickerClone.y += startPickerClone.height;
					pickerButtonClone.y += startPickerClone.height;
					if ( m_inputPorts[ 1 ].IsConnected )
					{
						EditorGUI.DrawPreviewTexture( startPickerClone, m_inputPorts[ 1 ].GetOutputConnection( 0 ).OutputPreviewTexture, null, ScaleMode.ScaleAndCrop );
					}
					else if ( m_midTexture.Value != null )
					{
						EditorGUI.DrawPreviewTexture( startPickerClone, m_midTexture.Value, null, ScaleMode.ScaleAndCrop );
						if ( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
							GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
					}
					else
					{
						GUI.Label( startPickerClone, string.Empty, UIUtils.ObjectFieldThumb );
						if ( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
						{
							GUI.Label( startPickerClone, "None (Texture2D)", UIUtils.MiniObjectFieldThumbOverlay );
							GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
						}
					}
					GUI.Label( startPickerClone, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );

					// Bot
					startPickerClone.y += startPickerClone.height;
					startPickerClone.height = 42 * drawInfo.InvertedZoom;
					pickerButtonClone.y += startPickerClone.height;
					if ( m_inputPorts[ 2 ].IsConnected )
					{
						EditorGUI.DrawPreviewTexture( startPickerClone, m_inputPorts[ 2 ].GetOutputConnection( 0 ).OutputPreviewTexture, null, ScaleMode.ScaleAndCrop );
					}
					else if ( m_botTexture.Value != null )
					{
						EditorGUI.DrawPreviewTexture( startPickerClone, m_botTexture.Value, null, ScaleMode.ScaleAndCrop );
						if ( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
							GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
					}
					else
					{
						GUI.Label( startPickerClone, string.Empty, UIUtils.ObjectFieldThumb );
						if ( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
						{
							GUI.Label( startPickerClone, "None (Texture2D)", UIUtils.MiniObjectFieldThumbOverlay );
							GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
						}
					}
					GUI.Label( startPickerClone, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
				}
			}
		}

		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if ( currentMousePos2D.y - m_globalPosition.y > Constants.NODE_HEADER_HEIGHT + Constants.NODE_HEADER_EXTRA_HEIGHT )
			{
				ContainerGraph.ParentWindow.ParametersWindow.IsMaximized = !ContainerGraph.ParentWindow.ParametersWindow.IsMaximized;
			}
			else
			{
				m_editPropertyNameMode = true;
				GUI.FocusControl( m_uniqueName );
				TextEditor te = ( TextEditor )GUIUtility.GetStateObject( typeof( TextEditor ), GUIUtility.keyboardControl );
				if ( te != null )
				{
					te.SelectAll();
				}
			}
		}

		public override void OnNodeSelected( bool value )
		{
			base.OnNodeSelected( value );
			if ( !value )
				m_editPropertyNameMode = false;
		}

		public override void DrawTitle( Rect titlePos )
		{
			if ( m_editPropertyNameMode )
			{
				titlePos.height = Constants.NODE_HEADER_HEIGHT;
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName( m_uniqueName );
				m_propertyInspectorName = GUITextField( titlePos, m_propertyInspectorName, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
				if ( EditorGUI.EndChangeCheck() )
				{
					SetTitleText( m_propertyInspectorName );
				}

				if ( Event.current.isKey && ( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter ) )
				{
					m_editPropertyNameMode = false;
					GUIUtility.keyboardControl = 0;
				}
			}
			else
			{
				base.DrawTitle( titlePos );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			//ConfigureFunctions();
			dataCollector.AddPropertyNode( m_topTexture );
			dataCollector.AddPropertyNode( m_midTexture );
			dataCollector.AddPropertyNode( m_botTexture );

			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation || dataCollector.PortCategory == MasterNodePortCategory.Vertex );

			string texTop = string.Empty;
			string texMid = string.Empty;
			string texBot = string.Empty;

			if ( m_inputPorts[ 0 ].IsConnected )
			{
				texTop = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			}
			else
			{
				dataCollector.AddToUniforms( UniqueId, m_topTexture.GetTexture2DUniformValue() );
				dataCollector.AddToProperties( UniqueId, m_topTexture.GetTexture2DPropertyValue(), m_topTexture.OrderIndex );
				texTop = m_topTexture.PropertyName;
			}

			if ( m_selectedTriplanarType == TriplanarType.Spherical )
			{
				texMid = texTop;
				texBot = texTop;
			}
			else
			{
				if ( m_inputPorts[ 1 ].IsConnected )
				{
					texMid = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
				}
				else
				{
					dataCollector.AddToUniforms( UniqueId, m_midTexture.GetTexture2DUniformValue() );
					dataCollector.AddToProperties( UniqueId, m_midTexture.GetTexture2DPropertyValue(), m_midTexture.OrderIndex );
					texMid = m_midTexture.PropertyName;
				}

				if ( m_inputPorts[ 2 ].IsConnected )
				{
					texBot = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
				}
				else
				{
					dataCollector.AddToUniforms( UniqueId, m_botTexture.GetTexture2DUniformValue() );
					dataCollector.AddToProperties( UniqueId, m_botTexture.GetTexture2DPropertyValue(), m_botTexture.OrderIndex );
					texBot = m_botTexture.PropertyName;
				}
			}

			dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_POS ), true );
			dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
			string tilling = m_inputPorts[ 3 ].GeneratePortInstructions( ref dataCollector );
			string falloff = m_inputPorts[ 4 ].GeneratePortInstructions( ref dataCollector );
			dataCollector.ForceNormal = true;

			dataCollector.AddToInput( UniqueId, Constants.InternalData, false );

			if ( m_normalCorrection )
			{
				string worldToTangent = GeneratorUtils.GenerateWorldToTangentMatrix( ref dataCollector, UniqueId, m_currentPrecisionType );

				string pos = GeneratorUtils.GenerateWorldPosition( ref dataCollector, UniqueId );
				string norm = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
				if ( m_selectedTriplanarSpace == TriplanarSpace.Object )
				{

					dataCollector.AddLocalVariable( UniqueId, "float3 localTangent = mul( unity_WorldToObject, float4( " + GeneratorUtils.WorldTangentStr + ", 0 ) );" );
					dataCollector.AddLocalVariable( UniqueId, "float3 localBitangent = mul( unity_WorldToObject, float4( " + GeneratorUtils.WorldBitangentStr + ", 0 ) );" );
					dataCollector.AddLocalVariable( UniqueId, "float3 localNormal = mul( unity_WorldToObject, float4( "+ GeneratorUtils.WorldNormalStr + ", 0 ) );" );
					norm = "localNormal";
					dataCollector.AddLocalVariable( UniqueId, "float3x3 objectToTangent = float3x3(localTangent, localBitangent, localNormal);" );
					dataCollector.AddLocalVariable( UniqueId, "float3 localPos = mul( unity_WorldToObject, float4( " + pos + ", 1 ) );" );
					pos = "localPos";
					worldToTangent = "objectToTangent";
				}

				string normalTriplanar = string.Empty;
				IOUtils.AddFunctionHeader( ref normalTriplanar, m_functionNormalHeader );
				if ( m_selectedTriplanarType == TriplanarType.Spherical )
				{
					for ( int i = 0; i < m_functionNormalBody.Length; i++ )
					{
						IOUtils.AddFunctionLine( ref normalTriplanar, m_functionNormalBody[ i ] );
					}
				}
				else
				{
					for ( int i = 0; i < m_functionNormalBodyTMB.Length; i++ )
					{
						IOUtils.AddFunctionLine( ref normalTriplanar, m_functionNormalBodyTMB[ i ] );
					}
				}
				IOUtils.CloseFunctionBody( ref normalTriplanar );

				string call = dataCollector.AddFunctions( m_functionNormalCall, normalTriplanar, texTop, texMid, texBot, pos, norm, falloff, tilling, ( isVertex ? "1" : "0" ) );
				dataCollector.AddToLocalVariables( dataCollector.PortCategory, UniqueId, "float3 worldTriplanarNormal" + OutputId + " = " + call + ";" );
				dataCollector.AddToLocalVariables( dataCollector.PortCategory, UniqueId, "float3 tanTriplanarNormal" + OutputId + " = mul( " + worldToTangent + ", worldTriplanarNormal" + OutputId + " );" );
				return GetOutputVectorItem( 0, outputId, "tanTriplanarNormal" + OutputId );
			} else
			{
				string samplingTriplanar = string.Empty;
				IOUtils.AddFunctionHeader( ref samplingTriplanar, m_functionSamplingHeader );
				if(m_selectedTriplanarType == TriplanarType.Spherical )
				{
					for ( int i = 0; i < m_functionSamplingBody.Length; i++ )
					{
						IOUtils.AddFunctionLine( ref samplingTriplanar, m_functionSamplingBody[ i ] );
					}
				}
				else
				{
					for ( int i = 0; i < m_functionSamplingBodyTMB.Length; i++ )
					{
						IOUtils.AddFunctionLine( ref samplingTriplanar, m_functionSamplingBodyTMB[ i ] );
					}
				}
				IOUtils.CloseFunctionBody( ref samplingTriplanar );

				string pos = GeneratorUtils.GenerateWorldPosition( ref dataCollector, UniqueId );
				string norm = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
				if (m_selectedTriplanarSpace == TriplanarSpace.Object )
				{
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, UniqueId, "float3 localPos = mul( unity_WorldToObject, float4( " + pos + ", 1 ) );" );
					pos = "localPos";
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, UniqueId, "float3 localNormal = mul( unity_WorldToObject, float4( " + norm + ", 0 ) );" );
					norm = "localNormal";
				}

				string call = dataCollector.AddFunctions( m_functionSamplingCall, samplingTriplanar, texTop, texMid, texBot, pos, norm, falloff, tilling, ( isVertex ? "1" : "0") );
				dataCollector.AddToLocalVariables( dataCollector.PortCategory, UniqueId, "float4 triplanar" + OutputId + " = " + call + ";" );
				return GetOutputVectorItem( 0, outputId, "triplanar" + OutputId );
			}
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			m_topTexture.OnPropertyNameChanged();
			if ( mat.HasProperty( m_topTexture.PropertyName ) )
			{
				mat.SetTexture( m_topTexture.PropertyName, m_topTexture.MaterialValue );
			}

			m_midTexture.OnPropertyNameChanged();
			if ( mat.HasProperty( m_midTexture.PropertyName ) )
			{
				mat.SetTexture( m_midTexture.PropertyName, m_midTexture.MaterialValue );
			}

			m_botTexture.OnPropertyNameChanged();
			if ( mat.HasProperty( m_botTexture.PropertyName ) )
			{
				mat.SetTexture( m_botTexture.PropertyName, m_botTexture.MaterialValue );
			}
		}

		public void SetDelayedMaterialMode( Material mat )
		{
			if ( mat.HasProperty( m_topTexture.PropertyName ) )
			{
				m_topTexture.MaterialValue = mat.GetTexture( m_topTexture.PropertyName );
			}

			if ( mat.HasProperty( m_midTexture.PropertyName ) )
			{
				m_midTexture.MaterialValue = mat.GetTexture( m_midTexture.PropertyName );
			}

			if ( mat.HasProperty( m_botTexture.PropertyName ) )
			{
				m_botTexture.MaterialValue = mat.GetTexture( m_botTexture.PropertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			base.ForceUpdateFromMaterial( material );
			if ( material.HasProperty( m_topTexture.PropertyName ) )
			{
				m_topTexture.MaterialValue = material.GetTexture( m_topTexture.PropertyName );
			}

			if ( material.HasProperty( m_midTexture.PropertyName ) )
			{
				m_midTexture.MaterialValue = material.GetTexture( m_midTexture.PropertyName );
			}

			if ( material.HasProperty( m_botTexture.PropertyName ) )
			{
				m_botTexture.MaterialValue = material.GetTexture( m_botTexture.PropertyName );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedTriplanarType = ( TriplanarType )Enum.Parse( typeof( TriplanarType ), GetCurrentParam( ref nodeParams ) );
			m_selectedTriplanarSpace = ( TriplanarSpace )Enum.Parse( typeof( TriplanarSpace ), GetCurrentParam( ref nodeParams ) );
			m_normalCorrection = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			m_tempTopInspectorName = GetCurrentParam( ref nodeParams );
			m_tempTopName = GetCurrentParam( ref nodeParams );
			m_tempTopDefaultValue = ( TexturePropertyValues )Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_tempTopOrderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_tempTopDefaultTexture = AssetDatabase.LoadAssetAtPath<Texture2D>( GetCurrentParam( ref nodeParams ) );

			m_tempMidInspectorName = GetCurrentParam( ref nodeParams );
			m_tempMidName = GetCurrentParam( ref nodeParams );
			m_tempMidDefaultValue = ( TexturePropertyValues )Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_tempMidOrderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_tempMidDefaultTexture = AssetDatabase.LoadAssetAtPath<Texture2D>( GetCurrentParam( ref nodeParams ) );

			m_tempBotInspectorName = GetCurrentParam( ref nodeParams );
			m_tempBotName = GetCurrentParam( ref nodeParams );
			m_tempBotDefaultValue = ( TexturePropertyValues )Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_tempBotOrderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_tempBotDefaultTexture = AssetDatabase.LoadAssetAtPath<Texture2D>( GetCurrentParam( ref nodeParams ) );

			if ( UIUtils.CurrentShaderVersion() > 6102 )
				m_propertyInspectorName = GetCurrentParam( ref nodeParams );
			SetTitleText( m_propertyInspectorName );

			ConfigurePorts();
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			ReadPropertiesData();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedTriplanarType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedTriplanarSpace );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalCorrection );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_topTexture.PropertyInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_topTexture.PropertyName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_topTexture.DefaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_topTexture.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_topTexture.DefaultValue != null ) ? AssetDatabase.GetAssetPath( m_topTexture.DefaultValue ) : Constants.NoStringValue );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_midTexture.PropertyInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_midTexture.PropertyName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_midTexture.DefaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_midTexture.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_midTexture.DefaultValue != null ) ? AssetDatabase.GetAssetPath( m_midTexture.DefaultValue ) : Constants.NoStringValue );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_botTexture.PropertyInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_botTexture.PropertyName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_botTexture.DefaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_botTexture.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_botTexture.DefaultValue != null ) ? AssetDatabase.GetAssetPath( m_botTexture.DefaultValue ) : Constants.NoStringValue );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_propertyInspectorName );
		}
		public override void RefreshOnUndo()
		{
			base.RefreshOnUndo();
			if ( m_topTexture != null )
			{
				m_topTexture.BeginPropertyFromInspectorCheck();
			}

			if ( m_midTexture != null )
			{
				m_midTexture.BeginPropertyFromInspectorCheck();
			}

			if ( m_botTexture != null )
			{
				m_botTexture.BeginPropertyFromInspectorCheck();
			}
		}
	}
}
