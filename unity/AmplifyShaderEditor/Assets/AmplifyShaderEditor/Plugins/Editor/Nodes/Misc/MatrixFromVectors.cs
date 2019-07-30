using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Matrix From Vectors", "Matrix Operators", "Matrix From Vectors" )]
	public sealed class MatrixFromVectors : ParentNode
	{
		[SerializeField]
		private WirePortDataType m_selectedOutputType = WirePortDataType.FLOAT3x3;

		[SerializeField]
		private int m_selectedOutputTypeInt = 0;

		[SerializeField]
		private Vector3[] m_defaultValuesV3 = { Vector3.zero, Vector3.zero, Vector3.zero };

		[SerializeField]
		private Vector4[] m_defaultValuesV4 = { Vector4.zero, Vector4.zero, Vector4.zero, Vector4.zero };

		private string[] m_defaultValuesStr = { "[0]", "[1]", "[2]", "[3]" };

		private readonly string[] _outputValueTypes ={  "Matrix3X3",
														"Matrix4X4"};

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT4, false, "[0]" );
			AddInputPort( WirePortDataType.FLOAT4, false, "[1]" );
			AddInputPort( WirePortDataType.FLOAT4, false, "[2]" );
			AddInputPort( WirePortDataType.FLOAT4, false, "[3]" );
			AddOutputPort( m_selectedOutputType, Constants.EmptyPortValue );
			m_textLabelWidth = 90;
			m_autoWrapProperties = true;
			UpdatePorts();
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();
			if( PaddingTitleLeft == 0 )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}


		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );
			m_upperLeftWidget.OnNodeLayout( m_globalPosition, drawInfo );
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );
			m_upperLeftWidget.DrawGUIControls( drawInfo );
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );
			if( !m_isVisible )
				return;
			m_upperLeftWidget.OnNodeRepaint( ContainerGraph.LodLevel );
		}


		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			EditorGUI.BeginChangeCheck();
			m_selectedOutputTypeInt = m_upperLeftWidget.DrawWidget( this, m_selectedOutputTypeInt, _outputValueTypes );
			if( EditorGUI.EndChangeCheck() )
			{
				switch( m_selectedOutputTypeInt )
				{
					case 0: m_selectedOutputType = WirePortDataType.FLOAT3x3; break;
					case 1: m_selectedOutputType = WirePortDataType.FLOAT4x4; break;
				}

				UpdatePorts();
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUI.BeginChangeCheck();
			m_selectedOutputTypeInt = EditorGUILayoutPopup( "Output type", m_selectedOutputTypeInt, _outputValueTypes );
			if( EditorGUI.EndChangeCheck() )
			{
				switch( m_selectedOutputTypeInt )
				{
					case 0: m_selectedOutputType = WirePortDataType.FLOAT3x3; break;
					case 1: m_selectedOutputType = WirePortDataType.FLOAT4x4; break;
				}

				UpdatePorts();
			}

			int count = 0;
			switch( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT3x3:
				count = 3;
				for( int i = 0; i < count; i++ )
				{
					if( !m_inputPorts[ i ].IsConnected )
						m_defaultValuesV3[ i ] = EditorGUILayoutVector3Field( m_defaultValuesStr[ i ], m_defaultValuesV3[ i ] );
				}
				break;
				case WirePortDataType.FLOAT4x4:
				count = 4;
				for( int i = 0; i < count; i++ )
				{
					if( !m_inputPorts[ i ].IsConnected )
						m_defaultValuesV4[ i ] = EditorGUILayoutVector4Field( m_defaultValuesStr[ i ], m_defaultValuesV4[ i ] );
				}
				break;
			}
		}

		void UpdatePorts()
		{
			m_sizeIsDirty = true;
			ChangeOutputType( m_selectedOutputType, false );
			switch( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT3x3:
				m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
				m_inputPorts[ 1 ].ChangeType( WirePortDataType.FLOAT3, false );
				m_inputPorts[ 2 ].ChangeType( WirePortDataType.FLOAT3, false );
				m_inputPorts[ 3 ].ChangeType( WirePortDataType.FLOAT3, false );
				m_inputPorts[ 3 ].Visible = false;
				break;
				case WirePortDataType.FLOAT4x4:
				m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
				m_inputPorts[ 1 ].ChangeType( WirePortDataType.FLOAT4, false );
				m_inputPorts[ 2 ].ChangeType( WirePortDataType.FLOAT4, false );
				m_inputPorts[ 3 ].ChangeType( WirePortDataType.FLOAT4, false );
				m_inputPorts[ 3 ].Visible = true;
				break;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			string result = "";
			switch( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT3x3:
				result = "float3x3(" + m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar ) + ", "
				+ m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar ) + ", "
				+ m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar ) + ")";
				break;
				case WirePortDataType.FLOAT4x4:
				result = "float4x4(" + m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, ignoreLocalvar, true ) + ", "
				+ m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, ignoreLocalvar, true ) + ", "
				+ m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, ignoreLocalvar, true ) + ", "
				+ m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, ignoreLocalvar, true ) + ")";
				break;
			}

			return result;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedOutputType = (WirePortDataType)Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
			switch( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT3x3:
				m_selectedOutputTypeInt = 0;
				break;
				case WirePortDataType.FLOAT4x4:
				m_selectedOutputTypeInt = 1;
				break;
			}
			UpdatePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedOutputType );
		}
	}
}
