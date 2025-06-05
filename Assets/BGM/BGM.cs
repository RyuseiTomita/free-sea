using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
	[SerializeField]
	AudioClip[] m_clip;

	AudioSource m_audioSource;

	Coroutine m_coroutine;
	float m_durationTime;
	float m_elapsedTime;

	private void Start()
	{
		m_audioSource = GetComponent<AudioSource>();
		m_coroutine = null;
		Play(m_clip[0]);
	}

	void Play(AudioClip clip)
	{
		if (m_coroutine != null)
		{
			StopCoroutine(m_coroutine);
		}
		m_coroutine = null;

		m_audioSource.clip = clip;
		m_audioSource.volume = 0.5f;
		m_audioSource.Play();
	}

	public void PlaySecondHalf()
	{
		Play(m_clip[1]);
	}

	public void Fadeout(float duration)
	{
		m_durationTime = duration;
		m_elapsedTime = 0;

		m_coroutine = StartCoroutine(_Fadeout());
	}

	IEnumerator _Fadeout()
	{
		while (m_durationTime > m_elapsedTime)
		{
			m_elapsedTime += Time.deltaTime;
			m_audioSource.volume = Mathf.Lerp(1, 0, m_elapsedTime / m_durationTime);
			yield return null;
		}
	}
}
