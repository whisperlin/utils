using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using AmplifyShaderEditor;

[Serializable]
public class AmplifyShaderFunction : ScriptableObject
{
	[SerializeField]
	private string m_functionInfo = string.Empty;
	public string FunctionInfo
	{
		get { return m_functionInfo; }
		set { m_functionInfo = value; }
	}

	[SerializeField]
	private string m_functionName = string.Empty;
	public string FunctionName
	{
		get { if ( m_functionName.Length == 0 ) return name; else return m_functionName; }
		set { m_functionName = value; }
	}

	[SerializeField]
	[TextArea( 3, 8 )]
	private string m_description = string.Empty;
	public string Description
	{
		get { return m_description; }
		set { m_description = value; }
	}
}

public class ShaderFunctionDetector : AssetPostprocessor
{
	static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths )
	{
		if ( UIUtils.CurrentWindow == null )
			return;

		bool markForRefresh = false;
		for ( int i = 0; i < importedAssets.Length; i++ )
		{
			AmplifyShaderFunction function = AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( importedAssets[ i ] );
			if ( function != null )
			{
				markForRefresh = true;
				break;
			}
		}

		if ( deletedAssets.Length > 0 )
			markForRefresh = true;

		for ( int i = 0; i < movedAssets.Length; i++ )
		{
			AmplifyShaderFunction function = AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( movedAssets[ i ] );
			if ( function != null )
			{
				markForRefresh = true;
				break;
			}
		}

		for ( int i = 0; i < movedFromAssetPaths.Length; i++ )
		{
			AmplifyShaderFunction function = AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( movedFromAssetPaths[ i ] );
			if ( function != null )
			{
				markForRefresh = true;
				break;
			}
		}

		if ( markForRefresh )
		{
			markForRefresh = false;
			UIUtils.CurrentWindow.RefreshAvaibleNodes();
		}
	}
}
