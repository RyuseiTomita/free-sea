using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossMove : MonoBehaviour
{
	[SerializeField] GameObject[] m_model;
	[SerializeField] Slider slider; // HP
	[SerializeField] Transform m_lookPlayer; // �v���C���[�ɒǏ]
	[SerializeField] GameObject m_player;
	[SerializeField] GameObject[] m_effect;
	[SerializeField] AudioClip[] m_clip;

	// ���@�U��
	[SerializeField] float m_magicAttackTime;
	//private const float MaxMagicAttackTime = 60f;
	private bool m_magicAttack;
	[SerializeField] float m_magicCoolDown; // �Ԋu

	// �e�̐�
	[SerializeField] int m_magicNumber; // ���@�w�̏���
	private int m_magicNumberBomb;      // ���e�̏���
	private int MaxMagicNumber = 6;
	private GameObject[] m_magicAttackEffect = new GameObject[10];

	// �o�����̎��̒e�̐�

	//�@�[���̏���
	[SerializeField] GameObject m_skelton;
	[SerializeField] Transform[] m_skeltonpos;   // �[���̃|�W�V����

	// �o����Ԃ̎��̃X�P���g��Pos
	[SerializeField] GameObject m_awakeningSkelton;
	[SerializeField] Transform[] m_awakeningSkeltonPos;
	//private bool m_skeltonSpawn;

	// �􂢂̍U��(2�p�^�[��)
	[SerializeField] GameObject m_skeltonHead; // �[���̓�
	[SerializeField] Transform[] m_cursePos;
	[SerializeField] Transform[] m_cursePos2;
	private static int m_curse;

	// �V�[���h
	[SerializeField] GameObject m_shield;
	[SerializeField] Collider m_collider;
	private bool m_isShield;
	private bool m_canShield;
	//private float m_shieldTime;
 
	[SerializeField] GameObject m_curseEffect;
	private float m_curseDrawTime;
	private const float MaxCurseTime = 15f; 
	private bool m_curseDrawFlg;
	//private GameObject m_curse;

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


	// �o����Ԃ̎��̍U���p�^�[��
	[SerializeField] const float SickleAttackTime = 2f;
	[SerializeField] float m_takeStandTime; // ���ߎ���
	private bool m_sickleAttack;

	// �o�����[�h
	[SerializeField] GameObject[] m_grave; // ��̐�
	[SerializeField] GameObject m_sickle;  // ���̕���
	private bool m_awakeningMode; // �G���o������
	private int m_graveCount;

	// Boss��Hp
	[SerializeField] int m_bossHealth = 200;
	private bool m_isDeath;

	[SerializeField] GameObject gameManager;

	private Animator m_animator;

	void Start()
	{
		m_onMove = true;

		//m_shieldTime = 5f;
		m_curseDrawTime = 15f;
		m_curseDrawFlg = false;

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
		m_gameSet = false;
		//m_skeltonSpawn = false;
		//m_isMove = false;

		m_graveCount = 0;
		m_takeStandTime = 0;
	}


	void FixedUpdate()
	{
		slider.value = m_bossHealth;

		//Debug.Log(m_idleTime);

		if (m_isDeath || m_canShield || m_gameSet) return;

		if (m_idleTime <= 0)
		{
			m_onMove = false;
		
			if(!m_awakeningMode)
			{
				Debug.Log("NotAwaken");
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
				Debug.Log("YesAwaken");
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
						Debug.Log(m_bossAttackPattern);
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

		if(m_curseDrawFlg)
		{
			m_curseDrawTime -= Time.deltaTime;

			CurseDrawTime(m_curseDrawTime);
		}

		if (!m_onAttack)
		{
			m_idleTime -= Time.deltaTime;
		}

		if (!m_onMove) return;

		transform.rotation = Quaternion.Lerp(
		   transform.rotation,
		   Quaternion.LookRotation(m_lookPlayer.position - transform.position),
		   0.2f);

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

		if (m_bossHealth <= 75 && !m_isShield) // HP��������؂�����o�����[�h
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
			m_animator.SetTrigger("Death");
			m_effect[4].SetActive(false);
			m_effect[5].SetActive(false);
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

		if (m_magicNumber >= MaxMagicNumber && m_magicAttack) // �}�W�b�N�U�����I�������
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

	private void SkeltonSpawnAnimation() // �X�P���g���X�|�[���̃A�j���[�V����
	{
		m_animator.SetTrigger("SkeltonSpawn");
		if(m_awakeningMode)
		{
			for (int i = 0; i < m_awakeningSkeltonPos.Length; i++)
			{
				Instantiate(m_effect[2], m_awakeningSkeltonPos[i].transform.position, Quaternion.identity);
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

	public void SummonMob() //�[���X�|�[��
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

	private void m_awakeningMoveSkeleton() // �o�����̎��̃X�P���g������
	{
		for (int i = 0; i < m_awakeningSkeltonPos.Length; i++)
		{
			m_awakeningSkelton.GetComponent<SkeletonMove>().SetPlayer(m_player);
			Instantiate(m_awakeningSkelton, m_awakeningSkeltonPos[i].transform.position, Quaternion.identity);
		}
	}


	private void CurseAttackAnimation() // �􂢂̍U���A�j���[�V����
	{
		m_animator.SetTrigger("Curse");
		m_bossAttackPattern++;

		if(!m_awakeningMode)
		{
			Debug.Log("NotawakeningMode");
			m_idleTime = MagicAttackTime;
		}
		else
		{
			Debug.Log("awakeningMode");
			m_idleTime = SkeletonTime;
		}
		
	}

	public void CurseAttack() // �􂢂̍U��
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

		m_curseDrawFlg = true;

		switch (m_curse)
		{
			case 0:
				{
					for (int i = 0; i < m_cursePos.Length; i++)
					{
						m_skeltonHead.GetComponent<CurseSkeletonHead>().SetPlayer(m_lookPlayer);
						m_skeltonHead.GetComponent<BossCruseDamage>().SetPlayer(m_player);
						Instantiate(m_skeltonHead, m_cursePos[i].transform.position, Quaternion.identity);
					}
					break;
				}
			case 1:
				{
					for (int i = 0; i < m_cursePos.Length; i++)
					{
						m_skeltonHead.GetComponent<CurseSkeletonHead>().SetPlayer(m_lookPlayer);
						m_skeltonHead.GetComponent<BossCruseDamage>().SetPlayer(m_player);
						Instantiate(m_skeltonHead, m_cursePos2[i].transform.position, Quaternion.identity);
					}
					break;
				}
		}
		SoundEffect.Play2D(m_clip[5]);

		if(!m_awakeningMode)
		{
			Debug.Log("AAAAAAAa");
			m_bossAttackPattern = 0;
		}
	}

	private void CurseDrawTime(float curse)
	{
		curse = m_curseDrawTime;

		if (curse <= 0)
		{
			switch (m_curse)
			{
				case 0:
					{
						for (int i = 0; i < m_cursePos.Length; i++)
						{
							Instantiate(m_effect[3], m_cursePos[i].transform.position, Quaternion.Euler(-90, 0, 0));
						}
						break;
					}
				case 1:
					{
						for (int i = 0; i < m_cursePos.Length; i++)
						{
							Instantiate(m_effect[3], m_cursePos2[i].transform.position, Quaternion.Euler(-90, 0, 0));
						}
						break;
					}
			}

			SoundEffect.Play2D(m_clip[7]);

			m_curseDrawTime = MaxCurseTime;

			m_curseDrawFlg = false;
		}
	}

	// �o����Ԃ̎��ɔ������銙�̍U��(�ߐ�)
	private void SickleAttackAnimation()
	{
		Debug.Log(m_sickleAttack);

		m_takeStandTime += Time.deltaTime;

		if (m_takeStandTime >= 5f)
		{
			m_animator.SetTrigger("SickleAttack");
		}

		if (m_sickleAttack) return;
		Debug.Log("���ߎ���");
		m_animator.SetTrigger("TakeStand");
		m_sickleAttack = true;
	}

	public void SetNext()
	{
		
	}

	public void BossAttackSound()
	{
		SoundEffect.Play2D(m_clip[11]);
	}

	public void SickleAttackEnd()
	{
		m_takeStandTime = 0;
		m_bossAttackPattern++;
		m_sickleAttack = false;
		m_onMove = true;
		m_onAttack = false;
		m_idleTime = CurseTime;
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
		m_collider.enabled = false;
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

	// �o���A����
	public void ShieldBreak()
	{
		m_graveCount++;
		if (m_graveCount >= 4)
		{
			SoundEffect.Play2D(m_clip[10]);
			m_collider.enabled = true;
			m_shield.SetActive(false);
		}
	}

	// �Q�[���Z�b�g(player����������)
	public void GameSet()
	{
		m_gameSet = true;
	}
}
