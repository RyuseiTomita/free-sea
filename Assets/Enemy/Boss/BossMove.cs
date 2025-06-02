using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class BossMove : MonoBehaviour
{
	[SerializeField] GameObject m_player; // プレイヤーに追従
	[SerializeField] GameObject[] m_effect;
	[SerializeField] AudioClip[] m_clip;

	// 魔法攻撃
	[SerializeField] float m_magicAttackTime; 
	private const float MaxMagicAttackTime = 60f;
	private bool m_magicAttack;
	[SerializeField] float m_magicCoolDown; // 間隔

	// 弾の数
	[SerializeField] int m_magicNumber; // 魔法陣の順番
	private int m_magicNumberBomb;		// 爆弾の順番
	private const int  MaxMagicNumber = 5;
	private GameObject[] m_magicAttackEffect = new GameObject[10];

	//　骸骨の召喚
	[SerializeField] GameObject m_skelton;  
	private const int MaxskeltonSpawn = 3;
	[SerializeField] Transform[] m_pos;

	[SerializeField] float m_idleTime; // 何もしない時間

	private int m_bossAttackPattern = 0; // 攻撃パターン


	private Animator m_animator;

    void Start()
    {
		m_magicNumber = 0;
		m_magicNumberBomb = 0;

		m_magicCoolDown = 0;

		m_animator = GetComponent<Animator>();
		m_magicAttack = false;
    }

 
    void FixedUpdate()
    {
		m_idleTime -= Time.deltaTime;

		if (m_idleTime <= 0)
		{

			switch(m_bossAttackPattern)
			{
				case 0:
				{
					OnMagicAttackTime();
					break;
				}
				case 1:
				{
					SkeltonSpawn();
					break;
				}
			}
			Debug.Log("a");
			
		}


		if (m_magicAttack)
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

	public void AnimationSound()
	{
		SoundEffect.Play2D(m_clip[0]);
	}

	private void OnMagicAttack()
	{
		if (m_magicNumber >= MaxMagicNumber && m_magicAttack) // マジック攻撃が終わったら
		{
			m_magicNumber = 0;
			m_magicAttack = false;
			m_idleTime = 10f;
			m_bossAttackPattern++;
			return;
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
				SoundEffect.Play2D(m_clip[1]);
				m_magicNumber++;
				m_magicCoolDown = 1f;
				StartCoroutine(MagicAttackBomb());
			}
		}

		if(m_magicNumber >= MaxMagicNumber && m_magicNumberBomb >= MaxMagicNumber) // マジック攻撃が終わったら
		{
			
			
			
		}
	}
	private IEnumerator MagicAttackBomb()
	{
		yield return new WaitForSeconds(3);
		Instantiate(m_effect[1], m_magicAttackEffect[m_magicNumberBomb].transform.position, Quaternion.Euler(-90, 0, 0));
		SoundEffect.Play2D(m_clip[2]);
		m_magicNumberBomb++;
	}

	private void SkeltonSpawn()
	{
		m_animator.SetTrigger("SkeltonSpawn");
	}

	public void SummonMob()
	{
		for(int i = 0; i < m_pos.Length; i++)
		{
			Instantiate(m_skelton,m_pos[i].transform.position, Quaternion.identity);
		}
		SoundEffect.Play2D(m_clip[3]);
		m_bossAttackPattern++;
		m_idleTime = 10f;
	}
}
