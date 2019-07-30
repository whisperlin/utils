using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Function Input", "Functions", "Function Input", NodeAvailabilityFlags = ( int ) NodeAvailability.ShaderFunction )]
	public sealed class FunctionInput : ParentNode
	{
		private const string InputTypeStr = "Input Type";
		private readonly string[] m_inputValueTypes ={  "Int",
														"Float",
														"Vector2",
														"Vector3",
														"Vector4",
														"Color",
														"Matrix 3x3",
														"Matrix 4x4",
														"Sampler 1D",
														"Sampler 2D",
														"Sampler 3D",
														"Sampler Cube"};

		[SerializeField]
		private int m_selectedInputTypeInt = 1;

		private WirePortDataType m_selectedInputType = WirePortDataType.FLOAT;

		[SerializeField]
		private string m_inputName = "Input";

		[SerializeField]
		private bool m_autoCast = false;

		[SerializeField]
		private int m_orderIndex = -1;

		public delegate string PortGeneration(ref MasterNodeDataCollector dataCollector, int index, ParentGraph graph);
		public PortGeneration OnPortGeneration = null;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			m_inputPorts[ 0 ].Visible = false;
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_autoWrapProperties = true;
			m_textLabelWidth = 100;
			SetTitleText( m_inputName );
			UpdatePorts();
			SetAdditonalTitleText( "( " + m_inputValueTypes[ m_selectedInputTypeInt ] + " )" );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			UIUtils.RegisterFunctionInputNode( this );
		}


		public override void Destroy()
		{
			base.Destroy();
			OnPortGeneration = null;
			UIUtils.UnregisterFunctionInputNode( this );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			EditorGUI.BeginChangeCheck();
			m_inputName = EditorGUILayoutTextField( "Name", m_inputName );
			if ( EditorGUI.EndChangeCheck() )
			{
				SetTitleText( m_inputName );
				UIUtils.UpdateFunctionInputData( UniqueId, m_inputName );
			}
			EditorGUI.BeginChangeCheck();
			m_selectedInputTypeInt = EditorGUILayoutPopup( InputTypeStr, m_selectedInputTypeInt, m_inputValueTypes );
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
				SetAdditonalTitleText( "( "+ m_inputValueTypes[ m_selectedInputTypeInt ]+" )" );
			}

			m_autoCast = EditorGUILayoutToggle( "Auto Cast", m_autoCast );

			EditorGUILayout.Separator();
			if ( m_inputPorts[ 0 ].ValidInternalData )
			{
				m_inputPorts[ 0 ].ShowInternalData( this, true, "Default Value" );
			}


			EditorGUILayout.EndVertical();
		}

		void UpdatePorts()
		{
			switch ( m_selectedInputTypeInt )
			{
				case 0: m_selectedInputType = WirePortDataType.INT; break;
				default:
				case 1: m_selectedInputType = WirePortDataType.FLOAT; break;
				case 2: m_selectedInputType = WirePortDataType.FLOAT2; break;
				case 3: m_selectedInputType = WirePortDataType.FLOAT3; break;
				case 4: m_selectedInputType = WirePortDataType.FLOAT4; break;
				case 5: m_selectedInputType = WirePortDataType.COLOR; break;
				case 6: m_selectedInputType = WirePortDataType.FLOAT3x3; break;
				case 7: m_selectedInputType = WirePortDataType.FLOAT4x4; break;
				case 8: m_selectedInputType = WirePortDataType.SAMPLER1D; break;
				case 9: m_selectedInputType = WirePortDataType.SAMPLER2D; break;
				case 10: m_selectedInputType = WirePortDataType.SAMPLER3D; break;
				case 11: m_selectedInputType = WirePortDataType.SAMPLERCUBE; break;
			}

			ChangeInputType( m_selectedInputType, false );
			
			//This node doesn't have any restrictions but changing types should be restricted to prevent invalid connections
			m_outputPorts[ 0 ].ChangeTypeWithRestrictions( m_selectedInputType, PortCreateRestriction( m_selectedInputType ) );
			m_sizeIsDirty = true;
		}

		public int PortCreateRestriction( WirePortDataType dataType )
		{
			int restrictions = 0;
			WirePortDataType[] types = null;
			switch ( dataType )
			{
				case WirePortDataType.OBJECT:
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				{
					types = new WirePortDataType[] { WirePortDataType.FLOAT, WirePortDataType.FLOAT2, WirePortDataType.FLOAT3, WirePortDataType.FLOAT4, WirePortDataType.COLOR, WirePortDataType.INT, WirePortDataType.OBJECT };
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					types = new WirePortDataType[] { WirePortDataType.FLOAT3x3, WirePortDataType.FLOAT4x4, WirePortDataType.OBJECT };
				}
				break;
				case WirePortDataType.SAMPLER1D:
				case WirePortDataType.SAMPLER2D:
				case WirePortDataType.SAMPLER3D:
				case WirePortDataType.SAMPLERCUBE:
				{
					types = new WirePortDataType[] { WirePortDataType.SAMPLER1D, WirePortDataType.SAMPLER2D, WirePortDataType.SAMPLER3D, WirePortDataType.SAMPLERCUBE, WirePortDataType.OBJECT };
				}
				break;
				default:
				break;
			}

			if ( types != null )
			{
				for ( int i = 0; i < types.Length; i++ )
				{
					restrictions = restrictions | (int)types[ i ];
				}
			}

			return restrictions;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( OnPortGeneration != null )
				return OnPortGeneration( ref dataCollector, m_orderIndex, ContainerGraph.ParentWindow.CustomGraph );
			else
				return m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_inputName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedInputTypeInt );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndex );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoCast );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_inputName = GetCurrentParam( ref nodeParams );
			m_selectedInputTypeInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_autoCast = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			SetTitleText( m_inputName );
			UpdatePorts();
			SetAdditonalTitleText( "( " + m_inputValueTypes[ m_selectedInputTypeInt ] + " )" );
		}

		public WirePortDataType SelectedInputType
		{
			get { return m_selectedInputType; }
		}

		public string InputName
		{
			get { return m_inputName; }
		}

		public int OrderIndex
		{
			get { return m_orderIndex; }
			set { m_orderIndex = value; }
		}

		public bool AutoCast
		{
			get { return m_autoCast; }
			set { m_autoCast = value; }
		}
	}
}
