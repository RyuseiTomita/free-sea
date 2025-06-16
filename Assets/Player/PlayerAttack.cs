using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] Collider[] m_collider;

	private void Start()
	{
		
	}

	// ƒm[ƒ}ƒ‹ó‘Ô‚Ì‚Ì“–‚½‚è”»’è
	public void OnAttackNormal()
	{
		m_collider[0].enabled = true;
	}
	public void OnAttackNormalEnd()
	{
		m_collider[0].enabled = false;
	}

	// ŠoÁó‘Ô‚Ì‚Ì“–‚½‚è”»’è
	public void OnAttackSkill()
	{
		m_collider[1].enabled = true;
	}

	public void OnAttackSkillEnd()
	{
		m_collider[1].enabled = false;
	}
}
