using UnityEditor;
using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class UndoParentNode : ScriptableObject
	{
		private const string MessageFormat = "Changing value {0} on node {1}";

		[SerializeField]
		protected NodeAttributes m_nodeAttribs;

		public void UndoRecordObject( UndoParentNode objectToUndo, string name )
		{
			UIUtils.MarkUndoAction();
			Undo.RegisterCompleteObjectUndo( UIUtils.CurrentWindow, name );
			Undo.RecordObject( objectToUndo, name );
		}

		public string EditorGUILayoutStringField( string name, string value, params GUILayoutOption[] options )
		{
			string newValue = EditorGUILayout.TextField( name, value, options );
			if ( !newValue.Equals( value ) )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUILayoutStringField", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public string EditorGUILayoutTextField( GUIContent label, string text, params GUILayoutOption[] options )
		{
			string newValue = EditorGUILayout.TextField( label, text, options );
			if ( !text.Equals( newValue ) )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public string EditorGUILayoutTextField( string label, string text, params GUILayoutOption[] options )
		{
			string newValue = EditorGUILayout.TextField( label, text, options );
			if ( !text.Equals( newValue ) )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Enum EditorGUILayoutEnumPopup( GUIContent label, Enum selected, params GUILayoutOption[] options )
		{
			Enum newValue = EditorGUILayout.EnumPopup( label, selected, options );
			if ( !newValue.ToString().Equals( selected.ToString() ) )
			{
				UndoRecordObject( this, string.Concat( "Changing value ", label, " on node ", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
				//UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Enum EditorGUILayoutEnumPopup( string label, Enum selected, params GUILayoutOption[] options )
		{
			Enum newValue = EditorGUILayout.EnumPopup( label, selected, options );
			if ( !newValue.ToString().Equals( selected.ToString() ) )
			{
				UndoRecordObject( this, string.Concat( "Changing value ", label, " on node ", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
				//UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Enum EditorGUILayoutEnumPopup( Enum selected, params GUILayoutOption[] options )
		{
			Enum newValue = EditorGUILayout.EnumPopup( selected, options );
			if ( !newValue.ToString().Equals( selected.ToString() ) )
			{
				UndoRecordObject( this, string.Concat( "Changing value EditorGUILayoutEnumPopup on node ", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
				//UndoRecordObject( this, string.Format( MessageFormat, "EditorGUILayoutEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntPopup( string label, int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntPopup( label, selectedValue, displayedOptions, optionValues, options );
			if ( newValue != selectedValue )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}


		public int EditorGUILayoutPopup( string label, int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( label, selectedIndex, displayedOptions, style, options );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}


		public int EditorGUILayoutPopup( GUIContent label, int selectedIndex, GUIContent[] displayedOptions, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( label, selectedIndex, displayedOptions, options );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
		
		public int EditorGUILayoutPopup( GUIContent label, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( label, selectedIndex, displayedOptions, style, options );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
		
		public int EditorGUILayoutPopup( int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( selectedIndex, displayedOptions, options );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUILayoutPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutPopup( string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( label, selectedIndex, displayedOptions, options );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutToggle( GUIContent label, bool value, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.Toggle( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutToggle( string label, bool value, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.Toggle( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutToggle( string label, bool value, GUIStyle style, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.Toggle( label, value, style, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntField( int value, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntField( value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUILayoutIntField", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntField( GUIContent label, int value, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntField( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntField( string label, int value, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntField( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUILayoutFloatField( GUIContent label, float value, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.FloatField( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUILayoutFloatField( string label, float value, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.FloatField( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Color EditorGUILayoutColorField( string label, Color value, params GUILayoutOption[] options )
		{
			Color newValue = EditorGUILayout.ColorField( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Color EditorGUILayoutColorField( GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig, params GUILayoutOption[] options )
		{
			Color newValue = EditorGUILayout.ColorField( label, value, showEyedropper, showAlpha, hdr, hdrConfig, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
		
		public float EditorGUILayoutSlider( string label, float value, float leftValue, float rightValue, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.Slider( label, value, leftValue, rightValue, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUILayoutSlider( GUIContent label, float value, float leftValue, float rightValue, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.Slider( label, value, leftValue, rightValue, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
		public UnityEngine.Object EditorGUILayoutObjectField( string label, UnityEngine.Object obj, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options )
		{
			UnityEngine.Object newValue = EditorGUILayout.ObjectField( label, obj, objType, allowSceneObjects, options );
			if ( newValue != obj )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Vector2 EditorGUILayoutVector2Field( string label, Vector2 value, params GUILayoutOption[] options )
		{
			Vector2 newValue = EditorGUILayout.Vector2Field( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Vector3 EditorGUILayoutVector3Field( string label, Vector3 value, params GUILayoutOption[] options )
		{
			Vector3 newValue = EditorGUILayout.Vector3Field( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Vector4 EditorGUILayoutVector4Field( string label, Vector4 value, params GUILayoutOption[] options )
		{
			Vector4 newValue = EditorGUILayout.Vector4Field( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
		public int EditorGUILayoutIntSlider( GUIContent label, int value, int leftValue, int rightValue, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntSlider( label, value, leftValue, rightValue, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntSlider( string label, int value, int leftValue, int rightValue, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntSlider( label, value, leftValue, rightValue, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutToggleLeft( string label, bool value, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.ToggleLeft( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutToggleLeft( GUIContent label, bool value, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.ToggleLeft( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public string EditorGUILayoutTextArea( string text, GUIStyle style, params GUILayoutOption[] options )
		{
			string newValue = EditorGUILayout.TextArea( text, style, options );
			if ( !newValue.Equals( text ) )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUILayoutTextArea", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutFoldout( bool foldout, string content )
		{
			bool newValue = EditorGUILayout.Foldout( foldout, content );
			if ( newValue != foldout )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUILayoutFoldout", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;

		}

		public string EditorGUITextField( Rect position, string label, string text, [UnityEngine.Internal.DefaultValue( "EditorStyles.textField" )] GUIStyle style )
		{
			string newValue = EditorGUI.TextField( position, label, text, style );
			if ( !newValue.Equals( text ) )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Color EditorGUIColorField( Rect position, GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig )
		{
			Color newValue = EditorGUI.ColorField( position, label, value, showEyedropper, showAlpha, hdr, hdrConfig );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUIIntField( Rect position, string label, int value, [UnityEngine.Internal.DefaultValue( "EditorStyles.numberField" )] GUIStyle style )
		{
			int newValue = EditorGUI.IntField( position, label, value, style );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUIFloatField( Rect position, string label, float value, [UnityEngine.Internal.DefaultValue( "EditorStyles.numberField" )] GUIStyle style )
		{
			float newValue = EditorGUI.FloatField( position, label, value, style );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUIFloatField( Rect position, float value, [UnityEngine.Internal.DefaultValue( "EditorStyles.numberField" )] GUIStyle style )
		{
			float newValue = EditorGUI.FloatField( position, value, style );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUIFloatField", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Enum EditorGUIEnumPopup( Rect position, Enum selected, [UnityEngine.Internal.DefaultValue( "EditorStyles.popup" )] GUIStyle style )
		{
			Enum newValue = EditorGUI.EnumPopup( position, selected, style );
			if ( !newValue.ToString().Equals( selected.ToString() ) )
			{
				UndoRecordObject( this, string.Concat( "Changing value EditorGUIEnumPopup on node ", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
				//UndoRecordObject( this, string.Format( MessageFormat, "EditorGUIEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUIIntPopup( Rect position, int selectedValue, GUIContent[] displayedOptions, int[] optionValues, [UnityEngine.Internal.DefaultValue( "EditorStyles.popup" )] GUIStyle style )
		{
			int newValue = EditorGUI.IntPopup( position, selectedValue, displayedOptions, optionValues, style );
			if ( newValue != selectedValue )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUIIntEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUIPopup( Rect position, int selectedIndex, GUIContent[] displayedOptions, [UnityEngine.Internal.DefaultValue( "EditorStyles.popup" )] GUIStyle style )
		{
			int newValue = EditorGUI.Popup( position, selectedIndex, displayedOptions, style );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUIEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUIPopup( Rect position, int selectedIndex, string[] displayedOptions, [UnityEngine.Internal.DefaultValue( "EditorStyles.popup" )] GUIStyle style )
		{
			int newValue = EditorGUI.Popup( position, selectedIndex, displayedOptions, style );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUIEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public UnityEngine.Object EditorGUIObjectField( Rect position, UnityEngine.Object obj, System.Type objType, bool allowSceneObjects )
		{
			UnityEngine.Object newValue = EditorGUI.ObjectField( position, obj, objType, allowSceneObjects );
			if ( newValue != obj )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUIObjectField", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
		public int EditorGUIIntPopup( Rect position, int selectedValue, string[] displayedOptions, int[] optionValues, [UnityEngine.Internal.DefaultValue( "EditorStyles.popup" )] GUIStyle style )
		{
			int newValue = EditorGUI.IntPopup( position, selectedValue, displayedOptions, optionValues, style );
			if ( newValue != selectedValue )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUIIntPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUIToggle( Rect position, bool value )
		{
			bool newValue = EditorGUI.Toggle( position, value );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "EditorGUIToggle", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public string GUITextField( Rect position, string text, GUIStyle style )
		{
			string newValue = GUI.TextField( position, text, style );
			if ( !newValue.Equals( text ) )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "GUITextfield", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}


		public bool GUILayoutToggle( bool value, string text, GUIStyle style, params GUILayoutOption[] options )
		{
			bool newValue = GUILayout.Toggle( value, text, style, options );
			if ( newValue != value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "GUILayoutToggle", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool GUILayoutButton( string text, GUIStyle style, params GUILayoutOption[] options )
		{
			bool value = GUILayout.Button( text, style, options );
			if ( value )
			{
				UndoRecordObject( this, string.Format( MessageFormat, "GUILayoutButton", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return value;
		}

	}
}

