using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMagicAttackDamage : MonoBehaviour
{
	[SerializeField] int m_damage;
	[SerializeField] Collider m_collider;

	private void Start()
	{
		StartCoroutine(MagicAttackCollider());
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			m_collider.enabled = false;
			other.gameObject.GetComponent<PlayerMove>().HitDamage(m_damage);
		}
	}

	private IEnumerator MagicAttackCollider()
	{
		yield return new WaitForSeconds(0.5f);
		m_collider.enabled = false;
	}
}
