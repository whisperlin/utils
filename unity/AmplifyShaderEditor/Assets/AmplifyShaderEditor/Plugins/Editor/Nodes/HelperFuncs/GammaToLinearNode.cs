// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Gamma To Linear", "Image Effects", "Converts color from gamma space to linear space" )]
	public sealed class GammaToLinearNode : HelperParentNode
	{
		[SerializeField]
		private bool m_exact = false;

		private readonly static GUIContent GLExactContent = new GUIContent( "Exact Conversion", "Uses a precise version of the conversion, it's more expensive and often not needed." );

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "GammaToLinearSpace";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_inputPorts[ 0 ].Name = "RGB";
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_autoWrapProperties = true;
			m_previewShaderGUID = "e82a888a6ebdb1443823aafceaa051b9";
			m_textLabelWidth = 120;
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "gammaToLinear" + OutputId;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_exact = EditorGUILayoutToggle( GLExactContent, m_exact );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_exact )
			{
				string result = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				dataCollector.AddLocalVariable( UniqueId, "half3 " + m_localVarName + " = " + result + ";" );
				dataCollector.AddLocalVariable( UniqueId, m_localVarName + " = half3( GammaToLinearSpaceExact(" + m_localVarName + ".r), GammaToLinearSpaceExact(" + m_localVarName + ".g), GammaToLinearSpaceExact(" + m_localVarName + ".b) );" );

				return m_localVarName;
			}
			else
			{
				return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_exact );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 11003 )
				m_exact = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}
	}
}
