using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCruseDamage : MonoBehaviour
{
	[SerializeField] AudioClip m_clip;

	private float m_speedReduction; // ‘¬“x’á‰º

	private void Start()
	{
		m_speedReduction = 3;
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
