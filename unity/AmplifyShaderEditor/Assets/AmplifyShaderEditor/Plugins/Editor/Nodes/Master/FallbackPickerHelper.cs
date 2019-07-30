using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class FallbackPickerHelper : ScriptableObject
	{
		private const string FallbackFormat = "\tFallback \"{0}\"\n";
		private const string FallbackShaderStr = "Fallback";
		private const string ShaderPoputContext = "CONTEXT/ShaderPopup";

		private Material m_dummyMaterial;
		private MenuCommand m_dummyCommand;

		[SerializeField]
		private string m_fallbackShader = string.Empty;

		public FallbackPickerHelper()
		{
			m_dummyMaterial = null;
			m_dummyCommand = null;
		}

		public void Draw( ParentNode owner )
		{
			EditorGUILayout.BeginHorizontal();
			m_fallbackShader = owner.EditorGUILayoutTextField( FallbackShaderStr, m_fallbackShader );
			if ( GUILayout.Button( string.Empty, UIUtils.InspectorPopdropdownStyle, GUILayout.Width( 10 ), GUILayout.Height( 19 ) ) )
			{
				DisplayShaderContext( owner, GUILayoutUtility.GetRect( GUIContent.none, EditorStyles.popup ) );
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DisplayShaderContext( ParentNode node, Rect r )
		{
			if ( m_dummyCommand == null )
				m_dummyCommand = new MenuCommand( this, 0 );

			if ( m_dummyMaterial == null )
				m_dummyMaterial = new Material( Shader.Find( "Hidden/ASESShaderSelectorUnlit" ) );

			UnityEditorInternal.InternalEditorUtility.SetupShaderMenu( m_dummyMaterial );
			EditorUtility.DisplayPopupMenu( r, ShaderPoputContext, m_dummyCommand );
		}

		private void OnSelectedShaderPopup( string command, Shader shader )
		{
			if ( shader != null )
			{
				UIUtils.MarkUndoAction();
				Undo.RecordObject( this, "Selected fallback shader" );
				m_fallbackShader = shader.name;
			}
		}

		public string CreateFallbackShader()
		{
			return string.Format( FallbackFormat, m_fallbackShader );
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_fallbackShader = nodeParams[ index++ ];
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_fallbackShader );
		}

		public void Destroy()
		{
			GameObject.DestroyImmediate( m_dummyMaterial );
			m_dummyMaterial = null;
			m_dummyCommand = null;
		}

		public bool Active { get { return !string.IsNullOrEmpty( m_fallbackShader ); } }

	}
}
