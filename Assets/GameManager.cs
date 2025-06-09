using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	//private bool m_bgmSecond;
	[SerializeField] AudioClip[] m_audioClip;
	[SerializeField] AudioSource m_audioSource;
 
    void Start()
    {
		m_audioSource = GetComponent<AudioSource>();
		m_audioSource.clip = m_audioClip[0];
		//m_bgmSecond = false;
	}

    void Update()
    {
        
    }

	public void BgmChange()
	{
		m_audioSource.clip = m_audioClip[1];
		m_audioSource.Play();
	}
}
