// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum DisableBatchingTagValues
	{
		True,
		False,
		LODFading
	}

	[Serializable]
	public class RenderingOptionsOpHelper
	{
		private const string RenderingOptionsStr = " Rendering Options";

		private readonly static GUIContent LODCrossfadeContent = new GUIContent( " LOD Group Cross Fade", "Applies a dither crossfade to be used with LOD groups for smoother transitions. Uses one interpolator\nDefault: OFF" );
		private readonly static GUIContent DisableBatchingContent = new GUIContent( " Disable Batching", "\nDisables objects to be batched and used with DrawCallBatching Default: False" );
		private readonly static GUIContent IgnoreProjectorContent = new GUIContent( " Ignore Projector", "\nIf True then an object that uses this shader will not be affected by Projectors Default: False" );
		private readonly static GUIContent ForceNoShadowCastingContent = new GUIContent( " Force No Shadow Casting", "\nIf True then an object that is rendered using this subshader will never cast shadows Default: False" );
		private readonly static GUIContent EnableInstancingContent = new GUIContent( " Enable Instancing", "\nIf True enables instancing on shader independent of having instanced properties" );

		[SerializeField]
		private bool m_enableInstancing = false;

		[SerializeField]
		private bool m_lodCrossfade = false;

		[SerializeField]
		private DisableBatchingTagValues m_disableBatching = DisableBatchingTagValues.False;

		[SerializeField]
		private bool m_ignoreProjector = false;

		[SerializeField]
		private bool m_forceNoShadowCasting = false;

		[SerializeField]
		private List<CodeGenerationData> m_codeGenerationDataList;

		public RenderingOptionsOpHelper()
		{
			m_codeGenerationDataList = new List<CodeGenerationData>();
			m_codeGenerationDataList.Add( new CodeGenerationData( " Exclude Deferred", "exclude_path:deferred" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Exclude Forward", "exclude_path:forward" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Exclude Legacy Deferred", "exclude_path:prepass" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Shadows", "noshadow" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Ambient Light", "noambient" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Per Vertex Light", "novertexlights" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Lightmaps", "nolightmap " ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Dynamic Global GI", "nodynlightmap" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Directional lightmaps", "nodirlightmap" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Built-in Fog", "nofog" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Meta Pass", "nometa" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Add Pass", "noforwardadd" ) );
		}

		public void Draw( ParentNode owner )
		{
			bool value = EditorVariablesManager.ExpandedRenderingOptions.Value;
			NodeUtils.DrawPropertyGroup( ref value, RenderingOptionsStr, () =>
			{
				int codeGenCount = m_codeGenerationDataList.Count;
				// Starting from index 4 because other options are already contemplated with m_renderPath and add/receive shadows
				for ( int i = 4; i < codeGenCount; i++ )
				{
					m_codeGenerationDataList[ i ].IsActive = !owner.EditorGUILayoutToggleLeft( m_codeGenerationDataList[ i ].Name, !m_codeGenerationDataList[ i ].IsActive );
				}
				m_lodCrossfade = owner.EditorGUILayoutToggleLeft( LODCrossfadeContent, m_lodCrossfade );
				m_ignoreProjector = owner.EditorGUILayoutToggleLeft( IgnoreProjectorContent, m_ignoreProjector );
				m_forceNoShadowCasting = owner.EditorGUILayoutToggleLeft( ForceNoShadowCastingContent, m_forceNoShadowCasting );
				if ( owner.ContainerGraph.IsInstancedShader )
				{
					GUI.enabled = false;
					owner.EditorGUILayoutToggleLeft( EnableInstancingContent, true );
					GUI.enabled = true;
				}
				else
				{
					m_enableInstancing = owner.EditorGUILayoutToggleLeft( EnableInstancingContent, m_enableInstancing );
				}
				m_disableBatching = ( DisableBatchingTagValues ) owner.EditorGUILayoutEnumPopup( DisableBatchingContent, m_disableBatching );
			} );
			EditorVariablesManager.ExpandedRenderingOptions.Value = value;
		}

		public void Build( ref string OptionalParameters )
		{
			int codeGenCount = m_codeGenerationDataList.Count;

			for ( int i = 0; i < codeGenCount; i++ )
			{
				if ( m_codeGenerationDataList[ i ].IsActive )
				{
					OptionalParameters += m_codeGenerationDataList[ i ].Value + Constants.OptionalParametersSep;
				}
			}

#if UNITY_2017_1_OR_NEWER
		if( m_lodCrossfade )
		{
			OptionalParameters += Constants.LodCrossFadeOption2017 + Constants.OptionalParametersSep;
		}
#endif
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			for ( int i = 0; i < m_codeGenerationDataList.Count; i++ )
			{
				m_codeGenerationDataList[ i ].IsActive = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if ( UIUtils.CurrentShaderVersion() > 10005 )
			{
				m_lodCrossfade = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if ( UIUtils.CurrentShaderVersion() > 10007 )
			{
				m_disableBatching = ( DisableBatchingTagValues ) Enum.Parse( typeof( DisableBatchingTagValues ), nodeParams[ index++ ] );
				m_ignoreProjector = Convert.ToBoolean( nodeParams[ index++ ] );
				m_forceNoShadowCasting = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if ( UIUtils.CurrentShaderVersion() > 11002 )
			{
				m_enableInstancing = Convert.ToBoolean( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			for ( int i = 0; i < m_codeGenerationDataList.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_codeGenerationDataList[ i ].IsActive );
			}

			IOUtils.AddFieldValueToString( ref nodeInfo, m_lodCrossfade );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_disableBatching );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_ignoreProjector );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_forceNoShadowCasting );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_enableInstancing );
		}

		public void Destroy()
		{
			m_codeGenerationDataList.Clear();
			m_codeGenerationDataList = null;
		}

		public bool EnableInstancing { get { return m_enableInstancing; } }

		public bool LodCrossfade { get { return m_lodCrossfade; } }
		public bool IgnoreProjectorValue { get { return m_ignoreProjector; } set{ m_ignoreProjector = value; } }

		public string DisableBatchingTag { get { return ( m_disableBatching != DisableBatchingTagValues.False ) ? string.Format( Constants.TagFormat, "DisableBatching", m_disableBatching ) : string.Empty; } }
		public string IgnoreProjectorTag { get { return ( m_ignoreProjector ) ? string.Format( Constants.TagFormat, "IgnoreProjector",  "True" )  : string.Empty; } }
		public string ForceNoShadowCastingTag { get { return ( m_forceNoShadowCasting ) ? string.Format( Constants.TagFormat, "ForceNoShadowCasting", "True" ) : string.Empty; } }
	}
}
