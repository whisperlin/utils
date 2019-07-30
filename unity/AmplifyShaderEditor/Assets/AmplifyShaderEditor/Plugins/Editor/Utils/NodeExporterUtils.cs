using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public enum DebugNodeState
	{
		CreateNode,
		FocusOnNode,
		TakeScreenshot,
		WaitFrame,
		DeleteNode
	};

	public class NodeExporterUtils
	{
		//Auto-Screenshot nodes
		private RenderTexture m_screenshotRT;
		private Texture2D m_screenshotTex2D;
		private List<ContextMenuItem> m_screenshotList = new List<ContextMenuItem>();
		private DebugNodeState m_screenShotState;

		private AmplifyShaderEditorWindow m_window;
		private ParentNode m_node;

		private bool m_takingShots = false;
		private string m_pathname;
		public NodeExporterUtils( AmplifyShaderEditorWindow window )
		{
			m_window = window;
		}


		public void CalculateShaderInstructions( Shader shader )
		{
			//Type shaderutilType = Type.GetType( "UnityEditor.ShaderUtil, UnityEditor" );
			//shaderutilType.InvokeMember( "OpenCompiledShader", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, new object[] { shader, mode, customPlatformsMask, includeAllVariants } );
		}

		public void ActivateAutoScreenShot( string pathname )
		{
			m_pathname = pathname;
			if ( !System.IO.Directory.Exists( m_pathname ) )
			{
				System.IO.Directory.CreateDirectory( m_pathname );
			}

			m_screenshotRT = new RenderTexture( ( int ) m_window.position.width, ( int ) m_window.position.height, 0 );
			m_screenshotTex2D = new Texture2D( ( int ) m_window.position.width, ( int ) m_window.position.height, TextureFormat.RGB24, false );

			RenderTexture.active = m_screenshotRT;
			m_window.CurrentPaletteWindow.FillList( ref m_screenshotList, true );
			m_window.CurrentGraph.ClearGraph();
			m_window.CurrentGraph.CurrentMasterNode.Vec2Position = new Vector2( 1500, 0 );
			m_window.ResetCameraSettings();

			m_takingShots = true;
			m_screenShotState = DebugNodeState.CreateNode;
		}

		public void Update()
		{
			if ( m_takingShots )
			{
				m_window.Focus();
				switch ( m_screenShotState )
				{
					case DebugNodeState.CreateNode:
					{
						m_node = m_window.CreateNode( m_screenshotList[ 0 ].NodeType, Vector2.zero, null, false );
						m_screenShotState = DebugNodeState.FocusOnNode;
					}
					break;
					case DebugNodeState.FocusOnNode:
					{
						m_window.FocusOnNode( m_node, 1, false );
						m_screenShotState = DebugNodeState.TakeScreenshot;
					}
					break;
					case DebugNodeState.TakeScreenshot:
					{
						if ( m_screenshotRT != null && Event.current.type == EventType.repaint )
						{
							m_screenshotTex2D.ReadPixels( new Rect( 0, 0, m_screenshotRT.width, m_screenshotRT.height ), 0, 0 );
							m_screenshotTex2D.Apply();

							byte[] bytes = m_screenshotTex2D.EncodeToPNG();
							string pictureFilename = UIUtils.ReplaceInvalidStrings( m_screenshotList[ 0 ].Name );
							pictureFilename = UIUtils.RemoveInvalidCharacters( pictureFilename );

							System.IO.File.WriteAllBytes( m_pathname + pictureFilename + ".png", bytes );
							m_screenShotState = DebugNodeState.WaitFrame;
						}
					}
					break;
					case DebugNodeState.WaitFrame: { Debug.Log( "Wait Frame" ); m_screenShotState = DebugNodeState.DeleteNode; } break;
					case DebugNodeState.DeleteNode:
					{
						m_window.DestroyNode( m_node );
						m_screenshotList.RemoveAt( 0 );
						m_takingShots = m_screenshotList.Count > 0;
						Debug.Log( "Destroy Node " + m_screenshotList.Count );

						if ( m_takingShots )
						{
							m_screenShotState = DebugNodeState.CreateNode;
						}
						else
						{
							RenderTexture.active = null;
							m_screenshotRT.Release();
							UnityEngine.Object.DestroyImmediate( m_screenshotRT );
							m_screenshotRT = null;
							UnityEngine.Object.DestroyImmediate( m_screenshotTex2D );
							m_screenshotTex2D = null;
						}
					}
					break;
				};
			}
		}

		public void Destroy()
		{
			m_window = null;
			if ( m_screenshotRT != null )
			{
				m_screenshotRT.Release();
				UnityEngine.Object.DestroyImmediate( m_screenshotRT );
				m_screenshotRT = null;
			}

			if ( m_screenshotTex2D != null )
			{
				UnityEngine.Object.DestroyImmediate( m_screenshotTex2D );
				m_screenshotTex2D = null;
			}
		}
	}
}
