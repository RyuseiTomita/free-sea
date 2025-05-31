using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class BossMove : MonoBehaviour
{
	[SerializeField] GameObject m_player; // ÉvÉåÉCÉÑÅ[Ç…í«è]
	[SerializeField] GameObject[] m_effect;

	// ñÇñ@çUåÇ
	[SerializeField] float m_magicAttackTime; 
	private const float MaxMagicAttackTime = 60f;
	private bool m_magicAttack;
	private float m_magicCoolDown; // ä‘äu

	// íeÇÃêî
	[SerializeField] int m_magicNumber;
	private int m_magicNumberBomb;
	private const int  MaxMagicNumber = 5;
	private GameObject[] m_magicAttackEffect = new GameObject[10];

	[SerializeField] float m_idleTime; // âΩÇ‡ÇµÇ»Ç¢éûä‘

    private Animator m_animator;

    void Start()
    {
		m_magicNumber = 0;
		m_magicNumberBomb = 0;

		m_magicCoolDown = 1f;

		m_animator = GetComponent<Animator>();
		m_magicAttack = false;
    }

 
    void FixedUpdate()
    {
		m_idleTime -= Time.deltaTime;
		if (m_idleTime <= 0)
		{
			Debug.Log("a");
			OnMagicAttackTime();
		}

		if(m_magicAttack)
		{
			OnMagicAttack();
		}
	}

	private void OnMagicAttackTime()
	{
		m_magicAttackTime -= Time.deltaTime;

		if(m_magicAttackTime <= 0)
		{
			m_animator.SetTrigger("MagicAttack");
			m_magicAttackTime = MaxMagicAttackTime;
		}
	}

	public void OnMagicAttackAnimation()
	{
		m_magicAttack = true;
		
	}

	private void OnMagicAttack()
	{
		if (m_magicNumber >= MaxMagicNumber && m_magicAttack)
		{
			m_magicNumber = 0;
			m_magicAttack = false;
		}
		else
		{
			Debug.Log("B");

			m_magicCoolDown -= Time.deltaTime;

			if (m_magicCoolDown <= 0)
			{
				Debug.Log("a");
				if (m_magicNumber == 0) { m_magicNumberBomb = 0; }
				GameObject m_effectBomb = Instantiate(m_effect[0], m_player.transform.position, Quaternion.identity);
				m_magicAttackEffect[m_magicNumber] = m_effectBomb;
				m_magicNumber++;
				m_magicCoolDown = 1f;
				StartCoroutine(MagicAttackBomb());
			}
		}
	}

	private IEnumerator MagicAttackBomb()
	{
		yield return new WaitForSeconds(3);
		Instantiate(m_effect[1], m_magicAttackEffect[m_magicNumberBomb].transform.position, Quaternion.Euler(-90, 0, 0));
		m_magicNumberBomb++;
	}
}
