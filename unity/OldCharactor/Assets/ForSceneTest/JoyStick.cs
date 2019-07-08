using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class JoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler {
	RectTransform m_bar_transform;
	RectTransform m_stick_transform;
	Vector2 m_stick_offset;
	const float STICK_DIS_MAX = 78;

	public GameObject character;
	GameObject scene;
	Camera m_camera;
	DemoCharacter m_char;
    public bool useNav = false;

	Text m_fps;
	int m_frame;
	float m_time;

	int m_move_point_id = -2;
	int m_rot_point_id = -2;

	IEnumerator Start() {
		Screen.SetResolution(1280, 720, true);

		m_bar_transform = transform.Find("Bar").GetComponent<RectTransform>();
		m_stick_transform = transform.Find("Bar/Stick").GetComponent<RectTransform>();
		m_stick_offset = Vector2.zero;

		m_camera = Camera.main;

		m_fps = transform.parent.Find("Text").GetComponent<Text>();
		m_frame = 0;
		m_time = Time.realtimeSinceStartup;

		yield return 0;

		RaycastHit hit;
		if(Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out hit, 1000)) {
			if(character != null) {
				character.transform.position = hit.point;

				m_char = character.AddComponent<DemoCharacter>();
                m_char.useNav = this.useNav;
                character.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }
		}
	}

	public void OnBeginDrag(PointerEventData eve) {
		if(m_move_point_id == eve.pointerId) {
			if(character != null) {
				character.SendMessage("OnStartMove");
			}
		}
	}

	public void OnDrag(PointerEventData eve) {
		if(m_move_point_id == eve.pointerId) {
			m_stick_offset += eve.delta;
		
			var pos = m_stick_offset;
			if(pos.magnitude > STICK_DIS_MAX) {
				pos = pos.normalized * STICK_DIS_MAX;
			}

			m_stick_transform.anchoredPosition = pos;
		}

		if(m_rot_point_id == eve.pointerId) {
			var rot_deg = eve.delta.x * 0.2f;
			
			var target_pos = character.transform.position + new Vector3(0, m_char.y_offset, 0);
			var distance = (m_camera.transform.position - target_pos).magnitude;
			var new_dir = m_camera.transform.rotation.eulerAngles + new Vector3(0, rot_deg, 0);
			m_camera.transform.rotation = Quaternion.Euler(new_dir);
			m_camera.transform.position = target_pos - m_camera.transform.forward * distance;
		}
	}

	public void OnPointerDown(PointerEventData eve) {
		if(eve.position.x < Screen.width / 2) {
			m_bar_transform.anchoredPosition = eve.position;
			m_bar_transform.gameObject.SetActive(true);
			m_stick_offset = Vector2.zero;

			m_move_point_id = eve.pointerId;
		} else {
			m_rot_point_id = eve.pointerId;
		}
	}

	public void OnPointerUp(PointerEventData eve) {
		if(m_move_point_id == eve.pointerId) {
			m_stick_transform.anchoredPosition = Vector2.zero;
			m_bar_transform.gameObject.SetActive(false);
			m_stick_offset = Vector2.zero;

			if(character != null) {
				character.SendMessage("OnStop");
			}

			m_move_point_id = -2;
		} else {
			m_rot_point_id = -2;
		}
	}

	void Update() {
		if(m_stick_offset.sqrMagnitude > 0) {
			var dir = m_stick_offset.normalized;

			if(character != null) {
				character.SendMessage("OnMove", dir);
			}
		}

		m_frame++;
		if(Time.realtimeSinceStartup - m_time > 1) {
			m_fps.text = "fps:" + m_frame + " w:" + Screen.width + " h:" + Screen.height;
			m_time = Time.realtimeSinceStartup;
			m_frame = 0;
		}
	}
}