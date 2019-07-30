// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AmplifyShaderEditor
{
	public enum VertexMode
	{
		Relative,
		Absolute
	}

	public enum RenderPath
	{
		All,
		ForwardOnly,
		DeferredOnly
	}

	public enum StandardShaderLightModel
	{
		Standard,
		StandardSpecular,
		Lambert,
		BlinnPhong,
		Unlit,
		CustomLighting
	}

	public enum CullMode
	{
		Back,
		Front,
		Off
	}

	public enum AlphaMode
	{
		Opaque = 0,
		Masked = 1,
		Transparent = 2, // Transparent (alpha:fade)
		Translucent = 3,
		Premultiply = 4, // Alpha Premul (alpha:premul)
		Custom = 5,
	}

	public enum RenderType
	{
		Opaque,
		Transparent,
		TransparentCutout,
		Background,
		Overlay,
		TreeOpaque,
		TreeTransparentCutout,
		TreeBillboard,
		Grass,
		GrassBillboard
	}

	public enum RenderQueue
	{
		Background,
		Geometry,
		AlphaTest,
		Transparent,
		Overlay
	}

	public enum RenderPlatforms
	{
		d3d9,
		d3d11,
		glcore,
		gles,
		gles3,
		metal,
		d3d11_9x,
		xbox360,
		xboxone,
		ps4,
		psp2,
		n3ds,
		wiiu
	}

	[Serializable]
	public class NodeCache
	{
		public int TargetNodeId = -1;
		public int TargetPortId = -1;

		public NodeCache( int targetNodeId, int targetPortId )
		{
			SetData( targetNodeId, targetPortId );
		}

		public void SetData( int targetNodeId, int targetPortId )
		{
			TargetNodeId = targetNodeId;
			TargetPortId = targetPortId;
		}

		public void Invalidate()
		{
			TargetNodeId = -1;
			TargetPortId = -1;
		}

		public bool IsValid
		{
			get { return ( TargetNodeId >= 0 ); }
		}

		public override string ToString()
		{
			return "TargetNodeId " + TargetNodeId + " TargetPortId " + TargetPortId;
		}
	}

	[Serializable]
	public class CacheNodeConnections
	{
		public Dictionary<string, List<NodeCache>> NodeCacheArray;

		public CacheNodeConnections()
		{
			NodeCacheArray = new Dictionary<string, List<NodeCache>>();
		}

		public void Add( string key, NodeCache value )
		{
			if ( NodeCacheArray.ContainsKey( key ) )
			{
				NodeCacheArray[ key ].Add( value );
			}
			else
			{
				NodeCacheArray.Add( key, new List<NodeCache>() );
				NodeCacheArray[ key ].Add( value );
			}
		}

		public NodeCache Get( string key, int idx = 0 )
		{
			if ( NodeCacheArray.ContainsKey( key ) )
			{
				if ( idx < NodeCacheArray[ key ].Count )
					return NodeCacheArray[ key ][ idx ];
			}
			return null;
		}

		public List<NodeCache> GetList( string key )
		{
			if ( NodeCacheArray.ContainsKey( key ) )
			{
				return NodeCacheArray[ key ];
			}
			return null;
		}

		public void Clear()
		{
			foreach ( KeyValuePair<string, List<NodeCache>> kvp in NodeCacheArray )
			{
				kvp.Value.Clear();
			}
			NodeCacheArray.Clear();
		}
	}

	[Serializable]
	[NodeAttributes( "Standard Surface Output", "Master", "Surface shader generator output", null, KeyCode.None, false )]
	public sealed class StandardSurfaceOutputNode : MasterNode, ISerializationCallbackReceiver
	{
		private readonly static string[] VertexLitFunc = { "\t\tinline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )",
													"\t\t{",
													"\t\t\treturn fixed4 ( 0, 0, 0, s.Alpha );",
													"\t\t}\n"};

		private readonly static string[] FadeModeOptions = { "Opaque", "Masked", "Transparent", "Translucent", "Alpha Premultipled", "Custom" };
		private const string VertexModeStr = "Vertex Output";
		private const string CustomInspectorStr = "Custom Editor";
		private readonly static GUIContent RenderPathContent = new GUIContent( "Render Path", "Selects and generates passes for the supported rendering paths\nDefault: All" );
		private const string ShaderModelStr = "Shader Model";
		private readonly static GUIContent LightModelContent = new GUIContent( "Light Model", "Surface shader lighting model defines how the surface reflects light\nDefault: Standard" );
		private readonly static GUIContent ShaderLODContent = new GUIContent( "Shader LOD", "Shader LOD" );
		private readonly static GUIContent CullModeContent = new GUIContent( "Cull Mode", "Polygon culling mode prevents rendering of either back-facing or front-facing polygons to save performance, turn it off if you want to render both sides\nDefault: Back" );
		private const string ZWriteModeStr = "ZWrite Mode";
		private const string ZTestModeStr = "ZTest Mode";

		private const string DiscardStr = "Opacity Mask";
		private const string VertexDisplacementStr = "Local Vertex Offset";
		private const string VertexPositionStr = "Local Vertex Position";
		private const string VertexDataStr = "VertexData";
		private const string VertexNormalStr = "Local Vertex Normal";
		//private const string CustomLightModelStr = "C. Light Model";
		private const string AlbedoStr = "Albedo";
		private const string NormalStr = "Normal";
		private const string EmissionStr = "Emission";
		private const string MetallicStr = "Metallic";
		private const string SmoothnessStr = "Smoothness";
		private const string OcclusionDataStr = "Occlusion";
		private const string OcclusionLabelStr = "Ambient Occlusion";
		private const string TransmissionStr = "Transmission";
		private const string TranslucencyStr = "Translucency";
		private const string RefractionStr = "Refraction";
		private const string AlphaStr = "Opacity";
		private const string AlphaDataStr = "Alpha";
		private const string DebugStr = "Debug";
		private const string SpecularStr = "Specular";
		private const string GlossStr = "Gloss";
		private readonly static GUIContent AlphaModeContent = new GUIContent( " Blend Mode", "Defines how the surface blends with the background\nDefault: Opaque" );
		private const string OpacityMaskClipValueStr = "Mask Clip Value";
		private readonly static GUIContent OpacityMaskClipValueContent = new GUIContent( "Mask Clip Value", "Default clip value to be compared with opacity alpha ( 0 = fully Opaque, 1 = fully Masked )\nDefault: 0.5" );
		private readonly static GUIContent CastShadowsContent = new GUIContent( "Cast Shadows", "Generates a shadow caster pass for vertex modifications and point lights in forward rendering\nDefault: ON" );
		private readonly static GUIContent ReceiveShadowsContent = new GUIContent( "Receive Shadows", "Untick it to disable shadow receiving, this includes self-shadowing (only for forward rendering) \nDefault: ON" );
		private readonly static GUIContent QueueIndexContent = new GUIContent( "Queue Index", "Value to offset the render queue, accepts both positive values to render later and negative values to render sooner\nDefault: 0" );
		private readonly static GUIContent RefractionLayerStr = new GUIContent( "Refraction Layer", "Use it to group or ungroup different refraction shaders into the same or different grabpass (only for forward rendering) \nDefault: 0" );
		private readonly static GUIContent RenderQueueContent = new GUIContent( "Render Queue", "Base rendering queue index\n(Background = 1000, Geometry = 2000, AlphaTest = 2450, Transparent = 3000, Overlay = 4000)\nDefault: Geometry" );
		private readonly static GUIContent RenderTypeContent = new GUIContent( "Render Type", "Categorizes shaders into several predefined groups, usually to be used with screen shader effects\nDefault: Opaque" );

		private const string DefaultShaderName = "MyNewShader";

		private const string ShaderInputOrderStr = "Shader Input Order";
		private const string PropertyOderFoldoutStr = " Material Properties";

		[SerializeField]
		private BlendOpsHelper m_blendOpsHelper = new BlendOpsHelper();

		[SerializeField]
		private StencilBufferOpHelper m_stencilBufferHelper = new StencilBufferOpHelper();

		[SerializeField]
		private ZBufferOpHelper m_zBufferHelper = new ZBufferOpHelper();

		[SerializeField]
		private OutlineOpHelper m_outlineHelper = new OutlineOpHelper();

		[SerializeField]
		private TessellationOpHelper m_tessOpHelper = new TessellationOpHelper();

		[SerializeField]
		private ColorMaskHelper m_colorMaskHelper = new ColorMaskHelper();

		[SerializeField]
		private RenderingPlatformOpHelper m_renderingPlatformOpHelper = new RenderingPlatformOpHelper();

		[SerializeField]
		private RenderingOptionsOpHelper m_renderingOptionsOpHelper = new RenderingOptionsOpHelper();

		[SerializeField]
		private BillboardOpHelper m_billboardOpHelper = new BillboardOpHelper();

		[SerializeField]
		private FallbackPickerHelper m_fallbackHelper = null;

		[SerializeField]
		private AdditionalIncludesHelper m_additionalIncludes = new AdditionalIncludesHelper();

		[SerializeField]
		private CustomTagsHelper m_customTagsHelper = new CustomTagsHelper();

		[SerializeField]
		private StandardShaderLightModel m_currentLightModel;

		[SerializeField]
		private StandardShaderLightModel m_lastLightModel;

		[SerializeField]
		private CullMode m_cullMode = CullMode.Back;

		[SerializeField]
		private AlphaMode m_alphaMode = AlphaMode.Opaque;


		[SerializeField]
		private RenderType m_renderType = RenderType.Opaque;

		[SerializeField]
		private RenderQueue m_renderQueue = RenderQueue.Geometry;

		[SerializeField]
		private RenderPath m_renderPath = RenderPath.All;

		[SerializeField]
		private VertexMode m_vertexMode = VertexMode.Relative;

		[SerializeField]
		private bool m_customBlendMode = false;

		[SerializeField]
		private float m_opacityMaskClipValue = 0.5f;

		[SerializeField]
		private int m_discardPortId = -1;

		[SerializeField]
		private int m_opacityPortId = -1;

		[SerializeField]
		private int m_vertexPortId = -1;

		[SerializeField]
		private bool m_keepAlpha = true;

		[SerializeField]
		private bool m_castShadows = true;

		//[SerializeField]
		private bool m_customShadowCaster = false;

		[SerializeField]
		private bool m_receiveShadows = true;

		[SerializeField]
		private int m_queueOrder = 0;

		[SerializeField]
		private int m_grabOrder = 0;

		private InputPort m_transmissionPort;
		private InputPort m_translucencyPort;
		private InputPort m_tessellationPort;
		private bool m_previousTranslucencyOn = false;
		private bool m_previousRefractionOn = false;
		[SerializeField]
		private List<PropertyNode> m_propertyNodes = new List<PropertyNode>();

		[SerializeField]
		private CacheNodeConnections m_cacheNodeConnections = new CacheNodeConnections();

		private ReorderableList m_propertyReordableList;
		//private int m_availableCount = 0;
		private int m_lastCount = 0;

		private bool m_usingProSkin = false;
		private GUIStyle m_inspectorFoldoutStyle;
		private GUIStyle m_inspectorToolbarStyle;
		private GUIStyle m_inspectorTooldropdownStyle;


		private bool m_customBlendAvailable = false;

		private Color m_cachedColor = Color.white;
		private float m_titleOpacity = 0.5f;
		private float m_boxOpacity = 0.5f;

		private InputPort m_refractionPort;

		private GUIStyle m_inspectorDefaultStyle;
		private GUIStyle m_propertyAdjustment;

		[SerializeField]
		private ReordenatorNode m_maskClipReorder = null;

		[SerializeField]
		private int m_maskClipOrderIndex = -1;

		[SerializeField]
		private ReordenatorNode m_translucencyReorder = null;

		[SerializeField]
		private int m_translucencyOrderIndex = -1;

		[SerializeField]
		private ReordenatorNode m_refractionReorder = null;

		[SerializeField]
		private int m_refractionOrderIndex = -1;

		[SerializeField]
		private ReordenatorNode m_tessellationReorder = null;

		[SerializeField]
		private int m_tessellationOrderIndex = -1;

		private bool m_previousTessellationOn = false;
		private bool m_initialize = true;
		private bool m_checkChanges = true;

		private PropertyNode m_dummyProperty = null;

		protected override void CommonInit( int uniqueId )
		{
			m_currentLightModel = m_lastLightModel = StandardShaderLightModel.Standard;
			m_textLabelWidth = 120;
			base.CommonInit( uniqueId );
			m_zBufferHelper.ParentSurface = this;
			m_tessOpHelper.ParentSurface = this;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			if ( m_fallbackHelper == null )
				m_fallbackHelper = ScriptableObject.CreateInstance<FallbackPickerHelper>();
		}

		public override void AddMasterPorts()
		{
			int index = 2;
			base.AddMasterPorts();
			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.Standard:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1, MasterNodePortCategory.Fragment, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0, MasterNodePortCategory.Fragment, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr, index++, MasterNodePortCategory.Fragment, 2 );
					AddInputPort( WirePortDataType.FLOAT, false, MetallicStr, index++, MasterNodePortCategory.Fragment, 3 );
					AddInputPort( WirePortDataType.FLOAT, false, SmoothnessStr, index++, MasterNodePortCategory.Fragment, 4 );
					AddInputPort( WirePortDataType.FLOAT, false, OcclusionLabelStr, OcclusionDataStr, index++, MasterNodePortCategory.Fragment, 5 );
				}
				break;
				case StandardShaderLightModel.StandardSpecular:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1, MasterNodePortCategory.Fragment, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0, MasterNodePortCategory.Fragment, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr, index++, MasterNodePortCategory.Fragment, 2 );
					AddInputPort( WirePortDataType.FLOAT3, false, SpecularStr, index++, MasterNodePortCategory.Fragment, 3 );
					AddInputPort( WirePortDataType.FLOAT, false, SmoothnessStr, index++, MasterNodePortCategory.Fragment, 4 );
					AddInputPort( WirePortDataType.FLOAT, false, OcclusionLabelStr, OcclusionDataStr, index++, MasterNodePortCategory.Fragment, 5 );
				}
				break;
				//case StandardShaderLightModel.CustomLighting:
				//{
				//	AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1, MasterNodePortCategory.Fragment, 0 );
				//	m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;
				//	AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0, MasterNodePortCategory.Fragment, 1 );
				//	m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;
				//	AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr, index++, MasterNodePortCategory.Fragment, 2 );
				//	AddInputPort( WirePortDataType.FLOAT, false, SpecularStr, index++, MasterNodePortCategory.Fragment, 3 );
				//	m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;
				//	AddInputPort( WirePortDataType.FLOAT, false, GlossStr, index++, MasterNodePortCategory.Fragment, 4 );
				//	m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;
				//}
				//break;
				case StandardShaderLightModel.CustomLighting:
				case StandardShaderLightModel.Unlit:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1, MasterNodePortCategory.Fragment, 0 );
					m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0, MasterNodePortCategory.Fragment, 1 );
					m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr, index++, MasterNodePortCategory.Fragment, 2 );
					AddInputPort( WirePortDataType.FLOAT, false, SpecularStr, index++, MasterNodePortCategory.Fragment, 3 );
					m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;
					AddInputPort( WirePortDataType.FLOAT, false, GlossStr, index++, MasterNodePortCategory.Fragment, 4 );
					m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;
				}
				break;
				case StandardShaderLightModel.Lambert:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1, MasterNodePortCategory.Fragment, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0, MasterNodePortCategory.Fragment, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr, index++, MasterNodePortCategory.Fragment, 2 );
					AddInputPort( WirePortDataType.FLOAT, false, SpecularStr, index++, MasterNodePortCategory.Fragment, 3 );
					AddInputPort( WirePortDataType.FLOAT, false, GlossStr, index++, MasterNodePortCategory.Fragment, 4 );
				}
				break;
				case StandardShaderLightModel.BlinnPhong:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1, MasterNodePortCategory.Fragment, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0, MasterNodePortCategory.Fragment, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr, index++, MasterNodePortCategory.Fragment, 2 );
					AddInputPort( WirePortDataType.FLOAT, false, SpecularStr, index++, MasterNodePortCategory.Fragment, 3 );
					AddInputPort( WirePortDataType.FLOAT, false, GlossStr, index++, MasterNodePortCategory.Fragment, 4 );
				}
				break;
			}

			AddInputPort( WirePortDataType.FLOAT3, false, TransmissionStr, index++, MasterNodePortCategory.Fragment, 6 );
			m_transmissionPort = m_inputPorts[ m_inputPorts.Count - 1 ];
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_currentLightModel == StandardShaderLightModel.Standard ) || ( m_currentLightModel == StandardShaderLightModel.StandardSpecular ) ? false : true;

			AddInputPort( WirePortDataType.FLOAT3, false, TranslucencyStr, index++, MasterNodePortCategory.Fragment, 7 );
			m_translucencyPort = m_inputPorts[ m_inputPorts.Count - 1 ];
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_currentLightModel == StandardShaderLightModel.Standard ) || ( m_currentLightModel == StandardShaderLightModel.StandardSpecular ) ? false : true;

			AddInputPort( WirePortDataType.FLOAT, false, RefractionStr, index + 2, MasterNodePortCategory.Fragment, 8 );
			m_refractionPort = m_inputPorts[ m_inputPorts.Count - 1 ];
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_alphaMode == AlphaMode.Opaque || m_alphaMode == AlphaMode.Masked || m_currentLightModel == StandardShaderLightModel.Unlit );

			AddInputPort( WirePortDataType.FLOAT, false, AlphaStr, index++, MasterNodePortCategory.Fragment, 9 );
			m_inputPorts[ m_inputPorts.Count - 1 ].DataName = AlphaDataStr;
			m_opacityPortId = m_inputPorts.Count - 1;
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_alphaMode == AlphaMode.Opaque || m_alphaMode == AlphaMode.Masked );

			AddInputPort( WirePortDataType.OBJECT, false, DiscardStr, index++, MasterNodePortCategory.Fragment, 10 );
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_alphaMode != AlphaMode.Masked && m_alphaMode != AlphaMode.Custom );
			m_discardPortId = m_inputPorts.Count - 1;

			// This is done to take the index + 2 from refraction port into account and not overlap indexes 
			index++;

			////////////////////////////////////////////////////////////////////////////////////////////////
			// Vertex functions - Adding ordex index in order to force these to be the last ones 
			////////////////////////////////////////////////////////////////////////////////////////////////
			m_vertexPortId = m_inputPorts.Count;
			m_tessOpHelper.VertexOffsetIndexPort = m_vertexPortId;
			AddInputPort( WirePortDataType.FLOAT3, false, ( m_vertexMode == VertexMode.Relative ? VertexDisplacementStr : VertexPositionStr ), VertexDataStr, index++, MasterNodePortCategory.Vertex, 11 );
			AddInputPort( WirePortDataType.FLOAT3, false, VertexNormalStr, index++, MasterNodePortCategory.Vertex, 12 );

			//AddInputPort( WirePortDataType.FLOAT3, false, CustomLightModelStr, index++, MasterNodePortCategory.Fragment, 13 );
			//m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;// !(m_currentLightModel == StandardShaderLightModel.CustomLighting);

			AddInputPort( WirePortDataType.FLOAT4, false, TessellationOpHelper.TessellationPortStr, index++, MasterNodePortCategory.Tessellation, 14 );
			m_tessellationPort = m_inputPorts[ m_inputPorts.Count - 1 ];
			m_tessOpHelper.MasterNodeIndexPort = m_tessellationPort.PortId;

			////////////////////////////////////////////////////////////////////////////////////
			AddInputPort( WirePortDataType.FLOAT3, false, DebugStr, index++, MasterNodePortCategory.Debug, 15 );

			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].CustomColor = Color.white;
			}
			m_sizeIsDirty = true;
		}

		public override void ForcePortType()
		{
			int portId = 0;
			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.Standard:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
				case StandardShaderLightModel.StandardSpecular:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
				case StandardShaderLightModel.CustomLighting:
				case StandardShaderLightModel.Unlit:
				case StandardShaderLightModel.Lambert:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
				case StandardShaderLightModel.BlinnPhong:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
			}

			//Transmission
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			//Translucency
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			//Refraction
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
			//Alpha
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
			//Discard
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.OBJECT, false );
			//Vertex Offset
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			//Vertex Normal
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			//Custom Light
			//m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			//Tessellation
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT4, false );
			//Debug
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
		}

		public override void SetName( string name )
		{
			ShaderName = name;
		}

		public void DrawInspectorProperty()
		{
			if ( m_inspectorDefaultStyle == null )
			{
				m_inspectorDefaultStyle = UIUtils.GetCustomStyle( CustomStyle.ResetToDefaultInspectorButton );
			}

			EditorGUILayout.BeginHorizontal();
			m_customInspectorName = EditorGUILayoutTextField( CustomInspectorStr, m_customInspectorName );
			if ( GUILayoutButton( string.Empty, UIUtils.GetCustomStyle( CustomStyle.ResetToDefaultInspectorButton ) ) )
			{
				GUIUtility.keyboardControl = 0;
				m_customInspectorName = Constants.DefaultCustomInspector;
			}
			EditorGUILayout.EndHorizontal();
		}

		public void DrawMaterialInputs( GUIStyle toolbarstyle )
		{
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( toolbarstyle );
			GUI.color = cachedColor;

			EditorGUI.BeginChangeCheck();
			ContainerGraph.ParentWindow.ExpandedProperties = GUILayoutToggle( ContainerGraph.ParentWindow.ExpandedProperties, PropertyOderFoldoutStr, m_inspectorFoldoutStyle );
			if ( EditorGUI.EndChangeCheck() )
			{
				EditorPrefs.SetBool( "ExpandedProperties", ContainerGraph.ParentWindow.ExpandedProperties );
			}

			EditorGUILayout.EndHorizontal();
			if ( !ContainerGraph.ParentWindow.ExpandedProperties )
				return;

			cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
			EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
			GUI.color = cachedColor;
			List<ParentNode> nodes = UIUtils.PropertyNodesList();
			if ( m_propertyReordableList == null || nodes.Count != m_lastCount )
			{
				m_propertyNodes.Clear();

				for ( int i = 0; i < nodes.Count; i++ )
				{
					PropertyNode node = nodes[ i ] as PropertyNode;
					if ( node != null )
					{
						ReordenatorNode rnode = nodes[ i ] as ReordenatorNode;
						if ( ( rnode == null || !rnode.IsInside ) && ( !m_propertyNodes.Exists( x => x.PropertyName.Equals( node.PropertyName ) ) ) )
							m_propertyNodes.Add( node );
					}
				}

				m_propertyNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

				m_propertyReordableList = new ReorderableList( m_propertyNodes, typeof( PropertyNode ), true, false, false, false )
				{
					headerHeight = 0,
					footerHeight = 0,
					showDefaultBackground = false,

					drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
					{
						EditorGUI.LabelField( rect,/* m_propertyNodes[ index ].OrderIndex + " " +*/ m_propertyNodes[ index ].PropertyInspectorName );
					},

					onChangedCallback = ( list ) =>
					{
						ReorderList( ref nodes );
					}
				};
				ReorderList( ref nodes );

				m_lastCount = nodes.Count;
			}

			if ( m_propertyReordableList != null )
			{
				if ( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_propertyReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		}

		private void ReorderList( ref List<ParentNode> nodes )
		{
			for ( int i = 0; i < nodes.Count; i++ )
			{
				ReordenatorNode rnode = nodes[ i ] as ReordenatorNode;
				if ( rnode != null )
					rnode.RecursiveClear();
			}

			int propoffset = 0;
			//int renodeoffset = 0;
			int count = 0;
			for ( int i = 0; i < m_propertyNodes.Count; i++ )
			{
				ReordenatorNode renode = m_propertyNodes[ i ] as ReordenatorNode;
				if ( renode != null )
				{
					if ( !renode.IsInside )
					{
						m_propertyNodes[ i ].OrderIndex = count + propoffset;// - renodeoffset;

						if ( renode.PropertyListCount > 0 )
						{
							propoffset += renode.RecursiveCount();
						}

						for ( int j = 0; j < nodes.Count; j++ )
						{
							ReordenatorNode pnode = ( nodes[ j ] as ReordenatorNode );
							if ( pnode != null && pnode.PropertyName.Equals( renode.PropertyName ) )
							{
								pnode.OrderIndex = renode.RawOrderIndex;
								pnode.RecursiveSetOrderOffset( renode.RawOrderIndex, true );
							}
						}
						count++;
					}
					else
					{
						m_propertyNodes[ i ].OrderIndex = 0;
					}
				}
				else
				{
					m_propertyNodes[ i ].OrderIndex = count + propoffset;// - renodeoffset;
					count++;
				}
			}
		}

		public void DrawGeneralOptions()
		{
			EditorGUI.BeginChangeCheck();
			string newShaderName = EditorGUILayoutTextField( m_shaderNameContent, m_shaderName );
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( newShaderName.Length > 0 )
				{
					newShaderName = UIUtils.RemoveShaderInvalidCharacters( newShaderName );
				}
				else
				{
					newShaderName = DefaultShaderName;
				}
				ShaderName = newShaderName;
				ContainerGraph.ParentWindow.UpdateTabTitle( ShaderName, true );
			}
			m_shaderNameContent.tooltip = m_shaderName;

			DrawCurrentShaderType();

			EditorGUI.BeginChangeCheck();
			m_currentLightModel = ( StandardShaderLightModel ) EditorGUILayoutEnumPopup( LightModelContent, m_currentLightModel );
			if ( EditorGUI.EndChangeCheck() )
			{
				ContainerGraph.ChangedLightingModel = true;
				if ( m_currentLightModel == StandardShaderLightModel.CustomLighting )
				{
					ContainerGraph.ParentWindow.CurrentNodeAvailability = NodeAvailability.CustomLighting;
				}
				else
				{
					ContainerGraph.ParentWindow.CurrentNodeAvailability = NodeAvailability.SurfaceShader;
				}
			}

			m_shaderModelIdx = EditorGUILayoutPopup( ShaderModelStr, m_shaderModelIdx, ShaderModelTypeArr );

			EditorGUI.BeginChangeCheck();
			DrawPrecisionProperty();
			if ( EditorGUI.EndChangeCheck() )
				ContainerGraph.CurrentPrecision = m_currentPrecisionType;

			m_cullMode = ( CullMode ) EditorGUILayoutEnumPopup( CullModeContent, m_cullMode );

			m_renderPath = ( RenderPath ) EditorGUILayoutEnumPopup( RenderPathContent, m_renderPath );

			m_castShadows = EditorGUILayoutToggle( CastShadowsContent, m_castShadows );

			m_receiveShadows = EditorGUILayoutToggle( ReceiveShadowsContent, m_receiveShadows );

			m_queueOrder = EditorGUILayoutIntField( QueueIndexContent, m_queueOrder );
			EditorGUI.BeginChangeCheck();
			m_vertexMode = ( VertexMode ) EditorGUILayoutEnumPopup( VertexModeStr, m_vertexMode );
			if ( EditorGUI.EndChangeCheck() )
			{
				m_inputPorts[ m_vertexPortId ].Name = m_vertexMode == VertexMode.Relative ? VertexDisplacementStr : VertexPositionStr;
				m_sizeIsDirty = true;
			}

			m_shaderLOD = Mathf.Clamp( EditorGUILayoutIntField( ShaderLODContent, m_shaderLOD ), 0, Shader.globalMaximumLOD );
			////m_lodCrossfade = EditorGUILayoutToggle( LODCrossfadeContent, m_lodCrossfade );
			m_fallbackHelper.Draw( this );
			DrawInspectorProperty();

		}
		public override void DrawProperties()
		{
			if ( m_inspectorFoldoutStyle == null || EditorGUIUtility.isProSkin != m_usingProSkin )
				m_inspectorFoldoutStyle = new GUIStyle( GUI.skin.GetStyle( "foldout" ) );

			if ( m_inspectorToolbarStyle == null || EditorGUIUtility.isProSkin != m_usingProSkin )
			{
				m_inspectorToolbarStyle = new GUIStyle( GUI.skin.GetStyle( "toolbarbutton" ) )
				{
					fixedHeight = 20
				};
			}

			if ( m_inspectorTooldropdownStyle == null || EditorGUIUtility.isProSkin != m_usingProSkin )
			{
				m_inspectorTooldropdownStyle = new GUIStyle( GUI.skin.GetStyle( "toolbardropdown" ) )
				{
					fixedHeight = 20
				};
				m_inspectorTooldropdownStyle.margin.bottom = 2;
			}

			if ( EditorGUIUtility.isProSkin != m_usingProSkin )
				m_usingProSkin = EditorGUIUtility.isProSkin;

			base.DrawProperties();
			m_buttonStyle.fixedWidth = 200;
			m_buttonStyle.fixedHeight = 50;

			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.Separator();

				m_titleOpacity = 0.5f;
				m_boxOpacity = ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f );
				m_cachedColor = GUI.color;

				//  General
				bool generalIsVisible = EditorVariablesManager.ExpandedGeneralShaderOptions.Value;
				NodeUtils.DrawPropertyGroup( ref generalIsVisible, GeneralFoldoutStr, DrawGeneralOptions );
				EditorVariablesManager.ExpandedGeneralShaderOptions.Value = generalIsVisible;

				//Blend Mode
				GUI.color = new Color( m_cachedColor.r, m_cachedColor.g, m_cachedColor.b, m_titleOpacity );
				EditorGUILayout.BeginHorizontal( m_inspectorToolbarStyle );
				GUI.color = m_cachedColor;

				bool blendOptionsVisible = GUILayout.Toggle( EditorVariablesManager.ExpandedBlendOptions.Value, AlphaModeContent, UIUtils.MenuItemToggleStyle, GUILayout.ExpandWidth( true ) );
				if ( Event.current.button == Constants.FoldoutMouseId )
				{
					EditorVariablesManager.ExpandedBlendOptions.Value = blendOptionsVisible;
				}


				if ( !EditorGUIUtility.isProSkin )
					GUI.color = new Color( 0.25f, 0.25f, 0.25f, 1f );

				float boxSize = 60;
				switch ( m_alphaMode )
				{
					case AlphaMode.Transparent:
					boxSize = 85;
					break;
					case AlphaMode.Translucent:
					boxSize = 80;
					break;
					case AlphaMode.Premultiply:
					boxSize = 120;
					break;
				}
				EditorGUI.BeginChangeCheck();
				m_alphaMode = ( AlphaMode ) EditorGUILayoutPopup( string.Empty, ( int ) m_alphaMode, FadeModeOptions, UIUtils.InspectorPopdropdownStyle, GUILayout.Width( boxSize ), GUILayout.Height( 19 ) );
				if ( EditorGUI.EndChangeCheck() )
				{
					UpdateFromBlendMode();
				}

				GUI.color = m_cachedColor;
				EditorGUILayout.EndHorizontal();

				m_customBlendAvailable = ( m_alphaMode == AlphaMode.Custom || m_alphaMode == AlphaMode.Opaque );

				if ( EditorVariablesManager.ExpandedBlendOptions.Value )
				{
					GUI.color = new Color( m_cachedColor.r, m_cachedColor.g, m_cachedColor.b, m_boxOpacity );
					EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
					GUI.color = m_cachedColor;
					EditorGUI.indentLevel++;
					EditorGUILayout.Separator();
					EditorGUI.BeginChangeCheck();

					m_renderType = ( RenderType ) EditorGUILayoutEnumPopup( RenderTypeContent, m_renderType );

					m_renderQueue = ( RenderQueue ) EditorGUILayoutEnumPopup( RenderQueueContent, m_renderQueue );

					if ( EditorGUI.EndChangeCheck() )
					{
						if ( m_renderType == RenderType.Opaque && m_renderQueue == RenderQueue.Geometry )
							m_alphaMode = AlphaMode.Opaque;
						else if ( m_renderType == RenderType.TransparentCutout && m_renderQueue == RenderQueue.AlphaTest )
							m_alphaMode = AlphaMode.Masked;
						else if ( m_renderType == RenderType.Transparent && m_renderQueue == RenderQueue.Transparent )
							m_alphaMode = AlphaMode.Transparent;
						else if ( m_renderType == RenderType.Opaque && m_renderQueue == RenderQueue.Transparent )
							m_alphaMode = AlphaMode.Translucent;
						else
							m_alphaMode = AlphaMode.Custom;


						UpdateFromBlendMode();
					}

					bool bufferedEnabled = GUI.enabled;

					GUI.enabled = ( m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom );
					m_inputPorts[ m_discardPortId ].Locked = !GUI.enabled;
					EditorGUI.BeginChangeCheck();
					m_opacityMaskClipValue = EditorGUILayoutFloatField( OpacityMaskClipValueContent, m_opacityMaskClipValue );
					if ( EditorGUI.EndChangeCheck() )
					{
						if ( m_currentMaterial.HasProperty( IOUtils.MaskClipValueName ) )
						{
							m_currentMaterial.SetFloat( IOUtils.MaskClipValueName, m_opacityMaskClipValue );
						}
					}

					GUI.enabled = bufferedEnabled;

					EditorGUI.BeginDisabledGroup( !( m_alphaMode == AlphaMode.Transparent || m_alphaMode == AlphaMode.Premultiply || m_alphaMode == AlphaMode.Translucent || m_alphaMode == AlphaMode.Custom ) );
					m_grabOrder = EditorGUILayoutIntField( RefractionLayerStr, m_grabOrder );
					EditorGUI.EndDisabledGroup();

					EditorGUILayout.Separator();

					if ( !m_customBlendAvailable )
					{
						EditorGUI.indentLevel--;
						EditorGUILayout.HelpBox( "Advanced options are only available for Custom blend modes", MessageType.Warning );
						EditorGUI.indentLevel++;
					}

					EditorGUI.BeginDisabledGroup( !m_customBlendAvailable );
					m_blendOpsHelper.Draw( this, m_customBlendAvailable );
					m_colorMaskHelper.Draw( this );

					EditorGUI.EndDisabledGroup();
					EditorGUILayout.Separator();
					EditorGUI.indentLevel--;
					EditorGUILayout.EndVertical();
				}

				m_stencilBufferHelper.Draw( this, m_inspectorToolbarStyle );
				m_tessOpHelper.Draw( this, m_inspectorToolbarStyle, m_currentMaterial, m_tessellationPort.IsConnected );
				m_outlineHelper.Draw( this, m_inspectorToolbarStyle, m_currentMaterial );
				m_billboardOpHelper.Draw( this );
				m_zBufferHelper.Draw( this, m_inspectorToolbarStyle, m_customBlendAvailable );
				m_renderingOptionsOpHelper.Draw( this );
				m_renderingPlatformOpHelper.Draw( this );
				m_additionalIncludes.Draw( this );
				m_customTagsHelper.Draw( this );
				DrawMaterialInputs( m_inspectorToolbarStyle );
			}

			EditorGUILayout.EndVertical();

			if ( m_currentLightModel != m_lastLightModel )
			{
				CacheCurrentSettings();
				m_lastLightModel = m_currentLightModel;
				DeleteAllInputConnections( true );
				AddMasterPorts();
				ConnectFromCache();
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				if ( m_containerGraph.IsInstancedShader || m_renderingOptionsOpHelper.EnableInstancing )
				{
					DrawInstancedIcon( drawInfo );
				}
			}

			if ( m_initialize )
			{
				m_initialize = false;

				if ( m_dummyProperty == null )
				{
					m_dummyProperty = ScriptableObject.CreateInstance<PropertyNode>();
					m_dummyProperty.ContainerGraph = ContainerGraph;
				}
			}

			if ( m_transmissionPort != null && m_transmissionPort.IsConnected && m_renderPath != RenderPath.ForwardOnly )
			{
				m_renderPath = RenderPath.ForwardOnly;
				UIUtils.ShowMessage( "Render Path changed to Forward Only since transmission only works in forward rendering" );
			}

			if ( m_translucencyPort != null && m_translucencyPort.IsConnected && m_renderPath != RenderPath.ForwardOnly )
			{
				m_renderPath = RenderPath.ForwardOnly;
				UIUtils.ShowMessage( "Render Path changed to Forward Only since translucency only works in forward rendering" );
			}

			if ( m_translucencyPort.IsConnected != m_previousTranslucencyOn )
				m_checkChanges = true;

			if ( m_refractionPort.IsConnected != m_previousRefractionOn )
				m_checkChanges = true;

			if ( ( m_tessOpHelper.EnableTesselation && !m_tessellationPort.IsConnected ) != m_previousTessellationOn )
				m_checkChanges = true;

			m_previousTranslucencyOn = m_translucencyPort.IsConnected;

			m_previousRefractionOn = m_refractionPort.IsConnected;

			m_previousTessellationOn = ( m_tessOpHelper.EnableTesselation && !m_tessellationPort.IsConnected );

			if ( m_checkChanges )
			{
				if ( m_translucencyPort.IsConnected )
				{
					if ( m_translucencyReorder == null )
					{
						List<ParentNode> translucencyList = new List<ParentNode>();
						for ( int i = 0; i < 6; i++ )
						{
							translucencyList.Add( m_dummyProperty );
						}

						m_translucencyReorder = ScriptableObject.CreateInstance<ReordenatorNode>();
						m_translucencyReorder.ContainerGraph = ContainerGraph;
						m_translucencyReorder.OrderIndex = m_translucencyOrderIndex;
						m_translucencyReorder.Init( "_TranslucencyGroup", "Translucency", translucencyList );
					}

					UIUtils.RegisterPropertyNode( m_translucencyReorder );
				}
				else
				{
					if ( m_translucencyReorder != null )
						UIUtils.UnregisterPropertyNode( m_translucencyReorder );
				}

				if ( m_refractionPort.IsConnected )
				{
					if ( m_refractionReorder == null )
					{
						List<ParentNode> refractionList = new List<ParentNode>();
						for ( int i = 0; i < 1; i++ )
						{
							refractionList.Add( m_dummyProperty );
						}

						m_refractionReorder = ScriptableObject.CreateInstance<ReordenatorNode>();
						m_refractionReorder.ContainerGraph = ContainerGraph;
						m_refractionReorder.OrderIndex = m_refractionOrderIndex;
						m_refractionReorder.Init( "_RefractionGroup", "Refraction", refractionList );
					}

					UIUtils.RegisterPropertyNode( m_refractionReorder );
				}
				else
				{
					if ( m_refractionReorder != null )
						UIUtils.UnregisterPropertyNode( m_refractionReorder );
				}

				if ( m_tessOpHelper.EnableTesselation && !m_tessellationPort.IsConnected )
				{
					if ( m_tessellationReorder == null )
					{
						List<ParentNode> tessellationList = new List<ParentNode>();
						for ( int i = 0; i < 4; i++ )
						{
							tessellationList.Add( m_dummyProperty );
						}

						m_tessellationReorder = ScriptableObject.CreateInstance<ReordenatorNode>();
						m_tessellationReorder.ContainerGraph = ContainerGraph;
						m_tessellationReorder.OrderIndex = m_tessellationOrderIndex;
						m_tessellationReorder.Init( "_TessellationGroup", "Tessellation", tessellationList );
						m_tessellationReorder.HeaderTitle = "Tesselation";
					}

					UIUtils.RegisterPropertyNode( m_tessellationReorder );
				}
				else
				{
					if ( m_tessellationReorder != null )
						UIUtils.UnregisterPropertyNode( m_tessellationReorder );
				}

				if ( m_inputPorts[ m_discardPortId ].Available /*( m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom )*/)
				{
					if ( m_maskClipReorder == null )
					{
						// Create dragable clip material property
						m_maskClipReorder = ScriptableObject.CreateInstance<ReordenatorNode>();
						m_maskClipReorder.ContainerGraph = ContainerGraph;
						m_maskClipReorder.OrderIndex = m_maskClipOrderIndex;
						m_maskClipReorder.Init( "_MaskClipValue", "Mask Clip Value", null );
					}

					UIUtils.RegisterPropertyNode( m_maskClipReorder );
				}
				else
				{
					if ( m_maskClipReorder != null )
						UIUtils.UnregisterPropertyNode( m_maskClipReorder );
				}

				m_checkChanges = false;
			}
		}

		private void CacheCurrentSettings()
		{
			m_cacheNodeConnections.Clear();
			for ( int portId = 0; portId < m_inputPorts.Count; portId++ )
			{
				if ( m_inputPorts[ portId ].IsConnected )
				{
					WireReference connection = m_inputPorts[ portId ].GetConnection();
					m_cacheNodeConnections.Add( m_inputPorts[ portId ].Name, new NodeCache( connection.NodeId, connection.PortId ) );
				}
			}
		}

		private void ConnectFromCache()
		{
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				NodeCache cache = m_cacheNodeConnections.Get( m_inputPorts[ i ].Name );
				if ( cache != null )
				{
					UIUtils.SetConnection( UniqueId, m_inputPorts[ i ].PortId, cache.TargetNodeId, cache.TargetPortId );
				}
			}
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom )
			{
				if ( mat.HasProperty( IOUtils.MaskClipValueName ) )
					mat.SetFloat( IOUtils.MaskClipValueName, m_opacityMaskClipValue );
			}
		}

		public override void SetMaterialMode( Material mat, bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat, fetchMaterialValues );
			if ( m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom )
			{
				if ( fetchMaterialValues && m_materialMode && mat.HasProperty( IOUtils.MaskClipValueName ) )
				{
					m_opacityMaskClipValue = mat.GetFloat( IOUtils.MaskClipValueName );
				}
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			m_tessOpHelper.UpdateFromMaterial( material );
			m_outlineHelper.UpdateFromMaterial( material );

			if ( m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom )
			{
				if ( material.HasProperty( IOUtils.MaskClipValueName ) )
					m_opacityMaskClipValue = material.GetFloat( IOUtils.MaskClipValueName );
			}
		}

		public override void UpdateMasterNodeMaterial( Material material )
		{
			m_currentMaterial = material;
			UpdateMaterialEditor();
		}

		void UpdateMaterialEditor()
		{
			FireMaterialChangedEvt();
		}

		public string CreateInstructionsForVertexPort( InputPort port )
		{
			//Vertex displacement and per vertex custom data
			m_currentDataCollector.PortCategory = MasterNodePortCategory.Vertex;
			WireReference connection = port.GetConnection();
			ParentNode node = UIUtils.GetNode( connection.NodeId );

			string vertexInstructions = node.GetValueFromOutputStr( connection.PortId, port.DataType, ref m_currentDataCollector, false );

			if ( m_currentDataCollector.DirtySpecialLocalVariables )
			{
				m_currentDataCollector.AddVertexInstruction( m_currentDataCollector.SpecialLocalVariables, UniqueId, false );
				m_currentDataCollector.ClearSpecialLocalVariables();
			}

			if ( m_currentDataCollector.DirtyVertexVariables )
			{
				m_currentDataCollector.AddVertexInstruction( m_currentDataCollector.VertexLocalVariables, UniqueId, false );
				m_currentDataCollector.ClearVertexLocalVariables();
			}

			return vertexInstructions;
		}

		public void CreateInstructionsForPort( InputPort port, string portName, bool addCustomDelimiters = false, string customDelimiterIn = null, string customDelimiterOut = null, bool ignoreLocalVar = false, bool normalIsConnected = false )
		{
			WireReference connection = port.GetConnection();
			ParentNode node = UIUtils.GetNode( connection.NodeId );

			string newInstruction = node.GetValueFromOutputStr( connection.PortId, port.DataType, ref m_currentDataCollector, ignoreLocalVar );

			if ( m_currentDataCollector.DirtySpecialLocalVariables )
			{
				m_currentDataCollector.AddInstructions( m_currentDataCollector.SpecialLocalVariables );
				m_currentDataCollector.ClearSpecialLocalVariables();
			}

			if ( m_currentDataCollector.DirtyVertexVariables )
			{
				m_currentDataCollector.AddVertexInstruction( m_currentDataCollector.VertexLocalVariables, port.NodeId, false );
				m_currentDataCollector.ClearVertexLocalVariables();
			}

			if ( m_currentDataCollector.ForceNormal && !normalIsConnected )
			{
				m_currentDataCollector.AddToStartInstructions( "\t\t\t" + Constants.OutputVarStr + ".Normal = float3(0,0,1);\n" );
				m_currentDataCollector.DirtyNormal = true;
				m_currentDataCollector.ForceNormal = false;
			}

			m_currentDataCollector.AddInstructions( addCustomDelimiters ? customDelimiterIn : ( "\t\t\t" + portName + " = " ) );
			m_currentDataCollector.AddInstructions( newInstruction );
			m_currentDataCollector.AddInstructions( addCustomDelimiters ? customDelimiterOut : ";\n" );
		}

		public override Shader Execute( string pathname, bool isFullPath )
		{
			ForcePortType();
			base.Execute( pathname, isFullPath );

			bool isInstancedShader = m_renderingOptionsOpHelper.EnableInstancing || UIUtils.IsInstancedShader();
			bool hasVirtualTexture = UIUtils.HasVirtualTexture();
			bool hasTranslucency = false;
			bool hasTransmission = false;
			bool hasEmission = false;
			bool hasOpacity = false;
			bool hasRefraction = false;
			bool hasVertexOffset = false;
			//bool hasCustomLightingAlpha = false;
			bool hasCustomLightingMask = false;

			string customLightingCode = string.Empty;
			string customLightingAlphaCode = string.Empty;
			string customLightingMaskCode = string.Empty;
			string customLightingInstructions = string.Empty;

			string refractionCode = string.Empty;
			string refractionInstructions = string.Empty;
			string refractionFix = string.Empty;
			
			m_currentDataCollector.TesselationActive = m_tessOpHelper.EnableTesselation;
			m_currentDataCollector.CurrentRenderPath = m_renderPath;


			if ( isInstancedShader )
			{
				m_currentDataCollector.AddToPragmas( UniqueId, IOUtils.InstancedPropertiesHeader );
			}

			// See if each node is being used on frag and/or vert ports
			SetupNodeCategories();
			if ( m_refractionPort.IsConnected || m_inputPorts[ m_inputPorts.Count - 1 ].IsConnected )
			{
				m_currentDataCollector.DirtyNormal = true;
				m_currentDataCollector.ForceNormal = true;
			}
			//this.PropagateNodeData( nodeData );

			string tags = "\"RenderType\" = \"{0}\"  \"Queue\" = \"{1}\"";
			tags = string.Format( tags, m_renderType, ( m_renderQueue + ( ( m_queueOrder >= 0 ) ? "+" : string.Empty ) + m_queueOrder ) );
			//if ( !m_customBlendMode )
			{
				if ( m_alphaMode == AlphaMode.Transparent || m_alphaMode == AlphaMode.Premultiply )
				{
					//tags += " \"IgnoreProjector\" = \"True\"";
					if ( !m_renderingOptionsOpHelper.IgnoreProjectorValue )
					{
						Debug.Log( string.Format( "Setting Ignore Projector to True since it's requires by Blend Mode {0}.", m_alphaMode ) );
						m_renderingOptionsOpHelper.IgnoreProjectorValue = true;
					}
				}
			}

			tags += m_renderingOptionsOpHelper.IgnoreProjectorTag;
			tags += m_renderingOptionsOpHelper.ForceNoShadowCastingTag;
			tags += m_renderingOptionsOpHelper.DisableBatchingTag;

			//add virtual texture support
			if ( hasVirtualTexture )
			{
				tags += " \"Amplify\" = \"True\" ";
			}
			
			//tags = "Tags{ " + tags + " }";

			string outputStruct = "";
			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.CustomLighting: outputStruct = "SurfaceOutputCustomLightingCustom"; break;
				case StandardShaderLightModel.Standard: outputStruct = "SurfaceOutputStandard"; break;
				case StandardShaderLightModel.StandardSpecular: outputStruct = "SurfaceOutputStandardSpecular"; break;
				case StandardShaderLightModel.Unlit:
				case StandardShaderLightModel.Lambert:
				case StandardShaderLightModel.BlinnPhong: outputStruct = "SurfaceOutput"; break;
			}

			if ( m_currentLightModel == StandardShaderLightModel.CustomLighting )
			{
				m_currentDataCollector.AddToIncludes( UniqueId, Constants.UnityPBSLightingLib );

				m_currentDataCollector.ChangeCustomInputHeader( m_currentLightModel.ToString() + Constants.CustomLightStructStr );
				m_currentDataCollector.AddToCustomInput( UniqueId, "fixed3 Albedo", true );
				m_currentDataCollector.AddToCustomInput( UniqueId, "fixed3 Normal", true );
				m_currentDataCollector.AddToCustomInput( UniqueId, "half3 Emission", true );
				m_currentDataCollector.AddToCustomInput( UniqueId, "half Metallic", true );
				m_currentDataCollector.AddToCustomInput( UniqueId, "half Smoothness", true );
				m_currentDataCollector.AddToCustomInput( UniqueId, "half Occlusion", true );
				m_currentDataCollector.AddToCustomInput( UniqueId, "fixed Alpha", true );
				m_currentDataCollector.AddToCustomInput( UniqueId, "Input SurfInput", true );
				m_currentDataCollector.AddToCustomInput( UniqueId, "UnityGIInput GIData", true );
			}

			// Need to sort before creating local vars so they can inspect the normal port correctly
			SortedList<int, InputPort> sortedPorts = new SortedList<int, InputPort>();
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				sortedPorts.Add( m_inputPorts[ i ].OrderId, m_inputPorts[ i ] );
			}

			bool normalIsConnected = sortedPorts[ 0 ].IsConnected;

			m_tessOpHelper.Reset();
			if ( m_inputPorts[ m_inputPorts.Count - 1 ].IsConnected )
			{
				//Debug Port active
				m_currentDataCollector.PortCategory = MasterNodePortCategory.Debug;
				InputPort debugPort = m_inputPorts[ m_inputPorts.Count - 1 ];

				CreateInstructionsForPort( debugPort, Constants.OutputVarStr + ".Emission", false, null, null, false, false );
			}
			else
			{
				// Custom Light Model
				//TODO: Create Custom Light behaviour
				//Collect data from standard nodes
				for ( int i = 0; i < sortedPorts.Count; i++ )
				{
					if ( sortedPorts[ i ].IsConnected )
					{
						if ( i == 0 )// Normal Map is Connected
						{
							m_currentDataCollector.DirtyNormal = true;
						}
						if ( sortedPorts[ i ].Name.Equals( TranslucencyStr ) )
						{
							hasTranslucency = true;
						}
						if ( sortedPorts[ i ].Name.Equals( TransmissionStr ) )
						{
							hasTransmission = true;
						}
						if ( sortedPorts[ i ].Name.Equals( EmissionStr ) )
						{
							hasEmission = true;
						}

						if ( sortedPorts[ i ].Name.Equals( RefractionStr ) )
						{
							hasRefraction = true;
						}

						if ( sortedPorts[ i ].Name.Equals( AlphaStr ) )
						{
							hasOpacity = true;
						}

						if ( hasRefraction )
						{
							m_currentDataCollector.AddToInput( UniqueId, "float4 screenPos", true );
							m_currentDataCollector.AddToInput( UniqueId, "float3 worldPos", true );

							//not necessary, just being safe
							m_currentDataCollector.DirtyNormal = true;
							m_currentDataCollector.ForceNormal = true;

							if ( m_grabOrder != 0 )
							{
								m_currentDataCollector.AddGrabPass( "RefractionGrab" + m_grabOrder );
								m_currentDataCollector.AddToUniforms( UniqueId, "uniform sampler2D RefractionGrab" + m_grabOrder + ";" );
							}
							else
							{
								m_currentDataCollector.AddGrabPass( "" );
								m_currentDataCollector.AddToUniforms( UniqueId, "uniform sampler2D _GrabTexture;" );
							}

							m_currentDataCollector.AddToUniforms( UniqueId, "uniform float _ChromaticAberration;" );

							m_currentDataCollector.AddToProperties( UniqueId, "[Header(Refraction)]", m_refractionReorder.OrderIndex );
							m_currentDataCollector.AddToProperties( UniqueId, "_ChromaticAberration(\"Chromatic Aberration\", Range( 0 , 0.3)) = 0.1", m_refractionReorder.OrderIndex + 1 );

							m_currentDataCollector.AddToPragmas( UniqueId, "multi_compile _ALPHAPREMULTIPLY_ON" );
						}

						if ( hasTranslucency || hasTransmission )
						{
							//Translucency and Transmission Generation

							//Add properties and uniforms
							m_currentDataCollector.AddToIncludes( UniqueId, Constants.UnityPBSLightingLib );

							if ( hasTranslucency )
							{
								m_currentDataCollector.AddToProperties( UniqueId, "[Header(Translucency)]", m_translucencyReorder.OrderIndex );
								m_currentDataCollector.AddToProperties( UniqueId, "_Translucency(\"Strength\", Range( 0 , 50)) = 1", m_translucencyReorder.OrderIndex + 1 );
								m_currentDataCollector.AddToProperties( UniqueId, "_TransNormalDistortion(\"Normal Distortion\", Range( 0 , 1)) = 0.1", m_translucencyReorder.OrderIndex + 2 );
								m_currentDataCollector.AddToProperties( UniqueId, "_TransScattering(\"Scaterring Falloff\", Range( 1 , 50)) = 2", m_translucencyReorder.OrderIndex + 3 );
								m_currentDataCollector.AddToProperties( UniqueId, "_TransDirect(\"Direct\", Range( 0 , 1)) = 1", m_translucencyReorder.OrderIndex + 4 );
								m_currentDataCollector.AddToProperties( UniqueId, "_TransAmbient(\"Ambient\", Range( 0 , 1)) = 0.2", m_translucencyReorder.OrderIndex + 5 );
								m_currentDataCollector.AddToProperties( UniqueId, "_TransShadow(\"Shadow\", Range( 0 , 1)) = 0.9", m_translucencyReorder.OrderIndex + 6 );

								m_currentDataCollector.AddToUniforms( UniqueId, "uniform half _Translucency;" );
								m_currentDataCollector.AddToUniforms( UniqueId, "uniform half _TransNormalDistortion;" );
								m_currentDataCollector.AddToUniforms( UniqueId, "uniform half _TransScattering;" );
								m_currentDataCollector.AddToUniforms( UniqueId, "uniform half _TransDirect;" );
								m_currentDataCollector.AddToUniforms( UniqueId, "uniform half _TransAmbient;" );
								m_currentDataCollector.AddToUniforms( UniqueId, "uniform half _TransShadow;" );
							}

							//Add custom struct
							switch ( m_currentLightModel )
							{
								case StandardShaderLightModel.Standard:
								case StandardShaderLightModel.StandardSpecular:
								outputStruct = "SurfaceOutput" + m_currentLightModel.ToString() + Constants.CustomLightStructStr; break;
							}

							m_currentDataCollector.ChangeCustomInputHeader( m_currentLightModel.ToString() + Constants.CustomLightStructStr );
							m_currentDataCollector.AddToCustomInput( UniqueId, "fixed3 Albedo", true );
							m_currentDataCollector.AddToCustomInput( UniqueId, "fixed3 Normal", true );
							m_currentDataCollector.AddToCustomInput( UniqueId, "half3 Emission", true );
							switch ( m_currentLightModel )
							{
								case StandardShaderLightModel.Standard:
								m_currentDataCollector.AddToCustomInput( UniqueId, "half Metallic", true );
								break;
								case StandardShaderLightModel.StandardSpecular:
								m_currentDataCollector.AddToCustomInput( UniqueId, "fixed3 Specular", true );
								break;
							}
							m_currentDataCollector.AddToCustomInput( UniqueId, "half Smoothness", true );
							m_currentDataCollector.AddToCustomInput( UniqueId, "half Occlusion", true );
							m_currentDataCollector.AddToCustomInput( UniqueId, "fixed Alpha", true );
							if ( hasTranslucency )
								m_currentDataCollector.AddToCustomInput( UniqueId, "fixed3 Translucency", true );

							if ( hasTransmission )
								m_currentDataCollector.AddToCustomInput( UniqueId, "fixed3 Transmission", true );
						}

						if ( sortedPorts[ i ].Name.Equals( DiscardStr ) )
						{
							//Discard Op Node
							m_currentDataCollector.PortCategory = MasterNodePortCategory.Fragment;
							string opacityValue = "0.0";
							switch ( sortedPorts[ i ].ConnectionType() )
							{
								case WirePortDataType.INT:
								case WirePortDataType.FLOAT:
								{
									opacityValue = IOUtils.MaskClipValueName;//UIUtils.FloatToString( m_opacityMaskClipValue );
								}
								break;

								case WirePortDataType.FLOAT2:
								{
									opacityValue = string.Format( "( {0} ).xx", IOUtils.MaskClipValueName );
								}
								break;

								case WirePortDataType.FLOAT3:
								{
									opacityValue = string.Format( "( {0} ).xxx", IOUtils.MaskClipValueName );
								}
								break;

								case WirePortDataType.FLOAT4:
								{
									opacityValue = string.Format( "( {0} ).xxxx", IOUtils.MaskClipValueName );
								}
								break;
							}

							if ( m_currentLightModel == StandardShaderLightModel.CustomLighting )
							{
								hasCustomLightingMask = true;
								m_currentDataCollector.UsingCustomOutput = true;
								WireReference connection = sortedPorts[ i ].GetConnection();
								ParentNode node = UIUtils.GetNode( connection.NodeId );

								customLightingMaskCode = node.GetValueFromOutputStr( connection.PortId, WirePortDataType.FLOAT, ref m_currentDataCollector, false );
								customLightingMaskCode = "clip( " + customLightingMaskCode + " - " + opacityValue + " )";
								customLightingInstructions = m_currentDataCollector.CustomOutput;

								m_currentDataCollector.UsingCustomOutput = false;
								continue;
							} else
							{
								CreateInstructionsForPort( sortedPorts[ i ], Constants.OutputVarStr + "." + sortedPorts[ i ].DataName, true, "\t\t\tclip( ", " - " + opacityValue + " );\n", false, normalIsConnected );
							}
						}
						else if ( sortedPorts[ i ].DataName.Equals( VertexDataStr ) )
						{
							hasVertexOffset = true;
							ContainerGraph.ResetNodesLocalVariablesIfNot( MasterNodePortCategory.Vertex );
							string vertexInstructions = CreateInstructionsForVertexPort( sortedPorts[ i ] );
							m_currentDataCollector.AddToVertexDisplacement( vertexInstructions, m_vertexMode );
						}
						else if ( sortedPorts[ i ].DataName.Equals( VertexNormalStr ) )
						{
							if ( !hasVertexOffset )
								ContainerGraph.ResetNodesLocalVariablesIfNot( MasterNodePortCategory.Vertex );

							string vertexInstructions = CreateInstructionsForVertexPort( sortedPorts[ i ] );
							m_currentDataCollector.AddToVertexNormal( vertexInstructions );
						}
						else if ( m_tessOpHelper.EnableTesselation && m_tessOpHelper.IsTessellationPort( sortedPorts[ i ].PortId ) )
						{
							ContainerGraph.ResetNodesLocalVariablesIfNot( MasterNodePortCategory.Vertex );
							//Vertex displacement and per vertex custom data
							m_currentDataCollector.PortCategory = MasterNodePortCategory.Tessellation;
							WireReference connection = sortedPorts[ i ].GetConnection();
							ParentNode node = UIUtils.GetNode( connection.NodeId );

							string vertexInstructions = node.GetValueFromOutputStr( connection.PortId, sortedPorts[ i ].DataType, ref m_currentDataCollector, false );

							if ( m_currentDataCollector.DirtySpecialLocalVariables )
							{
								m_tessOpHelper.AddAdditionalData( m_currentDataCollector.SpecialLocalVariables );
								m_currentDataCollector.ClearSpecialLocalVariables();
							}

							if ( m_currentDataCollector.DirtyVertexVariables )
							{
								m_tessOpHelper.AddAdditionalData( m_currentDataCollector.VertexLocalVariables );
								m_currentDataCollector.ClearVertexLocalVariables();
							}

							m_tessOpHelper.AddCustomFunction( vertexInstructions );
						}
						else if ( sortedPorts[ i ].Name.Equals( RefractionStr ) )
						{
							ContainerGraph.ResetNodesLocalVariables();
							m_currentDataCollector.UsingCustomOutput = true;

							refractionFix = " + 0.00001 * i.screenPos * i.worldPos";
							m_currentDataCollector.AddInstructions( "\t\t\to.Normal = o.Normal" + refractionFix + ";\n" );
							WireReference connection = sortedPorts[ i ].GetConnection();
							ParentNode node = UIUtils.GetNode( connection.NodeId );

							refractionCode = node.GetValueFromOutputStr( connection.PortId, sortedPorts[ i ].DataType, ref m_currentDataCollector, true );
							refractionInstructions = m_currentDataCollector.CustomOutput;

							m_currentDataCollector.UsingCustomOutput = false;
						}
						else if ( sortedPorts[ i ].Name.Equals( EmissionStr ) && m_currentLightModel == StandardShaderLightModel.CustomLighting )
						{
							//if ( !hasCustomLightingAlpha )
							//	ContainerGraph.ResetNodesLocalVariables();

							m_currentDataCollector.UsingCustomOutput = true;
							WireReference connection = sortedPorts[ i ].GetConnection();
							ParentNode node = UIUtils.GetNode( connection.NodeId );

							customLightingCode = node.GetValueFromOutputStr( connection.PortId, WirePortDataType.FLOAT3, ref m_currentDataCollector, false );
							customLightingInstructions = m_currentDataCollector.CustomOutput;

							if ( m_currentDataCollector.ForceNormal )
							{
								m_currentDataCollector.AddToStartInstructions( "\t\t\t" + Constants.OutputVarStr + ".Normal = float3(0,0,1);\n" );
								m_currentDataCollector.DirtyNormal = true;
								m_currentDataCollector.ForceNormal = false;
							}

							m_currentDataCollector.UsingCustomOutput = false;
						}
						else if ( sortedPorts[ i ].Name.Equals( AlphaStr ) && m_currentLightModel == StandardShaderLightModel.CustomLighting )
						{
							//hasCustomLightingAlpha = true;
							//ContainerGraph.ResetNodesLocalVariables();
							m_currentDataCollector.UsingCustomOutput = true;

							WireReference connection = sortedPorts[ i ].GetConnection();
							ParentNode node = UIUtils.GetNode( connection.NodeId );

							customLightingAlphaCode = node.GetValueFromOutputStr( connection.PortId, WirePortDataType.FLOAT, ref m_currentDataCollector, false );
							customLightingInstructions = m_currentDataCollector.CustomOutput;

							m_currentDataCollector.UsingCustomOutput = false;
						}
						else
						{
							// Surface shader instruccions
							m_currentDataCollector.PortCategory = MasterNodePortCategory.Fragment;
							// if working on normals and have normal dependent node then ignore local var generation
							//bool ignoreLocalVar = ( i == 0 && UIUtils.IsNormalDependent() );

							CreateInstructionsForPort( sortedPorts[ i ], Constants.OutputVarStr + "." + sortedPorts[ i ].DataName, false, null, null, false, normalIsConnected );
						}
					}
					else if ( sortedPorts[ i ].Name.Equals( AlphaStr ) )
					{
						if ( m_currentLightModel != StandardShaderLightModel.CustomLighting )
						{
							m_currentDataCollector.AddInstructions( string.Format( "\t\t\t{0}.{1} = 1;\n", Constants.OutputVarStr, sortedPorts[ i ].DataName ) );
						}
					}
				}

				m_billboardOpHelper.FillDataCollectorWithInternalData( ref m_currentDataCollector );
			}


			if ( ( m_castShadows && hasOpacity ) || ( m_castShadows && ( m_currentDataCollector.UsingWorldNormal || m_currentDataCollector.UsingWorldReflection || m_currentDataCollector.UsingViewDirection ) ) )
				m_customShadowCaster = true;
			else
				m_customShadowCaster = false;


			for ( int i = 0; i < 4; i++ )
			{
				if ( m_currentDataCollector.GetChannelUsage( i ) == TextureChannelUsage.Required )
				{
					string channelName = UIUtils.GetChannelName( i );
					m_currentDataCollector.AddToProperties( -1, UIUtils.GetTex2DProperty( channelName, TexturePropertyValues.white ), -1 );
				}
			}

			m_currentDataCollector.AddToProperties( -1, IOUtils.DefaultASEDirtyCheckProperty, -2 );
			if ( m_inputPorts[ m_discardPortId ].Available /*m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom*/ )
			{
				m_currentDataCollector.AddToProperties( -1, string.Format( IOUtils.MaskClipValueProperty, OpacityMaskClipValueStr, m_opacityMaskClipValue ), ( m_maskClipReorder != null ) ? m_maskClipReorder.OrderIndex : -1 );
				m_currentDataCollector.AddToUniforms( -1, string.Format( IOUtils.MaskClipValueUniform, m_opacityMaskClipValue ) );
			}

			if ( !m_currentDataCollector.DirtyInputs )
				m_currentDataCollector.AddToInput( UniqueId, "fixed filler", true );
			
			if ( m_currentLightModel == StandardShaderLightModel.BlinnPhong )
				m_currentDataCollector.AddToProperties( -1, "[HideInInspector]_SpecColor(\"SpecularColor\",Color)=(1,1,1,1)", -1 );

			//Tesselation
			if ( m_tessOpHelper.EnableTesselation )
			{
				m_tessOpHelper.AddToDataCollector( ref m_currentDataCollector, m_tessellationReorder != null ? m_tessellationReorder.OrderIndex : -1 );
				if ( !m_currentDataCollector.DirtyPerVertexData )
				{
					m_currentDataCollector.OpenPerVertexHeader( false );
				}
			}

			if ( m_outlineHelper.EnableOutline )
			{
				m_outlineHelper.AddToDataCollector( ref m_currentDataCollector );
			}

#if !UNITY_2017_1_OR_NEWER
			if ( m_renderingOptionsOpHelper.LodCrossfade )
			{
				m_currentDataCollector.AddToPragmas( UniqueId, "multi_compile _ LOD_FADE_CROSSFADE" );
				m_currentDataCollector.AddToStartInstructions( "\t\t\tUNITY_APPLY_DITHER_CROSSFADE(i);\n" );
				m_currentDataCollector.AddToInput( UniqueId, "UNITY_DITHER_CROSSFADE_COORDS", false );
				m_currentDataCollector.AddVertexInstruction( "UNITY_TRANSFER_DITHER_CROSSFADE( " + Constants.VertexShaderOutputStr + ", " + Constants.VertexShaderInputStr + ".vertex )", UniqueId, true );
			}
#endif
			m_additionalIncludes.AddToDataCollector( ref m_currentDataCollector );

			m_currentDataCollector.CloseInputs();
			m_currentDataCollector.CloseCustomInputs();
			m_currentDataCollector.CloseProperties();
			m_currentDataCollector.ClosePerVertexHeader();

			//build Shader Body
			string ShaderBody = string.Empty;
			OpenShaderBody( ref ShaderBody, m_shaderName );
			{
				//set properties
				if ( m_currentDataCollector.DirtyProperties )
				{
					ShaderBody += m_currentDataCollector.BuildPropertiesString();
				}
				//set subshader
				OpenSubShaderBody( ref ShaderBody );
				{
					// Add optionalPasses
					if ( m_outlineHelper.EnableOutline )
					{
						AddMultilineBody( ref ShaderBody, m_outlineHelper.OutlineFunctionBody( isInstancedShader, m_customShadowCaster, UIUtils.RemoveInvalidCharacters( ShaderName ), ( m_billboardOpHelper.IsBillboard ? m_billboardOpHelper.GetInternalMultilineInstructions() : null ) ) );
					}

					//Add SubShader tags
					if ( hasEmission )
					{
						tags += " \"IsEmissive\" = \"true\" ";
					}

					tags += m_customTagsHelper.GenerateCustomTags();

					tags = "Tags{ " + tags + " }";
					AddRenderTags( ref ShaderBody, tags );
					AddShaderLOD( ref ShaderBody, m_shaderLOD );
					AddRenderState( ref ShaderBody, "Cull", m_cullMode.ToString() );
					m_customBlendAvailable = ( m_alphaMode == AlphaMode.Custom || m_alphaMode == AlphaMode.Opaque );
					if ( m_zBufferHelper.IsActive && m_customBlendAvailable )
					{
						ShaderBody += m_zBufferHelper.CreateDepthInfo();
					}
					if ( m_stencilBufferHelper.Active )
					{
						ShaderBody += m_stencilBufferHelper.CreateStencilOp();
					}

					if ( m_blendOpsHelper.Active )
					{
						ShaderBody += m_blendOpsHelper.CreateBlendOps();
					}

					// Build Color Mask
					m_colorMaskHelper.BuildColorMask( ref ShaderBody, m_customBlendAvailable );

					//ShaderBody += "\t\tZWrite " + _zWriteMode + '\n';
					//ShaderBody += "\t\tZTest " + _zTestMode + '\n';

					//Add GrabPass
					if ( m_currentDataCollector.DirtyGrabPass )
					{
						ShaderBody += m_currentDataCollector.GrabPass;
					}

					// build optional parameters
					string OptionalParameters = string.Empty;

					// addword standard to custom lighting to accepts standard lighting models
					string standardCustomLighting = string.Empty;
					if ( m_currentLightModel == StandardShaderLightModel.CustomLighting )
						standardCustomLighting = "Standard";

					//add cg program
					if ( m_customShadowCaster )
						OpenCGInclude( ref ShaderBody );
					else
						OpenCGProgram( ref ShaderBody );
					{
						//Add Includes
						if ( m_customShadowCaster )
						{
							m_currentDataCollector.AddToIncludes( UniqueId, Constants.UnityPBSLightingLib );
							m_currentDataCollector.AddToIncludes( UniqueId, "Lighting.cginc" );
						}
						if ( m_currentDataCollector.DirtyIncludes )
							ShaderBody += m_currentDataCollector.Includes;

						//define as surface shader and specify lighting model
						if ( UIUtils.GetTextureArrayNodeAmount() > 0 && m_shaderModelIdx < 3 )
						{
							Debug.Log( "Automatically changing Shader Model to 3.5 since it's the minimum required by texture arrays." );
							m_shaderModelIdx = 3;
						}

						// if tessellation is active then we need be at least using shader model 4.6
						if ( m_tessOpHelper.EnableTesselation && m_shaderModelIdx < 6 )
						{
							Debug.Log( "Automatically changing Shader Model to 4.6 since it's the minimum required by tessellation." );
							m_shaderModelIdx = 6;
						}

						// if translucency is ON change render path
						if ( hasTranslucency && m_renderPath != RenderPath.ForwardOnly )
						{
							Debug.Log( "Automatically changing Render Path to Forward Only since translucency only works in forward rendering." );
							m_renderPath = RenderPath.ForwardOnly;
						}

						// if transmission is ON change render path
						if ( hasTransmission && m_renderPath != RenderPath.ForwardOnly )
						{
							Debug.Log( "Automatically changing Render Path to Forward Only since transmission only works in forward rendering." );
							m_renderPath = RenderPath.ForwardOnly;
						}

						// if refraction is ON change render path
						if ( hasRefraction && m_renderPath != RenderPath.ForwardOnly )
						{
							Debug.Log( "Automatically changing Render Path to Forward Only since refraction only works in forward rendering." );
							m_renderPath = RenderPath.ForwardOnly;
						}

						ShaderBody += string.Format( IOUtils.PragmaTargetHeader, ShaderModelTypeArr[ m_shaderModelIdx ] );


						//Add pragmas (needs check to see if all pragmas work with custom shadow caster)
						if ( m_currentDataCollector.DirtyPragmas/* && !m_customShadowCaster */)
							ShaderBody += m_currentDataCollector.Pragmas;

						//if ( !m_customBlendMode )
						{
							switch ( m_alphaMode )
							{
								case AlphaMode.Opaque:
								case AlphaMode.Masked: break;
								case AlphaMode.Transparent:
								{
									OptionalParameters += "alpha:fade" + Constants.OptionalParametersSep;
								}
								break;
								case AlphaMode.Premultiply:
								{
									OptionalParameters += "alpha:premul" + Constants.OptionalParametersSep;
								}
								break;
							}
						}

						if ( m_keepAlpha )
						{
							OptionalParameters += "keepalpha" + Constants.OptionalParametersSep;
						}

						if ( hasRefraction )
						{
							OptionalParameters += "finalcolor:RefractionF" + Constants.OptionalParametersSep;
						}

						if ( !m_customShadowCaster && m_castShadows )
						{
							OptionalParameters += "addshadow" + Constants.OptionalParametersSep;
						}

						if ( m_castShadows )
						{
							OptionalParameters += "fullforwardshadows" + Constants.OptionalParametersSep;
						}

						if ( !m_receiveShadows )
						{
							OptionalParameters += "noshadow" + Constants.OptionalParametersSep;
						}

						switch ( m_renderPath )
						{
							case RenderPath.All: break;
							case RenderPath.DeferredOnly: OptionalParameters += "exclude_path:forward" + Constants.OptionalParametersSep; break;
							case RenderPath.ForwardOnly: OptionalParameters += "exclude_path:deferred" + Constants.OptionalParametersSep; break;
						}

						//Add code generation options
						m_renderingOptionsOpHelper.Build( ref OptionalParameters );

						if ( !m_customShadowCaster )
						{
							string customLightSurface = string.Empty;
							if ( hasTranslucency || hasTransmission )
								customLightSurface = "Custom";
							m_renderingPlatformOpHelper.SetRenderingPlatforms( ref ShaderBody );

							//Check if Custom Vertex is being used and add tag
							if ( m_currentDataCollector.DirtyPerVertexData )
								OptionalParameters += "vertex:" + Constants.VertexDataFunc + Constants.OptionalParametersSep;

							if ( m_tessOpHelper.EnableTesselation )
							{
								m_tessOpHelper.WriteToOptionalParams( ref OptionalParameters );
							}

							AddShaderPragma( ref ShaderBody, "surface surf " + standardCustomLighting + m_currentLightModel.ToString() + customLightSurface + Constants.OptionalParametersSep + OptionalParameters );
						}
						else
						{
							if ( m_currentDataCollector.UsingWorldNormal || m_currentDataCollector.UsingInternalData )
							{
								ShaderBody += "\t\t#ifdef UNITY_PASS_SHADOWCASTER\n";
								ShaderBody += "\t\t\t#undef INTERNAL_DATA\n";
								ShaderBody += "\t\t\t#undef WorldReflectionVector\n";
								ShaderBody += "\t\t\t#undef WorldNormalVector\n";
								ShaderBody += "\t\t\t#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;\n";
								ShaderBody += "\t\t\t#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))\n";
								ShaderBody += "\t\t\t#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))\n";
								ShaderBody += "\t\t#endif\n";
							}
						}

						if ( m_currentDataCollector.UsingHigherSizeTexcoords )
						{
							ShaderBody += "\t\t#undef TRANSFORM_TEX\n";
							ShaderBody += "\t\t#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)\n";
						}


						// Add Input struct
						if ( m_currentDataCollector.DirtyInputs )
							ShaderBody += m_currentDataCollector.Inputs + "\n\n";

						if ( m_tessOpHelper.EnableTesselation )
						{
							ShaderBody += TessellationOpHelper.CustomAppData;
						}

						// Add Custom Lighting struct
						if ( m_currentDataCollector.DirtyCustomInput )
							ShaderBody += m_currentDataCollector.CustomInput + "\n\n";

						//Add Uniforms
						if ( m_currentDataCollector.DirtyUniforms )
							ShaderBody += m_currentDataCollector.Uniforms + "\n";

						// Add Array Derivatives Macros
						if ( m_currentDataCollector.UsingArrayDerivatives )
						{
							ShaderBody += "\t\t#if defined(UNITY_COMPILER_HLSL2GLSL) || defined(SHADER_TARGET_SURFACE_ANALYSIS)\n";
							ShaderBody += "\t\t\t#define ASE_SAMPLE_TEX2DARRAY_GRAD(tex,coord,dx,dy) tex2DArray (tex,coord)\n";
							ShaderBody += "\t\t#else\n";
							ShaderBody += "\t\t\t#define ASE_SAMPLE_TEX2DARRAY_GRAD(tex,coord,dx,dy) tex.SampleGrad (sampler##tex,coord,dx,dy)\n";
							ShaderBody += "\t\t#endif\n\n";
						}

						//Add Instanced Properties
						if ( isInstancedShader && m_currentDataCollector.DirtyInstancedProperties )
						{
							m_currentDataCollector.SetupInstancePropertiesBlock( UIUtils.RemoveInvalidCharacters( ShaderName ) );
							ShaderBody += m_currentDataCollector.InstancedProperties + "\n";
						}

						if ( m_currentDataCollector.DirtyFunctions )
							ShaderBody += m_currentDataCollector.Functions + "\n";


						//Tesselation
						if ( m_tessOpHelper.EnableTesselation )
						{
							ShaderBody += m_tessOpHelper.GetCurrentTessellationFunction + "\n";
						}

						//Add Custom Vertex Data
						if ( m_currentDataCollector.DirtyPerVertexData )
						{
							ShaderBody += m_currentDataCollector.VertexData;
						}

						if ( m_currentLightModel == StandardShaderLightModel.Unlit )
						{
							for ( int i = 0; i < VertexLitFunc.Length; i++ )
							{
								ShaderBody += VertexLitFunc[ i ] + "\n";
							}
						}

						//Add custom lighting
						if ( m_currentLightModel == StandardShaderLightModel.CustomLighting )
						{
							ShaderBody += "\t\tinline half4 LightingStandard" + m_currentLightModel.ToString() + "( inout " + outputStruct + " " + Constants.CustomLightOutputVarStr + ", half3 viewDir, UnityGI gi )\n\t\t{\n";
							ShaderBody += "\t\t\tUnityGIInput data = s.GIData;\n";
							ShaderBody += "\t\t\tInput i = s.SurfInput;\n";
							ShaderBody += "\t\t\thalf4 c = 0;\n";
							if ( m_currentDataCollector.UsingLightAttenuation )
							{
								ShaderBody += "\t\t\t#if DIRECTIONAL\n";
								ShaderBody += "\t\t\tfloat ase_lightAtten = data.atten;\n";
								ShaderBody += "\t\t\tif( _LightColor0.a == 0)\n";
								ShaderBody += "\t\t\tase_lightAtten = 0;\n";
								ShaderBody += "\t\t\t#else\n";
								ShaderBody += "\t\t\tfloat3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );\n";
								ShaderBody += "\t\t\tfloat ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );\n";
								ShaderBody += "\t\t\t#endif\n";
							}

							//if( m_currentDataCollector.dirtyc )
							ShaderBody += customLightingInstructions;
							ShaderBody += "\t\t\tc.rgb = " + ( !string.IsNullOrEmpty( customLightingCode ) ? customLightingCode : "0" ) + ";\n";
							ShaderBody += "\t\t\tc.a = " + ( !string.IsNullOrEmpty( customLightingAlphaCode ) ? customLightingAlphaCode : "1" ) + ";\n";
							if ( hasCustomLightingMask )
								ShaderBody += "\t\t\t"+customLightingMaskCode+";\n";
							ShaderBody += "\t\t\treturn c;\n";
							ShaderBody += "\t\t}\n\n";

							//Add GI function
							ShaderBody += "\t\tinline void LightingStandard" + m_currentLightModel.ToString() + "_GI( inout " + outputStruct + " " + Constants.CustomLightOutputVarStr + ", UnityGIInput data, inout UnityGI gi )\n\t\t{\n";
							ShaderBody += "\t\t\ts.GIData = data;\n";
							//ShaderBody += "\t\t\tUNITY_GI(gi, " + Constants.CustomLightOutputVarStr + ", data);\n";
							ShaderBody += "\t\t}\n\n";
						}

						//Add custom lighting function
						if ( hasTranslucency || hasTransmission )
						{
							ShaderBody += "\t\tinline half4 Lighting" + m_currentLightModel.ToString() + Constants.CustomLightStructStr + "(" + outputStruct + " " + Constants.CustomLightOutputVarStr + ", half3 viewDir, UnityGI gi )\n\t\t{\n";
							if ( hasTranslucency )
							{
								ShaderBody += "\t\t\t#if !DIRECTIONAL\n";
								ShaderBody += "\t\t\tfloat3 lightAtten = gi.light.color;\n";
								ShaderBody += "\t\t\t#else\n";
								ShaderBody += "\t\t\tfloat3 lightAtten = lerp( _LightColor0, gi.light.color, _TransShadow );\n";
								ShaderBody += "\t\t\t#endif\n";
								ShaderBody += "\t\t\thalf3 lightDir = gi.light.dir + " + Constants.CustomLightOutputVarStr + ".Normal * _TransNormalDistortion;\n";
								ShaderBody += "\t\t\thalf transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );\n";
								ShaderBody += "\t\t\thalf3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * " + Constants.CustomLightOutputVarStr + ".Translucency;\n";
								ShaderBody += "\t\t\thalf4 c = half4( " + Constants.CustomLightOutputVarStr + ".Albedo * translucency * _Translucency, 0 );\n\n";
							}

							if ( hasTransmission )
							{
								ShaderBody += "\t\t\thalf3 transmission = max(0 , -dot(" + Constants.CustomLightOutputVarStr + ".Normal, gi.light.dir)) * gi.light.color * " + Constants.CustomLightOutputVarStr + ".Transmission;\n";
								ShaderBody += "\t\t\thalf4 d = half4(" + Constants.CustomLightOutputVarStr + ".Albedo * transmission , 0);\n\n";
							}

							ShaderBody += "\t\t\tSurfaceOutput" + m_currentLightModel.ToString() + " r;\n";
							ShaderBody += "\t\t\tr.Albedo = " + Constants.CustomLightOutputVarStr + ".Albedo;\n";
							ShaderBody += "\t\t\tr.Normal = " + Constants.CustomLightOutputVarStr + ".Normal;\n";
							ShaderBody += "\t\t\tr.Emission = " + Constants.CustomLightOutputVarStr + ".Emission;\n";
							switch ( m_currentLightModel )
							{
								case StandardShaderLightModel.Standard:
								ShaderBody += "\t\t\tr.Metallic = " + Constants.CustomLightOutputVarStr + ".Metallic;\n";
								break;
								case StandardShaderLightModel.StandardSpecular:
								ShaderBody += "\t\t\tr.Specular = " + Constants.CustomLightOutputVarStr + ".Specular;\n";
								break;
							}
							ShaderBody += "\t\t\tr.Smoothness = " + Constants.CustomLightOutputVarStr + ".Smoothness;\n";
							ShaderBody += "\t\t\tr.Occlusion = " + Constants.CustomLightOutputVarStr + ".Occlusion;\n";
							ShaderBody += "\t\t\tr.Alpha = " + Constants.CustomLightOutputVarStr + ".Alpha;\n";
							ShaderBody += "\t\t\treturn Lighting" + m_currentLightModel.ToString() + " (r, viewDir, gi)" + ( hasTranslucency ? " + c" : "" ) + ( hasTransmission ? " + d" : "" ) + ";\n";
							ShaderBody += "\t\t}\n\n";

							//Add GI function
							ShaderBody += "\t\tinline void Lighting" + m_currentLightModel.ToString() + Constants.CustomLightStructStr + "_GI(" + outputStruct + " " + Constants.CustomLightOutputVarStr + ", UnityGIInput data, inout UnityGI gi )\n\t\t{\n";
							ShaderBody += "\t\t\tUNITY_GI(gi, " + Constants.CustomLightOutputVarStr + ", data);\n";
							ShaderBody += "\t\t}\n\n";
						}

						if ( hasRefraction )
						{
							ShaderBody += "\t\tinline float4 Refraction( Input " + Constants.InputVarStr + ", " + outputStruct + " " + Constants.OutputVarStr + ", float indexOfRefraction, float chomaticAberration ) {\n";
							ShaderBody += "\t\t\tfloat3 worldNormal = " + Constants.OutputVarStr + ".Normal;\n";
							ShaderBody += "\t\t\tfloat4 screenPos = " + Constants.InputVarStr + ".screenPos;\n";
							ShaderBody += "\t\t\t#if UNITY_UV_STARTS_AT_TOP\n";
							ShaderBody += "\t\t\t\tfloat scale = -1.0;\n";
							ShaderBody += "\t\t\t#else\n";
							ShaderBody += "\t\t\t\tfloat scale = 1.0;\n";
							ShaderBody += "\t\t\t#endif\n";
							ShaderBody += "\t\t\tfloat halfPosW = screenPos.w * 0.5;\n";
							ShaderBody += "\t\t\tscreenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;\n";
							ShaderBody += "\t\t\t#if SHADER_API_D3D9 || SHADER_API_D3D11\n";
							ShaderBody += "\t\t\t\tscreenPos.w += 0.00000000001;\n";
							ShaderBody += "\t\t\t#endif\n";
							ShaderBody += "\t\t\tfloat2 projScreenPos = ( screenPos / screenPos.w ).xy;\n";
							ShaderBody += "\t\t\tfloat3 worldViewDir = normalize( UnityWorldSpaceViewDir( " + Constants.InputVarStr + ".worldPos ) );\n";
							ShaderBody += "\t\t\tfloat3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );\n";
							ShaderBody += "\t\t\tfloat2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );\n";

							string grabpass = "_GrabTexture";
							if ( m_grabOrder != 0 )
								grabpass = "RefractionGrab" + m_grabOrder;
							ShaderBody += "\t\t\tfloat4 redAlpha = tex2D( " + grabpass + ", ( projScreenPos + cameraRefraction ) );\n";
							ShaderBody += "\t\t\tfloat green = tex2D( " + grabpass + ", ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;\n";
							ShaderBody += "\t\t\tfloat blue = tex2D( " + grabpass + ", ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;\n";
							ShaderBody += "\t\t\treturn float4( redAlpha.r, green, blue, redAlpha.a );\n";
							ShaderBody += "\t\t}\n\n";

							ShaderBody += "\t\tvoid RefractionF( Input " + Constants.InputVarStr + ", " + outputStruct + " " + Constants.OutputVarStr + ", inout fixed4 color )\n";
							ShaderBody += "\t\t{\n";
							ShaderBody += "\t\t\t#ifdef UNITY_PASS_FORWARDBASE\n";
							ShaderBody += refractionInstructions;
							ShaderBody += "\t\t\t\tcolor.rgb = color.rgb + Refraction( " + Constants.InputVarStr + ", " + Constants.OutputVarStr + ", " + refractionCode + ", _ChromaticAberration ) * ( 1 - color.a );\n";
							ShaderBody += "\t\t\t\tcolor.a = 1;\n";
							ShaderBody += "\t\t\t#endif\n";
							ShaderBody += "\t\t}\n\n";
						}

						//Add Surface Shader body
						ShaderBody += "\t\tvoid surf( Input " + Constants.InputVarStr + " , inout " + outputStruct + " " + Constants.OutputVarStr + " )\n\t\t{\n";
						{
							// Pass input information to custom lighting function
							if ( m_currentLightModel == StandardShaderLightModel.CustomLighting )
								ShaderBody += "\t\t\t" + Constants.OutputVarStr + ".SurfInput = " + Constants.InputVarStr + ";\n";

							//add local vars
							if ( m_currentDataCollector.DirtyLocalVariables )
								ShaderBody += m_currentDataCollector.LocalVariables;

							//add nodes ops
							if ( m_currentDataCollector.DirtyInstructions )
								ShaderBody += m_currentDataCollector.Instructions;
						}
						ShaderBody += "\t\t}\n";
					}
					CloseCGProgram( ref ShaderBody );


					//Add custom Shadow Caster
					if ( m_customShadowCaster )
					{
						OpenCGProgram( ref ShaderBody );
						string customLightSurface = hasTranslucency || hasTransmission ? "Custom" : "";
						m_renderingPlatformOpHelper.SetRenderingPlatforms( ref ShaderBody );

						//Check if Custom Vertex is being used and add tag
						if ( m_currentDataCollector.DirtyPerVertexData )
							OptionalParameters += "vertex:" + Constants.VertexDataFunc + Constants.OptionalParametersSep;

						if ( m_tessOpHelper.EnableTesselation )
						{
							m_tessOpHelper.WriteToOptionalParams( ref OptionalParameters );
						}
						//if ( hasRefraction )
						//	ShaderBody += "\t\t#pragma multi_compile _ALPHAPREMULTIPLY_ON\n";

						AddShaderPragma( ref ShaderBody, "surface surf " + standardCustomLighting + m_currentLightModel.ToString() + customLightSurface + Constants.OptionalParametersSep + OptionalParameters );
						CloseCGProgram( ref ShaderBody );

						ShaderBody += "\t\tPass\n";
						ShaderBody += "\t\t{\n";
						ShaderBody += "\t\t\tName \"ShadowCaster\"\n";
						ShaderBody += "\t\t\tTags{ \"LightMode\" = \"ShadowCaster\" }\n";
						ShaderBody += "\t\t\tZWrite On\n";
						ShaderBody += "\t\t\tCGPROGRAM\n";
						ShaderBody += "\t\t\t#pragma vertex vert\n";
						ShaderBody += "\t\t\t#pragma fragment frag\n";
						ShaderBody += "\t\t\t#pragma target 3.0\n";
						//ShaderBody += "\t\t\t#pragma multi_compile_instancing\n";
						ShaderBody += "\t\t\t#pragma multi_compile_shadowcaster\n";
						ShaderBody += "\t\t\t#pragma multi_compile UNITY_PASS_SHADOWCASTER\n";
						ShaderBody += "\t\t\t#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2\n";
						ShaderBody += "\t\t\t# include \"HLSLSupport.cginc\"\n";
						ShaderBody += "\t\t\t#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )\n";
						ShaderBody += "\t\t\t\t#define CAN_SKIP_VPOS\n";
						ShaderBody += "\t\t\t#endif\n";
						ShaderBody += "\t\t\t#include \"UnityCG.cginc\"\n";
						ShaderBody += "\t\t\t#include \"Lighting.cginc\"\n";
						ShaderBody += "\t\t\t#include \"UnityPBSLighting.cginc\"\n";
						ShaderBody += "\t\t\tsampler3D _DitherMaskLOD;\n";

						ShaderBody += "\t\t\tstruct v2f\n";
						ShaderBody += "\t\t\t{\n";
						ShaderBody += "\t\t\t\tV2F_SHADOW_CASTER;\n";
						ShaderBody += "\t\t\t\tfloat3 worldPos : TEXCOORD6;\n";
						if ( m_currentDataCollector.UsingWorldNormal || m_currentDataCollector.UsingWorldPosition || m_currentDataCollector.UsingInternalData || m_currentDataCollector.DirtyNormal )
						{
							ShaderBody += "\t\t\t\tfloat4 tSpace0 : TEXCOORD1;\n";
							ShaderBody += "\t\t\t\tfloat4 tSpace1 : TEXCOORD2;\n";
							ShaderBody += "\t\t\t\tfloat4 tSpace2 : TEXCOORD3;\n";
						}
						if ( m_currentDataCollector.UsingTexcoord0 || m_currentDataCollector.UsingTexcoord1 )
							ShaderBody += "\t\t\t\tfloat4 texcoords01 : TEXCOORD4;\n";
						if ( m_currentDataCollector.UsingTexcoord2 || m_currentDataCollector.UsingTexcoord3 )
							ShaderBody += "\t\t\t\tfloat4 texcoords23 : TEXCOORD5;\n";
#if UNITY_5_5_OR_NEWER
						ShaderBody += "\t\t\t\tUNITY_VERTEX_INPUT_INSTANCE_ID\n";
#else
						ShaderBody += "\t\t\t\tUNITY_INSTANCE_ID\n";
#endif
						ShaderBody += "\t\t\t};\n";

						ShaderBody += "\t\t\tv2f vert( appdata_full v )\n";
						ShaderBody += "\t\t\t{\n";
						ShaderBody += "\t\t\t\tv2f o;\n";

						ShaderBody += "\t\t\t\tUNITY_SETUP_INSTANCE_ID( v );\n";
						ShaderBody += "\t\t\t\tUNITY_INITIALIZE_OUTPUT( v2f, o );\n";
						ShaderBody += "\t\t\t\tUNITY_TRANSFER_INSTANCE_ID( v, o );\n";


						if ( m_currentDataCollector.DirtyPerVertexData )
						{
							ShaderBody += "\t\t\t\tInput customInputData;\n";

							ShaderBody += "\t\t\t\tvertexDataFunc( v" + ( m_currentDataCollector.TesselationActive ? "" : ", customInputData" ) + " );\n";
						}

						ShaderBody += "\t\t\t\tfloat3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;\n";
						ShaderBody += "\t\t\t\thalf3 worldNormal = UnityObjectToWorldNormal( v.normal );\n";
						if ( m_currentDataCollector.UsingInternalData || m_currentDataCollector.DirtyNormal )
						{
							ShaderBody += "\t\t\t\tfixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );\n";
							ShaderBody += "\t\t\t\tfixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;\n";
							ShaderBody += "\t\t\t\tfixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;\n";
							ShaderBody += "\t\t\t\to.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );\n";
							ShaderBody += "\t\t\t\to.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );\n";
							ShaderBody += "\t\t\t\to.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );\n";
						}
						if ( m_currentDataCollector.UsingTexcoord0 || m_currentDataCollector.UsingTexcoord1 )
							ShaderBody += "\t\t\t\to.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );\n";
						if ( m_currentDataCollector.UsingTexcoord2 || m_currentDataCollector.UsingTexcoord3 )
							ShaderBody += "\t\t\t\to.texcoords23 = float4( v.texcoord2.xy, v.texcoord3.xy );\n";

						//if ( m_currentDataCollector.UsingWorldPosition && !m_currentDataCollector.UsingInternalData )
						ShaderBody += "\t\t\t\to.worldPos = worldPos;\n";
						ShaderBody += "\t\t\t\tTRANSFER_SHADOW_CASTER_NORMALOFFSET( o )\n";

						ShaderBody += "\t\t\t\treturn o;\n";
						ShaderBody += "\t\t\t}\n";

						ShaderBody += "\t\t\tfixed4 frag( v2f IN\n";
						ShaderBody += "\t\t\t#if !defined( CAN_SKIP_VPOS )\n";
						ShaderBody += "\t\t\t, UNITY_VPOS_TYPE vpos : VPOS\n";
						ShaderBody += "\t\t\t#endif\n";
						ShaderBody += "\t\t\t) : SV_Target\n";
						ShaderBody += "\t\t\t{\n";
						ShaderBody += "\t\t\t\tUNITY_SETUP_INSTANCE_ID( IN );\n";
						ShaderBody += "\t\t\t\tInput surfIN;\n";
						ShaderBody += "\t\t\t\tUNITY_INITIALIZE_OUTPUT( Input, surfIN );\n";
						if ( m_currentDataCollector.UsingTexcoord0 )
							ShaderBody += "\t\t\t\tsurfIN.uv_texcoord.xy = IN.texcoords01.xy;\n";
						if ( m_currentDataCollector.UsingTexcoord1 )
							ShaderBody += "\t\t\t\tsurfIN.uv2_texcoord2.xy = IN.texcoords01.zw;\n";
						if ( m_currentDataCollector.UsingTexcoord2 )
							ShaderBody += "\t\t\t\tsurfIN.uv3_texcoord3.xy = IN.texcoords23.xy;\n";
						if ( m_currentDataCollector.UsingTexcoord3 )
							ShaderBody += "\t\t\t\tsurfIN.uv4_texcoord4.xy = IN.texcoords23.zw;\n";

						if ( m_currentDataCollector.UsingInternalData )
							ShaderBody += "\t\t\t\tfloat3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );\n";
						else
							ShaderBody += "\t\t\t\tfloat3 worldPos = IN.worldPos;\n";
						ShaderBody += "\t\t\t\tfixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );\n";

						//fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
						//fixed3 viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;

						if ( m_currentDataCollector.UsingViewDirection && !m_currentDataCollector.DirtyNormal )
							ShaderBody += "\t\t\t\tsurfIN.viewDir = worldViewDir;\n";
						else if ( m_currentDataCollector.UsingViewDirection )
							ShaderBody += "\t\t\t\tsurfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;\n";

						if ( m_currentDataCollector.UsingWorldPosition )
							ShaderBody += "\t\t\t\tsurfIN.worldPos = worldPos;\n";

						if ( m_currentDataCollector.UsingWorldNormal )
							ShaderBody += "\t\t\t\tsurfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );\n";

						if ( m_currentDataCollector.UsingWorldReflection )
							ShaderBody += "\t\t\t\tsurfIN.worldRefl = -worldViewDir;\n";

						if ( m_currentDataCollector.UsingInternalData )
						{
							ShaderBody += "\t\t\t\tsurfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;\n";
							ShaderBody += "\t\t\t\tsurfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;\n";
							ShaderBody += "\t\t\t\tsurfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;\n";
						}

						ShaderBody += "\t\t\t\t" + outputStruct + " o;\n";
						ShaderBody += "\t\t\t\tUNITY_INITIALIZE_OUTPUT( " + outputStruct + ", o )\n";
						ShaderBody += "\t\t\t\tsurf( surfIN, o );\n";
						ShaderBody += "\t\t\t\t#if defined( CAN_SKIP_VPOS )\n";
						ShaderBody += "\t\t\t\tfloat2 vpos = IN.pos;\n";
						ShaderBody += "\t\t\t\t#endif\n";

						if ( hasOpacity )
						//if ( m_alphaMode == AlphaMode.Fade || m_alphaMode == AlphaMode.Transparent )
						{
							ShaderBody += "\t\t\t\thalf alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;\n";
							ShaderBody += "\t\t\t\tclip( alphaRef - 0.01 );\n";
						}

						ShaderBody += "\t\t\t\tSHADOW_CASTER_FRAGMENT( IN )\n";
						ShaderBody += "\t\t\t}\n";

						ShaderBody += "\t\t\tENDCG\n";

						ShaderBody += "\t\t}\n";
					}

				}
				CloseSubShaderBody( ref ShaderBody );

				if ( m_fallbackHelper.Active )
				{
					ShaderBody += m_fallbackHelper.CreateFallbackShader();
				}

				if ( m_castShadows )
				{
					AddShaderProperty( ref ShaderBody, "Fallback", "Diffuse" );
				}

				if ( !string.IsNullOrEmpty( m_customInspectorName ) )
				{
					AddShaderProperty( ref ShaderBody, "CustomEditor", m_customInspectorName );
				}
			}
			CloseShaderBody( ref ShaderBody );

			// Generate Graph info
			ShaderBody += ContainerGraph.ParentWindow.GenerateGraphInfo();

			//TODO: Remove current SaveDebugShader and uncomment SaveToDisk as soon as pathname is editable
			if ( !String.IsNullOrEmpty( pathname ) )
			{
				IOUtils.StartSaveThread( ShaderBody, ( isFullPath ? pathname : ( IOUtils.dataPath + pathname ) ) );
			}
			else
			{
				IOUtils.StartSaveThread( ShaderBody, Application.dataPath + "/AmplifyShaderEditor/Samples/Shaders/" + m_shaderName + ".shader" );
			}

			// Load new shader into material

			if ( CurrentShader == null )
			{
				AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
				CurrentShader = Shader.Find( ShaderName );
			}
			else
			{
				// need to always get asset datapath because a user can change and asset location from the project window 
				AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_currentShader ) );
				//ShaderUtil.UpdateShaderAsset( m_currentShader, ShaderBody );
			}

			if ( m_currentShader != null )
			{
				if ( m_currentMaterial != null )
				{
					m_currentMaterial.shader = m_currentShader;
#if UNITY_5_6_OR_NEWER
					if ( isInstancedShader )
					{
						m_currentMaterial.enableInstancing = true;
					}
#endif
					m_currentDataCollector.UpdateMaterialOnPropertyNodes( m_currentMaterial );
					UpdateMaterialEditor();
					// need to always get asset datapath because a user can change and asset location from the project window
					AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_currentMaterial ) );
				}

				m_currentDataCollector.UpdateShaderOnPropertyNodes( ref m_currentShader );
			}

			m_currentDataCollector.Destroy();
			m_currentDataCollector = null;

			return m_currentShader;
		}

		public override void UpdateFromShader( Shader newShader )
		{
			if ( m_currentMaterial != null )
			{
				m_currentMaterial.shader = newShader;
			}
			CurrentShader = newShader;
		}

		public override void Destroy()
		{
			base.Destroy();

			if ( m_dummyProperty != null )
			{
				m_dummyProperty.Destroy();
				GameObject.DestroyImmediate( m_dummyProperty );
				m_dummyProperty = null;
			}

			m_propertyReordableList = null;
			m_translucencyPort = null;
			m_transmissionPort = null;
			m_refractionPort = null;

			m_renderingOptionsOpHelper.Destroy();
			m_renderingOptionsOpHelper = null;

			m_additionalIncludes.Destroy();
			m_additionalIncludes = null;

			m_customTagsHelper.Destroy();
			m_customTagsHelper = null;

			m_renderingPlatformOpHelper = null;
			m_inspectorDefaultStyle = null;
			m_inspectorFoldoutStyle = null;
			m_propertyAdjustment = null;
			m_zBufferHelper = null;
			m_stencilBufferHelper = null;
			m_blendOpsHelper = null;
			m_tessOpHelper.Destroy();
			m_tessOpHelper = null;
			m_outlineHelper = null;
			m_colorMaskHelper.Destroy();
			m_colorMaskHelper = null;
			m_billboardOpHelper = null;
			m_fallbackHelper.Destroy();
			GameObject.DestroyImmediate( m_fallbackHelper );
			m_fallbackHelper = null;
		}

		public override int VersionConvertInputPortId( int portId )
		{
			int newPort = portId;

			//added translucency input after occlusion
			if ( UIUtils.CurrentShaderVersion() <= 2404 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 6 )
						newPort += 1;
					break;
					case StandardShaderLightModel.CustomLighting:
					case StandardShaderLightModel.Unlit:
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 5 )
						newPort += 1;
					break;
				}
			}

			//added transmission input after translucency
			if ( UIUtils.CurrentShaderVersion() < 2407 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 6 )
						newPort += 1;
					break;
					case StandardShaderLightModel.CustomLighting:
					case StandardShaderLightModel.Unlit:
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 5 )
						newPort += 1;
					break;
				}
			}

			//added tessellation ports
			if ( UIUtils.CurrentShaderVersion() < 3002 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 13 )
						newPort += 1;
					break;
					case StandardShaderLightModel.CustomLighting:
					case StandardShaderLightModel.Unlit:
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 10 )
						newPort += 1;
					break;
				}
			}

			//added refraction after translucency
			if ( UIUtils.CurrentShaderVersion() < 3204 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 8 )
						newPort += 1;
					break;
					case StandardShaderLightModel.CustomLighting:
					case StandardShaderLightModel.Unlit:
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 7 )
						newPort += 1;
					break;
				}
			}

			//removed custom lighting port
			if ( UIUtils.CurrentShaderVersion() < 10003 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 13 )
						newPort -= 1;
					break;
					case StandardShaderLightModel.CustomLighting:
					case StandardShaderLightModel.Unlit:
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 12 )
						newPort -= 1;
					break;
				}
			}

			return newPort;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			try
			{
				base.ReadFromString( ref nodeParams );
				m_currentLightModel = ( StandardShaderLightModel ) Enum.Parse( typeof( StandardShaderLightModel ), GetCurrentParam( ref nodeParams ) );
				if ( m_currentLightModel == StandardShaderLightModel.CustomLighting )
				{
					ContainerGraph.ParentWindow.CurrentNodeAvailability = NodeAvailability.CustomLighting;
				}
				//if ( _shaderCategory.Length > 0 )
				//	_shaderCategory = UIUtils.RemoveInvalidCharacters( _shaderCategory );
				ShaderName = GetCurrentParam( ref nodeParams );
				if ( m_shaderName.Length > 0 )
					ShaderName = UIUtils.RemoveShaderInvalidCharacters( ShaderName );

				m_renderingOptionsOpHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );

				m_cullMode = ( CullMode ) Enum.Parse( typeof( CullMode ), GetCurrentParam( ref nodeParams ) );
				m_zBufferHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );

				string alphaMode = GetCurrentParam( ref nodeParams );

				if ( UIUtils.CurrentShaderVersion() < 4003 )
				{
					if ( alphaMode.Equals( "Fade" ) )
					{
						alphaMode = "Transparent";
					}
					else if ( alphaMode.Equals( "Transparent" ) )
					{
						alphaMode = "Premultiply";
					}
				}

				m_alphaMode = ( AlphaMode ) Enum.Parse( typeof( AlphaMode ), alphaMode );
				m_opacityMaskClipValue = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
				m_keepAlpha = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_keepAlpha = true;
				m_castShadows = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_queueOrder = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				if ( UIUtils.CurrentShaderVersion() > 11 )
				{
					m_customBlendMode = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					m_renderType = ( RenderType ) Enum.Parse( typeof( RenderType ), GetCurrentParam( ref nodeParams ) );
					m_renderQueue = ( RenderQueue ) Enum.Parse( typeof( RenderQueue ), GetCurrentParam( ref nodeParams ) );
				}
				if ( UIUtils.CurrentShaderVersion() > 2402 )
				{
					m_renderPath = ( RenderPath ) Enum.Parse( typeof( RenderPath ), GetCurrentParam( ref nodeParams ) );
				}
				if ( UIUtils.CurrentShaderVersion() > 2405 )
				{
					m_renderingPlatformOpHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 2500 )
				{
					m_colorMaskHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 2501 )
				{
					m_stencilBufferHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 2504 )
				{
					m_tessOpHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 2505 )
				{
					m_receiveShadows = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				}

				if ( UIUtils.CurrentShaderVersion() > 3202 )
				{
					m_blendOpsHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 3203 )
				{
					m_grabOrder = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}

				if ( UIUtils.CurrentShaderVersion() > 5003 )
				{
					m_outlineHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 5110 )
				{
					m_billboardOpHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 6101 )
				{
					m_vertexMode = ( VertexMode ) Enum.Parse( typeof( VertexMode ), GetCurrentParam( ref nodeParams ) );
				}

				if ( UIUtils.CurrentShaderVersion() > 6102 )
				{
					m_shaderLOD = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
					m_fallbackHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 7102 )
				{
					m_maskClipOrderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
					m_translucencyOrderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
					m_refractionOrderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
					m_tessellationOrderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}

				if ( UIUtils.CurrentShaderVersion() > 10010 )
				{
					m_additionalIncludes.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 11006 )
				{
					m_customTagsHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				m_lastLightModel = m_currentLightModel;
				DeleteAllInputConnections( true );
				AddMasterPorts();
				UpdateFromBlendMode();
				m_customBlendMode = TestCustomBlendMode();

				ContainerGraph.CurrentPrecision = m_currentPrecisionType;
			}
			catch ( Exception e )
			{
				Debug.Log( e );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentLightModel );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shaderName );
			m_renderingOptionsOpHelper.WriteToString( ref nodeInfo );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_cullMode );
			m_zBufferHelper.WriteToString( ref nodeInfo );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_alphaMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_opacityMaskClipValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_keepAlpha );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_castShadows );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_queueOrder );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_customBlendMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderQueue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderPath );
			m_renderingPlatformOpHelper.WriteToString( ref nodeInfo );
			m_colorMaskHelper.WriteToString( ref nodeInfo );
			m_stencilBufferHelper.WriteToString( ref nodeInfo );
			m_tessOpHelper.WriteToString( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_receiveShadows );
			m_blendOpsHelper.WriteToString( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_grabOrder );
			m_outlineHelper.WriteToString( ref nodeInfo );
			m_billboardOpHelper.WriteToString( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_vertexMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shaderLOD );
			m_fallbackHelper.WriteToString( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_maskClipReorder != null ) ? m_maskClipReorder.OrderIndex : -1 );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_translucencyReorder != null ) ? m_translucencyReorder.OrderIndex : -1 );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_refractionReorder != null ) ? m_refractionReorder.OrderIndex : -1 );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_tessellationReorder != null ) ? m_tessellationReorder.OrderIndex : -1 );
			m_additionalIncludes.WriteToString( ref nodeInfo );
			m_customTagsHelper.WriteToString( ref nodeInfo );
		}

		private bool TestCustomBlendMode()
		{
			switch ( m_alphaMode )
			{
				case AlphaMode.Opaque:
				{
					if ( m_renderType == RenderType.Opaque && m_renderQueue == RenderQueue.Geometry )
						return false;
				}
				break;
				case AlphaMode.Masked:
				{
					if ( m_renderType == RenderType.TransparentCutout && m_renderQueue == RenderQueue.AlphaTest )
						return false;
				}
				break;
				case AlphaMode.Transparent:
				case AlphaMode.Premultiply:
				{
					if ( m_renderType == RenderType.Transparent && m_renderQueue == RenderQueue.Transparent )
						return false;
				}
				break;
				case AlphaMode.Translucent:
				{
					if ( m_renderType == RenderType.Opaque && m_renderQueue == RenderQueue.Transparent )
						return false;
				}
				break;
			}
			return true;
		}

		private void UpdateFromBlendMode()
		{
			m_checkChanges = true;
			bool lockRefractionPort = false;
			if ( m_currentLightModel == StandardShaderLightModel.Unlit || m_currentLightModel == StandardShaderLightModel.CustomLighting )
			{
				lockRefractionPort = true;
			}

			switch ( m_alphaMode )
			{
				case AlphaMode.Opaque:
				{
					m_renderType = RenderType.Opaque;
					m_renderQueue = RenderQueue.Geometry;
					m_keepAlpha = true;
					m_refractionPort.Locked = true;
					m_inputPorts[ m_opacityPortId ].Locked = true;
					m_inputPorts[ m_discardPortId ].Locked = true;
				}
				break;
				case AlphaMode.Masked:
				{
					m_renderType = RenderType.TransparentCutout;
					m_renderQueue = RenderQueue.AlphaTest;
					m_keepAlpha = true;
					m_refractionPort.Locked = true;
					m_inputPorts[ m_opacityPortId ].Locked = true;
					m_inputPorts[ m_discardPortId ].Locked = false;
				}
				break;
				case AlphaMode.Transparent:
				case AlphaMode.Premultiply:
				{
					m_renderType = RenderType.Transparent;
					m_renderQueue = RenderQueue.Transparent;
					m_refractionPort.Locked = false || lockRefractionPort;
					m_inputPorts[ m_opacityPortId ].Locked = false;
					m_inputPorts[ m_discardPortId ].Locked = true;
				}
				break;
				case AlphaMode.Translucent:
				{
					m_renderType = RenderType.Opaque;
					m_renderQueue = RenderQueue.Transparent;
					m_refractionPort.Locked = false || lockRefractionPort;
					m_inputPorts[ m_opacityPortId ].Locked = false;
					m_inputPorts[ m_discardPortId ].Locked = true;
				}
				break;
				case AlphaMode.Custom:
				{
					m_refractionPort.Locked = false || lockRefractionPort;
					m_inputPorts[ m_opacityPortId ].Locked = false;
					m_inputPorts[ m_discardPortId ].Locked = false;
				}
				break;
			}


		}

		public StandardShaderLightModel CurrentLightingModel { get { return m_currentLightModel; } }
	}
}
