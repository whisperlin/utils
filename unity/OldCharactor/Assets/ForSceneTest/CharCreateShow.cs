using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CharCreateShow : MonoBehaviour {
	Animation m_anim;
	Canvas m_canvas;
	List<Renderer> m_char_renderers = new List<Renderer>();
	bool m_finish;

	void OnEnable() {
		m_anim = GetComponent<Animation>();
		m_finish = false;
		m_canvas = transform.Find("Canvas").GetComponent<Canvas>();
		m_canvas.gameObject.SetActive(false);
		m_canvas.renderMode = RenderMode.ScreenSpaceCamera;

		m_char_renderers.Clear();
		var rs = gameObject.GetComponentsInChildren<Renderer>(true);
		for(int i = 0; i < rs.Length; i++) {
			if(rs[i] is MeshRenderer || rs[i] is SkinnedMeshRenderer) {
				rs[i].gameObject.SetActive(true);
				m_char_renderers.Add(rs[i]);
			}
		}

		StopAllCoroutines();
	}

	void Update() {
		if (!m_anim.isPlaying) {
			if (!m_finish) {
				m_finish = true;
				m_canvas.gameObject.SetActive(true);

				for (int i = 0; i < m_char_renderers.Count; i++) {
					m_char_renderers[i].gameObject.SetActive(false);
				}

				StartCoroutine(BeginLoop());
			}
		}
	}

	IEnumerator BeginLoop() {
		yield return null;

		m_canvas.renderMode = RenderMode.WorldSpace;
		m_anim.Play("Loop");
	}
}