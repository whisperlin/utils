using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public class InterpDataHelper
	{
		public string VarName;
		public WirePortDataType VarType;
		public InterpDataHelper( WirePortDataType varType, string varName )
		{
			VarName = varName;
			VarType = varType;
		}
	}

	public class TemplateCustomData
	{
		public WirePortDataType DataType;
		public string Name;
		public bool IsVertex;
		public bool IsFragment;
		public TemplateCustomData( string name, WirePortDataType dataType )
		{
			name = Name;
			DataType = dataType;
			IsVertex = false;
			IsFragment = false;
		}
	}

	public class TemplateInputParameters
	{
		public WirePortDataType Type;
		public string Name;
		public string Declaration;
		public TemplateSemantics Semantic;
		public TemplateInputParameters( WirePortDataType type, PrecisionType precision, string name, TemplateSemantics semantic )
		{
			Type = type;
			Name = name;
			Semantic = semantic;
			Declaration = string.Format( "{0} {1} : {2}", UIUtils.PrecisionWirePortToCgType( precision, type ), Name, Semantic );
		}
	}

	public class TemplateDataCollector
	{
		private Dictionary<string, TemplateCustomData> m_customInterpolatedData;

		private Dictionary<TemplateInfoOnSematics, InterpDataHelper> m_availableFragData;
		private Dictionary<TemplateInfoOnSematics, InterpDataHelper> m_availableVertData;
		private TemplateInterpData m_interpolatorData;
		private Dictionary<TemplateSemantics, TemplateVertexData> m_vertexDataDict;
		private TemplateData m_currentTemplateData;
		private MasterNodeDataCollector m_currentDataCollector;
		public Dictionary<TemplateSemantics, TemplateInputParameters> m_vertexInputParams;
		public Dictionary<TemplateSemantics, TemplateInputParameters> m_fragmentInputParams;

		public void BuildFromTemplateData( MasterNodeDataCollector dataCollector, TemplateData templateData )
		{
			m_customInterpolatedData = new Dictionary<string, TemplateCustomData>();


			m_currentDataCollector = dataCollector;
			m_currentTemplateData = templateData;

			m_vertexDataDict = new Dictionary<TemplateSemantics, TemplateVertexData>();
			for ( int i = 0; i < templateData.VertexDataList.Count; i++ )
			{
				m_vertexDataDict.Add( templateData.VertexDataList[ i ].Semantics, new TemplateVertexData( templateData.VertexDataList[ i ] ) );
			}

			m_interpolatorData = new TemplateInterpData( templateData.InterpolatorData );

			m_availableFragData = new Dictionary<TemplateInfoOnSematics, InterpDataHelper>();
			int fragCount = templateData.InterpolatorData.Interpolators.Count;
			for ( int i = 0; i < fragCount; i++ )
			{
				m_availableFragData.Add( templateData.InterpolatorData.Interpolators[ i ].DataInfo,
				new InterpDataHelper( templateData.InterpolatorData.Interpolators[ i ].SwizzleType,
				string.Format( TemplateHelperFunctions.TemplateVarFormat,
				templateData.FragFunctionData.InVarName,
				templateData.InterpolatorData.Interpolators[ i ].VarNameWithSwizzle ) ) );
			}

			m_availableVertData = new Dictionary<TemplateInfoOnSematics, InterpDataHelper>();
			if ( templateData.VertexFunctionData != null )
			{
				int vertCount = templateData.VertexDataList.Count;
				for ( int i = 0; i < vertCount; i++ )
				{
					m_availableVertData.Add( templateData.VertexDataList[ i ].DataInfo,
					new InterpDataHelper( templateData.VertexDataList[ i ].SwizzleType,
					string.Format( TemplateHelperFunctions.TemplateVarFormat,
					templateData.VertexFunctionData.InVarName,
					templateData.VertexDataList[ i ].VarNameWithSwizzle ) ) );
				}
			}
		}

		public void RegisterFragInputParams( WirePortDataType type, PrecisionType precision, string name, TemplateSemantics semantic )
		{
			if ( m_fragmentInputParams == null )
				m_fragmentInputParams = new Dictionary<TemplateSemantics, TemplateInputParameters>();

			m_fragmentInputParams.Add( semantic, new TemplateInputParameters( type, precision, name, semantic ) );
		}

		public void RegisterVertexInputParams( WirePortDataType type, PrecisionType precision, string name, TemplateSemantics semantic )
		{
			if ( m_vertexInputParams == null )
				m_vertexInputParams = new Dictionary<TemplateSemantics, TemplateInputParameters>();

			m_vertexInputParams.Add( semantic, new TemplateInputParameters( type, precision, name, semantic ) );
		}

		public string GetVFace()
		{
			if ( m_fragmentInputParams != null && m_fragmentInputParams.ContainsKey( TemplateSemantics.VFACE ) )
				return m_fragmentInputParams[ TemplateSemantics.VFACE ].Name;

			RegisterFragInputParams( WirePortDataType.FLOAT, PrecisionType.Fixed, TemplateHelperFunctions.SemanticsDefaultName[ TemplateSemantics.VFACE ], TemplateSemantics.VFACE );
			return m_fragmentInputParams[ TemplateSemantics.VFACE ].Name;
		}

		public bool HasUV( int uvChannel )
		{
			return ( m_currentDataCollector.PortCategory == MasterNodePortCategory.Fragment ) ? m_availableFragData.ContainsKey( TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] ) : m_availableVertData.ContainsKey( TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] );
		}

		public string GetUVName( int uvChannel )
		{
			return ( m_currentDataCollector.PortCategory == MasterNodePortCategory.Fragment ) ? m_availableFragData[ TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] ].VarName : m_availableVertData[ TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] ].VarName;
		}

		public string GetTextureCoord( int uvChannel , string propertyName, int uniqueId, PrecisionType precisionType )
		{
			bool isVertex = ( m_currentDataCollector.PortCategory == MasterNodePortCategory.Vertex || m_currentDataCollector.PortCategory == MasterNodePortCategory.Tessellation );
			string uvChannelName = string.Empty;
			string propertyHelperVar = propertyName + "_ST";
			m_currentDataCollector.AddToUniforms( uniqueId, "float4", propertyHelperVar );
			string uvName = string.Empty;
			if ( m_currentDataCollector.TemplateDataCollectorInstance.HasUV( uvChannel ) )
			{
				uvName = m_currentDataCollector.TemplateDataCollectorInstance.GetUVName( uvChannel );
			}
			else
			{
				uvName = m_currentDataCollector.TemplateDataCollectorInstance.RegisterUV( uvChannel );
			}

			uvChannelName = "uv" + propertyName;
			if ( isVertex )
			{
				string value = string.Format( Constants.TilingOffsetFormat, uvName, propertyHelperVar + ".xy", propertyHelperVar + ".zw" );
				string lodLevel = "0";

				value = "float4( " + value + ", 0 , " + lodLevel + " )";
				m_currentDataCollector.AddLocalVariable( uniqueId, precisionType, WirePortDataType.FLOAT4, uvChannelName, value );
			}
			else
			{
				m_currentDataCollector.AddLocalVariable( uniqueId, precisionType, WirePortDataType.FLOAT2, uvChannelName, string.Format( Constants.TilingOffsetFormat, uvName, propertyHelperVar + ".xy", propertyHelperVar + ".zw" ) );
			}
			return uvChannelName;
		}

		public InterpDataHelper GetUVInfo( int uvChannel )
		{
			return ( m_currentDataCollector.PortCategory == MasterNodePortCategory.Fragment ) ? m_availableFragData[ TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] ] : m_availableVertData[ TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] ];
		}

		public string RegisterUV( int UVChannel )
		{
			if ( m_currentDataCollector.PortCategory == MasterNodePortCategory.Vertex )
			{
				TemplateSemantics semantic = TemplateHelperFunctions.IntToSemantic[ UVChannel ];

				if ( m_vertexDataDict.ContainsKey( semantic ) )
				{
					return m_vertexDataDict[ semantic ].VarName;
				}

				string varName = TemplateHelperFunctions.BaseInterpolatorName + ( ( UVChannel > 0 ) ? UVChannel.ToString() : string.Empty );
				m_availableVertData.Add( TemplateHelperFunctions.IntToUVChannelInfo[ UVChannel ],
				new InterpDataHelper( WirePortDataType.FLOAT4,
				string.Format( TemplateHelperFunctions.TemplateVarFormat,
				m_currentTemplateData.VertexFunctionData.InVarName,
				 varName ) ) );

				m_currentDataCollector.AddToVertexInput(
				string.Format( TemplateHelperFunctions.TexFullSemantic,
				varName,
				semantic ) );
				RegisterOnVertexData( semantic, WirePortDataType.FLOAT2, varName );
				return m_availableVertData[ TemplateHelperFunctions.IntToUVChannelInfo[ UVChannel ] ].VarName;
			}
			else
			{
				//search if the correct vertex data is set ... 
				TemplateInfoOnSematics info = TemplateHelperFunctions.IntToInfo[ UVChannel ];
				TemplateSemantics vertexSemantics = TemplateSemantics.NONE;
				foreach ( KeyValuePair<TemplateSemantics, TemplateVertexData> kvp in m_vertexDataDict )
				{
					if ( kvp.Value.DataInfo == info )
					{
						vertexSemantics = kvp.Key;
						break;
					}
				}

				// if not, add vertex data and create interpolator 
				if ( vertexSemantics == TemplateSemantics.NONE )
				{
					vertexSemantics = TemplateHelperFunctions.IntToSemantic[ UVChannel ];

					if ( !m_vertexDataDict.ContainsKey( vertexSemantics ) )
					{
						string varName = TemplateHelperFunctions.BaseInterpolatorName + ( ( UVChannel > 0 ) ? UVChannel.ToString() : string.Empty );
						m_availableVertData.Add( TemplateHelperFunctions.IntToUVChannelInfo[ UVChannel ],
						new InterpDataHelper( WirePortDataType.FLOAT4,
						string.Format( TemplateHelperFunctions.TemplateVarFormat,
						m_currentTemplateData.VertexFunctionData.InVarName,
						 varName ) ) );

						m_currentDataCollector.AddToVertexInput(
						string.Format( TemplateHelperFunctions.TexFullSemantic,
						varName,
						vertexSemantics ) );
						RegisterOnVertexData( vertexSemantics, WirePortDataType.FLOAT2, varName );
					}
				}

				// either way create interpolator
				TemplateVertexData availableInterp = RequestNewInterpolator( WirePortDataType.FLOAT2, false );
				if ( availableInterp != null )
				{
					string interpVarName = m_currentTemplateData.VertexFunctionData.OutVarName + "." + availableInterp.VarNameWithSwizzle;
					InterpDataHelper vertInfo = m_availableVertData[ TemplateHelperFunctions.IntToUVChannelInfo[ UVChannel ] ];
					string interpDecl = string.Format( TemplateHelperFunctions.TemplateVariableDecl, interpVarName, TemplateHelperFunctions.AutoSwizzleData( vertInfo.VarName, vertInfo.VarType, WirePortDataType.FLOAT2 ) );
					m_currentDataCollector.AddToVertexInterpolatorsDecl( interpDecl );
					string finalVarName = m_currentTemplateData.FragFunctionData.InVarName + "." + availableInterp.VarNameWithSwizzle;
					m_availableFragData.Add( TemplateHelperFunctions.IntToUVChannelInfo[ UVChannel ], new InterpDataHelper( WirePortDataType.FLOAT2, finalVarName ) );
					return finalVarName;
				}
			}
			return string.Empty;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////
		bool IsSemanticUsedOnInterpolator( TemplateSemantics semantics )
		{
			for ( int i = 0; i < m_interpolatorData.Interpolators.Count; i++ )
			{
				if ( m_interpolatorData.Interpolators[ i ].Semantics == semantics )
				{
					return true;
				}
			}
			return false;
		}

		public bool HasInfo( TemplateInfoOnSematics info, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
			return ( category == MasterNodePortCategory.Fragment ) ? m_availableFragData.ContainsKey( info ) : m_availableVertData.ContainsKey( info );
		}

		public InterpDataHelper GetInfo( TemplateInfoOnSematics info, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
			return ( category == MasterNodePortCategory.Fragment ) ? m_availableFragData[ info ] : m_availableVertData[ info ];
		}

		public string RegisterInfoOnSemantic( TemplateInfoOnSematics info, TemplateSemantics semantic, string name, WirePortDataType dataType, bool requestNewInterpolator )
		{
			return RegisterInfoOnSemantic( m_currentDataCollector.PortCategory, info, semantic, name, dataType, requestNewInterpolator );
		}
		// This should only be used to semantics outside the text coord set
		public string RegisterInfoOnSemantic( MasterNodePortCategory portCategory, TemplateInfoOnSematics info, TemplateSemantics semantic, string name, WirePortDataType dataType, bool requestNewInterpolator )
		{
			if ( portCategory == MasterNodePortCategory.Vertex )
			{
				if ( m_vertexDataDict.ContainsKey( semantic ) )
				{
					return m_vertexDataDict[ semantic ].VarName;
				}

				m_availableVertData.Add( info,
				new InterpDataHelper( WirePortDataType.FLOAT4,
				string.Format( TemplateHelperFunctions.TemplateVarFormat,
				m_currentTemplateData.VertexFunctionData.InVarName,
				name ) ) );

				m_currentDataCollector.AddToVertexInput(
				string.Format( TemplateHelperFunctions.TexFullSemantic,
				name,
				semantic ) );
				RegisterOnVertexData( semantic, dataType, name );
				return m_availableVertData[ info ].VarName;
			}
			else
			{
				//search if the correct vertex data is set ... 
				TemplateSemantics vertexSemantics = TemplateSemantics.NONE;
				foreach ( KeyValuePair<TemplateSemantics, TemplateVertexData> kvp in m_vertexDataDict )
				{
					if ( kvp.Value.DataInfo == info )
					{
						vertexSemantics = kvp.Key;
						break;
					}
				}

				// if not, add vertex data and create interpolator 
				if ( vertexSemantics == TemplateSemantics.NONE )
				{
					vertexSemantics = semantic;

					if ( !m_vertexDataDict.ContainsKey( vertexSemantics ) )
					{
						m_availableVertData.Add( info,
						new InterpDataHelper( WirePortDataType.FLOAT4,
						string.Format( TemplateHelperFunctions.TemplateVarFormat,
						m_currentTemplateData.VertexFunctionData.InVarName,
						name ) ) );

						m_currentDataCollector.AddToVertexInput(
						string.Format( TemplateHelperFunctions.TexFullSemantic,
						name,
						vertexSemantics ) );
						RegisterOnVertexData( vertexSemantics, dataType, name );
					}
				}

				// either way create interpolator

				TemplateVertexData availableInterp = null;
				if ( requestNewInterpolator || IsSemanticUsedOnInterpolator( semantic ) )
				{
					availableInterp = RequestNewInterpolator( dataType, false );
				}
				else
				{
					availableInterp = RegisterOnInterpolator( semantic, dataType );
				}

				if ( availableInterp != null )
				{
					string interpVarName = m_currentTemplateData.VertexFunctionData.OutVarName + "." + availableInterp.VarNameWithSwizzle;
					string interpDecl = string.Format( TemplateHelperFunctions.TemplateVariableDecl, interpVarName, TemplateHelperFunctions.AutoSwizzleData( m_availableVertData[ info ].VarName, m_availableVertData[ info ].VarType, dataType ) );
					m_currentDataCollector.AddToVertexInterpolatorsDecl( interpDecl );
					string finalVarName = m_currentTemplateData.FragFunctionData.InVarName + "." + availableInterp.VarNameWithSwizzle;
					m_availableFragData.Add( info, new InterpDataHelper( dataType, finalVarName ) );
					return finalVarName;
				}
			}
			return string.Empty;
		}

		TemplateVertexData RegisterOnInterpolator( TemplateSemantics semantics, WirePortDataType dataType )
		{
			TemplateVertexData data = new TemplateVertexData( semantics, dataType, TemplateHelperFunctions.SemanticsDefaultName[ semantics ] );
			m_interpolatorData.Interpolators.Add( data );
			string interpolator = string.Format( TemplateHelperFunctions.InterpFullSemantic, UIUtils.WirePortToCgType( dataType ), data.VarName, data.Semantics );
			m_currentDataCollector.AddToInterpolators( interpolator );
			return data;
		}

		public void RegisterOnVertexData( TemplateSemantics semantics, WirePortDataType dataType, string varName )
		{
			m_vertexDataDict.Add( semantics, new TemplateVertexData( semantics, dataType, varName ) );
		}

		public TemplateVertexData RequestNewInterpolator( WirePortDataType dataType, bool isColor )
		{
			for ( int i = 0; i < m_interpolatorData.AvailableInterpolators.Count; i++ )
			{
				if ( !m_interpolatorData.AvailableInterpolators[ i ].IsFull )
				{
					TemplateVertexData data = m_interpolatorData.AvailableInterpolators[ i ].RequestChannels( dataType, isColor );
					if ( data != null )
					{
						if ( m_interpolatorData.AvailableInterpolators[ i ].Usage == 1 )
						{
							// First time using this interpolator, so we need to register it
							string interpolator = string.Format( TemplateHelperFunctions.TexFullSemantic,
																data.VarName, data.Semantics );
							m_currentDataCollector.AddToInterpolators( interpolator );
						}
						return data;
					}
				}
			}
			return null;
		}

		// Unused channels in interpolators must be set to something so the compiler doesn't generate warnings
		public List<string> GetInterpUnusedChannels()
		{
			List<string> resetInstrucctions = new List<string>();



			for ( int i = 0; i < m_interpolatorData.AvailableInterpolators.Count; i++ )
			{
				if ( m_interpolatorData.AvailableInterpolators[ i ].Usage > 0 && !m_interpolatorData.AvailableInterpolators[ i ].IsFull )
				{
					string channels = string.Empty;
					bool[] availableChannels = m_interpolatorData.AvailableInterpolators[ i ].AvailableChannels;
					for ( int j = 0; j < availableChannels.Length; j++ )
					{
						if ( availableChannels[ j ] )
						{
							channels += TemplateHelperFunctions.VectorSwizzle[ j ];
						}
					}

					resetInstrucctions.Add( string.Format( "{0}.{1}.{2} = 0;", m_currentTemplateData.VertexFunctionData.OutVarName, m_interpolatorData.AvailableInterpolators[ i ].Name, channels ) );
				}
			}

			if ( resetInstrucctions.Count > 0 )
			{
				resetInstrucctions.Insert( 0, "\n//setting value to unused interpolator channels and avoid initialization warnings" );
			}

			return resetInstrucctions;
		}

		public string GetVertexPosition( WirePortDataType type, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			if ( HasInfo( TemplateInfoOnSematics.POSITION, useMasterNodeCategory, customCategory ) )
			{
				InterpDataHelper info = GetInfo( TemplateInfoOnSematics.POSITION, useMasterNodeCategory, customCategory );
				return TemplateHelperFunctions.AutoSwizzleData( info.VarName, info.VarType, type );
			}
			else
			{
				MasterNodePortCategory portCategory = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
				string name = "ase_vertex_pos";
				return RegisterInfoOnSemantic( portCategory, TemplateInfoOnSematics.POSITION, TemplateSemantics.POSITION, name, type, true );
			}
		}

		public string GetVertexColor()
		{
			if ( HasInfo( TemplateInfoOnSematics.COLOR ) )
			{
				return GetInfo( TemplateInfoOnSematics.COLOR ).VarName;
			}
			else
			{
				string name = "ase_color";
				return RegisterInfoOnSemantic( TemplateInfoOnSematics.COLOR, TemplateSemantics.COLOR, name, WirePortDataType.FLOAT4, false );
			}
		}


		public string GetVertexNormal( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			if ( HasInfo( TemplateInfoOnSematics.NORMAL, useMasterNodeCategory, customCategory ) )
			{
				InterpDataHelper info = GetInfo( TemplateInfoOnSematics.NORMAL, useMasterNodeCategory, customCategory );
				return TemplateHelperFunctions.AutoSwizzleData( info.VarName, info.VarType, WirePortDataType.FLOAT3 );
			}
			else
			{
				MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
				string name = "ase_normal";
				return RegisterInfoOnSemantic( category, TemplateInfoOnSematics.NORMAL, TemplateSemantics.NORMAL, name, WirePortDataType.FLOAT3, false );
			}
		}

		public string GetWorldNormal( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "worldNormal";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;
			string vertexTangent = GetVertexNormal( false, MasterNodePortCategory.Vertex );
			string worldNormalValue = string.Format( "UnityObjectToWorldNormal({0})", vertexTangent );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldNormalValue, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetWorldNormal( string normal )
		{
			string tanToWorld0 = string.Empty;
			string tanToWorld1 = string.Empty;
			string tanToWorld2 = string.Empty;

			GetWorldTangentTf( out tanToWorld0, out tanToWorld1, out tanToWorld2 );
			return string.Format( "float3(dot({1},{0}), dot({2},{0}), dot({3},{0}))", normal, tanToWorld0, tanToWorld1, tanToWorld2 );
		}

		public string GetVertexTangent( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			if ( HasInfo( TemplateInfoOnSematics.TANGENT, useMasterNodeCategory, customCategory ) )
			{
				InterpDataHelper info = GetInfo( TemplateInfoOnSematics.TANGENT, useMasterNodeCategory, customCategory );
				return info.VarName;
			}
			else
			{
				MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
				string name = "ase_tangent";
				return RegisterInfoOnSemantic( category, TemplateInfoOnSematics.TANGENT, TemplateSemantics.TANGENT, name, WirePortDataType.FLOAT4, false );
			}
		}

		public string GetWorldTangent( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "worldTangent";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;
			string vertexTangent = GetVertexTangent( false, MasterNodePortCategory.Vertex );
			string worldTangentValue = string.Format( "UnityObjectToWorldDir({0})", vertexTangent );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldTangentValue, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetTangentSign( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "tangentSign";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string tangentValue = GetVertexTangent( false, MasterNodePortCategory.Vertex );
			string tangentSignValue = string.Format( "{0}.w * unity_WorldTransformParams.w", tangentValue );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT, PrecisionType.Float, tangentSignValue, useMasterNodeCategory, customCategory );
			return varName;
		}


		public string GetWorldBinormal( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "worldBinormal";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string worldNormal = GetWorldNormal( false, MasterNodePortCategory.Vertex );
			string worldtangent = GetWorldTangent( false, MasterNodePortCategory.Vertex );
			string tangentSign = GetTangentSign( false, MasterNodePortCategory.Vertex );
			string worldBinormal = string.Format( "cross( {0}, {1} ) * {2}", worldNormal, worldtangent, tangentSign );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldBinormal, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetWorldReflection( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = UIUtils.GetInputValueFromType( AvailableSurfaceInputs.WORLD_REFL );
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string worldNormal = GetWorldNormal( false, MasterNodePortCategory.Vertex );
			string worldViewDir = GetViewDir( false, MasterNodePortCategory.Vertex );
			string worldRefl = string.Format( "reflect(-{0}, {1})", worldViewDir, worldNormal );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldRefl, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetWorldReflection( string normal )
		{
			string tanToWorld0 = string.Empty;
			string tanToWorld1 = string.Empty;
			string tanToWorld2 = string.Empty;

			GetWorldTangentTf( out tanToWorld0, out tanToWorld1, out tanToWorld2 );
			string worldRefl = GetNormalizedViewDir();

			return string.Format( "reflect( -{0}, float3( dot( {2}, {1} ), dot( {3}, {1} ), dot( {4}, {1} ) ) )", worldRefl, normal, tanToWorld0, tanToWorld1, tanToWorld2 );
		}

		public string GetWorldPos( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = UIUtils.GetInputValueFromType( AvailableSurfaceInputs.WORLD_POS );
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string vertexPos = m_availableVertData[ TemplateInfoOnSematics.POSITION ].VarName;
			string worldPosConversion = string.Format( "mul(unity_ObjectToWorld, {0}).xyz", vertexPos );

			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldPosConversion, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetClipPos( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "clipPos";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string vertexPos = m_availableVertData[ TemplateInfoOnSematics.POSITION ].VarName;
			string clipSpaceConversion = string.Format( "UnityObjectToClipPos({0})", vertexPos );

			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT4, PrecisionType.Float, clipSpaceConversion, useMasterNodeCategory, customCategory );

			return varName;
		}

		public string GetScreenPos( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = UIUtils.GetInputValueFromType( AvailableSurfaceInputs.SCREEN_POS );
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string clipSpacePos = GetClipPos( false, MasterNodePortCategory.Vertex );
			string screenPosConversion = string.Format( "ComputeScreenPos({0})", clipSpacePos );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT4, PrecisionType.Float, screenPosConversion, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetViewDir( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = UIUtils.GetInputValueFromType( AvailableSurfaceInputs.VIEW_DIR );
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string worldPos = GetWorldPos( false, MasterNodePortCategory.Vertex );
			string viewDir = string.Format( "UnityWorldSpaceViewDir({0})", worldPos );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, viewDir, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetNormalizedViewDir( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "normViewDir";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string viewDir = GetViewDir( false, MasterNodePortCategory.Vertex );
			string normViewDir = string.Format( "normalize({0})", viewDir );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, normViewDir, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetTangenViewDir( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "tanViewDir";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string tanToWorld0 = string.Empty;
			string tanToWorld1 = string.Empty;
			string tanToWorld2 = string.Empty;

			GetWorldTangentTf( out tanToWorld0, out tanToWorld1, out tanToWorld2, false, MasterNodePortCategory.Vertex );
			string viewDir = GetNormalizedViewDir( false, MasterNodePortCategory.Vertex );
			string tanViewDir = string.Format( " {0} * {3}.x + {1} * {3}.y  + {2} * {3}.z", tanToWorld0, tanToWorld1, tanToWorld2, viewDir );

			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, tanViewDir, useMasterNodeCategory, customCategory );
			return varName;
		}

		public void GetWorldTangentTf( out string tanToWorld0, out string tanToWorld1, out string tanToWorld2, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			tanToWorld0 = "tanToWorld0";
			tanToWorld1 = "tanToWorld1";
			tanToWorld2 = "tanToWorld2";

			if ( HasCustomInterpolatedData( tanToWorld0, useMasterNodeCategory, customCategory ) ||
				 HasCustomInterpolatedData( tanToWorld1, useMasterNodeCategory, customCategory ) ||
				 HasCustomInterpolatedData( tanToWorld2, useMasterNodeCategory, customCategory ) )
				return;

			string worldTangent = GetWorldTangent( false, MasterNodePortCategory.Vertex );
			string worldNormal = GetWorldNormal( false, MasterNodePortCategory.Vertex );
			string worldBinormal = GetWorldBinormal( false, MasterNodePortCategory.Vertex );

			string tanToWorldVar0 = string.Format( "float3( {0}.x, {1}.x, {2}.x )", worldTangent, worldBinormal, worldNormal );
			string tanToWorldVar1 = string.Format( "float3( {0}.y, {1}.y, {2}.y )", worldTangent, worldBinormal, worldNormal );
			string tanToWorldVar2 = string.Format( "float3( {0}.z, {1}.z, {2}.z )", worldTangent, worldBinormal, worldNormal );

			RegisterCustomInterpolatedData( tanToWorld0, WirePortDataType.FLOAT3, PrecisionType.Float, tanToWorldVar0, useMasterNodeCategory, customCategory );
			RegisterCustomInterpolatedData( tanToWorld1, WirePortDataType.FLOAT3, PrecisionType.Float, tanToWorldVar1, useMasterNodeCategory, customCategory );
			RegisterCustomInterpolatedData( tanToWorld2, WirePortDataType.FLOAT3, PrecisionType.Float, tanToWorldVar2, useMasterNodeCategory, customCategory );
		}

		public string GetWorldToTangentMatrix( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string worldTangent = GetWorldTangent();
			string worldNormal = GetWorldNormal();
			string worldBinormal = GetWorldBinormal();

			string varName = "worldToTanMat";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;
			string worldTanMat = string.Format( "float3x3({0},{1},{2})", worldTangent, worldBinormal, worldNormal );

			m_currentDataCollector.AddLocalVariable( -1, PrecisionType.Float, WirePortDataType.FLOAT3x3, varName, worldTanMat );
			return varName;
		}

		public string GetObjectToViewPos( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "objectToViewPos";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;
			string vertexPos = GetVertexPosition( WirePortDataType.FLOAT3, false, MasterNodePortCategory.Vertex );
			string objectToViewPosValue = string.Format( "UnityObjectToViewPos({0})", vertexPos );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, objectToViewPosValue, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetEyeDepth( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "eyeDepth";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;
			string objectToView = GetObjectToViewPos( false, MasterNodePortCategory.Vertex );
			string eyeDepthValue = string.Format( "-{0}.z", objectToView );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT, PrecisionType.Float, eyeDepthValue, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetObjectSpaceLightDir( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "objectSpaceLightDir";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			m_currentDataCollector.AddToIncludes( -1, Constants.UnityLightingLib );
			m_currentDataCollector.AddToIncludes( -1, Constants.UnityAutoLightLib );

			string vertexPos = GetVertexPosition( WirePortDataType.FLOAT4, false, MasterNodePortCategory.Vertex );
			string objectSpaceLightDir = string.Format( "ObjSpaceLightDir({0})", vertexPos );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, objectSpaceLightDir, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetWorldSpaceLightDir( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "worldSpaceLightDir";
			if ( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			m_currentDataCollector.AddToIncludes( -1, Constants.UnityLightingLib );
			m_currentDataCollector.AddToIncludes( -1, Constants.UnityAutoLightLib );

			string vertexPos = GetWorldPos( false, MasterNodePortCategory.Vertex );
			string worldSpaceLightDir = string.Format( "UnityWorldSpaceLightDir({0})", vertexPos );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldSpaceLightDir, useMasterNodeCategory, customCategory );
			return varName;
		}

		public void RegisterCustomInterpolatedData( string name, WirePortDataType dataType, PrecisionType precision, string vertexInstruction, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;

			if ( !m_customInterpolatedData.ContainsKey( name ) )
			{
				m_customInterpolatedData.Add( name, new TemplateCustomData( name, dataType ) );
			}

			if ( !m_customInterpolatedData[ name ].IsVertex )
			{
				m_customInterpolatedData[ name ].IsVertex = true;
				m_currentDataCollector.AddToVertexLocalVariables( -1, precision, dataType, name, vertexInstruction );
			}
			if ( category == MasterNodePortCategory.Fragment )
			{
				if ( !m_customInterpolatedData[ name ].IsFragment )
				{
					m_customInterpolatedData[ name ].IsFragment = true;
					TemplateVertexData interpData = RequestNewInterpolator( dataType, false );
					if ( interpData == null )
					{
						Debug.LogErrorFormat( "Could not assign interpolator of type {0} to variable {1}", dataType, name );
						return;
					}
					m_currentDataCollector.AddToVertexLocalVariables( -1, m_currentTemplateData.VertexFunctionData.OutVarName + "." + interpData.VarNameWithSwizzle, name );
					m_currentDataCollector.AddToLocalVariables( -1, precision, dataType, name, m_currentTemplateData.FragFunctionData.InVarName + "." + interpData.VarNameWithSwizzle );
				}
			}
		}

		public bool HasCustomInterpolatedData( string name, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			if ( m_customInterpolatedData.ContainsKey( name ) )
			{
				MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
				return ( category == MasterNodePortCategory.Fragment ) ? m_customInterpolatedData[ name ].IsFragment : m_customInterpolatedData[ name ].IsVertex;
			}
			return false;
		}

		public bool HasFragmentInputParams
		{
			get
			{
				if ( m_fragmentInputParams != null )
					return m_fragmentInputParams.Count > 0;

				return false;
			}
		}

		public string FragInputParamsStr
		{
			get
			{
				string value = string.Empty;
				if ( m_fragmentInputParams != null && m_fragmentInputParams.Count > 0 )
				{
					int count = m_fragmentInputParams.Count;
					if ( count > 0 )
					{
						value = ", ";
						foreach ( KeyValuePair<TemplateSemantics, TemplateInputParameters> kvp in m_fragmentInputParams )
						{
							value += kvp.Value.Declaration;

							if ( --count > 0 )
							{
								value += " , ";
							}
						}
					}
				} 
				return value;
			}
		}

		public string VertexInputParamsStr
		{
			get
			{
				string value = string.Empty;
				if ( m_vertexInputParams != null && m_fragmentInputParams.Count > 0 )
				{
					int count = m_vertexInputParams.Count;
					if ( count > 0 )
					{
						value = ", ";
						foreach ( KeyValuePair<TemplateSemantics, TemplateInputParameters> kvp in m_vertexInputParams )
						{
							value += kvp.Value.Declaration;

							if ( --count > 0 )
							{
								value += " , ";
							}
						}
					}
				}
				return value;
			}
		}

		public Dictionary<TemplateSemantics, TemplateInputParameters> FragInputParameters { get { return m_fragmentInputParams; } }
		
		public bool HasVertexInputParams
		{
			get
			{
				if ( m_vertexInputParams != null )
					return m_vertexInputParams.Count > 0;

				return false;
			}
		}

		public Dictionary<TemplateSemantics, TemplateInputParameters> VertexInputParameters { get { return m_vertexInputParams; } }
		public TemplateData CurrentTemplateData { get { return m_currentTemplateData; } }
		public void Destroy()
		{
			m_currentTemplateData = null;

			m_currentDataCollector = null;

			if ( m_vertexInputParams != null )
			{
				m_vertexInputParams.Clear();
				m_vertexInputParams = null;
			}

			if ( m_fragmentInputParams != null )
			{
				m_fragmentInputParams.Clear();
				m_fragmentInputParams = null;
			}

			if ( m_vertexDataDict != null )
			{
				m_vertexDataDict.Clear();
				m_vertexDataDict = null;
			}

			if ( m_interpolatorData != null )
			{
				m_interpolatorData.Destroy();
				m_interpolatorData = null;
			}

			if ( m_availableFragData != null )
			{
				m_availableFragData.Clear();
				m_availableFragData = null;
			}

			if ( m_availableVertData != null )
			{
				m_availableVertData.Clear();
				m_availableVertData = null;
			}

			if ( m_customInterpolatedData != null )
			{
				m_customInterpolatedData.Clear();
				m_customInterpolatedData = null;
			}
		}
	}
}
