// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEngine.Sprites;

public class SpriteMaskController : MonoBehaviour
{
	private SpriteRenderer m_spriteRenderer;
	private Vector4 m_uvs;

	void Start ()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_uvs = DataUtility.GetInnerUV( m_spriteRenderer.sprite );
		m_spriteRenderer.material.SetVector( "_CustomUVS", m_uvs );
	}
}
