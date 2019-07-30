// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Panner", "UV Coordinates", "Pans UV texture coordinates according to its inputs" )]
	public sealed class PannerNode : ParentNode
	{
		private const string _speedXStr = "Speed X";
		private const string _speedYStr = "Speed Y";
		[SerializeField]
		private float m_speedX = 1f;

		[SerializeField]
		private float m_speedY = 1f;

		private int m_cachedUsingEditorId = -1;
		private int m_cachedSpeedXId = -1;
		private int m_cachedSpeedYId = -1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddInputPort( WirePortDataType.FLOAT, false, "Time" );
			AddOutputPort( WirePortDataType.FLOAT2, "Out" );
			m_textLabelWidth = 70;
			m_autoWrapProperties = true;
			m_previewShaderGUID = "6f89a5d96bdad114b9bbd0c236cac622";
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			m_speedX = EditorGUILayoutFloatField( _speedXStr, m_speedX );
			m_speedY = EditorGUILayoutFloatField( _speedYStr, m_speedY );
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_cachedUsingEditorId == -1 )
				m_cachedUsingEditorId = Shader.PropertyToID( "_UsingEditor" );

			if ( m_cachedSpeedXId == -1 )
				m_cachedSpeedXId = Shader.PropertyToID( "_SpeedX" );

			if ( m_cachedSpeedYId == -1 )
				m_cachedSpeedYId = Shader.PropertyToID( "_SpeedY" );


			PreviewMaterial.SetFloat( m_cachedUsingEditorId, ( m_inputPorts[ 1 ].IsConnected ? 0 : 1 ) );
			PreviewMaterial.SetFloat( m_cachedSpeedXId, m_speedX );
			PreviewMaterial.SetFloat( m_cachedSpeedYId, m_speedY );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string timePort = string.Empty;
			if ( m_inputPorts[ 1 ].IsConnected )
			{
				timePort = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			}
			else
			{
				dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );
				timePort = "_Time[1]";
			}

			string result = "(abs( " + m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector )+ "+" + timePort + " * float2(" + m_speedX + "," + m_speedY + " )))";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_speedX = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			m_speedY = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
		}
		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_speedX );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_speedY );
		}
	}
}
