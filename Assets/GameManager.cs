using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
	[SerializeField] AudioClip[] m_audioClip;
	[SerializeField] AudioSource m_audioSource;

	[SerializeField] GameObject m_circle;

	private bool m_circleFlg;


	void Start()
    {
		m_audioSource = GetComponent<AudioSource>();
		m_audioSource.clip = m_audioClip[0];
		m_circleFlg = false;
	}

	public void BgmChange()
	{
		m_audioSource.clip = m_audioClip[1];
		m_audioSource.Play();
	}

	public void AwakeingCircle(bool circle)
	{
		m_circleFlg  = circle;

		if (m_circleFlg)
		{
			m_circle.SetActive(true);
		}
		else
		{
			m_circle.SetActive(false);
		}
	}
}
