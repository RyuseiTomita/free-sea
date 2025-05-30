using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
	[Header("�ړ��̑���"), SerializeField]
	private float m_speed = 6.0f;

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

	[SerializeField] AudioClip[] m_clip;

	[SerializeField] float m_chargeSkill;
	[SerializeField] const float MaxSkillCharge = 3f;

	private Animator m_animator;
	private Transform m_transform;
	private CharacterController m_characterController;
	private PlayerInput m_playerInput;

	private Vector2 m_inputMove;
	private float m_verticalVelocity;
	private float m_turnVelocity;
	private bool m_GroundedPrev;

	private bool m_canMove; // �v���C���[�𓮂�����邩
	private bool m_chargeAttack; // �X�L���`���[�W��
	private bool m_awakening;	 // �X�L������

	private void Awake()
	{
		m_transform = transform;
		m_characterController = GetComponent<CharacterController>();
		m_playerInput = GetComponent<PlayerInput>();
		m_animator = GetComponent<Animator>();

		if (m_targetCamera == null)
		{
			m_targetCamera = Camera.main;
		}

		m_chargeAttack = false;
		m_awakening = false;
	}
	private void Start()
	{
		m_canMove = true;
	}

	public void OnEnable()
	{
		m_playerInput.actions["Move"].performed += OnMove;
		m_playerInput.actions["Move"].canceled += OnMoveCancel;

		m_playerInput.actions["Attack"].performed += OnAttack;
		m_playerInput.actions["ChargeAttack"].performed += OnChargeAttack;
		m_playerInput.actions["ChargeAttack"].canceled += OnChargeAttackCansel;

		//m_playerInput.actions["Attack"].canceled += OnAttackCancel;

		//playerInput.actions["Jump"].performed += OnJump;
	}

	private void OnDisable()
	{
		m_playerInput.actions["Move"].performed -= OnMove;
		m_playerInput.actions["Move"].canceled -= OnMoveCancel;

		m_playerInput.actions["ChargeAttack"].performed -= OnChargeAttack;
		//m_playerInput.actions["Attack"].performed -= OnAttack;


		//m_playerInput.actions["Attack"].canceled -= OnAttackCancel;

		//playerInput.actions["Jump"].performed -= OnJump;
	}


	public void OnMove(InputAction.CallbackContext context)
	{
		if (!m_canMove) return;

		// ���͒l�ɕێ����Ă���
		m_inputMove = context.ReadValue<Vector2>();
		m_animator.SetBool("Run", true);
	}

	public void OnMoveCancel(InputAction.CallbackContext context)
	{
		//���͒l��ێ����Ă����@
		m_inputMove = context.ReadValue<Vector2>();
		m_animator.SetBool("Run", false);
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		// �{�^���������ꂽ�u�Ԃ����n���Ă��鎞��������
		if (!context.performed || !m_characterController.isGrounded) return;

		// ����������ɑ��x��^����
		m_verticalVelocity = m_jumpSpeed;
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		m_animator.SetTrigger("Attack");
		m_canMove = false;
	}

	public void OnChargeAttack(InputAction.CallbackContext context)
	{
		// �X�L���`���[�W���������Ă��Ȃ��Ƃ�
		if (m_awakening || !m_canMove) return;

		m_chargeAttack = true;
		m_animator.SetBool("ChargeSkill", true);
		SoundEffect.Play2D(m_clip[3]);
	}

	public void OnChargeAttackCansel(InputAction.CallbackContext context)
	{
		m_chargeAttack = false;
		m_animator.SetBool("ChargeSkill", false);
	}


	public void ResetTrigger()
	{
		m_canMove = true;
		m_animator.ResetTrigger("Attack");
	}

	public void SwordAudio1()
	{
		SoundEffect.Play2D(m_clip[0]);
	}

	public void SwordAudio2()
	{
		SoundEffect.Play2D(m_clip[1]);
	}

	public void SwordAudio3()
	{
		SoundEffect.Play2D(m_clip[2]);
	}

	private void SkillActivation() // �X�L������
	{
		float speedUp = 3f;

		SoundEffect.Play2D(m_clip[4]);

		m_speed += speedUp;
	}

	private void FixedUpdate()
    {
		if(m_chargeAttack && !m_awakening)
		{
			m_chargeSkill -= Time.deltaTime;

			if(m_chargeSkill <= 0)
			{
				m_awakening = true;
				SkillActivation();
				m_chargeSkill = MaxSkillCharge;
			}
		}
		else
		{
			m_chargeSkill = MaxSkillCharge;
		}

		if (!m_canMove || m_chargeAttack) return;

			var isGrounded = m_characterController.isGrounded;

		if (isGrounded && !m_GroundedPrev)
		{
			// ���n����u�Ԃɗ����̏������w�肵�Ă���
			m_verticalVelocity = -m_initFallSpeed;
		}
		else if(!isGrounded)
		{
			// �󒆂ɂ���Ƃ��́A�������ɏd�͉����x��^���ė���������
			m_verticalVelocity -= m_gravity * Time.deltaTime;

			// �������鑬���ȏ�ɂȂ�Ȃ��悤�ɕ␳
			if(m_verticalVelocity <= -m_fallSpeed)
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

		if(m_inputMove != Vector2.zero)
		{
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
