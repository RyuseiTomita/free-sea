using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraveHealth : MonoBehaviour
{
	[SerializeField] int m_graveHealth;
	[SerializeField] Slider m_healthSlider;
	[SerializeField] GameObject m_boss;
	[SerializeField] AudioClip m_clips;
	[SerializeField] GameObject m_graveobj;
	private int m_grave;
	private bool m_graveBreak;

	// Start is called before the first frame update
	void Start()
    {
		m_grave = 1;
		m_graveBreak = false;
	}

    // Update is called once per frame
    void Update()
    {
		m_healthSlider.value = m_graveHealth;
	}

	public void AttackHit(int damage)
	{
		Debug.Log("Hit");
		m_graveHealth -= damage;

		if(m_graveHealth <= 0)
		{
			m_boss.GetComponent<BossMove>().ShieldBreak();
			SoundEffect.Play2D(m_clips);
			Destroy(gameObject);
		}
	}
}
