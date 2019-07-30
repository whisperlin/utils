using System;
using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class NodeUsageRegister
	{
		// Sampler Nodes registry
		[SerializeField]
		private List<ParentNode> m_nodes;

		[SerializeField]
		private string[] m_nodesArr;

		public NodeUsageRegister()
		{
			m_nodes = new List<ParentNode>();
			m_nodesArr = new string[0];
		}

		public void Destroy()
		{
			m_nodes.Clear();
			m_nodes.Clear();
			m_nodes = null;
			m_nodesArr = null;
		}

		public void Clear()
		{
			m_nodes.Clear();
		}

		public int AddNode( ParentNode node )
		{
			if ( !m_nodes.Contains( node ) )
			{
				m_nodes.Add( node );
				UpdateNodeArr();
				return m_nodes.Count - 1;
			}
			return -1;
		}

		public void RemoveNode( ParentNode node )
		{
			if ( m_nodes.Contains( node ) )
			{
				m_nodes.Remove( node );
				UpdateNodeArr();
			}
		}

		public void UpdateNodeArr()
		{
			m_nodesArr = new string[ m_nodes.Count ];
			int count = m_nodesArr.Length;
			for ( int i = 0; i < count; i++ )
			{
				m_nodesArr[ i ] = m_nodes[ i ].DataToArray;
			}
		}

		public ParentNode GetNode( int idx )
		{
			if ( idx > -1 && idx < m_nodes.Count )
			{
				return m_nodes[ idx ];
			}
			return null;
		}
		
		public int GetNodeRegisterId( int uniqueId )
		{
			int count = m_nodes.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( m_nodes[ i ].UniqueId == uniqueId )
				{
					return i;
				}
			}
			return -1;
		}

		public void UpdateDataOnNode( int nodeIdx, string data )
		{
			int count = m_nodes.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( m_nodes[ i ].UniqueId == nodeIdx )
				{
					m_nodesArr[ i ] = data;
				}
			}
		}

		public void Dump()
		{
			string data = string.Empty;

			for ( int i = 0; i < m_nodesArr.Length; i++ )
			{
				data += m_nodesArr[ i ] + '\n';
			}
			Debug.Log( data );
		}

		public string[] NodesArr { get { return m_nodesArr; } }
		public List<ParentNode> NodesList { get { return m_nodes; } }
	}
}
