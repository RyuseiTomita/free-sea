using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
	[SerializeField] Collider m_collider;
	[SerializeField] int m_areaAttack;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			Debug.Log("Hit");
			other.GetComponent<PlayerMove>().HitDamage(m_areaAttack);
			m_collider.enabled = false;
		}
	}
}
