// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Rotator
// Donated by kebrus

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Rotator", "UV Coordinates", "Rotates UVs with time but can also be used to rotate other Vector2 values", null, KeyCode.None, true, false, null, null, true )]
	public sealed class RotatorNode : ParentNode
	{
		private int m_cachedUsingEditorId = -1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddInputPort( WirePortDataType.FLOAT2, false, "Anchor" );
			AddInputPort( WirePortDataType.FLOAT, false, "Time" );
			AddOutputPort( WirePortDataType.FLOAT2, "Out" );
			m_useInternalPortData = true;
			m_previewShaderGUID = "e21408a1c7f12f14bbc2652f69bce1fc";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_cachedUsingEditorId == -1 )
				m_cachedUsingEditorId = Shader.PropertyToID( "_UsingEditor" );

			PreviewMaterial.SetFloat( m_cachedUsingEditorId, (m_inputPorts[ 2 ].IsConnected ? 0 : 1 ) );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.HelpBox("Rotates UVs but can also be used to rotate other Vector2 values\n\nAnchor is the rotation point in UV space from which you rotate the UVs\nAngle is the amount of rotation applied [0,1], if left unconnected it will use time as the default value", MessageType.None);
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{

			if( m_outputPorts[0].IsLocalValue )
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );

			string result = string.Empty;
			string uv = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, false );
			string anchor = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false );

			string time = string.Empty;
			if ( m_inputPorts[ 2 ].IsConnected )
			{
				time = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, false );
			}
			else
			{
				dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );
				time = "_Time[1]";
			}

			result += uv;

			string cosVar = "cos" + OutputId;
			string sinVar = "sin" + OutputId;
			dataCollector.AddLocalVariable( UniqueId, "float " + cosVar + " = cos( "+time+" );");
			dataCollector.AddLocalVariable( UniqueId, "float " + sinVar + " = sin( "+time+" );");


			string rotatorVar = "rotator" + OutputId;
			dataCollector.AddLocalVariable( UniqueId, "float2 " + rotatorVar + " = mul(" + result + " - "+anchor+", float2x2("+cosVar+",-"+sinVar+","+sinVar+","+cosVar+")) + "+anchor+";" );

			return GetOutputVectorItem( 0, outputId, rotatorVar );
		}
	}
}
