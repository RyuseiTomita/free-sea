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
		// �{�X�ɓ���������
		if (other.TryGetComponent<BossMove>(out var health))
		{
			// �R���C�_�[���O��
			m_collider[(int)SwordType.Normal].enabled = false;
			m_collider[(int)SwordType.Skill].enabled = false;

			// �{�X�X�N���v�g��damageIndex��n��
			other.gameObject.GetComponent<BossMove>().HitAttack(m_damageList[m_damageIndex]);

			// �Ώ�Object��HitEffect�𐶐�
			Instantiate(m_hitEffect, other.transform.position + Vector3.up, Quaternion.identity);

			// �T�E���h��炷
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
