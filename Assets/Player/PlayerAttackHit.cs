using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class PlayerAttackHit : MonoBehaviour
{
	[SerializeField] AudioClip[] m_clips;
	[SerializeField] List<int> m_damageList;
	[SerializeField] Collider[] m_collider;
	[SerializeField] GameObject m_hitEffect;
	[SerializeField] AudioClip[] m_hitSound;
	[SerializeField] Collider m_boss;
	
	private int m_damageIndex;

	public void OnAttackNormal(int damage)
	{
		m_collider[0].enabled = true;
		m_damageIndex = damage;
	}

	public void OnAttackNormalEnd()
	{
		m_collider[0].enabled = false;
		m_damageIndex = 0;
	}

	public void OnAttackSkill(int damage)
	{
		m_collider[1].enabled = true;
		m_damageIndex = damage;
	}

	public void OnAttackSkillEnd()
	{
		m_collider[1].enabled = false;
		m_damageIndex = 0;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent<BossMove>(out var health))
		{
			m_collider[0].enabled = false;
			m_collider[1].enabled = false;
			other.gameObject.GetComponent<BossMove>().HitAttack(m_damageList[m_damageIndex]);
			Instantiate(m_hitEffect, other.transform.position + Vector3.up, Quaternion.identity);
			SoundEffect.Play2D(m_hitSound[m_damageIndex]);
		}

		if(other.TryGetComponent<GraveHealth>(out var grave))
		{
			grave.AttackHit(m_damageList[m_damageIndex]);
			Instantiate(m_hitEffect, other.transform.position + Vector3.up, Quaternion.identity);
			SoundEffect.Play2D(m_hitSound[m_damageIndex]);
		}

		if(other.gameObject.CompareTag("Shield"))
		{
			SoundEffect.Play2D(m_clips[0]);
			Instantiate(m_hitEffect, other.transform.position + Vector3.up, Quaternion.identity);
			SoundEffect.Play2D(m_hitSound[m_damageIndex]);
		}

		if (other.TryGetComponent<SkeletonMove>(out var skeleton))
		{
			skeleton.PlayerAttackHit();
			Instantiate(m_hitEffect, other.transform.position + Vector3.up, Quaternion.identity);
			SoundEffect.Play2D(m_hitSound[m_damageIndex]);
		}

		if (other.TryGetComponent<AwakeningSkeltonEndMove>(out var awakeningskeleton))
		{
			awakeningskeleton.PlayerAttackHit();
			Instantiate(m_hitEffect, other.transform.position + Vector3.up, Quaternion.identity);
			SoundEffect.Play2D(m_hitSound[m_damageIndex]);
		}
	}
}
