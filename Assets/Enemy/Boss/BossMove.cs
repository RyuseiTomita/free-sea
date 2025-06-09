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
	private const int MaxMagicNumber = 6;
	private GameObject[] m_magicAttackEffect = new GameObject[10];

	//�@�[���̏���
	[SerializeField] GameObject m_skelton;
	[SerializeField] Transform[] m_skeltonpos;   // �[���̃|�W�V����
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


	// �U���p�^�[��
	[SerializeField] int m_bossAttackPattern = 0; 
	private const float MagicAttackTime = 3f;
	private const float SkeletonTime = 5f;
	private const float CurseTime = 3f;

	// �o�����[�h
	[SerializeField] GameObject[] m_grave; // ��̐�
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
		//m_skeltonSpawn = false;
		//m_isMove = false;

		m_graveCount = 0;
	}


	void FixedUpdate()
	{
		slider.value = m_bossHealth;

		if (m_isDeath || m_canShield) return;

		if (m_idleTime <= 0)
		{
			m_onMove = false;

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

		if (m_bossHealth <= 100 && !m_isShield) // HP��������؂�����o�����[�h
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
		}
	}


	private void OnMagicAttackTime()
	{
		m_idleTime = SkeletonTime;
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
		for (int i = 0; i < m_skeltonpos.Length; i++)
		{
			Instantiate(m_effect[2], m_skeltonpos[i].transform.position, Quaternion.identity);
		}
		SoundEffect.Play2D(m_clip[6]);

		m_idleTime = CurseTime;
		
	}

	public void SummonMob() //�[���X�|�[��
	{
		m_onMove = true;
		m_onAttack = false;

		for (int i = 0; i < m_skeltonpos.Length; i++)
		{
			m_skelton.GetComponent<SkeletonMove>().SetPlayer(m_player);
			Instantiate(m_skelton, m_skeltonpos[i].transform.position, Quaternion.identity);
		}
		SoundEffect.Play2D(m_clip[3]);

		m_bossAttackPattern++;
	}


	private void CurseAttackAnimation() // �􂢂̍U���A�j���[�V����
	{
		m_animator.SetTrigger("Curse");
		m_bossAttackPattern++;
		m_idleTime = MagicAttackTime;
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
						Instantiate(m_skeltonHead, m_cursePos[i].transform.position, Quaternion.identity);
					}
					break;
				}
			case 1:
				{
					for (int i = 0; i < m_cursePos.Length; i++)
					{
						m_skeltonHead.GetComponent<CurseSkeletonHead>().SetPlayer(m_lookPlayer);
						Instantiate(m_skeltonHead, m_cursePos2[i].transform.position, Quaternion.identity);
					}
					break;
				}
		}
		SoundEffect.Play2D(m_clip[5]);
		m_bossAttackPattern = 0;
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

	public void ShieldSound()
	{
		SoundEffect.Play2D(m_clip[8]);
	}

	private void ShieldAnimation()
	{
		for(int i = 0; i < 4; i++)
		{
			m_grave[i].SetActive(true);
		}

		m_shield.SetActive(true);
		SoundEffect.Play2D(m_clip[9]);
		m_collider.enabled = false;
		gameManager.GetComponent<GameManager>().BgmChange();
	}

	public void ShieldAnimationEnd()
	{
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
}
