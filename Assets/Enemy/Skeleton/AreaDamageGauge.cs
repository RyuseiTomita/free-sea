using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaDamageGauge : MonoBehaviour
{
	[SerializeField] Slider m_areaDamageSlider;
	[SerializeField] const float MaxAreaDamage = 10;
	private float m_areaTime;
	[SerializeField] GameObject m_area;

    // Start is called before the first frame update
    void Start()
    {
		m_areaDamageSlider.value = m_areaTime;
		m_areaTime = 0;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        m_areaTime += Time.deltaTime;
		m_areaDamageSlider.value = m_areaTime;

		if(m_areaTime >= MaxAreaDamage)
		{
			m_area.SetActive(false);
			m_areaDamageSlider.value = 0;
			m_areaTime = 0f;
		}
	}
}
