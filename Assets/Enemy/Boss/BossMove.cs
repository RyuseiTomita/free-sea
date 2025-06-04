using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class BossMove : MonoBehaviour
{
	[SerializeField] Transform m_player; // �v���C���[�ɒǏ]
	[SerializeField] GameObject[] m_effect;
	[SerializeField] AudioClip[] m_clip;

	// ���@�U��
	[SerializeField] float m_magicAttackTime;
	private const float MaxMagicAttackTime = 60f;
	private bool m_magicAttack;
	[SerializeField] float m_magicCoolDown; // �Ԋu

	// �e�̐�
	[SerializeField] int m_magicNumber; // ���@�w�̏���
	private int m_magicNumberBomb;      // ���e�̏���
	private const int MaxMagicNumber = 5;
	private GameObject[] m_magicAttackEffect = new GameObject[10];

	//�@�[���̏���
	[SerializeField] GameObject m_skelton;
	[SerializeField] Transform[] m_skeltonpos;   // �[���̃|�W�V����
	//private bool m_skeltonSpawn;

	// �􂢂̍U��
	[SerializeField] GameObject[] m_skeltonHead; // �[���̓�
	[SerializeField] GameObject m_curseEffect;
	[SerializeField] Transform[] m_cursePos;

	[SerializeField] float m_idleTime; // �������Ȃ�����

	private int m_bossAttackPattern = 0; // �U���p�^�[��

	//private bool m_isMove; // �G�������Ă��邩


	private Animator m_animator;

	void Start()
	{
		m_magicNumber = 0;
		m_magicNumberBomb = 0;

		m_magicCoolDown = 0;

		m_animator = GetComponent<Animator>();
		m_magicAttack = false;
		//m_skeltonSpawn = false;
		//m_isMove = false;
	}


	void FixedUpdate()
	{
		// �v���C���[�Ɍ���
		transform.rotation = Quaternion.Lerp(
			transform.rotation,
			Quaternion.LookRotation(m_player.position - transform.position), 0.2f);

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
	}

	private void OnMagicAttackTime()
	{
		m_magicAttackTime -= Time.deltaTime;

		if (m_magicAttackTime <= 0)
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
		if (m_magicNumber >= MaxMagicNumber && m_magicAttack) // �}�W�b�N�U�����I�������
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
	}
	private IEnumerator MagicAttackBomb()
	{
		yield return new WaitForSeconds(3);
		Instantiate(m_effect[1], m_magicAttackEffect[m_magicNumberBomb].transform.position, Quaternion.Euler(-90, 0, 0));
		SoundEffect.Play2D(m_clip[2]);
		m_magicNumberBomb++;
	}

	private void SkeltonSpawnAnimation() // �X�P���g���X�|�[���̃A�j���[�V����
	{
		m_animator.SetTrigger("SkeltonSpawn");
		m_bossAttackPattern++;
		m_idleTime = 10f;
	}

	public void SummonMob() //�[���X�|�[��
	{
		for (int i = 0; i < m_skeltonpos.Length; i++)
		{
			Instantiate(m_skelton, m_skeltonpos[i].transform.position, Quaternion.identity);
		}
		SoundEffect.Play2D(m_clip[3]);
	}


	private void CurseAttackAnimation() // �􂢂̍U���A�j���[�V����
	{
		m_animator.SetTrigger("Curse");
		m_bossAttackPattern++;
	}

	public void CurseAttack() // �􂢂̍U��
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

		for(int i = 0; i < m_skeltonHead.Length; i++)
		{
			m_skeltonHead[i].SetActive(true);
		}
		SoundEffect.Play2D(m_clip[5]);
	}
}
