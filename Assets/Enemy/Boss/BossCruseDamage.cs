using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCruseDamage : MonoBehaviour
{
	[SerializeField] AudioClip m_clip;
	[SerializeField] GameObject m_player;

	private float m_speedReduction; // ‘¬“x’á‰º
	private bool m_curseRange;
	private float m_DestroyTime;

	private void Start()
	{
		m_speedReduction = 2f;
		m_DestroyTime = 15f; 
		m_curseRange = true;
	}

	private void Update()
	{
		m_DestroyTime -= Time.deltaTime;
		if(m_DestroyTime <= 0)
		{
			m_player.GetComponent<PlayerMove>().HitCruseAttackExit();
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
