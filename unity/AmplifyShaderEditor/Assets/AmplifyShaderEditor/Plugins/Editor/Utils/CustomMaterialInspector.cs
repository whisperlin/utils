// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using AmplifyShaderEditor;

//[CustomEditor( typeof( Material ) )]
internal class ASEMaterialInspector : ShaderGUI
{
	private const string CopyButtonStr = "Copy Values";
	private const string PasteButtonStr = "Paste Values";
	private static MaterialEditor m_instance = null;
	public override void OnGUI( MaterialEditor materialEditor, MaterialProperty[] properties )
	{
		IOUtils.Init();
		Material mat = materialEditor.target as Material;

		if ( mat == null )
			return;

		m_instance = materialEditor;
		

		if ( Event.current.type == EventType.repaint &&
			mat.HasProperty( IOUtils.DefaultASEDirtyCheckId ) &&
			mat.GetInt( IOUtils.DefaultASEDirtyCheckId ) == 1 )
		{
			mat.SetInt( IOUtils.DefaultASEDirtyCheckId, 0 );
			UIUtils.ForceUpdateFromMaterial();
#if !UNITY_5_5_OR_NEWER
			Event.current.Use();
#endif
		}



		if ( materialEditor.isVisible )
		{
			GUILayout.BeginVertical();
			{
				GUILayout.Space( 3 );
				if ( GUILayout.Button( "Open in Shader Editor" ) )
				{
					AmplifyShaderEditorWindow.LoadMaterialToASE( mat );
				}
				
				GUILayout.BeginHorizontal();
				{
					if ( GUILayout.Button( CopyButtonStr ) )
					{
						Shader shader = mat.shader;
						int propertyCount = UnityEditor.ShaderUtil.GetPropertyCount( shader );
						string allProperties = string.Empty;
						for ( int i = 0; i < propertyCount; i++ )
						{
							UnityEditor.ShaderUtil.ShaderPropertyType type = UnityEditor.ShaderUtil.GetPropertyType( shader, i );
							string name = UnityEditor.ShaderUtil.GetPropertyName( shader, i );
							string valueStr = string.Empty;
							switch ( type )
							{
								case UnityEditor.ShaderUtil.ShaderPropertyType.Color:
								{
									Color value = mat.GetColor( name );
									valueStr = value.r.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.g.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.b.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.a.ToString();
								}
								break;
								case UnityEditor.ShaderUtil.ShaderPropertyType.Vector:
								{
									Vector4 value = mat.GetVector( name );
									valueStr = value.x.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.y.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.z.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.w.ToString();
								}
								break;
								case UnityEditor.ShaderUtil.ShaderPropertyType.Float:
								{
									float value = mat.GetFloat( name );
									valueStr = value.ToString();
								}
								break;
								case UnityEditor.ShaderUtil.ShaderPropertyType.Range:
								{
									float value = mat.GetFloat( name );
									valueStr = value.ToString();
								}
								break;
								case UnityEditor.ShaderUtil.ShaderPropertyType.TexEnv:
								{
									Texture value = mat.GetTexture( name );
									valueStr = AssetDatabase.GetAssetPath( value );
								}
								break;
							}

							allProperties += name + IOUtils.FIELD_SEPARATOR + type + IOUtils.FIELD_SEPARATOR + valueStr;

							if ( i < ( propertyCount - 1 ) )
							{
								allProperties += IOUtils.LINE_TERMINATOR;
							}
						}
						EditorPrefs.SetString( IOUtils.MAT_CLIPBOARD_ID, allProperties );
					}

					if ( GUILayout.Button( PasteButtonStr ) )
					{
						string propertiesStr = EditorPrefs.GetString( IOUtils.MAT_CLIPBOARD_ID, string.Empty );
						if ( !string.IsNullOrEmpty( propertiesStr ) )
						{
							string[] propertyArr = propertiesStr.Split( IOUtils.LINE_TERMINATOR );
							bool validData = true;
							try
							{
								for ( int i = 0; i < propertyArr.Length; i++ )
								{
									string[] valuesArr = propertyArr[ i ].Split( IOUtils.FIELD_SEPARATOR );
									if ( valuesArr.Length != 3 )
									{
										Debug.LogWarning( "Material clipboard data is corrupted" );
										validData = false;
										break;
									}
									else if ( mat.HasProperty( valuesArr[ 0 ] ) )
									{
										UnityEditor.ShaderUtil.ShaderPropertyType type = ( UnityEditor.ShaderUtil.ShaderPropertyType ) Enum.Parse( typeof( UnityEditor.ShaderUtil.ShaderPropertyType ), valuesArr[ 1 ] );
										switch ( type )
										{
											case UnityEditor.ShaderUtil.ShaderPropertyType.Color:
											{
												string[] colorVals = valuesArr[ 2 ].Split( IOUtils.VECTOR_SEPARATOR );
												if ( colorVals.Length != 4 )
												{
													Debug.LogWarning( "Material clipboard data is corrupted" );
													validData = false;
													break;
												}
												else
												{
													mat.SetColor( valuesArr[ 0 ], new Color( Convert.ToSingle( colorVals[ 0 ] ),
																								Convert.ToSingle( colorVals[ 1 ] ),
																								Convert.ToSingle( colorVals[ 2 ] ),
																								Convert.ToSingle( colorVals[ 3 ] ) ) );
												}
											}
											break;
											case UnityEditor.ShaderUtil.ShaderPropertyType.Vector:
											{
												string[] vectorVals = valuesArr[ 2 ].Split( IOUtils.VECTOR_SEPARATOR );
												if ( vectorVals.Length != 4 )
												{
													Debug.LogWarning( "Material clipboard data is corrupted" );
													validData = false;
													break;
												}
												else
												{
													mat.SetVector( valuesArr[ 0 ], new Vector4( Convert.ToSingle( vectorVals[ 0 ] ),
																								Convert.ToSingle( vectorVals[ 1 ] ),
																								Convert.ToSingle( vectorVals[ 2 ] ),
																								Convert.ToSingle( vectorVals[ 3 ] ) ) );
												}
											}
											break;
											case UnityEditor.ShaderUtil.ShaderPropertyType.Float:
											{
												mat.SetFloat( valuesArr[ 0 ], Convert.ToSingle( valuesArr[ 2 ] ) );
											}
											break;
											case UnityEditor.ShaderUtil.ShaderPropertyType.Range:
											{
												mat.SetFloat( valuesArr[ 0 ], Convert.ToSingle( valuesArr[ 2 ] ) );
											}
											break;
											case UnityEditor.ShaderUtil.ShaderPropertyType.TexEnv:
											{
												mat.SetTexture( valuesArr[ 0 ], AssetDatabase.LoadAssetAtPath<Texture>( valuesArr[ 2 ] ) );
											}
											break;
										}
									}
								}
							}
							catch ( Exception e )
							{
								Debug.LogException( e );
								validData = false;
							}


							if ( validData )
							{
								materialEditor.PropertiesChanged();
								UIUtils.CopyValuesFromMaterial( mat );
							}
							else
							{
								EditorPrefs.SetString( IOUtils.MAT_CLIPBOARD_ID, string.Empty );
							}
						}
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space( 5 );
			}
			GUILayout.EndVertical();
		}
		EditorGUI.BeginChangeCheck();
		base.OnGUI( materialEditor, properties );
		materialEditor.LightmapEmissionProperty();
		if ( EditorGUI.EndChangeCheck() )
		{
			//mat.globalIlluminationFlags &= ( MaterialGlobalIlluminationFlags ) 3;
			string isEmissive = mat.GetTag( "IsEmissive", false, "false" );
			if ( isEmissive.Equals("true") )
			{
				mat.globalIlluminationFlags &= ( MaterialGlobalIlluminationFlags )3;
			}
			else
			{
				mat.globalIlluminationFlags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
			}

			UIUtils.CopyValuesFromMaterial( mat );
		}
	}

	public static MaterialEditor Instance { get { return m_instance; } set { m_instance = value; } }

	
}
