using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class AdditionalIncludesHelper
	{
		private const string AdditionalIncludesStr = " Additional Includes";
		private const float ShaderKeywordButtonLayoutWidth = 15;
		private ParentNode m_currentOwner;

		[SerializeField]
		private List<string> m_additionalIncludes = new List<string>();

		public void Draw( ParentNode owner )
		{
			m_currentOwner = owner;
			bool value = EditorVariablesManager.ExpandedAdditionalIncludes.Value;
			NodeUtils.DrawPropertyGroup( ref value, AdditionalIncludesStr, DrawMainBody, DrawButtons );
			EditorVariablesManager.ExpandedAdditionalIncludes.Value = value;

		}

		void DrawButtons()
		{
			EditorGUILayout.Separator();

			// Add keyword
			if ( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				m_additionalIncludes.Insert( 0, string.Empty );
			}

			//Remove keyword
			if ( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				if( m_additionalIncludes.Count > 0 )
					m_additionalIncludes.RemoveAt( m_additionalIncludes.Count - 1 );
			}
		}

		void DrawMainBody( )
		{
			EditorGUILayout.Separator();
			int itemCount = m_additionalIncludes.Count;
			int markedToDelete = -1;
			for ( int i = 0; i < itemCount; i++ )
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUI.BeginChangeCheck();
					m_additionalIncludes[ i ] = EditorGUILayout.TextField( m_additionalIncludes[ i ] );
					if ( EditorGUI.EndChangeCheck() )
					{
						m_additionalIncludes[ i ] = UIUtils.RemoveShaderInvalidCharacters( m_additionalIncludes[ i ] );
					}

					// Add new port
					if ( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
					{
						m_additionalIncludes.Insert( i, string.Empty );
					}

					//Remove port
					if ( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
					{
						markedToDelete = i;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			if ( markedToDelete > -1 )
			{
				if ( m_additionalIncludes.Count > markedToDelete )
					m_additionalIncludes.RemoveAt( markedToDelete );
			}
			EditorGUILayout.Separator();
			EditorGUILayout.HelpBox( "Please add your includes without the #include \"\" keywords", MessageType.Info );
		}
		
		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			int count = Convert.ToInt32( nodeParams[ index++ ] );
			for ( int i = 0; i < count; i++ )
			{
				m_additionalIncludes.Add( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalIncludes.Count );
			for ( int i = 0; i < m_additionalIncludes.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalIncludes[ i ] );
			}
		}

		public void AddToDataCollector( ref MasterNodeDataCollector dataCollector )
		{
			for ( int i = 0; i < m_additionalIncludes.Count; i++ )
			{
				if( !string.IsNullOrEmpty( m_additionalIncludes[ i ])) 
					dataCollector.AddToIncludes( -1, m_additionalIncludes[ i ] );
			}
		}

		public void Destroy()
		{
			m_additionalIncludes.Clear();
			m_additionalIncludes = null;
			m_currentOwner = null;
		}
	}
}
