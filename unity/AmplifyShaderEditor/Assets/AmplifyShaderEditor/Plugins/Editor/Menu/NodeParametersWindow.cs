// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

namespace AmplifyShaderEditor
{
	public sealed class NodeParametersWindow : MenuParent
	{
		private int m_lastSelectedNode = -1;
		private const string TitleStr = "Node Properties";
		private GUIStyle m_nodePropertiesStyle;
		private GUIContent m_dummyContent = new GUIContent();
		private GUIStyle m_propertyAdjustment;

		private ReorderableList m_functionInputsReordableList = null;
		private int m_functionInputsLastCount = 0;

		private ReorderableList m_functionOutputsReordableList = null;
		private int m_functionOutputsLastCount = 0;

		private ReorderableList m_propertyReordableList = null;
		private int m_lastCount = 0;

		private bool m_forceUpdate = false;

		[SerializeField]
		private List<PropertyNode> m_propertyReordableNodes = new List<PropertyNode>();

		// width and height are between [0,1] and represent a percentage of the total screen area
		public NodeParametersWindow( AmplifyShaderEditorWindow parentWindow ) : base( parentWindow, 0, 0, 250, 0, string.Empty, MenuAnchor.TOP_LEFT, MenuAutoSize.MATCH_VERTICAL )
		{
			SetMinimizedArea( -225, 0, 260, 0 );
		}

		public bool Draw( Rect parentPosition, ParentNode selectedNode, Vector2 mousePosition, int mouseButtonId, bool hasKeyboardFocus )
		{
			bool changeCheck = false;
			base.Draw( parentPosition, mousePosition, mouseButtonId, hasKeyboardFocus );
			if ( m_nodePropertiesStyle == null )
			{
				m_nodePropertiesStyle = UIUtils.GetCustomStyle( CustomStyle.NodePropertiesTitle );
				m_nodePropertiesStyle.normal.textColor = m_nodePropertiesStyle.active.textColor = EditorGUIUtility.isProSkin ? new Color( 1f, 1f, 1f ) : new Color( 0f, 0f, 0f );
			}

			if ( m_isMaximized )
			{
				KeyCode key = Event.current.keyCode;
				if ( m_isMouseInside || hasKeyboardFocus )
				{
					if ( key == ShortcutsManager.ScrollUpKey )
					{
						m_currentScrollPos.y -= 10;
						if ( m_currentScrollPos.y < 0 )
						{
							m_currentScrollPos.y = 0;
						}
						Event.current.Use();
					}

					if ( key == ShortcutsManager.ScrollDownKey )
					{
						m_currentScrollPos.y += 10;
						Event.current.Use();
					}
				}

				if( m_forceUpdate )
				{
					if( m_propertyReordableList != null )
						m_propertyReordableList.ReleaseKeyboardFocus();
					m_propertyReordableList = null;

					if ( m_functionInputsReordableList != null )
						m_functionInputsReordableList.ReleaseKeyboardFocus();
					m_functionInputsReordableList = null;

					if ( m_functionOutputsReordableList != null )
						m_functionOutputsReordableList.ReleaseKeyboardFocus();
					m_functionOutputsReordableList = null;
					m_forceUpdate = false;
				}

				GUILayout.BeginArea( m_transformedArea, m_content, m_style );
				{
					//Draw selected node parameters
					if ( selectedNode != null )
					{
						// this hack is need because without it the several FloatFields/Textfields/... would show wrong values ( different from the ones they were assigned to show )
						if ( m_lastSelectedNode != selectedNode.UniqueId )
						{
							m_lastSelectedNode = selectedNode.UniqueId;
							GUI.FocusControl( "" );
						}

						EditorGUILayout.BeginVertical();
						{
							EditorGUILayout.Separator();
							if ( selectedNode.UniqueId == ParentWindow.CurrentGraph.CurrentMasterNodeId )
							{
								m_dummyContent.text = "Output Node";
							}
							else
							{
								if ( selectedNode.Attributes != null )
								{

									m_dummyContent.text = selectedNode.Attributes.Name;
								}
								else if ( selectedNode is CommentaryNode )
								{
									m_dummyContent.text = "Commentary";
								}
								else
								{
									m_dummyContent.text = TitleStr;
								}
							}

							EditorGUILayout.LabelField( m_dummyContent, m_nodePropertiesStyle );

							EditorGUILayout.Separator();
							//UIUtils.RecordObject( selectedNode , "Changing properties on node " + selectedNode.UniqueId);
							m_currentScrollPos = EditorGUILayout.BeginScrollView( m_currentScrollPos, GUILayout.Width( 0 ), GUILayout.Height( 0 ) );
							float labelWidth = EditorGUIUtility.labelWidth;
							if ( selectedNode.TextLabelWidth > 0 )
								EditorGUIUtility.labelWidth = selectedNode.TextLabelWidth;

							changeCheck = selectedNode.SafeDrawProperties();
							EditorGUIUtility.labelWidth = labelWidth;
							EditorGUILayout.EndScrollView();
						}
						EditorGUILayout.EndVertical();

						if ( changeCheck )
						{
							if ( selectedNode.ConnStatus == NodeConnectionStatus.Connected )
								ParentWindow.SetSaveIsDirty();
						}
					}
					else
					{
						//Draw Graph Params
						EditorGUILayout.BeginVertical();
						{
							EditorGUILayout.Separator();
							EditorGUILayout.LabelField( "Graph Properties", m_nodePropertiesStyle );
							EditorGUILayout.Separator();

							m_currentScrollPos = EditorGUILayout.BeginScrollView( m_currentScrollPos, GUILayout.Width( 0 ), GUILayout.Height( 0 ) );
							float labelWidth = EditorGUIUtility.labelWidth;
							EditorGUIUtility.labelWidth = 90;

							bool generalIsVisible = EditorVariablesManager.ExpandedGeneralShaderOptions.Value;
							NodeUtils.DrawPropertyGroup( ref generalIsVisible, " General", DrawGeneralFunction );
							EditorVariablesManager.ExpandedGeneralShaderOptions.Value = generalIsVisible;

							bool inputIsVisible = EditorVariablesManager.ExpandedFunctionInputs.Value;
							NodeUtils.DrawPropertyGroup( ref inputIsVisible, " Function Inputs", DrawFunctionInputs );
							EditorVariablesManager.ExpandedFunctionInputs.Value = inputIsVisible;

							bool outputIsVisible = EditorVariablesManager.ExpandedFunctionOutputs.Value;
							NodeUtils.DrawPropertyGroup( ref outputIsVisible, " Function Outputs", DrawFunctionOutputs );
							EditorVariablesManager.ExpandedFunctionOutputs.Value = outputIsVisible;

							bool properties = ParentWindow.ExpandedProperties;
							NodeUtils.DrawPropertyGroup( ref properties, " Material Properties", DrawFunctionProperties );
							ParentWindow.ExpandedProperties = properties;

							EditorGUIUtility.labelWidth = labelWidth;
							EditorGUILayout.EndScrollView();
						}
						EditorGUILayout.EndVertical();
					}
				}
				// Close window area
				GUILayout.EndArea();
			}

			PostDraw();
			return changeCheck;
		}

		public void DrawGeneralFunction()
		{
			AmplifyShaderFunction function = ParentWindow.CurrentGraph.CurrentShaderFunction;
			if ( function == null )
				return;

			string oldName = function.FunctionName;

			//oldName = EditorGUILayout.TextField( "Name", oldName );
			//if ( oldName != function.FunctionName )
			//{
			//	function.FunctionName = oldName;
			//	EditorUtility.SetDirty( function );
			//}

			SerializedObject serializedObject = new UnityEditor.SerializedObject( function );

			if ( serializedObject != null )
			{
				SerializedProperty temo = serializedObject.FindProperty( "m_description" );
				oldName = temo.stringValue;
				EditorGUILayout.PropertyField( temo, new GUIContent( "    Description" ) );
				if ( !oldName.Equals( temo.stringValue ) )
				{
					function.Description = temo.stringValue;
					oldName = temo.stringValue;
				}
			}
		}


		public void DrawFunctionInputs()
		{
			List<ParentNode> nodes = UIUtils.FunctionInputList();

			if ( m_functionInputsReordableList == null || nodes.Count != m_functionInputsLastCount )
			{
				//List<FunctionInput> functionInputNodes = new List<FunctionInput>();
				//for ( int i = 0; i < nodes.Count; i++ )
				//{
				//	FunctionInput node = nodes[ i ] as FunctionInput;
				//	if ( node != null )
				//	{
				//		functionInputNodes.Add( node );
				//	}
				//}

				List<FunctionInput> functionInputNodes = new List<FunctionInput>();
				foreach ( FunctionInput y in UIUtils.FunctionInputList() )
				{
					functionInputNodes.Add( y );
				}

				functionInputNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

				m_functionInputsReordableList = new ReorderableList( functionInputNodes, typeof( FunctionInput ), true, false, false, false );
				m_functionInputsReordableList.headerHeight = 0;
				m_functionInputsReordableList.footerHeight = 0;
				m_functionInputsReordableList.showDefaultBackground = false;

				m_functionInputsReordableList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
				{
					EditorGUI.LabelField( rect, functionInputNodes[ index ].InputName );
				};

				m_functionInputsReordableList.onChangedCallback = ( list ) =>
				{
					for ( int i = 0; i < functionInputNodes.Count; i++ )
					{
						functionInputNodes[ i ].OrderIndex = i;
					}
				};

				m_functionInputsLastCount = m_functionInputsReordableList.count;
			}

			if ( m_functionInputsReordableList != null )
			{
				if ( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_functionInputsReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
		}

		public void DrawFunctionOutputs()
		{
			List<ParentNode> nodes = UIUtils.FunctionOutputList();

			if ( m_functionOutputsReordableList == null || nodes.Count != m_functionOutputsLastCount )
			{
				List<FunctionOutput> functionOutputNodes = new List<FunctionOutput>();
				for ( int i = 0; i < nodes.Count; i++ )
				{
					FunctionOutput node = nodes[ i ] as FunctionOutput;
					if ( node != null )
					{
						functionOutputNodes.Add( node );
					}
				}

				functionOutputNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

				m_functionOutputsReordableList = new ReorderableList( functionOutputNodes, typeof( FunctionOutput ), true, false, false, false );
				m_functionOutputsReordableList.headerHeight = 0;
				m_functionOutputsReordableList.footerHeight = 0;
				m_functionOutputsReordableList.showDefaultBackground = false;

				m_functionOutputsReordableList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
				{
					EditorGUI.LabelField( rect, functionOutputNodes[ index ].OutputName );
				};

				m_functionOutputsReordableList.onChangedCallback = ( list ) =>
				{
					for ( int i = 0; i < functionOutputNodes.Count; i++ )
					{
						functionOutputNodes[ i ].OrderIndex = i;
					}
				};

				m_functionOutputsLastCount = m_functionOutputsReordableList.count;
			}

			if ( m_functionOutputsReordableList != null )
			{
				if ( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_functionOutputsReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
		}

		public void DrawFunctionProperties()
		{
			List<ParentNode> nodes = UIUtils.PropertyNodesList();
			if ( m_propertyReordableList == null || nodes.Count != m_lastCount )
			{
				m_propertyReordableNodes.Clear();

				for ( int i = 0; i < nodes.Count; i++ )
				{
					PropertyNode node = nodes[ i ] as PropertyNode;
					if ( node != null )
					{
						ReordenatorNode rnode = nodes[ i ] as ReordenatorNode;
						if ( ( rnode == null || !rnode.IsInside ) && ( !m_propertyReordableNodes.Exists( x => x.PropertyName.Equals( node.PropertyName ) ) ) )
							m_propertyReordableNodes.Add( node );
					}
				}

				m_propertyReordableNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

				m_propertyReordableList = new ReorderableList( m_propertyReordableNodes, typeof( PropertyNode ), true, false, false, false );
				m_propertyReordableList.headerHeight = 0;
				m_propertyReordableList.footerHeight = 0;
				m_propertyReordableList.showDefaultBackground = false;

				m_propertyReordableList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
				{
					EditorGUI.LabelField( rect, /*m_propertyReordableNodes[ index ].OrderIndex + " " + */m_propertyReordableNodes[ index ].PropertyInspectorName );
				};

				m_propertyReordableList.onChangedCallback = ( list ) =>
				{
					ReorderList( ref nodes );
				};

				ReorderList( ref nodes );

				m_lastCount = m_propertyReordableList.count;
			}

			if ( m_propertyReordableList != null )
			{
				if ( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_propertyReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
		}

		private void ReorderList( ref List<ParentNode> nodes )
		{
			for ( int i = 0; i < nodes.Count; i++ )
			{
				ReordenatorNode rnode = nodes[ i ] as ReordenatorNode;
				if ( rnode != null )
					rnode.RecursiveClear();
			}

			int propoffset = 0;
			//int renodeoffset = 0;
			int count = 0;
			for ( int i = 0; i < m_propertyReordableNodes.Count; i++ )
			{
				ReordenatorNode renode = m_propertyReordableNodes[ i ] as ReordenatorNode;
				if ( renode != null )
				{
					if ( !renode.IsInside )
					{
						m_propertyReordableNodes[ i ].OrderIndex = count + propoffset;// - renodeoffset;

						if ( renode.PropertyListCount > 0 )
						{
							propoffset += renode.RecursiveCount();
						}

						for ( int j = 0; j < nodes.Count; j++ )
						{
							ReordenatorNode pnode = ( nodes[ j ] as ReordenatorNode );
							if ( pnode != null && pnode.PropertyName.Equals( renode.PropertyName ) )
							{
								pnode.OrderIndex = renode.RawOrderIndex;
								pnode.RecursiveSetOrderOffset( renode.RawOrderIndex, true );
							}
						}
						count++;
					}
					else
					{
						m_propertyReordableNodes[ i ].OrderIndex = 0;
					}
				}
				else
				{
					m_propertyReordableNodes[ i ].OrderIndex = count + propoffset;// - renodeoffset;
					count++;
				}
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_functionInputsReordableList = null;
			m_functionOutputsReordableList = null;
			m_propertyReordableList = null;
		}

		public bool ForceUpdate
		{
			get { return m_forceUpdate; }
			set { m_forceUpdate = value; }
		}
	}
}
