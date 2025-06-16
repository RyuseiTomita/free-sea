using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHit : MonoBehaviour
{
	[SerializeField] GameObject m_boss;
	[SerializeField] AudioClip[] m_clips;
	[SerializeField] List<int> m_damageList;
	[SerializeField] Collider m_collider;

	int m_damageIndex;

	public void OnAttackNormal(int damageIndex)
	{
		m_collider.enabled = true;
		m_damageIndex = damageIndex;
		Debug.Log("a");
	}

	public void OnAttackNormalEnd()
	{
		m_collider.enabled = false;
		m_damageIndex = 0;
		Debug.Log("b");
	}

	public void OnAttackSkill(int damageIndex)
	{
		m_collider.enabled = true;
		m_damageIndex = damageIndex;
	}

	public void OnAttackSkillEnd()
	{
		m_collider.enabled = false;
		m_damageIndex = 0;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Boss"))
		{
			m_boss.GetComponent<BossMove>().HitAttack(m_damageList[m_damageIndex]);
			Debug.Log(m_damageIndex);

			m_collider.enabled = false;
		}

		if(other.TryGetComponent<GraveHealth>(out var grave))
		{
			grave.AttackHit(m_damageList[m_damageIndex]);

			m_collider.enabled = false;
		}

		if(other.gameObject.CompareTag("Shield"))
		{
			SoundEffect.Play2D(m_clips[0]);
		}

		if(other.gameObject.CompareTag("Skelton"))
		{
			other.GetComponent<SkeletonMove>().PlayerAttackHit();
			m_collider.enabled = false;
		}
	}
}
