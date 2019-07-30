// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
    [Serializable]
    [NodeAttributes("Append", "Vector Operators", "Append channels to create a new component")]
    public sealed class DynamicAppendNode : ParentNode
    {
        private const string OutputTypeStr = "Output type";
        private const string OutputFormatStr = "({0}({1}))";

        [SerializeField]
        private WirePortDataType m_selectedOutputType = WirePortDataType.FLOAT4;

        [SerializeField]
        private int m_selectedOutputTypeInt = 2;



        private readonly string[] m_outputValueTypes ={  "Vector2",
                                                        "Vector3",
                                                        "Vector4",
                                                        "Color"};

        private readonly string[] m_channelNamesVector = { "X", "Y", "Z", "W" };
        private readonly string[] m_channelNamesColor = { "R", "G", "B", "A" };

		private Rect m_varRect;
		private bool m_editing;

		protected override void CommonInit(int uniqueId)
        {
            base.CommonInit(uniqueId);
            AddInputPort(WirePortDataType.FLOAT, false, m_channelNamesVector[0]);
            AddInputPort(WirePortDataType.FLOAT, false, m_channelNamesVector[1]);
            AddInputPort(WirePortDataType.FLOAT, false, m_channelNamesVector[2]);
            AddInputPort(WirePortDataType.FLOAT, false, m_channelNamesVector[3]);
            AddOutputPort(m_selectedOutputType, Constants.EmptyPortValue);
            m_textLabelWidth = 90;
            m_autoWrapProperties = true;
            m_useInternalPortData = true;
            m_previewShaderGUID = "d80ac81aabf643848a4eaa76f2f88d65";
        }

        public override void OnInputPortConnected(int portId, int otherNodeId, int otherPortId, bool activateNode = true)
        {
            base.OnInputPortConnected(portId, otherNodeId, otherPortId, activateNode);
            UpdatePorts();
        }

        public override void OnInputPortDisconnected(int portId)
        {
            base.OnInputPortDisconnected(portId);
            UpdatePorts();
        }

        public override void OnConnectedOutputNodeChanges(int portId, int otherNodeId, int otherPortId, string name, WirePortDataType type)
        {
            base.OnConnectedOutputNodeChanges(portId, otherNodeId, otherPortId, name, type);
            UpdatePorts();
        }

        void UpdatePorts()
        {
            m_sizeIsDirty = true;
            ChangeOutputType(m_selectedOutputType, false);
            int availableChannels = UIUtils.GetChannelsAmount(m_selectedOutputType);
            int maxPorts = availableChannels;

            for (int i = 0; i < 4; i++)
            {
                if (availableChannels > 0)
                {
                    m_inputPorts[i].Visible = true;
                    if (m_inputPorts[i].IsConnected)
                    {
                        int requestChannels = m_inputPorts[i].IsConnected ? UIUtils.GetChannelsAmount(m_inputPorts[i].ExternalReferences[0].DataType) : 1;
                        if (availableChannels >= requestChannels)
                        {
                            int resultAvailableChannels = availableChannels - requestChannels;
                            string portName = string.Empty;
                            for (int j = availableChannels; j > resultAvailableChannels; j--)
                            {
                                portName += (m_selectedOutputType == WirePortDataType.COLOR) ? m_channelNamesColor[maxPorts - j] : m_channelNamesVector[maxPorts - j];
                            }

                            m_inputPorts[i].Name = portName;
                            m_inputPorts[i].MatchPortToConnection();

                            availableChannels = resultAvailableChannels;
                        }
                        else
                        {
                            string portName = string.Empty;
                            for (int j = availableChannels; j > 0; j--)
                            {
                                portName += (m_selectedOutputType == WirePortDataType.COLOR) ? m_channelNamesColor[maxPorts - j] : m_channelNamesVector[maxPorts - j];
                            }

                            WirePortDataType type = UIUtils.GetWireTypeForChannelAmount(availableChannels);
                            m_inputPorts[i].ChangeProperties(portName, type, false);
                            availableChannels = 0;
                        }
                    }
                    else
                    {
                        m_inputPorts[i].ChangeProperties((m_selectedOutputType == WirePortDataType.COLOR) ? m_channelNamesColor[maxPorts - availableChannels] : m_channelNamesVector[maxPorts - availableChannels], WirePortDataType.FLOAT, false);
                        availableChannels -= 1;
                    }
                }
                else
                {
                    if (m_inputPorts[i].IsConnected)
                        m_containerGraph.DeleteConnection(true, UniqueId, m_inputPorts[i].PortId, false, true);

                    m_inputPorts[i].Visible = false;
                }
            }
        }

        void SetupPorts()
        {
            switch (m_selectedOutputTypeInt)
            {
                case 0: m_selectedOutputType = WirePortDataType.FLOAT2; break;
                case 1: m_selectedOutputType = WirePortDataType.FLOAT3; break;
                case 2: m_selectedOutputType = WirePortDataType.FLOAT4; break;
                case 3: m_selectedOutputType = WirePortDataType.COLOR; break;
            }

            UpdatePorts();
        }

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			m_varRect = m_globalPosition;
			m_varRect.x = m_varRect.x + ( Constants.NodeButtonDeltaX - 1 ) * drawInfo.InvertedZoom + 1;
			m_varRect.y = m_varRect.y + Constants.NodeButtonDeltaY * drawInfo.InvertedZoom;
			m_varRect.width = Constants.NodeButtonSizeX * drawInfo.InvertedZoom;
			m_varRect.height = Constants.NodeButtonSizeY * drawInfo.InvertedZoom;
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if ( drawInfo.CurrentEventType != EventType.MouseDown )
				return;

			if ( m_varRect.Contains( drawInfo.MousePosition ) )
			{
				m_editing = true;
			}
			else if ( m_editing )
			{
				m_editing = false;
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if ( m_editing )
			{
				EditorGUI.BeginChangeCheck();
				m_selectedOutputTypeInt = EditorGUIPopup( m_varRect, m_selectedOutputTypeInt, m_outputValueTypes, UIUtils.PropertyPopUp );
				if ( EditorGUI.EndChangeCheck() )
				{
					SetupPorts();
					m_editing = false;
				}
			}
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );

			if ( !m_isVisible )
				return;

			if( !m_editing && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD4 )
				GUI.Label( m_varRect, string.Empty, UIUtils.PropertyPopUp );
		}

		public override void DrawProperties()
        {
            base.DrawProperties();
            EditorGUILayout.BeginVertical();

            EditorGUI.BeginChangeCheck();
            m_selectedOutputTypeInt = EditorGUILayoutPopup(OutputTypeStr, m_selectedOutputTypeInt, m_outputValueTypes);
            if (EditorGUI.EndChangeCheck())
            {
                SetupPorts();
            }

            EditorGUILayout.EndVertical();
        }

        public override string GenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar)
        {
            if (m_outputPorts[0].IsLocalValue)
                return m_outputPorts[0].LocalValue;
            string result = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                if (m_inputPorts[i].Visible)
                {
                    result += m_inputPorts[i].GeneratePortInstructions(ref dataCollector);
                    if (i < 3 && m_inputPorts[i + 1].Visible)
                    {
                        result += " , ";
                    }
                }
                else
                {
                    break;
                }
            }

            result = string.Format(OutputFormatStr,
                                    UIUtils.FinalPrecisionWirePortToCgType(m_currentPrecisionType, m_selectedOutputType),
                                    result);

            RegisterLocalVariable(0, result, ref dataCollector, "appendResult" + OutputId);
            return m_outputPorts[0].LocalValue;
        }

        public override void ReadFromString(ref string[] nodeParams)
        {
            base.ReadFromString(ref nodeParams);
            m_selectedOutputType = (WirePortDataType)Enum.Parse(typeof(WirePortDataType), GetCurrentParam(ref nodeParams));
            switch (m_selectedOutputType)
            {
                case WirePortDataType.FLOAT2: m_selectedOutputTypeInt = 0; break;
                case WirePortDataType.FLOAT3: m_selectedOutputTypeInt = 1; break;
                case WirePortDataType.FLOAT4: m_selectedOutputTypeInt = 2; break;
                case WirePortDataType.COLOR: m_selectedOutputTypeInt = 3; break;
            }

            UpdatePorts();
        }

        public override void WriteToString(ref string nodeInfo, ref string connectionsInfo)
        {
            base.WriteToString(ref nodeInfo, ref connectionsInfo);
            IOUtils.AddFieldValueToString(ref nodeInfo, m_selectedOutputType);
        }
    }
}
