using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHit : MonoBehaviour
{
	[SerializeField] GameObject m_boss;
	private const int m_attack = 2; // É_ÉÅÅ[ÉW

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Boss"))
		{
			Debug.Log("Hit");
			m_boss.GetComponent<BossMove>().HitAttack(m_attack);
		}
	}
}
