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



	[Header("移動の速さ"), SerializeField]
	private float m_speed;

	[SerializeField] const float NormalSpeed = 5;
	private const float SpeedUp = 7;

	[Header("ジャンプする瞬間の速さ"), SerializeField]
	private float m_jumpSpeed = 30.0f;

	[Header("重力加速度"), SerializeField]
	private float m_gravity = 15.0f;

	[Header("落下時の速さ制限(Infinityで無制限)"), SerializeField]
	private float m_fallSpeed = 10.0f;

	[Header("落下の初速"), SerializeField]
	private float m_initFallSpeed = 2.0f;

	[Header("カメラ"), SerializeField]
	private Camera m_targetCamera;

	[SerializeField] GameObject m_player;
	[SerializeField] GameObject m_playerDebuffEffect;
	[SerializeField] Collider m_collider;

	[SerializeField] AudioClip[] m_clip;

	[SerializeField] float m_chargeSkill; // 発動までの時間
	[SerializeField] const float MaxSkillCharge = 3f;
	[SerializeField] float m_skillActivation;   // 発動時間
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

	private bool m_canMove; // プレイヤーを動かせれるか
	private bool m_chargeAttack; // スキルチャージ中
	private bool m_awakening;     // スキル発動

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
		// 入力値に保持しておく
		m_inputMove = context.ReadValue<Vector2>();
	}

	public void OnMoveCancel(InputAction.CallbackContext context)
	{
		//入力値を保持しておく　
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
		// スキルチャージ中または動いていないとき
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

	private void SkillActivation() // スキル発動
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
			if (m_chargeAttack && !m_awakening) // チャージ中かつまだ発動していないとき
			{
				m_chargeSkill -= Time.deltaTime;

				if (m_chargeSkill <= 0)
				{
					m_awakening = true;
					SkillActivation();              // 発動
					m_chargeSkill = MaxSkillCharge;
				}
			}
			else
			{
				m_chargeSkill = MaxSkillCharge;
			}

			if (m_awakening) // 発動中
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
				// 着地する瞬間に落下の初速を指定しておく
				m_verticalVelocity = -m_initFallSpeed;
			}
			else if (!isGrounded)
			{
				// 空中にいるときは、下向きに重力加速度を与えて落下させる
				m_verticalVelocity -= m_gravity * Time.deltaTime;

				// 落下する速さ以上にならないように補正
				if (m_verticalVelocity <= -m_fallSpeed)
				{
					m_verticalVelocity -= m_fallSpeed;
				}
			}

			m_GroundedPrev = isGrounded;

			// カメラの向き(角度[deg])取得
			var cameraAngleY = m_targetCamera.transform.eulerAngles.y;

			// 操作入力と鉛直方向速度から、現在速度を計算
			var moveVelocity = new Vector3(
				m_inputMove.x * m_speed,
				m_verticalVelocity,
				m_inputMove.y * m_speed
			);

			// カメラの角度部分だけ移動量を回転
			moveVelocity = Quaternion.Euler(0, cameraAngleY, 0) * moveVelocity;

			// 現フレームの移動量を移動速度から計算
			var moveDelta = moveVelocity * Time.deltaTime;



			// CharactorControllerに移動量を指定し、オブジェクトを動かす
			m_characterController.Move(moveDelta);

			if (m_inputMove != Vector2.zero)
			{
				m_animator.SetBool("Run", true);
				// 移動入力がある場合は、振り向き動作も行う

				// 操作入力からY軸周りの目標角度[deg]を計算
				var targetAngleY = -Mathf.Atan2(m_inputMove.y, m_inputMove.x) * Mathf.Rad2Deg + 90;

				// カメラの角度分だけ振り向く角度を補正
				targetAngleY += cameraAngleY;

				// イージングしながら次の回転速度[deg]を計算
				var angleY = Mathf.SmoothDampAngle(
					m_transform.eulerAngles.y,
					targetAngleY,
					ref m_turnVelocity,
					0.1f
				);

				// オブジェクトの回転を更新
				m_transform.rotation = Quaternion.Euler(0, angleY, 0);
			}
		}
	}

	// 敵からダメージを食らう

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

	public void HitSkeletonAttack(int skeletonAttack) // 骸骨
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

	public void HitCruseAttack(float cruseAttack) // 毒エリア
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

	private void OnDeath() // プレイヤーが死んだら
	{
		if (m_gameSet) return;
		m_playerLose.GetComponent<GameSetLose>().PlayerLose(true);
		m_playerDebuffEffect.SetActive(false);
	}
}
