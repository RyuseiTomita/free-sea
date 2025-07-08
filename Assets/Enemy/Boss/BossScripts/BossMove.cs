using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class BossMove : MonoBehaviour
{
	private enum TextType
	{
		MagicAttack,
		Skeleton,
		Cruse,
		SickleAttack,
		Shield,
		AreaDamage,
	}

	private enum SoundType
	{
		BossMagicAttack,
		MagicAttackChage,
		MagicAttackExplosion,
		SkeletonSpawnChage,
		SkeletonSpawn,
		CurseAttackChage,
		CurseAttack,
		CurseAttackEnd,
		ShieldChage,
		Shield,
		ShieldBreak,
		SickleChage,
		SickleAttack,
		AreaDamageChage,
		AreaDamageGhost,
	}

	private enum EffectType
	{
		MagicAttackCircle,		
		MagicAttackExplosion,	
		SkeletonSpawn,			
		CurseEnd,
		AwakeningBossCircle,
		SickleAttack,
		SickleAttackSlash,
	}

	private enum AttackPatternType
	{
		MagicAttack,
		SkeltonSpawn,
		CurseAttack,
		SickleAttack,
		AreaDamage,
	}

	// BossのHp
	[SerializeField] int m_bossHealth;
	private bool m_isDeath;

	[SerializeField] Collider m_bossCollider; //Bossの当たり判定
	[SerializeField] Slider slider; // HP
	[SerializeField] Transform m_lookPlayer; // プレイヤーに追従
	[SerializeField] GameObject m_player;    
	[SerializeField] GameObject[] m_effect;
	[SerializeField] AudioClip[] m_clip;

	// 魔法攻撃
	[SerializeField] float m_magicCoolDown; // 間隔
	private bool m_magicAttack;

	// 弾の数
	[SerializeField] int m_magicNumber; // 魔法陣の順番
	private int m_magicNumberBomb;      // 爆弾の順番
	private int MaxMagicNumber = 6;     // 魔法陣の数
	private int MaxAwakeingMagicAttack = 8; // 覚醒時の魔法陣の数
	private GameObject[] m_magicAttackEffect = new GameObject[10];

	//　骸骨の召喚
	[SerializeField] GameObject m_skelton;　　　　　
	[SerializeField] Transform[] m_skeltonpos;   // 骸骨のポジション

	// 覚醒状態の時のスケルトンと幽霊のPos
	[SerializeField] GameObject m_awakeningSkelton;　　// 金色の骸骨
	[SerializeField] GameObject m_awakengGhost;        // 爆弾幽霊
	[SerializeField] Transform[] m_awakeningSkeltonPos;  
	[SerializeField] Transform[] m_awakeningGhostPos;
	private int ghostSpawnPattern;

	// 呪いの攻撃(2パターン)
	[SerializeField] GameObject[] m_skeltonHead; // 骸骨の頭
	[SerializeField] Transform[] m_cursePos;     // 毒エリアのポジション
	[SerializeField] Transform[] m_cursePos2;
	private static int m_curse;

	// シールド
	[SerializeField] GameObject m_shield;
	[SerializeField] Collider m_shieldcollider;
	private bool m_isShield;
	private bool m_canShield;
	private bool m_nowShield;

	// 呪いの攻撃のステータス
	[SerializeField] GameObject m_curseEffect;

	// 技名テキスト
	[SerializeField] GameObject[] m_texts;

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
	private const float SickleAttackTime = 2f;
	private const float AreaDamageTime = 3f;

	// 鎌の攻撃(近接)
	[SerializeField] float m_takeStandTime; // 溜め時間
	[SerializeField] Collider m_sickleArea;
	[SerializeField] GameObject m_sickDrawGauge;
 	private bool m_sickleChage;
	private bool m_sickleAttack;

	// エリアダメージ
	[SerializeField] GameObject[] m_areaDamage;		// 紫のエリア
	[SerializeField] GameObject[] m_areaDamageEffect; // 幽霊
	[SerializeField] Collider[] m_areaDamageCollider; // 当たり判定
	[SerializeField] GameObject m_areaDamageGauge;
	private int m_areaDamagePattern;

	// 覚醒モード
	[SerializeField] GameObject[] m_grave; // 墓の数
	[SerializeField] GameObject m_sickle;  // 鎌の武器
	private bool m_awakeningMode; // 敵が覚醒中か
	private int m_graveCount;

	[SerializeField] GameObject gameManager;
	[SerializeField] GameObject playerWin;


	private Animator m_animator;

	void Start()
	{

		m_animator = GetComponent<Animator>();

		m_onMove = true;

		m_idleTime = 5f;

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
					case 3:
					{
						SickleAttackAnimation();
						m_onAttack = true;
						break;
					}
					case 4:
					{
						AreaDamage();
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
			m_sickle.SetActive(false);
			m_sickDrawGauge.SetActive(false);
			m_areaDamageGauge.SetActive(false);
			OnDeath();

			for(int i = 0; i < m_texts.Length; i++)
			{
				m_texts[i].SetActive(false);
			}

			for(int i = 0; i < m_effect.Length; i++)
			{
				m_effect[i].SetActive(false);
			}
		}
	}


	private void OnMagicAttackTime()
	{
		m_texts[(int)TextType.MagicAttack].SetActive(true); //[ソウル・インフェルノ]

		m_idleTime = m_awakeningMode ? SickleAttackTime : SkeletonTime;

		m_animator.SetTrigger("MagicAttack");
	}

	public void OnMagicAttackAnimation()
	{
		m_magicAttack = true;
	}

	public void AnimationSound()
	{
		SoundEffect.Play2D(m_clip[(int)SoundType.BossMagicAttack]);
	}

	private void OnMagicAttack()
	{
		m_onMove = true;

		if(m_awakeningMode)
		{
			MaxMagicNumber = MaxAwakeingMagicAttack;
		}

		if (m_magicNumber >= MaxMagicNumber && m_magicAttack) // マジック攻撃が終わったら
		{
			if (!m_nowShield && m_awakeningMode)
			{
				m_bossAttackPattern = (int)AttackPatternType.AreaDamage;
			}
			
			m_magicNumber = 0;
			m_magicAttack = false;
			m_magicCoolDown = 0f;
			m_onAttack = false;
			m_texts[(int)TextType.MagicAttack].SetActive(false);

			if (!m_nowShield && m_awakeningMode) return;
			m_bossAttackPattern = (int)AttackPatternType.SkeltonSpawn;
		}
		else
		{
			m_magicCoolDown -= Time.deltaTime;

			if (m_magicCoolDown <= 0)
			{
				if (m_magicNumber == 0) { m_magicNumberBomb = 0; }
				GameObject m_effectBomb = Instantiate(m_effect[(int)EffectType.MagicAttackCircle], m_player.transform.position, Quaternion.identity);
				m_magicAttackEffect[m_magicNumber] = m_effectBomb;
				SoundEffect.Play2D(m_clip[(int)SoundType.MagicAttackChage]);
				m_magicNumber++;
				m_magicCoolDown = 1f;
				StartCoroutine(MagicAttackBomb());
			}
		}
	}

	private IEnumerator MagicAttackBomb()
	{
		yield return new WaitForSeconds(2);
		Instantiate(m_effect[(int)EffectType.MagicAttackExplosion], m_magicAttackEffect[m_magicNumberBomb].transform.position, Quaternion.Euler(-90, 0, 0));
		SoundEffect.Play2D(m_clip[(int)SoundType.MagicAttackExplosion]);
		m_magicNumberBomb++;
	}

	private void SkeltonSpawnAnimation() // スケルトンスポーンのアニメーション
	{
		m_texts[(int)TextType.Skeleton].SetActive(true); // [デスコール]

		m_animator.SetTrigger("SkeltonSpawn");

		if(m_awakeningMode)
		{
			ghostSpawnPattern = UnityEngine.Random.Range(0, 3);

			for (int i = 0; i < m_awakeningSkeltonPos.Length; i++)
			{
				Instantiate(m_effect[(int)EffectType.SkeletonSpawn], m_awakeningSkeltonPos[i].transform.position, Quaternion.identity);
			}

			Instantiate(m_effect[(int)EffectType.SkeletonSpawn], m_awakeningGhostPos[ghostSpawnPattern].transform.position, Quaternion.identity);
			
		}
		else
		{
			for (int i = 0; i < m_skeltonpos.Length; i++)
			{
				Instantiate(m_effect[(int)EffectType.SkeletonSpawn], m_skeltonpos[i].transform.position, Quaternion.identity);
			}
		}
			SoundEffect.Play2D(m_clip[(int)SoundType.SkeletonSpawn]);

		m_idleTime = CurseTime;
		
	}

	public void SummonMob() //骸骨スポーン
	{
		m_texts[(int)TextType.Skeleton].SetActive(false);
		m_onMove = true;
		m_onAttack = false;

		// 第二フェーズになったら
		if (m_awakeningMode)
		{
			AwakeingEnemy();
		}
		else
		{
			// スケルトンを召喚
			for (int i = 0; i < m_skeltonpos.Length; i++)
			{
				m_skelton.GetComponent<SkeletonMove>().SetPlayer(m_player);
				Instantiate(m_skelton, m_skeltonpos[i].transform.position, Quaternion.identity);
			}
		}

		// 次の攻撃パターンへ移動
		m_bossAttackPattern = (int)AttackPatternType.CurseAttack;

		SoundEffect.Play2D(m_clip[(int)SoundType.SkeletonSpawnChage]);
	}

	private void AwakeingEnemy() // 覚醒中の時のスケルトンと幽霊
	{
		// 覚醒した骸骨を召喚
		for (int i = 0; i < m_awakeningSkeltonPos.Length; i++)
		{
			m_awakeningSkelton.GetComponent<SkeletonMove>().SetPlayer(m_player);
			Instantiate(m_awakeningSkelton, m_awakeningSkeltonPos[i].transform.position, Quaternion.identity);
		}

		//ゴーストを召喚 
		m_awakengGhost.GetComponent<GhostMove>().SetPlayer(m_player);
		Instantiate(m_awakengGhost, m_awakeningGhostPos[ghostSpawnPattern].transform.position, Quaternion.identity);
	}


	private void CurseAttackAnimation() // 呪いの攻撃アニメーション
	{
		m_texts[(int)TextType.Cruse].SetActive(true); // [カース・ネクロフィールド]

		m_animator.SetTrigger("Curse");
		m_idleTime = m_awakeningMode ? SkeletonTime : MagicAttackTime;

		if(!m_awakeningMode)
		{
			m_bossAttackPattern = (int)AttackPatternType.MagicAttack;
		}
		else
		{
			m_bossAttackPattern = (int)AttackPatternType.SickleAttack;
		}
	}

	public void CurseAttack() // 呪いの攻撃
	{
		m_onMove = true;
		m_curse = UnityEngine.Random.Range(0, 2);

		Transform[] cursePos = m_curse == 0 ? m_cursePos : m_cursePos2;

		for (int i = 0; i < m_cursePos.Length; i++)
		{
			Instantiate(m_curseEffect, cursePos[i].transform.position, Quaternion.identity);
		}

		m_onAttack = false;
		SoundEffect.Play2D(m_clip[(int)SoundType.CurseAttackChage]);
		StartCoroutine(Curse());
	}

	private IEnumerator Curse()
	{
		yield return new WaitForSeconds(4);

		m_texts[(int)TextType.Cruse].SetActive(false);

		if (m_awakeningMode)
		{
			m_skeltonHead[0] = m_skeltonHead[1];
		}

		Transform[] cursePos = m_curse == 0 ? m_cursePos : m_cursePos2;

		for (int i = 0; i < m_cursePos.Length; i++)
		{
			m_skeltonHead[0].GetComponent<CurseSkeletonHead>().SetPlayer(m_lookPlayer);
			m_skeltonHead[0].GetComponent<BossCruseDamage>().SetPlayer(m_player);
			Instantiate(m_skeltonHead[0], cursePos[i].transform.position, Quaternion.identity);
		}

		SoundEffect.Play2D(m_clip[(int)SoundType.CurseAttack]);
	}

	// 覚醒状態の時に発動する鎌の攻撃(近接)
	private void SickleAttackAnimation()
	{
		if (m_sickleAttack) return;
		m_takeStandTime += Time.deltaTime;
		m_sickDrawGauge.GetComponent<SickAreaGauge>().SickAttackDrawTime(true);
		if (m_takeStandTime >= 5f)
		{
			m_sickleAttack = true;
			m_takeStandTime = 0;
			m_animator.SetTrigger("SickleAttack");
			m_texts[(int)TextType.SickleAttack].SetActive(false);
		}

		if (m_sickleChage) return;
		m_animator.SetTrigger("TakeStand");
		m_texts[(int)TextType.SickleAttack].SetActive(true); // [グリムサイズ・エクスキューション]
		m_effect[(int)EffectType.SickleAttack].SetActive(true);
		SoundEffect.Play2D(m_clip[(int)SoundType.SickleChage]);
		m_sickDrawGauge.SetActive(true);
		m_sickleChage = true;
	}

	public void BossSlash()
	{
		m_effect[(int)EffectType.SickleAttackSlash].SetActive(true);
		m_effect[(int)EffectType.SickleAttack].SetActive(false);
		m_sickleArea.enabled = true;
	}

	public void BossAttackSound()
	{
		SoundEffect.Play2D(m_clip[(int)SoundType.SickleAttack]);
	}

	public void SickleAttackColliderEnd()
	{
		m_sickleArea.enabled = false;
	}

	public void SickleAttackEnd()
	{
		m_bossAttackPattern = (int)AttackPatternType.MagicAttack;		
		m_onMove = true;
		m_onAttack = false;
		m_idleTime = CurseTime;
		m_sickleChage = false;
		m_sickleAttack = false;
		m_effect[(int)EffectType.SickleAttackSlash].SetActive(false);
	}

	private void AreaDamage() // 一部ダメージ食らうエリア
	{
		m_idleTime = MagicAttackTime;
		m_bossAttackPattern = (int)AttackPatternType.SkeltonSpawn;

		if (m_nowShield) return;

		m_animator.SetTrigger("AreaDamage");
		m_texts[(int)TextType.AreaDamage].SetActive(true);
		m_areaDamageGauge.SetActive(true);
		SoundEffect.Play2D(m_clip[(int)SoundType.AreaDamageChage]);

		m_areaDamagePattern = UnityEngine.Random.Range(0, 4);

		m_areaDamage[m_areaDamagePattern].SetActive(true);
		StartCoroutine(AreaDamageEffect());
	}

	public void AreaDamageEnd()
	{
		m_onMove = true;
		m_onAttack = false;
	}

	private IEnumerator AreaDamageEffect()
	{
		yield return new WaitForSeconds(10);

		m_areaDamage[m_areaDamagePattern].SetActive(false);
		m_areaDamageEffect[m_areaDamagePattern].SetActive(true);
		m_areaDamageCollider[m_areaDamagePattern].enabled = true;
		SoundEffect.Play2D(m_clip[(int)SoundType.AreaDamageGhost]);
		m_areaDamageGauge.SetActive(false);
		m_texts[(int)TextType.AreaDamage].SetActive(false);
		StartCoroutine(AreaDamageEffectEnd());
	}

	private IEnumerator AreaDamageEffectEnd()
	{
		yield return new WaitForSeconds(2);

		// AreaDamageを非表示
		m_areaDamageEffect[m_areaDamagePattern].SetActive(false);
		m_areaDamageCollider[m_areaDamagePattern].enabled = false;
	}

	public void ShieldSound()
	{
		SoundEffect.Play2D(m_clip[(int)SoundType.ShieldChage]);
	}

	public void ShieldAnimation()
	{
		m_texts[(int)TextType.Shield].SetActive(true); // [トゥーム・シールド]

		for (int i = 0; i < m_grave.Length; i++)
		{
			m_grave[i].SetActive(true);
		}
		m_effect[(int)EffectType.AwakeningBossCircle].SetActive(true);
		m_awakeningMode = true;
		m_nowShield = true;

		m_bossCollider.enabled = false;
		m_shieldcollider.enabled = true;
		
		m_shield.SetActive(true);
		m_sickle.SetActive(true);
		gameManager.GetComponent<GameManager>().AwakeingCircle(true);
		gameManager.GetComponent<GameManager>().BgmChange();
		SoundEffect.Play2D(m_clip[(int)SoundType.Shield]);

		if(m_bossAttackPattern == (int)AttackPatternType.MagicAttack)
		{
			Debug.Log("MAGIC");
			m_bossAttackPattern = (int)AttackPatternType.SkeltonSpawn;
			m_magicNumber = 0;
			m_magicAttack = false;
			m_magicCoolDown = 0f;
			m_onAttack = false;
			m_texts[(int)TextType.MagicAttack].SetActive(false);
		}
	}

	public void ShieldAnimationEnd()
	{
		m_animator.SetBool("TakeStand", false);
		m_canShield = false;
		m_onMove = true;
		m_texts[(int)TextType.Shield].SetActive(false);
	}

	// バリアを壊す
	public void ShieldBreak()
	{
		m_graveCount++;
		if (m_graveCount >= 4)
		{
			SoundEffect.Play2D(m_clip[(int)SoundType.ShieldBreak]);
			m_bossCollider.enabled = true;
			m_shieldcollider.enabled = false;
			m_shield.SetActive(false);
			m_nowShield = false;
		}
	}

	// ゲームセット(playerが負けたら)
	public void GameSet(bool gameSet)
	{
		m_gameSet = gameSet;
		m_animator.SetBool("Walk", false);
	}

	private void OnDeath()
	{
		if (m_gameSet) return;
		playerWin.GetComponent<GameSetWin>().PlayerWin(true);
		gameManager.GetComponent<GameManager>().AwakeingCircle(false);
	}
}
