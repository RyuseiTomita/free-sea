using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BossCruseDamage : MonoBehaviour
{
	[SerializeField] AudioClip[] m_clips;
	[SerializeField] GameObject m_player;
	[SerializeField] GameObject m_effect;
	[SerializeField] Transform m_transform;

	private float m_speedReduction; // ���x�ቺ
	[SerializeField] float m_DestroyTime;

	private void Start()
	{
		m_speedReduction = 2f; 
	}

	private void Update()
	{
		m_DestroyTime -= Time.deltaTime;
		if(m_DestroyTime <= 0)
		{
			m_player.GetComponent<PlayerMove>().HitCruseAttackExit();
			SoundEffect.Play2D(m_clips[0]);
			Instantiate(m_effect, m_transform.transform.position, Quaternion.Euler(-90, 0, 0));
			Destroy(gameObject);
		}
	}

	public void SetPlayer(GameObject player)
	{
		m_player = player;
	}

	private void OnTriggerStay(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerMove>().HitCruseAttack(m_speedReduction);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerMove>().HitCruseAttackExit();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerMove>().HitCruseAttackSound();
		}
	}
}
