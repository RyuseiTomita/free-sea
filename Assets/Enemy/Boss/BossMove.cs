using System;
using System.Collections;
using UnityEngine;

public class BossMove : MonoBehaviour
{
	[SerializeField] GameObject[] m_model;
	[SerializeField] Transform m_lookPlayer; // プレイヤーに追従
	[SerializeField] GameObject m_player;
	[SerializeField] GameObject[] m_effect;
	[SerializeField] AudioClip[] m_clip;

	// 魔法攻撃
	[SerializeField] float m_magicAttackTime;
	//private const float MaxMagicAttackTime = 60f;
	private bool m_magicAttack;
	[SerializeField] float m_magicCoolDown; // 間隔

	// 弾の数
	[SerializeField] int m_magicNumber; // 魔法陣の順番
	private int m_magicNumberBomb;      // 爆弾の順番
	private const int MaxMagicNumber = 6;
	private GameObject[] m_magicAttackEffect = new GameObject[10];

	//　骸骨の召喚
	[SerializeField] GameObject m_skelton;
	[SerializeField] Transform[] m_skeltonpos;   // 骸骨のポジション
	//private bool m_skeltonSpawn;

	// 呪いの攻撃
	[SerializeField] GameObject[] m_skeltonHead; // 骸骨の頭
	[SerializeField] GameObject m_curseEffect;
	[SerializeField] Transform[] m_cursePos;
	private float m_curseDrawTime;
	private const float MaxCurseTime = 15f; 
	private bool m_curseDrawFlg;

	[SerializeField] float m_idleTime; // 何もしない時間

	private int m_bossAttackPattern = 0; // 攻撃パターン
	private const float MagicAttackTime = 10f;
	private const float SkeletonTime = 12f;
	private const float CurseTime = 5f;

	//private bool m_isMove; // 敵が動いているか


	private Animator m_animator;

	void Start()
	{
		m_curseDrawTime = 15f;
		m_curseDrawFlg = false;

		m_idleTime = MagicAttackTime;

		m_animator = m_model[0].GetComponent<Animator>();
		m_magicNumber = 0;
		m_magicNumberBomb = 0;

		m_magicCoolDown = 0;

	
		m_magicAttack = false;
		//m_skeltonSpawn = false;
		//m_isMove = false;
	}


	void FixedUpdate()
	{
		// プレイヤーに向く
		transform.rotation = Quaternion.Lerp(
			transform.rotation,
			Quaternion.LookRotation(m_lookPlayer.position - transform.position), 0.2f);

		m_idleTime -= Time.deltaTime;

		if (m_idleTime <= 0)
		{
			switch (m_bossAttackPattern)
			{
				case 0:
					{
						OnMagicAttackTime();
						break;
					}
				case 1:
					{
						SkeltonSpawnAnimation();
						break;
					}
				case 2:
					{
						CurseAttackAnimation();
						break;
					}
			}
		}

		if (m_magicAttack)
		{
			OnMagicAttack();
		}

		if(m_curseDrawFlg)
		{
			m_curseDrawTime -= Time.deltaTime;

			CurseDrawTime(m_curseDrawTime);
		}
	}

	private void OnMagicAttackTime()
	{
		m_animator.SetTrigger("MagicAttack");
		m_idleTime = SkeletonTime;
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
			m_bossAttackPattern++;
			m_magicCoolDown = 0f;
		}
		else
		{
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
	}

	private IEnumerator MagicAttackBomb()
	{
		yield return new WaitForSeconds(2);
		Instantiate(m_effect[1], m_magicAttackEffect[m_magicNumberBomb].transform.position, Quaternion.Euler(-90, 0, 0));
		SoundEffect.Play2D(m_clip[2]);
		m_magicNumberBomb++;
	}

	private void SkeltonSpawnAnimation() // スケルトンスポーンのアニメーション
	{
		m_animator.SetTrigger("SkeltonSpawn");
		for (int i = 0; i < m_skeltonpos.Length; i++)
		{
			Instantiate(m_effect[2], m_skeltonpos[i].transform.position, Quaternion.identity);
		}
		SoundEffect.Play2D(m_clip[6]);
		m_bossAttackPattern++;
		m_idleTime = CurseTime;
	}

	public void SummonMob() //骸骨スポーン
	{
		for (int i = 0; i < m_skeltonpos.Length; i++)
		{
			m_skelton.GetComponent<SkeletonMove>().SetPlayer(m_player);
			Instantiate(m_skelton, m_skeltonpos[i].transform.position, Quaternion.identity);
		}
		SoundEffect.Play2D(m_clip[3]);
	}


	private void CurseAttackAnimation() // 呪いの攻撃アニメーション
	{
		m_animator.SetTrigger("Curse");
		m_bossAttackPattern++;
	}

	public void CurseAttack() // 呪いの攻撃
	{
		for (int i = 0; i < m_cursePos.Length; i++)
		{
			Instantiate(m_curseEffect, m_cursePos[i].transform.position, Quaternion.identity);
		}
		SoundEffect.Play2D(m_clip[4]);
		StartCoroutine(Curse());
		m_idleTime = 10f;
	}

	private IEnumerator Curse()
	{
		yield return new WaitForSeconds(4);

		m_curseDrawFlg = true;

		for (int i = 0; i < m_skeltonHead.Length; i++)
		{
			m_skeltonHead[i].SetActive(true);
		}
		SoundEffect.Play2D(m_clip[5]);
		m_bossAttackPattern = 0;
	}

	private void CurseDrawTime(float curse)
	{
		

		curse = m_curseDrawTime;

		if(curse <= 0)
		{
			for(int i = 0; i <m_skeltonHead.Length; i++)
			{
				m_skeltonHead[i].SetActive(false);
			}


			SoundEffect.Play2D(m_clip[7]);

			m_curseDrawTime = MaxCurseTime;

			m_curseDrawFlg = false;
		}
	}
}
