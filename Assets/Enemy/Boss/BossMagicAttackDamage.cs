using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMagicAttackDamage : MonoBehaviour
{
	private int m_damage = 5;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerMove>().HitMagicAttack(m_damage);
		}
	}
}
