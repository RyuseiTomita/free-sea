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

	// Boss��Hp
	[SerializeField] int m_bossHealth;
	private bool m_isDeath;

	[SerializeField] Collider m_bossCollider; //Boss�̓����蔻��
	[SerializeField] Slider slider; // HP
	[SerializeField] Transform m_lookPlayer; // �v���C���[�ɒǏ]
	[SerializeField] GameObject m_player;    
	[SerializeField] GameObject[] m_effect;
	[SerializeField] AudioClip[] m_clip;

	// ���@�U��
	[SerializeField] float m_magicCoolDown; // �Ԋu
	private bool m_magicAttack;

	// �e�̐�
	[SerializeField] int m_magicNumber; // ���@�w�̏���
	private int m_magicNumberBomb;      // ���e�̏���
	private int MaxMagicNumber = 6;     // ���@�w�̐�
	private int MaxAwakeingMagicAttack = 8; // �o�����̖��@�w�̐�
	private GameObject[] m_magicAttackEffect = new GameObject[10];

	//�@�[���̏���
	[SerializeField] GameObject m_skelton;�@�@�@�@�@
	[SerializeField] Transform[] m_skeltonpos;   // �[���̃|�W�V����

	// �o����Ԃ̎��̃X�P���g���ƗH���Pos
	[SerializeField] GameObject m_awakeningSkelton;�@�@// ���F�̊[��
	[SerializeField] GameObject m_awakengGhost;        // ���e�H��
	[SerializeField] Transform[] m_awakeningSkeltonPos;  
	[SerializeField] Transform[] m_awakeningGhostPos;
	private int ghostSpawnPattern;

	// �􂢂̍U��(2�p�^�[��)
	[SerializeField] GameObject[] m_skeltonHead; // �[���̓�
	[SerializeField] Transform[] m_cursePos;     // �ŃG���A�̃|�W�V����
	[SerializeField] Transform[] m_cursePos2;
	private static int m_curse;

	// �V�[���h
	[SerializeField] GameObject m_shield;
	[SerializeField] Collider m_shieldcollider;
	private bool m_isShield;
	private bool m_canShield;
	private bool m_nowShield;

	// �􂢂̍U���̃X�e�[�^�X
	[SerializeField] GameObject m_curseEffect;

	// �Z���e�L�X�g
	[SerializeField] GameObject[] m_texts;

	[SerializeField] float m_idleTime; // �������Ȃ�����
	private float m_speed = 2;
	private bool m_onMove; // �G�������Ă��邩
	private bool m_onAttack = false;
	private bool m_gameSet;

	// �U���p�^�[��
	[SerializeField] int m_bossAttackPattern = 0;
	private const float MagicAttackTime = 3f;
	private const float SkeletonTime = 5f;
	private const float CurseTime = 3f;
	private const float SickleAttackTime = 2f;
	private const float AreaDamageTime = 3f;

	// ���̍U��(�ߐ�)
	[SerializeField] float m_takeStandTime; // ���ߎ���
	[SerializeField] Collider m_sickleArea;
	[SerializeField] GameObject m_sickDrawGauge;
 	private bool m_sickleChage;
	private bool m_sickleAttack;

	// �G���A�_���[�W
	[SerializeField] GameObject[] m_areaDamage;		// ���̃G���A
	[SerializeField] GameObject[] m_areaDamageEffect; // �H��
	[SerializeField] Collider[] m_areaDamageCollider; // �����蔻��
	[SerializeField] GameObject m_areaDamageGauge;
	private int m_areaDamagePattern;

	// �o�����[�h
	[SerializeField] GameObject[] m_grave; // ��̐�
	[SerializeField] GameObject m_sickle;  // ���̕���
	private bool m_awakeningMode; // �G���o������
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

		// �v���C���[�Ɍ����Ĉړ�
		bool isMove = false;
		if ((m_lookPlayer.position - transform.position).magnitude > 3)
		{
			transform.position += transform.forward * m_speed * Time.deltaTime;
			isMove = true;
		}

		m_animator.SetBool("Walk", isMove);
	}

	// �v���C���[����_���[�W��H�炤
	public void HitAttack(int hit)
	{	
		m_bossHealth -= hit;

		if (m_bossHealth <= 270 && !m_isShield) // HP��������؂�����o�����[�h
		{
			m_isShield = true;
			m_canShield = true;
			m_onMove = false;
			m_idleTime = 5f;
			m_animator.SetTrigger("Shield");
		}
		else if(m_bossHealth <= 0) // HP���Ȃ��Ȃ����玀��
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
		m_texts[(int)TextType.MagicAttack].SetActive(true); //[�\�E���E�C���t�F���m]

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

		if (m_magicNumber >= MaxMagicNumber && m_magicAttack) // �}�W�b�N�U�����I�������
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

	private void SkeltonSpawnAnimation() // �X�P���g���X�|�[���̃A�j���[�V����
	{
		m_texts[(int)TextType.Skeleton].SetActive(true); // [�f�X�R�[��]

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

	public void SummonMob() //�[���X�|�[��
	{
		m_texts[(int)TextType.Skeleton].SetActive(false);
		m_onMove = true;
		m_onAttack = false;

		// ���t�F�[�Y�ɂȂ�����
		if (m_awakeningMode)
		{
			AwakeingEnemy();
		}
		else
		{
			// �X�P���g��������
			for (int i = 0; i < m_skeltonpos.Length; i++)
			{
				m_skelton.GetComponent<SkeletonMove>().SetPlayer(m_player);
				Instantiate(m_skelton, m_skeltonpos[i].transform.position, Quaternion.identity);
			}
		}

		// ���̍U���p�^�[���ֈړ�
		m_bossAttackPattern = (int)AttackPatternType.CurseAttack;

		SoundEffect.Play2D(m_clip[(int)SoundType.SkeletonSpawnChage]);
	}

	private void AwakeingEnemy() // �o�����̎��̃X�P���g���ƗH��
	{
		// �o�������[��������
		for (int i = 0; i < m_awakeningSkeltonPos.Length; i++)
		{
			m_awakeningSkelton.GetComponent<SkeletonMove>().SetPlayer(m_player);
			Instantiate(m_awakeningSkelton, m_awakeningSkeltonPos[i].transform.position, Quaternion.identity);
		}

		//�S�[�X�g������ 
		m_awakengGhost.GetComponent<GhostMove>().SetPlayer(m_player);
		Instantiate(m_awakengGhost, m_awakeningGhostPos[ghostSpawnPattern].transform.position, Quaternion.identity);
	}


	private void CurseAttackAnimation() // �􂢂̍U���A�j���[�V����
	{
		m_texts[(int)TextType.Cruse].SetActive(true); // [�J�[�X�E�l�N���t�B�[���h]

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

	public void CurseAttack() // �􂢂̍U��
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

	// �o����Ԃ̎��ɔ������銙�̍U��(�ߐ�)
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
		m_texts[(int)TextType.SickleAttack].SetActive(true); // [�O�����T�C�Y�E�G�N�X�L���[�V����]
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

	private void AreaDamage() // �ꕔ�_���[�W�H�炤�G���A
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

		// AreaDamage���\��
		m_areaDamageEffect[m_areaDamagePattern].SetActive(false);
		m_areaDamageCollider[m_areaDamagePattern].enabled = false;
	}

	public void ShieldSound()
	{
		SoundEffect.Play2D(m_clip[(int)SoundType.ShieldChage]);
	}

	public void ShieldAnimation()
	{
		m_texts[(int)TextType.Shield].SetActive(true); // [�g�D�[���E�V�[���h]

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

	// �o���A����
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

	// �Q�[���Z�b�g(player����������)
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
