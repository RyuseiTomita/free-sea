using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravestoneDestroy : MonoBehaviour
{
	[SerializeField] Collider m_collider;
	[SerializeField] Transform m_gravesPos;
	[SerializeField] GameObject m_effect;
	[SerializeField] AudioClip[] m_clips;
	[SerializeField] float m_gravestoneDeathTime;

	// Start is called before the first frame update
	void Start()
    {
		SoundEffect.Play2D(m_clips[0]);
	}

    // Update is called once per frame
    void Update()
    {
		m_gravestoneDeathTime -= Time.deltaTime;

		if(m_gravestoneDeathTime <= 0)
		{
			m_collider.enabled = true;
			Instantiate(m_effect, m_gravesPos.transform.position, Quaternion.Euler(-90, 0, 0));
			SoundEffect.Play2D(m_clips[1]);
			Destroy(gameObject);
		}
	}

}
