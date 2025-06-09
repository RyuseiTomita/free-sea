using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHit : MonoBehaviour
{
	[SerializeField] GameObject m_boss;
	[SerializeField] AudioClip[] m_clips;
	private const int m_attack = 2; // É_ÉÅÅ[ÉW

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Boss"))
		{
			m_boss.GetComponent<BossMove>().HitAttack(m_attack);
		}

		if(other.TryGetComponent<GraveHealth>(out var grave))
		{
			grave.AttackHit(m_attack);
		}

		if(other.gameObject.CompareTag("Shield"))
		{
			SoundEffect.Play2D(m_clips[0]);
		}
	}
}
