// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

//https://www.shadertoy.com/view/XdS3RW
//http://www.deepskycolors.com/archivo/2010/04/21/formulas-for-Photoshop-blending-modes.html
//http://www.pegtop.net/delphi/articles/blendmodes/softlight.htm

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
    public enum BlendOps
    {
        ColorBurn,
        ColorDodge,
        Darken,
        Divide,
        Difference,
        Exclusion,
        SoftLight,
        HardLight,
        HardMix,
        Lighten,
        LinearBurn,
        LinearDodge,
        LinearLight,
        Multiply,
        Overlay,
        PinLight,
        Subtract,
        Screen,
        VividLight
    }
    [Serializable]
    [NodeAttributes( "Blend Operations", "Image Effects", "Common layer blending modes" )]
    public class BlendOpsNode : ParentNode
    {
        private const string BlendOpsModeStr = "Blend Op";
        private const string SaturateResultStr = "Saturate";
        
        [SerializeField]
        private BlendOps m_currentBlendOp = BlendOps.ColorBurn;

        [SerializeField]
        private WirePortDataType m_mainDataType = WirePortDataType.COLOR;

        [SerializeField]
        private bool m_saturate = true;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

        protected override void CommonInit( int uniqueId )
        {
            base.CommonInit( uniqueId );
            AddInputPort( WirePortDataType.COLOR, false, "Source" );
            AddInputPort( WirePortDataType.COLOR, false, "Destiny" );
            AddOutputPort( WirePortDataType.COLOR, Constants.EmptyPortValue );
            m_textLabelWidth = 75;
            m_autoWrapProperties = true;
            SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_currentBlendOp ) );
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

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
        {
            base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
            UpdateConnection( portId );
        }

        public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
        {
            base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
            UpdateConnection( inputPortId );
        }

        public override void OnInputPortDisconnected( int portId )
        {
            base.OnInputPortDisconnected( portId );
            UpdateDisconnection( portId );
        }

        void UpdateConnection( int portId )
        {
            m_inputPorts[portId].MatchPortToConnection();
            int otherPortId = ( portId + 1 ) % 2;
            if ( m_inputPorts[otherPortId].IsConnected )
            {
                m_mainDataType = UIUtils.GetPriority( m_inputPorts[0].DataType ) > UIUtils.GetPriority( m_inputPorts[1].DataType ) ? m_inputPorts[0].DataType : m_inputPorts[1].DataType;
            }
            else
            {
                m_mainDataType = m_inputPorts[portId].DataType;
                m_inputPorts[otherPortId].ChangeType( m_mainDataType, false );
            }
            m_outputPorts[0].ChangeType( m_mainDataType, false );
        }

        void UpdateDisconnection( int portId )
        {
            int otherPortId = ( portId + 1 ) % 2;
            if ( m_inputPorts[otherPortId].IsConnected )
            {
                m_mainDataType = m_inputPorts[otherPortId].DataType;
                m_inputPorts[portId].ChangeType( m_mainDataType, false );
                m_outputPorts[0].ChangeType( m_mainDataType, false );
            }
        }

        public override void DrawProperties()
        {
            base.DrawProperties();
            EditorGUI.BeginChangeCheck();
            m_currentBlendOp = (BlendOps)EditorGUILayoutEnumPopup( BlendOpsModeStr, m_currentBlendOp );
            if ( EditorGUI.EndChangeCheck() )
            {
                SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_currentBlendOp ) );
            }
            m_saturate = EditorGUILayoutToggle( SaturateResultStr, m_saturate );
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
			m_currentBlendOp = (BlendOps)m_upperLeftWidget.DrawWidget( this, m_currentBlendOp );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_currentBlendOp ) );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
        {
            if ( m_outputPorts[0].IsLocalValue )
                return m_outputPorts[0].LocalValue;

            string src = m_inputPorts[0].GeneratePortInstructions( ref dataCollector );
            string dst = m_inputPorts[1].GeneratePortInstructions( ref dataCollector );
            string srcLocalVar = "blendOpSrc" + OutputId;
            string dstLocalVar = "blendOpDest" + OutputId;
            dataCollector.AddLocalVariable( UniqueId, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_inputPorts[0].DataType ) + " " + srcLocalVar, src + ";" );
            dataCollector.AddLocalVariable( UniqueId, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_inputPorts[1].DataType ) + " " + dstLocalVar, dst + ";" );

            string result = string.Empty;
            switch ( m_currentBlendOp )
            {
                case BlendOps.ColorBurn:
                    {
                        result = "( 1.0 - ( ( 1.0 - " + dstLocalVar + ") / " + srcLocalVar + ") )";
                    }
                    break;
                case BlendOps.ColorDodge:
                    {
                        result = "( " + dstLocalVar + "/ ( 1.0 - " + srcLocalVar + " ) )";
                    }
                    break;
                case BlendOps.Darken:
                    {
                        result = "min( " + srcLocalVar + " , " + dstLocalVar + " )";
                    }
                    break;
                case BlendOps.Divide:
                    {
                        result = "( " + dstLocalVar + " / " + srcLocalVar + " )";
                    }
                    break;
                case BlendOps.Difference:
                    {
                        result = "abs( " + srcLocalVar + " - " + dstLocalVar + " )";
                    }
                    break;
                case BlendOps.Exclusion:
                    {
                        result = "( 0.5 - 2.0 * ( " + srcLocalVar + " - 0.5 ) * ( " + dstLocalVar + " - 0.5 ) )";
                    }
                    break;
                case BlendOps.SoftLight:
                    {
                        result = string.Format( "2.0f*{0}*{1} + {0}*{0}*(1.0f - 2.0f*{1})", srcLocalVar, dstLocalVar );
                    }
                    break;
                case BlendOps.HardLight:
                    {
                        result = " ( " + srcLocalVar + " > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( " + srcLocalVar + " - 0.5 ) ) * ( 1.0 - " + dstLocalVar + " ) ) : ( 2.0 * " + srcLocalVar + " * " + dstLocalVar + " ) )";
                    }
                    break;
                case BlendOps.HardMix:
                    {
                        result = " round( 0.5 * ( " + srcLocalVar + " + " + dstLocalVar + " ) )";
                    }
                    break;
                case BlendOps.Lighten:
                    {
                        result = "	max( " + srcLocalVar + ", " + dstLocalVar + " )";
                    }
                    break;
                case BlendOps.LinearBurn:
                    {
                        result = "( " + srcLocalVar + " + " + dstLocalVar + " - 1.0 )";
                    }
                    break;
                case BlendOps.LinearDodge:
                    {
                        result = "( " + srcLocalVar + " + " + dstLocalVar + " )";
                    }
                    break;
                case BlendOps.LinearLight:
                    {
                        result = "( " + srcLocalVar + " > 0.5 ? ( " + dstLocalVar + " + 2.0 * " + srcLocalVar + " - 1.0 ) : ( " + dstLocalVar + " + 2.0 * ( " + srcLocalVar + " - 0.5 ) ) )";
                    }
                    break;
                case BlendOps.Multiply:
                    {
                        result = "( " + srcLocalVar + " * " + dstLocalVar + " )";
                    }
                    break;
                case BlendOps.Overlay:
                    {
                        result = "( " + dstLocalVar + " > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( " + dstLocalVar + " - 0.5 ) ) * ( 1.0 - " + srcLocalVar + " ) ) : ( 2.0 * " + dstLocalVar + " * " + srcLocalVar + " ) )";
                    }
                    break;
                case BlendOps.PinLight:
                    {
                        result = "( " + srcLocalVar + " > 0.5 ? max( " + dstLocalVar + ", 2.0 * ( " + srcLocalVar + " - 0.5 ) ) : min( " + dstLocalVar + ", 2.0 * " + srcLocalVar + " ) )";
                    }
                    break;
                case BlendOps.Subtract:
                    {
                        result = "( " + dstLocalVar + " - " + srcLocalVar + " )";
                    }
                    break;
                case BlendOps.Screen:
                    {
                        result = "( 1.0 - ( 1.0 - " + srcLocalVar + " ) * ( 1.0 - " + dstLocalVar + " ) )";
                    }
                    break;
                case BlendOps.VividLight:
                    {
                        result = "( " + srcLocalVar + " > 0.5 ? ( " + dstLocalVar + " / ( ( 1.0 - " + srcLocalVar + " ) * 2.0 ) ) : ( 1.0 - ( ( ( 1.0 - " + dstLocalVar + " ) * 0.5 ) / " + srcLocalVar + " ) ) )";
                    }
                    break;
            }

            if ( m_saturate )
                result = "( saturate( " + result + " ))";

            return CreateOutputLocalVariable( 0, result, ref dataCollector );
        }

        public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
        {
            base.WriteToString( ref nodeInfo, ref connectionsInfo );
            IOUtils.AddFieldValueToString( ref nodeInfo, m_currentBlendOp );
            IOUtils.AddFieldValueToString( ref nodeInfo, m_saturate );
        }

        public override void ReadFromString( ref string[] nodeParams )
        {
            base.ReadFromString( ref nodeParams );
            m_currentBlendOp = (BlendOps)Enum.Parse( typeof( BlendOps ), GetCurrentParam( ref nodeParams ) );
            m_saturate = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
            SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_currentBlendOp ) );
        }

		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
		}
	}
}
