using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class BossMove : MonoBehaviour
{
	// BossのHp
	[SerializeField] int m_bossHealth;
	private bool m_isDeath;

	[SerializeField] GameObject[] m_model;
	[SerializeField] Collider m_bossCollider;
	[SerializeField] Slider slider; // HP
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
	private int MaxMagicNumber = 6;
	private GameObject[] m_magicAttackEffect = new GameObject[10];

	//　骸骨の召喚
	[SerializeField] GameObject m_skelton;
	[SerializeField] Transform[] m_skeltonpos;   // 骸骨のポジション

	// 覚醒状態の時のスケルトンPos
	[SerializeField] GameObject[] m_awakeningSkelton;
	[SerializeField] Transform[] m_awakeningSkeltonPos;
	[SerializeField] Transform[] m_awakeningSkeltonPosEnd;
	//private bool m_skeltonSpawn;

	// 呪いの攻撃(2パターン)
	[SerializeField] GameObject[] m_skeltonHead; // 骸骨の頭
	[SerializeField] Transform[] m_cursePos;
	[SerializeField] Transform[] m_cursePos2;
	private static int m_curse;

	// シールド
	[SerializeField] GameObject m_shield;
	[SerializeField] Collider m_shieldcollider;
	private bool m_isShield;
	private bool m_canShield;
	//private float m_shieldTime;
 

	// 呪いの攻撃のステータス
	[SerializeField] GameObject m_curseEffect;


	//private GameObject m_curse;

	[SerializeField] float m_idleTime; // 何もしない時間
	private float m_speed = 2;
	private bool m_onMove; // 敵が動いているか
	private bool m_onAttack = false;
	private bool m_gameSet;

	// 攻撃パターン
	[SerializeField] int m_bossAttackPattern = 0;
	private const float MagicAttackTime = 3f;
	private const float SkeletonTime = 5f;
	private const float CurseTime = 3f;


	// 鎌の攻撃(近接)
	[SerializeField] const float SickleAttackTime = 2f;
	[SerializeField] float m_takeStandTime; // 溜め時間
	[SerializeField] Collider m_sickleArea;
	private bool m_sickleChage;
	private bool m_sickleAttack;

	// 覚醒モード
	[SerializeField] GameObject[] m_grave; // 墓の数
	[SerializeField] GameObject m_sickle;  // 鎌の武器
	private bool m_awakeningMode; // 敵が覚醒中か
	private int m_graveCount;

	[SerializeField] GameObject gameManager;

	private Animator m_animator;

	void Start()
	{
		m_onMove = true;

		m_idleTime = 5f;

		m_animator = m_model[0].GetComponent<Animator>();
		m_magicNumber = 0;
		m_magicNumberBomb = 0;

		m_magicCoolDown = 0;
	
		m_magicAttack = false;
		m_isShield = false;
		m_canShield = false;

		m_isDeath = false;
		m_awakeningMode = false;
		m_sickleAttack = false;
		m_sickleChage = false;
		m_gameSet = false;
		//m_skeltonSpawn = false;
		//m_isMove = false;

		m_graveCount = 0;
		m_takeStandTime = 0;
	}


	void FixedUpdate()
	{
		slider.value = m_bossHealth;

		if (m_isDeath || m_canShield || m_gameSet) return;

		if (m_idleTime <= 0)
		{
			m_onMove = false;
		
			if(!m_awakeningMode)
			{ 
				switch (m_bossAttackPattern)
				{
					case 0:
					{
						OnMagicAttackTime();
						m_onAttack = true;
						break;
					}
					case 1:
					{
						SkeltonSpawnAnimation();
						m_onAttack = true;
						break;
					}
					case 2:
					{
						CurseAttackAnimation();
						m_onAttack = true;
						break;
					}
				}
			}
			else
			{
				switch (m_bossAttackPattern)
				{
					case 0:
					{
						OnMagicAttackTime();
						m_onAttack = true;
						break;
					}
					case 1:
					{
						SickleAttackAnimation();
						m_onAttack = true;
						break;
					}
					case 2:
					{
						CurseAttackAnimation();
						m_onAttack = true;
						break;
					}
					case 3:
					{
						SkeltonSpawnAnimation();
						m_onAttack = true;
						break;
					}
				}
			}
		}

		if (m_magicAttack)
		{
			OnMagicAttack();
		}

		if (!m_onAttack)
		{
			m_idleTime -= Time.deltaTime;
		}

		if (!m_onMove) return;

		Vector3 forward = m_lookPlayer.position - transform.position;
		forward.Scale(new Vector3(1, 0, 1));
		transform.rotation = Quaternion.LookRotation(forward.normalized);

		// プレイヤーに向けて移動
		bool isMove = false;
		if ((m_lookPlayer.position - transform.position).magnitude > 3)
		{
			transform.position += transform.forward * m_speed * Time.deltaTime;
			isMove = true;
		}

		m_animator.SetBool("Walk", isMove);
	}

	// プレイヤーからダメージを食らう
	public void HitAttack(int hit)
	{	
		m_bossHealth -= hit;

		if (m_bossHealth <= 270 && !m_isShield) // HPが半分を切ったら覚醒モード
		{
			m_isShield = true;
			m_canShield = true;
			m_onMove = false;
			m_idleTime = 5f;
			m_animator.SetTrigger("Shield");
		}
		else if(m_bossHealth <= 0) // HPがなくなったら死ぬ
		{
			m_isDeath = true;
			m_player.GetComponent<PlayerMove>().GameSet();
			m_animator.SetTrigger("Death");
			m_effect[4].SetActive(false);
			m_effect[5].SetActive(false);
			m_effect[6].SetActive(false);
			m_sickle.SetActive(false);
		}
	}


	private void OnMagicAttackTime()
	{
		if(!m_awakeningMode)
		{
			m_idleTime = SkeletonTime;
		}
		else
		{
			m_idleTime = SickleAttackTime;
		}
		m_animator.SetTrigger("MagicAttack");
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
		m_onMove = true;

		if(m_awakeningMode)
		{
			MaxMagicNumber = 8;
		}

		if (m_magicNumber >= MaxMagicNumber && m_magicAttack) // マジック攻撃が終わったら
		{
			m_magicNumber = 0;
			m_magicAttack = false;
			m_bossAttackPattern++;
			m_magicCoolDown = 0f;
			m_onAttack = false;
		}
		else
		{
			m_magicCoolDown -= Time.deltaTime;

			if (m_magicCoolDown <= 0)
			{
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

		if(m_awakeningMode)
		{
			for (int i = 0; i < m_awakeningSkeltonPos.Length; i++)
			{
				Instantiate(m_effect[2], m_awakeningSkeltonPos[i].transform.position, Quaternion.identity);
			}

			for (int i = 0; i < m_awakeningSkeltonPosEnd.Length; i++)
			{
				Instantiate(m_effect[2], m_awakeningSkeltonPosEnd[i].transform.position, Quaternion.identity);
			}
		}
		else
		{
			for (int i = 0; i < m_skeltonpos.Length; i++)
			{
				Instantiate(m_effect[2], m_skeltonpos[i].transform.position, Quaternion.identity);
			}
		}
			SoundEffect.Play2D(m_clip[6]);

		m_idleTime = CurseTime;
		
	}

	public void SummonMob() //骸骨スポーン
	{
		m_onMove = true;
		m_onAttack = false;


		if (m_awakeningMode)
		{
			m_awakeningMoveSkeleton();
			m_bossAttackPattern = 0;
		}
		else
		{
			for (int i = 0; i < m_skeltonpos.Length; i++)
			{
				m_skelton.GetComponent<SkeletonMove>().SetPlayer(m_player);
				Instantiate(m_skelton, m_skeltonpos[i].transform.position, Quaternion.identity);
			}
			m_bossAttackPattern++;
		}

		SoundEffect.Play2D(m_clip[3]);
	}

	private void m_awakeningMoveSkeleton() // 覚醒中の時のスケルトン生成
	{
		for (int i = 0; i < m_awakeningSkeltonPos.Length; i++)
		{
			m_awakeningSkelton[0].GetComponent<SkeletonMove>().SetPlayer(m_player);
			Instantiate(m_awakeningSkelton[0], m_awakeningSkeltonPos[i].transform.position, Quaternion.identity);
		}

		for(int i = 0; i < m_awakeningSkeltonPosEnd.Length; i++)
		{
			m_awakeningSkelton[1].GetComponent<AwakeningSkeltonEndMove>().SetPlayer(m_player);
			Instantiate(m_awakeningSkelton[1], m_awakeningSkeltonPosEnd[i].transform.position, Quaternion.identity);
		}
	}


	private void CurseAttackAnimation() // 呪いの攻撃アニメーション
	{
		m_animator.SetTrigger("Curse");
		m_bossAttackPattern++;

		if(!m_awakeningMode)
		{
			m_idleTime = MagicAttackTime;
		}
		else
		{
			m_idleTime = SkeletonTime;
		}
		
	}

	public void CurseAttack() // 呪いの攻撃
	{
		m_onMove = true;
		m_curse = UnityEngine.Random.Range(0, 2);

		switch (m_curse)
		{
			case 0:
			{
				for (int i = 0; i < m_cursePos.Length; i++)
				{
					Instantiate(m_curseEffect, m_cursePos[i].transform.position, Quaternion.identity);
				}
				break;
			}	
			case 1:
			{
				for (int i = 0; i < m_cursePos2.Length; i++)
				{
					Instantiate(m_curseEffect, m_cursePos2[i].transform.position, Quaternion.identity);
				}
				break;
			}
		}
		m_onAttack = false;
		SoundEffect.Play2D(m_clip[4]);
		StartCoroutine(Curse());
	}

	private IEnumerator Curse()
	{
		yield return new WaitForSeconds(4);

		if (m_awakeningMode)
		{
			m_skeltonHead[0] = m_skeltonHead[1];
		}

			switch (m_curse)
			{
				case 0:
					{
						for (int i = 0; i < m_cursePos.Length; i++)
						{
							m_skeltonHead[0].GetComponent<CurseSkeletonHead>().SetPlayer(m_lookPlayer);
							m_skeltonHead[0].GetComponent<BossCruseDamage>().SetPlayer(m_player);
							Instantiate(m_skeltonHead[0], m_cursePos[i].transform.position, Quaternion.identity);
						}
						break;
					}
				case 1:
					{
						for (int i = 0; i < m_cursePos.Length; i++)
						{
							m_skeltonHead[0].GetComponent<CurseSkeletonHead>().SetPlayer(m_lookPlayer);
							m_skeltonHead[0].GetComponent<BossCruseDamage>().SetPlayer(m_player);
							Instantiate(m_skeltonHead[0], m_cursePos2[i].transform.position, Quaternion.identity);
						}
						break;
					}
			
			}
		SoundEffect.Play2D(m_clip[5]);

		if(!m_awakeningMode)
		{
			m_bossAttackPattern = 0;
		}
	}

	// 覚醒状態の時に発動する鎌の攻撃(近接)
	private void SickleAttackAnimation()
	{
		if (m_sickleAttack) return;
		m_takeStandTime += Time.deltaTime;

		if (m_takeStandTime >= 5f)
		{
			m_sickleAttack = true;
			m_takeStandTime = 0;
			m_animator.SetTrigger("SickleAttack");
		}

		if (m_sickleChage) return;
		m_animator.SetTrigger("TakeStand");
		SoundEffect.Play2D(m_clip[12]);
		m_sickleChage = true;
		m_effect[6].SetActive(true);
	}

	public void BossSlash()
	{
		m_effect[7].SetActive(true);
		m_effect[6].SetActive(false);
		m_sickleArea.enabled = true;
	}

	public void BossAttackSound()
	{
		SoundEffect.Play2D(m_clip[11]);
	}

	public void SickleAttackColliderEnd()
	{
		m_sickleArea.enabled = false;
	}

	public void SickleAttackEnd()
	{
		m_bossAttackPattern++;
		m_onMove = true;
		m_onAttack = false;
		m_idleTime = CurseTime;
		m_sickleChage = false;
		m_sickleAttack = false;
		m_effect[7].SetActive(false);
	}

	public void ShieldSound()
	{
		SoundEffect.Play2D(m_clip[8]);
	}

	public void ShieldAnimation()
	{
		for(int i = 0; i < 4; i++)
		{
			m_grave[i].SetActive(true);
		}
		m_effect[4].SetActive(true);
		m_effect[5].SetActive(true);
		m_awakeningMode = true;
		m_shield.SetActive(true);
		m_sickle.SetActive(true);
		SoundEffect.Play2D(m_clip[9]);
		m_bossCollider.enabled = false;
		m_shieldcollider.enabled = true;
		gameManager.GetComponent<GameManager>().BgmChange();

		if(m_bossAttackPattern == 0)
		{
			m_bossAttackPattern++;
			m_magicNumber = 0;
			m_magicAttack = false;
			m_magicCoolDown = 0f;
			m_onAttack = false;
		}
	}

	public void ShieldAnimationEnd()
	{
		m_animator.SetBool("TakeStand", false);
		m_canShield = false;
		m_onMove = true;
	}

	// バリアを壊す
	public void ShieldBreak()
	{
		m_graveCount++;
		if (m_graveCount >= 4)
		{
			SoundEffect.Play2D(m_clip[10]);
			m_bossCollider.enabled = true;
			m_shieldcollider.enabled = false;
			m_shield.SetActive(false);
		}
	}

	// ゲームセット(playerが負けたら)
	public void GameSet(bool gameSet)
	{
		m_gameSet = gameSet;
		m_animator.SetBool("Walk", false);
	}
}
