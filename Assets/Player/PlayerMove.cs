using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
	private enum SoundType
	{
		Sword1,
		Sword2,
		Sword3,
		SkillActivation,
		Curse,
		SkeletonNotHit,
		SkeletonHit,
	}

	private enum EffectType
	{
		SkillChage,
		SkillActivation,
		SkillActivationCircle,
	}

	private enum SwordType
	{
		NormalSword,
		AwakingSword,
	}



	[Header("�ړ��̑���"), SerializeField]
	private float m_speed;

	[SerializeField] const float NormalSpeed = 5;
	private const float SpeedUp = 7;

	[Header("�W�����v����u�Ԃ̑���"), SerializeField]
	private float m_jumpSpeed = 30.0f;

	[Header("�d�͉����x"), SerializeField]
	private float m_gravity = 15.0f;

	[Header("�������̑�������(Infinity�Ŗ�����)"), SerializeField]
	private float m_fallSpeed = 10.0f;

	[Header("�����̏���"), SerializeField]
	private float m_initFallSpeed = 2.0f;

	[Header("�J����"), SerializeField]
	private Camera m_targetCamera;

	[SerializeField] GameObject m_player;
	[SerializeField] GameObject m_playerDebuffEffect;
	[SerializeField] Collider m_collider;

	[SerializeField] AudioClip[] m_clip;

	[SerializeField] float m_chargeSkill; // �����܂ł̎���
	[SerializeField] const float MaxSkillCharge = 3f;
	[SerializeField] float m_skillActivation;   // ��������
	[SerializeField] const float MaxSkillActivation = 15f;

	[SerializeField] GameObject[] m_effect;
	[SerializeField] GameObject[] m_sword;
	[SerializeField] GameObject m_skillImage;
	[SerializeField] GameObject m_skillUi;

	[SerializeField] Slider m_slider;
	[SerializeField] int m_playerHeath;
	private bool m_isDeath;

	[SerializeField] GameObject m_boss;

	[SerializeField] GameObject m_playerLose;
 
	private Animator m_animator;
	private AudioSource audioSource;
	private Transform m_transform;
	private CharacterController m_characterController;
	private PlayerInput m_playerInput;

	private Vector2 m_inputMove;
	private float m_verticalVelocity;
	private float m_turnVelocity;
	private bool m_GroundedPrev;

	private bool m_canMove; // �v���C���[�𓮂�����邩
	private bool m_chargeAttack; // �X�L���`���[�W��
	private bool m_awakening;     // �X�L������

	private bool m_gameSet;

	private void Awake()
	{
		m_transform = transform;
		m_characterController = GetComponent<CharacterController>();
		m_playerInput = GetComponent<PlayerInput>();
		m_animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();

		if (m_targetCamera == null)
		{
			m_targetCamera = Camera.main;
		}

		m_chargeAttack = false;
		m_awakening = false;
		m_isDeath = false;
	}
	private void Start()
	{
		m_canMove = true;
		m_gameSet = false;
		NormalTime();
	}

	public void OnEnable()
	{
		m_playerInput.actions["Move"].performed += OnMove;
		m_playerInput.actions["Move"].canceled += OnMoveCancel;

		m_playerInput.actions["Attack"].performed += OnAttack;
		m_playerInput.actions["ChargeAttack"].performed += OnChargeAttack;
		m_playerInput.actions["ChargeAttack"].canceled += OnChargeAttackCansel;
	}

	private void OnDisable()
	{
		m_playerInput.actions["Move"].performed -= OnMove;
		m_playerInput.actions["Move"].canceled -= OnMoveCancel;

		m_playerInput.actions["ChargeAttack"].performed -= OnChargeAttack;
	}


	public void OnMove(InputAction.CallbackContext context)
	{
		// ���͒l�ɕێ����Ă���
		m_inputMove = context.ReadValue<Vector2>();
	}

	public void OnMoveCancel(InputAction.CallbackContext context)
	{
		//���͒l��ێ����Ă����@
		m_inputMove = context.ReadValue<Vector2>();
		m_animator.SetBool("Run", false);
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		if(!m_awakening)
		{
			m_animator.SetTrigger("Attack");
			m_canMove = false;
		}
		else
		{
			m_animator.SetTrigger("SkillSword");
			m_canMove = false;
		}
		
	}

	public void OnChargeAttack(InputAction.CallbackContext context)
	{
		// �X�L���`���[�W���܂��͓����Ă��Ȃ��Ƃ�
		if (m_awakening || !m_canMove || m_isDeath) return;

		m_chargeAttack = true;
		m_animator.SetBool("ChargeSkill", true);
		audioSource.Play();
		m_effect[(int)EffectType.SkillChage].SetActive(true);
	}

	public void OnChargeAttackCansel(InputAction.CallbackContext context)
	{
		m_chargeAttack = false;
		m_animator.SetBool("ChargeSkill", false); 
		audioSource.Stop();
		m_effect[(int)EffectType.SkillChage].SetActive(false);
	}


	public void ResetTrigger()
	{
		m_canMove = true;
		m_animator.ResetTrigger("Attack");
		m_animator.ResetTrigger("SkillSword");
	}

	public void SwordAudio1()
	{
		SoundEffect.Play2D(m_clip[(int)SoundType.Sword1]);
	}

	public void SwordAudio2()
	{
		SoundEffect.Play2D(m_clip[(int)SoundType.Sword2]);
	}

	public void SwordAudio3()
	{
		SoundEffect.Play2D(m_clip[(int)SoundType.Sword3]);
	}

	private void SkillActivation() // �X�L������
	{
		m_chargeAttack = false;
		m_animator.SetBool("ChargeSkill",false);
		m_skillUi.SetActive(false);
		m_skillImage.GetComponent<SkillTimer>().CoolDown(true);

		SoundEffect.Play2D(m_clip[(int)SoundType.SkillActivation]);
		

		m_speed = SpeedUp;

		m_effect[(int)EffectType.SkillChage].SetActive(false);
		m_effect[(int)EffectType.SkillActivation].SetActive(true);
		m_effect[(int)EffectType.SkillActivationCircle].SetActive(true);

		m_sword[(int)SwordType.NormalSword].SetActive(false);
		m_sword[(int)SwordType.AwakingSword].SetActive(true);
	}

	private void NormalTime()
	{
		m_speed = NormalSpeed;
		m_sword[(int)SwordType.NormalSword].SetActive(true);
		m_sword[(int)SwordType.AwakingSword].SetActive(false);

		m_effect[(int)EffectType.SkillActivationCircle].SetActive(false);
		m_skillImage.GetComponent<SkillTimer>().CoolDown(false);
	}

	private void FixedUpdate()
    {
		m_slider.value = m_playerHeath;

		if (m_gameSet) return;

		if (m_isDeath || m_playerHeath <= 0)
		{
			m_animator.SetBool("Death", true);
			OnDeath();
		}
		else
		{
			if (m_chargeAttack && !m_awakening) // �`���[�W�����܂��������Ă��Ȃ��Ƃ�
			{
				m_chargeSkill -= Time.deltaTime;

				if (m_chargeSkill <= 0)
				{
					m_awakening = true;
					SkillActivation();              // ����
					m_chargeSkill = MaxSkillCharge;
				}
			}
			else
			{
				m_chargeSkill = MaxSkillCharge;
			}

			if (m_awakening) // ������
			{
				m_skillActivation -= Time.deltaTime;

				if (m_skillActivation <= 0)
				{
					m_awakening = false;
					m_effect[(int)EffectType.SkillActivation].SetActive(false);
					NormalTime();
					m_skillActivation = MaxSkillActivation;
				}
			}

			if (!m_canMove || m_chargeAttack) return;

			var isGrounded = m_characterController.isGrounded;

			if (isGrounded && !m_GroundedPrev)
			{
				// ���n����u�Ԃɗ����̏������w�肵�Ă���
				m_verticalVelocity = -m_initFallSpeed;
			}
			else if (!isGrounded)
			{
				// �󒆂ɂ���Ƃ��́A�������ɏd�͉����x��^���ė���������
				m_verticalVelocity -= m_gravity * Time.deltaTime;

				// �������鑬���ȏ�ɂȂ�Ȃ��悤�ɕ␳
				if (m_verticalVelocity <= -m_fallSpeed)
				{
					m_verticalVelocity -= m_fallSpeed;
				}
			}

			m_GroundedPrev = isGrounded;

			// �J�����̌���(�p�x[deg])�擾
			var cameraAngleY = m_targetCamera.transform.eulerAngles.y;

			// ������͂Ɖ����������x����A���ݑ��x���v�Z
			var moveVelocity = new Vector3(
				m_inputMove.x * m_speed,
				m_verticalVelocity,
				m_inputMove.y * m_speed
			);

			// �J�����̊p�x���������ړ��ʂ���]
			moveVelocity = Quaternion.Euler(0, cameraAngleY, 0) * moveVelocity;

			// ���t���[���̈ړ��ʂ��ړ����x����v�Z
			var moveDelta = moveVelocity * Time.deltaTime;



			// CharactorController�Ɉړ��ʂ��w�肵�A�I�u�W�F�N�g�𓮂���
			m_characterController.Move(moveDelta);

			if (m_inputMove != Vector2.zero)
			{
				m_animator.SetBool("Run", true);
				// �ړ����͂�����ꍇ�́A�U�����������s��

				// ������͂���Y������̖ڕW�p�x[deg]���v�Z
				var targetAngleY = -Mathf.Atan2(m_inputMove.y, m_inputMove.x) * Mathf.Rad2Deg + 90;

				// �J�����̊p�x�������U������p�x��␳
				targetAngleY += cameraAngleY;

				// �C�[�W���O���Ȃ��玟�̉�]���x[deg]���v�Z
				var angleY = Mathf.SmoothDampAngle(
					m_transform.eulerAngles.y,
					targetAngleY,
					ref m_turnVelocity,
					0.1f
				);

				// �I�u�W�F�N�g�̉�]���X�V
				m_transform.rotation = Quaternion.Euler(0, angleY, 0);
			}
		}
	}

	// �G����_���[�W��H�炤

	public void HitDamage(int hit) // MagicAttack
	{
		m_playerHeath -= hit;

		if (m_playerHeath <= 0)
		{
			m_isDeath = true;
			m_animator.SetBool("Death", true);
			m_boss.GetComponent<BossMove>().GameSet(true);
			OnDeath();
		}
	}

	public void HitSkeletonAttack(int skeletonAttack) // �[��
	{
		if (m_playerHeath <= 0)
		{
			m_isDeath = true;
			m_animator.SetBool("Death", true);
			m_boss.GetComponent<BossMove>().GameSet(true);
			OnDeath();
		}

		if (!m_awakening)
		{
			m_playerHeath -= skeletonAttack;
			SoundEffect.Play2D(m_clip[(int)SoundType.SkeletonHit]);
		}
		else
		{
			SoundEffect.Play2D(m_clip[(int)SoundType.SkeletonNotHit]);
		}
	}

	public void HitCruseAttack(float cruseAttack) // �ŃG���A
	{
		if (m_awakening)
		{
			m_speed = NormalSpeed;
		}
		else
		{
			m_speed = cruseAttack;
		}
		m_playerDebuffEffect.SetActive(true);
	}

	public void HitCruseAttackSound()
	{
		SoundEffect.Play2D(m_clip[(int)SoundType.Curse]);
	}

	public void HitCruseAttackExit()
	{
		if (m_awakening)
		{
			m_speed = SpeedUp;
		}
		else
		{
			m_speed = NormalSpeed;
		}
		m_playerDebuffEffect.SetActive(false);
	}
	public void GameSet()
	{
		m_gameSet = true;
		m_collider.enabled = false;
	}

	private void OnDeath() // �v���C���[�����񂾂�
	{
		if (m_gameSet) return;
		m_playerLose.GetComponent<GameSetLose>().PlayerLose(true);
		m_playerDebuffEffect.SetActive(false);
	}
}
