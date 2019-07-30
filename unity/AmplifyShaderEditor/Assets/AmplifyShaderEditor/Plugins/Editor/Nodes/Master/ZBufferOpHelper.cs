using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum ZWriteMode
	{
		On,
		Off
	}

	public enum ZTestMode
	{
		Less,
		Greater,
		LEqual,
		GEqual,
		Equal,
		NotEqual,
		Always
	}

	[Serializable]
	class ZBufferOpHelper
	{
		private const string DepthParametersStr = " Depth";
		private const string ZWriteModeStr = "ZWrite Mode";
		private const string ZTestModeStr = "ZTest Mode";
		private const string OffsetStr = "Offset";
		private const string OffsetFactorStr = "Factor";
		private const string OffsetUnitsStr = "Units";

		private readonly string[] ZTestModeLabels = {   "<Default>",
														"Less",
														"Greater",
														"Less or Equal",
														"Greater or Equal",
														"Equal",
														"Not Equal",
														"Always" };

		private readonly string[] ZTestModeValues = {   "<Default>",
														"Less",
														"Greater",
														"LEqual",
														"GEqual",
														"Equal",
														"NotEqual",
														"Always"};

		private readonly string[] ZWriteModeValues = {  "<Default>",
														"On",
														"Off"};
		[SerializeField]
		private int m_zTestMode = 0;

		[SerializeField]
		private int m_zWriteMode = 0;

		[SerializeField]
		private float m_offsetFactor;

		[SerializeField]
		private float m_offsetUnits;

		[SerializeField]
		private bool m_offsetEnabled;

		[SerializeField]
		private StandardSurfaceOutputNode m_parentSurface;

		public string CreateDepthInfo()
		{
			string result = string.Empty;
			if ( m_zWriteMode != 0 )
			{
				MasterNode.AddRenderState( ref result, "ZWrite", ZWriteModeValues[ m_zWriteMode ] );
			}

			if ( m_zTestMode != 0 )
			{
				MasterNode.AddRenderState( ref result, "ZTest", ZTestModeValues[ m_zTestMode ] );
			}

			if ( m_offsetEnabled )
			{
				MasterNode.AddRenderState( ref result, "Offset ", m_offsetFactor + " , " + m_offsetUnits );
			}

			return result;
		}

		public void Draw( UndoParentNode owner, GUIStyle toolbarstyle, bool customBlendAvailable )
		{
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( toolbarstyle );
			GUI.color = cachedColor;
			EditorGUI.BeginChangeCheck();
			m_parentSurface.ContainerGraph.ParentWindow.ExpandedDepth = owner.GUILayoutToggle( m_parentSurface.ContainerGraph.ParentWindow.ExpandedDepth, DepthParametersStr, UIUtils.MenuItemToggleStyle );
			if ( EditorGUI.EndChangeCheck() )
			{
				EditorPrefs.SetBool( "ExpandedDepth", m_parentSurface.ContainerGraph.ParentWindow.ExpandedDepth );
			}
			EditorGUILayout.EndHorizontal();

			if ( m_parentSurface.ContainerGraph.ParentWindow.ExpandedDepth )
			{
				cachedColor = GUI.color;
				GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
				EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
				GUI.color = cachedColor;

				if ( !customBlendAvailable )
					EditorGUILayout.HelpBox( "Depth Writing is only available for Opaque or Custom blend modes", MessageType.Warning );

				EditorGUI.indentLevel++;
				EditorGUILayout.Separator();
				EditorGUI.BeginDisabledGroup( !customBlendAvailable );

				m_zWriteMode = owner.EditorGUILayoutPopup( ZWriteModeStr, m_zWriteMode, ZWriteModeValues );
				m_zTestMode = owner.EditorGUILayoutPopup( ZTestModeStr, m_zTestMode, ZTestModeLabels );
				m_offsetEnabled = owner.EditorGUILayoutToggle( OffsetStr, m_offsetEnabled );
				if ( m_offsetEnabled )
				{
					EditorGUI.indentLevel++;
					m_offsetFactor = owner.EditorGUILayoutFloatField( OffsetFactorStr, m_offsetFactor );
					m_offsetUnits = owner.EditorGUILayoutFloatField( OffsetUnitsStr, m_offsetUnits );
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.Separator();
				EditorGUI.indentLevel--;
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndVertical();
			}

			EditorGUI.EndDisabledGroup();
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			if ( UIUtils.CurrentShaderVersion() < 2502 )
			{
				string zWriteMode = nodeParams[ index++ ];
				m_zWriteMode = zWriteMode.Equals( "Off" ) ? 2 : 0;

				string zTestMode = nodeParams[ index++ ];
				for ( int i = 0; i < ZTestModeValues.Length; i++ )
				{
					if ( zTestMode.Equals( ZTestModeValues[ i ] ) )
					{
						m_zTestMode = i;
						break;
					}
				}
			}
			else
			{
				m_zWriteMode = Convert.ToInt32( nodeParams[ index++ ] );
				m_zTestMode = Convert.ToInt32( nodeParams[ index++ ] );
				m_offsetEnabled = Convert.ToBoolean( nodeParams[ index++ ] );
				m_offsetFactor = Convert.ToSingle( nodeParams[ index++ ] );
				m_offsetUnits = Convert.ToSingle( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_zWriteMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_zTestMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_offsetEnabled );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_offsetFactor );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_offsetUnits );
		}
		public bool IsActive { get { return m_zTestMode != 0 || m_zWriteMode != 0 || m_offsetEnabled; } }
		public StandardSurfaceOutputNode ParentSurface { get { return m_parentSurface; } set { m_parentSurface = value; } }
	}
}
