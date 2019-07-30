// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#define SHOW_TEMPLATE_HELP_BOX

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	

	[Serializable]
	[NodeAttributes( "Template Master Node", "Master", "Shader Generated according to template rules", null, KeyCode.None, false )]
	public class TemplateMasterNode : MasterNode
	{
		private const string WarningMessage = "Templates is a feature that is still heavily under development and users may experience some problems.\nPlease email support@amplify.pt if any issue occurs.";
		private const string CurrentTemplateLabel = "Current Template";
		private const string OpenTemplateStr = "Edit Template";
		//protected const string SnippetsFoldoutStr = " Snippets";
		//[SerializeField]
		//private bool m_snippetsFoldout = true;

		[SerializeField]
		private TemplateData m_currentTemplate = null;

		private bool m_fireTemplateChange = false;

		private bool m_fetchMasterNodeCategory = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_masterNodeCategory = 1;// First Template
			m_marginPreviewLeft = 20;
			m_insideSize.y = 60;
		}

		public override void ReleaseResources()
		{
			if ( m_currentTemplate != null && m_currentTemplate.AvailableShaderProperties != null )
			{
				// Unregister old template properties
				int oldPropertyCount = m_currentTemplate.AvailableShaderProperties.Count;
				for ( int i = 0; i < oldPropertyCount; i++ )
				{
					UIUtils.ReleaseUniformName( UniqueId, m_currentTemplate.AvailableShaderProperties[ i ].PropertyName );
				}
			}
		}

		public override void RefreshAvailableCategories()
		{
			int templateCount = TemplatesManager.TemplateCount;
			m_availableCategories = new MasterNodeCategoriesData[ templateCount + 1 ];
			m_availableCategoryLabels = new GUIContent[ templateCount + 1 ];

			m_availableCategories[ 0 ] = new MasterNodeCategoriesData( AvailableShaderTypes.SurfaceShader, string.Empty );
			m_availableCategoryLabels[ 0 ] = new GUIContent( "Surface Shader" );
			if ( m_currentTemplate == null )
			{
				m_masterNodeCategory = -1;
			}

			for ( int i = 0; i < templateCount; i++ )
			{
				int idx = i + 1;
				TemplateData templateData = TemplatesManager.GetTemplate( i );

				if ( m_currentTemplate != null && m_currentTemplate.GUID.Equals( templateData.GUID ) )
					m_masterNodeCategory = idx;

				m_availableCategories[ idx ] = new MasterNodeCategoriesData( AvailableShaderTypes.Template, templateData.GUID );
				m_availableCategoryLabels[ idx ] = new GUIContent( templateData.Name );
			}
		}

		void SetCategoryIdxFromTemplate()
		{
			int templateCount = TemplatesManager.TemplateCount;
			for ( int i = 0; i < templateCount; i++ )
			{
				int idx = i + 1;
				TemplateData templateData = TemplatesManager.GetTemplate( i );
				if ( templateData != null && m_currentTemplate != null && m_currentTemplate.GUID.Equals( templateData.GUID ) )
					m_masterNodeCategory = idx;
			}
		}

		public void SetTemplate( TemplateData newTemplate, bool writeDefaultData, bool fetchMasterNodeCategory )
		{
			ReleaseResources();

			if ( newTemplate == null || newTemplate.InputDataList == null )
				return;

			m_fetchMasterNodeCategory = fetchMasterNodeCategory;

			DeleteAllInputConnections( true );
			m_currentTemplate = newTemplate;
			m_currentShaderData = newTemplate.Name;

			List<TemplateInputData> inputDataList = newTemplate.InputDataList;
			int count = inputDataList.Count;
			for ( int i = 0; i < count; i++ )
			{
				AddInputPort( inputDataList[ i ].DataType, false, inputDataList[ i ].PortName, inputDataList[ i ].OrderId, inputDataList[ i ].PortCategory, inputDataList[ i ].PortUniqueId );
			}

			if ( writeDefaultData )
			{
				ShaderName = newTemplate.DefaultShaderName;
			}

			// Register old template properties
			int newPropertyCount = m_currentTemplate.AvailableShaderProperties.Count;
			for ( int i = 0; i < newPropertyCount; i++ )
			{
				int nodeId = UIUtils.CheckUniformNameOwner( m_currentTemplate.AvailableShaderProperties[ i ].PropertyName );
				if ( nodeId > -1 )
				{
					ParentNode node = m_containerGraph.GetNode( nodeId );
					if ( node != null )
					{
						UIUtils.ShowMessage( string.Format( "Template requires property name {0} which is currently being used by {1}. Please rename it and reload template.", m_currentTemplate.AvailableShaderProperties[ i ].PropertyName, node.Attributes.Name ) );
					}
					else
					{
						UIUtils.ShowMessage( string.Format( "Template requires property name {0} which is currently being on your graph. Please rename it and reload template.", m_currentTemplate.AvailableShaderProperties[ i ].PropertyName ) );
					}
				}
				else
				{
					UIUtils.RegisterUniformName( UniqueId, m_currentTemplate.AvailableShaderProperties[ i ].PropertyName );
				}
			}

			m_fireTemplateChange = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			bool generalIsVisible = EditorVariablesManager.ExpandedGeneralShaderOptions.Value;
			NodeUtils.DrawPropertyGroup( ref generalIsVisible, GeneralFoldoutStr, DrawGeneralOptions );
			EditorVariablesManager.ExpandedGeneralShaderOptions.Value = generalIsVisible;
			//	NodeUtils.DrawPropertyGroup( ref m_snippetsFoldout, SnippetsFoldoutStr, DrawSnippetOptions );
			if ( GUILayout.Button( OpenTemplateStr ) && m_currentTemplate != null )
			{
				AssetDatabase.OpenAsset( AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( m_currentTemplate.GUID ) ), 1 );
			}
#if SHOW_TEMPLATE_HELP_BOX
			EditorGUILayout.HelpBox( WarningMessage, MessageType.Warning );
#endif

		}

		public void DrawGeneralOptions()
		{
			DrawShaderName();
			DrawCurrentShaderType();
		}

		public void DrawSnippetOptions()
		{
			m_currentTemplate.DrawSnippetProperties( this );
		}

		protected bool CreateInstructionsForList( ref List<InputPort> ports, ref string shaderBody, ref List<string> vertexInstructions, ref List<string> fragmentInstructions )
		{
			if ( ports.Count == 0 )
				return true;

			bool isValid = true;
			UIUtils.CurrentWindow.CurrentGraph.ResetNodesLocalVariables();
			for ( int i = 0; i < ports.Count; i++ )
			{
				TemplateInputData inputData = m_currentTemplate.InputDataFromId( ports[ i ].PortId );
				if ( ports[ i ].IsConnected )
				{
					m_currentDataCollector.ResetInstructions();
					m_currentDataCollector.ResetVertexInstructions();

					m_currentDataCollector.PortCategory = ports[ i ].Category;
					string newPortInstruction = ports[ i ].GeneratePortInstructions( ref m_currentDataCollector );


					if ( m_currentDataCollector.DirtySpecialLocalVariables )
					{
						for ( int localIdx = 0; localIdx < m_currentDataCollector.SpecialLocalVariablesList.Count; localIdx++ )
						{
							m_currentDataCollector.AddInstructions( m_currentDataCollector.SpecialLocalVariablesList[ localIdx ].PropertyName );
						}
						m_currentDataCollector.ClearSpecialLocalVariables();
					}

					if ( m_currentDataCollector.DirtyVertexVariables )
					{
						for ( int localIdx = 0; localIdx < m_currentDataCollector.VertexLocalVariablesList.Count; localIdx++ )
						{
							m_currentDataCollector.AddVertexInstruction( m_currentDataCollector.VertexLocalVariablesList[ localIdx ].PropertyName, ports[ i ].NodeId, false );
						}
						m_currentDataCollector.ClearVertexLocalVariables();
					}

					// fill functions 
					for ( int j = 0; j < m_currentDataCollector.InstructionsList.Count; j++ )
					{
						fragmentInstructions.Add( m_currentDataCollector.InstructionsList[ j ].PropertyName );
					}

					for ( int j = 0; j < m_currentDataCollector.VertexDataList.Count; j++ )
					{
						vertexInstructions.Add( m_currentDataCollector.VertexDataList[ j ].PropertyName );
					}

					isValid = m_currentTemplate.FillTemplateBody( inputData.TagId, ref shaderBody, newPortInstruction ) && isValid;
				}
				else
				{
					isValid = m_currentTemplate.FillTemplateBody( inputData.TagId, ref shaderBody, inputData.DefaultValue ) && isValid;
				}
			}
			return isValid;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			
			if ( m_containerGraph.IsInstancedShader )
			{
				DrawInstancedIcon( drawInfo );
			}
			
			if ( m_fetchMasterNodeCategory )
			{
				if ( m_availableCategories != null )
				{
					m_fetchMasterNodeCategory = false;
					SetCategoryIdxFromTemplate();
				}
			}

			if ( m_fireTemplateChange )
			{
				m_fireTemplateChange = false;
				m_containerGraph.FireMasterNodeReplacedEvent();
			}
		}

		public override void UpdateFromShader( Shader newShader )
		{
			if ( m_currentMaterial != null )
			{
				m_currentMaterial.shader = newShader;
			}
			CurrentShader = newShader;
		}

		public override void UpdateMasterNodeMaterial( Material material )
		{
			m_currentMaterial = material;
			FireMaterialChangedEvt();
		}

		public override Shader Execute( string pathname, bool isFullPath )
		{
			if ( m_currentTemplate == null )
				return m_currentShader;

			//Create data collector
			base.Execute( pathname, isFullPath );

			m_currentDataCollector.TemplateDataCollectorInstance.BuildFromTemplateData( m_currentDataCollector, m_currentTemplate );
			int shaderPropertiesAmount = m_currentTemplate.AvailableShaderProperties.Count;
			for ( int i = 0; i < shaderPropertiesAmount; i++ )
			{
				m_currentDataCollector.SoftRegisterUniform( m_currentTemplate.AvailableShaderProperties[ i ].PropertyName );
			}
			
			//Sort ports by both 
			List<InputPort> fragmentPorts = new List<InputPort>();
			List<InputPort> vertexPorts = new List<InputPort>();
			SortInputPorts( ref vertexPorts, ref fragmentPorts );

			string shaderBody = m_currentTemplate.TemplateBody;

			List<string> vertexInstructions = new List<string>();
			List<string> fragmentInstructions = new List<string>();

			bool validBody = true;

			validBody = CreateInstructionsForList( ref fragmentPorts, ref shaderBody, ref vertexInstructions, ref fragmentInstructions ) && validBody;
			validBody = CreateInstructionsForList( ref vertexPorts, ref shaderBody, ref vertexInstructions, ref fragmentInstructions ) && validBody;

			m_currentTemplate.ResetTemplateUsageData();

			// Fill vertex interpolators assignment
			for ( int i = 0; i < m_currentDataCollector.VertexInterpDeclList.Count; i++ )
			{
				vertexInstructions.Add( m_currentDataCollector.VertexInterpDeclList[ i ] );
			}

			vertexInstructions.AddRange( m_currentDataCollector.TemplateDataCollectorInstance.GetInterpUnusedChannels() );
			//Fill common local variables and operations
			
			validBody = m_currentTemplate.FillVertexInstructions( ref shaderBody, vertexInstructions.ToArray() ) && validBody;
			validBody = m_currentTemplate.FillFragmentInstructions( ref shaderBody, fragmentInstructions.ToArray() ) && validBody;

			// Add Instanced Properties
			if ( m_containerGraph.IsInstancedShader )
			{
				m_currentDataCollector.TabifyInstancedVars();
				m_currentDataCollector.InstancedPropertiesList.Insert( 0, new PropertyDataCollector( -1, string.Format( IOUtils.InstancedPropertiesBegin, UIUtils.RemoveInvalidCharacters( m_shaderName ) ) ) );
				m_currentDataCollector.InstancedPropertiesList.Add( new PropertyDataCollector( -1, IOUtils.InstancedPropertiesEnd ) );
				m_currentDataCollector.UniformsList.AddRange( m_currentDataCollector.InstancedPropertiesList );
			}


			//Add Functions
			m_currentDataCollector.UniformsList.AddRange( m_currentDataCollector.FunctionsList );

			// Fill common tags
			m_currentDataCollector.IncludesList.AddRange( m_currentDataCollector.PragmasList );

			validBody = m_currentTemplate.FillTemplateBody( m_currentTemplate.ShaderNameId, ref shaderBody, string.Format( TemplatesManager.NameFormatter, m_shaderName ) ) && validBody;
			validBody = m_currentTemplate.FillTemplateBody( TemplatesManager.TemplatePassTag, ref shaderBody, m_currentDataCollector.GrabPassList ) && validBody;
			validBody = m_currentTemplate.FillTemplateBody( TemplatesManager.TemplatePragmaTag, ref shaderBody, m_currentDataCollector.IncludesList ) && validBody;
			validBody = m_currentTemplate.FillTemplateBody( TemplatesManager.TemplateTagsTag, ref shaderBody, m_currentDataCollector.TagsList ) && validBody;
			validBody = m_currentTemplate.FillTemplateBody( TemplatesManager.TemplatePropertyTag, ref shaderBody, m_currentDataCollector.PropertiesList ) && validBody;
			validBody = m_currentTemplate.FillTemplateBody( TemplatesManager.TemplateGlobalsTag, ref shaderBody, m_currentDataCollector.UniformsList ) && validBody;
			validBody = m_currentTemplate.FillTemplateBody( m_currentTemplate.VertexDataId, ref shaderBody, m_currentDataCollector.VertexInputList.ToArray() ) && validBody;
			validBody = m_currentTemplate.FillTemplateBody( m_currentTemplate.InterpDataId, ref shaderBody, m_currentDataCollector.InterpolatorList.ToArray() ) && validBody;
			
			if ( m_currentDataCollector.TemplateDataCollectorInstance.HasVertexInputParams )
			{
				validBody = m_currentTemplate.FillTemplateBody( TemplatesManager.TemplateInputsVertParamsTag, ref shaderBody, m_currentDataCollector.TemplateDataCollectorInstance.VertexInputParamsStr ) && validBody;
			}
			
			if ( m_currentDataCollector.TemplateDataCollectorInstance.HasFragmentInputParams )
			{
				validBody = m_currentTemplate.FillTemplateBody( TemplatesManager.TemplateInputsFragParamsTag, ref shaderBody, m_currentDataCollector.TemplateDataCollectorInstance.FragInputParamsStr ) && validBody;
			}
			
			m_currentTemplate.FillEmptyTags( ref shaderBody );

			m_currentTemplate.InsertSnippets( ref shaderBody );

			vertexInstructions.Clear();
			vertexInstructions = null;

			fragmentInstructions.Clear();
			fragmentInstructions = null;
			if ( validBody )
			{
				UpdateShaderAsset( ref pathname, ref shaderBody, isFullPath );
			}

			return m_currentShader;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			try
			{
				ShaderName = GetCurrentParam( ref nodeParams );
				if ( m_shaderName.Length > 0 )
					ShaderName = UIUtils.RemoveShaderInvalidCharacters( ShaderName );

				string templateName = GetCurrentParam( ref nodeParams );
				TemplateData template = TemplatesManager.GetTemplate( templateName );
				if ( template != null )
				{
					SetTemplate( template, false, true );
				}
				else
				{
					m_masterNodeCategory = -1;
				}
			}
			catch ( Exception e )
			{
				Debug.LogException( e, this );
			}
			m_containerGraph.CurrentCanvasMode = NodeAvailability.TemplateShader;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_currentTemplate = null;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shaderName );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_currentTemplate != null ) ? m_currentTemplate.GUID : string.Empty );
		}

		public TemplateData CurrentTemplate { get { return m_currentTemplate; } }
	}
}
