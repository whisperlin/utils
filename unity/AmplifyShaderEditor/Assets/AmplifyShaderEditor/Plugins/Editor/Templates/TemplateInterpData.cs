// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateInterpElement
	{
		public TemplateSemantics Semantic;
		public bool[] AvailableChannels = { true, true, true, true };
		public bool IsFull = false;
		public int Usage = 0;
		public string Name;

		public TemplateInterpElement( TemplateInterpElement other )
		{
			Semantic = other.Semantic;
			for ( int i = 0; i < AvailableChannels.Length; i++ )
			{
				AvailableChannels[ i ] = other.AvailableChannels[ i ];
			}
			IsFull = other.IsFull;
			Usage = other.Usage;
			Name = other.Name;
		}

		public TemplateInterpElement( TemplateSemantics semantic )
		{
			Semantic = semantic;
			int semanticId = TemplateHelperFunctions.SemanticToInt[ Semantic ];
			Name = ( semanticId == 0 ) ? TemplateHelperFunctions.BaseInterpolatorName : TemplateHelperFunctions.BaseInterpolatorName + semanticId.ToString();
		}

		public void SetAvailableChannelsFromString( string channels )
		{
			for ( int i = 0; i < AvailableChannels.Length; i++ )
			{
				AvailableChannels[ i ] = false;
			}
			Usage = AvailableChannels.Length;

			for ( int i = 0; i < channels.Length; i++ )
			{
				switch ( channels[ i ] )
				{
					case 'x': if ( !AvailableChannels[ 0 ] ) { AvailableChannels[ 0 ] = true; Usage--; } break;
					case 'y': if ( !AvailableChannels[ 1 ] ) { AvailableChannels[ 1 ] = true; Usage--; } break;
					case 'z': if ( !AvailableChannels[ 2 ] ) { AvailableChannels[ 2 ] = true; Usage--; } break;
					case 'w': if ( !AvailableChannels[ 3 ] ) { AvailableChannels[ 3 ] = true; Usage--; } break;
				}
			}
		}

		public TemplateVertexData RequestChannels( WirePortDataType type, bool isColor )
		{
			if ( IsFull )
				return null;

			int channelsRequired = TemplateHelperFunctions.DataTypeChannelUsage[ type ];
			if ( channelsRequired == 0 )
				return null;

			int firstChannel = -1;
			for ( int i = 0; i < AvailableChannels.Length; i++ )
			{
				if ( AvailableChannels[ i ] )
				{
					if ( firstChannel < 0 )
					{
						firstChannel = i;
					}
					channelsRequired -= 1;
					if ( channelsRequired == 0 )
						break;
				}
			}

			//did not found enough channels to fill request
			if ( channelsRequired > 0 )
				return null;

			Usage += 1;
			TemplateVertexData data = null;

			if ( type == WirePortDataType.COLOR || type == WirePortDataType.FLOAT4 )
			{
				// Automatically lock all channels
				for ( int i = firstChannel; i < ( firstChannel + channelsRequired ); i++ )
				{
					AvailableChannels[ i ] = false;
				}
				IsFull = true;
				data = new TemplateVertexData( Semantic, type, Name );
			}
			else
			{
				string[] swizzleArray = ( isColor ) ? TemplateHelperFunctions.ColorSwizzle : TemplateHelperFunctions.VectorSwizzle;
				string channels = ".";
				int count = firstChannel + TemplateHelperFunctions.DataTypeChannelUsage[ type ];
				for ( int i = firstChannel; i < count; i++ )
				{
					AvailableChannels[ i ] = false;
					channels += swizzleArray[ i ];
					if ( i == ( AvailableChannels.Length - 1 ) )
					{
						IsFull = true;
					}
				}

				data = new TemplateVertexData( Semantic, type, Name, channels );
			}
			return data;
		}
	}

	[Serializable]
	public class TemplateInterpData
	{
		public List<TemplateInterpElement> AvailableInterpolators = new List<TemplateInterpElement>();
		public List<TemplateVertexData> Interpolators = new List<TemplateVertexData>();

		public TemplateInterpData() { }
		public TemplateInterpData( TemplateInterpData other )
		{
			foreach ( TemplateInterpElement data in other.AvailableInterpolators )
			{
				AvailableInterpolators.Add( new TemplateInterpElement( data ) );
			}

			for ( int i = 0; i < other.Interpolators.Count; i++ )
			{
				Interpolators.Add( new TemplateVertexData( other.Interpolators[ i ] ) );
			}
		}

		public void ReplaceNameOnInterpolator( TemplateSemantics semantic, string newName )
		{
			for ( int i = 0; i < AvailableInterpolators.Count; i++ )
			{
				if ( AvailableInterpolators[ i ].Semantic == semantic )
				{
					AvailableInterpolators[ i ].Name = newName;
					break;
				}
			}
		}

		public void Destroy()
		{
			AvailableInterpolators.Clear();
			AvailableInterpolators = null;

			Interpolators.Clear();
			Interpolators = null;
		}
	}
}
