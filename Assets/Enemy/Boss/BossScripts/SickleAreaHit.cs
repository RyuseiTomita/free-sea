using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SickleAreaHit : MonoBehaviour
{
	[SerializeField] Collider m_colider;

	[SerializeField] const int m_damage = 10;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Debug.Log("Hit");
			other.GetComponent<PlayerMove>().HitDamage(m_damage);
			m_colider.enabled = false;
		}
	}
}
