using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class PlayerAttackHit : MonoBehaviour
{
	private enum SwordType
	{
		Normal,
		Skill,
	}

	[SerializeField] AudioClip m_shieldSound;
	[SerializeField] List<int> m_damageList;
	[SerializeField] Collider[] m_collider;
	[SerializeField] GameObject m_hitEffect;
	[SerializeField] AudioClip[] m_hitSound;
	[SerializeField] Collider m_boss;
	
	private int m_damageIndex;

	public void OnAttackNormal(int damage)
	{
		m_collider[(int)SwordType.Normal].enabled = true;
		m_damageIndex = damage;
	}

	public void OnAttackNormalEnd()
	{
		m_collider[(int)SwordType.Normal].enabled = false;
		m_damageIndex = 0;
	}

	public void OnAttackSkill(int damage)
	{
		m_collider[(int)SwordType.Skill].enabled = true;
		m_damageIndex = damage;
	}

	public void OnAttackSkillEnd()
	{
		m_collider[(int)SwordType.Skill].enabled = false;
		m_damageIndex = 0;
	}

	void OnTriggerEnter(Collider other)
	{
		// ボスに当たったら
		if (other.TryGetComponent<BossMove>(out var health))
		{
			// コライダーを外す
			m_collider[(int)SwordType.Normal].enabled = false;
			m_collider[(int)SwordType.Skill].enabled = false;

			// ボススクリプトにdamageIndexを渡す
			other.gameObject.GetComponent<BossMove>().HitAttack(m_damageList[m_damageIndex]);

			// 対象ObjectにHitEffectを生成
			Instantiate(m_hitEffect, other.transform.position + Vector3.up, Quaternion.identity);

			// サウンドを鳴らす
			SoundEffect.Play2D(m_hitSound[m_damageIndex]);
		}

		if (other.TryGetComponent<GraveHealth>(out var grave))
		{
			grave.AttackHit(m_damageList[m_damageIndex]);
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

		if (other.gameObject.CompareTag("Shield"))
		{
			SoundEffect.Play2D(m_shieldSound);
			Instantiate(m_hitEffect, other.transform.position + Vector3.up, Quaternion.identity);
			SoundEffect.Play2D(m_hitSound[m_damageIndex]);
		}
	}
}
