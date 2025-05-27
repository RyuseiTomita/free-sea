using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
	[Header("移動の速さ"), SerializeField]
	private float m_speed = 6.0f;

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

	private Transform m_transform;
	private CharacterController m_characterController;
	private PlayerInput playerInput;

	private Vector2 m_inputMove;
	private float m_verticalVelocity;
	private float m_turnVelocity;
	private bool m_GroundedPrev;

	private void Awake()
	{
		m_transform = transform;
		m_characterController = GetComponent<CharacterController>();
		playerInput = GetComponent<PlayerInput>();

		if(m_targetCamera == null)
		{
			m_targetCamera = Camera.main;
		}
	}

	public void OnEnable()
	{
		playerInput.actions["Move"].performed += OnMove;
		playerInput.actions["Move"].performed += OnMoveCancel;

		//playerInput.actions["Jump"].performed += OnJump;
	}

	private void OnDisable()
	{
		playerInput.actions["Move"].performed -= OnMove;
		playerInput.actions["Move"].performed -= OnMoveCancel;

		//playerInput.actions["Jump"].performed -= OnJump;
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
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		// ボタンが押された瞬間かつ着地している時だけ処理
		if (!context.performed || !m_characterController.isGrounded) return;

		// 鉛直上向きに速度を与える
		m_verticalVelocity = m_jumpSpeed;
	}

	private void FixedUpdate()
    {
        var isGrounded = m_characterController.isGrounded;

		if (isGrounded && !m_GroundedPrev)
		{
			// 着地する瞬間に落下の初速を指定しておく
			m_verticalVelocity = -m_initFallSpeed;
		}
		else if(!isGrounded)
		{
			// 空中にいるときは、下向きに重力加速度を与えて落下させる
			m_verticalVelocity -= m_gravity * Time.deltaTime;

			// 落下する速さ以上にならないように補正
			if(m_verticalVelocity <= -m_fallSpeed)
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

		if(m_inputMove != Vector2.zero)
		{
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
