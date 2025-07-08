using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SickAreaGauge : MonoBehaviour
{
	[SerializeField] Slider m_sickAreaSlider;
	[SerializeField] GameObject m_sickDamageTime;

	[SerializeField] const float MaxTime = 5f;
	[SerializeField] float m_sickAreatime = 0;

	private bool m_sickDraw;

    // Start is called before the first frame update
    void Start()
    {
		m_sickAreaSlider.value = 0;
		m_sickDraw = false;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		Debug.Log(m_sickDraw);

		if (!m_sickDraw) return;

		m_sickAreatime += Time.deltaTime;
		m_sickAreaSlider.value = m_sickAreatime;

		if (m_sickAreatime >= MaxTime)
		{
			m_sickAreaSlider.value = 0f;
			m_sickAreatime = 0f;
			m_sickDraw = false;
			m_sickDamageTime.SetActive(false);
		}
	}

	public void SickAttackDrawTime(bool draw)
	{
		m_sickDraw = draw;
	}
}
