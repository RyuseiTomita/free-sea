using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] Collider[] m_collider;

	private void Start()
	{
		
	}

	// �m�[�}����Ԃ̎��̓����蔻��
	public void OnAttackNormal()
	{
		m_collider[0].enabled = true;
	}
	public void OnAttackNormalEnd()
	{
		m_collider[0].enabled = false;
	}

	// �o����Ԃ̎��̓����蔻��
	public void OnAttackSkill()
	{
		m_collider[1].enabled = true;
	}

	public void OnAttackSkillEnd()
	{
		m_collider[1].enabled = false;
	}
}
