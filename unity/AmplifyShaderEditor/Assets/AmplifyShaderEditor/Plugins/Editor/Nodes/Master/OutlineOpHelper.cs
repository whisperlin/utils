// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{



	public enum OutlineMode
	{
		VertexOffset,
		VertexScale
	}

	[Serializable]
	public sealed class OutlineOpHelper
	{
		private readonly string[] OutlineBodyBegin = {  "Tags{ }",
														"Cull Front",
														"CGPROGRAM",
														"#pragma target 3.0",
														"#pragma surface outlineSurf Outline keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:outlineVertexDataFunc"
														};

		private readonly string[] OutlineBodyStruct = {"struct Input",
														"{",
														"\tfixed filler;",
														"};",
														};

		private readonly string[] OutlineBodyDefaultBegin = {
														"uniform fixed4 _ASEOutlineColor;",
														"uniform fixed _ASEOutlineWidth;",
														"void outlineVertexDataFunc( inout appdata_full v, out Input o )",
														"{",
														"\tUNITY_INITIALIZE_OUTPUT( Input, o );" };
		private readonly string[] OutlineBodyDefaultEnd = {
														"}",
														"inline fixed4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return fixed4 ( 0,0,0, s.Alpha); }",
														"void outlineSurf( Input i, inout SurfaceOutput o ) { o.Emission = _ASEOutlineColor.rgb; o.Alpha = 1; }",
														"ENDCG",
														"\n"};

		private const string OutlineInstancedHeader = "#pragma multi_compile_instancing";

		private readonly string[] OutlineBodyInstancedBegin = {
														"UNITY_INSTANCING_CBUFFER_START({0})",
														"\tUNITY_DEFINE_INSTANCED_PROP( fixed4, _ASEOutlineColor )",
														"\tUNITY_DEFINE_INSTANCED_PROP(fixed, _ASEOutlineWidth)",
														"UNITY_INSTANCING_CBUFFER_END",
														"void outlineVertexDataFunc( inout appdata_full v, out Input o )",
														"{",
														"\tUNITY_INITIALIZE_OUTPUT( Input, o );" };


		private readonly string[] OutlineBodyInstancedEnd = {
														"}",
														"inline fixed4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return fixed4 ( 0,0,0, s.Alpha); }",
														"void outlineSurf( Input i, inout SurfaceOutput o ) { o.Emission = UNITY_ACCESS_INSTANCED_PROP( _ASEOutlineColor ).rgb; o.Alpha = 1; }",
														"ENDCG",
														"\n"};

		private const string WidthVariableAccessInstanced = "UNITY_ACCESS_INSTANCED_PROP( _ASEOutlineWidth )";

		private const string OutlineVertexOffsetMode = "\tv.vertex.xyz += ( v.normal * {0} );";
		private const string OutlineVertexScaleMode = "\tv.vertex.xyz *= ( 1 + {0});";


		private const string OutlineColorLabel = "Color";
		private const string OutlineWidthLabel = "Width";

		private const string ColorPropertyName = "_ASEOutlineColor";
		private const string WidthPropertyName = "_ASEOutlineWidth";
		private const string ColorPropertyDec = "_ASEOutlineColor( \"Outline Color\", Color ) = ({0})";
		private const string OutlinePropertyDec = "_ASEOutlineWidth( \"Outline Width\", Float ) = {0}";

		private const string ModePropertyStr = "Mode";

		private const string BillboardInstructionFormat = "\t{0};";

		[SerializeField]
		private Color m_outlineColor;

		[SerializeField]
		private float m_outlineWidth;

		[SerializeField]
		private bool m_enabled;

		[SerializeField]
		private OutlineMode m_mode = OutlineMode.VertexOffset;

		public void Draw( UndoParentNode owner, GUIStyle toolbarstyle, Material mat )
		{
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( toolbarstyle );
			GUI.color = cachedColor;
			EditorVariablesManager.OutlineActiveMode.Value = owner.GUILayoutToggle( EditorVariablesManager.OutlineActiveMode.Value, EditorVariablesManager.OutlineActiveMode.LabelName, UIUtils.MenuItemToggleStyle, GUILayout.ExpandWidth( true ) );
			EditorGUI.BeginChangeCheck();
			m_enabled = owner.EditorGUILayoutToggle( string.Empty, m_enabled, UIUtils.MenuItemEnableStyle, GUILayout.Width( 16 ) );
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( m_enabled )
					UpdateToMaterial( mat );

				UIUtils.RequestSave();
			}
			EditorGUILayout.EndHorizontal();

			if ( EditorVariablesManager.OutlineActiveMode.Value )
			{
				cachedColor = GUI.color;
				GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
				EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
				GUI.color = cachedColor;

				EditorGUILayout.Separator();
				EditorGUI.BeginDisabledGroup( !m_enabled );

				EditorGUI.indentLevel += 1;
				{
					m_mode = ( OutlineMode ) owner.EditorGUILayoutEnumPopup( ModePropertyStr, m_mode );

					EditorGUI.BeginChangeCheck();
					m_outlineColor = owner.EditorGUILayoutColorField( OutlineColorLabel, m_outlineColor );
					if ( EditorGUI.EndChangeCheck() && mat != null )
					{
						if ( mat.HasProperty( ColorPropertyName ) )
						{
							mat.SetColor( ColorPropertyName, m_outlineColor );
						}
					}

					EditorGUI.BeginChangeCheck();
					m_outlineWidth = owner.EditorGUILayoutFloatField( OutlineWidthLabel, m_outlineWidth );
					if ( EditorGUI.EndChangeCheck() && mat != null )
					{
						if ( mat.HasProperty( WidthPropertyName ) )
						{
							mat.SetFloat( WidthPropertyName, m_outlineWidth );
						}
					}
				}
				EditorGUI.indentLevel -= 1;
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.Separator();
				EditorGUILayout.EndVertical();
			}
		}

		public void UpdateToMaterial( Material mat )
		{
			if ( mat == null )
				return;

			if ( mat.HasProperty( ColorPropertyName ) )
			{
				mat.SetColor( ColorPropertyName, m_outlineColor );
			}

			if ( mat.HasProperty( WidthPropertyName ) )
			{
				mat.SetFloat( WidthPropertyName, m_outlineWidth );
			}
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_enabled = Convert.ToBoolean( nodeParams[ index++ ] );
			m_outlineWidth = Convert.ToSingle( nodeParams[ index++ ] );
			m_outlineColor = IOUtils.StringToColor( nodeParams[ index++ ] );
			if ( UIUtils.CurrentShaderVersion() > 5004 )
				m_mode = ( OutlineMode ) Enum.Parse( typeof( OutlineMode ), nodeParams[ index++ ] );
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_enabled );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outlineWidth );
			IOUtils.AddFieldValueToString( ref nodeInfo, IOUtils.ColorToString( m_outlineColor ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_mode );
		}

		public void AddToDataCollector( ref MasterNodeDataCollector dataCollector )
		{
			dataCollector.AddToProperties( -1, string.Format( ColorPropertyDec, IOUtils.ColorToString( m_outlineColor ) ), -1 );
			dataCollector.AddToProperties( -1, string.Format( OutlinePropertyDec, m_outlineWidth ), -1 );
		}

		public void UpdateFromMaterial( Material mat )
		{
			if ( mat.HasProperty( ColorPropertyName ) )
			{
				m_outlineColor = mat.GetColor( ColorPropertyName );
			}

			if ( mat.HasProperty( WidthPropertyName ) )
			{
				m_outlineWidth = mat.GetFloat( WidthPropertyName );
			}
		}

		public string[] OutlineFunctionBody( bool instanced, bool isShadowCaster, string shaderName, string[] billboardInfo )
		{
			List<string> body = new List<string>();
			for ( int i = 0; i < OutlineBodyBegin.Length; i++ )
			{
				body.Add( OutlineBodyBegin[ i ] );
			}

			if ( instanced )
			{
				body.Add( OutlineInstancedHeader );
			}

			if ( !isShadowCaster )
			{
				for ( int i = 0; i < OutlineBodyStruct.Length; i++ )
				{
					body.Add( OutlineBodyStruct[ i ] );
				}
			}

			if ( instanced )
			{
				for ( int i = 0; i < OutlineBodyInstancedBegin.Length; i++ )
				{
					body.Add( ( i == 0 ) ? string.Format( OutlineBodyInstancedBegin[ i ], shaderName ) : OutlineBodyInstancedBegin[ i ] );
				}

				if ( ( object ) billboardInfo != null )
				{
					for ( int j = 0; j < billboardInfo.Length; j++ )
					{
						body.Add( string.Format( BillboardInstructionFormat, billboardInfo[ j ] ) );
					}
				}

				switch ( m_mode )
				{
					case OutlineMode.VertexOffset: body.Add( string.Format( OutlineVertexOffsetMode, WidthVariableAccessInstanced ) ); break;
					case OutlineMode.VertexScale: body.Add( string.Format( OutlineVertexScaleMode, WidthVariableAccessInstanced ) ); break;
				}
				for ( int i = 0; i < OutlineBodyInstancedEnd.Length; i++ )
				{
					body.Add( OutlineBodyInstancedEnd[ i ] );
				}
			}
			else
			{
				for ( int i = 0; i < OutlineBodyDefaultBegin.Length; i++ )
				{
					body.Add( OutlineBodyDefaultBegin[ i ] );
				}

				if ( ( object ) billboardInfo != null )
				{
					for ( int j = 0; j < billboardInfo.Length; j++ )
					{
						body.Add( string.Format( BillboardInstructionFormat, billboardInfo[ j ] ) );
					}
				}

				switch ( m_mode )
				{
					case OutlineMode.VertexOffset: body.Add( string.Format( OutlineVertexOffsetMode, WidthPropertyName ) ); break;
					case OutlineMode.VertexScale: body.Add( string.Format( OutlineVertexScaleMode, WidthPropertyName ) ); break;
				}
				for ( int i = 0; i < OutlineBodyDefaultEnd.Length; i++ )
				{
					body.Add( OutlineBodyDefaultEnd[ i ] );
				}
			}

			string[] bodyArr = body.ToArray();
			body.Clear();
			body = null;
			return bodyArr;
		}

		public bool EnableOutline { get { return m_enabled; } }
	}
}
