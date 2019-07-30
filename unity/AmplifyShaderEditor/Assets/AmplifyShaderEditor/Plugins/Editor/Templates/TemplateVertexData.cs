// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateVertexData
	{
		[SerializeField]
		private TemplateSemantics m_semantics = TemplateSemantics.NONE;
		[SerializeField]
		private WirePortDataType m_dataType = WirePortDataType.OBJECT;
		[SerializeField]
		private string m_varName = string.Empty;
		[SerializeField]
		private TemplateInfoOnSematics m_dataInfo = TemplateInfoOnSematics.NONE;
		[SerializeField]
		private string m_dataSwizzle = string.Empty;
		[SerializeField]
		private bool m_available = false;
		[SerializeField]
		private string m_varNameWithSwizzle = string.Empty;

		public TemplateVertexData( TemplateSemantics semantics, WirePortDataType dataType, string varName )
		{
			m_semantics = semantics;
			m_dataType = dataType;
			m_varName = varName;
			m_varNameWithSwizzle = varName;
		}

		public TemplateVertexData( TemplateSemantics semantics, WirePortDataType dataType, string varName, string dataSwizzle )
		{
			m_semantics = semantics;
			m_dataType = dataType;
			m_varName = varName;
			m_dataSwizzle = dataSwizzle;
			m_varNameWithSwizzle = varName + dataSwizzle;
		}

		public TemplateVertexData( TemplateVertexData other )
		{
			m_semantics = other.m_semantics;
			m_dataType = other.m_dataType;
			m_varName = other.m_varName;
			m_dataInfo = other.m_dataInfo;
			m_dataSwizzle = other.m_dataSwizzle;
			m_available = other.m_available;
			m_varNameWithSwizzle = other.m_varNameWithSwizzle;
		}

		public TemplateSemantics Semantics { get { return m_semantics; } }
		public WirePortDataType DataType { get { return m_dataType; } }
		public string VarName { get { return m_varName; } set { m_varName = value; m_varNameWithSwizzle = value + m_dataSwizzle; } }
		public string DataSwizzle { get { return m_dataSwizzle; } set { m_dataSwizzle = value; m_varNameWithSwizzle = m_varName + value; } }
		public TemplateInfoOnSematics DataInfo { get { return m_dataInfo; } set { m_dataInfo = value; } }
		public bool Available { get { return m_available; } set { m_available = value; } }
		public string VarNameWithSwizzle { get { return m_varNameWithSwizzle; } }
		public WirePortDataType SwizzleType
		{
			get
			{
				if ( string.IsNullOrEmpty( m_dataSwizzle ) )
					return m_dataType;

				WirePortDataType newType = m_dataType;
				switch ( m_dataSwizzle.Length )
				{
					case 2: newType = WirePortDataType.FLOAT;break;
					case 3: newType = WirePortDataType.FLOAT2; break;
					case 4: newType = WirePortDataType.FLOAT3; break;
					case 5: newType = WirePortDataType.FLOAT4; break;
				}

				return newType;
			}
		}

	}
}
