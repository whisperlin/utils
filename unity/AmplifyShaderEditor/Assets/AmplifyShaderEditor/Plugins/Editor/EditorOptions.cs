using UnityEditor;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class OptionsWindow
	{
		private AmplifyShaderEditorWindow m_parentWindow = null;

		private bool m_coloredPorts = false;
		private bool m_multiLinePorts = false;

		public OptionsWindow( AmplifyShaderEditorWindow parentWindow )
		{
			m_parentWindow = parentWindow;
			//Load ();
		}

		public void Init()
		{
			Load();
		}

		public void Destroy()
		{
			Save();
		}

		public void Save()
		{
			EditorPrefs.SetBool( "ColoredPorts", ColoredPorts );
			EditorPrefs.SetBool( "MultiLinePorts", ParentWindow.ToggleMultiLine );
			EditorPrefs.SetBool( "ExpandedStencil", ParentWindow.ExpandedStencil );
			EditorPrefs.SetBool( "ExpandedTesselation", ParentWindow.ExpandedTesselation );
			EditorPrefs.SetBool( "ExpandedDepth", ParentWindow.ExpandedDepth );
			EditorPrefs.SetBool( "ExpandedRenderingOptions", ParentWindow.ExpandedRenderingOptions );
			EditorPrefs.SetBool( "ExpandedRenderingPlatforms", ParentWindow.ExpandedRenderingPlatforms );
			EditorPrefs.SetBool( "ExpandedProperties", ParentWindow.ExpandedProperties );
		}

		public void Load()
		{
			ColoredPorts = EditorPrefs.GetBool( "ColoredPorts" );
			ParentWindow.ToggleMultiLine = EditorPrefs.GetBool( "MultiLinePorts" );
			MultiLinePorts = ParentWindow.ToggleMultiLine;
			ParentWindow.ExpandedStencil = EditorPrefs.GetBool( "ExpandedStencil" );
			ParentWindow.ExpandedTesselation = EditorPrefs.GetBool( "ExpandedTesselation" );
			ParentWindow.ExpandedDepth = EditorPrefs.GetBool( "ExpandedDepth" );
			ParentWindow.ExpandedRenderingOptions = EditorPrefs.GetBool( "ExpandedRenderingOptions" );
			ParentWindow.ExpandedRenderingPlatforms = EditorPrefs.GetBool( "ExpandedRenderingPlatforms" );
			ParentWindow.ExpandedProperties = EditorPrefs.GetBool( "ExpandedProperties" );
		}

		public bool ColoredPorts
		{
			get { return m_coloredPorts; }
			set
			{
				if ( m_coloredPorts != value )
					EditorPrefs.SetBool( "ColoredPorts", value );

				m_coloredPorts = value;
			}
		}

		public bool MultiLinePorts
		{
			get { return m_multiLinePorts; }
			set
			{
				if ( m_multiLinePorts != value )
					EditorPrefs.SetBool( "MultiLinePorts", value );

				m_multiLinePorts = value;
			}
		}

		public AmplifyShaderEditorWindow ParentWindow { get { return m_parentWindow; } set { m_parentWindow = value; } }
	}
}
