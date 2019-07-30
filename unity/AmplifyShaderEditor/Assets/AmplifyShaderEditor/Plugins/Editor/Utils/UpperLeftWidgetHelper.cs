using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public class UpperLeftWidgetHelper
	{
		private Rect m_varRect;
		private bool m_editing;

		public void DrawGUIControls( DrawInfo drawInfo )
		{
			if( drawInfo.CurrentEventType != EventType.MouseDown )
				return;

			if( m_varRect.Contains( drawInfo.MousePosition ) )
			{
				m_editing = true;
			}
			else if( m_editing )
			{
				m_editing = false;
			}
		}

		public void OnNodeLayout( Rect nodeGlobalPosition , DrawInfo drawInfo )
		{
			m_varRect = nodeGlobalPosition;
			m_varRect.x = m_varRect.x + ( Constants.NodeButtonDeltaX - 1 ) * drawInfo.InvertedZoom + 1;
			m_varRect.y = m_varRect.y + Constants.NodeButtonDeltaY * drawInfo.InvertedZoom;
			m_varRect.width = Constants.NodeButtonSizeX * drawInfo.InvertedZoom;
			m_varRect.height = Constants.NodeButtonSizeY * drawInfo.InvertedZoom;
		}
		
		public void OnNodeRepaint( ParentGraph.NodeLOD lodLevel )
		{
			if( !m_editing && lodLevel <= ParentGraph.NodeLOD.LOD4 )
				GUI.Label( m_varRect, string.Empty, UIUtils.PropertyPopUp );
		}
		
		public int DrawWidget( UndoParentNode owner, int selectedIndex, GUIContent[] displayedOptions )
		{
			if( m_editing )
			{
				int newValue = owner.EditorGUIPopup( m_varRect, selectedIndex, displayedOptions, UIUtils.PropertyPopUp );
				if( newValue != selectedIndex )
				{
					m_editing = false;
				}
				return newValue;
			}
			return selectedIndex;
		}

		public int DrawWidget( UndoParentNode owner, int selectedIndex, string[] displayedOptions )
		{
			if( m_editing )
			{
				int newValue = owner.EditorGUIPopup( m_varRect, selectedIndex, displayedOptions, UIUtils.PropertyPopUp );
				if( newValue != selectedIndex )
				{
					m_editing = false;
				}
				return newValue;
			}
			return selectedIndex;
		}
		
		public System.Enum DrawWidget( UndoParentNode owner, System.Enum selectedIndex )
		{
			if( m_editing )
			{
				EditorGUI.BeginChangeCheck();
				System.Enum newValue = owner.EditorGUIEnumPopup( m_varRect, selectedIndex, UIUtils.PropertyPopUp );
				if( EditorGUI.EndChangeCheck() )
				{
					m_editing = false;
				}
				return newValue;
			}
			return selectedIndex;
		}

		/* 
		 * USE THIS OVERRIDE IN CASE THE NODE DOESN'T HAVE PREVIEW
		 */
		//public override void AfterCommonInit()
		//{
		//	base.AfterCommonInit();
		//	if( PaddingTitleLeft == 0 )
		//	{
		//		PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
		//		if( PaddingTitleRight == 0 )
		//			PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
		//	}
		//}


		/* 
		 * USE THE SOURCE CODE BELLOW INTO THE NODE YOU WANT THE WIDGET TO SHOW
		 */
		//private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();
		//
		//public override void OnNodeLayout( DrawInfo drawInfo )
		//{
		//	base.OnNodeLayout( drawInfo );
		//	m_upperLeftWidget.OnNodeLayout( m_globalPosition, drawInfo );
		//}
		//
		//public override void DrawGUIControls( DrawInfo drawInfo )
		//{
		//	base.DrawGUIControls( drawInfo );
		//	m_upperLeftWidget.DrawGUIControls( drawInfo );
		//}
		//
		//public override void OnNodeRepaint( DrawInfo drawInfo )
		//{
		//	base.OnNodeRepaint( drawInfo );
		//	if( !m_isVisible )
		//		return;
		//	m_upperLeftWidget.OnNodeRepaint( ContainerGraph.LodLevel );
		//}

	}
}
