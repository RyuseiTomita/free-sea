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

	[SerializeField] int m_playerHeath;
	[SerializeField] Slider m_playerHp;


	[Header("移動の速さ"), SerializeField]
	private float m_speed;

	[SerializeField] const float NormalSpeed = 5;
	private const float SpeedUp = 7;

	[Header("カメラ"), SerializeField]
	private Camera m_targetCamera;

	[SerializeField] GameObject m_player;
	[SerializeField] GameObject m_playerDebuffEffect;
	[SerializeField] Collider m_collider;

	[SerializeField] AudioClip[] m_clip;

	// スキル
	[SerializeField] float m_chargeSkill; // 発動までの時間
	[SerializeField] const float MaxSkillCharge = 3f;
	[SerializeField] float m_skillActivation;   // 発動時間
	[SerializeField] const float MaxSkillActivation = 15f;

	[SerializeField] GameObject[] m_effect;
	[SerializeField] GameObject[] m_sword;
	[SerializeField] GameObject m_skillImage;
	[SerializeField] GameObject m_skillUi;


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
		m_effect[(int)EffectType.SkillChage].SetActive(m_chargeAttack);
	}

	public void OnChargeAttackCansel(InputAction.CallbackContext context)
	{
		m_chargeAttack = false;
		m_animator.SetBool("ChargeSkill", false); 
		audioSource.Stop();
		m_effect[(int)EffectType.SkillChage].SetActive(m_chargeAttack);
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

		// スキル発動中のEffectを生成
		m_effect[(int)EffectType.SkillChage].SetActive(m_awakening);
		m_effect[(int)EffectType.SkillActivation].SetActive(m_awakening);
		m_effect[(int)EffectType.SkillActivationCircle].SetActive(m_awakening);

		// 剣を切り替える
		m_sword[(int)SwordType.NormalSword].SetActive(false);
		m_sword[(int)SwordType.AwakingSword].SetActive(m_awakening);
	}

	private void NormalTime() // 通常時
	{
		m_speed = NormalSpeed;
		m_sword[(int)SwordType.NormalSword].SetActive(true);
		m_sword[(int)SwordType.AwakingSword].SetActive(m_awakening);

		m_effect[(int)EffectType.SkillActivationCircle].SetActive(m_awakening);
		m_skillImage.GetComponent<SkillTimer>().CoolDown(false);
	}

	private void FixedUpdate()
	{
		m_playerHp.value = m_playerHeath;

		// プレイヤーが死んだら
		if (m_playerHeath <= 0)
		{
			m_isDeath = true;
			m_animator.SetBool("Death", m_isDeath);
			m_boss.GetComponent<BossMove>().GameSet(true);
			OnDeath();
		}

		if (m_gameSet || m_playerHeath <= 0) return;

		// チャージ中
		if (m_chargeAttack) 
		{
			if (m_playerHeath <= 0) return;

			m_chargeSkill -= Time.deltaTime;

			// チャージ時間が一定時間以上を満たすと覚醒
			if (m_chargeSkill <= 0) 
			{
				m_awakening = true;

				// 発動
				SkillActivation();   
				

				m_chargeSkill = MaxSkillCharge;
			}
		}
		else
		{
			m_chargeSkill = MaxSkillCharge;
		}

		// 発動中
		if (m_awakening) 
		{
			m_skillActivation -= Time.deltaTime;

			if (m_skillActivation <= 0)
			{
				m_effect[(int)EffectType.SkillActivation].SetActive(false);
				m_awakening = false;
				NormalTime();
				m_skillActivation = MaxSkillActivation;
			}
		}

		if (!m_canMove || m_chargeAttack) return;

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

	// 敵からダメージを食らう

	public void HitDamage(int hit) 
	{
		m_playerHeath -= hit;
	}

	public void HitSkeletonAttack(int skeletonAttack) // 骸骨
	{
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

		// 覚醒Effectを消す
		m_effect[(int)EffectType.SkillActivation].SetActive(false);
		m_effect[(int)EffectType.SkillActivationCircle].SetActive(false);
	}
}
