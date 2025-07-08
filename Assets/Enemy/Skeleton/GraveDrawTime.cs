using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraveDrawTime : MonoBehaviour
{

	[SerializeField] Slider m_graveSlider;
	[SerializeField] float m_graveTime;

    // Start is called before the first frame update
    void Start()
    {
		m_graveSlider.value = m_graveTime;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		m_graveTime -= Time.deltaTime;
		m_graveSlider.value = m_graveTime;
		transform.rotation = Camera.main.transform.rotation;
	}
}
