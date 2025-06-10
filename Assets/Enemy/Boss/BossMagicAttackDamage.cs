using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMagicAttackDamage : MonoBehaviour
{
	private int m_damage;
	[SerializeField] Collider m_collider;

	private void Start()
	{
		m_damage = 3;
		StartCoroutine(MagicAttackCollider());
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			m_collider.enabled = false;
			other.gameObject.GetComponent<PlayerMove>().HitMagicAttack(m_damage);
		}
	}

	private IEnumerator MagicAttackCollider()
	{
		yield return new WaitForSeconds(1);
		m_collider.enabled = false;
	}
}
