// Amplify Impostors
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#if AMPLIFY_SHADER_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

public static class AmplifyASEHelper
{
	public class R_RangedFloatNode
	{
		private AmplifyShaderEditor.RangedFloatNode m_instance;
		private System.Type m_type;

		public R_RangedFloatNode( AmplifyShaderEditor.RangedFloatNode instance )
		{
			m_instance = instance;
			m_type = instance.GetType();
		}

		public float Max
		{
			set
			{
				var field = m_type.GetField( "m_max", BindingFlags.Instance | BindingFlags.NonPublic );
				field.SetValue( m_instance, value );
			}
		}
	}

	public class R_PropertyNode
	{
		private AmplifyShaderEditor.PropertyNode m_instance;
		private System.Type m_type;

		public R_PropertyNode( AmplifyShaderEditor.PropertyNode instance )
		{
			m_instance = instance;
			m_type = instance.GetType();
		}

		public List<int> SelAttr
		{
			get
			{
				var field = m_type.GetField( "m_selectedAttribs", BindingFlags.Instance | BindingFlags.NonPublic );
				return (List<int>)field.GetValue( m_instance );
			}
			set
			{
				var field = m_type.GetField( "m_selectedAttribs", BindingFlags.Instance | BindingFlags.NonPublic );
				field.SetValue( m_instance, value );
			}
		}

		public string InspectorName
		{
			get
			{
				var field = m_type.GetField( "m_propertyInspectorName", BindingFlags.Instance | BindingFlags.NonPublic );
				return (string)field.GetValue( m_instance );
			}
			set
			{
				var field = m_type.GetField( "m_propertyInspectorName", BindingFlags.Instance | BindingFlags.NonPublic );
				field.SetValue( m_instance, value );
			}
		}
	}
}

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Amplify Impostor", "Miscellaneous", "Impostor" )]
	public sealed class AmplifyImpostorNode : ParentNode
	{
		private string m_functionHeaderSphereFrag = "SphereImpostorFragment( o, clipPos, worldPos, {0}, {1} )";
		private string m_functionHeaderSphere = "SphereImpostorVertex( {0}, {1}, {2} )";
		private string m_functionHeaderFrag = "OctaImpostorFragment( o, clipPos, worldPos, {0}, {1}, {2}, {3}, {4} )";
		private string m_functionHeader = "OctaImpostorVertex( {0}, {1}, {2}, {3}, {4}, {5} )";
		private string m_functionBody = string.Empty;

		private enum CustomImpostorType
		{
			Spherical = 0,
			Octahedron = 1,
			HemiOctahedron = 2
		}

		private const string WorkflowStr = "Workflow";

		[SerializeField]
		private ASEStandardSurfaceWorkflow m_workflow = ASEStandardSurfaceWorkflow.Specular;

		[SerializeField]
		private CustomImpostorType m_customImpostorType = CustomImpostorType.Octahedron;

		[SerializeField]
		private bool m_speedTreeHueSupport;

		[SerializeField]
		private bool m_showExtraData;

		[SerializeField]
		private RangedFloatNode m_framesProp;
		private int m_orderFramesProp = -1;

		[SerializeField]
		private RangedFloatNode m_framesXProp;
		private int m_orderFramesXProp = -1;

		[SerializeField]
		private RangedFloatNode m_framesYProp;
		private int m_orderFramesYProp = -1;

		[SerializeField]
		private RangedFloatNode m_sizeProp;
		private int m_orderSizeProp = -1;

		[SerializeField]
		private RangedFloatNode m_parallaxProp;
		private int m_orderParallaxProp = -1;

		[SerializeField]
		private Vector3Node m_offsetProp;
		private int m_orderOffsetProp = -1;

		[SerializeField]
		private RangedFloatNode m_biasProp;
		private int m_orderBiasProp = -1;

		[SerializeField]
		private TexturePropertyNode m_albedoTexture;
		private int m_orderAlbedoTexture = -1;

		[SerializeField]
		private TexturePropertyNode m_normalTexture;
		private int m_orderNormalTexture = -1;

		[SerializeField]
		private TexturePropertyNode m_specularTexture;
		private int m_orderSpecularTexture = -1;

		[SerializeField]
		private TexturePropertyNode m_emissionTexture;
		private int m_orderEmissionTexture = -1;

		[SerializeField]
		private RangedFloatNode m_depthProp;
		private int m_orderDepthProp = -1;

		[SerializeField]
		private RangedFloatNode m_shadowBiasProp;
		private int m_orderShadowBiasProp = -1;

		[SerializeField]
		private RangedFloatNode m_clipProp;
		private int m_orderClipProp = -1;

		[SerializeField]
		private ColorNode m_hueProp;
		private int m_orderHueProp = -1;

		[SerializeField]
		private int m_extraSamplers = 0;

		private const int MaxExtraSamplers = 8;

		private string[] m_extraPropertyNames;

		bool m_propertiesInitialize = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );

			for( int i = 0; i < MaxExtraSamplers; i++ )
			{
				AddInputPort( WirePortDataType.SAMPLER2D, true, "Tex" + i );
				AddOutputPort( WirePortDataType.FLOAT4, "Tex" + i, i + 8 );
			}

			AddOutputPort( WirePortDataType.FLOAT3, "Albedo", 0 );
			AddOutputPort( WirePortDataType.FLOAT3, "World Normal", 1 );
			AddOutputPort( WirePortDataType.FLOAT3, "Emission", 2 );
			AddOutputPort( WirePortDataType.FLOAT3, "Specular", 3 );
			AddOutputPort( WirePortDataType.FLOAT, "Smoothness", 4 );
			AddOutputPort( WirePortDataType.FLOAT, "Occlusion", 5 );
			AddOutputPort( WirePortDataType.FLOAT, "Alpha", 6 );
			AddOutputPort( WirePortDataType.FLOAT3, "World Pos", 7 );
			AddOutputPort( WirePortDataType.FLOAT3, "(VS) View Pos", 16 );

			m_autoWrapProperties = true;
			m_textLabelWidth = 160;

			UpdateTitle();
			UpdatePorts();
			UpdateInputPorts();
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();
			UpdateTag();
		}

		void UpdateTag()
		{
			List<CustomTagData> allTags = null;
			if( VersionInfo.FullNumber > 15500 )
			{
				allTags = ( (TemplateMultiPassMasterNode)m_containerGraph.CurrentMasterNode ).SubShaderModule.TagsHelper.AvailableTags;
			}
			else
			{
				allTags = ( m_containerGraph.MultiPassMasterNodes.NodesList[ m_containerGraph.MultiPassMasterNodes.Count - 1 ] ).SubShaderModule.TagsHelper.AvailableTags;
			}

			CustomTagData importorTag = allTags.Find( x => x.TagName == "ImpostorType" );
			if( importorTag != null )
				importorTag.TagValue = m_customImpostorType.ToString();
			else
				allTags.Add( new CustomTagData( "ImpostorType", m_customImpostorType.ToString(), 0 ) );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_customImpostorType = (CustomImpostorType)EditorGUILayoutEnumPopup( "Impostor Type", m_customImpostorType );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateTitle();
				UpdateTag();
			}

			EditorGUI.BeginChangeCheck();
			m_workflow = (ASEStandardSurfaceWorkflow)EditorGUILayoutEnumPopup( WorkflowStr, m_workflow );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
			}

			m_speedTreeHueSupport = EditorGUILayoutToggle( "SpeedTree Hue Support", m_speedTreeHueSupport );
			EditorGUI.BeginChangeCheck();
			m_showExtraData = EditorGUILayoutToggle( "Output Extra Data", m_showExtraData );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
			}

			EditorGUI.BeginChangeCheck();
			float cacha = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 120;
			m_extraSamplers = EditorGUILayoutIntSlider( "Extra Samplers", m_extraSamplers, 0, MaxExtraSamplers );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateInputPorts();
			}
			EditorGUIUtility.labelWidth = cacha;
		}

		public void UpdateInputPorts()
		{
			m_extraPropertyNames = new string[ m_extraSamplers ];
			for( int i = 0; i < MaxExtraSamplers; i++ )
			{
				GetOutputPortByArrayId( i ).Visible = i < m_extraSamplers;
				GetInputPortByArrayId( i ).Visible = i < m_extraSamplers;
			}
			m_sizeIsDirty = true;
		}

		public void UpdatePorts()
		{
			if( m_showExtraData )
			{
				GetOutputPortByUniqueId( 7 ).Visible = true;
				GetOutputPortByUniqueId( 16 ).Visible = true;
			}
			else
			{
				GetOutputPortByUniqueId( 7 ).Visible = false;
				GetOutputPortByUniqueId( 16 ).Visible = false;
			}

			if(m_workflow == ASEStandardSurfaceWorkflow.Specular )
			{
				GetOutputPortByUniqueId( 3 ).ChangeProperties( "Specular", WirePortDataType.FLOAT3, false );
			} else
			{
				GetOutputPortByUniqueId( 3 ).ChangeProperties( "Metallic", WirePortDataType.FLOAT, false );
			}
			m_sizeIsDirty = true;
		}

		public void Init()
		{
			if( m_propertiesInitialize )
				return;
			else
				m_propertiesInitialize = true;

			if( m_framesProp == null )
			{
				m_framesProp = ScriptableObject.CreateInstance<RangedFloatNode>();
			}
			m_framesProp.ContainerGraph = ContainerGraph;
			AmplifyASEHelper.R_PropertyNode temp2 = new AmplifyASEHelper.R_PropertyNode( m_framesProp );
			temp2.SelAttr.Add( 0 );
			m_framesProp.OrderIndex = m_orderFramesProp;
			m_framesProp.ChangeParameterType( PropertyType.Property );
			m_framesProp.UniqueId = UniqueId;
			m_framesProp.RegisterPropertyName( true, "AI_Frames" );
			temp2.InspectorName = "Impostor Frames";

			if( m_framesXProp == null )
			{
				m_framesXProp = ScriptableObject.CreateInstance<RangedFloatNode>();
			}
			m_framesXProp.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_framesXProp );
			temp2.SelAttr.Add( 0 );
			m_framesXProp.OrderIndex = m_orderFramesXProp;
			m_framesXProp.ChangeParameterType( PropertyType.Property );
			m_framesXProp.UniqueId = UniqueId;
			m_framesXProp.RegisterPropertyName( true, "AI_Frames X" );
			temp2.InspectorName = "Impostor Frames X";

			if( m_framesYProp == null )
			{
				m_framesYProp = ScriptableObject.CreateInstance<RangedFloatNode>();
			}
			m_framesYProp.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_framesYProp );
			temp2.SelAttr.Add( 0 );
			m_framesYProp.OrderIndex = m_orderFramesYProp;
			m_framesYProp.ChangeParameterType( PropertyType.Property );
			m_framesYProp.UniqueId = UniqueId;
			m_framesYProp.RegisterPropertyName( true, "AI_Frames Y" );
			temp2.InspectorName = "Impostor Frames Y";

			if( m_sizeProp == null )
			{
				m_sizeProp = ScriptableObject.CreateInstance<RangedFloatNode>();
			}
			m_sizeProp.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_sizeProp );
			temp2.SelAttr.Add( 0 );
			m_sizeProp.OrderIndex = m_orderSizeProp;
			m_sizeProp.ChangeParameterType( PropertyType.Property );
			m_sizeProp.UniqueId = UniqueId;
			m_sizeProp.RegisterPropertyName( true, "AI_Impostor Size" );
			temp2.InspectorName = "Impostor Size";

			if( m_parallaxProp == null )
			{
				m_parallaxProp = ScriptableObject.CreateInstance<RangedFloatNode>();
			}
			m_parallaxProp.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_parallaxProp );
			AmplifyASEHelper.R_RangedFloatNode temp = new AmplifyASEHelper.R_RangedFloatNode( m_parallaxProp );
			temp.Max = 1f;
			m_parallaxProp.SetFloatMode( false );
			m_parallaxProp.Value = 1;
			m_parallaxProp.OrderIndex = m_orderParallaxProp;
			m_parallaxProp.ChangeParameterType( PropertyType.Property );
			m_parallaxProp.UniqueId = UniqueId;
			m_parallaxProp.RegisterPropertyName( true, "AI_Parallax" );
			temp2.InspectorName = "Impostor Parallax";

			if( m_offsetProp == null )
			{
				m_offsetProp = ScriptableObject.CreateInstance<Vector3Node>();
			}
			m_offsetProp.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_offsetProp );
			temp2.SelAttr.Add( 0 );
			m_offsetProp.OrderIndex = m_orderOffsetProp;
			m_offsetProp.ChangeParameterType( PropertyType.Property );
			m_offsetProp.UniqueId = UniqueId;
			m_offsetProp.RegisterPropertyName( true, "AI_Offset" );
			temp2.InspectorName = "Impostor Offset";

			if( m_biasProp == null )
			{
				m_biasProp = ScriptableObject.CreateInstance<RangedFloatNode>();
			}
			m_biasProp.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_biasProp );
			m_biasProp.Value = -1;
			m_biasProp.OrderIndex = m_orderBiasProp;
			m_biasProp.ChangeParameterType( PropertyType.Property );
			m_biasProp.UniqueId = UniqueId;
			m_biasProp.RegisterPropertyName( true, "AI_Texture Bias" );
			temp2.InspectorName = "Impostor Texture Bias";

			if( m_albedoTexture == null )
			{
				m_albedoTexture = ScriptableObject.CreateInstance<TexturePropertyNode>();
			}
			m_albedoTexture.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_albedoTexture );
			temp2.SelAttr.Add( 4 );
			m_albedoTexture.OrderIndex = m_orderAlbedoTexture;
			m_albedoTexture.CustomPrefix = "Albedo";
			m_albedoTexture.CurrentParameterType = PropertyType.Property;
			m_albedoTexture.DrawAutocast = false;
			m_albedoTexture.UniqueId = UniqueId;
			m_albedoTexture.RegisterPropertyName( true, "Albedo" );
			temp2.InspectorName = "Impostor Albedo & Alpha";

			if( m_normalTexture == null )
			{
				m_normalTexture = ScriptableObject.CreateInstance<TexturePropertyNode>();
			}
			m_normalTexture.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_normalTexture );
			temp2.SelAttr.Add( 4 );
			m_normalTexture.OrderIndex = m_orderNormalTexture;
			m_normalTexture.CustomPrefix = "Normals";
			m_normalTexture.CurrentParameterType = PropertyType.Property;
			m_normalTexture.DrawAutocast = false;
			m_normalTexture.UniqueId = UniqueId;
			m_normalTexture.RegisterPropertyName( true, "Normals" );
			temp2.InspectorName = "Impostor Normal & Depth";

			if( m_specularTexture == null )
			{
				m_specularTexture = ScriptableObject.CreateInstance<TexturePropertyNode>();
			}
			m_specularTexture.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_specularTexture );
			temp2.SelAttr.Add( 4 );
			m_specularTexture.OrderIndex = m_orderSpecularTexture;
			m_specularTexture.CustomPrefix = "Specular";
			m_specularTexture.DefaultTextureValue = TexturePropertyValues.black;
			m_specularTexture.CurrentParameterType = PropertyType.Property;
			m_specularTexture.DrawAutocast = false;
			m_specularTexture.UniqueId = UniqueId;
			m_specularTexture.RegisterPropertyName( true, "Specular" );
			temp2.InspectorName = "Impostor Specular & Smoothness";

			if( m_emissionTexture == null )
			{
				m_emissionTexture = ScriptableObject.CreateInstance<TexturePropertyNode>();
			}
			m_emissionTexture.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_emissionTexture );
			temp2.SelAttr.Add( 4 );
			m_emissionTexture.OrderIndex = m_orderEmissionTexture;
			m_emissionTexture.CustomPrefix = "Emission";
			m_emissionTexture.DefaultTextureValue = TexturePropertyValues.black;
			m_emissionTexture.CurrentParameterType = PropertyType.Property;
			m_emissionTexture.DrawAutocast = false;
			m_emissionTexture.UniqueId = UniqueId;
			m_emissionTexture.RegisterPropertyName( true, "Emission" );
			temp2.InspectorName = "Impostor Emission & Occlusion";

			if( m_depthProp == null )
			{
				m_depthProp = ScriptableObject.CreateInstance<RangedFloatNode>();
			}
			m_depthProp.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_depthProp );
			temp2.SelAttr.Add( 0 );
			m_depthProp.OrderIndex = m_orderDepthProp;
			m_depthProp.ChangeParameterType( PropertyType.Property );
			m_depthProp.UniqueId = UniqueId;
			m_depthProp.RegisterPropertyName( true, "AI_Depth Size" );
			temp2.InspectorName = "Impostor Depth Size";

			if( m_shadowBiasProp == null )
			{
				m_shadowBiasProp = ScriptableObject.CreateInstance<RangedFloatNode>();
			}
			m_shadowBiasProp.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_shadowBiasProp );
			temp = new AmplifyASEHelper.R_RangedFloatNode( m_shadowBiasProp );
			temp.Max = 2f;
			m_shadowBiasProp.SetFloatMode( false );
			m_shadowBiasProp.Value = 0.25f;
			m_shadowBiasProp.OrderIndex = m_orderShadowBiasProp;
			m_shadowBiasProp.ChangeParameterType( PropertyType.Property );
			m_shadowBiasProp.UniqueId = UniqueId;
			m_shadowBiasProp.RegisterPropertyName( true, "AI_Shadow Bias" );
			temp2.InspectorName = "Impostor Shadow Bias";

			if( m_clipProp == null )
			{
				m_clipProp = ScriptableObject.CreateInstance<RangedFloatNode>();
			}
			m_clipProp.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_clipProp );
			temp = new AmplifyASEHelper.R_RangedFloatNode( m_clipProp );
			temp.Max = 1f;
			m_clipProp.Value = 0.5f;
			m_clipProp.SetFloatMode( false );
			m_clipProp.OrderIndex = m_orderClipProp;
			m_clipProp.ChangeParameterType( PropertyType.Property );
			m_clipProp.UniqueId = UniqueId;
			m_clipProp.RegisterPropertyName( true, "AI_Clip" );
			temp2.InspectorName = "Impostor Clip";

			if( m_hueProp == null )
			{
				m_hueProp = ScriptableObject.CreateInstance<ColorNode>();
			}
			m_hueProp.ContainerGraph = ContainerGraph;
			temp2 = new AmplifyASEHelper.R_PropertyNode( m_hueProp );
			m_hueProp.OrderIndex = m_orderHueProp;
			m_hueProp.ChangeParameterType( PropertyType.Property );
			m_hueProp.UniqueId = UniqueId;
			m_hueProp.RegisterPropertyName( true, "AI_Hue Variation" );
			temp2.InspectorName = "Impostor Hue Variation";
		}

		public override void SetMaterialMode( Material mat, bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat, fetchMaterialValues );

			if( !m_propertiesInitialize )
				return;

			m_framesProp.SetMaterialMode( mat, fetchMaterialValues );
			m_framesXProp.SetMaterialMode( mat, fetchMaterialValues );
			m_framesYProp.SetMaterialMode( mat, fetchMaterialValues );
			m_sizeProp.SetMaterialMode( mat, fetchMaterialValues );
			m_parallaxProp.SetMaterialMode( mat, fetchMaterialValues );
			m_offsetProp.SetMaterialMode( mat, fetchMaterialValues );
			m_biasProp.SetMaterialMode( mat, fetchMaterialValues );
			m_albedoTexture.SetMaterialMode( mat, fetchMaterialValues );
			m_normalTexture.SetMaterialMode( mat, fetchMaterialValues );
			m_specularTexture.SetMaterialMode( mat, fetchMaterialValues );
			m_emissionTexture.SetMaterialMode( mat, fetchMaterialValues );
			m_depthProp.SetMaterialMode( mat, fetchMaterialValues );
			m_shadowBiasProp.SetMaterialMode( mat, fetchMaterialValues );
			m_clipProp.SetMaterialMode( mat, fetchMaterialValues );
			m_hueProp.SetMaterialMode( mat, fetchMaterialValues );
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();

			Init();

			UpdateTitle();
			UpdatePorts();
			UpdateInputPorts();
		}

		void UpdateTitle()
		{
			SetAdditonalTitleText( "Type( " + m_customImpostorType + " )" );

			//List<CustomTagData> allTags = null;
			//if( VersionInfo.FullNumber > 15500 )
			//{
			//	allTags = ( (TemplateMultiPassMasterNode)m_containerGraph.CurrentMasterNode ).SubShaderModule.TagsHelper.AvailableTags;
			//}
			//else
			//{
			//	allTags = ( m_containerGraph.MultiPassMasterNodes.NodesList[ m_containerGraph.MultiPassMasterNodes.Count - 1 ] ).SubShaderModule.TagsHelper.AvailableTags;
			//}

			//CustomTagData importorTag = allTags.Find( x => x.TagName == "ImpostorType" );
			//if( importorTag != null )
			//	importorTag.TagValue = m_customImpostorType.ToString();
			//else
			//	allTags.Add( new CustomTagData( "ImpostorType", m_customImpostorType.ToString(), 0 ) );
		}

		public override void OnNodeLogicUpdate( DrawInfo drawInfo )
		{
			base.OnNodeLogicUpdate( drawInfo );

			Init();
			m_framesProp.OnNodeLogicUpdate( drawInfo );
			m_framesXProp.OnNodeLogicUpdate( drawInfo );
			m_framesYProp.OnNodeLogicUpdate( drawInfo );
			m_sizeProp.OnNodeLogicUpdate( drawInfo );
			m_parallaxProp.OnNodeLogicUpdate( drawInfo );
			m_offsetProp.OnNodeLogicUpdate( drawInfo );
			m_biasProp.OnNodeLogicUpdate( drawInfo );
			m_albedoTexture.OnNodeLogicUpdate( drawInfo );
			m_normalTexture.OnNodeLogicUpdate( drawInfo );
			m_specularTexture.OnNodeLogicUpdate( drawInfo );
			m_emissionTexture.OnNodeLogicUpdate( drawInfo );
			m_depthProp.OnNodeLogicUpdate( drawInfo );
			m_shadowBiasProp.OnNodeLogicUpdate( drawInfo );
			m_clipProp.OnNodeLogicUpdate( drawInfo );
			m_hueProp.OnNodeLogicUpdate( drawInfo );
		}

		public override void Destroy()
		{
			base.Destroy();

			if( m_framesProp != null )
				m_framesProp.Destroy();
			m_framesProp = null;

			if( m_framesXProp != null )
				m_framesXProp.Destroy();
			m_framesXProp = null;

			if( m_framesYProp != null )
				m_framesYProp.Destroy();
			m_framesYProp = null;

			if( m_sizeProp != null )
				m_sizeProp.Destroy();
			m_sizeProp = null;

			if( m_parallaxProp != null )
				m_parallaxProp.Destroy();
			m_parallaxProp = null;

			if( m_offsetProp != null )
				m_offsetProp.Destroy();
			m_offsetProp = null;

			if( m_biasProp != null )
				m_biasProp.Destroy();
			m_biasProp = null;

			if( m_albedoTexture != null )
				m_albedoTexture.Destroy();
			m_albedoTexture = null;

			if( m_normalTexture != null )
				m_normalTexture.Destroy();
			m_normalTexture = null;

			if( m_specularTexture != null )
				m_specularTexture.Destroy();
			m_specularTexture = null;

			if( m_emissionTexture != null )
				m_emissionTexture.Destroy();
			m_emissionTexture = null;

			if( m_depthProp != null )
				m_depthProp.Destroy();
			m_depthProp = null;

			if( m_shadowBiasProp != null )
				m_shadowBiasProp.Destroy();
			m_shadowBiasProp = null;

			if( m_clipProp != null )
				m_clipProp.Destroy();
			m_clipProp = null;

			if( m_hueProp != null )
				m_hueProp.Destroy();
			m_hueProp = null;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			UpdateInputPorts();

			bool hasSpec = true;
			if( !GetOutputPortByUniqueId( 3 ).IsConnected && !GetOutputPortByUniqueId( 4 ).IsConnected )
				hasSpec = false;

			bool hasEmission = true;
			if( !GetOutputPortByUniqueId( 2 ).IsConnected && !GetOutputPortByUniqueId( 5 ).IsConnected )
				hasEmission = false;

			m_framesProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_framesXProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_framesYProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_sizeProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_parallaxProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_offsetProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_biasProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_albedoTexture.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_normalTexture.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			if( hasSpec )
				m_specularTexture.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			if( hasEmission )
				m_emissionTexture.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_depthProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_shadowBiasProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			m_clipProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );

			if( m_speedTreeHueSupport )
			{
				dataCollector.AddToProperties( UniqueId, "[Toggle( EFFECT_HUE_VARIATION )] _Hue(\"Use SpeedTree Hue\", Float) = 0", m_hueProp.OrderIndex );
				dataCollector.AddToPragmas( UniqueId, "shader_feature EFFECT_HUE_VARIATION" );
				m_hueProp.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalvar );
			}

			switch( m_customImpostorType )
			{
				case CustomImpostorType.Octahedron:
				{
					GenerateVectorToOctahedron();
					dataCollector.AddFunctions( "VectortoOctahedron", m_functionBody );

					GenerateOctahedronToVector();
					dataCollector.AddFunctions( "OctahedronToVector", m_functionBody );
				}
				break;
				case CustomImpostorType.HemiOctahedron:
				{
					GenerateVectorToHemiOctahedron();
					dataCollector.AddFunctions( "VectortoHemiOctahedron", m_functionBody );

					GenerateHemiOctahedronToVector();
					dataCollector.AddFunctions( "HemiOctahedronToVector", m_functionBody );
				}
				break;
				default:
				break;
			}

			if( m_customImpostorType == CustomImpostorType.Spherical )
				GenerateSphereImpostorVertex();
			else
				GenerateImpostorVertex();

			string uvFrame1Name = "UVsFrame1" + OutputId;
			string uvFrame2Name = "UVsFrame2" + OutputId;
			string uvFrame3Name = "UVsFrame3" + OutputId;
			string octaFrameName = "octaframe" + OutputId;
			string sphereFrames = "frameUVs" + OutputId;
			string viewPosName = "viewPos" + OutputId;
			TemplateVertexData data = null;
			if( m_customImpostorType == CustomImpostorType.Spherical )
			{
				data = dataCollector.TemplateDataCollectorInstance.RequestNewInterpolator( WirePortDataType.FLOAT4, false, sphereFrames );
				if( data != null )
					sphereFrames = data.VarName;
			}
			else
			{
				data = dataCollector.TemplateDataCollectorInstance.RequestNewInterpolator( WirePortDataType.FLOAT4, false, uvFrame1Name );
				if( data != null )
					uvFrame1Name = data.VarName;
				data = dataCollector.TemplateDataCollectorInstance.RequestNewInterpolator( WirePortDataType.FLOAT4, false, uvFrame2Name );
				if( data != null )
					uvFrame2Name = data.VarName;
				data = dataCollector.TemplateDataCollectorInstance.RequestNewInterpolator( WirePortDataType.FLOAT4, false, uvFrame3Name );
				if( data != null )
					uvFrame3Name = data.VarName;
				data = dataCollector.TemplateDataCollectorInstance.RequestNewInterpolator( WirePortDataType.FLOAT4, false, octaFrameName );
				if( data != null )
					octaFrameName = data.VarName;
			}
			data = dataCollector.TemplateDataCollectorInstance.RequestNewInterpolator( WirePortDataType.FLOAT4, false, viewPosName );
			if( data != null )
				viewPosName = data.VarName;

			MasterNodePortCategory portCategory = dataCollector.PortCategory;
			dataCollector.PortCategory = MasterNodePortCategory.Vertex;

			string vertOut = dataCollector.TemplateDataCollectorInstance.CurrentTemplateData.VertexFunctionData.OutVarName;
			string vertIN = dataCollector.TemplateDataCollectorInstance.CurrentTemplateData.VertexFunctionData.InVarName;
			string functionResult = string.Empty;
			if( m_customImpostorType == CustomImpostorType.Spherical )
				functionResult = dataCollector.AddFunctions( m_functionHeaderSphere, m_functionBody, vertIN, vertOut + "." + sphereFrames, vertOut + "." + viewPosName );
			else
				functionResult = dataCollector.AddFunctions( m_functionHeader, m_functionBody, vertIN, vertOut + "." + uvFrame1Name, vertOut + "." + uvFrame2Name, vertOut + "." + uvFrame3Name, vertOut + "." + octaFrameName, vertOut + "." + viewPosName );
			dataCollector.AddLocalVariable( UniqueId, functionResult + ";" );

			dataCollector.PortCategory = portCategory;

			if( dataCollector.IsFragmentCategory )
			{
				string extraHeader = string.Empty;

				for( int i = 0; i < m_extraSamplers; i++ )
				{
					if( GetInputPortByArrayId( i ).IsConnected && GetOutputPortByArrayId( i ).IsConnected )
					{
						m_extraPropertyNames[ i ] = GetInputPortByArrayId( i ).GeneratePortInstructions( ref dataCollector );
						dataCollector.AddLocalVariable( UniqueId, "float4 output" + i + " = 0;" );
						extraHeader += ", output" + i;
					}
				}

				string finalHeader = m_functionHeaderFrag;
				if( m_customImpostorType == CustomImpostorType.Spherical )
				{
					finalHeader = m_functionHeaderSphereFrag;
					finalHeader = finalHeader.Replace( "{1}", "{1}" + extraHeader );
				}
				else
				{
					finalHeader = finalHeader.Replace( "{4}", "{4}" + extraHeader );
				}

				string fragIN = dataCollector.TemplateDataCollectorInstance.CurrentTemplateData.FragmentFunctionData.InVarName;
				if( m_customImpostorType == CustomImpostorType.Spherical )
				{
					GenerateSphereImpostorFragment( hasSpec, hasEmission );
					functionResult = dataCollector.AddFunctions( finalHeader, m_functionBody, fragIN + "." + sphereFrames, fragIN + "." + viewPosName );
				}
				else
				{
					GenerateImpostorFragment( hasSpec, hasEmission );
					functionResult = dataCollector.AddFunctions( finalHeader, m_functionBody, fragIN + "." + uvFrame1Name, fragIN + "." + uvFrame2Name, fragIN + "." + uvFrame3Name, fragIN + "." + octaFrameName, fragIN + "." + viewPosName );
				}
				dataCollector.AddLocalVariable( UniqueId, functionResult + ";" );

				switch( outputId )
				{
					case 0:
					return "o.Albedo";
					case 1:
					return "o.Normal";
					case 2:
					return "o.Emission";
					case 3:
					return m_workflow == ASEStandardSurfaceWorkflow.Specular ? "o.Specular" : "o.Metallic";
					case 4:
					return "o.Smoothness";
					case 5:
					return "o.Occlusion";
					case 6:
					return "o.Alpha";
					case 7:
					return "worldPos";
					case 16:
					return "0";
					default:
					return "output" + ( outputId - 8 );
				}
			}
			else
			{
				switch( outputId )
				{
					case 16:
					return vertOut + "." + viewPosName + ".xyz";
					default:
					return "0";
				}
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_customImpostorType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_speedTreeHueSupport );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_showExtraData );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_framesProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_framesXProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_framesYProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_sizeProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_parallaxProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_offsetProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_biasProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_albedoTexture.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalTexture.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_specularTexture.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_emissionTexture.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_depthProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shadowBiasProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_clipProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_hueProp.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_extraSamplers );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_workflow );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );

			m_customImpostorType = (CustomImpostorType)Enum.Parse( typeof( CustomImpostorType ), GetCurrentParam( ref nodeParams ) );
			m_speedTreeHueSupport = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			m_showExtraData = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			m_orderFramesProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderFramesXProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderFramesYProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderSizeProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderParallaxProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderOffsetProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderBiasProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderAlbedoTexture = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderNormalTexture = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderSpecularTexture = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderEmissionTexture = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderDepthProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderShadowBiasProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderClipProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderHueProp = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );

			if( UIUtils.CurrentShaderVersion() > 15405 )
			{
				m_extraSamplers = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				m_workflow = (ASEStandardSurfaceWorkflow)Enum.Parse( typeof( ASEStandardSurfaceWorkflow ), GetCurrentParam( ref nodeParams ) );
			}

			UpdateTitle();
			UpdatePorts();
			UpdateInputPorts();
		}

		private void GenerateVectorToOctahedron()
		{
			m_functionBody = string.Empty;
			IOUtils.AddFunctionHeader( ref m_functionBody, "float2 VectortoOctahedron( float3 N )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "N /= dot( 1.0, abs( N ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "if( N.z <= 0 )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "{" );
			IOUtils.AddFunctionLine( ref m_functionBody, "N.xy = ( 1 - abs( N.yx ) ) * ( N.xy >= 0 ? 1.0 : -1.0 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.AddFunctionLine( ref m_functionBody, "return N.xy;" );
			IOUtils.CloseFunctionBody( ref m_functionBody );
		}

		private void GenerateOctahedronToVector()
		{
			m_functionBody = string.Empty;
			IOUtils.AddFunctionHeader( ref m_functionBody, "float3 OctahedronToVector( float2 Oct )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 N = float3( Oct, 1.0 - dot( 1.0, abs( Oct ) ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "if(N.z< 0 )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "{" );
			IOUtils.AddFunctionLine( ref m_functionBody, "N.xy = ( 1 - abs( N.yx) ) * (N.xy >= 0 ? 1.0 : -1.0 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.AddFunctionLine( ref m_functionBody, "return normalize( N);" );
			IOUtils.CloseFunctionBody( ref m_functionBody );
		}

		private void GenerateVectorToHemiOctahedron()
		{
			m_functionBody = string.Empty;
			IOUtils.AddFunctionHeader( ref m_functionBody, "float2 VectortoHemiOctahedron( float3 N )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "{" );
			IOUtils.AddFunctionLine( ref m_functionBody, "N.xy /= dot( 1.0, abs( N ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "return float2( N.x + N.y, N.x - N.y );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.CloseFunctionBody( ref m_functionBody );
		}

		private void GenerateHemiOctahedronToVector()
		{
			m_functionBody = string.Empty;
			IOUtils.AddFunctionHeader( ref m_functionBody, "float3 HemiOctahedronToVector( float2 Oct )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "{" );
			IOUtils.AddFunctionLine( ref m_functionBody, "Oct = float2( Oct.x + Oct.y, Oct.x - Oct.y ) * 0.5;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 N = float3( Oct, 1 - dot( 1.0, abs( Oct ) ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "return normalize( N );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.CloseFunctionBody( ref m_functionBody );
		}

		private void GenerateSphereImpostorVertex()
		{
			m_functionBody = string.Empty;
			IOUtils.AddFunctionHeader( ref m_functionBody, "inline void SphereImpostorVertex( inout appdata_full v, inout float4 frameUVs, inout float4 viewPos )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "{" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float sizeX = _AI_FramesX;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float sizeY = _AI_FramesY - 1;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 fractions = 1 / float3( sizeX, _AI_FramesY, sizeY );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 sizeFraction = fractions.xy;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float axisSizeFraction = fractions.z;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "v.vertex.xyz += _AI_Offset.xyz;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 worldOrigin = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#if defined(UNITY_PASS_SHADOWCASTER)" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 worldCameraPos = 0;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "if( unity_LightShadowBias.y == 0.0 ){" );
			IOUtils.AddFunctionLine( ref m_functionBody, "if( _WorldSpaceLightPos0.w == 1 )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "worldCameraPos = _WorldSpaceLightPos0.xyz;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "else" );
			IOUtils.AddFunctionLine( ref m_functionBody, "worldCameraPos = _WorldSpaceCameraPos;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "} else {" );
			IOUtils.AddFunctionLine( ref m_functionBody, "worldCameraPos = UnityWorldSpaceLightDir( mul(unity_ObjectToWorld, v.vertex).xyz ) * -5000.0;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#else" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 worldCameraPos = _WorldSpaceCameraPos;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#endif" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 objectCameraDirection = normalize( mul( (float3x3)unity_WorldToObject, worldCameraPos - worldOrigin ) - _AI_Offset.xyz );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 upVector = float3( 0,1,0 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 objectHorizontalVector = normalize( cross( objectCameraDirection, upVector ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 objectVerticalVector = cross( objectHorizontalVector, objectCameraDirection );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float verticalAngle = frac( atan2( -objectCameraDirection.z, -objectCameraDirection.x ) / UNITY_TWO_PI ) * sizeX + 0.5;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float verticalDot = dot( objectCameraDirection, upVector );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float upAngle = ( acos( -verticalDot ) / UNITY_PI ) + axisSizeFraction * 0.5f;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float yRot = sizeFraction.x * UNITY_PI * verticalDot * ( 2 * frac( verticalAngle ) - 1 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 uvExpansion = v.texcoord.xy - 0.5;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float cosY = cos( yRot );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float sinY = sin( yRot );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 uvRotator = mul( uvExpansion, float2x2( cosY , -sinY , sinY , cosY ) ) * _AI_ImpostorSize;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 billboard = objectHorizontalVector * uvRotator.x + objectVerticalVector * uvRotator.y + _AI_Offset.xyz;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 relativeCoords = float2( floor( verticalAngle ), min( floor( upAngle * sizeY ), sizeY ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 frameUV = ( v.texcoord.xy + relativeCoords ) * sizeFraction;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "frameUVs.xy = frameUV;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "viewPos.xyz = UnityObjectToViewPos( billboard );" );
			if( m_speedTreeHueSupport )
			{
				IOUtils.AddFunctionLine( ref m_functionBody, "#ifdef EFFECT_HUE_VARIATION" );
				IOUtils.AddFunctionLine( ref m_functionBody, "float hueVariationAmount = frac(unity_ObjectToWorld[0].w + unity_ObjectToWorld[1].w + unity_ObjectToWorld[2].w);" );
				IOUtils.AddFunctionLine( ref m_functionBody, "viewPos.w = saturate(hueVariationAmount * _AI_HueVariation.a);" );
				IOUtils.AddFunctionLine( ref m_functionBody, "#endif" );
			}
			IOUtils.AddFunctionLine( ref m_functionBody, "v.vertex.xyz = billboard;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "v.normal.xyz = objectCameraDirection;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.CloseFunctionBody( ref m_functionBody );
		}

		private void GenerateSphereImpostorFragment( bool withSpec = true, bool withEmission = true )
		{
			m_functionBody = string.Empty;

			string extraHeader = string.Empty;
			for( int i = 0; i < m_extraSamplers; i++ )
			{
				if( GetInputPortByArrayId( i ).IsConnected && GetOutputPortByArrayId( i ).IsConnected )
					extraHeader += ", out float4 output" + i;
			}

			if( m_workflow == ASEStandardSurfaceWorkflow.Specular )
				IOUtils.AddFunctionHeader( ref m_functionBody, "inline void SphereImpostorFragment( inout SurfaceOutputStandardSpecular o, out float4 clipPos, out float3 worldPos, float4 frameUVs, float4 viewPos" + extraHeader + " )" );
			else
				IOUtils.AddFunctionHeader( ref m_functionBody, "inline void SphereImpostorFragment( inout SurfaceOutputStandard o, out float4 clipPos, out float3 worldPos, float4 frameUVs, float4 viewPos" + extraHeader + " )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "{" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 albedoSample = tex2Dbias( _Albedo, float4( frameUVs.xy, 0, _AI_TextureBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 normalSample = tex2Dbias( _Normals, float4( frameUVs.xy, 0, _AI_TextureBias) );" );
			if( withSpec )
				IOUtils.AddFunctionLine( ref m_functionBody, "float4 specularSample = tex2Dbias( _Specular, float4( frameUVs.xy, 0, _AI_TextureBias) );" );
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "float4 emissionSample = tex2Dbias( _Emission, float4( frameUVs.xy, 0, _AI_TextureBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 albedo = albedoSample.rgb;" );
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "float3 emission = emissionSample.rgb;" );
			if( withSpec )
			{
				IOUtils.AddFunctionLine( ref m_functionBody, "float3 specular = specularSample.rgb;" );
				IOUtils.AddFunctionLine( ref m_functionBody, "float smoothness = specularSample.a;" );
			}
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "float occlusion = emissionSample.a;" );

			for( int i = 0; i < m_extraSamplers; i++ )
			{
				if( GetInputPortByArrayId( i ).IsConnected && GetOutputPortByArrayId( i ).IsConnected )
				{
					IOUtils.AddFunctionLine( ref m_functionBody, "output" + i + " = tex2Dbias( " + m_extraPropertyNames[ i ] + ", float4( frameUVs.xy, 0, _AI_TextureBias) );" );
				}
			}

			IOUtils.AddFunctionLine( ref m_functionBody, "float alphaMask = albedoSample.a;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 remapNormal = normalSample * 2 - 1;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 worldNormal = normalize( mul( (float3x3)unity_ObjectToWorld, remapNormal.xyz ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float depth = remapNormal.a * _AI_DepthSize * 0.5;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#if defined(UNITY_PASS_SHADOWCASTER)" );
			IOUtils.AddFunctionLine( ref m_functionBody, "if( _WorldSpaceLightPos0.w == 0 )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "{" );
			IOUtils.AddFunctionLine( ref m_functionBody, "depth = depth * 0.95 - 0.05  - _AI_ShadowBias * unity_LightShadowBias.y;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "} else {" );
			IOUtils.AddFunctionLine( ref m_functionBody, "depth = depth * 0.95 - 0.05  - _AI_ShadowBias;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#endif" );
			IOUtils.AddFunctionLine( ref m_functionBody, "viewPos.z += depth;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "worldPos = mul( UNITY_MATRIX_I_V, float4( viewPos.xyz, 1 ) ).xyz;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "clipPos = mul( UNITY_MATRIX_P, float4( viewPos.xyz, 1 ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#ifdef UNITY_PASS_SHADOWCASTER" );
			IOUtils.AddFunctionLine( ref m_functionBody, "clipPos = UnityApplyLinearShadowBias( clipPos );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#endif" );
			IOUtils.AddFunctionLine( ref m_functionBody, "clipPos.xyz /= clipPos.w;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "if( UNITY_NEAR_CLIP_VALUE < 0 )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "clipPos = clipPos * 0.5 + 0.5;" );
			if( m_speedTreeHueSupport )
			{
				IOUtils.AddFunctionLine( ref m_functionBody, "#ifdef EFFECT_HUE_VARIATION" );
				IOUtils.AddFunctionLine( ref m_functionBody, "half3 shiftedColor = lerp(albedo.rgb, _AI_HueVariation.rgb, viewPos.w);" );
				IOUtils.AddFunctionLine( ref m_functionBody, "half maxBase = max(albedo.r, max(albedo.g, albedo.b));" );
				IOUtils.AddFunctionLine( ref m_functionBody, "half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));" );
				IOUtils.AddFunctionLine( ref m_functionBody, "maxBase /= newMaxBase;" );
				IOUtils.AddFunctionLine( ref m_functionBody, "maxBase = maxBase * 0.5f + 0.5f;" );
				IOUtils.AddFunctionLine( ref m_functionBody, "shiftedColor.rgb *= maxBase;" );
				IOUtils.AddFunctionLine( ref m_functionBody, "albedo.rgb = saturate(shiftedColor);" );
				IOUtils.AddFunctionLine( ref m_functionBody, "#endif" );
			}
			IOUtils.AddFunctionLine( ref m_functionBody, "o.Albedo = albedo;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "o.Normal = worldNormal;" );
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "o.Emission = emission;" );
			if( withSpec )
			{
				if( m_workflow == ASEStandardSurfaceWorkflow.Specular )
					IOUtils.AddFunctionLine( ref m_functionBody, "o.Specular = specular;" );
				else
					IOUtils.AddFunctionLine( ref m_functionBody, "o.Metallic = saturate((specular-unity_ColorSpaceDielectricSpec.rgb)/(albedo-unity_ColorSpaceDielectricSpec.rgb));" );
				IOUtils.AddFunctionLine( ref m_functionBody, "o.Smoothness = smoothness;" );
			}
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "o.Occlusion = occlusion;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "o.Alpha = ( alphaMask - _AI_Clip );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "clip( o.Alpha );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.CloseFunctionBody( ref m_functionBody );
		}

		private void GenerateImpostorVertex()
		{
			m_functionBody = string.Empty;
			IOUtils.AddFunctionHeader( ref m_functionBody, "inline void OctaImpostorVertex( inout appdata_full v, inout float4 uvsFrame1, inout float4 uvsFrame2, inout float4 uvsFrame3, inout float4 octaFrame, inout float4 viewPos )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float framesXY = _AI_Frames;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float prevFrame = framesXY - 1;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 fractions = 1.0 / float2( framesXY, prevFrame );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float fractionsFrame = fractions.x;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float fractionsPrevFrame = fractions.y;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float UVscale = _AI_ImpostorSize;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float parallax = -_AI_Parallax;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "v.vertex.xyz += _AI_Offset.xyz;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 worldOrigin = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#if defined(UNITY_PASS_SHADOWCASTER)" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 worldCameraPos = 0;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "if( unity_LightShadowBias.y == 0.0 ){" );
			IOUtils.AddFunctionLine( ref m_functionBody, "if( _WorldSpaceLightPos0.w == 1 )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "worldCameraPos = _WorldSpaceLightPos0.xyz;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "else" );
			IOUtils.AddFunctionLine( ref m_functionBody, "worldCameraPos = _WorldSpaceCameraPos;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "} else {" );
			IOUtils.AddFunctionLine( ref m_functionBody, "worldCameraPos = UnityWorldSpaceLightDir( mul(unity_ObjectToWorld, v.vertex).xyz ) * -5000.0;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#else" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 worldCameraPos = _WorldSpaceCameraPos;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#endif" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 objectCameraDirection = normalize( mul( (float3x3)unity_WorldToObject, worldCameraPos - worldOrigin ) - _AI_Offset.xyz );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 objectCameraPosition = mul( unity_WorldToObject, float4( worldCameraPos, 1 ) ).xyz - _AI_Offset.xyz;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 upVector = float3( 0,1,0 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 objectHorizontalVector = normalize( cross( objectCameraDirection, upVector ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 objectVerticalVector = cross( objectHorizontalVector, objectCameraDirection );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 uvExpansion = ( v.texcoord.xy - 0.5f ) * framesXY * fractionsFrame * UVscale;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 billboard = objectHorizontalVector * uvExpansion.x + objectVerticalVector * uvExpansion.y + _AI_Offset.xyz;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 localDir = billboard - objectCameraPosition - _AI_Offset.xyz;" );
			if( m_customImpostorType == CustomImpostorType.Octahedron )
			{
				IOUtils.AddFunctionLine( ref m_functionBody, "float2 frameOcta = VectortoOctahedron( objectCameraDirection.xzy ) * 0.5 + 0.5;" );
			}
			else
			{
				IOUtils.AddFunctionLine( ref m_functionBody, "objectCameraDirection.y = max(0.001, objectCameraDirection.y);" );
				IOUtils.AddFunctionLine( ref m_functionBody, "float2 frameOcta = VectortoHemiOctahedron( objectCameraDirection.xzy ) * 0.5 + 0.5;" );
			}
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 prevOctaFrame = frameOcta * prevFrame;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 baseOctaFrame = floor( prevOctaFrame );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 fractionOctaFrame = ( baseOctaFrame * fractionsFrame );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 octaFrame1 = ( baseOctaFrame * fractionsPrevFrame ) * 2.0 - 1.0;" );
			if( m_customImpostorType == CustomImpostorType.Octahedron )
				IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa1WorldY = OctahedronToVector( octaFrame1 ).xzy;" );
			else
				IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa1WorldY = HemiOctahedronToVector( octaFrame1 ).xzy;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa1WorldX = normalize( cross( upVector, octa1WorldY ) + float3(-0.001,0,0) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa1WorldZ = cross( octa1WorldX , octa1WorldY );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float dotY1 = dot( octa1WorldY , localDir );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa1LocalY = normalize( float3( dot( octa1WorldX , localDir ), dotY1, dot( octa1WorldZ , localDir ) ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float lineInter1 = dot( octa1WorldY , -objectCameraPosition ) / dotY1;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 intersectPos1 = ( lineInter1 * localDir + objectCameraPosition );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float dotframeX1 = dot( octa1WorldX , -intersectPos1 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float dotframeZ1 = dot( octa1WorldZ , -intersectPos1 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 uvFrame1 = float2( dotframeX1 , dotframeZ1 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 uvParallax1 = octa1LocalY.xz * fractionsFrame * parallax;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "uvFrame1 = ( ( uvFrame1 / UVscale ) + 0.5 ) * fractionsFrame + fractionOctaFrame;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "uvsFrame1 = float4( uvParallax1, uvFrame1);" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 fractPrevOctaFrame = frac( prevOctaFrame );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 cornerDifference = lerp( float2( 0,1 ) , float2( 1,0 ) , saturate( ceil( ( fractPrevOctaFrame.x - fractPrevOctaFrame.y ) ) ));" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 octaFrame2 = ( ( baseOctaFrame + cornerDifference ) * fractionsPrevFrame ) * 2.0 - 1.0;" );
			if( m_customImpostorType == CustomImpostorType.Octahedron )
				IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa2WorldY = OctahedronToVector( octaFrame2 ).xzy;" );
			else
				IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa2WorldY = HemiOctahedronToVector( octaFrame2 ).xzy;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa2WorldX = normalize( cross( upVector, octa2WorldY ) + float3(-0.001,0,0) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa2WorldZ = cross( octa2WorldX , octa2WorldY );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float dotY2 = dot( octa2WorldY , localDir );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa2LocalY = normalize( float3( dot( octa2WorldX , localDir ), dotY2, dot( octa2WorldZ , localDir ) ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float lineInter2 = dot( octa2WorldY , -objectCameraPosition ) / dotY2; " );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 intersectPos2 = ( lineInter2 * localDir + objectCameraPosition );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float dotframeX2 = dot( octa2WorldX , -intersectPos2 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float dotframeZ2 = dot( octa2WorldZ , -intersectPos2 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 uvFrame2 = float2( dotframeX2 , dotframeZ2 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 uvParallax2 = octa2LocalY.xz * fractionsFrame * parallax;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "uvFrame2 = ( ( uvFrame2 / UVscale ) + 0.5 ) * fractionsFrame + ( ( cornerDifference * fractionsFrame ) + fractionOctaFrame );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "uvsFrame2 = float4( uvParallax2, uvFrame2);" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 octaFrame3 = ( ( baseOctaFrame + 1 ) * fractionsPrevFrame  ) * 2.0 - 1.0;" );
			if( m_customImpostorType == CustomImpostorType.Octahedron )
				IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa3WorldY = OctahedronToVector( octaFrame3 ).xzy;" );
			else
				IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa3WorldY = HemiOctahedronToVector( octaFrame3 ).xzy;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa3WorldX = normalize( cross( upVector, octa3WorldY ) + float3(-0.001,0,0) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa3WorldZ = cross( octa3WorldX , octa3WorldY );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float dotY3 = dot( octa3WorldY , localDir );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 octa3LocalY = normalize( float3( dot( octa3WorldX , localDir ), dotY3, dot( octa3WorldZ , localDir ) ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float lineInter3 = dot( octa3WorldY , -objectCameraPosition ) / dotY3;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 intersectPos3 = ( lineInter3 * localDir + objectCameraPosition );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float dotframeX3 = dot( octa3WorldX , -intersectPos3 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float dotframeZ3 = dot( octa3WorldZ , -intersectPos3 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 uvFrame3 = float2( dotframeX3 , dotframeZ3 );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 uvParallax3 = octa3LocalY.xz * fractionsFrame * parallax;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "uvFrame3 = ( ( uvFrame3 / UVscale ) + 0.5 ) * fractionsFrame + ( fractionOctaFrame + fractionsFrame );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "uvsFrame3 = float4( uvParallax3, uvFrame3);" );
			IOUtils.AddFunctionLine( ref m_functionBody, "octaFrame = 0;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "octaFrame.xy = prevOctaFrame;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "viewPos = 0;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "viewPos.xyz = UnityObjectToViewPos( billboard );" );
			if( m_speedTreeHueSupport )
			{
				IOUtils.AddFunctionLine( ref m_functionBody, "#ifdef EFFECT_HUE_VARIATION" );
				IOUtils.AddFunctionLine( ref m_functionBody, "float hueVariationAmount = frac(unity_ObjectToWorld[0].w + unity_ObjectToWorld[1].w + unity_ObjectToWorld[2].w);" );
				IOUtils.AddFunctionLine( ref m_functionBody, "viewPos.w = saturate(hueVariationAmount * _AI_HueVariation.a);" );
				IOUtils.AddFunctionLine( ref m_functionBody, "#endif" );
			}
			IOUtils.AddFunctionLine( ref m_functionBody, "v.vertex.xyz = billboard;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "v.normal.xyz = objectCameraDirection;" );

			IOUtils.CloseFunctionBody( ref m_functionBody );
		}

		void GenerateImpostorFragment( bool withSpec = true, bool withEmission = true )
		{
			m_functionBody = string.Empty;

			string extraHeader = string.Empty;
			for( int i = 0; i < m_extraSamplers; i++ )
			{
				if( GetInputPortByArrayId( i ).IsConnected && GetOutputPortByArrayId( i ).IsConnected )
					extraHeader += ", out float4 output" + i;
			}
			if( m_workflow == ASEStandardSurfaceWorkflow.Specular )
				IOUtils.AddFunctionHeader( ref m_functionBody, "inline void OctaImpostorFragment( inout SurfaceOutputStandardSpecular o, out float4 clipPos, out float3 worldPos, float4 uvsFrame1, float4 uvsFrame2, float4 uvsFrame3, float4 octaFrame, float4 interpViewPos" + extraHeader + " )" );
			else
				IOUtils.AddFunctionHeader( ref m_functionBody, "inline void OctaImpostorFragment( inout SurfaceOutputStandard o, out float4 clipPos, out float3 worldPos, float4 uvsFrame1, float4 uvsFrame2, float4 uvsFrame3, float4 octaFrame, float4 interpViewPos" + extraHeader + " )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float depthBias = -1.0;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float textureBias = _AI_TextureBias;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 parallaxSample1 = tex2Dbias( _Normals, float4( uvsFrame1.zw, 0, depthBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 parallax1 = ( ( 0.5 - parallaxSample1.a ) * uvsFrame1.xy ) + uvsFrame1.zw;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 albedo1 = tex2Dbias( _Albedo, float4( parallax1, 0, textureBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 normals1 = tex2Dbias( _Normals, float4( parallax1, 0, textureBias) );" );
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "float4 mask1 = tex2Dbias( _Emission, float4( parallax1, 0, textureBias) );" );
			if( withSpec )
				IOUtils.AddFunctionLine( ref m_functionBody, "float4 spec1 = tex2Dbias( _Specular, float4( parallax1, 0, textureBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 parallaxSample2 = tex2Dbias( _Normals, float4( uvsFrame2.zw, 0, depthBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 parallax2 = ( ( 0.5 - parallaxSample2.a ) * uvsFrame2.xy ) + uvsFrame2.zw;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 albedo2 = tex2Dbias( _Albedo, float4( parallax2, 0, textureBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 normals2 = tex2Dbias( _Normals, float4( parallax2, 0, textureBias) );" );
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "float4 mask2 = tex2Dbias( _Emission, float4( parallax2, 0, textureBias) );" );
			if( withSpec )
				IOUtils.AddFunctionLine( ref m_functionBody, "float4 spec2 = tex2Dbias( _Specular, float4( parallax2, 0, textureBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 parallaxSample3 = tex2Dbias( _Normals, float4( uvsFrame3.zw, 0, depthBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 parallax3 = ( ( 0.5 - parallaxSample3.a ) * uvsFrame3.xy ) + uvsFrame3.zw;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 albedo3 = tex2Dbias( _Albedo, float4( parallax3, 0, textureBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 normals3 = tex2Dbias( _Normals, float4( parallax3, 0, textureBias) );" );
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "float4 mask3 = tex2Dbias( _Emission, float4( parallax3, 0, textureBias) );" );
			if( withSpec )
				IOUtils.AddFunctionLine( ref m_functionBody, "float4 spec3 = tex2Dbias( _Specular, float4( parallax3, 0, textureBias) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 fraction = frac( octaFrame.xy );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float2 invFraction = 1 - fraction;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 weights;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "weights.x = min( invFraction.x, invFraction.y );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "weights.y = abs( fraction.x - fraction.y );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "weights.z = min( fraction.x, fraction.y );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 blendedAlbedo = albedo1 * weights.x  + albedo2 * weights.y + albedo3 * weights.z;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float4 blendedNormal = normals1 * weights.x  + normals2 * weights.y + normals3 * weights.z;" );
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "float4 blendedMask = mask1 * weights.x  + mask2 * weights.y + mask3 * weights.z;" );
			if( withSpec )
				IOUtils.AddFunctionLine( ref m_functionBody, "float4 blendedSpec = spec1 * weights.x  + spec2 * weights.y + spec3 * weights.z;" );

			for( int i = 0; i < m_extraSamplers; i++ )
			{
				if( GetInputPortByArrayId( i ).IsConnected && GetOutputPortByArrayId( i ).IsConnected )
				{
					IOUtils.AddFunctionLine( ref m_functionBody, "float4 output" + i + "a = tex2Dbias( " + m_extraPropertyNames[ i ] + ", float4( parallax1, 0, textureBias) );" );
					IOUtils.AddFunctionLine( ref m_functionBody, "float4 output" + i + "b = tex2Dbias( " + m_extraPropertyNames[ i ] + ", float4( parallax2, 0, textureBias) );" );
					IOUtils.AddFunctionLine( ref m_functionBody, "float4 output" + i + "c = tex2Dbias( " + m_extraPropertyNames[ i ] + ", float4( parallax3, 0, textureBias) );" );
					IOUtils.AddFunctionLine( ref m_functionBody, "output" + i + " = output" + i + "a * weights.x  + output" + i + "b * weights.y + output" + i + "c * weights.z;" );
				}
			}

			IOUtils.AddFunctionLine( ref m_functionBody, "float3 localNormal = blendedNormal.rgb * 2.0 - 1.0;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 worldNormal = normalize( mul( unity_ObjectToWorld, float4( localNormal, 0 ) ).xyz );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "float3 viewPos = interpViewPos.xyz;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "viewPos.z += ( ( parallaxSample1.a * weights.x + parallaxSample2.a * weights.y + parallaxSample3.a * weights.z ) * 2.0 - 1.0) * 0.5 * _AI_DepthSize * length( unity_ObjectToWorld[2].xyz );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#ifdef UNITY_PASS_SHADOWCASTER" );
			IOUtils.AddFunctionLine( ref m_functionBody, "if( _WorldSpaceLightPos0.w == 0 )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "{" );
			IOUtils.AddFunctionLine( ref m_functionBody, "viewPos.z += -_AI_ShadowBias * unity_LightShadowBias.y;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.AddFunctionLine( ref m_functionBody, "else" );
			IOUtils.AddFunctionLine( ref m_functionBody, "{" );
			IOUtils.AddFunctionLine( ref m_functionBody, "viewPos.z += -_AI_ShadowBias;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "}" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#endif" );
			IOUtils.AddFunctionLine( ref m_functionBody, "worldPos = mul( UNITY_MATRIX_I_V, float4( viewPos.xyz, 1 ) ).xyz;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "clipPos = mul( UNITY_MATRIX_P, float4( viewPos, 1 ) );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#ifdef UNITY_PASS_SHADOWCASTER" );
			IOUtils.AddFunctionLine( ref m_functionBody, "clipPos = UnityApplyLinearShadowBias( clipPos );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "#endif" );
			IOUtils.AddFunctionLine( ref m_functionBody, "clipPos.xyz /= clipPos.w;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "if( UNITY_NEAR_CLIP_VALUE < 0 )" );
			IOUtils.AddFunctionLine( ref m_functionBody, "clipPos = clipPos * 0.5 + 0.5;" );
			if( m_speedTreeHueSupport )
			{
				IOUtils.AddFunctionLine( ref m_functionBody, "#ifdef EFFECT_HUE_VARIATION" );
				IOUtils.AddFunctionLine( ref m_functionBody, "half3 shiftedColor = lerp(blendedAlbedo.rgb, _AI_HueVariation.rgb, interpViewPos.w);" );
				IOUtils.AddFunctionLine( ref m_functionBody, "half maxBase = max(blendedAlbedo.r, max(blendedAlbedo.g, blendedAlbedo.b));" );
				IOUtils.AddFunctionLine( ref m_functionBody, "half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));" );
				IOUtils.AddFunctionLine( ref m_functionBody, "maxBase /= newMaxBase;" );
				IOUtils.AddFunctionLine( ref m_functionBody, "maxBase = maxBase * 0.5f + 0.5f;" );
				IOUtils.AddFunctionLine( ref m_functionBody, "shiftedColor.rgb *= maxBase;" );
				IOUtils.AddFunctionLine( ref m_functionBody, "blendedAlbedo.rgb = saturate(shiftedColor);" );
				IOUtils.AddFunctionLine( ref m_functionBody, "#endif" );
			}
			IOUtils.AddFunctionLine( ref m_functionBody, "o.Albedo = blendedAlbedo.rgb;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "o.Normal = worldNormal;" );
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "o.Emission = blendedMask.rgb;" );
			if( withSpec )
			{
				if( m_workflow == ASEStandardSurfaceWorkflow.Specular )
					IOUtils.AddFunctionLine( ref m_functionBody, "o.Specular = blendedSpec.rgb;" );
				else
					IOUtils.AddFunctionLine( ref m_functionBody, "o.Metallic = saturate((blendedSpec.rgb-unity_ColorSpaceDielectricSpec.rgb)/(blendedAlbedo.rgb-unity_ColorSpaceDielectricSpec.rgb));" );
			}
			if( withSpec )
				IOUtils.AddFunctionLine( ref m_functionBody, "o.Smoothness = blendedSpec.a;" );
			if( withEmission )
				IOUtils.AddFunctionLine( ref m_functionBody, "o.Occlusion = blendedMask.a;" );
			IOUtils.AddFunctionLine( ref m_functionBody, "o.Alpha = ( blendedAlbedo.a - _AI_Clip );" );
			IOUtils.AddFunctionLine( ref m_functionBody, "clip( o.Alpha );" );
			IOUtils.CloseFunctionBody( ref m_functionBody );
		}
	}
}
#endif
