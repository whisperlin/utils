using UnityEngine;
using System.Collections;

public class ForTest : MonoBehaviour {
	Animation m_anim;
	ParticleSystem m_ps;

	void Start() {
		m_anim = GetComponent<Animation>();
		//m_anim["Attack8"].wrapMode = WrapMode.Loop;
		//m_anim.Play("Attack8");

		//m_ps = transform.Find("Attack8").GetComponent<ParticleSystem>();
	}

	void OnEnable() {
		if(m_anim != null) {
			//m_anim.Play("Attack8");
		}
	}

	void Update() {
		if(m_ps != null && m_ps.gameObject.activeSelf) {
			if(!m_ps.IsAlive(true)) {
				m_ps.Play(true);
			}
		}
	}

	public void ShowEffect(bool show) {
		if(m_ps != null) {
			m_ps.gameObject.SetActive(show);
		}
	}

	public void PlayAnim(bool play) {
		if(play) {
			m_anim.Play("Attack8");
		} else {
			m_anim.Stop();
		}
	}
}
